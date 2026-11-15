using Moq;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests;

public class WorkItemServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsWorkItemAndReturnsItsData()
    {
        var repository = new Mock<IWorkItemRepository>();
        WorkItem? addedWorkItem = null;
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var dueDate = new DateTime(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new WorkItemRequest
        {
            Title = "  Prepare release  ",
            Description = "Complete release checklist",
            Asignee = "  Alex  ",
            DueDate = dueDate
        };

        repository
            .Setup(r => r.AddAsync(It.IsAny<WorkItem>(), cancellationToken))
            .Callback<WorkItem, CancellationToken>((workItem, _) => addedWorkItem = workItem)
            .Returns(Task.CompletedTask);

        var service = new CreateWorkItemService(repository.Object);

        var response = await service.CreateAsync(request, cancellationToken);

        Assert.NotNull(addedWorkItem);
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(response.Id, addedWorkItem.Id);
        Assert.Equal("Prepare release", addedWorkItem.Title);
        Assert.Equal("Alex", addedWorkItem.Asignee);
        Assert.Equal(request.Description, addedWorkItem.Description);
        Assert.Equal(WorkItemStatus.Pending, addedWorkItem.WorkItemStatus);
        Assert.Equal(dueDate, addedWorkItem.DueDate);
        Assert.NotEqual(default, addedWorkItem.CreatedAt);
        Assert.Equal(addedWorkItem.Id, response.Id);
        Assert.Equal(addedWorkItem.Title, response.Title);
        Assert.Equal(addedWorkItem.Description, response.Description);
        Assert.Equal(addedWorkItem.WorkItemStatus, response.Status);
        Assert.Equal(addedWorkItem.Asignee, response.Asignee);
        Assert.Equal(addedWorkItem.DueDate, response.DueDate);
        Assert.Equal(addedWorkItem.CreatedAt, response.CreatedAt);
        Assert.Equal(addedWorkItem.UpdatedAt, response.UpdatedAt);

        repository.Verify(
            r => r.AddAsync(It.IsAny<WorkItem>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingWorkItemAndReturnsItsData()
    {
        var repository = new Mock<IWorkItemRepository>();
        var workItemId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var workItem = new WorkItem
        {
            Id = workItemId,
            Title = "Old title",
            Description = "Old description",
            WorkItemStatus = WorkItemStatus.Pending,
            Asignee = "Alex",
            DueDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAt = createdAt
        };
        var request = new WorkItemRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            WorkItemStatus = WorkItemStatus.InProcess,
            Asignee = "Jordan",
            DueDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        WorkItem? updatedWorkItem = null;
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        repository
            .Setup(r => r.GetWorkItemByIdAsync(workItemId, cancellationToken))
            .ReturnsAsync(workItem);
        repository
            .Setup(r => r.UpdateAsync(workItem, cancellationToken))
            .Callback<WorkItem, CancellationToken>((item, _) => updatedWorkItem = item)
            .Returns(Task.CompletedTask);

        var service = new UpdateWorkItemService(repository.Object);

        var response = await service.UpdateAsync(workItemId, request, cancellationToken);

        var persistedWorkItem = Assert.IsType<WorkItem>(updatedWorkItem);
        Assert.Same(workItem, persistedWorkItem);
        Assert.Equal(workItemId, response.Id);
        Assert.Equal(request.Title, persistedWorkItem.Title);
        Assert.Equal(request.Description, persistedWorkItem.Description);
        Assert.Equal(request.WorkItemStatus, persistedWorkItem.WorkItemStatus);
        Assert.Equal(request.Asignee, persistedWorkItem.Asignee);
        Assert.Equal(request.DueDate, persistedWorkItem.DueDate);
        Assert.Equal(createdAt, persistedWorkItem.CreatedAt);
        Assert.NotNull(persistedWorkItem.UpdatedAt);
        Assert.Equal(persistedWorkItem.Id, response.Id);
        Assert.Equal(persistedWorkItem.Title, response.Title);
        Assert.Equal(persistedWorkItem.Description, response.Description);
        Assert.Equal(persistedWorkItem.WorkItemStatus, response.Status);
        Assert.Equal(persistedWorkItem.Asignee, response.Asignee);
        Assert.Equal(persistedWorkItem.DueDate, response.DueDate);
        Assert.Equal(persistedWorkItem.CreatedAt, response.CreatedAt);
        Assert.Equal(persistedWorkItem.UpdatedAt, response.UpdatedAt);

        repository.Verify(
            r => r.GetWorkItemByIdAsync(workItemId, cancellationToken),
            Times.Once);
        repository.Verify(
            r => r.UpdateAsync(workItem, cancellationToken),
            Times.Once);
    }
}

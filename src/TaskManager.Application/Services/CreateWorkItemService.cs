using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public class CreateWorkItemService
{
    private readonly IWorkItemRepository _workItemRepository;
    public CreateWorkItemService(IWorkItemRepository workItemRepository)
    {
        _workItemRepository = workItemRepository;
    }

    public async Task<WorkItemResponse> CreateAsync(CreateWorkItemRequest request, CancellationToken cancellationToken= default)
    {
        var workItem = new WorkItem
        {
            Id = new Guid(),
            Title = request.Title.Trim(),
            Description = request.Description,
            WorkItemStatus = WorkItemStatus.Pending,
            Asignee = request.Asignee.Trim(),
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        await _workItemRepository.AddAsync(workItem, cancellationToken);

        return new WorkItemResponse
        {
            Id = workItem.Id,
            Title = workItem.Title,
            Description = workItem.Description,
            Status = workItem.WorkItemStatus,
            Asignee = workItem.Asignee,
            DueDate = workItem.DueDate,
            CreatedAt = workItem.CreatedAt,
            UpdatedAt = workItem.UpdatedAt
        };
    }
}

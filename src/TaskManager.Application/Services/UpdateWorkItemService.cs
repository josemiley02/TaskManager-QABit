using System;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services;

public class UpdateWorkItemService
{
    private readonly IWorkItemRepository _repository;
    public UpdateWorkItemService(IWorkItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkItemResponse> UpdateAsync(Guid id, WorkItemRequest request, CancellationToken cancellationToken = default)
    {
        var workItem = await _repository.GetWorkItemByIdAsync(id, cancellationToken);
        if(workItem is null)
            return null!;
        workItem.Title = request.Title;
        workItem.Description = request.Description;
        workItem.Asignee = request.Asignee;
        workItem.WorkItemStatus = request.WorkItemStatus;
        workItem.DueDate = request.DueDate;
        workItem.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(workItem, cancellationToken);

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

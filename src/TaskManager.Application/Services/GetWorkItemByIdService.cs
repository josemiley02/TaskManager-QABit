using System;
using System.Data;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class GetWorkItemByIdService
{
    private readonly IWorkItemRepository _repository;
    public GetWorkItemByIdService(IWorkItemRepository repository)
    {
        _repository = repository;
    }
    public async Task<WorkItemResponse?> GetItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetWorkItemByIdAsync(id);
        if (item is null)
            return null;
        
        return new WorkItemResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Status = item.WorkItemStatus,
            Asignee = item.Asignee,
            DueDate = item.DueDate,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}

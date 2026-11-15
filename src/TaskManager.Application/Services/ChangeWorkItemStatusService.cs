using System;
using System.Diagnostics;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public class ChangeWorkItemStatusService
{
    private readonly IWorkItemRepository _repository;
    public ChangeWorkItemStatusService(IWorkItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id, WorkItemStatus status, CancellationToken cancellationToken = default)
    {
        var workItem = await _repository.GetWorkItemByIdAsync(id);
        if (workItem is null)
            return false;
        workItem.WorkItemStatus = status;

        await _repository.UpdateAsync(workItem, cancellationToken);

        return true;
    }
}

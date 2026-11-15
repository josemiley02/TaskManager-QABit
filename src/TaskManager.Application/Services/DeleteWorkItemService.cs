using System;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services;

public class DeleteWorkItemService
{
    private readonly IWorkItemRepository _repository;

    public DeleteWorkItemService(IWorkItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> DeleteWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteWorkItemAsync(id, cancellationToken);
    }
}

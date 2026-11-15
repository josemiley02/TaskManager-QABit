using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface IWorkItemRepository
{
    Task AddAsync(WorkItem work, CancellationToken cancellationToken = default);
}

using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Interfaces;

public interface IWorkItemRepository
{
    Task AddAsync(WorkItem work, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<WorkItem> items, int totalCount)> GetPagedAsync(WorkItemStatus? status, string asignee, int page, int pageSize, CancellationToken cancellationToken= default);
}

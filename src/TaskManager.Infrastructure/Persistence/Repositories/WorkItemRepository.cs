using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly TaskManagerDbContext _dbContext;
    public WorkItemRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(WorkItem work, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(work, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateAsync(WorkItem work, CancellationToken cancellationToken = default)
    {
        _dbContext.Update(work);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<(IReadOnlyList<WorkItem> items, int totalCount)> GetPagedAsync(WorkItemStatus? status, string asignee, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkItems.AsNoTracking();
        if (status.HasValue)
        {
            query = query.Where(w => w.WorkItemStatus == status);
        }
        if (!string.IsNullOrWhiteSpace(asignee))
        {
            query = query.Where(w =>
                EF.Functions.ILike(w.Asignee, $"%{asignee.Trim()}%")
            );
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    public async Task<WorkItem?> GetWorkItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems.FirstOrDefaultAsync(w => w.Id==id, cancellationToken);
    }
    public async Task<bool> DeleteWorkItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.WorkItems.FirstOrDefaultAsync(w => w.Id==id, cancellationToken);
        if (item is null)
            return false;
        
        _dbContext.WorkItems.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

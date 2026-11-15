using System;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

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
}

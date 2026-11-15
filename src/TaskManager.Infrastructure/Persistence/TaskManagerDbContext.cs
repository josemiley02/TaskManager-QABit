using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
    }
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);
    }
}

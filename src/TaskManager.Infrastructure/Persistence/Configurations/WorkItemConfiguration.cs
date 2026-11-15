using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(w => w.Description)
            .HasMaxLength(1500);
        
        builder.Property(w => w.WorkItemStatus)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(w => w.Asignee)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(w => w.DueDate)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.UpdatedAt);

        builder.HasIndex(w => w.Asignee);

        builder.HasIndex(w => w.WorkItemStatus);
    }
}

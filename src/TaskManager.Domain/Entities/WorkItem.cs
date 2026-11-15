using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class WorkItem
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public WorkItemStatus WorkItemStatus {get; set;} = WorkItemStatus.Pending;
    public string Asignee {get; set;} = string.Empty;
    public DateTime DueDate {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set;}
}

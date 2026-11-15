using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.WorkItems;

public class WorkItemRequest
{
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public WorkItemStatus WorkItemStatus {get; set;} = WorkItemStatus.Pending;
    public string Asignee {get; set;} = string.Empty;
    public DateTime DueDate {get; set;}
}

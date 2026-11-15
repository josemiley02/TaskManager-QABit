using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.WorkItems;

public class GetWorkItemsRequest
{
    public WorkItemStatus? Status {get; set;}
    public string? Asignee {get; set;}
    public int Pages {get; set;} = 1;
    public int PerPages {get; set;} = 10;
}

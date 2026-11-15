namespace TaskManager.Application.DTOs.WorkItems;

public class CreateWorkItemRequest
{
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string Asignee {get; set;} = string.Empty;
    public DateTime DueDate {get; set;}
}

using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.WorkItems;

public class WorkItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkItemStatus Status { get; set; }
    public string Asignee { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WorkItemResponseList
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkItemStatus Status { get; set; }
    public string Asignee { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}
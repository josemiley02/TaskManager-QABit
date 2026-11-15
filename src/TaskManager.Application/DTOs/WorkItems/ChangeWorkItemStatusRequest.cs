using System;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.WorkItems;

public class ChangeWorkItemStatusRequest
{
    public WorkItemStatus WorkItemStatus {get; set;}
}

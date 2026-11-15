namespace TaskManager.Application.Services;

internal static class WorkItemDateTime
{
    public static DateTime NormalizeDueDateToUtc(DateTime dueDate) => dueDate.Kind switch
    {
        DateTimeKind.Utc => dueDate,
        DateTimeKind.Local => dueDate.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dueDate, DateTimeKind.Utc)
    };
}

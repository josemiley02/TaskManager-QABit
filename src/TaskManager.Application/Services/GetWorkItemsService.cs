using System;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services;

public class GetWorkItemsService
{
    private readonly IWorkItemRepository _repository;
    public GetWorkItemsService(IWorkItemRepository repository)
    {
        _repository = repository;
    }
    public async Task<PagedResponse<WorkItemResponseList>> ExecuteAsync(GetWorkItemsRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Status,
            request.Asignee!,
            request.Pages,
            request.PerPages,
            cancellationToken
        );
        var responseItems = items.Select(w => new WorkItemResponseList
        {
            Id = w.Id,
            Title = w.Title,
            Description = w.Description,
            Status = w.WorkItemStatus,
            Asignee = w.Asignee,
            DueDate = w.DueDate
        });

        return new PagedResponse<WorkItemResponseList>
        {
            Items = responseItems,
            Page = request.Pages,
            PageSize = request.PerPages,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PerPages)
        };
    }
}

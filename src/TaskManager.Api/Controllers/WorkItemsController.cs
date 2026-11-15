using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Common;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Services;

namespace TaskManager.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class WorkItemsController : ControllerBase
{
    private readonly CreateWorkItemService _createWorkItemService;
    private readonly UpdateWorkItemService _updateWorkItemService;
    private readonly IValidator<WorkItemRequest> _workItemRequestValidator;
    private readonly GetWorkItemsService _getWorkItemsService;
    private readonly IValidator<GetWorkItemsRequest> _getWorkItemsValidator;
    private readonly GetWorkItemByIdService _getWorkItemByIdService;
    private readonly DeleteWorkItemService _deleteWorkItemService;

    public WorkItemsController(CreateWorkItemService createWorkItemService, 
        UpdateWorkItemService updateWorkItemService,
        IValidator<WorkItemRequest> createWorkItemValidator, 
        GetWorkItemsService getWorkItemsService, 
        IValidator<GetWorkItemsRequest> getWorkItemsValidator, 
        GetWorkItemByIdService getWorkItemByIdService,
        DeleteWorkItemService deleteWorkItemService)
    {
        //POST and PUT
        _createWorkItemService = createWorkItemService;
        _updateWorkItemService = updateWorkItemService;
        _workItemRequestValidator = createWorkItemValidator;

        //GET
        _getWorkItemsService = getWorkItemsService;
        _getWorkItemsValidator = getWorkItemsValidator;

        //GET BY ID
        _getWorkItemByIdService = getWorkItemByIdService;

        //DELETE
        _deleteWorkItemService = deleteWorkItemService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] WorkItemRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _workItemRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()
                );
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        var createdWorkItem = await _createWorkItemService.CreateAsync(request, cancellationToken);
        return Created($"/api/tasks/{createdWorkItem.Id}", createdWorkItem);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] WorkItemRequest request, [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var validationResult = await _workItemRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()
                );
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        var updatedWorkItem = await _updateWorkItemService.UpdateAsync(id, request, cancellationToken);
        if (updatedWorkItem is null)
        {
            return NotFound();
        }
        return Ok(updatedWorkItem);
    }


    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<WorkItemResponseList>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] GetWorkItemsRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _getWorkItemsValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()
                );
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        var result = await _getWorkItemsService.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkItemById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _getWorkItemByIdService.GetItemAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(NoContent), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkItem([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _deleteWorkItemService.DeleteWorkItemAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}

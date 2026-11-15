using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.WorkItems;
using TaskManager.Application.Services;
using TaskManager.Application.Validators.Tasks;

namespace TaskManager.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CreateWorkItemController : ControllerBase
{
    private readonly CreateWorkItemService _service;
    private readonly IValidator<CreateWorkItemRequest> _validator;

    public CreateWorkItemController(CreateWorkItemService service, IValidator<CreateWorkItemRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWorkItemRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
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

        var createdWorkItem = await _service.CreateAsync(request, cancellationToken);
        return Created($"/api/tasks/{createdWorkItem.Id}", createdWorkItem);
    }
}

using FluentValidation;
using TaskManager.Application.DTOs.WorkItems;

namespace TaskManager.Application.Validators.Tasks;

public class CreateWorkItemValidator : AbstractValidator<CreateWorkItemRequest>
{
    public CreateWorkItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("The title cannot be empty")
            .MaximumLength(200)
            .WithMessage("The title cannot be greater than 200 characters");
        
        RuleFor(x => x.Asignee)
            .NotEmpty()
            .WithMessage("A Task must have an responsible")
            .MaximumLength(500)
            .WithMessage("The asignee name is so large");
        
        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due Date is required");
    }
}
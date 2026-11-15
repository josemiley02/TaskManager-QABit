using System;
using FluentValidation;
using TaskManager.Application.DTOs.WorkItems;

namespace TaskManager.Application.Validators.WorkItems;

public class GetWorkItemsValidator : AbstractValidator<GetWorkItemsRequest>
{
    public GetWorkItemsValidator()
    {
        RuleFor(p => p.Asignee)
            .MaximumLength(150)
            .WithMessage("The Asignee name is too large");
        
        RuleFor(p => p.PerPages)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
        
        RuleFor(x => x.Asignee)
            .MaximumLength(150)
            .WithMessage("Asignee cannot exceed 150 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Asignee));

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("The task status is invalid.");
    }
}

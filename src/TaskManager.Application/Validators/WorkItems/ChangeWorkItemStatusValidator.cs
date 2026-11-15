using System;
using FluentValidation;
using TaskManager.Application.DTOs.WorkItems;

namespace TaskManager.Application.Validators.WorkItems;

public class ChangeWorkItemStatusValidator : AbstractValidator<ChangeWorkItemStatusRequest>
{
    public ChangeWorkItemStatusValidator()
    {
        RuleFor(x => x.WorkItemStatus)
            .IsInEnum()
            .WithMessage("Invalid Status");
    }
}

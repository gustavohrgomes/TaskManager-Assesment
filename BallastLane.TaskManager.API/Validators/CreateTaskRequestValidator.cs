using BallastLane.TaskManager.Models;
using FluentValidation;

namespace BallastLane.TaskManager.Validators;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DueDate)
            .Must(d => d == null || d > timeProvider.GetUtcNow())
            .WithMessage("Due date must be in the future.");
    }
}

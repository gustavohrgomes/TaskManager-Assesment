using BallastLane.TaskManager.Models;
using FluentValidation;

namespace BallastLane.TaskManager.Validators;

public sealed class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    private static readonly string[] ValidStatuses = ["pending", "inProgress", "completed"];

    public UpdateTaskRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).NotEmpty()
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: pending, inProgress, completed.");
        RuleFor(x => x.DueDate)
            .Must(d => d == null || d > timeProvider.GetUtcNow())
            .WithMessage("Due date must be in the future.");
    }
}

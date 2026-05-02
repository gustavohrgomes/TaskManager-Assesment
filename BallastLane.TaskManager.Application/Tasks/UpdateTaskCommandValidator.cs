using FluentValidation;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// FluentValidation rules for <see cref="UpdateTaskCommand"/>: enforces required identifiers, title
/// length limits, a future due date when supplied, and a known status enum value.
/// </summary>
public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(TaskItem.MaxTitleLength);
        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .When(x => x.Description is not null);
        RuleFor(x => x.DueDate)
            .GreaterThan(timeProvider.GetUtcNow())
            .When(x => x.DueDate.HasValue);
        RuleFor(x => x.Status).IsInEnum();
    }
}

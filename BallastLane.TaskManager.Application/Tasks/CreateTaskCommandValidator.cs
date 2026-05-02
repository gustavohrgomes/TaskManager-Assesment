using FluentValidation;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// FluentValidation rules for <see cref="CreateTaskCommand"/>: enforces required title, length limits,
/// and a future due date. The injected <see cref="TimeProvider"/> defines "now" for the due-date check.
/// </summary>
public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(TaskItem.MaxTitleLength);

        RuleFor(x => x.Description)
            .MaximumLength(4000)
            .When(x => x.Description is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(timeProvider.GetUtcNow())
            .When(x => x.DueDate.HasValue);
    }
}

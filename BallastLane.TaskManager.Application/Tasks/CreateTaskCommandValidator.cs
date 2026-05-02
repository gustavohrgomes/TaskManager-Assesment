using BallastLane.TaskManager.Domain.Tasks;
using FluentValidation;

namespace BallastLane.TaskManager.Application.Tasks;

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

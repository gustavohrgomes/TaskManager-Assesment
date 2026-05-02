using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace BallastLane.TaskManager.Application.Tests.Tasks;

public class CreateTaskHandlerTests
{
    [Fact]
    public async Task CreateTaskHandler_HappyPath_ReturnsTaskResult()
    {
        var fixedNow = new DateTimeOffset(2026, 5, 2, 12, 0, 0, TimeSpan.Zero);
        var time = new FakeTimeProvider(fixedNow);

        var validator = Substitute.For<IValidator<CreateTaskCommand>>();
        validator.Validate(Arg.Any<CreateTaskCommand>()).Returns(new ValidationResult());

        var tasks = Substitute.For<ITaskRepository>();
        var ownerId = Guid.NewGuid();
        var userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(ownerId);

        var handler = new CreateTaskHandler(validator, tasks, userContext, time);

        var command = new CreateTaskCommand(
            Title: "Buy milk",
            Description: null,
            DueDate: fixedNow.AddDays(1));

        var result = await handler.Handle(command, CancellationToken.None);

        result.Title.ShouldBe("Buy milk");
        result.OwnerId.ShouldBe(ownerId);
        result.Status.ShouldBe(TaskStatus.Pending);
        result.CreatedAt.ShouldBe(fixedNow);
        await tasks.Received(1).AddAsync(
            Arg.Is<TaskItem>(t => t.Title == "Buy milk" && t.OwnerId == ownerId),
            Arg.Any<CancellationToken>());
    }
}

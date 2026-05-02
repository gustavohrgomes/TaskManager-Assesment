using BallastLane.TaskManager.Exceptions;
using BallastLane.TaskManager.Tasks;

namespace BallastLane.TaskManager.Domain.Tests.Tasks;

public class TaskItemTests
{
    [Fact]
    public void Start_FromPending_TransitionsToInProgress()
    {
        var now = DateTimeOffset.UtcNow;
        var task = TaskItemFactory.AValidTask(now);

        task.Start(now.AddMinutes(1));

        task.Status.ShouldBe(TaskStatus.InProgress);
        task.UpdatedAt.ShouldBe(now.AddMinutes(1));
    }

    [Fact]
    public void Create_AlwaysSetsStatusToPending()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Status.ShouldBe(TaskStatus.Pending);
    }

    [Fact]
    public void Create_WithEmptyTitle_Throws()
    {
        var ex = Should.Throw<DomainValidationException>(() =>
            TaskItem.Create(Guid.NewGuid(), title: "", description: null, dueDate: null, now: DateTimeOffset.UtcNow));

        ex.Errors.ShouldContain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Create_WithTitleExceedingMax_Throws()
    {
        var longTitle = new string('a', TaskItem.MaxTitleLength + 1);

        Should.Throw<DomainValidationException>(() =>
            TaskItem.Create(Guid.NewGuid(), title: longTitle, description: null, dueDate: null, now: DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AssertOwnedBy_WithDifferentUser_Throws()
    {
        var ownerId = Guid.NewGuid();
        var task = TaskItem.Create(ownerId, title: "test", description: null, dueDate: null, now: DateTimeOffset.UtcNow);

        var ex = Should.Throw<TaskNotOwnedByUserException>(() => task.AssertOwnedBy(Guid.NewGuid()));
        ex.UserId.ShouldNotBe(ownerId);
    }

    [Fact]
    public void AssertOwnedBy_WithCorrectUser_DoesNotThrow()
    {
        var ownerId = Guid.NewGuid();
        var task = TaskItem.Create(ownerId, title: "test", description: null, dueDate: null, now: DateTimeOffset.UtcNow);

        Should.NotThrow(() => task.AssertOwnedBy(ownerId));
    }
}

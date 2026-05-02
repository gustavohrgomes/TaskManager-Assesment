using BallastLane.TaskManager.Domain.Exceptions;
using BallastLane.TaskManager.Domain.Tasks;

namespace BallastLane.TaskManager.Domain.Tests.Tasks;

public class TaskStatusTransitionTests
{
    [Fact]
    public void Complete_FromInProgress_TransitionsToCompleted()
    {
        var now = DateTimeOffset.UtcNow;
        var task = TaskItemFactory.AValidTask(now);
        task.Start(now.AddMinutes(1));

        task.Complete(now.AddMinutes(2));

        task.Status.ShouldBe(TaskStatus.Completed);
        task.UpdatedAt.ShouldBe(now.AddMinutes(2));
    }

    [Fact]
    public void Complete_FromPending_Throws()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        var ex = Should.Throw<InvalidStatusTransitionException>(() => task.Complete(DateTimeOffset.UtcNow));
        ex.From.ShouldBe(TaskStatus.Pending);
        ex.To.ShouldBe(TaskStatus.Completed);
    }

    [Fact]
    public void Start_FromInProgress_Throws()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Start(DateTimeOffset.UtcNow);
        Should.Throw<InvalidStatusTransitionException>(() => task.Start(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Start_FromCompleted_Throws()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Start(DateTimeOffset.UtcNow);
        task.Complete(DateTimeOffset.UtcNow);
        Should.Throw<InvalidStatusTransitionException>(() => task.Start(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Complete_FromCompleted_Throws()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Start(DateTimeOffset.UtcNow);
        task.Complete(DateTimeOffset.UtcNow);
        Should.Throw<InvalidStatusTransitionException>(() => task.Complete(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Complete_FromInProgress_TwiceThrowsOnSecond()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Start(DateTimeOffset.UtcNow);
        task.Complete(DateTimeOffset.UtcNow);
        Should.Throw<InvalidStatusTransitionException>(() => task.Complete(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Start_FromPending_TwiceThrowsOnSecond()
    {
        var task = TaskItemFactory.AValidTask(DateTimeOffset.UtcNow);
        task.Start(DateTimeOffset.UtcNow);
        Should.Throw<InvalidStatusTransitionException>(() => task.Start(DateTimeOffset.UtcNow));
    }
}

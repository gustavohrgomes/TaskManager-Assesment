using BallastLane.TaskManager.Domain.Tasks;

namespace BallastLane.TaskManager.Domain.Tests.Tasks;

internal static class TaskItemFactory
{
    public static TaskItem AValidTask(DateTimeOffset now) =>
        TaskItem.Create(
            ownerId: Guid.NewGuid(),
            title: "test",
            description: null,
            dueDate: null,
            now: now);
}

using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Tasks;

public sealed class TaskItem
{
    public const int MaxTitleLength = 200;

    public Guid Id { get; }
    public Guid OwnerId { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TaskItem(
        Guid id,
        Guid ownerId,
        string title,
        string? description,
        TaskStatus status,
        DateTimeOffset? dueDate,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        OwnerId = ownerId;
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static TaskItem Create(
        Guid ownerId,
        string title,
        string? description,
        DateTimeOffset? dueDate,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException(nameof(Title), "Title is required.");
        if (title.Length > MaxTitleLength)
            throw new DomainValidationException(nameof(Title), $"Title cannot exceed {MaxTitleLength} characters.");

        return new TaskItem(
            id: Guid.NewGuid(),
            ownerId: ownerId,
            title: title,
            description: description,
            status: TaskStatus.Pending,
            dueDate: dueDate,
            createdAt: now,
            updatedAt: now);
    }

    public void Start(DateTimeOffset now)
    {
        if (Status != TaskStatus.Pending)
            throw new InvalidStatusTransitionException(Status, TaskStatus.InProgress);
        Status = TaskStatus.InProgress;
        UpdatedAt = now;
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status != TaskStatus.InProgress)
            throw new InvalidStatusTransitionException(Status, TaskStatus.Completed);
        Status = TaskStatus.Completed;
        UpdatedAt = now;
    }

    public void AssertOwnedBy(Guid userId)
    {
        if (OwnerId != userId)
            throw new TaskNotOwnedByUserException(Id, userId);
    }

    public void UpdateDetails(
        string title,
        string? description,
        DateTimeOffset? dueDate,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException(nameof(Title), "Title is required.");
        if (title.Length > MaxTitleLength)
            throw new DomainValidationException(nameof(Title), $"Title cannot exceed {MaxTitleLength} characters.");

        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = now;
    }
}

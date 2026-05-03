using BallastLane.TaskManager.Exceptions;

namespace BallastLane.TaskManager.Tasks;

/// <summary>
/// Aggregate root representing a single task owned by a user. Encapsulates the title/description/due-date
/// state, the lifecycle state machine, and the per-user ownership invariant.
/// </summary>
public sealed class TaskItem
{
    /// <summary>Maximum number of characters allowed in <see cref="Title"/>.</summary>
    public const int MaxTitleLength = 200;

    /// <summary>Unique identifier assigned to the task at creation.</summary>
    public Guid TaskId { get; }
    /// <summary>Identifier of the user who owns this task. Immutable after creation.</summary>
    public Guid OwnerId { get; }
    /// <summary>Short human-readable title describing the task.</summary>
    public string Title { get; private set; }
    /// <summary>Optional longer-form description providing additional context.</summary>
    public string? Description { get; private set; }
    /// <summary>Current lifecycle state of the task.</summary>
    public TaskStatus Status { get; private set; }
    /// <summary>Optional UTC timestamp by which the task should be completed.</summary>
    public DateTimeOffset? DueDate { get; private set; }
    /// <summary>UTC timestamp at which the task was created.</summary>
    public DateTimeOffset CreatedAt { get; }
    /// <summary>UTC timestamp of the most recent mutation to the task.</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    public TaskItem(
        Guid taskId,
        Guid ownerId,
        string title,
        string? description,
        TaskStatus status,
        DateTimeOffset? dueDate,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        TaskId = taskId;
        OwnerId = ownerId;
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Creates a new <see cref="TaskItem"/> in the <see cref="TaskStatus.Pending"/> state with a freshly generated identifier.
    /// </summary>
    /// <param name="ownerId">Identifier of the user who will own the task.</param>
    /// <param name="title">Required short title; must be non-empty and at most <see cref="MaxTitleLength"/> characters.</param>
    /// <param name="description">Optional long-form description.</param>
    /// <param name="dueDate">Optional UTC due date.</param>
    /// <param name="now">UTC timestamp to record as both <see cref="CreatedAt"/> and <see cref="UpdatedAt"/>.</param>
    /// <returns>A new pending task owned by <paramref name="ownerId"/>.</returns>
    public static TaskItem Create(
        Guid ownerId,
        string title,
        string? description,
        DateTimeOffset? dueDate,
        DateTimeOffset now)
    {
        DomainValidationException.ThrowIfNullOrWhiteSpace(title, nameof(Title));

        return title.Length > MaxTitleLength
            ? throw new DomainValidationException(nameof(Title), $"Title cannot exceed {MaxTitleLength} characters.")
            : new TaskItem(
            taskId: Guid.NewGuid(),
            ownerId: ownerId,
            title: title,
            description: description,
            status: TaskStatus.Pending,
            dueDate: dueDate,
            createdAt: now,
            updatedAt: now);
    }

    /// <summary>
    /// Transitions the task from <see cref="TaskStatus.Pending"/> to <see cref="TaskStatus.InProgress"/>.
    /// </summary>
    /// <param name="now">UTC timestamp to record as the new <see cref="UpdatedAt"/>.</param>
    public void Start(DateTimeOffset now)
    {
        if (Status != TaskStatus.Pending)
            throw new InvalidStatusTransitionException(Status, TaskStatus.InProgress);
        Status = TaskStatus.InProgress;
        UpdatedAt = now;
    }

    /// <summary>
    /// Transitions the task from <see cref="TaskStatus.InProgress"/> to <see cref="TaskStatus.Completed"/>.
    /// </summary>
    /// <param name="now">UTC timestamp to record as the new <see cref="UpdatedAt"/>.</param>
    public void Complete(DateTimeOffset now)
    {
        if (Status != TaskStatus.InProgress)
            throw new InvalidStatusTransitionException(Status, TaskStatus.Completed);
        Status = TaskStatus.Completed;
        UpdatedAt = now;
    }

    /// <summary>
    /// Enforces the per-user ownership invariant by throwing if the supplied caller does not own this task.
    /// Callers should invoke this immediately after loading a task and before any read or mutation.
    /// </summary>
    /// <param name="userId">Identifier of the caller attempting to access the task.</param>
    public void AssertOwnedBy(Guid userId)
    {
        if (OwnerId != userId)
            throw new TaskNotOwnedByUserException(TaskId, userId);
    }

    /// <summary>
    /// Replaces the mutable details (title, description, due date) of the task and stamps a new <see cref="UpdatedAt"/>.
    /// </summary>
    /// <param name="title">New required short title; same rules as <see cref="Create"/>.</param>
    /// <param name="description">New optional description.</param>
    /// <param name="dueDate">New optional UTC due date.</param>
    /// <param name="now">UTC timestamp to record as the new <see cref="UpdatedAt"/>.</param>
    public void UpdateDetails(
        string title,
        string? description,
        DateTimeOffset? dueDate,
        DateTimeOffset now)
    {
        DomainValidationException.ThrowIfNullOrWhiteSpace(title, nameof(Title));

        if (title.Length > MaxTitleLength)
            throw new DomainValidationException(nameof(Title), $"Title cannot exceed {MaxTitleLength} characters.");

        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = now;
    }
}

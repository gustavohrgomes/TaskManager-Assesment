namespace BallastLane.TaskManager.Abstractions;

/// <summary>
/// Coordinates a database transaction around a unit of repository work, ensuring that all writes
/// performed inside the supplied delegate either commit together or roll back together.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Runs <paramref name="work"/> inside a transaction and returns its result. The transaction commits
    /// when the delegate completes successfully and rolls back if it throws.
    /// </summary>
    /// <typeparam name="T">Type of value produced by the work delegate.</typeparam>
    /// <param name="work">Caller-supplied unit of work, given a cancellation token scoped to the transaction.</param>
    /// <param name="ct">Token used to cancel the transaction and the work it wraps.</param>
    /// <returns>The value returned by <paramref name="work"/>.</returns>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken ct);

    /// <summary>
    /// Runs <paramref name="work"/> inside a transaction with no return value. The transaction commits
    /// when the delegate completes successfully and rolls back if it throws.
    /// </summary>
    /// <param name="work">Caller-supplied unit of work, given a cancellation token scoped to the transaction.</param>
    /// <param name="ct">Token used to cancel the transaction and the work it wraps.</param>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> work,
        CancellationToken ct);
}

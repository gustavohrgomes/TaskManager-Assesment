using BallastLane.TaskManager.Auth.Login;
using BallastLane.TaskManager.Auth.Register;
using BallastLane.TaskManager.Tasks.CreateTask;
using BallastLane.TaskManager.Tasks.DeleteTask;
using BallastLane.TaskManager.Tasks.GetTasks;
using BallastLane.TaskManager.Tasks.UpdateTask;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.TaskManager;

/// <summary>
/// Composition root for the application layer: registers handlers, validators, and the shared
/// <see cref="TimeProvider"/> so the API project can wire up the layer with a single call.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application-layer services (handlers, validators, time provider) into the supplied container.
    /// </summary>
    /// <param name="services">Service collection to register into.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<GetTaskHandler>();
        services.AddScoped<GetTasksHandler>();
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginHandler>();

        return services;
    }
}

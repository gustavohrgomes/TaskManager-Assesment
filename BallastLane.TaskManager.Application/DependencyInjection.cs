using BallastLane.TaskManager.Application.Auth;
using BallastLane.TaskManager.Application.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddSingleton<IValidator<CreateTaskCommand>>(sp =>
            new CreateTaskCommandValidator(sp.GetRequiredService<TimeProvider>()));
        services.AddSingleton<IValidator<UpdateTaskCommand>>(sp =>
            new UpdateTaskCommandValidator(sp.GetRequiredService<TimeProvider>()));
        services.AddSingleton<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
        services.AddSingleton<IValidator<LoginCommand>, LoginCommandValidator>();

        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<GetTaskHandler>();
        services.AddScoped<ListTasksHandler>();
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginHandler>();

        return services;
    }
}

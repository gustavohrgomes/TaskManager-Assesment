using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Auth;
using BallastLane.TaskManager.Persistence;

namespace BallastLane.TaskManager;

public static class InfrastructureDependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDataSource("taskmanager");

        builder.Services.AddScoped<IDbContext, NpgsqlDbContext>();
        builder.Services.AddScoped<IUserRepository, NpgsqlUserRepository>();
        builder.Services.AddScoped<ITaskRepository, NpgsqlTaskRepository>();

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        builder.Services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();

        return builder;
    }
}

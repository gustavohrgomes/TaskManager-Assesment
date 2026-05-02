using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using BallastLane.TaskManager.Abstractions;
using BallastLane.TaskManager.Auth;
using BallastLane.TaskManager.Persistence;

namespace BallastLane.TaskManager;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("taskmanager");
        var dataSource = NpgsqlDataSource.Create(connectionString!);
        services.AddSingleton(dataSource);

        services.AddSingleton<IDbContext, NpgsqlDbContext>();

        services.AddScoped<IUserRepository, NpgsqlUserRepository>();
        services.AddScoped<ITaskRepository, NpgsqlTaskRepository>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();

        return services;
    }
}

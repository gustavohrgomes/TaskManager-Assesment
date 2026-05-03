using BallastLane.TaskManager.Tasks;
using BallastLane.TaskManager.Tasks.CreateTask;
using NetArchTest.Rules;

namespace BallastLane.TaskManager.ArchitectureTests;

public class LayerBoundaryTests
{
    [Fact]
    public void Domain_should_have_no_external_dependencies()
    {
        var domain = typeof(TaskItem).Assembly;

        var result = Types.InAssembly(domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.AspNetCore",
                "Microsoft.Extensions",
                "Npgsql",
                "FluentValidation",
                "BallastLane.TaskManager.Application",
                "BallastLane.TaskManager.Infrastructure",
                "BallastLane.TaskManager.API")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Domain leaked: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_should_not_reference_Npgsql()
    {
        var application = typeof(CreateTaskHandler).Assembly;

        var result = Types.InAssembly(application)
            .ShouldNot()
            .HaveDependencyOnAny("Npgsql", "Microsoft.EntityFrameworkCore", "Dapper")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Application leaked: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Controllers_should_not_depend_on_Npgsql()
    {
        var api = typeof(IApiMarker).Assembly;

        var result = Types.InAssembly(api)
            .That()
            .ResideInNamespaceMatching(@"BallastLane\.TaskManager\.API\.Controllers.*")
            .ShouldNot()
            .HaveDependencyOnAny("Npgsql")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            $"Controllers depend on Npgsql: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Banned_packages_are_absent_from_all_layers()
    {
        var assemblies = new[]
        {
            typeof(TaskItem).Assembly,
            typeof(CreateTaskHandler).Assembly,
            typeof(AssemblyMarker).Assembly,
            typeof(IApiMarker).Assembly,
        };

        var disallowedAssemblyPrefixes = new[]
        {
            "Microsoft.EntityFrameworkCore",
            "Dapper",
            "MediatR",
            "FluentValidation.AspNetCore",
        };

        foreach (var asm in assemblies)
        {
            var leaked = asm.GetReferencedAssemblies()
                .Select(a => a.Name ?? string.Empty)
                .Where(name => disallowedAssemblyPrefixes.Any(p => name.StartsWith(p, StringComparison.Ordinal)))
                .ToList();

            leaked.ShouldBeEmpty(
                $"{asm.GetName().Name} references banned assemblies: {string.Join(", ", leaked)}");
        }
    }
}

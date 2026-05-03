var builder = DistributedApplication.CreateBuilder(args);

var jwtKey = builder.AddParameter("jwt-key", secret: true);

var taskdb = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .WithEnvironment("POSTGRES_DB", "taskmanager")
    .WithInitFiles("../db")
    .AddDatabase("taskmanager");

var api = builder.AddProject<Projects.BallastLane_TaskManager_API>("ballastlane-taskmanager-api")
    .WithReference(taskdb)
    .WaitFor(taskdb)
    .WithExternalHttpEndpoints()
    .WithEnvironment("Jwt__Key", jwtKey)
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.AddJavaScriptApp("web", "../apps/web", "start")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();

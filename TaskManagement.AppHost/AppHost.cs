var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BallastLane_TaskManager_API>("ballastlane-taskmanager-api");

builder.Build().Run();

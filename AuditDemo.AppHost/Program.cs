var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AuditDemo_API>("api");

builder.Build().Run();

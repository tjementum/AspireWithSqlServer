using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("default")
    .AddDatabase("weather");

builder.AddProject<WebApi>("WebApi")
    .WithReference(sqlServer);

builder.Build().Run();
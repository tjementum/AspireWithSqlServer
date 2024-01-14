using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServerContainer("default")
    .AddDatabase("weather");

builder.AddProject<AspireWithSqlServer_WebApi>("WebApi")
    .WithReference(sqlServer);

builder.Build().Run();
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = "YourSecretPassword01!"; 

var sqlServer = builder.AddSqlServerContainer("Default", sqlPassword, port: 1433)
    .AddDatabase("weather");

builder.AddProject<AspireWithSqlServer_WebApi>("WebApi")
    .WithReference(sqlServer);

builder.Build().Run();
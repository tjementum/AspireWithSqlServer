using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = "YourSecretPassword01!"; 

var sqlServer = builder.AddSqlServerContainer("localhost", sqlPassword, 1433);

builder.AddProject<AspireWithSqlServer_WebApi>("WebApi")
    .WithEnvironment("SQL_PASSWORD", sqlPassword)
    .WithEnvironment("SQL_SERVER", sqlServer.Resource.Name);

builder.Build().Run();
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = "YourSecretPassword01!"; 

var sqlServer = builder.AddSqlServerContainer("127.0.0.1", sqlPassword, 1433)
    .WithAnnotation(
        new ContainerImageAnnotation { Registry = "mcr.microsoft.com", Image = "azure-sql-edge", Tag = "latest" }
    );

builder.AddProject<AspireWithSqlServer_WebApi>("WebApi")
    .WithEnvironment("SQL_PASSWORD", sqlPassword)
    .WithEnvironment("SQL_SERVER", sqlServer.Resource.Name);

builder.Build().Run();
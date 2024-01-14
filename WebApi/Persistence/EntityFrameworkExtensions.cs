using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AspireWithSqlServer.WebApi.Persistence;

public static class EntityFrameworkExtensions
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        var sqlServer = Environment.GetEnvironmentVariable("SQL_SERVER") ?? "localhost";
        var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? "YourSecretPassword01!";
        return connectionString!
            .Replace("${SQL_SERVER}", sqlServer)
            .Replace("${SQL_PASSWORD}", sqlPassword);
    }

    public static void ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(EntityFrameworkExtensions));

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        logger.LogInformation("Start migrating database. Version: {Version}", version);

        var retryCount = 0;
        while (retryCount <= 30)
        {
            if (retryCount >= 1)
                logger.LogInformation("Waiting for databases to be ready. Retry count: {RetryCount}", retryCount);

            try
            {
                var dbContext = scope.ServiceProvider.GetService<WeatherForecastContext>() ??
                                throw new UnreachableException("Missing DbContext.");

                dbContext.Database.Migrate();

                logger.LogInformation("Finished migrating database.");

                dbContext.WeatherForecasts.AddRange(new List<WeatherForecast>
                {
                    new() { Id = Guid.NewGuid(), Date = DateTime.Today, Summary = "Sunny", TemperatureC = 30 },
                    new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(1), Summary = "Rainy", TemperatureC = 10 },
                    new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(2), Summary = "Cloudy", TemperatureC = 20 },
                    new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(3), Summary = "Sunny", TemperatureC = 25 }
                });
                dbContext.SaveChanges();

                logger.LogInformation("Created dummy data in database.");

                break;
            }
            catch (SqlException ex) when (ex.Message.Contains("an error occurred during the pre-login handshake"))
            {
                // Known error in Aspire, when SQL Server is not ready. See: https://github.com/dotnet/aspire/issues/1023
                retryCount++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            catch (SocketException ex) when (ex.Message.Contains("Invalid argument"))
            {
                // Known error in Aspire, when SQL Server is not ready See: https://github.com/dotnet/aspire/issues/1023
                retryCount++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations.");

                // Wait for the logger to flush
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
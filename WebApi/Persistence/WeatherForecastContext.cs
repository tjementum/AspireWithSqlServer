using Microsoft.EntityFrameworkCore;

namespace AspireWithSqlServer.WebApi.Persistence;

public class WeatherForecastContext(DbContextOptions<WeatherForecastContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        base.OnConfiguring(optionsBuilder);
    }
}
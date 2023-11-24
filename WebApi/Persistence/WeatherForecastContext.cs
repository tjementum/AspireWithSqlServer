using Microsoft.EntityFrameworkCore;

namespace AspireWithSqlServer.WebApi.Persistence;

public class WeatherForecastContext(DbContextOptions<WeatherForecastContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
}
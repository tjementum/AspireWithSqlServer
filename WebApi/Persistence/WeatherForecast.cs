using System.ComponentModel.DataAnnotations;

namespace AspireWithSqlServer.WebApi.Persistence;

public class WeatherForecast
{
    [Key] public Guid Id { get; init; } = new();

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; init; }
}
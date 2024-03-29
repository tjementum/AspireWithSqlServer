using AspireWithSqlServer.ServiceDefaults;
using AspireWithSqlServer.WebApi.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("weather");
builder.Services.AddSqlServer<WeatherForecastContext>(connectionString);
builder.EnrichSqlServerDbContext<WeatherForecastContext>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
await app.Services.ApplyMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.MapGet("/weatherforecast", (WeatherForecastContext dbContext) => dbContext.WeatherForecasts.ToListAsync());

app.Run();
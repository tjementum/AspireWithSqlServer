using AspireWithSqlServer.ServiceDefaults;
using AspireWithSqlServer.WebApi.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString();
Console.WriteLine("connectionString: " + connectionString);
builder.Services.AddDbContext<WeatherForecastContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();
app.Services.ApplyMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.MapGet("/weatherforecast", (WeatherForecastContext dbContext) => dbContext.WeatherForecasts.ToListAsync());

app.Run();
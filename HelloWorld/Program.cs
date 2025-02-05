using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Define service name and version for telemetry
var serviceName = "HelloWorld";
var serviceVersion = "1.0.0";

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .WithMetrics(metrics => metrics
        // Add default metrics providers
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        // Add custom meters
        .AddMeter("HelloWorld.Metrics"));

// Add Prometheus endpoint
//builder.Services.AddOpenTelemetryPrometheusScrapingEndpoint();

// Create custom meter for application-specific metrics
var meter = new Meter("HelloWorld.Metrics", "1.0.0");
var requestCounter = meter.CreateCounter<int>("hello_requests", "Number of hello requests");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add a simple hello endpoint
app.MapGet("/hello", () =>
{
    requestCounter.Add(1);
    return "Hello, World! (.NET Core)";
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Add the weather forecast endpoint
app.MapGet("/weather", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Define service name and version for telemetry
var serviceName = "HelloWorld";
var serviceVersion = "1.0.0";

// Get logger early to use during startup
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger(serviceName);

// Get OpenTelemetry configuration
var otelConfig = builder.Configuration.GetSection("OpenTelemetry");

// Log the configuration
logger.LogInformation("OpenTelemetry Configuration: Endpoint={Endpoint}, Protocol={Protocol}",
    otelConfig["Endpoint"] ?? "unknown",
    otelConfig["Protocol"] ?? "unknown");

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
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri(otelConfig["Endpoint"]);
            otlpOptions.Protocol = otelConfig["Protocol"]?.ToLower() == "http" ? OtlpExportProtocol.HttpProtobuf : OtlpExportProtocol.Grpc;

        })
        // Add custom meters
        .AddMeter("HelloWorld.Metrics"));

// Create custom meter for application-specific metrics
var meter = new Meter("HelloWorld.Metrics", "1.0.0");
var helloCounter = meter.CreateCounter<int>("hello_requests", "Number of hello requests");
var weatherCounter = meter.CreateCounter<int>("weather_requests", "Number of weather requests");


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
    helloCounter.Add(1);
    return "Hello, World! (.NET Core)";
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Add the weather forecast endpoint
app.MapGet("/weather", () =>
{
    weatherCounter.Add(1);
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

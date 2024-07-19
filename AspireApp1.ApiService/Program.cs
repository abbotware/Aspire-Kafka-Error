using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddHostedService<MyWorker>();

builder.AddKafkaProducer<string, string>("messaging");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


internal sealed class MyWorker(IProducer<string, string> producer, ILogger<MyWorker> logger) : BackgroundService
{
    // Use producer...
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        for (int i = 0; i < 10; i++)
        {
            var m = new Message<string, string>();
            m.Key = i.ToString();
            m.Value = $"abc{i}";

            producer.Produce("test3", m, dr => { logger.LogInformation(dr.Status.ToString()); });
        }

        return Task.CompletedTask;
    }
}
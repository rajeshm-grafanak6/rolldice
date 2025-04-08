using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
const string serviceName = "roll-dice";

string apps = "app1";

builder.Services.Configure<OtlpExporterOptions>("tracing", builder.Configuration.GetSection("OpenTelemetry:tracing:otlp"));
builder.Services.Configure<OtlpExporterOptions>("metrics", builder.Configuration.GetSection("OpenTelemetry:metrics:otlp"));
builder.Services.Configure<OtlpExporterOptions>("logging", builder.Configuration.GetSection("OpenTelemetry:logging:otlp"));

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName)
                .AddAttributes(new []
                    {
                        new KeyValuePair<string, object>("loki.resource.labels", "apps"),
                        new KeyValuePair<string, object>("apps", apps),
                    })
                )
        .AddConsoleExporter()
        .AddOtlpExporter(
            "logging",
            options => {
                options.Protocol = OtlpExportProtocol.Grpc;
                // options.Endpoint = new Uri("http://0.0.0.0:4317");
                options.Endpoint = new Uri("http://127.0.0.1:4317");
            }
        );
});
builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName)
                                        .AddAttributes(new Dictionary<string, object>
                                        {
                                            { "deployment.environment", builder.Environment.EnvironmentName }
                                        })
      )
      
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(
            "tracing",
            options => {
                options.Protocol = OtlpExportProtocol.Grpc;
                // options.Endpoint = new Uri("http://0.0.0.0:4317");
                options.Endpoint = new Uri("http://127.0.0.1:4317");
            }
          ))
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(
            "metrics",
            options => {
                options.Protocol = OtlpExportProtocol.Grpc;
                // options.Endpoint = new Uri("http://0.0.0.0:4317");
                options.Endpoint = new Uri("http://127.0.0.1:4317");
            }
          ));

var app = builder.Build();

string HandleRollDice([FromServices]ILogger<Program> logger, string? player)
{
    var result = RollDice();
    var activity = Activity.Current;
    activity?.SetTag("app.name", apps);
    

    if (string.IsNullOrEmpty(player))
    {
        logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
        #pragma warning disable CS8602
        string traceId = activity.TraceId.ToString();
        #pragma warning restore CS8602
        Console.WriteLine("trace_id="+ traceId +" Anonymous player is rolling the dice: " +result);
    }
    else
    {
        logger.LogInformation("{player} is rolling the dice: {result}", player, result);
        Console.WriteLine("{player} is rolling the dice: "+player+"-"+result);
    }

    return result.ToString(CultureInfo.InvariantCulture);
}

int RollDice()
{
    return Random.Shared.Next(1, 7);
}

app.MapGet("/rolldice/{player?}", HandleRollDice);

app.Run();

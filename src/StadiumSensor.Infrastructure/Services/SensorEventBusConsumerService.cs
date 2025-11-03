using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using StadiumSensor.Core.Interfaces;
using StadiumSensor.Core.Models;
using Microsoft.Extensions.Configuration;
using StadiumSensor.Core.Models.AppSettings;
using Microsoft.Extensions.Options;

public class SensorEventBusConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SensorEventBusConsumerService> _logger;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;
    private readonly AzureServiceBus _azureServiceBus;

    public SensorEventBusConsumerService(
        IServiceScopeFactory scopeFactory,
        ILogger<SensorEventBusConsumerService> logger,
        IOptions<AzureServiceBus> azureServiceBus
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _azureServiceBus = azureServiceBus.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client = new ServiceBusClient(_azureServiceBus.ConnectionString);
        _processor = _client.CreateProcessor(_azureServiceBus.QueueName, new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += async args =>
        {
            try
            {
                var body = args.Message.Body.ToString();
                var sensorEvent = System.Text.Json.JsonSerializer.Deserialize<SensorEvent>(body);

                if (sensorEvent != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ISensorEventRepo>();
                    await repo.AddAsync(sensorEvent, stoppingToken);
                    _logger.LogInformation("Processed event from bus: {gate} {type} {count}", sensorEvent.Gate, sensorEvent.Type, sensorEvent.NumberOfPeople);
                }
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Service Bus message");
                await args.AbandonMessageAsync(args.Message);
            }
        };

        _processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Service Bus error");
            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync(stoppingToken);

        // Wait until cancelled
        await Task.Delay(Timeout.Infinite, stoppingToken);

        await _processor.StopProcessingAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
            await _processor.DisposeAsync();
        if (_client != null)
            await _client.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
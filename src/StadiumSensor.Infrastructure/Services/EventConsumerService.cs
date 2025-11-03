using Microsoft.Extensions.Logging;
using StadiumSensor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StadiumSensor.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace StadiumSensor.Infrastructure.Services
{
    public class EventConsumerService : BackgroundService
    {
        private readonly ChannelReader<SensorEvent> _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventConsumerService> _logger;

        public EventConsumerService(ChannelReader<SensorEvent> channel, IServiceScopeFactory scopeFactory, ILogger<EventConsumerService> logger)
        {
            _channel = channel;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var evt in _channel.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        // Create a scope per event so we can resolve scoped services
                        using var scope = _scopeFactory.CreateScope();
                        var repository = scope.ServiceProvider.GetRequiredService<ISensorEventRepo>();
                        await repository.AddAsync(evt, stoppingToken);
                        _logger.LogInformation("Processed event: {gate} {type} {count} people", evt.Gate, evt.Type, evt.NumberOfPeople);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error processing event: {gate} {type} {count}",
                            evt.Gate, evt.Type, evt.NumberOfPeople);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event processing");
            }
        }
    }


}

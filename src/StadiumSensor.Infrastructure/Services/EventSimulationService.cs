using Microsoft.Extensions.Logging;
using StadiumSensor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace StadiumSensor.Infrastructure.Services
{
   public class EventSimulationService : BackgroundService
    {
        private readonly ChannelWriter<SensorEvent> _channel;
        private readonly ILogger<EventSimulationService> _logger;
        private readonly Random _random = new();

        // In production environment where simulation would not be needed,real sensor event would send messages (as JSON) to the Service Bus queue/topic.
        // The API will consume, process, and store events dynamically,no hardcoded gates and event types would be here.
        private readonly string[] _gates = ["Gate A", "Gate B", "Gate C"];
        private readonly string[] _types = ["enter", "leave"];

        public EventSimulationService(ChannelWriter<SensorEvent> channel, ILogger<EventSimulationService> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Create random event
                    var evt = new SensorEvent
                    {
                        Gate = _gates[_random.Next(_gates.Length)],
                        Type = _types[_random.Next(_types.Length)],
                        Timestamp = DateTime.UtcNow,
                        NumberOfPeople = _random.Next(1, 11) // 1-10 people
                    };

                    await _channel.WriteAsync(evt, stoppingToken);
                    _logger.LogInformation(
                        "Simulated event: {gate} {type} {count} people",
                        evt.Gate, evt.Type, evt.NumberOfPeople);

                    // Random delay between 1-5 seconds
                    await Task.Delay(TimeSpan.FromSeconds(_random.Next(1, 6)), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}

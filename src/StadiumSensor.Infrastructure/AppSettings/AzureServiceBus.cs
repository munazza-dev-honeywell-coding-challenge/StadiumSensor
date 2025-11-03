using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StadiumSensor.Core.Models.AppSettings
{
    public class AzureServiceBus
    {
        public string? ConnectionString { get; set; }
        public string? QueueName { get; set; }
    }
}

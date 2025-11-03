using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StadiumSensor.Core.Models
{
    public class SensorEvent
    {
        public int Id { get; set; }
        public required string Gate { get; set; }
        public DateTime Timestamp { get; set; }
        public int NumberOfPeople { get; set; }
        public required string Type { get; set; } // "enter" or "leave"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

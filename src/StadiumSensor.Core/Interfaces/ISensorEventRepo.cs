using StadiumSensor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace StadiumSensor.Core.Interfaces
{
   public interface ISensorEventRepo
    {
        Task AddAsync(SensorEvent sensorEvent, CancellationToken cancellationToken = default);
        Task<ContentResult> GetSummaryAsync(string? gate = null, string? type = null, DateTime? startTime = null, DateTime? endTime = null,
            CancellationToken cancellationToken = default);
    }
}

public class SensorEventSummaryDto
{
    public required string Gate { get; set; }
    public int NumberOfPeople { get; set; }
    public required string Type { get; set; }

    public static SensorEventSummaryDto NewObject(string gate, string type, int numberOfPeople)
    {
        return new SensorEventSummaryDto()
        {
            Gate = gate,
            Type = type,
            NumberOfPeople = numberOfPeople
        };
    }
}

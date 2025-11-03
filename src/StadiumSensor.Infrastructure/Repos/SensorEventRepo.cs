using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StadiumSensor.Core.Interfaces;
using StadiumSensor.Core.Models;
using StadiumSensor.Infrastructure.Data;
using StadiumSensor.Infrastructure.Services;
//using ILogger = Serilog.ILogger;

namespace StadiumSensor.Infrastructure.Repos
{
   public class SensorEventRepo : ServiceBase,ISensorEventRepo
    {
        private readonly SensorEventDbContext _context;
        private readonly ILogger<SensorEventRepo> _logger;

        public SensorEventRepo(SensorEventDbContext context, ILogger<SensorEventRepo> logger)
        {
            _context = context;
            Log.ForContext<SensorEventRepo>();
        }

        public async Task AddAsync(SensorEvent sensorEvent, CancellationToken cancellationToken = default)
        {
            await _context.SensorEvents.AddAsync(sensorEvent, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ContentResult> GetSummaryAsync(string? gate = null, string? type = null, DateTime? startTime = null, DateTime? endTime = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.SensorEvents.AsNoTracking();

                if (!string.IsNullOrEmpty(gate))
                    query = query.Where(e => e.Gate == gate);

                if (!string.IsNullOrEmpty(type))
                    query = query.Where(e => e.Type == type);

                if (startTime.HasValue)
                    query = query.Where(e => e.Timestamp >= startTime.Value);

                if (endTime.HasValue)
                    query = query.Where(e => e.Timestamp <= endTime.Value);

                var summary = await query
                    .GroupBy(e => new { e.Gate, e.Type }).Select(g =>
                        SensorEventSummaryDto.NewObject(g.Key.Gate, g.Key.Type, g.Sum(e => e.NumberOfPeople)))
                    .ToListAsync(cancellationToken);

                return await CreateResponse(HttpStatusCode.OK, summary);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not Event summary");
                throw;
            }
        }
        
    }
}

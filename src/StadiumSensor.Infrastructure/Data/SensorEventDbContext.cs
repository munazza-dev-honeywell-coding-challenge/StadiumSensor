using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StadiumSensor.Core.Models;

namespace StadiumSensor.Infrastructure.Data
{
   public class SensorEventDbContext: DbContext
    {
        public SensorEventDbContext(DbContextOptions<SensorEventDbContext> options) : base(options)
        {
        }

        public DbSet<SensorEvent> SensorEvents => Set<SensorEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorEvent>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Gate).IsRequired();
                builder.Property(e => e.Type).IsRequired();
                builder.Property(e => e.Timestamp).IsRequired();

                // Index to support filtering and grouping queries
                builder.HasIndex(e => new { e.Gate, e.Type, e.Timestamp });
            });
        }
    }
}

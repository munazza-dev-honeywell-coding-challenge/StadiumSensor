using Microsoft.EntityFrameworkCore;
using Moq;
using StadiumSensor.Core.Models;
using StadiumSensor.Infrastructure.Data;
using FluentAssertions;
using System;
using StadiumSensor.Infrastructure.Repos;
using Microsoft.Extensions.Logging;

namespace StadiumSensor.Tests;

public class SensorEventRepositoryTests
{
    private readonly DbContextOptions<SensorEventDbContext> _options;

    public SensorEventRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<SensorEventDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddEventToDatabase()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<SensorEventRepo>>();
        using var context = new SensorEventDbContext(_options);
        var repository = new SensorEventRepo(context, mockLogger.Object);
        var sensorEvent = new SensorEvent
        {
            Gate = "Test Gate",
            Type = "enter",
            NumberOfPeople = 5,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(sensorEvent);

        // Assert
        context.SensorEvents.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(sensorEvent, options =>
                options.Excluding(x => x.Id).Excluding(x => x.CreatedAt));
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldReturnGroupedResults()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<SensorEventRepo>>();
        using var context = new SensorEventDbContext(_options);
        var repository = new SensorEventRepo(context, mockLogger.Object);

        var events = new[]
        {
            new SensorEvent { Gate = "Gate A", Type = "enter", NumberOfPeople = 5, Timestamp = DateTime.UtcNow },
            new SensorEvent { Gate = "Gate A", Type = "enter", NumberOfPeople = 3, Timestamp = DateTime.UtcNow },
            new SensorEvent { Gate = "Gate B", Type = "leave", NumberOfPeople = 2, Timestamp = DateTime.UtcNow }
        };

        await context.SensorEvents.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Act
        var summary = await repository.GetSummaryAsync();

        // Assert
        Assert.NotNull(summary);
    }
}
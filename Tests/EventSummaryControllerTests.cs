using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StadiumSensor.API.Controllers;
using StadiumSensor.Core.Interfaces;

namespace StadiumSensor.Tests.Unit
{
    public class SummaryControllerTests
    {
        [Fact]
        public async Task GetSummary_ReturnsOk()
        {
            // Arrange
            var mockRepo = new Mock<ISensorEventRepo>();
            mockRepo
                .Setup(r => r.GetSummaryAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentResult(){
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json"
                });
            

            var mockLogger = new Mock<ILogger<EventSummaryController>>();
            var controller = new EventSummaryController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetEventSummary(gate: "Gate A", type: "enter");

            // Assert
            result.Should().BeOfType<ContentResult>();
            var ok = result as ContentResult;
            ok!.StatusCode.Should().Be(200);

            mockRepo.Verify(r => r.GetSummaryAsync("Gate A", "enter", null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetSummary_WhenRepositoryThrows_ReturnsProblem()
        {
            // Arrange
            var mockRepo = new Mock<ISensorEventRepo>();
            mockRepo
                .Setup(r => r.GetSummaryAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("db error"));

            var mockLogger = new Mock<ILogger<EventSummaryController>>();
            var controller = new EventSummaryController(mockRepo.Object, mockLogger.Object);

            // Act
            var result = await controller.GetEventSummary();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var obj = result as ObjectResult;
            obj!.StatusCode.Should().Be(500);
        }
    }
}

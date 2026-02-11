using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Analysis;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class AnalyticsControllerTests
    {
        private readonly Mock<IEventAnalysisService> _serviceMock;
        private readonly AnalyticsController _controller;

        public AnalyticsControllerTests()
        {
            _serviceMock = new Mock<IEventAnalysisService>();
            _controller = new AnalyticsController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetEventsSummary_ReturnsOkResult_WithData()
        {
            // Arrange
            var summary = new List<EventSummaryDto>
            {
                new EventSummaryDto { EventId = 1, Title = "Test Event", TicketsSold = 10, SharesCount = 5 }
            };

            _serviceMock.Setup(s => s.AnalyzeEventsAsync())
                        .ReturnsAsync(summary);

            // Act
            var result = await _controller.GetEventsSummary();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsType<List<EventSummaryDto>>(okResult.Value);
            Assert.Single(returnedData);
            Assert.Equal(1, returnedData[0].EventId);
        }

        [Fact]
        public async Task GetEventsSummary_ReturnsProblem_WhenExceptionThrown()
        {
            // Arrange
            _serviceMock.Setup(s => s.AnalyzeEventsAsync())
                        .ThrowsAsync(new Exception("Service failed"));

            // Act
            var result = await _controller.GetEventsSummary();

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);

            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("REAL ERROR", problemDetails.Title);
            Assert.Equal("Service failed", problemDetails.Detail);
        }
    }
}

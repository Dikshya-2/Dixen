using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Hall;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class HallControllerTest
    {
        private readonly Mock<IHallService> _hallServiceMock;
        private readonly HallController _controller;
        public HallControllerTest()
        {
            _hallServiceMock = new Mock<IHallService>();
            _controller = new HallController(_hallServiceMock.Object);
        }
        private HallResponse GetSampleHall(int id = 1)
        {
            return new HallResponse
            {
                Id = id,
                EventId = 1,
                Name = "Main Hall",
                Capacity = 100
            };
        }
        [Fact]
        public async Task GetAllHalls_ReturnsOkWithList()
        {
            // Arrange
            var halls = new List<HallResponse> { GetSampleHall(), GetSampleHall(2) };
            _hallServiceMock.Setup(s => s.GetAllHallsAsync()).ReturnsAsync(halls);

            // Act
            var result = await _controller.GetAllHalls();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<List<HallResponse>>(okResult.Value);
            Assert.Equal(2, value.Count);
        }
        [Fact]
        public async Task GetById_ReturnsOk_WhenHallExists()
        {
            // Arrange
            var hall = GetSampleHall();
            _hallServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(hall);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<HallResponse>(okResult.Value);
            Assert.Equal(1, value.Id);
            Assert.Equal("Main Hall", value.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenHallMissing()
        {
            // Arrange
            _hallServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((HallResponse)null);
            // Act
            var result = await _controller.GetById(99);
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new HallDto { EventId = 1, Name = "New Hall", Capacity = 50, VenueId=1 };
            var createdHall = GetSampleHall();
            _hallServiceMock.Setup(s => s.CreateHallAsync(dto.EventId, dto)).ReturnsAsync(createdHall);
            // Act
            var result = await _controller.Create(dto);
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<HallResponse>(createdResult.Value);
            Assert.Equal("Main Hall", value.Name);
        }

        [Fact]
        public async Task UpdateHall_ReturnsOk_WhenHallExists()
        {
            // Arrange
            var dto = new HallDto { EventId = 1, Name = "Updated Hall", Capacity = 120, VenueId=1 };
            var updatedHall = GetSampleHall();
            _hallServiceMock.Setup(s => s.UpdateHallAsync(1, dto)).ReturnsAsync(updatedHall);
            // Act
            var result = await _controller.UpdateHall(1, dto);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<HallResponse>(okResult.Value);
            Assert.Equal("Main Hall", value.Name);
        }

        [Fact]
        public async Task UpdateHall_ReturnsNotFound_WhenHallMissing()
        {
            // Arrange
            var dto = new HallDto { EventId = 1, Name = "NonExistent", Capacity = 50, VenueId=1 };
            _hallServiceMock.Setup(s => s.UpdateHallAsync(99, dto)).ReturnsAsync((HallResponse)null);
            // Act
            var result = await _controller.UpdateHall(99, dto);
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteHall_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            _hallServiceMock.Setup(s => s.DeleteHallAsync(1)).ReturnsAsync(true);
            // Act
            var result = await _controller.DeleteHall(1);
            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteHall_ReturnsNotFound_WhenHallMissing()
        {
            // Arrange
            _hallServiceMock.Setup(s => s.DeleteHallAsync(99)).ReturnsAsync(false);
            // Act
            var result = await _controller.DeleteHall(99);
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetHallsByEvent_ReturnsOkWithList()
        {
            // Arrange
            var halls = new List<HallResponse> { GetSampleHall(), GetSampleHall(2) };
            _hallServiceMock.Setup(s => s.GetHallsByEventAsync(1)).ReturnsAsync(halls);
            // Act
            var result = await _controller.GetHalls(1);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<List<HallResponse>>(okResult.Value);
            Assert.Equal(2, value.Count);
        }
    }
}

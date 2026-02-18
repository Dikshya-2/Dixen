using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Event;
using Dixen.Repo.DTOs.Filter;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ControllerTests
{
    public class EventControllerTest
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly EventController _controller;
        public EventControllerTest()
        {
            _eventServiceMock = new Mock<IEventService>();
            _controller = new EventController(_eventServiceMock.Object);
        }
        private EventResponseDto GetSampleEvent()
        {
            return new EventResponseDto
            {
                Id = 1,
                Title = "Sample Event",
                Description = "Description",
                StartTime = DateTime.Now
            };
        }
        [Fact]
        public async Task GetAll_ReturnsOk_WithEvents()
        {
            var events = new List<EventResponseDto> { GetSampleEvent() };
            _eventServiceMock.Setup(s => s.GetAllEventsAsync())
                             .ReturnsAsync(events);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<EventResponseDto>>(okResult.Value);
            Assert.Single(value);
            Assert.Equal("Sample Event", ((List<EventResponseDto>)value)[0].Title);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenEventExists()
        {
            var evt = GetSampleEvent();
            _eventServiceMock.Setup(s => s.GetEventByIdAsync(1))
                             .ReturnsAsync(evt);
            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<EventResponseDto>(okResult.Value);
            Assert.Equal("Sample Event", value.Title);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenEventMissing()
        {
            _eventServiceMock.Setup(s => s.GetEventByIdAsync(99))
                             .ReturnsAsync((EventResponseDto)null);
            var result = await _controller.GetById(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithEvent()
        {
            var dto = new CreateUpdateEventDto { Title = "New Event", Description = "Desc" };
            var createdEvent = new EventResponseDto { Id = 2, Title = dto.Title, Description = dto.Description };
            _eventServiceMock.Setup(s => s.CreateEventAsync(dto))
                             .ReturnsAsync(createdEvent);
            var result = await _controller.Create(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsType<EventResponseDto>(createdResult.Value);
            Assert.Equal(2, value.Id);
            Assert.Equal("New Event", value.Title);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(2, createdResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenEventExists()
        {
            var dto = new CreateUpdateEventDto { Title = "Updated", Description = "Updated Desc" };
            var updatedEvent = new EventResponseDto { Id = 1, Title = dto.Title, Description = dto.Description };
            _eventServiceMock.Setup(s => s.UpdateEventAsync(1, dto))
                             .ReturnsAsync(updatedEvent);
            var result = await _controller.Update(1, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<EventResponseDto>(okResult.Value);
            Assert.Equal("Updated", value.Title);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenEventMissing()
        {
            var dto = new CreateUpdateEventDto { Title = "DoesNotExist" };
            _eventServiceMock.Setup(s => s.UpdateEventAsync(99, dto))
                             .ReturnsAsync((EventResponseDto)null);
            var result = await _controller.Update(99, dto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            _eventServiceMock.Setup(s => s.DeleteEventAsync(1))
                             .ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEventMissing()
        {
            _eventServiceMock.Setup(s => s.DeleteEventAsync(99))
                             .ReturnsAsync(false);
            var result = await _controller.Delete(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOk_WithFilteredEvents()
        {
            var filter = new EventSearchFilterDto { Keyword = "Sample" };
            var events = new List<EventResponseDto> { GetSampleEvent() };
            _eventServiceMock.Setup(s => s.SearchEventsAsync(filter))
                             .ReturnsAsync(events);
            var result = await _controller.Search(filter);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<EventResponseDto>>(okResult.Value);
            Assert.Single(value);
            Assert.Equal("Sample Event", ((List<EventResponseDto>)value)[0].Title);
        }
    }
}

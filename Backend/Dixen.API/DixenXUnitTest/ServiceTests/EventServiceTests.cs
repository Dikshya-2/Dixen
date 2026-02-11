using Dixen.Repo.DTOs.Event;
using Dixen.Repo.DTOs.Filter;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ServiceTests
{
    public class EventServiceTests
    {
        private readonly Mock<IGRepo<Evnt>> _eventRepoMock = new();
        private readonly Mock<IGRepo<Category>> _categoryRepoMock = new();
        private readonly Mock<IGRepo<Hall>> _hallRepoMock = new();
        private readonly Mock<IGRepo<Booking>> _bookingRepoMock = new();

        private EventService CreateService()
            => new EventService(
                _eventRepoMock.Object,
                _categoryRepoMock.Object,
                _hallRepoMock.Object,
                _bookingRepoMock.Object);
        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnDto_WhenEventExists()
        {
            var evnt = new Evnt
            {
                Id = 1,
                Title = "Tech Event",
                Description = "Description",
                StartTime = DateTime.UtcNow,
                OrganizerId = 5,
                Organizer = new Organizer { OrganizationName = "Test Org" },
                Categories = new List<Category>(),
                Halls = new List<Hall>()
            };

            _eventRepoMock
                .Setup(r => r.GetById(
                    It.IsAny<object>(),
                    It.IsAny<Func<IQueryable<Evnt>, IIncludableQueryable<Evnt, object>>?>()))
                .ReturnsAsync(evnt);

            var service = CreateService();

            var result = await service.GetEventByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Tech Event", result!.Title);
        }
        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnNull_WhenEventNotFound()
        {
            _eventRepoMock
                .Setup(r => r.GetById(
                    It.IsAny<object>(),
                    It.IsAny<Func<IQueryable<Evnt>, IIncludableQueryable<Evnt, object>>?>()))
                .ReturnsAsync((Evnt?)null);

            var service = CreateService();

            var result = await service.GetEventByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnList()
        {
            var events = new List<Evnt>
            {
                new Evnt { Id = 1, Title = "Event1", Description="test", StartTime = DateTime.UtcNow, Categories = new List<Category>(), Halls = new List<Hall>() },
                new Evnt { Id = 2, Title = "Event2", Description = "test", StartTime = DateTime.UtcNow, Categories = new List<Category>(), Halls = new List<Hall>() }
            };

            _eventRepoMock
                .Setup(r => r.GetAll(It.IsAny<Func<IQueryable<Evnt>, IIncludableQueryable<Evnt, object>>?>()))
                .ReturnsAsync(events);

            var service = CreateService();

            var result = await service.GetAllEventsAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Title == "Event1");
        }

        [Fact]
        public async Task CreateEventAsync_ShouldReturnCreatedEvent()
        {
            var dto = new CreateUpdateEventDto
            {
                Title = "New Event",
                Description = "Desc",
                StartTime = DateTime.UtcNow,
                OrganizerId = 1,
                CategoryIds = new List<int>(),
                HallIds = new List<int>()
            };

            _eventRepoMock
                .Setup(r => r.Create(It.IsAny<Evnt>()))
                .ReturnsAsync((Evnt e) =>
                {
                    e.Id = 10;
                    e.Categories = new List<Category>();
                    e.Halls = new List<Hall>();
                    return e;
                });

            _eventRepoMock
                .Setup(r => r.GetById(
                    10,
                    It.IsAny<Func<IQueryable<Evnt>, IIncludableQueryable<Evnt, object>>?>()))
                .ReturnsAsync(new Evnt
                {
                    Id = 10,
                    Title = dto.Title,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    OrganizerId = dto.OrganizerId,
                    Categories = new List<Category>(),
                    Halls = new List<Hall>()
                });

            var service = CreateService();

            var result = await service.CreateEventAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("New Event", result.Title);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldReturnTrue_WhenEventExists()
        {
            var evnt = new Evnt { Id = 1,Title="test",Description="test", Categories = new List<Category>(), Halls = new List<Hall>() };

            _eventRepoMock
                .Setup(r => r.GetById(1, null))
                .ReturnsAsync(evnt);

            _eventRepoMock
                .Setup(r => r.Update(1, evnt))
                .ReturnsAsync(evnt);

            var service = CreateService();

            var result = await service.DeleteEventAsync(1);

            Assert.True(result);
            Assert.True(evnt.IsDeleted);
            _eventRepoMock.Verify(r => r.Update(1, evnt), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEvent()
        {
            var existingEvent = new Evnt
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Desc",
                Categories = new List<Category>(),
                Halls = new List<Hall>()
            };

            var dto = new CreateUpdateEventDto
            {
                Title = "New Title",
                Description = "Desc",
                StartTime = DateTime.UtcNow,
                OrganizerId = 2,
                CategoryIds = new List<int>(),
                HallIds = new List<int>()
            };

            _eventRepoMock
                .Setup(r => r.GetById(
                    1,
                    It.IsAny<Func<IQueryable<Evnt>, IIncludableQueryable<Evnt, object>>?>()))
                .ReturnsAsync(existingEvent);

            _eventRepoMock
                .Setup(r => r.Update(1, existingEvent))
                .ReturnsAsync(existingEvent);

            var service = CreateService();

            var result = await service.UpdateEventAsync(1, dto);

            Assert.NotNull(result);
            Assert.Equal("New Title", result!.Title);
        }

        //[Fact]
        //public async Task SearchEventsAsync_ShouldReturnFilteredEvents()
        //{
        //    var events = new List<Evnt>
        //    {
        //        new Evnt { Id = 1, Title = "Tech Conference", Description = "Desc", StartTime = DateTime.UtcNow, Categories = new List<Category>(), Halls = new List<Hall>() },
        //        new Evnt { Id = 2, Title = "Music Festival", Description = "Desc", StartTime = DateTime.UtcNow, Categories = new List<Category>(), Halls = new List<Hall>() }
        //    }.ToAsyncQueryable(); // Uses helper class!

        //    _eventRepoMock
        //        .Setup(r => r.GetAllQuery())
        //        .Returns(events);


        //    var service = CreateService();

        //    var filter = new EventSearchFilterDto
        //    {
        //        Title = "Tech"
        //    };

        //    var result = await service.SearchEventsAsync(filter);

        //    Assert.Single(result);
        //    Assert.Equal("Tech Conference", result.First().Title);
        //}


    }
}

using Dixen.Repo.DTOs.Filter;
using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ServiceTests
{
    public class EventServiceSearchTests
    {
        private readonly DatabaseContext _context;
        private readonly Mock<IGRepo<Category>> _categoryRepoMock = new();
        private readonly Mock<IGRepo<Hall>> _hallRepoMock = new();
        private readonly Mock<IGRepo<Booking>> _bookingRepoMock = new();
        private readonly EventService _service;

        public EventServiceSearchTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(options);

            var eventRepo = new GRepo<Evnt>(_context);
            _service = new EventService(eventRepo, _categoryRepoMock.Object,
                                       _hallRepoMock.Object, _bookingRepoMock.Object);
        }

        [Fact]
        public async Task SearchEventsAsync_ShouldReturnFilteredEvents()
        {
            // Arrange - Seed real data
            var techEvent = new Evnt
            {
                Id = 1,
                Title = "Tech Conference",
                Description = "Technology conference",
                StartTime = DateTime.UtcNow,
                Categories = new List<Category>(),
                Halls = new List<Hall>()
            };

            var musicEvent = new Evnt
            {
                Id = 2,
                Title = "Music Festival",
                Description = "Music event",
                StartTime = DateTime.UtcNow,
                Categories = new List<Category>(),
                Halls = new List<Hall>()
            };

            await _context.Set<Evnt>().AddRangeAsync(techEvent, musicEvent);
            await _context.SaveChangesAsync();

            // Act
            var filter = new EventSearchFilterDto { Title = "Tech" };
            var result = await _service.SearchEventsAsync(filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("Tech Conference", result.First().Title);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

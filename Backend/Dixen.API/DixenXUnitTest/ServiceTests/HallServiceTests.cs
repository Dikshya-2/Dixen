using Dixen.Repo.DTOs.Hall;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ServiceTests
{
    public class HallServiceTests
    {
        private readonly Mock<IGRepo<Hall>> _hallRepoMock;
        private readonly Mock<IGRepo<Evnt>> _eventRepoMock;
        private readonly Mock<IGRepo<Venue>> _venueRepoMock;
        private readonly HallService _hallService;

        public HallServiceTests()
        {
            _hallRepoMock = new Mock<IGRepo<Hall>>();
            _eventRepoMock = new Mock<IGRepo<Evnt>>();
            _venueRepoMock = new Mock<IGRepo<Venue>>();

            _hallService = new HallService(_hallRepoMock.Object, _eventRepoMock.Object, _venueRepoMock.Object);
        }

        [Fact]
        public async Task GetAllHallsAsync_ShouldReturnMappedHalls()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall { Id = 1, Name = "Hall A", Capacity = 100, EventId = 1, VenueId = 1 },
                new Hall { Id = 2, Name = "Hall B", Capacity = 200, EventId = 2, VenueId = 2 }
            };
            _hallRepoMock.Setup(r => r.GetAll(null)).ReturnsAsync(halls);

            // Act
            var result = await _hallService.GetAllHallsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Hall A", result[0].Name);
            Assert.Equal(200, result[1].Capacity);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnHall_WhenExists()
        {
            // Arrange
            var hall = new Hall { Id = 1, Name = "Hall A", Capacity = 100 };
            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(hall);

            // Act
            var result = await _hallService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Hall A", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Hall?)null);

            // Act
            var result = await _hallService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetHallsByEventAsync_ShouldReturnHallsFilteredByEvent()
        {
            // Arrange
            var halls = new List<Hall>
            {
                new Hall { Id = 1, EventId = 1, Name = "Hall A" },
                new Hall { Id = 2, EventId = 2, Name = "Hall B" }
            };
            _hallRepoMock.Setup(r => r.Find(h => h.EventId == 1, null))
                         .ReturnsAsync(halls.Where(h => h.EventId == 1));

            // Act
            var result = await _hallService.GetHallsByEventAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal("Hall A", result[0].Name);
        }

        [Fact]
        public async Task CreateHallAsync_ShouldReturnCreatedHall()
        {
            // Arrange
            var dto = new HallDto { Name = "New Hall", EventId=1, Capacity = 150, VenueId = 1 };
            var evt = new Evnt { Id = 1, Title = "Event A" , Description="hhh"};
            _eventRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(evt);

            _hallRepoMock.Setup(r => r.Create(It.IsAny<Hall>()))
                         .ReturnsAsync((Hall h) =>
                         {
                             h.Id = 10; // simulate DB auto-generated ID
                             return h;
                         });

            // Act
            var result = await _hallService.CreateHallAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New Hall", result.Name);
            Assert.Equal(150, result.Capacity);
        }

        [Fact]
        public async Task CreateHallAsync_ShouldThrow_WhenEventNotFound()
        {
            // Arrange
            var dto = new HallDto { Name = "New Hall", EventId=1, Capacity = 150, VenueId = 1 };
            _eventRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Evnt?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _hallService.CreateHallAsync(1, dto));
        }

        [Fact]
        public async Task UpdateHallAsync_ShouldReturnUpdatedHall_WhenValid()
        {
            // Arrange
            var existingHall = new Hall { Id = 1, Name = "Old Hall", Capacity = 100, EventId = 1, VenueId = 1 };
            var venue = new Venue { Id = 2, Name = "New Venue" };
            var dto = new HallDto { Name = "Updated Hall", Capacity = 150, EventId = 1, VenueId = 2 };

            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(existingHall);
            _venueRepoMock.Setup(r => r.GetById(2, null)).ReturnsAsync(venue);
            _hallRepoMock
                .Setup(r => r.Update(It.IsAny<object>(), It.IsAny<Hall>()))
                .ReturnsAsync((object id, Hall h) => h);

            // Act
            var result = await _hallService.UpdateHallAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Hall", result!.Name);
            Assert.Equal(150, result.Capacity);
            Assert.Equal(2, result.VenueId);
        }

        [Fact]
        public async Task UpdateHallAsync_ShouldReturnNull_WhenHallNotFound()
        {
            // Arrange
            var dto = new HallDto { Name = "Updated Hall", Capacity = 150, EventId = 1, VenueId = 2 };
            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Hall?)null);

            // Act
            var result = await _hallService.UpdateHallAsync(1, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateHallAsync_ShouldReturnNull_WhenVenueNotFound()
        {
            // Arrange
            var existingHall = new Hall { Id = 1, Name = "Old Hall", Capacity = 100, EventId = 1, VenueId = 1 };
            var dto = new HallDto { Name = "Updated Hall", Capacity = 150, EventId = 1, VenueId = 2 };

            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(existingHall);
            _venueRepoMock.Setup(r => r.GetById(2, null)).ReturnsAsync((Venue?)null);

            // Act
            var result = await _hallService.UpdateHallAsync(1, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteHallAsync_ShouldReturnTrue_WhenHallExists()
        {
            // Arrange
            var hall = new Hall { Id = 1, Name = "Hall A" };
            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(hall);
            _hallRepoMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            // Act
            var result = await _hallService.DeleteHallAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteHallAsync_ShouldReturnFalse_WhenHallDoesNotExist()
        {
            // Arrange
            _hallRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Hall?)null);

            // Act
            var result = await _hallService.DeleteHallAsync(1);

            // Assert
            Assert.False(result);
        }

    }
}

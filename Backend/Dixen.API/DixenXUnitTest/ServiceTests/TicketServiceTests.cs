using Dixen.Repo.DTOs.Ticket;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Model.Enums;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DixenXUnitTest.ServiceTests
{
    public class TicketServiceTests
    {
        private readonly Mock<IGRepo<Ticket>> _ticketRepoMock;
        private readonly Mock<IGRepo<Booking>> _bookingRepoMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketRepoMock = new Mock<IGRepo<Ticket>>();
            _bookingRepoMock = new Mock<IGRepo<Booking>>();
            _ticketService = new TicketService(_ticketRepoMock.Object, _bookingRepoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTicket_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                UserId = "123" 
            };

            var dto = new TicketDto
            {
                Type = TicketType.VIP, // enum
                Price = 100,
                Quantity = 2
            };

            _bookingRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(booking);
            _ticketRepoMock.Setup(r => r.Create(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) =>
                {
                    t.Id = 2222222; // simulate DB-generated ID
                    return t;
                });

            // Act
            var result = await _ticketService.CreateAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2222222, result.Id);
            Assert.Equal(TicketType.VIP, result.Type);
            Assert.Equal(100, result.Price);
            Assert.Equal(2, result.Quantity);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenBookingNotFound()
        {
            // Arrange
            var dto = new TicketDto { Type = TicketType.VIP, Price = 100, Quantity = 2 };
            _bookingRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Booking?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _ticketService.CreateAsync(1, dto));
        }

        [Fact]
        public async Task GetByBookingAsync_ShouldReturnTickets()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket { Id = 1, BookingId = 1, Type = TicketType.VIP, Price = 100, Quantity = 2 },
                new Ticket { Id = 2, BookingId = 1, Type = TicketType.Student, Price = 50, Quantity = 1 }
            };
            _ticketRepoMock.Setup(r => r.Find(t => t.BookingId == 1, null))
                .ReturnsAsync(tickets);

            // Act
            var result = await _ticketService.GetByBookingAsync(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Type == TicketType.VIP);
            Assert.Contains(result, t => t.Type == TicketType.Student);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTicket_WhenExists()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Type = TicketType.VIP, Price = 100, Quantity = 2 };
            _ticketRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TicketType.VIP, result!.Type);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _ticketRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Ticket?)null);

            // Act
            var result = await _ticketService.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedTicket_WhenExists()
        {
            // Arrange
            var ticket = new Ticket { Id = 1, Type = TicketType.VIP, Price = 100, Quantity = 2 };
            var dto = new TicketDto { Type = TicketType.General, Price = 50, Quantity = 1 };

            _ticketRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync(ticket);
            _ticketRepoMock.Setup(r => r.Update(It.IsAny<object>(), It.IsAny<Ticket>()))
                .ReturnsAsync((object id, Ticket t) => t);

            // Act
            var result = await _ticketService.UpdateAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TicketType.General, result!.Type);
            Assert.Equal(50, result.Price);
            Assert.Equal(1, result.Quantity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenTicketNotFound()
        {
            // Arrange
            var dto = new TicketDto { Type = TicketType.General, Price = 50, Quantity = 1 };
            _ticketRepoMock.Setup(r => r.GetById(1, null)).ReturnsAsync((Ticket?)null);

            // Act
            var result = await _ticketService.UpdateAsync(1, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            _ticketRepoMock.Setup(r => r.Delete(1)).ReturnsAsync(true);

            // Act
            var result = await _ticketService.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotDeleted()
        {
            // Arrange
            _ticketRepoMock.Setup(r => r.Delete(1)).ReturnsAsync(false);

            // Act
            var result = await _ticketService.DeleteAsync(1);

            // Assert
            Assert.False(result);
        }
    }
}

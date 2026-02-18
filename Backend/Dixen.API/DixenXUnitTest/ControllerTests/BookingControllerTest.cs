using Dixen.API.Controllers;
using Dixen.Repo.DTOs.Booking;
using Dixen.Repo.DTOs.EventAttendance;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ControllerTests
{
    public class BookingControllerTest
    {
        private readonly Mock<IBookingService> _serviceMock;
        private readonly BookingController _controller;

        public BookingControllerTest()
        { 
            _serviceMock = new Mock<IBookingService>();
            _controller = new BookingController(_serviceMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Email, "test@test.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateBooking_ReturnsOk_WithBooking()
        {
            // Arrange
            var booking = new BookingDto { Id = 1, EventId = 42 };

            // Mock the service
            _serviceMock.Setup(s => s.CreateBookingAsync(42, "test@test.com"))
                        .ReturnsAsync(booking);

            // Mock the User claims
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Email, "test@test.com")
                    }, "mock"))
                }
            };

            // Act
            var result = await _controller.CreateBooking(42);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(42, returnedBooking.EventId);
        }

        [Fact]
        public async Task GetMyBookings_ReturnsOk_WithList()
        {
            // Arrange
            var bookings = new List<BookingDto>
        {
            new BookingDto { Id = 1, EventId = 42 }
        };
            _serviceMock.Setup(s => s.GetUserBookingsAsync("test@test.com"))
                        .ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetMyBookings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Single(returnedList);
        }

        [Fact]
        public async Task CancelBooking_ReturnsNoContent_WhenDeleted()
        {
            _serviceMock.Setup(s => s.CancelBookingAsync(1)).ReturnsAsync(true);
            var result = await _controller.CancelBooking(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CancelBooking_ReturnsNotFound_WhenNotDeleted()
        {
            _serviceMock.Setup(s => s.CancelBookingAsync(1)).ReturnsAsync(false);
            var result = await _controller.CancelBooking(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetEventAvailability_ReturnsOk_WithList()
        {
            var availability = new List<HallCapacityDto>
            {
                new HallCapacityDto { HallId = 1, Capacity = 50 }
            };
            _serviceMock.Setup(s => s.GetEventAvailabilityAsync(42))
                        .ReturnsAsync(availability);

            var result = await _controller.GetEventAvailability(42);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsType<List<HallCapacityDto>>(okResult.Value);
            Assert.Single(returnedList);
        }
    }
}

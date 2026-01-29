using Dixen.Repo.DTOs.Booking;
using Dixen.Repo.DTOs.EventAttendance;
using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Services;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpPost("{eventId}")]
        public async Task<ActionResult<BookingDto>> CreateBooking(int eventId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("No email in token");

            var booking = await _bookingService.CreateBookingAsync(eventId, userEmail);
            return Ok(booking);
        }
        [HttpGet("all")]
        public async Task<ActionResult<List<BookingDto>>> GetMyBookings()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("No email in token");

            var bookings = await _bookingService.GetUserBookingsAsync(userEmail);
            return Ok(bookings);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var deleted = await _bookingService.CancelBookingAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        [HttpGet("availability/{eventId}")]
        public async Task<ActionResult<List<HallCapacityDto>>> GetEventAvailability(int eventId)
        {
            var availability = await _bookingService.GetEventAvailabilityAsync(eventId);
            return Ok(availability);
        }

    }
}

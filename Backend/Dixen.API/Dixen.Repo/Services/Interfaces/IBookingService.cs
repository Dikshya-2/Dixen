using Dixen.Repo.DTOs.Booking;
using Dixen.Repo.DTOs.EventAttendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Create a booking for a user for a specific event.
        /// Throws exception if user not found, event not found, or no seats available.
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <param name="userEmail">Email of the user booking</param>
        /// <returns>Details of the booking</returns>
        Task<BookingDto> CreateBookingAsync(int eventId, string userEmail);

        /// <summary>
        /// Get all bookings for a user
        /// </summary>
        /// <param name="userEmail">User's email</param>
        /// <returns>List of bookings</returns>
        Task<List<BookingDto>> GetUserBookingsAsync(string userEmail);

        /// <summary>
        /// Cancel a booking by booking ID
        /// </summary>
        /// <param name="bookingId">ID of the booking</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> CancelBookingAsync(int bookingId);

        /// <summary>
        /// Check available seats for an event across its halls
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>Number of seats available per hall</returns>
        Task<List<HallCapacityDto>> GetEventAvailabilityAsync(int eventId);
    }
}

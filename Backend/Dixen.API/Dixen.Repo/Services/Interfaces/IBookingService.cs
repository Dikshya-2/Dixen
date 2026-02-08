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
        Task<BookingDto> CreateBookingAsync(int eventId, string userEmail);
        Task<List<BookingDto>> GetUserBookingsAsync(string userEmail);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<List<HallCapacityDto>> GetEventAvailabilityAsync(int eventId);
    }
}

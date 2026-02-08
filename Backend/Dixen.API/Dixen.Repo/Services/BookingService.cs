using Dixen.Repo.DTOs.Booking;
using Dixen.Repo.DTOs.EventAttendance;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dixen.Repo.Services
{
    public class BookingService : IBookingService
    {
        private readonly IGRepo<Booking> _bookingRepo;
        private readonly IGRepo<Evnt> _eventRepo;
        private readonly IGRepo<Hall> _hallRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BookingService(
            IGRepo<Booking> bookingRepo,
            IGRepo<Evnt> eventRepo,
            IGRepo<Hall> hallRepo,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _bookingRepo = bookingRepo;
            _eventRepo = eventRepo;
            _hallRepo = hallRepo;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task<BookingDto> CreateBookingAsync(int eventId, string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail)
                       ?? throw new Exception("User not found");

            var evt = await _eventRepo.GetById(eventId, q => q.Include(e => e.Halls))
                       ?? throw new Exception("Event not found");

            Hall hall = null!;
            foreach (var h in evt.Halls)
            {
                var bookings = await _bookingRepo.Find(b => b.EventId == evt.Id && b.HallId == h.Id,q => q.Include(b => b.Tickets));
                var currentBookings = bookings.Sum(b => b.Tickets?.Sum(t => t.Quantity) ?? 0);

                if (currentBookings < h.Capacity)
                {
                    hall = h;
                    break;
                }
            }

            if (hall == null)
                throw new Exception("No available seats in any hall");

            var booking = new Booking
            {
                EventId = evt.Id,
                HallId = hall.Id,
                UserId = user.Id,
                BookedTime = DateTime.UtcNow
            };

            await _bookingRepo.Create(booking);

            _ = SendConfirmationEmailAsync(user, evt, hall, booking);

            return new BookingDto
            {
                Id = booking.Id,
                EventId = evt.Id,
                EventTitle = evt.Title,
                HallId = hall.Id,
                HallName = hall.Name,
                UserId = user.Id,
                UserName = user.FullName,
                BookedTime = booking.BookedTime
            };
        }
        public async Task<List<BookingDto>> GetUserBookingsAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail)
                       ?? throw new Exception("User not found");

            var bookings = await _bookingRepo.Find(
                b => b.UserId == user.Id,
                q => q.Include(b => b.Event).Include(b => b.Hall));

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                EventId = b.EventId,
                EventTitle = b.Event.Title,
                HallId = b.HallId,
                HallName = b.Hall.Name,
                UserId = user.Id,
                UserName = user.FullName,
                BookedTime = b.BookedTime
            }).ToList();
        }
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetById(bookingId);
            if (booking == null) return false;

            await _bookingRepo.Delete(bookingId);
            return true;
        }
        public async Task<List<HallCapacityDto>> GetEventAvailabilityAsync(int eventId)
        {
            var evt = await _eventRepo.GetById(eventId, q => q.Include(e => e.Halls))
                       ?? throw new Exception("Event not found");

            var hallAvailability = new List<HallCapacityDto>();

            foreach (var hall in evt.Halls)
            {
                var bookings = await _bookingRepo.Find(
            b => b.EventId == eventId && b.HallId == hall.Id,
            q => q.Include(b => b.Tickets)
            );
                var count = bookings.Sum(b => b.Tickets?.Sum(t => t.Quantity) ?? 0);

                hallAvailability.Add(new HallCapacityDto
                {
                    HallId = hall.Id,
                    HallName = hall.Name,
                    Capacity = hall.Capacity,
                    CurrentAttendance = count,
                    SeatsAvailable = hall.Capacity - count
                });
            }

            return hallAvailability;
        }
        private async Task SendConfirmationEmailAsync(ApplicationUser user, Evnt evt, Hall hall, Booking booking)
        {
            try
            {
                var html = $@"
                    <h2> Booking Confirmed</h2>
                    <p>Event: {evt.Title}</p>
                    <p>Hall: {hall.Name}</p>
                    <p>Date: {evt.StartTime:MMM dd, yyyy HH:mm}</p>
                    <p>Booked By: {user.FullName}</p>
                    <p>Booking Time: {booking.BookedTime:HH:mm}</p>";

                await _emailSender.SendEmailAsync(user.Email!, "Booking Confirmed", html);
            }
            catch
            {
                
            }
        }
    }
}

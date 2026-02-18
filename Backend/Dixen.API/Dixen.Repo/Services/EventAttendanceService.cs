//using Dixen.Repo.DTOs.EventAttendance;
//using Dixen.Repo.Model.Entities;
//using Dixen.Repo.Repositories.Interfaces;
//using Dixen.Repo.Services.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace Dixen.Repo.Services
//{
//    public class EventAttendanceService : IEventAttendanceService
//    {
//        private readonly IGRepo<EventAttendance> _attendanceRepo;
//        private readonly IGRepo<Evnt> _eventRepo;
//        private readonly IGRepo<Hall> _hallRepo;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IEmailSender _emailSender;

//        public EventAttendanceService(
//            IGRepo<EventAttendance> attendanceRepo,
//            IGRepo<Evnt> eventRepo,
//            IGRepo<Hall> hallRepo,
//            UserManager<ApplicationUser> userManager,
//            IEmailSender emailSender)
//        {
//            _attendanceRepo = attendanceRepo;
//            _eventRepo = eventRepo;
//            _hallRepo = hallRepo;
//            _userManager = userManager;
//            _emailSender = emailSender;
//        }

//        // -------------------------------
//        // REGISTER ATTENDANCE
//        // -------------------------------
//        public async Task<AttendanceResponse> RegisterAttendanceAsync(
//            int eventId,
//            int hallId,
//            string userEmail)
//        {
//            var user = await _userManager.FindByEmailAsync(userEmail)
//                ?? throw new Exception("User not found");

//            var evt = await _eventRepo.GetById(eventId)
//                ?? throw new Exception("Event not found");

//            if (evt.StartTime < DateTime.UtcNow)
//                throw new Exception("Event has already started");

//            var hall = await _hallRepo.GetById(hallId)
//                ?? throw new Exception("Hall not found");

//            var currentAttendance = (await _attendanceRepo.Find(
//                ea => ea.EventId == eventId && ea.HallId == hallId)).Count();

//            if (currentAttendance >= hall.Capacity)
//                throw new Exception($"Hall '{hall.Name}' is full");

//            var alreadyRegistered = (await _attendanceRepo.Find(
//                ea => ea.EventId == eventId &&
//                      ea.HallId == hallId &&
//                      ea.UserId == user.Id)).Any();

//            if (alreadyRegistered)
//                throw new Exception("Already registered for this hall");

//            var attendance = new EventAttendance
//            {
//                EventId = eventId,
//                HallId = hallId,
//                UserId = user.Id,
//                AttendedDate = DateTime.UtcNow
//            };

//            await _attendanceRepo.Create(attendance);

//            _ = SendConfirmationEmailAsync(user, evt, hall, attendance);

//            return new AttendanceResponse
//            {
//                EventId = eventId,
//                HallId = hallId,
//                RegisteredAt = attendance.AttendedDate,
//                SeatsAvailable = hall.Capacity - currentAttendance - 1,
//                Message = $"Registered for '{evt.Title}' in '{hall.Name}'!"
//            };
//        }

//        // -------------------------------
//        // MY ATTENDANCES
//        // -------------------------------
//        public async Task<List<MyAttendanceDto>> GetMyAttendancesAsync(string userEmail)
//        {
//            var user = await _userManager.FindByEmailAsync(userEmail)
//                ?? throw new Exception("User not found");

//            var attendances = await _attendanceRepo.Find(
//                ea => ea.UserId == user.Id,
//                q => q.Include(ea => ea.Event)
//                      .Include(ea => ea.Hall));

//            return attendances.Select(ea => new MyAttendanceDto
//            {
//                EventId = ea.EventId,
//                EventTitle = ea.Event.Title,
//                HallId = ea.HallId,
//                HallName = ea.Hall.Name,
//                EventStartTime = ea.Event.StartTime,
//                RegisteredAt = ea.AttendedDate
//            }).ToList();
//        }

//        // -------------------------------
//        // HALL CAPACITY
//        // -------------------------------
//        public async Task<HallCapacityDto> GetHallCapacityAsync(int eventId, int hallId)
//        {
//            var hall = await _hallRepo.GetById(hallId)
//                ?? throw new Exception("Hall not found");

//            var count = (await _attendanceRepo.Find(
//                ea => ea.EventId == eventId && ea.HallId == hallId)).Count();

//            return new HallCapacityDto
//            {
//                HallId = hall.Id,
//                HallName = hall.Name,
//                Capacity = hall.Capacity,
//                CurrentAttendance = count,
//                SeatsAvailable = hall.Capacity - count
//            };
//        }
//        private async Task SendConfirmationEmailAsync(
//            ApplicationUser user,
//            Evnt evt,
//            Hall hall,
//            EventAttendance attendance)
//        {
//            try
//            {
//                var html = $@"
//                <h2>✅ Attendance Confirmed</h2>
//                <p><b>Event:</b> {evt.Title}</p>
//                <p><b>Hall:</b> {hall.Name}</p>
//                <p><b>Date:</b> {evt.StartTime:MMM dd, yyyy HH:mm}</p>
//                <p><b>Registered:</b> {attendance.AttendedDate:HH:mm}</p>";

//                await _emailSender.SendEmailAsync(
//                    user.Email!,
//                    "Attendance Confirmed",
//                    html);
//            }
//            catch
//            {
//                // TODO: log error
//            }
//        }
//    }
//}

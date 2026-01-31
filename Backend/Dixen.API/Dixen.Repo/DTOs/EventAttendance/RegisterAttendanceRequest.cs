using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.EventAttendance
{
    public class RegisterAttendanceRequest
    {
        public int EventId { get; set; }
        public int HallId { get; set; }
    }

    // Response DTOs (Output)
    public class AttendanceResponse
    {
        public string Message { get; set; } = string.Empty;
        public int EventId { get; set; }
        public int HallId { get; set; }
        public int SeatsAvailable { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class HallCapacityDto
    {
        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CurrentAttendance { get; set; }
        public int SeatsAvailable { get; set; }
    }

    public class MyAttendanceDto
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public DateTime EventStartTime { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}

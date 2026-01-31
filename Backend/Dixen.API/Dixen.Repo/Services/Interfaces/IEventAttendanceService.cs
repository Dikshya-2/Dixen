using Dixen.Repo.DTOs.EventAttendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IEventAttendanceService
    {
        Task<AttendanceResponse> RegisterAttendanceAsync( int eventId, int hallId, string userEmail);
        Task<List<MyAttendanceDto>> GetMyAttendancesAsync(string userEmail);
        Task<HallCapacityDto> GetHallCapacityAsync(int eventId, int hallId);


    }
}
 


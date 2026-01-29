using Dixen.Repo.DTOs.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(int eventId);
        Task<EventResponseDto> CreateEventAsync(CreateUpdateEventDto dto);
        Task<EventResponseDto?> UpdateEventAsync(int eventId, CreateUpdateEventDto dto);
        Task<bool> DeleteEventAsync(int eventId);
    }


}

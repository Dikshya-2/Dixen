using Dixen.Repo.DTOs.Hall;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dixen.Repo.Services
{
    public class HallService : IHallService
    {
        private readonly IGRepo<Hall> _hallRepo;
        private readonly IGRepo<Evnt> _eventRepo;
        private readonly IGRepo<Venue> _venueRepo;
        public HallService(IGRepo<Hall> hallRepo, IGRepo<Evnt> eventRepo, IGRepo<Venue> venueRepo) 
        {
            _hallRepo = hallRepo;
            _eventRepo = eventRepo;
            _venueRepo = venueRepo;
        }
        public async Task<List<HallResponse>> GetAllHallsAsync()
        {
            var halls = await _hallRepo.GetAll();
            return halls.Select(MapToResponse).ToList();
        }
        public async Task<HallResponse?> GetByIdAsync(int id)
        {
            var hall = await _hallRepo.GetById(id);
            return hall == null ? null : MapToResponse(hall);
        }
        public async Task<List<HallResponse>> GetHallsByEventAsync(int eventId)
        {
            var halls = await _hallRepo.Find(h => h.EventId == eventId);
            return halls.Select(MapToResponse).ToList();
        }
        public async Task<HallResponse> CreateHallAsync(int eventId, HallDto dto)
        {
            var evt = await _eventRepo.GetById(eventId);
            if (evt == null) throw new Exception("Event not found");

            var hall = new Hall
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                VenueId = dto.VenueId,
                EventId = eventId
            };

            await _hallRepo.Create(hall);

            return new HallResponse
            {
                Id = hall.Id,
                Name = hall.Name,
                Capacity = hall.Capacity,
                VenueId = hall.VenueId,
                EventId = hall.EventId
            };
        }
        public async Task<HallResponse?> UpdateHallAsync(int hallId, HallDto dto)
        {
            var hall = await _hallRepo.GetById(hallId);
            if (hall == null) return null;

            var venue = await _venueRepo.GetById(dto.VenueId);  
            if (venue == null) return null; 

            var updateData = new Hall
            {
                Id = hallId,
                Name = dto.Name,
                Capacity = dto.Capacity,
                EventId = dto.EventId,
                VenueId = dto.VenueId
            };

            var updatedHall = await _hallRepo.Update(hallId, updateData);
            return updatedHall != null ? MapToResponse(updatedHall) : null;
        }
        public async Task<bool> DeleteHallAsync(int hallId)
        {
            var hall = await _hallRepo.GetById(hallId);
            if (hall == null) return false;

            await _hallRepo.Delete(hallId);
            return true;
        }
        private HallResponse MapToResponse(Hall h) => new()
        {
            Id = h.Id,
            Name = h.Name,
            Capacity = h.Capacity,
            VenueId = h.VenueId,
            EventId = h.EventId
        };
    }
}


using Dixen.Repo.DTOs.Event;
using Dixen.Repo.DTOs.Filter;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dixen.Repo.Services
{
    public class EventService : IEventService
    {
        private readonly IGRepo<Evnt> _eventRepo;
        private readonly IGRepo<Category> _categoryRepo;
        private readonly IGRepo<Hall> _hallRepo;
        private readonly IGRepo<Booking> _bookingRepo;

        public EventService(
            IGRepo<Evnt> eventRepo,
            IGRepo<Category> categoryRepo,
            IGRepo<Hall> hallRepo,
            IGRepo<Booking> bookingRepo)
        {
            _eventRepo = eventRepo;
            _categoryRepo = categoryRepo;
            _hallRepo = hallRepo;
            _bookingRepo = bookingRepo;
        }
        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepo.GetAll(q => q
                .Include(e => e.Categories)
                .Include(e => e.Halls)
                    .ThenInclude(h => h.Venue)
                .Include(e => e.Organizer));

            return events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title ?? string.Empty,
                Description = e.Description ?? string.Empty,
                StartTime = e.StartTime,
                ImageUrl = e.ImageUrl,
                OrganizerId = e.OrganizerId,
                OrganizerName = e.Organizer?.OrganizationName ?? string.Empty,
                CategoryNames = e.Categories?
                    .Select(c => c.Name ?? string.Empty)
                    .ToList()
                    ?? new List<string>(),
                HallNames = e.Halls?
                    .Select(h => h.Name ?? string.Empty)
                    .ToList()
                    ?? new List<string>()
            }).ToList();
        }
        public async Task<EventResponseDto?> GetEventByIdAsync(int eventId)
        {
            var evt = await _eventRepo.GetById(eventId, q => q
                .Include(e => e.Categories)
                .Include(e => e.Halls)
                    .ThenInclude(h => h.Venue)
                .Include(e => e.Organizer));

            if (evt == null) return null;

            return new EventResponseDto
            {
                Id = evt.Id,
                Title = evt.Title ?? string.Empty,
                Description = evt.Description ?? string.Empty,
                StartTime = evt.StartTime,
                ImageUrl = evt.ImageUrl,
                OrganizerId = evt.OrganizerId,
                OrganizerName = evt.Organizer?.OrganizationName ?? string.Empty,
                CategoryNames = evt.Categories?
                    .Select(c => c.Name ?? string.Empty)
                    .ToList()
                    ?? new List<string>(),
                HallNames = evt.Halls?
                    .Select(h => h.Name ?? string.Empty)
                    .ToList()
                    ?? new List<string>(),
                Name = evt.Halls?.FirstOrDefault()?.Venue?.Name,
                Address = evt.Halls?.FirstOrDefault()?.Venue?.Address,
                City = evt.Halls?.FirstOrDefault()?.Venue?.City
            };
        }
        public async Task<EventResponseDto> CreateEventAsync(CreateUpdateEventDto dto)
        {
            var categories = dto.CategoryIds.Any()
                ? (await _categoryRepo.Find(c => dto.CategoryIds.Contains(c.Id))).ToList()
                : new List<Category>();

            var halls = dto.HallIds.Any()
                ? (await _hallRepo.Find(h => dto.HallIds.Contains(h.Id))).ToList()
                : new List<Hall>();

            var evt = new Evnt
            {
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                ImageUrl = dto.ImageUrl,
                OrganizerId = dto.OrganizerId,
                Categories = categories,
                Halls = halls
            };

            await _eventRepo.Create(evt);

            return (await GetEventByIdAsync(evt.Id))!;
        }

        public async Task<EventResponseDto?> UpdateEventAsync(int eventId, CreateUpdateEventDto dto)
        {
            var evt = await _eventRepo.GetById(eventId, q => q
                .Include(e => e.Categories)
                .Include(e => e.Halls));

            if (evt == null) return null;

            evt.Title = dto.Title;
            evt.Description = dto.Description;
            evt.StartTime = dto.StartTime;
            evt.ImageUrl = dto.ImageUrl;
            evt.OrganizerId = dto.OrganizerId;
            evt.Categories.Clear();
            if (dto.CategoryIds.Any())
            {
                var categories = await _categoryRepo.Find(c => dto.CategoryIds.Contains(c.Id));
                evt.Categories.AddRange(categories);
            }

            evt.Halls.Clear();
            if (dto.HallIds.Any())
            {
                var halls = await _hallRepo.Find(h => dto.HallIds.Contains(h.Id));
                evt.Halls.AddRange(halls);
            }
            await _eventRepo.Update(eventId, evt);
            return await GetEventByIdAsync(eventId);
        }
        public async Task<bool> DeleteEventAsync(int eventId)
        {
            var evt = await _eventRepo.GetById(eventId, null);
            if (evt == null) return false;

            evt.IsDeleted = true;
            await _eventRepo.Update(eventId, evt);

            return true;
        }
        public async Task<List<EventResponseDto>> SearchEventsAsync(EventSearchFilterDto filter)
        {
            IQueryable<Evnt> query = _eventRepo
                .GetAllQuery()
                .AsNoTracking()
                .Include(e => e.Categories)
                .Include(e => e.Halls)
                    .ThenInclude(h => h.Venue);

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var titleLower = filter.Title.ToLower();
                query = query.Where(e => e.Title.ToLower().StartsWith(titleLower)); //Contains for all letter
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keywordLower = filter.Keyword.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(keywordLower)
                                      || e.Description.ToLower().Contains(keywordLower));
            }

            if (filter.OrganizerId.HasValue)
                query = query.Where(e => e.OrganizerId == filter.OrganizerId.Value);

            if (filter.CategoryIds != null && filter.CategoryIds.Count > 0)
                query = query.Where(e => e.Categories.Any(c => filter.CategoryIds.Contains(c.Id)));

            if (!string.IsNullOrWhiteSpace(filter.VenueCity))
            {
                var cityLower = filter.VenueCity.ToLower();
                query = query.Where(e => e.Halls.Any(h => h.Venue.City.ToLower() == cityLower));
            }

            if (filter.EventDate.HasValue)
            {
                var dateUtc = filter.EventDate.Value.Date.ToUniversalTime();
                var nextDayUtc = dateUtc.AddDays(1);
                query = query.Where(e => e.StartTime >= dateUtc && e.StartTime < nextDayUtc);
            }

            query = query.OrderBy(e => e.Title);

            var events = await query.ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        //public async Task<List<EventResponseDto>> SearchEventsAsync(EventSearchFilterDto filter)
        //{
        //    var query = _eventRepo.GetAllQuery()
        //        .AsNoTracking()                    // 🔥 LINE 1: +50% faster
        //        .Include(e => e.Categories)
        //        .Include(e => e.Halls)
        //        .ThenInclude(h => h.Venue);

        //    // YOUR EXISTING FILTERS (unchanged)...
        //    if (!string.IsNullOrWhiteSpace(filter.Title))
        //        query = query.Where(e => e.Title.ToLower().StartsWith(filter.Title.ToLower()));

        //    if (!string.IsNullOrWhiteSpace(filter.Keyword))
        //        query = query.Where(e => e.Title.ToLower().Contains(filter.Keyword.ToLower())
        //                              || e.Description.ToLower().Contains(filter.Keyword.ToLower()));


        //    query = query.OrderBy(e => e.Title);

        //    var events = await query
        //        .Take(20)                             
        //        .Select(e => new EventResponseDto     
        //        {
        //            Id = e.Id,
        //            Title = e.Title,
        //            StartTime = e.StartTime,
        //            ImageUrl = e.ImageUrl,
        //            Address = e.Halls.FirstOrDefault().Venue.Address,
        //            City = e.Halls.FirstOrDefault().Venue.City,
        //            CategoryNames = e.Categories.Select(c => c.Name).ToList()
        //        })
        //        .ToListAsync();

        //    return events;
        //}

        private EventResponseDto MapToDto(Evnt e)
        {
            var venue = e.Halls?.FirstOrDefault()?.Venue;

            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title ?? string.Empty,
                StartTime = e.StartTime,
                ImageUrl = e.ImageUrl,
                Address = venue?.Address,
                City = venue?.City,
                CategoryNames = e.Categories?.Select(c => c.Name).ToList() ?? new List<string>()
            };
        }
    }
}

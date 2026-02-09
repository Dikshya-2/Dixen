using Dixen.Repo.DTOs.Analysis;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services
{
    public class EventAnalysisService
    {
        private readonly IGRepo<Evnt> _eventRepo;
        private readonly IGRepo<SocialShare> _socialShareRepo;
        public EventAnalysisService(
            IGRepo<Evnt> eventRepo,
            IGRepo<SocialShare> socialShareRepo)
        {
            _eventRepo = eventRepo;
            _socialShareRepo = socialShareRepo;
        }

        public async Task<List<EventSummaryDto>> AnalyzeEventsAsync()
        {
            var events = await _eventRepo.GetAllForAnalytics(q =>
                q.Include(e => e.Categories)
                 .Include(e => e.Bookings)
                    .ThenInclude(b => b.Tickets)
            );

            var shares = await _socialShareRepo.GetAll() ?? new List<SocialShare>();

            var eventList = events.ToList();

            //  in-memory LINQ)
            var summary = eventList.Select(e => new EventSummaryDto
            {
                EventId = e.Id,
                Title = e.Title,
                EventType = e.Categories.FirstOrDefault()?.Name ?? "Unknown",

                TicketsSold = e.Bookings
                    .SelectMany(b => b.Tickets)
                    .Sum(t => t.Quantity),

                SharesCount = shares.Count(s => s.EventId == e.Id)
            })
            .ToList();

            return summary;
        }

        public object CalculateAnalytics(List<EventSummaryDto> data)
        {
            if (data == null || data.Count == 0)
            {
                return new { IsEmpty = true };
            }

            double meanTickets = data.Average(x => x.TicketsSold);
            double meanShares = data.Average(x => x.SharesCount);

            double numerator = data.Sum(x =>
                (x.TicketsSold - meanTickets) *
                (x.SharesCount - meanShares));

            double denominator = Math.Sqrt(
                data.Sum(x => Math.Pow(x.TicketsSold - meanTickets, 2)) *
                data.Sum(x => Math.Pow(x.SharesCount - meanShares, 2)));

            return new
            {
                Correlation = denominator == 0 ? 0 : numerator / denominator
            };
        }
    }
}



using Dixen.Repo.DTOs.Ticket;
using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services
{
    public class TicketService: ITicketService
    {
        private readonly IGRepo<Ticket> _ticketRepo;
        private readonly IGRepo<Booking> _bookingRepo;
        public TicketService(
            IGRepo<Ticket> ticketRepo,
            IGRepo<Booking> bookingRepo)
        {
            _ticketRepo = ticketRepo;
            _bookingRepo = bookingRepo;
        }
        public async Task<TicketResponse> CreateAsync(int bookingId, TicketDto dto)
        {
            var booking = await _bookingRepo.GetById(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            var ticket = await _ticketRepo.Create(new Ticket
            {
                BookingId = bookingId,
                Type = dto.Type,
                Price = dto.Price,
                Quantity = dto.Quantity
            });

            return Map(ticket);
        }
        public async Task<List<TicketResponse>> GetByBookingAsync(int bookingId)
        {
            var tickets = await _ticketRepo.Find(t => t.BookingId == bookingId);
            return tickets.Select(Map).ToList();
        }
        public async Task<TicketResponse?> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepo.GetById(id);
            return ticket == null ? null : Map(ticket);
        }
        public async Task<TicketResponse?> UpdateAsync(int id, TicketDto dto)
        {
            var ticket = await _ticketRepo.GetById(id);
            if (ticket == null) return null;
            ticket.Type = dto.Type;
            ticket.Price = dto.Price;
            ticket.Quantity = dto.Quantity;
            await _ticketRepo.Update(id, ticket);
            return Map(ticket);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _ticketRepo.Delete(id);
        }
        private static TicketResponse Map(Ticket t) => new()
        {
            Id = t.Id,
            Type = t.Type,
            Price = t.Price,
            Quantity = t.Quantity
        };
    }
}

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
        //    private readonly DatabaseContext _context;

        //    public TicketService(DatabaseContext context)
        //    {
        //        _context = context;
        //    }

        //    public async Task<List<Ticket>> GetAllTicketsAsync()
        //    {
        //        return await _context.Tickets
        //            .Include(t => t.Booking)
        //            .ThenInclude(b => b.Event)
        //            .ToListAsync();
        //    }

        //    public async Task<Ticket?> GetTicketByIdAsync(int id)
        //    {
        //        return await _context.Tickets
        //            .Include(t => t.Booking)
        //            .ThenInclude(b => b.Event)
        //            .FirstOrDefaultAsync(t => t.Id == id);
        //    }

        //    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        //    {
        //        _context.Tickets.Add(ticket);
        //        await _context.SaveChangesAsync();
        //        return ticket;
        //    }

        //    public async Task<bool> UpdateTicketAsync(Ticket ticket)
        //    {
        //        var existingTicket = await _context.Tickets.FindAsync(ticket.Id);
        //        if (existingTicket == null) return false;

        //        existingTicket.Type = ticket.Type;
        //        existingTicket.Price = ticket.Price;
        //        existingTicket.Quantity = ticket.Quantity;
        //        existingTicket.BookingId = ticket.BookingId;

        //        await _context.SaveChangesAsync();
        //        return true;
        //    }

        //    public async Task<bool> DeleteTicketAsync(int id)
        //    {
        //        var ticket = await _context.Tickets.FindAsync(id);
        //        if (ticket == null) return false;

        //        _context.Tickets.Remove(ticket);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }

        //    public async Task<List<Ticket>> GetTicketsByBookingAsync(int bookingId)
        //    {
        //        return await _context.Tickets
        //            .Where(t => t.BookingId == bookingId)
        //            .ToListAsync();
        //    }

        //    // Create tickets when a new booking is made
        //    public async Task<Ticket> CreateTicketsForBookingAsync(Booking booking, List<Ticket> tickets)
        //    {
        //        // Link tickets to the booking
        //        foreach (var ticket in tickets)
        //        {
        //            ticket.BookingId = booking.Id;
        //            _context.Tickets.Add(ticket);
        //        }

        //        await _context.SaveChangesAsync();

        //        return tickets.Count > 0 ? tickets[0] : null;
        //    }
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

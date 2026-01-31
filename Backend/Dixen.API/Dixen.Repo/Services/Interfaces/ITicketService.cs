using Dixen.Repo.DTOs.Ticket;
using Dixen.Repo.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface ITicketService
    {
        //Task<List<Ticket>> GetAllTicketsAsync();
        //Task<Ticket?> GetTicketByIdAsync(int id);
        //Task<Ticket> CreateTicketAsync(Ticket ticket);
        //Task<bool> UpdateTicketAsync(Ticket ticket);
        //Task<bool> DeleteTicketAsync(int id);
        //Task<List<Ticket>> GetTicketsByBookingAsync(int bookingId);
        //Task<Ticket> CreateTicketsForBookingAsync(Booking booking, List<Ticket> tickets);

        Task<TicketResponse> CreateAsync(int bookingId, TicketDto dto);
        Task<List<TicketResponse>> GetByBookingAsync(int bookingId);
        Task<TicketResponse?> GetByIdAsync(int id);
        Task<TicketResponse?> UpdateAsync(int id, TicketDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

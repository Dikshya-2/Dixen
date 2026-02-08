using Dixen.Repo.DTOs.Ticket;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<ActionResult<List<TicketResponse>>> GetByBooking(int bookingId)
        {
            return Ok(await _service.GetByBookingAsync(bookingId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> Get(int id)
        {
            var ticket = await _service.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }
        [HttpPost("booking/{bookingId}")]
        public async Task<ActionResult<TicketResponse>> Create(
            int bookingId,
            TicketDto dto)
        {
            var ticket = await _service.CreateAsync(bookingId, dto);
            return CreatedAtAction(nameof(Get), new { id = ticket.Id }, ticket);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponse>> Update(int id, TicketDto dto)
        {
            var ticket = await _service.UpdateAsync(id, dto);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

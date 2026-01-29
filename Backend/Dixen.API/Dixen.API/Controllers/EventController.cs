using Dixen.Repo.DTOs.Event;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _eventService.GetEventByIdAsync(id);
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Organization")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateEventDto dto)
        {
            var created = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Organization")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateEventDto dto)
        {
            var updated = await _eventService.UpdateEventAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin,Organization")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventService.DeleteEventAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

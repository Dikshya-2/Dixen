using Dixen.Repo.DTOs.Hall;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class HallController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallController(IHallService hallService)
        {
            _hallService = hallService;
        }
        [HttpGet]
        public async Task<ActionResult<List<HallResponse>>> GetAllHalls()
        {
            var halls = await _hallService.GetAllHallsAsync();
            return Ok(halls);
        }
        [HttpGet("ByEvent")]
        public async Task<ActionResult<List<HallResponse>>> GetHalls([FromQuery] int eventId)
        {
            return Ok(await _hallService.GetHallsByEventAsync(eventId));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<HallResponse>> GetById(int id)
        {
            var hall = await _hallService.GetByIdAsync(id);
            if (hall == null) return NotFound();
            return Ok(hall);
        }

        [HttpPost]
        public async Task<ActionResult<HallResponse>> Create([FromBody] HallDto dto)
        {
            try
            {
                var hall = await _hallService.CreateHallAsync(dto.EventId, dto);

                return CreatedAtAction(nameof(GetById), new { id = hall.Id }, hall);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]  
        public async Task<ActionResult<HallResponse>> UpdateHall(int id, [FromBody] HallDto dto)  
        {
            
            var updated = await _hallService.UpdateHallAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{hallId}")]
        public async Task<IActionResult> DeleteHall(int hallId)  
        {
            var success = await _hallService.DeleteHallAsync(hallId);
            if (!success) return NotFound();
            return NoContent();
        }



    }
}

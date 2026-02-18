using Dixen.Repo.DTOs.Venue;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly IGRepo<Venue> _venueRepo;
        public VenueController(IGRepo<Venue> venueRepo)
        {
            _venueRepo = venueRepo;
        }
        [HttpGet]
        public async Task<ActionResult<List<Venue>>> GetAll()
        {
            var venues = await _venueRepo.GetAll();
            return Ok(venues);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Venue>> GetById(int id)
        {
            var venue = await _venueRepo.GetById(id);
            if (venue == null) return NotFound();
            return Ok(venue);
        }

        [HttpPost]
        public async Task<ActionResult<Venue>> Create([FromBody] VenueCreateDto dto)
        {
            var venue = new Venue
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City
            };
            await _venueRepo.Create(venue);
            return CreatedAtAction(nameof(GetById), new { id = venue.Id }, venue);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Venue>> Update(int id, [FromBody] VenueCreateDto dto)
        {
            var venue = await _venueRepo.GetById(id);
            if (venue == null) return NotFound();

            venue.Name = dto.Name;
            venue.Address = dto.Address;
            venue.City = dto.City;

            var updated = await _venueRepo.Update(id, venue);
            return Ok(updated);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _venueRepo.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}

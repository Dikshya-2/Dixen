using Dixen.Repo.DTOs;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformerController : ControllerBase
    {
        private readonly IGRepo<Performer> _performerRepo;

        public PerformerController(IGRepo<Performer> performerRepo)
        {
            _performerRepo = performerRepo;
        }

        // Get all performers
        [HttpGet]
        public async Task<ActionResult<List<PerformerDto>>> GetAll()
        {
            var performers = await _performerRepo.GetAll();

            var performerDtos = performers.Select(performer => new PerformerDto
            {
                Id = performer.Id,
                Name = performer.Name,
                Bio = performer.Bio,
                EventId = performer.EventId
            }).ToList();

            return Ok(performerDtos);
        }

        // Get performer by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PerformerDto>> GetById(int id)
        {
            var performer = await _performerRepo.GetById(id);
            if (performer == null)
                return NotFound();

            var performerDto = new PerformerDto
            {
                Id = performer.Id,
                Name = performer.Name,
                Bio = performer.Bio,
                EventId = performer.EventId
            };

            return Ok(performerDto);
        }

        // Create performer
        [HttpPost]
        public async Task<ActionResult<PerformerDto>> Create(PerformerDto performerDto)
        {
            var performer = new Performer
            {
                Name = performerDto.Name,
                Bio = performerDto.Bio,
                EventId = performerDto.EventId
            };

            await _performerRepo.Create(performer);

            performerDto.Id = performer.Id;  // After creation, set the Id
            return CreatedAtAction(nameof(GetById), new { id = performerDto.Id }, performerDto);
        }
        // Update performer
        [HttpPut("{id}")]
        public async Task<ActionResult<PerformerDto>> Update(int id, PerformerDto performerDto)
        {
            var existingPerformer = await _performerRepo.GetById(id);
            if (existingPerformer == null)
            {
                return NotFound();
            }

            // Update fields
            existingPerformer.Name = performerDto.Name;
            existingPerformer.Bio = performerDto.Bio;
            existingPerformer.EventId = performerDto.EventId;

            await _performerRepo.Update(id, existingPerformer);

            var updatedPerformerDto = new PerformerDto
            {
                Id = existingPerformer.Id,
                Name = existingPerformer.Name,
                Bio = existingPerformer.Bio,
                EventId = existingPerformer.EventId
            };

            return Ok(updatedPerformerDto);
        }


        // Delete performer
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _performerRepo.Delete(id);
            return NoContent();
        }
    }
}


using Dixen.Repo.DTOs;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dixen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialShareController : ControllerBase
    {
        private readonly IGRepo<SocialShare> _socialShareRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private IGRepo<Evnt> _eventRepo;

        public SocialShareController(IGRepo<SocialShare> socialShareRepo, UserManager<ApplicationUser> userManager, IGRepo<Evnt> eventRepo)
        {
            _socialShareRepo = socialShareRepo;
            _userManager = userManager;
            _eventRepo = eventRepo;

        }

        [HttpGet]
        public async Task<ActionResult<List<SocialShareDto>>> GetAll()
        {
            var socialShares = await _socialShareRepo.GetAll();

            var socialShareDtos = socialShares.Select(share => new SocialShareDto
            {
                Id = share.Id,
                Platform = share.Platform,
                SharedAt = share.SharedAt,
                EventId = share.EventId,
                UserEmail = share.UserId
            }).ToList();

            return Ok(socialShareDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SocialShareDto>> GetById(int id)
        {
            var socialShare = await _socialShareRepo.GetById(id);
            if (socialShare == null)
                return NotFound();

            var socialShareDto = new SocialShareDto
            {
                Id = socialShare.Id,
                Platform = socialShare.Platform,
                SharedAt = socialShare.SharedAt,
                EventId = socialShare.EventId,
                UserEmail = socialShare.UserId
            };

            return Ok(socialShareDto);
        }

        [HttpPost]
        public async Task<ActionResult<SocialShareDto>> Create([FromBody] SocialShareDto socialShareDto)
        {
            var user = await _userManager.FindByEmailAsync(socialShareDto.UserEmail);
            if (user == null)
                return BadRequest("Specified user does not exist.");

            var eventExists = await _eventRepo.Exists(socialShareDto.EventId);
            if (!eventExists)
                return NotFound("Event not found.");

            var share = new SocialShare
            {
                Platform = socialShareDto.Platform,
                SharedAt = DateTime.UtcNow,
                EventId = socialShareDto.EventId,
                UserId = user.Id 
            };

            await _socialShareRepo.Create(share);

            var resultDto = new SocialShareDto
            {
                Id = share.Id,
                Platform = share.Platform,
                SharedAt = share.SharedAt,
                EventId = share.EventId,
                UserEmail = user.Email
            };

            return CreatedAtAction(nameof(GetById), new { id = share.Id }, resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _socialShareRepo.Delete(id);
            return NoContent();
        }

        [HttpGet("count/{eventId}")]
        public async Task<ActionResult<int>> GetShareCount(int eventId)
        {
            var socialShares = await _socialShareRepo.GetAll();
            var count = socialShares.Count(s => s.EventId == eventId);
            return Ok(count);
        }


        [HttpGet("shared/{eventId}")]
        public async Task<ActionResult<List<SocialShareDto>>> GetSharesByEventId(int eventId)
        {
            var socialShares = await _socialShareRepo.GetAll();
            var filteredShares = socialShares.Where(share => share.EventId == eventId).ToList();

            if (filteredShares.Count == 0)
            {
                return NotFound("No shares found for this event.");
            }
            var socialShareDtos = filteredShares.Select(share => new SocialShareDto
            {
                Id = share.Id,
                Platform = share.Platform,
                SharedAt = share.SharedAt,
                EventId = share.EventId,
                UserEmail = share.UserId
            }).ToList();

            return Ok(socialShareDtos);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<SocialShareDto>> Update(int id, [FromBody] SocialShareDto socialShareDto)
        {
            var existing = await _socialShareRepo.GetById(id);
            if (existing == null)
                return NotFound("SocialShare not found.");

            var eventExists = await _eventRepo.Exists(socialShareDto.EventId);
            if (!eventExists)
                return NotFound("Event not found.");

            var user = await _userManager.FindByEmailAsync(socialShareDto.UserEmail);
            if (user == null)
                return BadRequest("Specified user does not exist.");

            existing.Platform = socialShareDto.Platform;
            existing.EventId = socialShareDto.EventId;
            existing.UserId = user.Id;
            existing.SharedAt = DateTime.UtcNow;

            await _socialShareRepo.Update(id, existing);

            var resultDto = new SocialShareDto
            {
                Id = existing.Id,
                Platform = existing.Platform,
                SharedAt = existing.SharedAt,
                EventId = existing.EventId,
                UserEmail = user.Email
            };

            return Ok(resultDto);
        }

        [HttpGet("stats/event-platforms")]
        public async Task<ActionResult<IEnumerable<object>>> GetShareStatsByEventAndPlatform()
        {
            var shares = await _socialShareRepo.GetAll();
            var stats = shares
                .GroupBy(s => new { s.EventId, s.Platform })
                .Select(g => new {
                    eventId = g.Key.EventId,
                    platform = g.Key.Platform,
                    count = g.Count()
                })
                .ToList();

            return Ok(stats);
        }
    }
}


using Dixen.Repo.DTOs.Organizer;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizerController : ControllerBase
    {
        private readonly IGRepo<Organizer> _organizerRepo;
        private readonly IGRepo<EventSubmission> _submissionRepo;
        private readonly IGRepo<Evnt> _eventRepo;
        private readonly IGRepo<Category> _categoryRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizerController(IGRepo<Organizer> repo, IGRepo<EventSubmission> submissionRepo,
        IGRepo<Evnt> eventRepo, IGRepo<Category> categoryRepo, UserManager<ApplicationUser> userManager)
        {
            _organizerRepo = repo;
            _submissionRepo = submissionRepo;
            _eventRepo = eventRepo;
            _categoryRepo = categoryRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrganizerResponseDto>>> GetAll()
        {
            var organizers = await _organizerRepo.GetAll(q => q.Include(o => o.Events));
            var result = organizers.Select(o => new OrganizerResponseDto
            {
                Id = o.Id,
                OrganizationName = o.OrganizationName,
                ContactEmail = o.ContactEmail,
                EventTitles = o.Events.Select(e => e.Title).ToList()
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizerResponseDto>> Get(int id)
        {
            var organizer = await _organizerRepo.GetById(id, q => q.Include(o => o.Events));
            if (organizer == null) return NotFound();

            return Ok(new OrganizerResponseDto
            {
                Id = organizer.Id,
                OrganizationName = organizer.OrganizationName,
                ContactEmail = organizer.ContactEmail,
                EventTitles = organizer.Events.Select(e => e.Title).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<OrganizerResponseDto>> Create(
            OrganizerCreateUpdateDto dto)
        {
            var organizer = await _organizerRepo.Create(new Organizer
            {
                OrganizationName = dto.OrganizationName,
                ContactEmail = dto.ContactEmail
            });

            return CreatedAtAction(nameof(Get), new { id = organizer.Id },
                new OrganizerResponseDto
                {
                    Id = organizer.Id,
                    OrganizationName = organizer.OrganizationName,
                    ContactEmail = organizer.ContactEmail,
                    EventTitles = new List<string>()
                });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrganizerCreateUpdateDto response)
        {
            var organizer = await _organizerRepo.GetById(id);
            if (organizer == null) return NotFound();
            organizer.OrganizationName = response.OrganizationName;
            organizer.ContactEmail = response.ContactEmail;
            await _organizerRepo.Update(id, organizer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _organizerRepo.Delete(id);
            return success ? NoContent() : NotFound();
        }

        //[Authorize(Roles = "Host")]
        //[HttpPost("submit-event")]
        //public async Task<IActionResult> SubmitEvent([FromBody] EventSubmissionDto dto)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var organizer = await _organizerRepo.GetAllQuery()
        //        .FirstOrDefaultAsync(o => o.UserId == userId);

        //    if (organizer == null)
        //        return Forbid();

        //    if (await _submissionRepo.GetAllQuery()
        //        .AnyAsync(x => x.EventId == dto.EventId && x.SubmittedById == organizer.Id))
        //        return Conflict("You have already submitted a proposal for this event.");

        //    var evt = await _eventRepo.GetById(dto.EventId);
        //    if (evt == null)
        //        return BadRequest("Event not found");

        //    var submission = new EventSubmission
        //    {
        //        EventId = dto.EventId,
        //        SubmittedById = organizer.Id,
        //        SubmittedBy = organizer.OrganizationName,
        //        Title = dto.Title,
        //        Description = dto.Description,
        //        StartTime = dto.StartTime,
        //        ImageUrl = dto.ImageUrl,
        //        SubmittedAt = DateTime.UtcNow,
        //        IsApproved = false
        //    };

        //    await _submissionRepo.Create(submission);

        //    return Ok(new { Message = "Event submission sent to admin for approval", SubmissionId = submission.Id });
        //}

        [Authorize(Roles = "Host")]
        [HttpPost("{organizerId}/submit-event")]
        public async Task<IActionResult> SubmitEvent(int organizerId, [FromBody] EventSubmissionDto dto)
        {
            var organizer = await _organizerRepo.GetById(organizerId);
            if (organizer == null)
                return NotFound("Organizer not found");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in");

            var userExists = await _userManager.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                userId = "9f0bd209-3b56-410c-b4fc-5654161c3925";
                Console.WriteLine($"Using fallback UserId: '{userId}'");
            }

            var duplicateExists = await _submissionRepo.GetAllQuery()
                .Where(x => x.EventId == dto.EventId && x.SubmittedById == organizerId)
                .AnyAsync();

            if (duplicateExists)
                return Conflict("You have already submitted a proposal for this event.");

            var evt = await _eventRepo.GetById(dto.EventId);
            if (evt == null)
                return BadRequest("Event not found");

            var submission = new EventSubmission
            {
                EventId = dto.EventId,
                SubmittedById = organizer.Id,
                SubmittedBy = organizer.OrganizationName,
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                ImageUrl = dto.ImageUrl,
                SubmittedAt = DateTime.UtcNow,
                IsApproved = false,
            };

            await _submissionRepo.Create(submission);

            return Ok(new
            {
                Message = "Event submission sent to admin for approval",
                SubmissionId = submission.Id
            });
        }


        [Authorize(Roles = "admin")]
        [HttpPost("admin/event-submissions/{id}/reject")]
        public async Task<IActionResult> RejectSubmission(int id)
        {
            var submission = await _submissionRepo.GetById(id);
            if (submission == null)
                return NotFound();

            submission.IsApproved = false;

            await _submissionRepo.Update(submission.Id, submission);

            return Ok("Submission rejected");
        }

    }

}



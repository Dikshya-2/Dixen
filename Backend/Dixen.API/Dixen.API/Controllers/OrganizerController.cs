using Dixen.Repo.DTOs;
using Dixen.Repo.DTOs.Organizer;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories;
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
        private readonly IEventSubmissionRepository _eventSubmissionRepository;  

        public OrganizerController(IGRepo<Organizer> repo, IGRepo<EventSubmission> submissionRepo,
        IGRepo<Evnt> eventRepo, IGRepo<Category> categoryRepo, UserManager<ApplicationUser> userManager, IEventSubmissionRepository eventSubmissionRepository)
        {
            _organizerRepo = repo;
            _submissionRepo = submissionRepo;
            _eventRepo = eventRepo;
            _categoryRepo = categoryRepo;
            _userManager = userManager;
            _eventSubmissionRepository = eventSubmissionRepository;
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
        [Authorize(Roles = "Host, User, Admin")]
        [HttpPost("{organizerId}/submit-event")]
        public async Task<IActionResult> SubmitEvent(int organizerId, [FromBody] EventSubmissionDto dto)
        {
            // 1️⃣ Get the organizer
            var organizer = await _organizerRepo.GetById(organizerId);
            if (organizer == null)
                return NotFound("Organizer not found");

            // 2️⃣ Get the current logged-in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in");

            // 3️⃣ Verify user exists in the system
            var userExists = await _userManager.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                userId = "9f0bd209-3b56-410c-b4fc-5654161c3925"; // fallback user
                Console.WriteLine($"Using fallback UserId: '{userId}'");
            }

            // 4️⃣ Check for duplicate submissions
            var duplicateExists = await _submissionRepo.GetAllQuery()
                .Where(x => x.EventId == dto.EventId && x.SubmittedById == organizerId)
                .AnyAsync();

            if (duplicateExists)
                return Conflict("You have already submitted a proposal for this event.");

            // 5️⃣ Verify the event exists
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
                IsApproved = null,   
                Details = dto.Details
            };


            await _submissionRepo.Create(submission);

            // 7️⃣ Return response
            return Ok(new
            {
                Message = "Event submission sent to admin for approval",
                SubmissionId = submission.Id,
                Status = "Pending"
            });
        }


        //[Authorize(Roles = "Host, User, Admin")]
        //[HttpPost("{organizerId}/submit-event")]
        //public async Task<IActionResult> SubmitEvent(int organizerId, [FromBody] EventSubmissionDto dto)
        //{
        //    var organizer = await _organizerRepo.GetById(organizerId);
        //    if (organizer == null)
        //        return NotFound("Organizer not found");

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized("User not logged in");

        //    var userExists = await _userManager.Users.AnyAsync(u => u.Id == userId);
        //    if (!userExists)
        //    {
        //        userId = "9f0bd209-3b56-410c-b4fc-5654161c3925";
        //        Console.WriteLine($"Using fallback UserId: '{userId}'");
        //    }

        //    var duplicateExists = await _submissionRepo.GetAllQuery()
        //        .Where(x => x.EventId == dto.EventId && x.SubmittedById == organizerId)
        //        .AnyAsync();

        //    if (duplicateExists)
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
        //        IsApproved = null,
        //    };

        //    await _submissionRepo.Create(submission);

        //    return Ok(new
        //    {
        //        Message = "Event submission sent to admin for approval",
        //        SubmissionId = submission.Id
        //    });
        //}
    }

}



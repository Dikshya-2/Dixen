using Dixen.Repo.DTOs.Organizer;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        ////[Authorize (Roles ="Host")]
        //[HttpPost("{organizerId}/submit-event")]
        //public async Task<IActionResult> SubmitEvent(int organizerId, [FromBody] EventSubmissionDto dto)
        //{
        //    var organizer = await _organizerRepo.GetById(organizerId);
        //    if (organizer == null)
        //        return NotFound("Organizer not found");

        //    var submission = new EventSubmission
        //    {
        //        SubmittedBy = organizer.OrganizationName, // track who submitted
        //        SubmittedAt = DateTime.UtcNow,
        //        IsApproved = false,
        //        Details = $"Title: {dto.Title}\nDescription: {dto.Description}\nStartTime: {dto.StartTime}\nImageUrl: {dto.ImageUrl}",
        //        UserId = null // optional, if organizers don’t have accounts
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


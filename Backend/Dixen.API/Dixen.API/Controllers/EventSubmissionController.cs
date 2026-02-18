using Dixen.Repo.Repositories;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventSubmissionController : ControllerBase
    {
        private readonly IEventSubmissionRepository _eventSubmissionRepo;

        public EventSubmissionController(IEventSubmissionRepository eventSubmissionRepo)
        {
            _eventSubmissionRepo = eventSubmissionRepo;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("event-submissions/list")]
        public async Task<IActionResult> GetPendingSubmissions()
        {
            var submissions = await _eventSubmissionRepo.GetAll();
            return Ok(submissions);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("event-submissions/{id}/approve")]
        public async Task<IActionResult> ApproveSubmission(int id)
        {
            await _eventSubmissionRepo.ApproveEventSubmission(id);
            return Ok(new { message = "Submission approved!" });
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("event-submissions/{id}/reject")]
        public async Task<IActionResult> RejectSubmission(int id)
        {
            await _eventSubmissionRepo.RejectEventSubmission(id);
            return Ok(new { message = "Submission rejected!" });
        }
    }
}

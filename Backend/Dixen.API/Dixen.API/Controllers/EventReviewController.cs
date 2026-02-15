using Dixen.Repo.DTOs.Rating;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventReviewController : ControllerBase
    {
        private readonly IGRepo<EventReview> _reviewRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventReviewController(IGRepo<EventReview> reviewRepo, UserManager<ApplicationUser> userManager)
        {
            _reviewRepo = reviewRepo;
            _userManager = userManager;
        }

        // Add or update a review
        [HttpPost]
        public async Task<ActionResult> AddReview([FromBody] EventReviewDto reviewDto)
        {
            var user = await _userManager.FindByIdAsync(reviewDto.UserId);
            if (user == null) return BadRequest("User not found");

            // Check if user already reviewed this event
            var existing = (await _reviewRepo.GetAll())
                .FirstOrDefault(r => r.EventId == reviewDto.EventId && r.UserId == reviewDto.UserId);

            if (existing != null)
            {
                existing.Rating = reviewDto.Rating;
                existing.Comment = reviewDto.Comment;
                await _reviewRepo.Update(existing.Id, existing);
            }
            else
            {
                var review = new EventReview
                {
                    EventId = reviewDto.EventId,
                    UserId = reviewDto.UserId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment
                };
                await _reviewRepo.Create(review);
            }

            return Ok();
        }

        // Get all reviews for an event
        [HttpGet("{eventId}")]
        public async Task<ActionResult<IEnumerable<EventReviewDto>>> GetReviews(int eventId)
        {
            var reviews = (await _reviewRepo.GetAll())
                .Where(r => r.EventId == eventId)
                .Select(r => new EventReviewDto
                {
                    EventId = r.EventId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment
                })
                .ToList();

            return Ok(reviews);
        }

        // Get average rating
        [HttpGet("average/{eventId}")]
        public async Task<ActionResult<double>> GetAverageRating(int eventId)
        {
            var reviews = (await _reviewRepo.GetAll()).Where(r => r.EventId == eventId).ToList();
            if (!reviews.Any()) return Ok(0.0);
            var avg = reviews.Average(r => r.Rating);
            return Ok(avg);
        }
    }
}

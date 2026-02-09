using Dixen.Repo.DTOs.UserProfile;
using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dixen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserprofileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _context;

        public UserprofileController(UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet("profile/byemail/{email}")]
        public async Task<ActionResult<UserProfileDto>> GetProfileByEmail(string email)
        {
            var user = await _userManager.Users
                .Include(u => u.Bookings)
                    .ThenInclude(a => a.Event)
                .Include(u => u.SocialShares)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var hostedEvents = new List<string>();
            if (roles.Contains("organization"))
            {
                hostedEvents = await _context.Events
                    .Where(e => e.Organizer.ContactEmail == user.Email)
                    .Select(e => e.Title)
                    .ToListAsync();
            }

            var proposedEvents = await _context.EventSubmissions
                .Where(s => s.SubmittedBy == user.FullName)
                .Select(s => s.Details)
                .ToListAsync();

            var preferredCategories = user.Bookings
                .SelectMany(a => a.Event.Categories)
                .Distinct()
                .Select(c => c.Name)
                .ToList();

            var profile = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Gender = user.Gender,
                Age = user.Age,
                Roles = roles.ToList(),
                Is2FAEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                //AttendedEvents = user.Bookings.Select(a => a.Event.Title).ToList(),
                HostedEvents = hostedEvents,
                ProposedEvents = proposedEvents,
                PreferredCategories = preferredCategories
            };

            return Ok(profile);
        }

    }
}

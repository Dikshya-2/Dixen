using Dixen.Repo.DTOs.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.UserProfile
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool Is2FAEnabled { get; set; }
        public List<BookingDto> Bookings { get; set; } = new();
        public List<string> HostedEvents { get; set; } = new();
        public List<string> ProposedEvents { get; set; } = new();
        public List<string> PreferredCategories { get; set; } = new();
    }
}


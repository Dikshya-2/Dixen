using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public required string FullName { get; set; } = string.Empty;
        public required int Age { get; set; }
        public string Gender { get; set; }
        //public List<EventAttendance> Attendances { get; set; } = new();
        public List<SocialShare> SocialShares { get; set; } = new();
        public List<EventReview> Reviews { get; set; } = new();
        public List<EventSubmission> EventSubmissions { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public bool IsDeleted { get; set; } = false;

    }
}

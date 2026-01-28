using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class EventSubmission
    {
        public int Id { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public int EventId { get; set; }
        public Evnt Event { get; set; }
        public bool IsApproved { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 
    }
}

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
        public int SubmittedById { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public int EventId { get; set; }
        public Evnt Event { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? ImageUrl { get; set; }
        public string Details { get; set; } = string.Empty;
        public bool? IsApproved { get; set; } 
    }
}

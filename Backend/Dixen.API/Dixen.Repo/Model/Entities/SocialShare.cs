using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class SocialShare
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public DateTime SharedAt { get; set; }
        public int EventId { get; set; }
        public Evnt Event { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}

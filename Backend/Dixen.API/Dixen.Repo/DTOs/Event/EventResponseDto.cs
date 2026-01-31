using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Event
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string? ImageUrl { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public List<string> CategoryNames { get; set; } = new();
        public List<string> HallNames { get; set; } = new();
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
    }
}

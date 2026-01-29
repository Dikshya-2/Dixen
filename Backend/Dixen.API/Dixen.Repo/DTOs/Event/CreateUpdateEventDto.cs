using System;
using System.Collections.Generic;

namespace Dixen.Repo.DTOs.Event
{
    public class CreateUpdateEventDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string? ImageUrl { get; set; }
        public int OrganizerId { get; set; }
        public List<int> CategoryIds { get; set; } = new();
        public List<int> HallIds { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Organizer
{
    public class EventSubmissionDto
    {
        [Required]
        public int EventId { get; set; }     

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public string? ImageUrl { get; set; }

        public string Details { get; set; } = string.Empty;
    }
}

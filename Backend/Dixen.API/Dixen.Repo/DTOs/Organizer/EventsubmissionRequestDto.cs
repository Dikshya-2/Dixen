using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Organizer
{
    public class EventsubmissionRequestDto 
    {
        //public string EventTitle { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;
        //public DateTime StartTime { get; set; }
        //public List<int> CategoryIds { get; set; } = new(); 
        public List<EventSubmissionItemDto> Events { get; set; } = new();

    }
}

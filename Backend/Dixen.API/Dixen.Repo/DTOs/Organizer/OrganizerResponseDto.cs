using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Organizer
{
    public class OrganizerResponseDto
    {
        public int Id { get; set; } 
        public string OrganizationName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public List<string> EventTitles { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Filter
{
    public class EventSearchFilterDto
    {
        public string? Title { get; set; }
        public int? OrganizerId { get; set; }
        public List<int>? CategoryIds { get; set; }
        public string? VenueCity { get; set; }
        public string? Keyword { get; set; }
        public DateTime? EventDate { get; set; }  

    }
}

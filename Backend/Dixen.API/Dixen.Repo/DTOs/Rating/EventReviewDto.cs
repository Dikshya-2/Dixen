using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Rating
{
    public class EventReviewDto 
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } 
        public string Comment { get; set; } = "";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Hall
{
    public class HallDto  
    {
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public required int Capacity { get; set; }
        public required int VenueId { get; set; }
        public required int EventId { get; set; }
    }
}

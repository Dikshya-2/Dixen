using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int EventId { get; set; }
        public Evnt Event { get; set; }
        //public List<EventAttendance> Attendances { get; set; } = new();
        public int VenueId { get; set; }
        public Venue Venue { get; set; }
    }
}

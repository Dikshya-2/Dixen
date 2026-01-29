using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookedTime { get; set; }
        public required string UserId { get; set; }=string.Empty;
        public ApplicationUser User { get; set; }
        public int EventId { get; set; }
        public Evnt Event { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }
        public List<Ticket> Tickets { get; set; } = new();
        public bool IsDeleted { get; set; } = false;



    }
}

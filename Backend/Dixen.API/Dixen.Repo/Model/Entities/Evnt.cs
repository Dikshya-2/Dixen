using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class Evnt
    {
        public int Id { get; set; }
        public required string Title { get; set; } = string.Empty; 
        public required string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string ImageUrl { get; set; }
        public int OrganizerId { get; set; }
        public Organizer Organizer { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Performer> Performers { get; set; }= new();
        //public List<EventAttendance> Attendances { get; set; }= new();
        public List<SocialShare> SocialShares { get; set; }= new();
        public List<EventSubmission> Submissions { get; set; }= new();

        // New property for Halls with individual capacities
        public List<Hall> Halls { get; set; }= new();
        public List<Booking> Bookings { get; set; }= new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<EventReview> Reviews { get; set; } = new();

    }
}

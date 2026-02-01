using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs
{
    public class SocialShareDto
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public DateTime SharedAt { get; set; }
        public int EventId { get; set; }
        public string? UserEmail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
    public class Organizer
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;  
        public List<Evnt> Events { get; set; } = new();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        //public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int EventCount { get; set; }
        public List<OnlyEvent> Events { get; set; } = new();

    }
}


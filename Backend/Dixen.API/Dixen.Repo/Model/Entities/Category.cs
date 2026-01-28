using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dixen.Repo.Model.Entities
{
   public class Category
   {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public List<Evnt> Events { get; set; } = new();
   }
}

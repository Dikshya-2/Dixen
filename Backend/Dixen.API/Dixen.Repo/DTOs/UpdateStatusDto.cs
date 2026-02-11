using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs
{
    public class UpdateStatusDto
    {
        public string Title { get; set; }
        public string status { get; set; }= "Pending";
    }

}

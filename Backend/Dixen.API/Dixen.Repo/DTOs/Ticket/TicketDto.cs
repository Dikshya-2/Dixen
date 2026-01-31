using Dixen.Repo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

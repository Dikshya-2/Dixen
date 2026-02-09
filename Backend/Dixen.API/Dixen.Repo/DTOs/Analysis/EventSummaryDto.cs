using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Analysis
{
    public class EventSummaryDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int TicketsSold { get; set; }
        public int SharesCount { get; set; }

    }
}

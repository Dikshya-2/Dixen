using Dixen.Repo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;

        public int HallId { get; set; }   
        public string HallName { get; set; } = string.Empty;  

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime BookedTime { get; set; }
    }

    //public class CreateBookingDto
    //{
    //    public int HallId { get; set; }
    //    public TicketType TicketType { get; set; }
    //    public int Quantity { get; set; }
    //}

    //public class PaymentDto
    //{
    //    public decimal Amount { get; set; }
    //}

}

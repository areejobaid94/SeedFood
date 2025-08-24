using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Booking.Dtos
{
    public class BookingOffDaysEntity
    {
        public List<BookingOffDays> bookingOffDays { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
    }
}

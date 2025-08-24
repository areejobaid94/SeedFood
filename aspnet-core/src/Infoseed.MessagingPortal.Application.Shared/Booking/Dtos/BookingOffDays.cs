using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Booking.Dtos
{
    public class BookingOffDays
    {
        public long Id { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
        public bool IsOffDayBooking { get; set; }
        // public bool IsOffDay { get; set; }
    }
}

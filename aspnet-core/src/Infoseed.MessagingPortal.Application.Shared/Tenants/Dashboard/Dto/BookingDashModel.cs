using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class BookingDashModel
    {
        public long UserId { get; set; }
        public int TotalBooked { get; set; }
        public int TotalConfirmed { get; set; }
        public int TotalCancelled { get; set; }
        public int TotalDeleted { get; set; }
        public int TotalPending { get; set; }

    }
}

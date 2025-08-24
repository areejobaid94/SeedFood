using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class BookingDashbordModel
    {
        public List<BookingDashModel> bookingDashModel { get; set; }
        public long TotalBooking { get; set; }
    }
}

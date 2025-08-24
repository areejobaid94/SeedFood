using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class BookingStatisticsModel
    {
        public long TotalAppointments { get; set; }
        public long TotalBooked { get; set; }
        public long TotalConfirmed { get; set; }
        public long TotalCancelled { get; set; }
        public long TotalPending { get; set; }
        public decimal PercentageBooked { get; set; }
        public decimal PercentageConfirmed { get; set; }
        public decimal PercentageCanceled { get; set; }
        public decimal PercentagePending { get; set; }
    }
}

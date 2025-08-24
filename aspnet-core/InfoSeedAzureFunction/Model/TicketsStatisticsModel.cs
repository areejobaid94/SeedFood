using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class TicketsStatisticsModel
    {
        public long TotalTickets { get; set; }
        public long TotalPending { get; set; }
        public long TotalOpened { get; set; }
        public long TotalClosed { get; set; }
        public long TotalExpired { get; set; }
        public DateTime? LastClosedTicketDate { get; set; }
        public int PercentagePending { get; set; }
        public int PercentageOpened { get; set; }
        public int PercentageClosed { get; set; }
        public int PercentageExpired { get; set; }
        public decimal AvgResolutionTime { get; set; }
        public string TenantName { get; set; }
    }
}

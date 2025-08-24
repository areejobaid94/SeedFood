using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UsageStatisticsModel
    {
        public int TotalDelivered { get; set; }
        public int TotalRead { get; set; }
        public int TotalSent { get; set; }
        public int TotalReplied { get; set; }
        public int TotalFailed { get; set; }
    }
}

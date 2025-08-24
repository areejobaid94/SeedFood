using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class CampaignStatisticsModel
    {
        public long TotalContact { get; set; }
        public long TotalDelivered { get; set; }
        public long TotalRead { get; set; }
        public long TotalSent { get; set; }
        public long TotalReplied { get; set; }
        public long TotalFailed { get; set; }
    }
}

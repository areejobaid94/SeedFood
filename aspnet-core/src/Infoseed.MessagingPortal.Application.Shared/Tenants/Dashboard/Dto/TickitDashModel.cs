using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class TickitDashModel
    {
        public long AgentId { get; set; }
        public int TotalOpen { get; set; }
        public int TotalClose { get; set; }
        public int TotalPending { get; set; }
        public long AvgTimeMinutes { get; set; }

    }
}

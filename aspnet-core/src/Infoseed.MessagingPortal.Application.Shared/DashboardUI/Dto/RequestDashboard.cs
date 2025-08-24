using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class RequestDashboard
    {
        public int TotalRequest { get; set; }
        public int TotalRequestPending { get; set; }
        public int TotalRequestOpen { get; set; }
        public int TotalRequestClose { get; set; }

    }
}

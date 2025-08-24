using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class ContactDashboard
    {
        public int TotalContact { get; set; }
        public int TotalContactOptIn { get; set; }
        public int TotalContactOptOut { get; set; }
        public int TotalContactNeutral { get; set; }
    }
}

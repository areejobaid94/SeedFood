using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class ContactStatisticsModel
    {
        public long TotalContact { get; set; }
        public long TotalContactOptIn { get; set; }
        public long TotalContactOptOut { get; set; }
        public long TotalContactNeutral { get; set; }
    }
}

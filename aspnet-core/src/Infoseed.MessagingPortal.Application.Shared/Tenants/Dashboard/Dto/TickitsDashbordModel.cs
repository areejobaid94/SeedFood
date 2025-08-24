using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class TickitsDashbordModel
    {
        public List<TickitDashModel> tickitDashModel { get; set; }
        public long TotalTickits { get; set; }
    }
}

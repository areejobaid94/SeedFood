using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto
{
   public  class GetTenantsDataFilterOutput
    {

        public int TenantsId { get; set; }
        public int RecentTenantsDayCount { get; set; }

        public int MaxRecentTenantsShownCount { get; set; }

        public DateTime TenantCreationStartDate { get; set; }

        public List<RecentTenant> RecentTenants { get; set; }
    }
}

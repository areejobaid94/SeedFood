using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DashboardUI.Dto
{
    public class DashboardNumbers
    {
        public long Id { get; set; }
        public DateTime CreatetionDateTime { get; set; } = DateTime.UtcNow;
        public int TenantId { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }

    }
}

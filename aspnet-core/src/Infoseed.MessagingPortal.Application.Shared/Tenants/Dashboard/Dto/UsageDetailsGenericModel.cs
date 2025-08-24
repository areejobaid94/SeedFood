using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UsageDetailsGenericModel
    {
        public List<UsageDetailsModel> usageDetails { get; set; }
        public long Total { get; set; }
    }
}

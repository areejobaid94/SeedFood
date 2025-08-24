using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class ConversationPriceModel
    {
        public string Country { get; set; }
        public float TotalDeposit { get; set; }
        public int TotalMarketingCount { get; set; }
        public int TotalUtilityCount { get; set; }
        public int TotalServicesCount { get; set; }
    }
}

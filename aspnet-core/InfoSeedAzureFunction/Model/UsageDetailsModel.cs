using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class UsageDetailsModel
    {
        public int TenantId { get; set; }
        public string categoryType { get; set; }
        public DateTime dateTime { get; set; } = DateTime.UtcNow;
        public string sentBy { get; set; }
        public string templateName { get; set; }
        public string campaignName { get; set; }
        public int quantity { get; set; }
        public decimal totalCost { get; set; }
        public decimal totalCreditRemaining { get; set; }
        public string countries { get; set; }
        public long campaignId { get; set; }

    }
}

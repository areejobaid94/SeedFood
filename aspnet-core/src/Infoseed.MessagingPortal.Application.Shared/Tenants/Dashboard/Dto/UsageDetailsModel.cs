using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class UsageDetailsModel
    {
        public string transactionIdS { get; set; }
        public string CategoryType { get; set; }
        public DateTime TransactionDate { get; set;} = DateTime.UtcNow;
        public string DoneBy { get; set; }
        public string TemplateName { get; set; }
        public string CampaignName { get; set; }
        public long CampaignId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalTransaction { get; set; }
        public decimal TotalRemaining { get; set; }
        public string Country { get; set; }
    }
}

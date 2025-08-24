using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class TransactionModel
    {
        public long Id { get; set; }
        public string DoneBy { get; set; }
        public decimal TotalTransaction { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string CategoryType { get; set; }
        public decimal TotalRemaining { get; set; }
        public long WalletId { get; set; }
        public string Country { get; set; }
        public int TenantId { get; set; }
        public string InvoiceId { get; set; }
        public string InvoiceUrl { get; set; }
        public bool IsPayed { get; set; }
        public string Note { get; set; }
        public string CampaignName { get; set; }
        public string TemplateName { get; set; }
        public int Quantity { get; set; }
        public string Countries { get; set; }
        public long CampaignId { get; set; }
        public bool IsOnHold { get; set; }
    }
}

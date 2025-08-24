using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard
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
        public string invoiceId { get; set; }
        public string invoiceUrl { get; set; }
        public bool IsPayed { get; set; }
        public string Note { get; set; }

    }
}

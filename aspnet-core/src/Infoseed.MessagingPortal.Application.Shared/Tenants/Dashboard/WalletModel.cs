using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard
{
    public class WalletModel
    {
        public long WalletId { get; set; }
        public int TenantId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountSAR { get; set; } = 0;
        public decimal OnHold { get; set; } = 0;
        public DateTime DepositDate { get; set; } = DateTime.UtcNow;
        public string Country { get; set; }   
    }
}

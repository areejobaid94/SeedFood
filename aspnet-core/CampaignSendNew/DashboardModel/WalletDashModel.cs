using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.Model
{
    public class WalletDashModel
    {
        public long WalletId { get; set; }
        public int TenantId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OnHold { get; set; } = 0;
        public DateTime DepositDate { get; set; } = DateTime.UtcNow;
        public string Country { get; set; }
    }
}

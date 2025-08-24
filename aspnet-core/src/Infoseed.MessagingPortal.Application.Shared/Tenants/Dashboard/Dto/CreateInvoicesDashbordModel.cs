using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class CreateInvoicesDashbordModel
    {
        public string ZohoCustomerIds { get; set; }

        public long UserId { get; set; }
        public long WalletId { get; set; }
        public int TenantId { get; set; }
        public decimal TotalAmount { get; set; }
        //public double OnHold { get; set; } = 0;
        public DateTime DepositDate { get; set; } = DateTime.UtcNow;
        public string Country { get; set; }
    }
}

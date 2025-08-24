using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSyncFunction.WhatsAppApi.Dto
{
    public class UTracOrderModel
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int TenantId { get; set; }
        public long UTracTenantId { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string ResturantName { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? OrderDoneTime { get; set; }
        public bool IsSentEvaluation { get; set; }
    }
    public enum UTracOrderStatusEunm
    {
        Pending = 0,
        Done = 1,
        Cancel = 2
    }
}

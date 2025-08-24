using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class OrderDashModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public DateTime CreationTime { get; set; }= DateTime.UtcNow;
        public DateTime ActionTime { get; set; } = DateTime.UtcNow;
        public int OrderStatus { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public int Count_OrderStatus_2 { get; set; }
        public int Count_OrderStatus_3 { get; set; }
        public int Avg_ActionTime_Minutes { get; set; }
        public string Avg_ActionTime { get; set; }
        public long totalOrders { get; set; }

    }
}

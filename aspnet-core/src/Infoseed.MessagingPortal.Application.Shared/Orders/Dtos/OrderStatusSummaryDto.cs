using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class OrderStatusSummaryDto
    {
        public string Period { get; set; }
        public int TotalOrders { get; set; }
        public int Pending { get; set; }
        public int Done { get; set; }
        public int Deleted { get; set; }
        public int Canceled { get; set; }
        public int PreOrder { get; set; }
        public int? DayCount { get; set; }
    }
}

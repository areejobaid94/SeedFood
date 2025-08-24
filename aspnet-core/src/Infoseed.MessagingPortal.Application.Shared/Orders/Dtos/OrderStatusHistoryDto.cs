using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class OrderStatusHistoryDto
    {
        public long OrderId { get; set; }
        public int DoneCount { get; set; }
        public int CancelOrDeleteCount { get; set; }
    }
}

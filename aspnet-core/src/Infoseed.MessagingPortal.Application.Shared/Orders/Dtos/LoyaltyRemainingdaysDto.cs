using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class LoyaltyRemainingdaysDto
    {
        public decimal Points { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int LoyaltyExpiration { get; set; }
        public long OrderNumber { get; set; }
        public decimal Total { get; set; }
        public int Remainingdays { get; set; }
        public  int OrderType { get; set; }
    }
}

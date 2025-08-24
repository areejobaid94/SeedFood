using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class OrderOfferCostModel
    {
        public decimal afterOrderDeliveryCost { get; set; }
        public decimal beforOrderDeliveryCost { get; set; }
    }
}

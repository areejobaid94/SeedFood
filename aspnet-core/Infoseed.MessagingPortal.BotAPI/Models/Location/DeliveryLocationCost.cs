using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models.Location
{
    public class DeliveryLocationCost
    {
        public int? TenantId { get; set; }

        public int Id { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public decimal DeliveryCost { get; set; }
        public int BranchAreaId { get; set; }
    }
}

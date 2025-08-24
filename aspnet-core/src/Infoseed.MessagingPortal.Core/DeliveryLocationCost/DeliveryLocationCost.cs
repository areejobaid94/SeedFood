using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryLocationCost
{
    [Table("DeliveryLocationCost")]
    public class DeliveryLocationCost 
    {
        public int? TenantId { get ; set; }

        public int Id { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public decimal DeliveryCost { get; set; }
        public int BranchAreaId { get; set; }
    }
}

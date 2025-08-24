using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryCost.Dto
{
    public class DeliveryCostDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public string  DeliveryCostJson { get; set; }
        public string AreaIds { get; set; }
        public decimal AboveValue   { get; set; }
        public string AreaNames { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
        public List<DeliveryCostDetailsDto> lstDeliveryCostDetailsDto { get; set; }


    }
}

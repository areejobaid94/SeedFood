using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.DeliveryCost.Dto
{
    public class DeliveryCostEntity
    {
        public List<DeliveryCostDto> lstDeliveryCostDto { get; set; }
        public int TotalCount { get; set; }
    }
}

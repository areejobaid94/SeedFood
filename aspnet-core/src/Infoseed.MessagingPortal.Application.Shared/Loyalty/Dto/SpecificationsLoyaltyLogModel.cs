using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Loyalty.Dto
{
    public class SpecificationsLoyaltyLogModel
    {
        public long Id { get; set; }
        public long SpecificationId { get; set; }
        public long SpecificationChoicesId { get; set; }

        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public int TenantId { get; set; }


        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsLatest { get; set; }

        public long LoyaltyDefinitionId { get; set; }


        public bool IsOverrideLoyaltyPoints { get; set; }
    }
}

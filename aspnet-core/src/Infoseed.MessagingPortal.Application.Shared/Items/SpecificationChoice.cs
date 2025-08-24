using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Items
{
    public class SpecificationChoice
    {
        public int Id { get; set; }
        public string SpecificationChoiceDescription { get; set; }
        public string SpecificationChoiceDescriptionEnglish { get; set; }

        public string SKU { get; set; }
        public bool IsInService { get; set; }
        
        public int LanguageBotId { get; set; }
        public int SpecificationId { get; set; }
        
        public int TenantId { get; set; }
        public decimal? Price { get; set; }
        public int UniqueId { get; set; }
        public int SpecificationUniqueId { get; set; }

        public decimal LoyaltyPoints { get; set; }

        public long LoyaltyDefinitionId { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications.Dtos
{
    public class SpecificationChoicesDto
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public string SpecificationChoiceDescription { get; set; }
        public string SpecificationChoiceDescriptionEnglish { get; set; }
        public string SKU { get; set; }
        public int LanguageBotId { get; set; }
        public int SpecificationId { get; set; }
        public decimal Price { get; set; }


        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }


        public long LoyaltyDefinitionId { get; set; }
        public long CreatedBy { get; set; }
    }
}

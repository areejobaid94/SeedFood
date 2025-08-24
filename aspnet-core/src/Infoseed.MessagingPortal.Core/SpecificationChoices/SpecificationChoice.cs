using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.SpecificationChoices
{
    [Table("SpecificationChoices")]
    public class SpecificationChoice : Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string SpecificationChoiceDescription { get; set; }
        public string SpecificationChoiceDescriptionEnglish { get; set; }
        public string SKU { get; set; }
        public int LanguageBotId { get; set; }
        public int SpecificationId { get; set; }
        public decimal Price { get; set; }
    }
}

using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications
{
    [Table("Specifications")]
    public class Specification : Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string SpecificationDescription { get; set; }
        public string SpecificationDescriptionEnglish { get; set; }
        public bool IsMultipleSelection { get; set; }
        public int MaxSelectNumber { get; set; }
        public int LanguageBotId { get; set; }
        public DateTime LastModificationTime { get; set; }



        public int CreatorUserId { get; set; }
        public int LastModifierUserId { get; set; }

        public bool IsDeleted { get; set; }

        public int DeleterUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime DeletionTime { get; set; }
        public string DeletionNote { get; set; }


        public decimal LoyaltyPoints { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }
    }
}

using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ItemSpecificationsDetails
{
    [Table("ItemSpecificationsDetail")]
    public class ItemSpecificationsDetail : Entity<long>, IMayHaveTenant
    {
        public int CopiedFromId { get; set; }
        public int? TenantId { get; set; }
        
            
        public int ItemId { get; set; }
        public int MenuType { get; set; }

        
        public int SpecificationChoicesId { get; set; }

        public bool IsInService { get; set; }
    }
}

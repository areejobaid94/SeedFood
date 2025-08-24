using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ItemSpecifications
{
    [Table("ItemSpecifications")]
    public class ItemSpecification : Entity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public int ItemId { get; set; }
        public int SpecificationId { get; set; }

        public bool IsRequired { get; set; }

    }
}

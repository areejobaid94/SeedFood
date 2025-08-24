using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAndAdditionsCategorys
{
    [Table("ItemAndAdditionsCategorys")]
    public class ItemAndAdditionsCategory : Entity<long>, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public int ItemId { get; set; }

        public int SpecificationId { get; set; }
        public int AdditionsCategorysId { get; set; }

        public int MenuType { get; set; }
    }
}

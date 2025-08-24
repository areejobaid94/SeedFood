using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Specifications.Dtos
{
   public class ItemSpecificationsDto
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public long ItemId { get; set; }
        public int SpecificationId { get; set; }

        public bool IsRequired { get; set; }

        public int MaxSelectNumber { get; set; }
    }
}

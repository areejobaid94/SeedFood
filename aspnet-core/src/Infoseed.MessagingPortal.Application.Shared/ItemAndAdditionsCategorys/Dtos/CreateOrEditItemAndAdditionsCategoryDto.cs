using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAndAdditionsCategorys.Dtos
{
   public  class CreateOrEditItemAndAdditionsCategoryDto
    {
        public int AdditionsAndItemId { get; set; }
        public long Id { get; set; }
        public int? TenantId { get; set; }

        public int ItemId { get; set; }

        public int SpecificationId { get; set; }
        public int AdditionsCategorysId { get; set; }

        public int MenuType { get; set; }
    }
}

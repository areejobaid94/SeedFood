using Infoseed.MessagingPortal.Specifications.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos
{
    public class ItemCategoryEntity
    {
        public List<GetItemAdditionsCategorysModel> lstItemAdditionsCategory { get; set; }
        public List<GetSpecificationsCategorysModel> lstSpecificationsCategory { get; set; }
        public int TotalCount { get; set; }
    }
}

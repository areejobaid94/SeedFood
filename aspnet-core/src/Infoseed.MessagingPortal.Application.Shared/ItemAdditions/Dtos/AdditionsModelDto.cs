using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ItemAdditions.Dtos
{
    public class AdditionsModelDto
    {
        public CreateOrEditItemAdditionCategoryDto createOrEditItemAdditionCategoryDto { get; set; }
        public List<ItemAdditionDto> itemAdditions { get; set; }
        public long LoyaltyDefinitionId { get; set; }

        public long CreatedBy { get; set; }
    }
}

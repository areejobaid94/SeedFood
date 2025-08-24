using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Menus.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.InfoSeedParser
{
   public  class ParseMenuModel
    {
        public List<CreateOrEditMenuDto> Menu { get; set; }
        public List<CreateOrEditMenuCategoryDto> Category { get; set; }
        public List<ItemDto> Item { get; set; }
        public ItemAdditionDto[] itemAdditionDtos { get; set; }
    }
}

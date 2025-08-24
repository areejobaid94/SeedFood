using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus
{
   public  class CategorysInItemModel
    {

        public string bgImg { get; set; }

        public string logImg { get; set; }

        public int menuPriority { get; set; }

        public long menuId { get; set; }

        public long categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryNameEnglish { get; set; }


        public bool isSubCategory { get; set; }

        public List<SubCategorysInItemModel> subCategorysInItemModels{ get; set; }


       public List<ItemDto>  listItemInCategories { get; set; }

        
    }
}

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{


    public class OrderingMenuMenuModel
    {


        public long Id { get; set; }
        public string MenuName { get; set; }

        public string MenuNameEnglish { get; set; }
        public string MenuNameArabic { get; set; }
        public int Priority { get; set; }
        public string ImageUri { get; set; }
        public List<OrderingMenuCategoryModel> lstOrderingMenuCategoryModel { get; set; }

        public List<OrderingMenuCategoryModel> lstOrderingMenuCategoryLoyaltyModel { get; set; }
    }


    public class OrderingMenuCategoryModel
    {

        public string BgImg { get; set; }

        public string LogImg { get; set; }

        public int MenuPriority { get; set; }

        public long MenuId { get; set; }

        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameEnglish { get; set; }

        public string CategoryNameArabic { get; set; }
        public bool IsSubCategory { get; set; }

        public List<OrderingMenuItemModel> lstOrderingMenuItemModel { get; set; }
        public List<OrderingMenuSubCategoryModel> lstOrderingMenuSubCategoryModel { get; set; }
        public List<OrderingMenuSubCategoryModel> AlllstOrderingMenuSubCategoryModel { get; set; }
    }
}

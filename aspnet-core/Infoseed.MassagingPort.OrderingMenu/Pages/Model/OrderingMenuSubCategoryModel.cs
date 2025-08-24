namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuSubCategoryModel
    {

        public long ItemCategoryId { get; set; }
        public long ItemSubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryNameEnglish { get; set; }
        public int MenuPriority { get; set; }
        public long MenuId { get; set; }


      
        public List<OrderingMenuItemModel> lstOrderingMenuItemModel { get; set; }
    }
}

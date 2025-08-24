using Microsoft.AspNetCore.Mvc;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuItemPagenationModel
    {
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public long SubCategoryId { get; set; }
        public string? SubCategoryName{ get; set; }
        public string Search { get; set; }
        public bool IsApplayLoyal { get; set; }
        public List<OrderingMenuItemModel> lstOrderingMenuItemModel { get; set; }
    }
}

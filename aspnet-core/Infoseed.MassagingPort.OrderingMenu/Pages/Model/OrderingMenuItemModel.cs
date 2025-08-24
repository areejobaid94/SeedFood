using Infoseed.MassagingPort.OrderingMenu.Pages.Model;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuItemModel
    { public long Id { get; set; }
		public int Qty { get; set; }
		public string Ingredients { get; set; }
		public string ItemName { get; set; }
		public string ItemDescription { get; set; }
		public string ItemNameEnglish { get; set; }
		public string ItemDescriptionEnglish { get; set; }
        public string ItemNameArabic { get; set; }
        public string ItemDescriptionArabic { get; set; }
        public string CategoryNames { get; set; }
		public string CategoryNamesEnglish { get; set; }
		public string SubCategoryName { get; set; }
		public string SubCategoryNameEnglish { get; set; }
		public bool IsInService { get; set; }
		public string ImageUri { get; set; } 
		public decimal? Price { get; set; }
		public decimal? ViewPrice { get; set; }
		public int Priority { get; set; }
		public virtual string SKU { get; set; }
		public long ItemCategoryId { get; set; }
		public long ItemSubCategoryId { get; set; }
		public string Size { get; set; }
		public int TenantId { get; set; }
		public decimal? OldPrice { get; set; }
		public long MenuId { get; set; }
		public int MenuType { get; set; }
		public int LanguageBotId { get; set; }
		public string Barcode { get; set; }
		public string Discount { get; set; }
		public string DiscountImg { get; set; }
		public string AreaIds { get; set; }
		public bool IsQuantitative { get; set; }
       
        public bool HasOption { get; set; }
		public string CurrencyCode { get; set; }

		//loyalty
        public bool IsLoyal { get; set; }
        public long LoyaltyDefinitionId { get; set; }

		public decimal LoyaltyPoints { get; set; }

        public decimal OriginalLoyaltyPoints { get; set; }
        public bool IsOverrideLoyaltyPoints { get; set; }
        public decimal LoyaltycreditPoints { get; set; }

        public string InServiceIds { get; set; }



        public string AreaId { get; set; }

    }
}

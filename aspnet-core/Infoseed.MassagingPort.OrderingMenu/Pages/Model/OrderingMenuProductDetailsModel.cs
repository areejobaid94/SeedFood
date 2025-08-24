using Infoseed.MessagingPortal.Items.Dtos;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuProductDetailsModel
    {
        public long Id { get; set; }
        public string  Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string ImageUri { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? ViewPrice { get; set; }
        public decimal? ViewPoint { get; set; }
        public decimal LoyaltycreditPoints { get; set; }
        public decimal? TotalOrderNotComblet { get; set; }
        public string Discount { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDescriptionEnglish { get; set; }
        public string ItemDescriptionArabic { get; set; }
        public int Qty { get; set; }
        public int TenantId { get; set; }
        public int ContactId { get; set; }
        public bool IsInService { get; set; }
        public bool IsQuantitative { get; set; }
        public bool IsLoyalClick { get; set; }
        public bool IsLoyal { get; set; }
        public decimal LoyaltyPoints { get; set; }

        public bool IsApplayLoyalty { get; set; }
        public decimal OriginalLoyaltyPoints { get; set; }
        public long LoyaltyDefinitionId { get; set; }
        
        public bool IsOverrideLoyaltyPoints { get; set; }
        public List<OrderingMenuItemSpecificationsModel> lstOrderingMenuItemSpecificationsModel { get; set; }
       public List<OrderingMenuItemAdditionsCategorysModel> lstOrderingMenuItemAdditionsCategorysModel { get; set; }
        public List<ItemImagesModel> lstItemImages { get; set; }

        public bool ViewIsMultipleSelection { get; set; }
        public int ViewMaxSelectNumber { get; set; }

    }
    public class OrderingMenuItemAdditionsCategorysModel
    {
        public List<OrderingMenuItemAdditionModel> OrderingMenuItemAdditionModel { get; set; }
        public int AdditionsAndItemId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public string NameEnglish { get; set; }
        public string NameArabic{ get; set; }
        public bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }

    }
    public class OrderingMenuItemAdditionModel
    {
        public virtual long ItemAdditionsId { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEnglish { get; set; }
        public virtual string NameArabic { get; set; }
        public virtual decimal? price { get; set; }
        public virtual long? itemId { get; set; }
        public virtual string SKU { get; set; }
        public int MenuType { get; set; }
        public int LanguageBotId { get; set; }
        public virtual long? ItemAdditionsCategoryId { get; set; }
        public bool IsInService { get; set; }
        public int TenantId { get; set; }
        public string ImageUri { get; set; }


        public decimal LoyaltyPoints { get; set; }


        public decimal OriginalLoyaltyPoints { get; set; }
        public long LoyaltyDefinitionId { get; set; }

        public bool IsOverrideLoyaltyPoints { get; set; }

    }
    public class OrderingMenuItemSpecificationsModel
    {
        public int Id { get; set; }
        public int ItemSpecificationId { get; set; }
        public string SpecificationDescription { get; set; }
        public string SpecificationDescriptionEnglish { get; set; }
        public string SpecificationDescriptionArabic{ get; set; }
        public bool IsMultipleSelection { get; set; }
        public bool IsRequired { get; set; }

        public int MaxSelectNumber { get; set; }
        //public bool IsInService { get; set; }

        public int TenantId { get; set; }
        public int Priority { get; set; }

        public int UniqueId { get; set; }
        public List<OrderingMenuSpecificationChoiceModel> lstOrderingMenuSpecificationChoiceModel { get; set; }
    }
    public class OrderingMenuSpecificationChoiceModel
    {
        public long Id { get; set; }
        public string SpecificationChoiceDescription { get; set; }
        public string SpecificationChoiceDescriptionEnglish { get; set; }
        public string SpecificationChoiceDescriptionArabic { get; set; }
        public string SKU { get; set; }
        public bool IsInService { get; set; }

        public int LanguageBotId { get; set; }
        public int SpecificationId { get; set; }

        public int TenantId { get; set; }
        public decimal? Price { get; set; }
        public decimal? viewPrice { get; set; }
        public int UniqueId { get; set; }
        public int SpecificationUniqueId { get; set; }
        //loyalty


        public decimal LoyaltyPoints { get; set; }


        public decimal OriginalLoyaltyPoints { get; set; }
        public long LoyaltyDefinitionId { get; set; }

        public bool IsOverrideLoyaltyPoints { get; set; }
    }

   
}
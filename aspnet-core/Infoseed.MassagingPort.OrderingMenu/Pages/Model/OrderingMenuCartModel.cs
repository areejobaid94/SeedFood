namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuCartModel
    {

        public List<OrderingMenuItemModel> lstOrderingMenuCartModel { get; set; }
    }


    public class OrderingCartItemModel
    {
        public Guid CartItemId { get; set; }
        public long Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CreateDateTime { get; set; }
        public int ContactId { get; set; }
        public int Qty { get; set; }
        public decimal Total { get;  set; }
        public decimal CridetPoint { get; set; }
        public string ItemNote { get; set; }
        public bool HasItemNote { get; set; }

        public bool IsLoyal { get; set; }
        public bool IsLoyalClick { get; set; }

        public decimal LoyaltyPoints { get; set; }
        public decimal TotalLoyaltyPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }
        public List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel { get; set; }
        public List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModel { get; set; }
        
    }
    public class OrderingCartItemSpecificationModel
    {
        public long SpecificationId { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationNameEnglish { get; set; }
        public string SpecificationNameArabic { get; set; }

        public int TenantId { get; set; }
        public bool IsMultipleSelection { get; set; }
        public int MaxSelectNumber { get; set; }

        public List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel { get; set; }

    }

    public class OrderingCartItemSpecificationChoicesModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public long LoyaltyDefinitionId { get; set; }
        public decimal TotalLoyaltyPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }

        public decimal Total { get; set; }
        public int Qty { get; set; }
    }
    public class OrderingCartItemAdditionalModel
    {
        public long Id { get; set; }
        public long ItemAdditionsCategoryId { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public long LoyaltyDefinitionId { get; set; }
        public decimal TotalLoyaltyPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }

        public int Qty { get; set; }
    }



    public class OrderModel
    {

        public int TenantId { get; set; }
        public int ContactId { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }

        public decimal TotalPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }
        public string OrderNotes { get; set; }
        public bool HasNote { get; set; }

        public int OrderNumber { get; set; } = new Random().Next(0001, 9999);
        public List<OrderDetailsModel> lstOrderDetailsModel { get; set; }

    }

    public class OrderDetailsModel
    {

        public Guid CartItemId { get; set; }
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int ContactId { get; set; }

        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public decimal Discount { get; set; }
        public bool IsCondiments { get; set; }
        public bool IsCrispy { get; set; }
        public bool IsDeserts { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal UnitPrice { get; set; }
        public string ItemNote { get; set; }
        public bool HasItemNote { get; set; }

        public decimal UnitPoints { get; set; }

        public decimal TotalLoyaltyPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }

        public List<OrderDetailsExtraModel> lstOrderDetailsExtraModel { get; set; }

    }


    public class OrderDetailsExtraModel
    {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public int Quantity { get; set; }
        public int TenantId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal UnitPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }

        public decimal TotalLoyaltyPoints { get; set; }
        public virtual string SpecificationName { get; set; }
        public virtual string SpecificationNameEnglish { get; set; }
        public virtual string SpecificationNameArabic { get; set; }
        public virtual int SpecificationUniqueId { get; set; }
        public virtual int? SpecificationChoiceId { get; set; }
        public virtual int? TypeExtraDetails { get; set; }

        public virtual int? SpecificationId { get; set; }


        public bool IsMultipleSelection { get; set; }
        public int MaxSelectNumber { get; set; }


        // public List<OrderingCartItemModel> lstOrderingCartItemModel { get; set; }

    }
}
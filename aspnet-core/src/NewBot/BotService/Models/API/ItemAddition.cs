namespace BotService.Models.API
{
    public class ItemAddition
    {
        public virtual int Id { get; set; }
        public virtual long ItemAdditionsId { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEnglish { get; set; }

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
        public bool IsOverrideLoyaltyPoints { get; set; }

        public long LoyaltyDefinitionId { get; set; }

        public long CreatedBy { get; set; }
    }
}

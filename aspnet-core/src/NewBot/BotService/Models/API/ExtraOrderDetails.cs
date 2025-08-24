namespace BotService.Models.API
{
    public class ExtraOrderDetails
    {
        public int? TenantId { get; set; }
        public long? Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string NameEnglish { get; set; }
        public virtual int? Quantity { get; set; }

        public virtual decimal? UnitPrice { get; set; }
        public virtual string StringUnitPrice { get; set; }


        public virtual decimal? UnitPoints { get; set; }
        public virtual string StringUnitPoints { get; set; }


        public virtual decimal? Total { get; set; }
        public virtual string StringTotal { get; set; }

        public virtual decimal? TotalLoyaltyPoints { get; set; }
        public virtual string StringTotalLoyaltyPoints { get; set; }


        public virtual long? OrderDetailId { get; set; }


        public virtual string SpecificationName { get; set; }
        public virtual string SpecificationNameEnglish { get; set; }
        public virtual int SpecificationUniqueId { get; set; }

        public virtual int? SpecificationChoiceId { get; set; }
        public virtual int? SpecificationId { get; set; }
        public virtual int? TypeExtraDetails { get; set; }
    }
}

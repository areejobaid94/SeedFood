using System.Collections.Generic;
using System;

namespace BotService.Models.API
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }
        public virtual string StringUnitPrice { get; set; }

        public decimal? Total { get; set; }


        public decimal UnitPoints { get; set; }

        public decimal TotalLoyaltyPoints { get; set; }
        public virtual string StringTotal { get; set; }
        public decimal? Discount { get; set; }

        public decimal? TotalAfterDiscunt { get; set; }

        public decimal? Tax { get; set; }

        public decimal? TotalAfterTax { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }


        public long? OrderId { get; set; }

        public long? ItemId { get; set; }
        public virtual string ItemNote { get; set; }

        public bool IsCondiments { get; set; }
        public bool IsDeserts { get; set; }
        public bool IsCrispy { get; set; }



        public List<ExtraOrderDetails> extraOrderDetailsDtos { get; set; }
        public List<CategoryExtraOrderDetails> lstCategoryExtraOrderDetailsDto { get; set; }
    }
}

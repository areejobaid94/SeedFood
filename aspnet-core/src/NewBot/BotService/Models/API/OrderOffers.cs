using System;

namespace BotService.Models.API
{
    public class OrderOffers
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public bool isAvailable { get; set; }
        public bool isPersentageDiscount { get; set; }
        public DateTime OrderOfferStart { get; set; }
        public DateTime OrderOfferEnd { get; set; }



        public DateTime OrderOfferDateStart { get; set; }
        public DateTime OrderOfferDateEnd { get; set; }
        public string OrderOfferDateStartS { get; set; }
        public string OrderOfferDateEndS { get; set; }


        public string OrderOfferStartS { get; set; }
        public string OrderOfferEndS { get; set; }
        public string Cities { get; set; }
        public string Area { get; set; }

        public decimal FeesStart { get; set; }
        public decimal FeesEnd { get; set; }
        public decimal NewFees { get; set; }

        public bool isBranchDiscount { get; set; }
        public string BranchesName { get; set; }
        public string BranchesIds { get; set; }
    }
}

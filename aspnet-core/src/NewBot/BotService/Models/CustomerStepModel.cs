using System;

namespace BotService.Models
{
    public class CustomerStepModel
    {
        public int LangId { get; set; }
        public string LangString { get; set; } = "ar";
        public int ChatStepId { get; set; }
        public int ChatStepPervoiusId { get; set; }
        public int ChatStepNextId { get; set; }
        public int ChatStepLevelId { get; set; }
        public int ChatStepLevelPreviousId { get; set; }
        public int SelectedAreaId { get; set; }
        public long OrderId { get; set; }
        public long OrderNumber { get; set; }
        public string OrderTypeId { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal OrderDeliveryCost { get; set; }
        public int PageNumber { get; set; }

        public string Address { get; set; }

        public decimal? DeliveryCostAfter { get; set; }
        public decimal? DeliveryCostBefor { get; set; }

        public bool isOrderOfferCost { get; set; }

        public long LocationId { get; set; }
        public string LocationAreaName { get; set; }
        public string AddressLatLong { get; set; }
        public bool IsLinkMneuStep { get; set; }
        public bool IsNotSupportLocation { get; set; }
        public decimal Discount { get; set; }

        public int BotCancelOrderId { get; set; }
        public DateTime OrderCreationTime { get; set; }

    }
}

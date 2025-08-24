using System;

namespace Infoseed.MessagingPortal.BotAPI.Models.FlwosRestorant
{
    public class OrderDetalisRes
    {

        public long SelectedAreaId { get; set; }
        public long OrderId { get; set; }
        public long OrderNumber { get; set; }
        public string OrderTypeId { get; set; }
        public decimal? OrderTotal { get; set; }
        public decimal OrderDeliveryCost { get; set; }
        public int PageNumber { get; set; }

        public string Address { get; set; }

        public decimal? DeliveryCostAfter { get; set; }
        public decimal? DeliveryCostBefor { get; set; }
        public decimal? DeliveryOffer { get; set; }
        public decimal? Tax { get; set; } = 0;

        public bool isOrderOfferCost { get; set; }
        public string IsDeliveryOffer { get; set; }

        public long LocationId { get; set; }
        public string LocationAreaName { get; set; }
        public string AddressLatLong { get; set; }
        public bool IsLinkMneuStep { get; set; }
        public bool IsNotSupportLocation { get; set; }
        public decimal Discount { get; set; }

        public int BotCancelOrderId { get; set; }
        public DateTime? OrderCreationTime { get; set; } = DateTime.UtcNow;
        public string AgentIds { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }


        public string SelectDay { get; set; }
        public string SelectTime { get; set; }
        public bool IsPreOrder { get; set; }
        public string IsItemOffer { get; set; }

        public string BayType { get; set; }
        public string detailText { get; set; }
        public decimal ItemOffer { get; set; }


        public bool IsLiveChat { get; set; }

        public string EvaluationQuestionText { get; set; }
        public string EvaluationsReat { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? DeletionTime { get; set; }


    }
}

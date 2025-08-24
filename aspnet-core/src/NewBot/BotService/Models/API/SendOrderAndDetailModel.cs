namespace BotService.Models.API
{
    public class SendOrderAndDetailModel
    {
        public int TenantID { get; set; }
        public int ContactId { get; set; }
        public string lang { get; set; }

        public string MenuType { get; set; }

        public string captionQuantityText { get; set; }
        public string captionAddtionText { get; set; }
        public string captionTotalText { get; set; }
        public string captionTotalOfAllText { get; set; }


        public string TypeChoes { get; set; }

        public decimal? deliveryCostBefor { get; set; }
        public decimal? deliveryCostAfter { get; set; }


        public string DeliveryCostText { get; set; }

        public decimal? Cost { get; set; } = 0;
        public LocationInfoModel LocationInfo { get; set; }

        public bool isOrderOffer { get; set; }

        public string DeliveryCostTextTow { get; set; }

        public int LocationId { get; set; }
    }
}

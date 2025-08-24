using System.Collections.Generic;

namespace BotService.Models.API
{
    public class OrderAndDetailsModel
    {
        public decimal? total { get; set; }
        public int orderId { get; set; }
        public Order order { get; set; }
        public string DetailText { get; set; }

        public List<OrderDetail> orderDetailDtos { get; set; }
        public List<ExtraOrderDetails> extraOrderDetailsDtos { get; set; }
        public List<Item> itemDtos { get; set; }
        public LocationInfoModel GetLocationInfo { get; set; }
        public bool IsDiscount { get; set; }
        public decimal Discount { get; set; }


        public bool IsItemOffer { get; set; }
        public decimal? ItemOffer { get; set; }

        public bool IsDeliveryOffer { get; set; }
        public decimal? DeliveryOffer { get; set; }
    }
}

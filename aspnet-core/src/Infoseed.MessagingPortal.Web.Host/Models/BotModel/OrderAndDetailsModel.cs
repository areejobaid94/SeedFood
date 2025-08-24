using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class OrderAndDetailsModel
    {

        public decimal? total { get; set; }
        public int orderId { get; set; }
        public OrderDto  order { get; set; }
        public string DetailText { get; set; }

        public List<OrderDetailDto>  orderDetailDtos { get; set; }
        public List<ExtraOrderDetailsDto>  extraOrderDetailsDtos { get; set; }
        public List<ItemDto>  itemDtos { get; set; }

        public bool IsDiscount { get; set; }
        public decimal Discount { get; set; }

    }
}

using Infoseed.MessagingPortal.Orders;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuOrdersHistoryModel
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public decimal OrderTotalPoint { get; set; }

        public OrderStatusEunm OrderStatus { get; set; }
        public OrderTypeEunm OrderType { get; set; }
        public decimal? DeliveryCost { get; set; }
        public bool IsItemOffer { get; set; }
        public decimal ItemOffer { get; set; }

        public string OrderTime { get; set; }
        public DateTime? OrderDate { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }
        
        public List<OrderingOrdersItemModel> lstOrderingtemModel { get; set; }




    }


    public class OrderingOrdersDetailsModel
    {
        public string OrderStatus { get; set; }

        public List<OrderingOrdersItemModel> lstOrderingtemModel { get; set; }



    }
    public class OrderingOrdersItemModel
    {
        public int OrderId { get; set; }

        public long ItemId { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Qty { get; set; }
        public decimal Total { get; set; }
        public decimal? TotalPoint { get; set; }
        public string ItemNote { get; set; }

        public List<OrderingCartItemSpecificationModel> lstOrderingOrderItemSpecificationModel { get; set; }
        public List<OrderingCartItemAdditionalModel> lstOrderingOrderItemAdditionalModel { get; set; }

    }




}

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infoseed.MessagingPortal.Orders;

namespace BotService.Models.API
{
    public class Order
    {
        public long Id { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime OrderTime { get; set; }

        public string OrderRemarks { get; set; }

        public long OrderNumber { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? DeletionTime { get; set; }

        public DateTime? LastModificationTime { get; set; }


        public long? DeliveryChangeId { get; set; }

        public long? BranchId { get; set; }

        public int? ContactId { get; set; }

        public OrderStatusEunm orderStatus { get; set; }
        public OrderTypeEunm OrderType { get; set; }

        public virtual bool IsLockByAgent { get; set; }
        public virtual long AgentId { get; set; }
        public virtual string LockByAgentName { get; set; }

        public virtual decimal Total { get; set; }
        public virtual decimal TotalPoints { get; set; }
        public virtual string StringTotal { get; set; }

        public virtual string Address { get; set; }
        public virtual long? AreaId { get; set; }
        //public virtual string AreaName { get; set; }

        public virtual decimal? DeliveryCost { get; set; }
        public virtual decimal? AfterDeliveryCost { get; set; }



        public virtual int BranchAreaId { get; set; }

        public virtual string BranchAreaName { get; set; }


        public virtual string FromLocationDescribation { get; set; }
        public virtual string ToLocationDescribation { get; set; }
        public virtual string OrderDescribation { get; set; }

        public virtual bool IsSpecialRequest { get; set; }
        public virtual string SpecialRequestText { get; set; }

        public string SelectDay { get; set; }
        public string SelectTime { get; set; }
        public bool IsPreOrder { get; set; }

        public virtual string RestaurantName { get; set; }

        public string HtmlPrint { get; set; }

        public string BuyType { get; set; }

        public virtual DateTime ActionTime { get; set; }
        public string AgentIds { get; set; }
        public string OrderDetailDtoJson { get; set; }
        public int TenantId { get; set; }

        public string CaptionJson { get; set; }
        public string OrderOfferJson { get; set; }


        public bool IsItemOffer { get; set; }
        public decimal? ItemOffer { get; set; }

        public bool IsDeliveryOffer { get; set; }
        public decimal? DeliveryOffer { get; set; }


        public string OrderLocal { get; set; }

     
        public virtual bool IsEvaluation { get; set; }

        public virtual bool IsBranchArea { get; set; }

        public virtual int LocationID { get; set; }

        public virtual int FromLocationID { get; set; }
        public virtual int ToLocationID { get; set; }



        public virtual string FromLongitudeLatitude { get; set; }
        public virtual string ToLongitudeLatitude { get; set; }



      
    }
    //public enum OrderStatusEunm
    //{
    //    NoN = 0,
    //    Pending = 1,
    //    Done = 2,
    //    Deleted = 3,
    //    Draft = 4,
    //    Cancel = 5,
    //    Canceled = 6,
    //    Pre_Order = 7
    //}
    //public enum OrderTypeEunm
    //{
    //    Takeaway = 0,
    //    Delivery = 1,

    //}
}

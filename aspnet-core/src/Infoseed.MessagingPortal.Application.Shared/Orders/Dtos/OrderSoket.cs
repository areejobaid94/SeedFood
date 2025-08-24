using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders
{
    public class OrderSoket
    {
        public long Id { get; set; }
        public long CreatorUserId { get; set; }
        public long LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public long DeleterUserId { get; set; }
        public int TenantId { get; set; }
        public DateTime? OrderTime { get; set; } = DateTime.UtcNow;
        public string OrderRemarks { get; set; }
        public long OrderNumber { get; set; }
        public DateTime? CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime? DeletionTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModificationTime { get; set; } = DateTime.UtcNow;
        public long BranchId { get; set; }
        public int ContactId { get; set; }
        public long AgentId { get; set; }
        public bool IsLockByAgent { get; set; }
        public string LockByAgentName { get; set; }
        public decimal Total { get; set; }
        public decimal? Tax { get; set; }

        public OrderStatusEunm OrderStatus { get; set; }
        public OrderTypeEunm OrderType { get; set; }
        public string Address { get; set; }
        public long AreaId { get; set; }
        public decimal DeliveryCost { get; set; }
        public bool IsEvaluation { get; set; }
        public bool IsBranchArea { get; set; }
        public int BranchAreaId { get; set; }
        public string BranchAreaName { get; set; }
        public int LocationID { get; set; }
        public int FromLocationID { get; set; }
        public int ToLocationID { get; set; }
        public string FromLocationDescribation { get; set; }
        public string FromLongitudeLatitude { get; set; }
        public string OrderDescribation { get; set; }
        public string ToLocationDescribation { get; set; }
        public string ToLongitudeLatitude { get; set; }
        public decimal AfterDeliveryCost { get; set; }
        public bool IsSpecialRequest { get; set; }
        public string SpecialRequestText { get; set; }
        public bool IsPreOrder { get; set; }
        public string SelectDay { get; set; }
        public string SelectTime { get; set; }
        public string RestaurantName { get; set; }
        public string HtmlPrint { get; set; }
        public string BuyType { get; set; }
        public string OrderLocal { get; set; }
        public bool IsArchived { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }
        public DateTime? ActionTime { get; set; } = DateTime.UtcNow;
        public string AgentIds { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal TotalCreditPoints { get; set; }
        public bool IsItemOffer { get; set; }
        public decimal ItemOffer { get; set; }
        public bool IsDeliveryOffer { get; set; }
        public decimal DeliveryOffer { get; set; }
    }
}

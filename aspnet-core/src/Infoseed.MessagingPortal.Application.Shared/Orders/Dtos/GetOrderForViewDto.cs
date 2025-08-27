using System;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class GetOrderForViewDto
    {
		public OrderDto Order { get; set; }
        public LoyaltyRemainingdaysDto LoyaltyRemainingdays { get; set; }
		public decimal Total { set; get; }
        public decimal? Tax { set; get; }


        public int? ZeedlyOrderStatus { get; set; }         
        public string? DeliveryEstimation { get; set; }     
        public string? OrderDetailsCareem { get; set; }     
        public bool? IsZeedlyOrder { get; set; }

        public decimal TotalPoints { set; get; }
		public bool IsLockedByAgent { get; set; }
		public bool IsConversationExpired { get; set; }
		public DateTime LastMessageDate { get; set; } = DateTime.UtcNow;


		public string DeliveryChangeDeliveryServiceProvider { get; set;}

		public string CustomerID { get; set; }
		public string BranchName { get; set;}
		public string AreahName { get; set; }

		public string CustomerCustomerName { get; set;}
		public string CustomerMobile { get; set; }

		public string OrderStatusName { get; set;}
		public string OrderTypeName { get; set; }

		public string OrderNumber { get; set; }
		public string CreatTime { get; set; }
		public string CreatDate { get; set; }

		public string BranchAreaName { get; set; }
		

		public decimal? DeliveryCost { get; set; }
		public string StringdeliveryCost { get; set; }

		
		public  bool IsAssginToAllUser { get; set; }
		public  bool IsAvailableBranch { get; set; }
		public int? TenantId { get; set; }

        public string AreaCoordinatetow { get; set; }
        public string AreahNametow { get; set; }
		public string AreaCoordinatetowTak { get; set; }
		public string AreahNametowTak { get; set; }


		public string BuyType { get; set; }
		public string StreetName { get; set; }
		public string BuildingNumber { get; set; }
		public string FloorNo { get; set; }
		public string ApartmentNumber { get; set; }

        public string ActionTime { get; set; }
        public string AgentIds { get; set; }






    }
}
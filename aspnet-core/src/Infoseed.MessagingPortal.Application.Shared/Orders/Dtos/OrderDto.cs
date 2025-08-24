
using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.OrderDetails.Dtos;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class OrderDto : EntityDto<long>
    {
		public DateTime OrderTime { get; set; } = DateTime.Now;

		public string OrderRemarks { get; set; }

		public long OrderNumber { get; set; }

		public DateTime CreationTime { get; set; } = DateTime.Now;

        public DateTime? DeletionTime { get; set; } = DateTime.Now;

        public DateTime? LastModificationTime { get; set; } = DateTime.Now;


        public long? DeliveryChangeId { get; set; }

		public long? BranchId { get; set; }

		public int? ContactId { get; set; }

		public OrderStatusEunm OrderStatus { get; set; }
		public OrderTypeEunm OrderType { get; set; } 

		public virtual bool IsLockByAgent { get; set; }
		public virtual long AgentId { get; set; }
		public virtual string LockByAgentName { get; set; }

		public virtual decimal Total { get; set; }
        public virtual decimal? Tax { get; set; }

        public virtual decimal TotalPoints { get; set; }
		public virtual decimal TotalCreditPoints { get; set; }
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

		public virtual DateTime ActionTime { get; set; } = DateTime.Now;
        public string  AgentIds { get; set; }
		public string OrderDetailDtoJson { get; set; }
		public int TenantId { get; set; }

		public string CaptionJson { get; set; }
		public string OrderOfferJson { get; set; }


        public bool IsItemOffer { get; set; }
        public decimal? ItemOffer { get; set; }

        public bool IsDeliveryOffer { get; set; }
        public decimal? DeliveryOffer { get; set; }


		public string OrderLocal { get; set; }
        public short IsZeedlyOrder { get; set; }
        public ZeedlyOrderStatus ZeedlyOrderStatus { get; set; }
        public string DeliveryEstimation { get; set; }
	}


}
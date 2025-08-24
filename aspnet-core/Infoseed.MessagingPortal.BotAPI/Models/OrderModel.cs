using Infoseed.MessagingPortal.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class OrderModel
    {
		public int? TenantId { get; set; }


		[Required]
		public virtual DateTime OrderTime { get; set; }

		[StringLength(OrderConsts.MaxOrderRemarksLength, MinimumLength = OrderConsts.MinOrderRemarksLength)]
		public virtual string OrderRemarks { get; set; }

		[Required]
		public virtual long OrderNumber { get; set; }

		[Required]
		public virtual DateTime CreationTime { get; set; }

		public virtual DateTime? DeletionTime { get; set; }

		public virtual DateTime? LastModificationTime { get; set; }

		public virtual bool IsLockByAgent { get; set; }
		public virtual long AgentId { get; set; }
		public virtual string LockByAgentName { get; set; }

		public virtual decimal Total { get; set; }



		public virtual decimal? DeliveryCost { get; set; }

		public virtual decimal? AfterDeliveryCost { get; set; }


		public virtual long? BranchId { get; set; }


		public virtual int? ContactId { get; set; }


		//public virtual long? OrderStatusId { get; set; }		
		public OrderStatusEunm orderStatus { get; set; }

		public OrderTypeEunm OrderType { get; set; }
		public virtual string Address { get; set; }


		public virtual long? AreaId { get; set; }



		public virtual bool IsEvaluation { get; set; }


		public virtual bool IsBranchArea { get; set; }
		public virtual int BranchAreaId { get; set; }

		public virtual string BranchAreaName { get; set; }
		public virtual string RestaurantName { get; set; }


		public virtual int LocationID { get; set; }

		public virtual int FromLocationID { get; set; }
		public virtual int ToLocationID { get; set; }



		public virtual string FromLocationDescribation { get; set; }
		public virtual string ToLocationDescribation { get; set; }
		public virtual string OrderDescribation { get; set; }


		public virtual string FromLongitudeLatitude { get; set; }
		public virtual string ToLongitudeLatitude { get; set; }



		public virtual bool IsSpecialRequest { get; set; }
		public virtual string SpecialRequestText { get; set; }

		public string SelectDay { get; set; }
		public string SelectTime { get; set; }
		public bool IsPreOrder { get; set; }



		public string HtmlPrint { get; set; }
		public string BuyType { get; set; }
		public string OrderLocal { get; set; }
		public virtual DateTime? ActionTime { get; set; }
		public virtual string AgentIds { get; set; }
	}
}

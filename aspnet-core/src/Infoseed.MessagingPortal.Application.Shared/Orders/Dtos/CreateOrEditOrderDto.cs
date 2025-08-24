
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class CreateOrEditOrderDto : EntityDto<long?>
    {

		[Required]
		public DateTime OrderTime { get; set; }
		
		
		[StringLength(OrderConsts.MaxOrderRemarksLength, MinimumLength = OrderConsts.MinOrderRemarksLength)]
		public string OrderRemarks { get; set; }
		
		
		[Required]
		public long OrderNumber { get; set; }
		
		
		[Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		public DateTime? LastModificationTime { get; set; }
		
		
		 public long? DeliveryChangeId { get; set; }
		 
		 		 public long? BranchId { get; set; }
		 
		 		 public long? CustomerId { get; set; }
		 
		 		 public long? OrderStatusId { get; set; }
		public virtual string Address { get; set; }


		public virtual long? AreaId { get; set; }


		public virtual decimal? DeliveryCost { get; set; }


		public string HtmlPrint { get; set; }

	}
}
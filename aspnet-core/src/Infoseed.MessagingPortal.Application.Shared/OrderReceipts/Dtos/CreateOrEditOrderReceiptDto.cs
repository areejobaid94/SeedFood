
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderReceipts.Dtos
{
    public class CreateOrEditOrderReceiptDto : EntityDto<long?>
    {

		public DateTime? OrderTime { get; set; }
		
		
		public int? OrderAmount { get; set; }
		
		
		public decimal? OrderDiscount { get; set; }
		
		
		public decimal? TotalAfterDiscunt { get; set; }
		
		
		public bool IsCashReceived { get; set; }
		
		
		[Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		public DateTime? LastModificationTime { get; set; }
		
		
		 public long? OrderId { get; set; }
		 
		 
    }
}

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class CreateOrEditOrderDetailDto : EntityDto<long?>
    {

		public int? Quantity { get; set; }
		
		
		public decimal? UnitPrice { get; set; }
		
		
		public decimal? Total { get; set; }
		
		
		public decimal? Discount { get; set; }
		
		
		public decimal? TotalAfterDiscunt { get; set; }
		
		
		public decimal? Tax { get; set; }
		
		
		public decimal? TotalAfterTax { get; set; }
		
		
		[Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		public DateTime? LastModificationTime { get; set; }
		
		
		 public long? OrderId { get; set; }
		 
		 		 public long? itemId { get; set; }
		 
		 
    }
}
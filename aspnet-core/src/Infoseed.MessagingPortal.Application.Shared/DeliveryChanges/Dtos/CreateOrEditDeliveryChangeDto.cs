
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.DeliveryChanges.Dtos
{
    public class CreateOrEditDeliveryChangeDto : EntityDto<long?>
    {

		public decimal? Charges { get; set; }
		
		
		[StringLength(DeliveryChangeConsts.MaxDeliveryServiceProviderLength, MinimumLength = DeliveryChangeConsts.MinDeliveryServiceProviderLength)]
		public string DeliveryServiceProvider { get; set; }
		
		
		[Required]
		public DateTime CreationTime { get; set; }
		
		
		public DateTime? DeletionTime { get; set; }
		
		
		public DateTime? LastModificationTime { get; set; }
		
		
		 public long AreaId { get; set; }
		 
		 
    }
}
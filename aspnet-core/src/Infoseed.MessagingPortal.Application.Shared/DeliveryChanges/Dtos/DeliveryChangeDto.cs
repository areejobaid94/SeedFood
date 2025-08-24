
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.DeliveryChanges.Dtos
{
    public class DeliveryChangeDto : EntityDto<long>
    {
		public decimal? Charges { get; set; }

		public string DeliveryServiceProvider { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime? DeletionTime { get; set; }

		public DateTime? LastModificationTime { get; set; }


		 public long AreaId { get; set; }

		 
    }
}
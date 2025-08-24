using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.DeliveryChanges.Dtos
{
    public class GetAllDeliveryChangesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public decimal? MaxChargesFilter { get; set; }
		public decimal? MinChargesFilter { get; set; }

		public string DeliveryServiceProviderFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public DateTime? MaxLastModificationTimeFilter { get; set; }
		public DateTime? MinLastModificationTimeFilter { get; set; }


		 public string AreaAreaNameFilter { get; set; }

		 
    }
}
using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class GetAllOrdersInput : PagedAndSortedResultRequestDto
    {

		public string Filter { get; set; }


		public long? UserIdFilter { get; set; }

		public DateTime? MaxOrderTimeFilter { get; set; }
		public DateTime? MinOrderTimeFilter { get; set; }

		public string OrderRemarksFilter { get; set; }

		public long? MaxOrderNumberFilter { get; set; }
		public long? MinOrderNumberFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public DateTime? MaxLastModificationTimeFilter { get; set; }
		public DateTime? MinLastModificationTimeFilter { get; set; }


		 public string DeliveryChangeDeliveryServiceProviderFilter { get; set; }

		 		 public string BranchNameFilter { get; set; }

		 		 public string CustomerCustomerNameFilter { get; set; }

		 		 public string OrderStatusNameFilter { get; set; }
		public bool IsArchived { get; set; } = false;

		 
    }
}
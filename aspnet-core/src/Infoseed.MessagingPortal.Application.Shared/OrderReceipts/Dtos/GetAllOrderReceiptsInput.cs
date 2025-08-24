using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.OrderReceipts.Dtos
{
    public class GetAllOrderReceiptsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxOrderTimeFilter { get; set; }
		public DateTime? MinOrderTimeFilter { get; set; }

		public int? MaxOrderAmountFilter { get; set; }
		public int? MinOrderAmountFilter { get; set; }

		public decimal? MaxOrderDiscountFilter { get; set; }
		public decimal? MinOrderDiscountFilter { get; set; }

		public decimal? MaxTotalAfterDiscuntFilter { get; set; }
		public decimal? MinTotalAfterDiscuntFilter { get; set; }

		public int? IsCashReceivedFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public DateTime? MaxLastModificationTimeFilter { get; set; }
		public DateTime? MinLastModificationTimeFilter { get; set; }


		 public string OrderOrderRemarksFilter { get; set; }

		 
    }
}
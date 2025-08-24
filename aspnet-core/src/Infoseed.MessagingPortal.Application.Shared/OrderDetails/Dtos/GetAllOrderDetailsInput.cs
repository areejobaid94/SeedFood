using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class GetAllOrderDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public int? MaxQuantityFilter { get; set; }
		public int? MinQuantityFilter { get; set; }

		public decimal? MaxUnitPriceFilter { get; set; }
		public decimal? MinUnitPriceFilter { get; set; }

		public decimal? MaxTotalFilter { get; set; }
		public decimal? MinTotalFilter { get; set; }

		public decimal? MaxDiscountFilter { get; set; }
		public decimal? MinDiscountFilter { get; set; }

		public decimal? MaxTotalAfterDiscuntFilter { get; set; }
		public decimal? MinTotalAfterDiscuntFilter { get; set; }

		public decimal? MaxTaxFilter { get; set; }
		public decimal? MinTaxFilter { get; set; }

		public decimal? MaxTotalAfterTaxFilter { get; set; }
		public decimal? MinTotalAfterTaxFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public DateTime? MaxLastModificationTimeFilter { get; set; }
		public DateTime? MinLastModificationTimeFilter { get; set; }


		 public string OrderOrderRemarksFilter { get; set; }

		 		 public string MenuMenuNameFilter { get; set; }

		 
    }
}
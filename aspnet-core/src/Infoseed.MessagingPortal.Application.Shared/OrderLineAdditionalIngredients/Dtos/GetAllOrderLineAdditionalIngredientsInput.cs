using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos
{
    public class GetAllOrderLineAdditionalIngredientsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string RemarksFilter { get; set; }

		public decimal? MaxTotalFilter { get; set; }
		public decimal? MinTotalFilter { get; set; }

		public int? MaxQuantityFilter { get; set; }
		public int? MinQuantityFilter { get; set; }

		public decimal? MaxUnitPriceFilter { get; set; }
		public decimal? MinUnitPriceFilter { get; set; }


		 public string OrderOrderRemarksFilter { get; set; }

		 
    }
}
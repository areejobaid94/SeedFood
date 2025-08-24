using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.MenuDetails.Dtos
{
    public class GetAllMenuDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DescriptionFilter { get; set; }

		public int? IsStandAloneFilter { get; set; }

		public decimal? MaxPriceFilter { get; set; }
		public decimal? MinPriceFilter { get; set; }


		 public string ItemItemNameFilter { get; set; }

		 		 public string MenuMenuNameFilter { get; set; }

		 		 public string MenuItemStatusNameFilter { get; set; }

		 
    }
}
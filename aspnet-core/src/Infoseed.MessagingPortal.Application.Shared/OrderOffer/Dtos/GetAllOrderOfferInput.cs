using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.OrderOffer.Dtos
{
   public  class GetAllOrderOfferInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
		public string MenuNameFilter { get; set; }

		public string MenuDescriptionFilter { get; set; }

		public DateTime? MaxEffectiveTimeFromFilter { get; set; }
		public DateTime? MinEffectiveTimeFromFilter { get; set; }

		public DateTime? MaxEffectiveTimeToFilter { get; set; }
		public DateTime? MinEffectiveTimeToFilter { get; set; }

		public decimal? MaxTaxFilter { get; set; }
		public decimal? MinTaxFilter { get; set; }




		//public string MenuItemStatusNameFilter { get; set; }

		//public string MenuCategoryNameFilter { get; set; }

		public int PriorityFilter { get; set; }
	}
}

using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class GetAllItemsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string ItemDescriptionFilter { get; set; }

		public string IngredientsFilter { get; set; }

		public string ItemNameFilter { get; set; }

		public int? IsInServiceFilter { get; set; }

		public string CategoryNamesFilter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }

		public DateTime? MaxDeletionTimeFilter { get; set; }
		public DateTime? MinDeletionTimeFilter { get; set; }

		public DateTime? MaxLastModificationTimeFilter { get; set; }
		public DateTime? MinLastModificationTimeFilter { get; set; }
		public decimal? MinPriceFilter { get; set; }

		public string ImageUriFilter { get; set; }

		public int PriorityFilter { get; set; }

		public string MenuNameFilter { get; set; }

		public string CategoryNameFilter { get; set; }

	}
}
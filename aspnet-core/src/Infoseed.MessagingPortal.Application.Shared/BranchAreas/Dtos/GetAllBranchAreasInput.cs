using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.BranchAreas.Dtos
{
    public class GetAllBranchAreasInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxCreationTimeFilter { get; set; }
		public DateTime? MinCreationTimeFilter { get; set; }


		 public string AreaAreaNameFilter { get; set; }

		 		 public string BranchNameFilter { get; set; }

		 
    }
}
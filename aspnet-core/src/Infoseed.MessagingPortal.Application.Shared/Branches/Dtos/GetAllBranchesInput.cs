using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Branches.Dtos
{
    public class GetAllBranchesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}
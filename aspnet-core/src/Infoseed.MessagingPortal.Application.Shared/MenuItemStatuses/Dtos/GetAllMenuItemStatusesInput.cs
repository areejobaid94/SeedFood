using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Dtos
{
    public class GetAllMenuItemStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}
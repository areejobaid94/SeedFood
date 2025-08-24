using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.OrderStatuses.Dtos
{
    public class GetAllOrderStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }



    }
}
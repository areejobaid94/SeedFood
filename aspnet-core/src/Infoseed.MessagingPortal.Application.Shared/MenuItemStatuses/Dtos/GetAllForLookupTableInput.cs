using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
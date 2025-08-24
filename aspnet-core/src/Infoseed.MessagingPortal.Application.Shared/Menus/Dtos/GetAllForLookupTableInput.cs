using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
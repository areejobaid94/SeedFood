using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuCategories.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
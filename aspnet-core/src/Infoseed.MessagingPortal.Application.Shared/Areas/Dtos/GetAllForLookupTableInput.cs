using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Areas.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
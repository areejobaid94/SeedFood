using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Orders.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
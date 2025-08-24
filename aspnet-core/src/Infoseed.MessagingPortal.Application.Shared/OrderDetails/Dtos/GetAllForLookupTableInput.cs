using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
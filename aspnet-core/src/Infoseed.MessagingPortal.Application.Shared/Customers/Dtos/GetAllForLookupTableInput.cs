using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
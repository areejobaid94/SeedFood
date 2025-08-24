using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderReceipts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
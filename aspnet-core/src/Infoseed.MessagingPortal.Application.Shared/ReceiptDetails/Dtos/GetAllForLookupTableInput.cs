using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ReceiptDetails.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
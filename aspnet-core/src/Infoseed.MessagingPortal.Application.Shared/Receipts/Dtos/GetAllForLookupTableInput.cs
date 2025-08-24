using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Receipts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
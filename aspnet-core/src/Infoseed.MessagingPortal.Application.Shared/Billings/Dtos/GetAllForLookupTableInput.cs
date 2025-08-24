using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
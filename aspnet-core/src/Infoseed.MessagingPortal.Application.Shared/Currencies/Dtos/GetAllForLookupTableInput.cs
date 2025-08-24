using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Currencies.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
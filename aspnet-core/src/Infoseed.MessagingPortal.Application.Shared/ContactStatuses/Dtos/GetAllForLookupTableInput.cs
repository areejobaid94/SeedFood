using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ContactStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceStatuses.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
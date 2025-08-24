using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceFrequencies.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
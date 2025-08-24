using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceTypes.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
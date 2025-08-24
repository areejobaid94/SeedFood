using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
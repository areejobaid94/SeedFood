using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Forcasts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Banks.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
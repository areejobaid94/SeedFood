using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
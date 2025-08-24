using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
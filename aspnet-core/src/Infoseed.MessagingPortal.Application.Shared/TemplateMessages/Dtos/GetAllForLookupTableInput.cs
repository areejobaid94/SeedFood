using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.TemplateMessages.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
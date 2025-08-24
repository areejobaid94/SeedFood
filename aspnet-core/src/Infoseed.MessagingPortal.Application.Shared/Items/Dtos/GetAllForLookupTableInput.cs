using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Branches.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
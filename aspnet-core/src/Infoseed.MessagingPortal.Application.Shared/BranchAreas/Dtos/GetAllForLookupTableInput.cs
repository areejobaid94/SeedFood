using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.BranchAreas.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
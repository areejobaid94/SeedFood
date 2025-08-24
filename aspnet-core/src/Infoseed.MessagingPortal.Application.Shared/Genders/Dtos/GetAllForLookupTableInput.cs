using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Genders.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderLineAdditionalIngredients.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}
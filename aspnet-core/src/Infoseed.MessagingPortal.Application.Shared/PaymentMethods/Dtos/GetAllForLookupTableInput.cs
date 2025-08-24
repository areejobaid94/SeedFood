using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.PaymentMethods.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
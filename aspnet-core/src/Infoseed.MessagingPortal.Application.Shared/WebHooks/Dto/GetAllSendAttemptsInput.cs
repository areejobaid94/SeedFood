using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.WebHooks.Dto
{
    public class GetAllSendAttemptsInput : PagedInputDto
    {
        public string SubscriptionId { get; set; }
    }
}

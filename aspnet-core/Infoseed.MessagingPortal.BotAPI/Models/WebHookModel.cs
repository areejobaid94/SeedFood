using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.WhatsApp.Dto;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class WebHookModel
    {
        public WhatsAppModel whatsApp { get; set; }
        public TenantModel tenant { get; set; }
        public CustomerModel customer { get; set; }
        public int TenantId { get; set; }
    }
}

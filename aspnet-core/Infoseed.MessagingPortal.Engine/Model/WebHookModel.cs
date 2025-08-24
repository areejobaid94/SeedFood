using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.WhatsApp.Dto;

namespace Infoseed.MessagingPortal.Engine.Model
{
    public class WebHookModel
    {
        public WhatsAppModel whatsApp { get; set; }
        public TenantModel tenant { get; set; }
        public CustomerModel customer { get; set; }
    }
}

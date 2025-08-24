using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class WhatsAppEntity
    {
        public List<WhatsAppCampaignModel> lstWhatsAppCampaignModel { get; set; }
        public List<MessageTemplateModel> lstWhatsAppTemplateModel { get; set; }
        public List<WhatsAppFreeMessageModel> lstWhatsAppMessageConversationModel { get; set; }
        public int TotalCount { get; set; }
    }
}

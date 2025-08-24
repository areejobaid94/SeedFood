using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppEntity
    {
        public List<WhatsAppCampaignModel> lstWhatsAppCampaignModel { get; set; }
        public List<MessageTemplateModel> lstWhatsAppTemplateModel { get; set; }
        public List<WhatsAppFreeMessageModel> lstWhatsAppFreeMessageModel { get; set; }
        public List<WhatsAppCampaignHistoryModel> lstwhatsAppCampaignHistoryModels { get; set; }
        public int TotalCount { get; set; }
    }
}

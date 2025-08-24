using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppCampaignHistoryModel
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int CampaignType { get; set; }
        public long CampaignId { get; set; }
        public long? TemplateId { get; set; }
        public long? SentByUserId { get; set; }
        public DateTime SentTime { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; }
        public System.Guid SentCampaignId { get; set; }
        public long Sent { get; set; }
        public long Delivered { get; set; }
        public long Read { get; set; }
        public long Failed { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.Model
{
    public class WhatsAppCampaignHistoryModel
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int CampaignType { get; set; }
        public long CampaignId { get; set; }
        public long TemplateId { get; set; }
        public long SentByUserId { get; set; }
        public DateTime SentTime { get; set; }

    }
}

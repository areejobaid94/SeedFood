using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.Model
{
    public class SendCampaignFailedModel
    {
   
        public int TenantId { get; set; }
        public long CampaignId { get; set; }
        public string PhoneNumber { get; set; }
        public int ContactId { get; set; }
        public long TemplateId { get; set; }
        public long ConversationMessageId { get; set; }
        public System.Guid SentCampaignId { get; set; }


    }
}

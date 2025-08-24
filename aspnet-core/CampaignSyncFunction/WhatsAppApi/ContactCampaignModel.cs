using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSyncFunction.WhatsAppApi
{
    public class ContactCampaignModel
    {

        public string PhoneNumber { get; set; }
        public int TenantId { get; set; }
        public long CampaignId { get; set; }
        public string ResultJson { get; set; }
        public string MessageId { get; set; }
        public int ContactId { get; set; }
        public long TemplateId { get; set; }
        public Guid SentCampaignId { get; set; }
        public decimal MessageRate { get; set; }
        public bool IsSent { get; set; } = false;

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
{
    public class WhatsAppScheduledCampaignModel
    {
        public long Id { get; set; }
        public string ContactsJson { get; set; }
        public int TenantId { get; set; }
        public long CampaignId { get; set; }
        public long TemplateId { get; set; }
        public DateTime SendDateTime { get; set; }
        public bool IsExternalContact { get; set; }
        public long CreatedByUserId { get; set; }

    }
}

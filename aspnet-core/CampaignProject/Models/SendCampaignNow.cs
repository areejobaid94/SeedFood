using System.Collections.Generic;
using System;

namespace CampaignProject.Models
{
    public class SendCampaignNow
    {
        public long rowId { get; set; }
        public long campaignId { get; set; }
        public string campaignName { get; set; }
        public string templateName { get; set; }
        public long templateId { get; set; }
        public bool IsExternal { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
        public string JopName { get; set; }
        public List<ListContactCampaign> contacts { get; set; }
        public MessageTemplateModel model { get; set; }
        public TemplateVariablles2 templateVariablles { get; set; }

    }
}

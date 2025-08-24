using CampaignSendNew.WhatsAppApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class CampinQueueNew
    {
        public long campaignId { get; set; }
        public long templateId { get; set; }
        public bool IsExternal { get; set; }
        public int TenantId { get; set; }
        public string campaignName { get; set; }
        public string D360Key { get; set; }
        public string AccessToken { get; set; }
        public string functionName { get; set; }
        public string msg { get; set; }
        public string type { get; set; }
        public long rowId { get; set; }
        public ListContactToCampin contacts { get; set; }
        public TemplateVariablles templateVariables { get; set; }
        public MessageTemplateModel messageTemplateModel { get; set; }

    }
}

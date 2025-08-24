using System.Collections.Generic;

namespace CampaignProject.Models
{
    public class CampaignModel
    {
        public long campaignId { get; set; }
        //public long templateId { get; set; }
        public int TenantId { get; set; }
        public int NumberMessagesPerSecond { get; set; }
        public int Port { get; set; }
        public string AccessToken { get; set; }
        public string D360Key { get; set; }
        public List<ListContactCampaign> contacts { get; set; }
        //public MessageTemplateModel messageTemplateModel { get; set; }

    }
}

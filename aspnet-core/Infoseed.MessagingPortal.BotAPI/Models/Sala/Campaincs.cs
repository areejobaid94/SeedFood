using System;

namespace Infoseed.MessagingPortal.BotAPI.Models.Sala
{
    public class Campaincs
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public long CampaignId { get; set; }
        public int TemplateId { get; set; }
        public string ContactsJson { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public bool IsExternalContact { get; set; }
        public string JobName { get; set; }
        public string CampaignName { get; set; }
        public string TemplateName { get; set; }
        public bool IsSent { get; set; }
        public string TemplateJson { get; set; }
        public string TemplateVariables { get; set; }
        public string HeaderVariablesTemplate { get; set; }
        public string URLButton1VariablesTemplate { get; set; }
        public string URLButton2VariablesTemplate { get; set; }
        public string CarouselVariablesTemplate { get; set; }
    }

}

using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class CampaginModel
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
        public string TemplateJson { get; set; }
        public List<ListContactToCampin> contacts { get; set; }

        public MessageTemplateModel model { get; set; }
        [JsonPropertyName("templateVariablles")]
        public TemplateVariablles templateVariablles { get; set; }

        [JsonPropertyName("headerVariabllesTemplate")]
        public HeaderVariablesTemplate headerVariabllesTemplate { get; set; }

        [JsonPropertyName("firstButtonURLVariabllesTemplate")]
        public FirstButtonURLVariabllesTemplate FirstButtonURLVariabllesTemplate { get; set; }


        [JsonPropertyName("secondButtonURLVariabllesTemplate")]
        public SecondButtonURLVariabllesTemplate SecondButtonURLVariabllesTemplate { get; set; }


        public CarouselVariabllesTemplate CarouselTemplate { get; set; }

        [JsonPropertyName("ButtonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate { get; set; }

    }
}

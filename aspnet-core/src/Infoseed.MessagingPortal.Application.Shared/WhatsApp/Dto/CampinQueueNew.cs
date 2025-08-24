using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
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

        public List<ListContactToCampin> contacts { get; set; }
        public TemplateVariablles templateVariables { get; set; }
        public HeaderVariablesTemplate headerVariabllesTemplate { get; set; }

        [JsonPropertyName("carouselVariabllesTemplate")]
        public CarouselVariabllesTemplate carouselVariabllesTemplate { get; set; }

        [JsonPropertyName("secondButtonURLVariabllesTemplate")]
        public FirstButtonURLVariabllesTemplate FirstButtonURLVariabllesTemplate { get; set; }

        [JsonPropertyName("firstButtonURLVariabllesTemplate")]
        public SecondButtonURLVariabllesTemplate SecondButtonURLVariabllesTemplate { get; set; }

        [JsonPropertyName("buttonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate { get; set; }



        public MessageTemplateModel messageTemplateModel { get; set; }
    }
}

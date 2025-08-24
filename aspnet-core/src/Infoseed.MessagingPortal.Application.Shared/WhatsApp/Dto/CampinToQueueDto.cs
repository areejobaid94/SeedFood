using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class CampinToQueueDto
    {
        public long campaignId { get; set; }
        public string campaignName { get; set; }
        public string templateName { get; set; }
        public string templateLanguage { get; set; }
        public long templateId { get; set; }
        public bool IsExternal { get; set; }
        public int TotalCount { get; set; }
        public int TotalOptOut { get; set; }
        public long groupId { get; set; } = 0;
        public List<ListContactToCampin> contacts { get; set; }
        public TemplateVariablles templateVariables { get; set; }
        public HeaderVariablesTemplate headerVariabllesTemplate { get; set; }
        public FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate { get; set; }
        public SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate { get; set; }
        public CarouselVariabllesTemplate CarouselTemplate { get; set; }

        [JsonPropertyName("buttonCopyCodeVariabllesTemplate")]
        public ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate { get; set; }
    }



}

using System;
using System.Collections.Generic;

namespace NewFunctionApp
{
    public class WhatsAppCampaignModel
    {
        public long id { get; set; }

        public string title { get; set; }

        public int status { get; set; }

        public string language { get; set; }

        public string sender { get; set; }
        public string fromNumber { get; set; }
        public long sent { get; set; }
        public long delivered { get; set; }
        public long read { get; set; }
        public long templateId { get; set; }
        public int tenantId { get; set; }
        public DateTime? sentTime { get; set; }
        public System.Guid SentCampaignId { get; set; }

    }

    public class WhatsAppMessageTemplateResult
    {
        public string messaging_product { get; set; }
        public List<MessageTemplateResultContact> contacts { get; set; }
        public List<MessageTemplateResultMessage> messages { get; set; }
    }

    public class MessageTemplateResultContact
    {
        public string input { get; set; }
        public string wa_id { get; set; }
    }

    public class MessageTemplateResultMessage
    {
        public string id { get; set; }
    }

}

using System;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppCampaignModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Language { get; set; }
        public long Sent { get; set; }
        public long Delivered { get; set; }
        public long Read { get; set; }
        public long Failed { get; set; }
        public long Replied { get; set; }
        public long Hanged { get; set; }
        public long TemplateId { get; set; }
        public int TenantId { get; set; }
        public DateTime? SentTime { get; set; } = DateTime.UtcNow;
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;


        public int Type { get; set; }


        public long TotalNumbers { get; set; }
        public float SentPercentage { get; set; }
        public float DeliveredPercentage { get; set; }
        public float ReadPercentage { get; set; }
        public float RepliedPercentage { get; set; }
        public float HangedPercentage { get; set; }
        public float FailedPercentage { get; set; }


        public string TemplateName { get; set; }



        public string UserName { get; set; }


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

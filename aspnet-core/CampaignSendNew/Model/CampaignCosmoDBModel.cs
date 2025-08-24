using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class CampaignCosmoDBModel
    {
        public ContainerItemTypes itemType { get; set; }
        public string messagesId { get; set; }
        public string campaignId { get; set; }
        public long templateId { get; set; }
        public string phoneNumber { get; set; }
        public string contactName { get; set; }
        public string contactId { get; set; }
        public string msg { get; set; }
        public string templateName { get; set; }
        public string campaignName { get; set; }
        public string type { get; set; }
        public string mediaUrl { get; set; }
        public bool isRead { get; set; }
        public bool isSent { get; set; }
        public bool isDelivered { get; set; }
        public bool isFailed { get; set; }
        public bool isReplied { get; set; }
        public int tenantId { get; set; }
        public DateTime sendTime { get; set; }
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }

    }
}

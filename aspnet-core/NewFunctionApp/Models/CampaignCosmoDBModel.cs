using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewFunctionApp.Models
{
    public class CampaignCosmoDBModel
    {
        public string messagesId { get; set; } = "";
        public string campaignId { get; set; } = "";
        public string templateId { get; set; } = "";
        public string phoneNumber { get; set; } = "";
        public string contactName { get; set; } = "";
        public string contactId { get; set; } = "";
        public string templateName { get; set; } = "";
        public string campaignName { get; set; } = "";
        public string msg { get; set; } = "";
        public string type { get; set; }
        public string mediaUrl { get; set; } = "";
        public bool isRead { get; set; } = false;
        public bool isSent { get; set; } = false;
        public bool isDelivered { get; set; } = false;
        public bool isFailed { get; set; } = false;
        public bool isReplied { get; set; } = false;
        public int tenantId { get; set; } = 0;
        public DateTime? sendTime { get; set; } = new DateTime();
        public int itemType { get; set; } = 0;
        public string id { get; set; } = "";
        public string _rid { get; set; } = "";
        public string _self { get; set; } = "";
        public string _etag { get; set; } = "";
        public string _attachments { get; set; } = "";
        public long _ts { get; set; } = 0;
    }
}

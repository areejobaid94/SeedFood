using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampaignSyncFunction
{
    public class CustomerChat
    {
        public DateTime lastNotificationsData { get; set; }
        public string notificationsText { get; set; }
        public string notificationID { get; set; }
        public int UnreadMessagesCount { get; set; }

        public int? TenantId { get; set; }
        public string messageId { get; set; }
        public string userId { get; set; }
        public string SunshineConversationId { get; set; }
        public DateTime CreateDate { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public int status { get; set; }
        public string fileName { get; set; }
        public MessageSenderType sender { get; set; }
        public string mediaUrl { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }
        public ContainerItemTypes ItemType { get; } = ContainerItemTypes.ConversationItem;
        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
    }
    public enum MessageSenderType
    {
        Customer=1,
        TeamInbox=2
    }

    public  enum Messagestatus 
    {
      New=1,
      Read=2,
    }
}

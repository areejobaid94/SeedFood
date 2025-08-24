using Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Customer
{
    public class CustomerChat
    {
        public string messageId { get; set; }
        public string userId { get; set; }
        public string SunshineConversationId { get; set; }
        public DateTime CreateDate { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public int status { get; set; }
        public MessageSenderType sender { get; set; }
        public string mediaUrl { get; set; }
        public string agentName { get; set; }
        public string agentId { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; } = InfoSeedContainerItemTypes.ConversationItem;
    }
    public enum MessageSenderType
    {
        Customer = 1,
        TeamInbox = 2
    }

    public enum Messagestatus
    {
        New = 1,
        Read = 2,
    }
}

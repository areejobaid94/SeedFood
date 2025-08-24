using System;

namespace Infoseed.MessagingPortal.Web.Models.LiveChat
{
    public class LiveChatModel
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestedLiveChatTime { get; set; }
    }
}

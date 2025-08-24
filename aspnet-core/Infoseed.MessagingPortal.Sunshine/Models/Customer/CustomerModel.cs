using Framework.Data;
using System;

namespace Infoseed.MessagingPortal.Sunshine.Models
{
    public class CustomerModel
    {
  
      
        public string userId { get; set; }
        public string avatarUrl { get; set; }
        public string displayName { get; set; }
        public string phoneNumber { get; set; }
        public string type { get; set; }
        public string MicrosoftBotId { get; set; }
        public string SunshineConversationId { get; set; }
        public string SunshineAppID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime LastMessageData { get; set; }
        public bool IsLockedByAgent { get; set; }
        public bool IsConversationExpired { get; set; }
        public string LockedByAgentName { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; } = InfoSeedContainerItemTypes.CustomerItem;
        public CustomerStatus CustomerStatus { get; set; }
        public int CustomerStatusID
        {
            get { return (int)this.CustomerStatus; }
            set { this.CustomerStatus = (CustomerStatus)value; }
        }
        public CustomerChatStatus CustomerChatStatus { get; set; }
        public int CustomerChatStatusID
        {
            get { return (int)this.CustomerChatStatus; }
            set { this.CustomerChatStatus = (CustomerChatStatus)value; }
        }

        public bool IsOpen { get; set; }

        public string Website { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }
        public bool IsNew { get; set; }

        public int LiveChatStatus { get; set; }

        public bool IsliveChat { get; set; }
        public DateTime requestedLiveChatTime { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }
    }
}

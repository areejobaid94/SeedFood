//using BotService.Models.API;
//using System;

//namespace BotService.Models
//{
//    public class CustomerModel
//    {
//        public int ConversationsCount { get; set; }
//        public int DeliveryOrder { get; set; }
//        public int TakeAwayOrder { get; set; }
//        public int TotalOrder { get; set; }
//        public int loyalityPoint { get; set; }

//        public string ContactID { get; set; }
//        public bool IsSelectedPage { get; set; } = false;
//        public bool IsComplaint { get; set; }
//        public bool IsBotCloseChat { get; set; }
//        public bool IsBotChat { get; set; }
//        public bool IsSupport { get; set; }

//        public bool IsNewContact { get; set; }
//        public bool IsBlock { get; set; }
//        public CustomerChat customerChat { get; set; }
//        public string blockByAgentName { get; set; }
//        public bool isBlockCustomer { get; set; }
//        public DateTime lastNotificationsData { get; set; }
//        public string notificationsText { get; set; }
//        public string notificationID { get; set; }
//        public int UnreadMessagesCount { get; set; }
//        public int agentId { get; set; }
//        public string userId { get; set; }
//        public string avatarUrl { get; set; }
//        public string displayName { get; set; }
//        public string phoneNumber { get; set; }
//        public string type { get; set; }
//        public string MicrosoftBotId { get; set; }
//        public string ConversationId { get; set; }

//        public string D360Key { get; set; }
//        public string SunshineAppID { get; set; }
//        public DateTime CreateDate { get; set; }
//        public DateTime? ModifyDate { get; set; }
//        public DateTime LastMessageData { get; set; }
//        public DateTime LastConversationStartDateTime { get; set; }
//        public string LastMessageText { get; set; }
//        public bool IsLockedByAgent { get; set; }
//        public bool IsConversationExpired { get; set; }
//        public string LockedByAgentName { get; set; }
//        public InfoSeedContainerItemTypes ItemType { get; } = InfoSeedContainerItemTypes.CustomerItem;
//        public CustomerStatus CustomerStatus { get; set; }
//        public int CustomerStatusID
//        {
//            get { return (int)this.CustomerStatus; }
//            set { this.CustomerStatus = (CustomerStatus)value; }
//        }
//        public CustomerChatStatus CustomerChatStatus { get; set; }
//        public int CustomerChatStatusID
//        {
//            get { return (int)this.CustomerChatStatus; }
//            set { this.CustomerChatStatus = (CustomerChatStatus)value; }
//        }


//        public bool IsOpen { get; set; }

//        public string Website { get; set; }

//        public string EmailAddress { get; set; }

//        public string Description { get; set; }
//        public bool IsNew { get; set; }

//        public int? TenantId { get; set; }


//        public int LiveChatStatus { get; set; }
//        public string LiveChatStatusName { get; set; }
//        public bool IsliveChat { get; set; }
//        public DateTime requestedLiveChatTime { get; set; }


//        public DateTime OpenTime { get; set; }
//        public DateTime CloseTime { get; set; }
//        public string Department { get; set; }

//        public string id { get; set; }
//        public string _rid { get; set; }
//        public string _self { get; set; }
//        public string _etag { get; set; }
//        public string _attachments { get; set; }
//        public long _ts { get; set; }
//        public CustomerStepModel CustomerStepModel { get; set; }
//    }
//}

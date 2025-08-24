using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class CustomerLiveChatModel
    {
        
        public long AssignedToUserId { get; set; }
        public string AssignedToUserName { get; set; }
        public string DepartmentUserIds { get; set; }
        public long IdLiveChat { get; set; }
        public int ConversationsCount { get; set; }
        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }

        public string ContactID { get; set; }
        public bool IsSelectedPage { get; set; } = false;
        public bool IsComplaint { get; set; }
        public bool IsBotCloseChat { get; set; }
        public bool IsBotChat { get; set; }
        public bool IsSupport { get; set; }

        public bool IsNewContact { get; set; }
        public bool IsBlock { get; set; }
        public string blockByAgentName { get; set; }
        public bool isBlockCustomer { get; set; }
        public DateTime? lastNotificationsData { get; set; }
        public string notificationsText { get; set; }
        public string notificationID { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int agentId { get; set; }
        public string userId { get; set; }
        public string avatarUrl { get; set; }
        public string displayName { get; set; }
        public string phoneNumber { get; set; }
        public string type { get; set; }
        public string MicrosoftBotId { get; set; }
        public string ConversationId { get; set; }

        public string D360Key { get; set; }
        public string SunshineAppID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? LastMessageData { get; set; }
        public DateTime? LastConversationStartDateTime { get; set; }
        public string LastMessageText { get; set; }
        public bool IsLockedByAgent { get; set; }
        public bool IsConversationExpired { get; set; }
        public string LockedByAgentName { get; set; }




        public bool IsOpen { get; set; }

        public string Website { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }
        public bool IsNew { get; set; }

        public int? TenantId { get; set; }


        public int LiveChatStatus { get; set; }
        public string LiveChatStatusName { get; set; }
        public bool IsliveChat { get; set; }
        public DateTime? requestedLiveChatTime { get; set; }


        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public string Department { get; set; }


        public DateTime? OpenTimeTicket { get; set; }
        public DateTime? CloseTimeTicket { get; set; }
        public int timeToOpen { get; set; }
        public string TicketSummary { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public long _ts { get; set; }

        public int creation_timestamp { get; set; }
        public int expiration_timestamp { get; set; }

        public int DurationTime { get; set; }

        public string UserIds { get; set; }

        public string RequestDescription { get; set; }
        public string ContactInfo { get; set; }
        public List<SellingRequestDetailsDto> lstSellingRequestDetailsDto { get; set; }
        public bool IsRequestForm { get; set; } = false;
        public long? AreaId { get; set; }
        public int ContactId { get; set; }
        public string CategoryType { get; set; }
        public DateTime? ActionTime { get; set; }

        public bool IsEvaluation { get; set; } = false;
    }
}

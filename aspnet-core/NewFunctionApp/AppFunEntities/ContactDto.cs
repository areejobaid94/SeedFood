using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class ContactDto
    {
       public int Id { get; set; }
        public string DisplayName { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsLockedByAgent { get; set; }
        public bool IsConversationExpired { get; set; }

        public string LockedByAgentName { get; set; }

        public bool IsBlock { get; set; }
        public bool IsOpen { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public int? ChatStatuseId { get; set; }

        public int? ContactStatuseId { get; set; }
        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }
        public string UserId { get; set; }


        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }
        public string ConversationId { get; set; }


        public string Website { get; set; }
        public string Description { get; set; }
        public string ContactDisplayName { get; set; }
        public int? TenantId { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? CreationTime { get; set; } = DateTime.MinValue;
        public bool? IsDeleted { get; set; }

        public int? ConversationsCount { get; set; }
        public long? CreatorUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; } = DateTime.UtcNow;
        public string SunshineAppID { get; set; }

        public int CustomerOPT { get; set; }
        public int? TotalLiveChat { get; set; }
        public int? TotalRequest { get; set; }
        public string Branch { get; set; }
        public int? OrderType { get; set; }
        public string Group { get; set; }
        public string GroupName { get; set; }
    }
}

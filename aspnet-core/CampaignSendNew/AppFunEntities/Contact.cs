
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CampaignSendNew
{
  
    public class Contact
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }

        public virtual string AvatarUrl { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string SunshineAppID { get; set; }

        public virtual bool IsLockedByAgent { get; set; }
        public virtual bool IsBlock { get; set; }

        public virtual bool IsConversationExpired { get; set; }

        public virtual string LockedByAgentName { get; set; }

        public virtual bool IsOpen { get; set; }

        public virtual string Website { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string Description { get; set; }

        public virtual string UserId { get; set; }

        public virtual int? ChatStatuseId { get; set; }


        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }


        public int ConversationsCount { get; set; }


        public  string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }

    }
}
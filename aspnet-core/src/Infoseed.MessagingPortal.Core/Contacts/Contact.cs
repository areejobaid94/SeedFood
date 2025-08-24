using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.ContactStatuses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Contacts
{
    [Table("Contacts")]
    public class Contact : FullAuditedEntity, IMayHaveTenant
    {
        //public int AgentId { get; set; }
        public int? TenantId { get; set; }

        [StringLength(ContactConsts.MaxAvatarUrlLength, MinimumLength = ContactConsts.MinAvatarUrlLength)]
        public virtual string AvatarUrl { get; set; }

        [StringLength(ContactConsts.MaxDisplayNameLength, MinimumLength = ContactConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(ContactConsts.MaxPhoneNumberLength, MinimumLength = ContactConsts.MinPhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }

        [StringLength(ContactConsts.MaxSunshineAppIDLength, MinimumLength = ContactConsts.MinSunshineAppIDLength)]
        public virtual string SunshineAppID { get; set; }

        public virtual bool IsLockedByAgent { get; set; }
        public virtual bool IsBlock { get; set; }

        public virtual bool IsConversationExpired { get; set; }

        [StringLength(ContactConsts.MaxLockedByAgentNameLength, MinimumLength = ContactConsts.MinLockedByAgentNameLength)]
        public virtual string LockedByAgentName { get; set; }

        [Required]
        public virtual bool IsOpen { get; set; }

        [StringLength(ContactConsts.MaxWebsiteLength, MinimumLength = ContactConsts.MinWebsiteLength)]
        public virtual string Website { get; set; }

        [StringLength(ContactConsts.MaxEmailAddressLength, MinimumLength = ContactConsts.MinEmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        [StringLength(ContactConsts.MaxDescriptionLength, MinimumLength = ContactConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        public virtual string UserId { get; set; }

        public virtual int? ChatStatuseId { get; set; }

        [ForeignKey("ChatStatuseId")]
        public ChatStatuse ChatStatuseFk { get; set; }

        public virtual int? ContactStatuseId { get; set; }

        [ForeignKey("ContactStatuseId")]
        public ContactStatuse ContactStatuseFk { get; set; }


        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }


        public int ConversationsCount { get; set; }


        public  string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorNo { get; set; }
        public string ApartmentNumber { get; set; }
        public string ConversationId { get; set; }
        public string ContactDisplayName { get; set; }

    }
}
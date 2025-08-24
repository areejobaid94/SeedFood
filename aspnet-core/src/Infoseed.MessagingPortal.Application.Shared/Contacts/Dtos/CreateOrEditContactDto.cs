using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class CreateOrEditContactDto : EntityDto<int?>
    {

        [StringLength(ContactConsts.MaxAvatarUrlLength, MinimumLength = ContactConsts.MinAvatarUrlLength)]
        public string AvatarUrl { get; set; }

        [StringLength(ContactConsts.MaxDisplayNameLength, MinimumLength = ContactConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [StringLength(ContactConsts.MaxPhoneNumberLength, MinimumLength = ContactConsts.MinPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        [StringLength(ContactConsts.MaxSunshineAppIDLength, MinimumLength = ContactConsts.MinSunshineAppIDLength)]
        public string SunshineAppID { get; set; }

        public bool IsLockedByAgent { get; set; }
        public virtual bool IsBlock { get; set; }


        public bool IsConversationExpired { get; set; }

        [StringLength(ContactConsts.MaxLockedByAgentNameLength, MinimumLength = ContactConsts.MinLockedByAgentNameLength)]
        public string LockedByAgentName { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [StringLength(ContactConsts.MaxWebsiteLength, MinimumLength = ContactConsts.MinWebsiteLength)]
        public string Website { get; set; }

        [StringLength(ContactConsts.MaxEmailAddressLength, MinimumLength = ContactConsts.MinEmailAddressLength)]
        public string EmailAddress { get; set; }

        [StringLength(ContactConsts.MaxDescriptionLength, MinimumLength = ContactConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public string UserId { get; set; }

        public int? ChatStatuseId { get; set; }

        public int? ContactStatuseId { get; set; }
        public string ContactID { get; set; }

    }
}
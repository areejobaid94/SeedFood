using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ContactStatuses
{
    [Table("ContactStatuses")]
    public class ContactStatuse : CreationAuditedEntity
    {

        [Required]
        [StringLength(ContactStatuseConsts.MaxContactStatusNameLength, MinimumLength = ContactStatuseConsts.MinContactStatusNameLength)]
        public virtual string ContactStatusName { get; set; }

        [Required]
        public virtual bool IsEnabled { get; set; }

    }
}
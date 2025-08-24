using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.ContactNotification
{
    [Table("ContactNotifications")]
    public  class ContactNotification : FullAuditedEntity
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string ContactId { get; set; }
        [Required]
        public virtual string NotificationId { get; set; }

        [Required]
        public virtual DateTime NotificationCreateDate { get; set; }

        [Required]
        public virtual string NotificationText { get; set; }

        [Required]
        public virtual int UserId { get; set; }
    }
}

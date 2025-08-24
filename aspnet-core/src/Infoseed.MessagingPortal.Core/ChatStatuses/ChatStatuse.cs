using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.ChatStatuses
{
    [Table("ChatStatuses")]
    public class ChatStatuse : CreationAuditedEntity
    {

        [Required]
        [StringLength(ChatStatuseConsts.MaxChatStatusNameLength, MinimumLength = ChatStatuseConsts.MinChatStatusNameLength)]
        public virtual string ChatStatusName { get; set; }

        [Required]
        public virtual bool IsEnabled { get; set; }

    }
}
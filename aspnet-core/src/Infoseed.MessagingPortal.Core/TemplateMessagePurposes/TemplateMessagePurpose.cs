using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes
{
    [Table("TemplateMessagePurposes")]
    public class TemplateMessagePurpose : Entity
    {

        [Required]
        [StringLength(TemplateMessagePurposeConsts.MaxPurposeLength, MinimumLength = TemplateMessagePurposeConsts.MinPurposeLength)]
        public virtual string Purpose { get; set; }

    }
}
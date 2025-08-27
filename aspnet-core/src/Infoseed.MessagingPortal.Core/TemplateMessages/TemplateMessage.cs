using Infoseed.MessagingPortal.TemplateMessagePurposes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Infoseed.MessagingPortal.TemplateMessages
{
    [Table("TemplateMessages")]
    [Audited]
    public class TemplateMessage : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        [StringLength(TemplateMessageConsts.MaxTemplateMessageNameLength, MinimumLength = TemplateMessageConsts.MinTemplateMessageNameLength)]
        public virtual string TemplateMessageName { get; set; }

        public virtual DateTime MessageCreationDate { get; set; }

        [Required]
        [StringLength(TemplateMessageConsts.MaxMessageTextLength, MinimumLength = TemplateMessageConsts.MinMessageTextLength)]
        public virtual string MessageText { get; set; }

        public virtual Guid AttachmentId { get; set; }

        public virtual int TemplateMessagePurposeId { get; set; }

        [ForeignKey("TemplateMessagePurposeId")]
        public TemplateMessagePurpose TemplateMessagePurposeFk { get; set; }
        public virtual string Category { get; set; }

    }
}
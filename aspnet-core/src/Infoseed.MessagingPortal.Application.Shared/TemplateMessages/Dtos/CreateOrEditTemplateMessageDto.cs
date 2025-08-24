using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TemplateMessages.Dtos
{
    public class CreateOrEditTemplateMessageDto : EntityDto<int?>
    {

        [Required]
        [StringLength(TemplateMessageConsts.MaxTemplateMessageNameLength, MinimumLength = TemplateMessageConsts.MinTemplateMessageNameLength)]
        public string TemplateMessageName { get; set; }

        public DateTime MessageCreationDate { get; set; }

        [Required]
        [StringLength(TemplateMessageConsts.MaxMessageTextLength, MinimumLength = TemplateMessageConsts.MinMessageTextLength)]
        public string MessageText { get; set; }

        public Guid AttachmentId { get; set; }

        public int TemplateMessagePurposeId { get; set; }

    }
}
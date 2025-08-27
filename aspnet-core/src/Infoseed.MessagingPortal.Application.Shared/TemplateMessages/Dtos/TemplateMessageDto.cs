using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.TemplateMessages.Dtos
{
    public class TemplateMessageDto : EntityDto
    {
        public int Id { get; set; }

        public string TemplateMessageName { get; set; }

        public DateTime MessageCreationDate { get; set; } = DateTime.UtcNow;

        public int TemplateMessagePurposeId { get; set; }
        public Guid AttachmentId { get; set; }
        public string Category { get; set; }


    }
}
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos
{
    public class TemplateMessagePurposeDto : EntityDto
    {
        public string Purpose { get; set; }

    }
}
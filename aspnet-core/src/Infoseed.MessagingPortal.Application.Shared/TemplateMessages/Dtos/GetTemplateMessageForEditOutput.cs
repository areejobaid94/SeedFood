using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TemplateMessages.Dtos
{
    public class GetTemplateMessageForEditOutput
    {
        public CreateOrEditTemplateMessageDto TemplateMessage { get; set; }

        public string TemplateMessagePurposePurpose { get; set; }

    }
}
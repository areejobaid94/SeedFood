using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos
{
    public class GetTemplateMessagePurposeForEditOutput
    {
        public CreateOrEditTemplateMessagePurposeDto TemplateMessagePurpose { get; set; }

    }
}
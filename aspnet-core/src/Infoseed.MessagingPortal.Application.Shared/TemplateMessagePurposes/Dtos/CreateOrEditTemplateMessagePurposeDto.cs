using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos
{
    public class CreateOrEditTemplateMessagePurposeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(TemplateMessagePurposeConsts.MaxPurposeLength, MinimumLength = TemplateMessagePurposeConsts.MinPurposeLength)]
        public string Purpose { get; set; }

    }
}
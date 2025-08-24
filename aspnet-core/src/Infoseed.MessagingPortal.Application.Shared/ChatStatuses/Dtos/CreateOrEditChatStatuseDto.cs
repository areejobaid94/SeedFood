using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ChatStatuses.Dtos
{
    public class CreateOrEditChatStatuseDto : EntityDto<int?>
    {

        [Required]
        [StringLength(ChatStatuseConsts.MaxChatStatusNameLength, MinimumLength = ChatStatuseConsts.MinChatStatusNameLength)]
        public string ChatStatusName { get; set; }

        [Required]
        public bool IsEnabled { get; set; }

    }
}
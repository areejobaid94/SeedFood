using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ChatStatuses.Dtos
{
    public class ChatStatuseDto : EntityDto
    {
        public string ChatStatusName { get; set; }

        public bool IsEnabled { get; set; }

    }
}
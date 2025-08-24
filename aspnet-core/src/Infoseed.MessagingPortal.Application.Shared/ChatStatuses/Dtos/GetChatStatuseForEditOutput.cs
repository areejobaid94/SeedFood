using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ChatStatuses.Dtos
{
    public class GetChatStatuseForEditOutput
    {
        public CreateOrEditChatStatuseDto ChatStatuse { get; set; }

    }
}
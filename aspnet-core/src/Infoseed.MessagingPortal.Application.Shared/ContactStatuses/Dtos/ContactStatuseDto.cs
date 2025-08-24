using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ContactStatuses.Dtos
{
    public class ContactStatuseDto : EntityDto
    {
        public string ContactStatusName { get; set; }

        public bool IsEnabled { get; set; }

    }
}
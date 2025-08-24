using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceStatuses.Dtos
{
    public class ServiceStatusDto : EntityDto
    {
        public string ServiceStatusName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
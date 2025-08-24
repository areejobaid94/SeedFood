using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceTypes.Dtos
{
    public class ServiceTypeDto : EntityDto
    {
        public string ServicetypeName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
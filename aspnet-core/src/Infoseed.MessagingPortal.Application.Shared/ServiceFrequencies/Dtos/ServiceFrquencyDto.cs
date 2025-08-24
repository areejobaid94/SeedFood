using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.ServiceFrequencies.Dtos
{
    public class ServiceFrquencyDto : EntityDto
    {
        public string ServiceFrequencyName { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceFrequencies.Dtos
{
    public class GetServiceFrquencyForEditOutput
    {
        public CreateOrEditServiceFrquencyDto ServiceFrquency { get; set; }

    }
}
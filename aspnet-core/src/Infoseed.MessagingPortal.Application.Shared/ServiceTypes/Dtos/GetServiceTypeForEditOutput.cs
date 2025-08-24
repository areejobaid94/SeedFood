using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceTypes.Dtos
{
    public class GetServiceTypeForEditOutput
    {
        public CreateOrEditServiceTypeDto ServiceType { get; set; }

    }
}
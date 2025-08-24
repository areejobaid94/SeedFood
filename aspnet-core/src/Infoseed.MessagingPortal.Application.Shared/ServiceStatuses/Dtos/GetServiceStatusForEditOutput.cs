using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ServiceStatuses.Dtos
{
    public class GetServiceStatusForEditOutput
    {
        public CreateOrEditServiceStatusDto ServiceStatus { get; set; }

    }
}
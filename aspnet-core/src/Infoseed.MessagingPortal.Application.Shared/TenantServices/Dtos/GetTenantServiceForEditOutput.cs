using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class GetTenantServiceForEditOutput
    {
        public CreateOrEditTenantServiceDto TenantService { get; set; }

        public string InfoSeedServiceServiceName { get; set; }

    }
}
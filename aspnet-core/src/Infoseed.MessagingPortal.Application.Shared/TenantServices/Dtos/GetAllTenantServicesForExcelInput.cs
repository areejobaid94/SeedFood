using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class GetAllTenantServicesForExcelInput
    {
        public string Filter { get; set; }

        public decimal? MaxServiceFeesFilter { get; set; }
        public decimal? MinServiceFeesFilter { get; set; }

        public string InfoSeedServiceServiceNameFilter { get; set; }

    }
}
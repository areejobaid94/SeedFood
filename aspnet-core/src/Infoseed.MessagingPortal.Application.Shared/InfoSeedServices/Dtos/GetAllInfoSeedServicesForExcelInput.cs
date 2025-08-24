using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class GetAllInfoSeedServicesForExcelInput
    {
        public string Filter { get; set; }

        public string ServiceIDFilter { get; set; }

        public decimal? MaxServiceFeesFilter { get; set; }
        public decimal? MinServiceFeesFilter { get; set; }

        public string ServiceNameFilter { get; set; }

        public DateTime? MaxServiceCreationDateFilter { get; set; }
        public DateTime? MinServiceCreationDateFilter { get; set; }

        public DateTime? MaxServiceStoppingDateFilter { get; set; }
        public DateTime? MinServiceStoppingDateFilter { get; set; }

        public string ServiceTypeServicetypeNameFilter { get; set; }

        public string ServiceStatusServiceStatusNameFilter { get; set; }

        public string ServiceFrquencyServiceFrequencyNameFilter { get; set; }

    }
}
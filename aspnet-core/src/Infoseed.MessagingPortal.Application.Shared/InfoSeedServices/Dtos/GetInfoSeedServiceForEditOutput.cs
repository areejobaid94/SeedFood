using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class GetInfoSeedServiceForEditOutput
    {
        public CreateOrEditInfoSeedServiceDto InfoSeedService { get; set; }

        public string ServiceTypeServicetypeName { get; set; }

        public string ServiceStatusServiceStatusName { get; set; }

        public string ServiceFrquencyServiceFrequencyName { get; set; }

    }
}
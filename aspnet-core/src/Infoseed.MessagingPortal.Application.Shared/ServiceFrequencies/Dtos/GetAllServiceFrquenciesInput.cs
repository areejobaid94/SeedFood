using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ServiceFrequencies.Dtos
{
    public class GetAllServiceFrquenciesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ServiceFrequencyNameFilter { get; set; }

        public int? IsEnabledFilter { get; set; }

        public DateTime? MaxCreationDateFilter { get; set; }
        public DateTime? MinCreationDateFilter { get; set; }

    }
}
using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ServiceStatuses.Dtos
{
    public class GetAllServiceStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ServiceStatusNameFilter { get; set; }

        public int? IsEnabledFilter { get; set; }

        public DateTime? MaxCreationDateFilter { get; set; }
        public DateTime? MinCreationDateFilter { get; set; }

    }
}
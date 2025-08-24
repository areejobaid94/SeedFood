using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ServiceTypes.Dtos
{
    public class GetAllServiceTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ServicetypeNameFilter { get; set; }

        public int? IsEnabledFilter { get; set; }

        public DateTime? MaxCreationDateFilter { get; set; }
        public DateTime? MinCreationDateFilter { get; set; }

    }
}
using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ContactStatuses.Dtos
{
    public class GetAllContactStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ContactStatusNameFilter { get; set; }

        public int? IsEnabledFilter { get; set; }

    }
}
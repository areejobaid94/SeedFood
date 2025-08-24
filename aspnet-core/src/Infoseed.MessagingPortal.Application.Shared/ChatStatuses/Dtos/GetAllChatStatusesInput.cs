using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.ChatStatuses.Dtos
{
    public class GetAllChatStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ChatStatusNameFilter { get; set; }

        public int? IsEnabledFilter { get; set; }

    }
}
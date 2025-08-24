using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos
{
    public class GetAllTemplateMessagePurposesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string PurposeFilter { get; set; }

    }
}
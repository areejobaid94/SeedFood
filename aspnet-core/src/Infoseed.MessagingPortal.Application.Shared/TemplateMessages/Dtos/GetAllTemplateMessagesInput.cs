using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.TemplateMessages.Dtos
{
    public class GetAllTemplateMessagesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string TemplateMessageNameFilter { get; set; }

        public DateTime? MaxMessageCreationDateFilter { get; set; }
        public DateTime? MinMessageCreationDateFilter { get; set; }

        public string TemplateMessagePurposePurposeFilter { get; set; }

    }
}
using System.Collections.Generic;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.TemplateMessages.Exporting
{
    public interface ITemplateMessagesExcelExporter
    {
        FileDto ExportToFile(List<GetTemplateMessageForViewDto> templateMessages);
    }
}
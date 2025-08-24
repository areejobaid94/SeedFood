using System.Collections.Generic;
using Abp;
using Infoseed.MessagingPortal.Chat.Dto;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(UserIdentifier user, List<ChatMessageExportDto> messages);
    }
}

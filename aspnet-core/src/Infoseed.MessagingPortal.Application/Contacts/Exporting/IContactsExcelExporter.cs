using System.Collections.Generic;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Contacts.Exporting
{
    public interface IContactsExcelExporter
    {
        FileDto ExportToFile(List<ContactDto> contacts);
        FileDto BackUpConversation(List<BackUpConversationModel> backUpConversation);
    }
}
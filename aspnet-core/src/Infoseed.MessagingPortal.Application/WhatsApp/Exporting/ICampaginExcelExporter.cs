using System.Collections.Generic;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.WhatsApp.Dto;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public interface ICampaginExcelExporter
    {
        //FileDto ExportToFile(List<ContactDto> contacts);
        FileDto BackUpCampaginForAll(List<CampaginMongoModel> backUpConversation);
    }
}
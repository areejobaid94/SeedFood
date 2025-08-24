using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;
using System;

namespace Infoseed.MessagingPortal.Contacts.Exporting
{
    public class ContactsExcelExporter : NpoiExcelExporterBase, IContactsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ContactsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<ContactDto> contacts)
        {
            return CreateExcelPackage(
                "Contacts1.xlsx",excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Contacts"));
                    AddHeader(sheet,L("DisplayName"),L("PhoneNumber"),L("Address"), L("CustomerBehavior"));
                    AddObjects(sheet,2, contacts,_ => _.DisplayName, _ => _.PhoneNumber, _ => _.EmailAddress,
                _ => _.CustomerOPT == 0 ? "Neutral" :
                     _.CustomerOPT == 1 ? "Unsubscribed" :
                     _.CustomerOPT == 2 ? "Subscribed" : "Unknown"
            );
                });
        }
        public FileDto BackUpConversation(List<BackUpConversationModel> contacts)
        {
            try
            {
                return CreateExcelPackage(
                "BackUpconversation.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Contacts"));

                    AddHeader(
                        sheet,
                        L("UserName"),
                        L("PhoneNumber"),
                        L("TextDate"),
                         L("Text"),
                         L("MediaUrl")
                        );

                    AddObjects(
                        sheet, 2, contacts,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.TextDate.ToString(),
                         _ => _.Text,
                         _ => _.MediaUrl
                        );

                });
            }
            catch(Exception )
            {
                return CreateExcelPackage(
                "BackUpconversation.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Contacts"));

                    AddHeader(
                        sheet,
                        L("UserName"),
                        L("PhoneNumber"),
                        L("TextDate"),
                         L("Text"),
                         L("MediaUrl")
                        );

                    AddObjects(
                        sheet, 2, contacts,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.TextDate,
                         _ => _.Text,
                         _ => _.MediaUrl
                        );

                });
            }
            
        }
    }
}
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.Storage;
using System;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.LiveChat.Exporting
{
    public class LivChatExcelExporter : NpoiExcelExporterBase, ILiveChatExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public LivChatExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
          base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<CustomerLiveChatModel> model)
        {
            try
            {
                return CreateExcelPackage(
               "LiveChat.xlsx",
               excelPackage =>
               {

                   var sheet = excelPackage.CreateSheet("LiveChat");

                   AddHeader(
                       sheet,
                        L("Agent"),
                        L("Customer"),
                        L("Phone Number"),
                        L("Time"),
                        L("Status"),
                        L("Department"),
                        L("Open time"),
                        L("Closing time"),
                        L("ContactCreationDate")

                       );

                   AddObjects(
                       sheet, 2, model,
                        _ => _.LockedByAgentName,
                        _ => _.displayName,
                        _ => _.phoneNumber,
                        _ => _.requestedLiveChatTime.ToString(),
                        _ => _.LiveChatStatusName,
                        _ => _.Department,
                        _ => _.OpenTime,
                        _ => _.CloseTime,
                        _ => _.ContactCreationDate.ToString()
                       );;

               });

            }
            catch(Exception)
            {
                return CreateExcelPackage(
               "LiveChat.xlsx",
               excelPackage =>
               {

                   var sheet = excelPackage.CreateSheet(L("LiveChat"));

                   AddHeader(
                       sheet,
                       // L("Agent"),
                        L("Customer"),
                        L("Phone Number"),
                        L("Time"),
                        L("Status"),
                        L("Open time"),
                        L("Closing time"),
                         L("ContactCreationDate")
                       );

                   AddObjects(
                       sheet, 2, model,
                       // _ => _.LockedByAgentName,
                        _ => _.displayName,
                        _ => _.phoneNumber,
                        _ => _.requestedLiveChatTime,
                        _ => _.LiveChatStatusName,
                        _ => _.OpenTime,
                        _ => _.CloseTime,
                        _ => _.ContactCreationDate.ToString()
                       );

               });
            }
           
        }

    }
}
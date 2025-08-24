using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;
using System;
using Infoseed.MessagingPortal.WhatsApp.Dto;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public class CampaginExcelExporter : NpoiExcelExporterBase, ICampaginExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CampaginExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto BackUpCampaginForAll(List<CampaginMongoModel> contacts)
        {

            try
            {
                return CreateExcelPackage(
               "BackUpCampagin.xlsx",
               excelPackage =>
               {

                   var sheet = excelPackage.CreateSheet("Campagin");

                   AddHeader(
                       sheet,
                       L("phoneNumber"),
                       //L("status"),
                        L("statusCode"),
                        L("is_sent"),
                        L("is_delivered"),
                        L("is_read"),
                        L("failedDetails")
                       );

                   AddObjects(
                       sheet, 2, contacts,
                        _ => _.phoneNumber,
                       // _ => _.status,
                        _ => _.statusCode,
                        _ => _.is_sent,
                        _ => _.is_delivered,
                        _ => _.is_read,
                        _ => _.failedDetails
                       );

               });

            }
            catch(Exception ex)
            {
                return null;

            }
          
               
            
        }
    }
}
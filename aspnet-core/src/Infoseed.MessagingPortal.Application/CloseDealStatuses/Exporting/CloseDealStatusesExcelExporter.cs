//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.CloseDealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.CloseDealStatuses.Exporting
//{
//    public class CloseDealStatusesExcelExporter : NpoiExcelExporterBase, ICloseDealStatusesExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public CloseDealStatusesExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetCloseDealStatusForViewDto> closeDealStatuses)
//        {
//            return CreateExcelPackage(
//                "CloseDealStatuses.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("CloseDealStatuses"));

//                    AddHeader(
//                        sheet,
//                        L("Status")
//                        );

//                    AddObjects(
//                        sheet, 2, closeDealStatuses,
//                        _ => _.CloseDealStatus.Status
//                        );

//                });
//        }
//    }
//}
//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.DealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.DealStatuses.Exporting
//{
//    public class DealStatusesExcelExporter : NpoiExcelExporterBase, IDealStatusesExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public DealStatusesExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetDealStatusForViewDto> dealStatuses)
//        {
//            return CreateExcelPackage(
//                "DealStatuses.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("DealStatuses"));

//                    AddHeader(
//                        sheet,
//                        L("DealStatus")
//                        );

//                    AddObjects(
//                        sheet, 2, dealStatuses,
//                        _ => _.DealStatus.DealStatus
//                        );

//                });
//        }
//    }
//}
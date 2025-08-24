//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.DealTypes.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.DealTypes.Exporting
//{
//    public class DealTypesExcelExporter : NpoiExcelExporterBase, IDealTypesExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public DealTypesExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetDealTypeForViewDto> dealTypes)
//        {
//            return CreateExcelPackage(
//                "DealTypes.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("DealTypes"));

//                    AddHeader(
//                        sheet,
//                        L("Deal_Type")
//                        );

//                    AddObjects(
//                        sheet, 2, dealTypes,
//                        _ => _.DealType.Deal_Type
//                        );

//                });
//        }
//    }
//}
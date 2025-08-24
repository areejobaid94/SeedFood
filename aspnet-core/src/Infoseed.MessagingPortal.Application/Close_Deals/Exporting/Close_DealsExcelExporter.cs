//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.Close_Deals.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.Close_Deals.Exporting
//{
//    public class Close_DealsExcelExporter : NpoiExcelExporterBase, IClose_DealsExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public Close_DealsExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetClose_DealForViewDto> close_Deals)
//        {
//            return CreateExcelPackage(
//                "Close_Deals.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("Close_Deals"));

//                    AddHeader(
//                        sheet,
//                        L("UserName"),
//                        L("CustomerName"),
//                        L("CustomerCommercialNameArabic"),
//                        L("ARR"),
//                        L("OrderFees"),
//                        L("ContrevelDate"),
//                        L("FirstPay"),
//                        L("SecondPay"),
//                        L("AMClosed"),
//                        (L("CloseDealStatus")) + L("Status")
//                        );

//                    AddObjects(
//                        sheet, 2, close_Deals,
//                        _ => _.Close_Deal.UserName,
//                        _ => _.Close_Deal.CustomerName,
//                        _ => _.Close_Deal.CustomerCommercialNameArabic,
//                        _ => _.Close_Deal.ARR,
//                        _ => _.Close_Deal.OrderFees,
//                        _ => _timeZoneConverter.Convert(_.Close_Deal.ContrevelDate, _abpSession.TenantId, _abpSession.GetUserId()),
//                        _ => _.Close_Deal.FirstPay,
//                        _ => _.Close_Deal.SecondPay,
//                        _ => _.Close_Deal.AMClosed,
//                        _ => _.CloseDealStatusStatus
//                        );

//                    for (var i = 1; i <= close_Deals.Count; i++)
//                    {
//                        SetCellDataFormat(sheet.GetRow(i).Cells[6], "yyyy-mm-dd");
//                    }
//                    sheet.AutoSizeColumn(6);
//                });
//        }
//    }
//}
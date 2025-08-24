//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.Deals.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.Deals.Exporting
//{
//    public class DealsExcelExporter : NpoiExcelExporterBase, IDealsExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public DealsExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetDealForViewDto> deals)
//        {
//            return CreateExcelPackage(
//                "Deals.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("Deals"));

//                    AddHeader(
//                        sheet,
//                        L("UserName"),
//                        L("CustomerName"),
//                        L("DealName"),
//                        L("ARR"),
//                        L("OrderFees"),
//                        L("CloseDate"),
//                        L("DealAge"),
//                        (L("DealStatus")) + L("Status"),
//                        (L("DealType")) + L("Deal_Type")
//                        );

//                    AddObjects(
//                        sheet, 2, deals,
//                        _ => _.Deal.UserName,
//                        _ => _.Deal.CustomerName,
//                        _ => _.Deal.DealName,
//                        _ => _.Deal.ARR,
//                        _ => _.Deal.OrderFees,
//                        _ => _timeZoneConverter.Convert(_.Deal.CloseDate, _abpSession.TenantId, _abpSession.GetUserId()),
//                        _ => _.Deal.DealAge,
//                        _ => _.DealStatusStatus,
//                        _ => _.DealTypeDeal_Type
//                        );

//                    for (var i = 1; i <= deals.Count; i++)
//                    {
//                        SetCellDataFormat(sheet.GetRow(i).Cells[6], "yyyy-mm-dd");
//                    }
//                    sheet.AutoSizeColumn(6);
//                });
//        }
//    }
//}
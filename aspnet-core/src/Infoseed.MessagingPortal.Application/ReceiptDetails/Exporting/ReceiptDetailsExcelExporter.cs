using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.ReceiptDetails.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.ReceiptDetails.Exporting
{
    public class ReceiptDetailsExcelExporter : NpoiExcelExporterBase, IReceiptDetailsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ReceiptDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetReceiptDetailForViewDto> receiptDetails)
        {
            return CreateExcelPackage(
                "ReceiptDetails.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("ReceiptDetails"));

                    AddHeader(
                        sheet,
                        L("BillingNumber"),
                        L("BillDateFrom"),
                        L("BillDateTo"),
                        L("ServiceName"),
                        L("BillAmount"),
                        L("OpenAmount"),
                        L("CurrencyName"),
                        (L("Receipt")) + L("ReceiptNumber"),
                        (L("AccountBilling")) + L("BillID")
                        );

                    AddObjects(
                        sheet, 2, receiptDetails,
                        _ => _.ReceiptDetail.BillingNumber,
                        _ => _timeZoneConverter.Convert(_.ReceiptDetail.BillDateFrom, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.ReceiptDetail.BillDateTo, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ReceiptDetail.ServiceName,
                        _ => _.ReceiptDetail.BillAmount,
                        _ => _.ReceiptDetail.OpenAmount,
                        _ => _.ReceiptDetail.CurrencyName,
                        _ => _.ReceiptReceiptNumber,
                        _ => _.AccountBillingBillID
                        );

                    for (var i = 1; i <= receiptDetails.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2); for (var i = 1; i <= receiptDetails.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3);
                });
        }
    }
}
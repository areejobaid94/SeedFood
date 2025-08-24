using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Receipts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Receipts.Exporting
{
    public class ReceiptsExcelExporter : NpoiExcelExporterBase, IReceiptsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ReceiptsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetReceiptForViewDto> receipts)
        {
            return CreateExcelPackage(
                "Receipts.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Receipts"));

                    AddHeader(
                        sheet,
                        L("ReceiptNumber"),
                        L("ReceiptDate"),
                        L("PaymentReferenceNumber"),
                        (L("Bank")) + L("BankName"),
                        (L("PaymentMethod")) + L("PaymnetMethod")
                        );

                    AddObjects(
                        sheet, 2, receipts,
                        _ => _.Receipt.ReceiptNumber,
                        _ => _timeZoneConverter.Convert(_.Receipt.ReceiptDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Receipt.PaymentReferenceNumber,
                        _ => _.BankBankName,
                        _ => _.PaymentMethodPaymnetMethod
                        );

                    for (var i = 1; i <= receipts.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2);
                });
        }
    }
}
using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Billings.Exporting
{
    public class BillingsExcelExporter : NpoiExcelExporterBase, IBillingsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BillingsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBillingForViewDto> billings)
        {
            return CreateExcelPackage(
                "Billings.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Billings"));

                    AddHeader(
                        sheet,
                        L("BillingID"),
                        L("BillingDate"),
                        L("TotalAmount"),
                        L("BillPeriodTo"),
                        L("BillPeriodFrom"),
                        L("DueDate"),
                        (L("Currency")) + L("CurrencyName")
                        );

                    AddObjects(
                        sheet, 2, billings,
                        _ => _.Billing.BillingID,
                        _ => _timeZoneConverter.Convert(_.Billing.BillingDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Billing.TotalAmount,
                        _ => _timeZoneConverter.Convert(_.Billing.BillPeriodTo, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Billing.BillPeriodFrom, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Billing.DueDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.CurrencyCurrencyName
                        );

                    for (var i = 1; i <= billings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2); for (var i = 1; i <= billings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4); for (var i = 1; i <= billings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[5], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(5); for (var i = 1; i <= billings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[6], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(6);
                });
        }
    }
}
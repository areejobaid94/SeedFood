using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.AccountBillings.Exporting
{
    public class AccountBillingsExcelExporter : NpoiExcelExporterBase, IAccountBillingsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AccountBillingsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAccountBillingForViewDto> accountBillings)
        {
            return CreateExcelPackage(
                "AccountBillings.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("AccountBillings"));

                    AddHeader(
                        sheet,
                        L("BillID"),
                        L("BillDateFrom"),
                        L("BillDateTo"),
                        L("OpenAmount"),
                        L("BillAmount"),
                        (L("InfoSeedService")) + L("ServiceName"),
                        (L("ServiceType")) + L("ServicetypeName"),
                        (L("Currency")) + L("CurrencyName"),
                        (L("Billing")) + L("BillingID")
                        );

                    AddObjects(
                        sheet, 2, accountBillings,
                        _ => _.AccountBilling.BillID,
                        _ => _timeZoneConverter.Convert(_.AccountBilling.BillDateFrom, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.AccountBilling.BillDateTo, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.AccountBilling.OpenAmount,
                        _ => _.AccountBilling.BillAmount,
                        _ => _.InfoSeedServiceServiceName,
                        _ => _.ServiceTypeServicetypeName,
                        _ => _.CurrencyCurrencyName,
                        _ => _.BillingBillingID
                        );

                    for (var i = 1; i <= accountBillings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2); for (var i = 1; i <= accountBillings.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3);
                });
        }
    }
}
using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Currencies.Exporting
{
    public class CurrenciesExcelExporter : NpoiExcelExporterBase, ICurrenciesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CurrenciesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCurrencyForViewDto> currencies)
        {
            return CreateExcelPackage(
                "Currencies.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Currencies"));

                    AddHeader(
                        sheet,
                        L("CurrencyName"),
                        L("ISOName")
                        );

                    AddObjects(
                        sheet, 2, currencies,
                        _ => _.Currency.CurrencyName,
                        _ => _.Currency.ISOName
                        );

                });
        }
    }
}
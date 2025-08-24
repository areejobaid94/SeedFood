using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Forcasts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Forcasts.Exporting
{
    public class ForcatsesExcelExporter : NpoiExcelExporterBase, IForcatsesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ForcatsesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetForcatsForViewDto> forcatses)
        {
            return CreateExcelPackage(
                "Forcatses.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Forcatses"));

                    AddHeader(
                        sheet,
                        L("UserName"),
                        L("TotalCommit"),
                        L("TotalClosed"),
                        L("SubmitDate")
                        );

                    AddObjects(
                        sheet, 2, forcatses,
                        _ => _.Forcats.UserName,
                        _ => _.Forcats.TotalCommit,
                        _ => _.Forcats.TotalClosed,
                        _ => _timeZoneConverter.Convert(_.Forcats.SubmitDate, _abpSession.TenantId, _abpSession.GetUserId())
                        );

                    for (var i = 1; i <= forcatses.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4);
                });
        }
    }
}
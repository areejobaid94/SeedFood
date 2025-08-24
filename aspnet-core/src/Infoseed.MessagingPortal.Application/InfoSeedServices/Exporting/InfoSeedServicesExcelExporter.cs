using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.InfoSeedServices.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.InfoSeedServices.Exporting
{
    public class InfoSeedServicesExcelExporter : NpoiExcelExporterBase, IInfoSeedServicesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public InfoSeedServicesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetInfoSeedServiceForViewDto> infoSeedServices)
        {
            return CreateExcelPackage(
                "InfoSeedServices.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("InfoSeedServices"));

                    AddHeader(
                        sheet,
                        L("ServiceID"),
                        L("ServiceName"),
                        L("ServiceCreationDate"),
                        L("ServiceStoppingDate"),
                        (L("ServiceType")) + L("ServicetypeName"),
                        (L("ServiceStatus")) + L("ServiceStatusName"),
                        (L("ServiceFrquency")) + L("ServiceFrequencyName")
                        );

                    AddObjects(
                        sheet, 2, infoSeedServices,
                        _ => _.InfoSeedService.ServiceID,
                        _ => _.InfoSeedService.ServiceName,
                        _ => _timeZoneConverter.Convert(_.InfoSeedService.ServiceCreationDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.InfoSeedService.ServiceStoppingDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.ServiceTypeServicetypeName,
                        _ => _.ServiceStatusServiceStatusName,
                        _ => _.ServiceFrquencyServiceFrequencyName
                        );

                    for (var i = 1; i <= infoSeedServices.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3); for (var i = 1; i <= infoSeedServices.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4);
                });
        }
    }
}
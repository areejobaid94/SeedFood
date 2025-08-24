using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.TenantServices.Exporting
{
    public class TenantServicesExcelExporter : NpoiExcelExporterBase, ITenantServicesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TenantServicesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTenantServiceForViewDto> tenantServices)
        {
            return CreateExcelPackage(
                "TenantServices.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("TenantServices"));

                    AddHeader(
                        sheet,
                        L("ServiceFees"),
                        (L("InfoSeedService")) + L("ServiceName")
                        );

                    AddObjects(
                        sheet, 2, tenantServices,
                        _ => _.TenantService.ServiceFees,
                        _ => _.InfoSeedServiceServiceName
                        );

                });
        }
    }
}
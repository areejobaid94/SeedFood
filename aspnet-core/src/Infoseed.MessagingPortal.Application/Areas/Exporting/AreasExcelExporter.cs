using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Areas.Exporting
{
    public class AreasExcelExporter : NpoiExcelExporterBase, IAreasExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AreasExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAreaForViewDto> areas)
        {
            return CreateExcelPackage(
                "Areas.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Areas"));

                    AddHeader(
                        sheet,
                        L("AreaName"),
                        L("AreaCoordinate")
                        );

                    AddObjects(
                        sheet, 2, areas,
                        _ => _.Area.AreaName,
                        _ => _.Area.AreaCoordinate
                        );

					
					
                });
        }
    }
}

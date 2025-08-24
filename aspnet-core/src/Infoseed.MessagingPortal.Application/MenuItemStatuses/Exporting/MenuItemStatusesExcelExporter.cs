using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.MenuItemStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.MenuItemStatuses.Exporting
{
    public class MenuItemStatusesExcelExporter : NpoiExcelExporterBase, IMenuItemStatusesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MenuItemStatusesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetMenuItemStatusForViewDto> menuItemStatuses)
        {
            return CreateExcelPackage(
                "MenuItemStatuses.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("MenuItemStatuses"));

                    AddHeader(
                        sheet,
                        L("Name")
                        );

                    AddObjects(
                        sheet, 2, menuItemStatuses,
                        _ => _.MenuItemStatus.Name
                        );

					
					
                });
        }
    }
}

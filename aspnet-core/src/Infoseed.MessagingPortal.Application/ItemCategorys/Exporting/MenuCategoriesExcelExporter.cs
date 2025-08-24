using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.MenuCategories.Exporting
{
    public class MenuCategoriesExcelExporter : NpoiExcelExporterBase, IMenuCategoriesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MenuCategoriesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetMenuCategoryForViewDto> menuCategories)
        {
            return CreateExcelPackage(
                "MenuCategories.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("MenuCategories"));

                    AddHeader(
                        sheet,
                        L("Name")
                        );

                    AddObjects(
                        sheet, 2, menuCategories,
                        _ => _.MenuCategory.Name
                        );

					
					
                });
        }
    }
}

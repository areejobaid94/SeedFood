using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.MenuDetails.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.MenuDetails.Exporting
{
    public class MenuDetailsExcelExporter : NpoiExcelExporterBase, IMenuDetailsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MenuDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetMenuDetailForViewDto> menuDetails)
        {
            return CreateExcelPackage(
                "MenuDetails.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("MenuDetails"));

                    AddHeader(
                        sheet,
                        L("Description"),
                        L("IsStandAlone"),
                        L("Price"),
                        (L("Item")) + L("ItemName"),
                        (L("Menu")) + L("MenuName"),
                        (L("MenuItemStatus")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, menuDetails,
                        _ => _.MenuDetail.Description,
                        _ => _.MenuDetail.IsStandAlone,
                        _ => _.MenuDetail.Price,
                        _ => _.ItemItemName,
                        _ => _.MenuMenuName,
                        _ => _.MenuItemStatusName
                        );

					
					
                });
        }
    }
}

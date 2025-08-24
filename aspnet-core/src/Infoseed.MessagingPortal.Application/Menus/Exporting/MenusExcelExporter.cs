using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.Menus.Exporting
{
    public class MenusExcelExporter : NpoiExcelExporterBase, IMenusExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public MenusExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetMenuForViewDto> menus)
        {
            return CreateExcelPackage(
                "Menus.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Menus"));

                    AddHeader(
                        sheet,
                        L("MenuName"),
                        L("MenuDescription"),
                        L("EffectiveTimeFrom"),
                        L("EffectiveTimeTo"),
                        L("Tax"),
                        L("ImageUri")
                        //(L("MenuItemStatus")) + L("Name"),
                        //(L("MenuCategory")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, menus,
                        _ => _.Menu.MenuName,
                        _ => _.Menu.MenuDescription,
                        _ => _timeZoneConverter.Convert(_.Menu.EffectiveTimeFrom, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _timeZoneConverter.Convert(_.Menu.EffectiveTimeTo, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Menu.Tax,
                        _ => _.Menu.ImageUri
                        //_ => _.MenuItemStatusName,
                        //_ => _.MenuCategoryName
                        );

					
					for (var i = 1; i <= menus.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[3], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(3);for (var i = 1; i <= menus.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[4], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(4);
                });
        }
    }
}

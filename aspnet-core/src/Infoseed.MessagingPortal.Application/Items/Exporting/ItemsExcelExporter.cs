using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;
using System;

namespace Infoseed.MessagingPortal.Items.Exporting
{
    public class ItemsExcelExporter : NpoiExcelExporterBase, IItemsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ItemsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetItemForViewDto> items)
        {
            try
            {
                return CreateExcelPackage(
                    "Items.xlsx",
                    excelPackage =>
                    {

                        var sheet = excelPackage.CreateSheet(L("Items"));

                        AddHeader(
                            sheet,
                            L("Id"),
                            L("ItemName"),
                            L("price")
                            );

                        AddObjects(
                            sheet, 2, items,
                            _ => _.Item.Id,
                            _ => _.Item.ItemName,
                            _ => _.Item.Price.ToString()

                            );

                    });

            }
            catch(Exception )
            {
                return CreateExcelPackage(
                    "Items.xlsx",
                    excelPackage =>
                    {

                        var sheet = excelPackage.CreateSheet(L("Items"));

                        AddHeader(
                            sheet,
                            L("Id"),
                            L("ItemName"),
                            L("price")
                            );

                        AddObjects(
                            sheet, 2, items,
                            _ => _.Item.Id,
                            _ => _.Item.ItemName,
                            _ => _.Item.Price.ToString()

                            );

                    });
            }

        }
    }
}

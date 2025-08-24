using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Orders.Exporting;
using Infoseed.MessagingPortal.Storage;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Exporting
{
    public class UsageDetailsExcelExporter : NpoiExcelExporterBase, IUsageDetailsExcelExport
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UsageDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UsageDetailsModel> model)
        {
            return  CreateExcelPackage(
                "UsageDetails.xlsx",
                excelPackage =>
                {
                    var sheetName = L("UsageDetails").Replace("[", "").Replace("]", "");

                    var sheet = excelPackage.CreateSheet(sheetName);
                    //var sheet = excelPackage.CreateSheet(L("UsageDetails"));

                    AddHeader(
                        sheet,
                        L("Category type"),
                        L("Date/Time"),
                        L("Sent By"),
                        L("Template Name"),
                        L("Campaign Name"),
                        L("Quantity"),
                        L("Total Cost"),
                        L("Total Credit Remaining"),
                        L("Countries")
                    );
                    AddObjects(
                        sheet, 1, model,
                        _ => _.CategoryType,
                        _ => _.TransactionDate,
                        _ => _.DoneBy,
                        _ => _.TemplateName,
                        _ => _.CampaignName,
                        _ => _.TotalQuantity,
                        _ => _.TotalTransaction,
                        _ => _.TotalRemaining,
                        _ => _.Country
                        );
                });
        }
    }
}

using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Orders.Exporting
{
    public class OrderExcelExporter : NpoiExcelExporterBase, IOrderExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public OrderExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetOrderForViewDto> contacts)
        {
            return CreateExcelPackage(
                "Orders.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Orders"));

                    AddHeader(
                        sheet,
                        L("Agent"),
                        L("Order"),
                        L("Customer"),
                        L("Date"),
                        L("Time"),
                        L("Status"),
                        L("Type"),
                        L("Total"),
                        L("Branch"),
                        L("Total points"),
                        L("Action time"),
                        L("Phone Number"),
                        L("Address"),
                        L("Location")
                    );

                    AddObjects(
                        sheet, 2, contacts,
                        _ => _.Order.LockByAgentName,
                        _ => _.Order.OrderNumber,
                        _ => _.CustomerCustomerName,
                        _ => _.CreatDate,
                        _ => _.CreatTime,
                        _ => _.OrderStatusName,
                        _ => _.OrderTypeName,
                        _ => _.Order.Total,                    
                        _ => _.AreahName,                    
                        _ => _.Order.TotalPoints,                    
                        _ => _.Order.OrderTime,               
                        _ => _.CustomerMobile,
                        _ => _.Order.Address,
                        _ => _.Order.FromLocationDescribation
                        );

                });
        }
      
    }
}
using DocumentFormat.OpenXml.Spreadsheet;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Storage;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.General
{
    public class ExcelExporterAppService : NpoiExcelExporterBase, IExcelExporterAppService
    {
        public ExcelExporterAppService(ITempFileCacheManager tempFileCacheManager) : base(tempFileCacheManager)
        {

        }


        public FileDto ExportTenantsFileHost(List<ExportToExcelHost> model)
        {
            try
            {
                return CreateExcelPackage("TicketsStatistics.xlsx", excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("Tickets");

                    AddHeader(sheet,
                         "Tenant Name", "Phone Number", "Domain Name", "Integration", "CreationTime",
                         "Total Tickets", "Total Pending", "Total Opened", "Total Closed", "Last Closed Ticket Date",
                         "Last Month Total Tickets", "Last Month Pending", "Last Month Opened", "Last Month Closed",
                         "Wallet Balance",
                         "Total Orders", "Pending Orders", "Done Orders", "Deleted Orders", "Canceled Orders", "PreOrders",
                         "Last Month Orders", "Last Month Pending", "Last Month Done", "Last Month Deleted", "Last Month Canceled", "Last Month PreOrders"
                     );

                    AddObjects(sheet, 2, model,
                        _ => _.TenantName,
                        _ => _.PhoneNumber,
                        _ => _.DomainName,
                        _ => _.Integration,
                        _ => _.CreationTime?.ToString("yyyy-MM-dd HH:mm"),
                        _ => _.TotalTickets,
                        _ => _.TotalPending,
                        _ => _.TotalOpened,
                        _ => _.TotalClosed,
                        _ => _.LastClosedTicketDate?.ToString("yyyy-MM-dd HH:mm"),
                        _ => _.LastMonthTotalTickets,
                        _ => _.LastMonthTotalPending,
                        _ => _.LastMonthTotalOpened,
                        _ => _.LastMonthTotalClosed,
                        _ => _.WalletBalance,
                        _ => _.TotalOrder,
                        _ => _.TotalOrderPending,
                        _ => _.DoneOrders,
                        _ => _.TotalOrderDeleted,
                        _ => _.TotalOrderCanceled,
                        _ => _.TotalOrderPreOrder,
                        _ => _.LastMonthTotalOrders,
                        _ => _.LastMonthPendingOrders,
                        _ => _.LastMonthDoneOrders,
                        _ => _.LastMonthDeletedOrders,
                        _ => _.LastMonthCanceledOrders,
                        _ => _.LastMonthPreOrders
                    );
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to export tickets statistics to Excel", ex);
            }
        }
        public FileDto ExportTenantsToFile(List<TenantsToExcelDto> model)
        {
            try
            {
                return CreateExcelPackage("Tenants.xlsx", excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet("Tenants");

                    AddHeader(sheet,
                         L("Tenant Name"),
                         L("Phone Number"),
                         L("Total Free Conversation"),
                         L("Remaining Free Conversation"),
                         L("Total UI Conversation"),
                         L("Remaining UI Conversation"),
                         L("Total BI Conversation"),
                         L("Remaining BI Conversation")
                     );

                    AddObjects(sheet, 2, model,
                         _ => _.TenantName,
                         _ => _.PhoneNumber,
                         _ => _.TotalFreeConversation,
                         _ => _.RemainingFreeConversation,
                         _ => _.TotalUIConversation,
                         _ => _.RemainingUIConversation,
                         _ => _.TotalBIConversation,
                         _ => _.RemainingBIConversation
                     );

                });

            }
            catch (Exception)
            {
                return CreateExcelPackage("Tenants.xlsx", excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("Tenants"));

                    AddHeader(sheet,
                         L("Tenant Name"),
                         L("Customer"),
                         L("Total Free Conversation"),
                         L("Remaining Free Conversation"),
                         L("Total UI Conversation"),
                         L("Remaining UI Conversation"),
                         L("Total BI Conversation"),
                         L("Remaining BI Conversation")
                     );

                    AddObjects(sheet, 2, model,
                         _ => _.TenantName,
                         _ => _.TotalFreeConversation,
                         _ => _.RemainingFreeConversation,
                         _ => _.TotalUIConversation,
                         _ => _.RemainingUIConversation,
                         _ => _.TotalBIConversation,
                         _ => _.RemainingBIConversation
                     );

                });
            }

        }
        public FileDto ExportContactCampaignToFile(List<ContactCampaignDto> model)
        {
            try
            {
                return CreateExcelPackage("Contact.xlsx", excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet("Contact");

                    AddHeader(sheet,
                         L("Phone Number"),
                         L("Sent"),
                         L("Delivered"),
                         L("Read"),
                         L("Failed"),
                         L("Replied"),
                         L("Template"),
                         L("Campaign")
                     );

                    AddObjects(sheet, 2, model,
                         _ => _.PhoneNumber,
                         _ => _.IsSent,
                         _ => _.IsDelivered,
                         _ => _.IsRead,
                         _ => _.IsFailed,
                         _ => _.IsReplied,
                         _ => _.TemplateName,
                         _ => _.CampaignName
                     );

                });

            }
            catch (Exception)
            {
                throw;
            }

        }

        public  FileDto ExportItemsToExcel(List<ItemDto> items)
        {
            try
            {
                return CreateExcelPackage("Items.xlsx", excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet("Items");

                    AddHeader(sheet,
                         L("Id"),
                         L("Item Name"),
                         L("Price")
                     );

                    AddObjects(sheet, 2, items,
                         _ => _.Id,
                         _ => _.ItemName,
                         _ => _.Price
                     );
                });
            }
            catch (Exception)
            {
                throw;
            }

        }
        public FileDto ExportLiveChatToExcel(List<CustomerLiveChatModel> liveChat)
        {
            try
            {
                return CreateExcelPackage(
               "LiveChat.xlsx",
               excelPackage =>
               {

                   var sheet = excelPackage.CreateSheet("LiveChat");

                   AddHeader(
                       sheet,
                        L("ID"),
                        L("Type"),
                        L("Agent"),
                        L("Departemnt"),
                        L("Customer"),
                        L("Phone Number"),
                        L("Time"),
                        L("Status"),
                        L("Open time"),
                        L("Closing time"),
                        L("Resolution"),
                        L("Ticket Summary"),
                         L("ContactCreationDate")

                       );

                   AddObjects(
                       sheet, 2, liveChat,
                        _ => _.IdLiveChat,
                        _ => _.CategoryType,
                        _ => _.LockedByAgentName,
                        _ => _.Department,
                        _ => _.displayName,
                        _ => _.phoneNumber,
                        _ => _.requestedLiveChatTime.ToString(),
                        _ => _.LiveChatStatusName,
                        _ => _.OpenTimeTicket,
                        _ => _.CloseTimeTicket,
                        _ => _.ResolutionTicket,
                        _ => _.TicketSummary,
                        _ => _.ContactCreationDate.ToString()
                       ); ;

               });

            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
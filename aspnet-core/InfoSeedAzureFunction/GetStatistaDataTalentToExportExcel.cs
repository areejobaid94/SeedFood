using InfoSeedAzureFunction.Model.WalletJopModel;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using InfoSeedAzureFunction.Model;
using System.Data.SqlClient;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
namespace InfoSeedAzureFunction
{

    namespace InfoSeedAzureFunction
    {
        public class GetStatistaDataTalentToExportExcel
        {
            [FunctionName("ExportStatisticsFunction")]
            public static void Run(
                        [TimerTrigger("0 0 3 * * *", RunOnStartup = true)] TimerInfo myTimer,
                        ILogger log)
            {
                ExportToExcelHost(log).Wait();
            }
  



            public static async Task ExportToExcelHost(ILogger log)
            {
                try
                {
                    List<HostTenantListDto> tenants = GetTenants();

                    DateTime now = DateTime.Now;

                    DateTime currentWeekEnd = now;
                    DateTime currentWeekStart = currentWeekEnd.AddDays(-7);

                    DateTime lastMonthEnd = now;
                    DateTime lastMonthStart = lastMonthEnd.AddMonths(-1);

                    List<ExportToExcelHost> statisticsList = new List<ExportToExcelHost>();

                    foreach (var tenant in tenants)
                    {
                        var service = new GetStatistaDataTalentToExportExcel();

                        var allStats = service.GetAllTenantStatistics(tenant.Id, currentWeekStart, currentWeekEnd, lastMonthStart, lastMonthEnd).FirstOrDefault(); ;
                        var exportModel = new ExportToExcelHost
                        {
                            TenantName = tenant.Name,
                            Name = tenant.Name,
                            TenancyName = tenant.TenancyName,
                            IsActive = tenant.IsActive,
                            CreatedBy = tenant.CreatedBy,
                            CreationTime = tenant.CreationTime,
                            InvoiceId = tenant.InvoiceId,
                            PhoneNumber = tenant.PhoneNumber,
                            DomainName = tenant.TenancyName,
                            CustomerName = tenant.Name,
                            Integration = tenant.Integration,

                            TotalTickets = allStats.TotalTickets,
                            TotalPending = allStats.TotalPending,
                            TotalOpened = allStats.TotalOpened,
                            TotalClosed = allStats.TotalClosed,
                            LastClosedTicketDate = allStats.LastClosedTicketDate,

                            WalletBalance = allStats.WalletBalance,

                            LastMonthTotalTickets = allStats.LastMonthTotalTickets,
                            LastMonthTotalPending = allStats.LastMonthTotalPending,
                            LastMonthTotalOpened = allStats.LastMonthTotalOpened,
                            LastMonthTotalClosed = allStats.LastMonthTotalClosed,
 
                            TotalOrder = allStats.TotalOrder,
                            TotalOrderPending = allStats.TotalOrderPending,
                            DoneOrders = allStats.DoneOrders,
                            TotalOrderDeleted = allStats.TotalOrderDeleted,
                            TotalOrderCanceled = allStats.TotalOrderCanceled,
                            TotalOrderPreOrder = allStats.TotalOrderPreOrder,

                            LastMonthTotalOrders = allStats.LastMonthTotalOrders,
                            LastMonthPendingOrders = allStats.LastMonthPendingOrders,
                            LastMonthDoneOrders = allStats.LastMonthDoneOrders,
                            LastMonthDeletedOrders = allStats.LastMonthDeletedOrders,
                            LastMonthCanceledOrders = allStats.LastMonthCanceledOrders,
                            LastMonthPreOrders = allStats.LastMonthPreOrders
                        };

                        CreateExportToExcelHostRecord(exportModel);
                        statisticsList.Add(exportModel);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error while exporting statistics");
                    throw;
                }
            }
            private List<ExportToExcelHost> GetAllTenantStatistics(int tenantId, DateTime currentWeekStart, DateTime currentWeekEnd, DateTime lastMonthStart, DateTime lastMonthEnd)
            {
                var sqlParameters = new List<SqlParameter>
               {
                   new SqlParameter("@TenantId", tenantId),
                   new SqlParameter("@CurrentWeekStart", currentWeekStart),
                   new SqlParameter("@CurrentWeekEnd", currentWeekEnd),
                   new SqlParameter("@LastMonthStart", lastMonthStart),
                   new SqlParameter("@LastMonthEnd", lastMonthEnd)
               };

                var result = new ExportToExcelHost();

                SqlDataHelper.ExecuteReader<ExportToExcelHost>(
                    Constants.LiveChat.SP_GetAllTenantStatisticsToExportExcel,
                    sqlParameters.ToArray(),
                    (reader) =>
                    {
                        reader.NextResult();
                        if (reader.Read())
                        {
                            result.TotalTickets = SqlDataHelper.GetValue<long>(reader, "TotalTickets") ?? 0;
                            result.TotalPending = SqlDataHelper.GetValue<long>(reader, "TotalPending") ?? 0;
                            result.TotalOpened = SqlDataHelper.GetValue<long>(reader, "TotalOpened") ?? 0;
                            result.TotalClosed = SqlDataHelper.GetValue<long>(reader, "TotalClosed") ?? 0;
                            result.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(reader, "LastClosedTicketDate");
                        }

                        reader.NextResult();
                        if (reader.Read())
                        {
                            result.LastMonthTotalTickets = SqlDataHelper.GetValue<long>(reader, "TotalTickets") ?? 0;
                            result.LastMonthTotalPending = SqlDataHelper.GetValue<long>(reader, "TotalPending") ?? 0;
                            result.LastMonthTotalOpened = SqlDataHelper.GetValue<long>(reader, "TotalOpened") ?? 0;
                            result.LastMonthTotalClosed = SqlDataHelper.GetValue<long>(reader, "TotalClosed") ?? 0;
                        }

                        reader.NextResult();
                        if (reader.Read())
                        {
                            result.TotalOrder = SqlDataHelper.GetValue<long>(reader, "TotalOrders") ?? 0;
                            result.TotalOrderPending = SqlDataHelper.GetValue<long>(reader, "Pending") ?? 0;
                            result.DoneOrders = SqlDataHelper.GetValue<long>(reader, "Done") ?? 0;
                            result.TotalOrderDeleted = SqlDataHelper.GetValue<long>(reader, "Deleted") ?? 0;
                            result.TotalOrderCanceled = SqlDataHelper.GetValue<long>(reader, "Canceled") ?? 0;
                            result.TotalOrderPreOrder = SqlDataHelper.GetValue<long>(reader, "PreOrder") ?? 0;
                        }

                        reader.NextResult();
                        if (reader.Read())
                        {
                            result.LastMonthTotalOrders = SqlDataHelper.GetValue<long>(reader, "TotalOrders") ?? 0;
                            result.LastMonthPendingOrders = SqlDataHelper.GetValue<long>(reader, "Pending") ?? 0;
                            result.LastMonthDoneOrders = SqlDataHelper.GetValue<long>(reader, "Done") ?? 0;
                            result.LastMonthDeletedOrders = SqlDataHelper.GetValue<long>(reader, "Deleted") ?? 0;
                            result.LastMonthCanceledOrders = SqlDataHelper.GetValue<long>(reader, "Canceled") ?? 0;
                            result.LastMonthPreOrders = SqlDataHelper.GetValue<long>(reader, "PreOrder") ?? 0;
                        }

                        reader.NextResult();
                        if (reader.Read())
                        {
                            result.WalletBalance = SqlDataHelper.GetValue<decimal>(reader, "TotalAmount") ?? 0;
                        }

                        return result;
                    },
                    Constants.ConnectionString);

                return new List<ExportToExcelHost> { result };
            }

            private static List<HostTenantListDto> GetTenants()
            {
                var SP_Name = Constants.Tenant.SP_HostTenantsInfoGet;

                var sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@PageSize", 1000),
                        new SqlParameter("@PageNumber", 0),
                        new SqlParameter("@Filter", "")
                    };

                var outputTotalPending = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TenantCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(outputTotalPending);

                return SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                     MapHostTenantsInfo,
                     Constants.ConnectionString).ToList();
            }

            private static TicketsStatisticsModel GetTicketStatistics(int tenantId, DateTime start, DateTime end)
            {
                var sqlParameters = new List<SqlParameter>
                    {
                    new SqlParameter("@TenantId", tenantId),
                    new SqlParameter("@Start", start),
                    new SqlParameter("@End", end)
                 };

                return SqlDataHelper.ExecuteReader(Constants.LiveChat.SP_TicketsStatisticsGet,
                         sqlParameters.ToArray(),
                         MapTicketsGetStatistics,
                        Constants.ConnectionString).FirstOrDefault();
            }

            private static OrderStatisticsModel GetOrderStatistics(int tenantId, DateTime start, DateTime end)
            {
                var sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@TenantId", tenantId),
                        new SqlParameter("@StartDate", start),
                        new SqlParameter("@EndDate", end)
                    };

                return SqlDataHelper.ExecuteReader("GetOrderStatusSummaryByTenantAndDate",
                         sqlParameters.ToArray(),
                         MapOrderGetStatistics,
                         Constants.ConnectionString).FirstOrDefault();
            }


            public static TicketsStatisticsModel MapTicketsGetStatistics(IDataReader dataReader)
            {
                try
                {
                    TicketsStatisticsModel model = new TicketsStatisticsModel();
                    model.TotalTickets = SqlDataHelper.GetValue<long>(dataReader, "TotalTickets") ?? (long?)0;
                    model.TotalPending = SqlDataHelper.GetValue<long>(dataReader, "TotalPending") ?? (long?)0;
                    model.TotalOpened = SqlDataHelper.GetValue<long>(dataReader, "TotalOpened") ?? (long?)0;
                    model.TotalClosed = SqlDataHelper.GetValue<long>(dataReader, "TotalClosed") ?? (long?)0;
                    model.TotalExpired = SqlDataHelper.GetValue<long>(dataReader, "TotalExpired") ?? (long?)0;
                    //model.AvgResolutionTime = SqlDataHelper.GetValue<decimal>(dataReader, "AvgResolutionTime") ?? (decimal?)0;
                    model.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime>(dataReader, "LastClosedTicketDate");

                    return model;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            public static OrderStatisticsModel MapOrderGetStatistics(IDataReader dataReader)
            {
                try
                {
                    OrderStatisticsModel model = new OrderStatisticsModel();
                    model.TotalOrder = SqlDataHelper.GetValue<long>(dataReader, "TotalOrder") ?? (long?)0;
                    model.TotalOrderPending = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderPending") ?? (long?)0;
                    model.TotalOrderCompleted = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderCompleted") ?? (long?)0;
                    model.TotalOrderDeleted = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderDeleted") ?? (long?)0;
                    model.TotalOrderCanceled = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderCanceled") ?? (long?)0;
                    model.TotalOrderPreOrder = SqlDataHelper.GetValue<long>(dataReader, "TotalOrderPreOrder") ?? (long?)0;
                    model.TotalRevenue = SqlDataHelper.GetValue<decimal>(dataReader, "TotalRevenue") ?? (decimal?)0;
                    model.RewardPoints = SqlDataHelper.GetValue<decimal>(dataReader, "RewardPoints") ?? (decimal?)0;
                    model.RedeemedPoints = SqlDataHelper.GetValue<decimal>(dataReader, "RedeemedPoints") ?? (decimal?)0;
                    model.TotalTakeaway = SqlDataHelper.GetValue<long>(dataReader, "TotalTakeaway") ?? (long?)0;
                    model.TotalDelivery = SqlDataHelper.GetValue<long>(dataReader, "TotalDelivery") ?? (long?)0;
                    model.DeliveryCost = SqlDataHelper.GetValue<decimal>(dataReader, "DeliveryCost") ?? (decimal?)0;

                    return model;
                }
                catch (Exception)
                {
                    throw;
                }
            }


            private static  WalletModel walletGetByTenantId(int TenantId)
            {
                try
                {
                    WalletModel walletModel = new WalletModel();

                    var SP_Name = Constants.Wallet.SP_WalletGet;

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", TenantId)
        };

                    walletModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapWallet, Constants.ConnectionString).FirstOrDefault();

                    if (walletModel != null)
                    {
                        walletModel.TotalAmountSAR = (walletModel.TotalAmount > 0)
                            ? Math.Round(walletModel.TotalAmount * (decimal)3.75, 3)
                            : 0;
                    }
                    else
                    {
                        // Handle the case where no wallet data is returned, e.g., return a new WalletModel
                        walletModel = new WalletModel();  // or return null based on your business logic
                    }

                    return walletModel;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static WalletModel MapWallet(IDataReader dataReader)
            {
                try
                {
                    WalletModel model = new WalletModel();
                    model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                    model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                    model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
                    model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
                    model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

                    return model;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public static HostTenantListDto MapHostTenantsInfo(IDataReader dataReader)
            {
                try
                {
                    HostTenantListDto tenant = new HostTenantListDto();
                    tenant.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
                    tenant.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");
                    tenant.TenancyName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName");
                    tenant.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
                    tenant.CreatedBy = SqlDataHelper.GetValue<string>(dataReader, "CreatedBy");
                    tenant.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");
          

                    try
                    {

                        tenant.InvoiceId = SqlDataHelper.GetValue<string>(dataReader, "InvoiceId");

                    }
                    catch
                    {
                        tenant.InvoiceId ="";
                    }

                    try
                    {

                        tenant.Integration = SqlDataHelper.GetValue<string>(dataReader, "Integration");

                    }
                    catch
                    {
                        tenant.Integration =false;
                    }

                    try
                    {

                        tenant.CustomerName = SqlDataHelper.GetValue<string>(dataReader, "CommercialName");

                    }
                    catch
                    {
                        tenant.CustomerName = "";
                    }

                    try
                    {

                        tenant.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");

                    }
                    catch
                    {
                        tenant.PhoneNumber ="";
                    }

                    return tenant;
                }
                catch (Exception)
                {

                    throw;
                }
            }

            public static void CreateExportToExcelHostRecord(ExportToExcelHost model)
            {
                var sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@TenantName", (object)model.TenantName ?? DBNull.Value),
                        new SqlParameter("@Name", (object)model.Name ?? DBNull.Value),
                        new SqlParameter("@TenancyName", (object)model.TenancyName ?? DBNull.Value),
                        new SqlParameter("@IsActive", model.IsActive),
                        new SqlParameter("@Integration", (object)model.Integration ?? DBNull.Value),
                        new SqlParameter("@CreatedBy", (object)model.CreatedBy ?? DBNull.Value),
                        new SqlParameter("@CreationTime", model.CreationTime),
                        new SqlParameter("@InvoiceId", (object)model.InvoiceId ?? DBNull.Value),
                        new SqlParameter("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value),
                        new SqlParameter("@DomainName", (object)model.DomainName ?? DBNull.Value),
                        new SqlParameter("@CustomerName", (object)model.CustomerName ?? DBNull.Value),

                        // Ticket Statistics
                        new SqlParameter("@TotalTickets", model.TotalTickets),
                        new SqlParameter("@TotalPending", model.TotalPending),
                        new SqlParameter("@TotalOpened", model.TotalOpened),
                        new SqlParameter("@TotalClosed", model.TotalClosed),
                        new SqlParameter("@TotalExpired", model.TotalExpired),
                        new SqlParameter("@AvgResolutionTime", model.AvgResolutionTime),
                        new SqlParameter("@LastClosedTicketDate", (object)model.LastClosedTicketDate ?? DBNull.Value),

                        // Last Month Ticket Statistics
                        new SqlParameter("@LastMonthTotalTickets", model.LastMonthTotalTickets),
                        new SqlParameter("@LastMonthTotalPending", model.LastMonthTotalPending),
                        new SqlParameter("@LastMonthTotalOpened", model.LastMonthTotalOpened),
                        new SqlParameter("@LastMonthTotalClosed", model.LastMonthTotalClosed),
                        new SqlParameter("@LastMonthTotalExpired", model.LastMonthTotalExpired),
                        //new SqlParameter("@LastMonthLastClosedTicketDate", (object)model.LastMonthLastClosedTicketDate ?? DBNull.Value),

                        // Wallet
                        new SqlParameter("@WalletBalance", model.WalletBalance),

                        // Order Statistics


                        new SqlParameter("@TotalOrders", model.TotalOrder),
                        new SqlParameter("@PendingOrders", model.TotalOrderPending),
                        new SqlParameter("@DoneOrders", model.DoneOrders),
                        new SqlParameter("@DeletedOrders", model.TotalOrderDeleted),
                        new SqlParameter("@CanceledOrders", model.TotalOrderCanceled),
                        new SqlParameter("@PreOrders", model.TotalOrderPreOrder),

                        // Last Month Order Statistics
                        new SqlParameter("@LastMonthTotalOrders", model.LastMonthTotalOrders),
                        new SqlParameter("@LastMonthPendingOrders", model.LastMonthPendingOrders),
                        new SqlParameter("@LastMonthDoneOrders", model.LastMonthDoneOrders),
                        new SqlParameter("@LastMonthDeletedOrders", model.LastMonthDeletedOrders),
                        new SqlParameter("@LastMonthCanceledOrders", model.LastMonthCanceledOrders),
                        new SqlParameter("@LastMonthPreOrders", model.LastMonthPreOrders)
                    };

                try

                {
                    SqlDataHelper.ExecuteNoneQuery("sp_InsertExportToExcelHost", sqlParameters.ToArray(), Constants.ConnectionString);

                }
                catch (Exception ex)
                {
                    // Log error here
                    throw new Exception("Failed to create ExportToExcelHost record", ex);
                }
            }

        }
    }
    
}

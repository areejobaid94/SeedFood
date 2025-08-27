using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Domain.Repositories;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Elasticsearch.Net;
using Framework.Data;
using Framework.Data.Sql; // <-- Import PostgresDataHelper
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Tenants.Dashboard.Exporting;
using Infoseed.MessagingPortal.Wallet;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Infoseed.MessagingPortal.Zoho;
using Infoseed.MessagingPortal.Zoho.Dto;
using InfoSeedAzureFunction.AppFunEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using Npgsql;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static Infoseed.MessagingPortal.Tenants.Dashboard.Dto.CategoryTypeEnum;
using static Infoseed.MessagingPortal.Tenants.Dashboard.Dto.UserPerformanceBookingGenarecModel;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;


namespace Infoseed.MessagingPortal.Tenants.Dashboard
{
    [DisableAuditing]
    //[AbpAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardAppService : MessagingPortalAppServiceBase, ITenantDashboardAppService
    {
       // public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindb.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";
        public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindbstg.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";

        private readonly IRepository<Order, long> _orderRepository;
        private readonly IRepository<Evaluation, long> _evaluationRepository;

        private readonly IRepository<Contact> _contactRepository;
        private readonly IDocumentClient _IDocumentClient;
        private readonly ZohoAppService _zohoAppService;
        private readonly IWalletAppService _walletAppService;
        private readonly IUsageDetailsExcelExport _usageDetailsExcelExport;
        private readonly string _postgresConnection;

        public TenantDashboardAppService(IRepository<Order, long> orderRepository, IRepository<Evaluation, long> evaluationRepository, IRepository<Contact> contactRepository
            , IDocumentClient iDocumentClient
            ,ZohoAppService zohoAppService
            , IWalletAppService walletAppService
            , IUsageDetailsExcelExport usageDetailsExcelExport
            , IConfiguration configuration
            )
        {
            _orderRepository = orderRepository;
            _evaluationRepository = evaluationRepository;
            _contactRepository = contactRepository;
            _IDocumentClient = iDocumentClient;
            _zohoAppService = zohoAppService;
            _walletAppService = walletAppService;
            _usageDetailsExcelExport = usageDetailsExcelExport;
            _postgresConnection = configuration.GetConnectionString("postgres");
        }
        public GetMemberActivityOutput GetMemberActivity()
        {
            return new GetMemberActivityOutput
            (
                DashboardRandomDataGenerator.GenerateMemberActivities()
            );
        }
        public async System.Threading.Tasks.Task StatisticsWAUpdateSync()
        {

            var tenant = GetTenantList();
            DataTable tbl2 = new DataTable();
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("TenantId", typeof(int)));
            tbl.Columns.Add(new DataColumn("Year", typeof(int)));
            tbl.Columns.Add(new DataColumn("Month", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsageFreeConversationWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsageFreeUIWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsageFreeBIWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsagePaidConversationWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsagePaidUIWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsagePaidBIWA", typeof(int)));
            tbl.Columns.Add(new DataColumn("TotalUsageFreeEntry", typeof(int)));

            foreach (var TenantItem in tenant)
            {
                if (!string.IsNullOrEmpty(TenantItem.AccessToken) && TenantItem.TenantId == AbpSession.TenantId)
                {
                    DataTable dt = await GetUsageConversation(TenantItem, tbl);
                }
            }
            UpdateStatistics(tbl);
        }
        public GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input)
        {
            var output = new GetDashboardDataOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500),
                SalesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod),
                Expenses = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(1000, 9000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(10000, 90000),
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50),
                ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };

            return output;
        }

        public GetTopStatsOutput GetTopStats()
        {
            return new GetTopStatsOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500)
            };
        }

        public GetProfitShareOutput GetProfitShare()
        {
            return new GetProfitShareOutput
            {
                ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };
        }

        public GetDailySalesOutput GetDailySales()
        {
            return new GetDailySalesOutput
            {
                DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50)
            };
        }

        public GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input)
        {
            var salesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod);
            return new GetSalesSummaryOutput(salesSummary)
            {
                Expenses = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(0, 3000)
            };
        }

        public GetRegionalStatsOutput GetRegionalStats()
        {
            return new GetRegionalStatsOutput(
                DashboardRandomDataGenerator.GenerateRegionalStat()
            );
        }

        public GetGeneralStatsOutput GetGeneralStats()
        {
            return new GetGeneralStatsOutput
            {
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
            };
        }

        public GetTest GetTestlStats()
        {
            int[] test = new int[2];
            test[1] = 11;
            test[2] = 22;
            test[3] = 33;
            return new GetTest { DailySales = test };
        }

        public async Task<GetAllUserOutput> GetUserData(MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date)
        {
            try
            {

                //item.TenantId
                var userlist = GetUserInfoAsync(date);

                return new GetAllUserOutput()
                {
                    UserModel = userlist,
                };

            }
            catch
            {

                return new GetAllUserOutput()
                {
                    UserModel = new List<UserModel> { new UserModel {

                      profilePictureUrl = new Guid("N"),
                        CreationTime = DateTime.Now,
                        Id ="",
                        Name ="error",
                        userName = "error",
                        TenantId = 28,
                        TotalOfClose = 0,
                        SendMessagesCount = 0,
                        TotalOfOrder = 0

                    } },
                };

            }

        }

        private List<UserModel> GetUserInfoAsync(MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date)
        {
            var today = DateTime.Now;
            var dateselecte = today;
            dateselecte = SwitchFilter(date, ref today);


            //get all order 
            // var orderlist = _orderRepository.GetAll().ToList();

            // var orderlist = _orderRepository.GetAll().Where(e => e.orderStatus != OrderStatusEunm.Draft && e.CreationTime <= today.AddDays(1) && e.CreationTime >= dateselecte);//.ToList();

            //  order = await filteredorder.CountAsync();




            List<UserModel> tenantlistMap = new List<UserModel>();

            //get all user
            var queryUser = UserManager.Users.Include(t => t.CreatorUser);
            var Userlist = queryUser.ToList();
            //item.TenantId
            foreach (var item in Userlist)
            {
                // var Result = GetMessagesInfoAsync(item.TenantId, item.Id.ToString(), date).Result;

                var order = _orderRepository.GetAll().Where(o => o.orderStatus == OrderStatusEunm.Done && o.AgentId == item.Id && o.CreationTime <= today.AddDays(1) && o.CreationTime >= dateselecte);

                tenantlistMap.Add(new UserModel
                {
                    // ErrorMsg= Result.ErrorMsg,
                    profilePictureUrl = item.ProfilePictureId,
                    CreationTime = item.CreationTime,
                    Id = item.Id.ToString(),
                    Name = item.FullName,
                    userName = item.UserName,
                    TenantId = item.TenantId,
                    //TotalOfClose = Result.CloseCount,
                    //SendMessagesCount = Result.SendMessagesCount,
                    TotalOfOrder = order.Count()

                });


                //foreach (var orderr in orderlist)
                //{

                //    if (orderr.OrderType == OrderTypeEunm.Takeaway)
                //    {
                //        if (orderr.AreaId != null && orderr.AreaId != 0)
                //        {
                //            //var area = GetAreas(orderr.AreaId);


                //            _orderRepository.GetAll().Where(o=>o.orderStatus == OrderStatusEunm.Done && o.AgentId == item.Id && o.CreationTime <= today.AddDays(1) && o.CreationTime >= dateselecte)
                //            {

                //                order = order + 1;

                //            }




                //        }


                //    }
                //    else if (orderr.OrderType == OrderTypeEunm.Delivery)
                //    {
                //          // var branch = GetAreas(orderr.BranchAreaId);


                //                if (orderr.orderStatus == OrderStatusEunm.Done && orderr.AgentId == item.Id && orderr.CreationTime <= today.AddDays(1) && orderr.CreationTime >= dateselecte)
                //                {

                //                    order = order + 1;

                //                }








                //    }



                //}






            }




            return tenantlistMap;

        }

        public async Task<GetMeassagesInfoOutput> GetMessagesInfoAsync(long agentId, string agentName, Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date)
        {
            return null;

        }


        public async Task<GetAllDashboard> GetAllInfoAsync(Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date)
        {
            try
            {
                GetAllDashboard getAllDashboard = new GetAllDashboard();
                var today = DateTime.Now;

                var TenantId = (int?)AbpSession.TenantId;


                if (TenantId == null)
                {
                    return getAllDashboard;
                }
                //if (date.StartDate.AddDays(1).Month!=date.StartDate.Month)
                //{
                //    date.StartDate=today;

                //}
                var dateselecte = today;
                dateselecte = SwitchFilter(date, ref today);

                getAllDashboard = GetTenantDashboardStatistic(dateselecte.Year, dateselecte.Month, dateselecte, today);

                getAllDashboard.TotalOfRating = double.Parse(String.Format("{0:0.##}", getAllDashboard.TotalOfRating));

                return getAllDashboard;

            }
            catch (Exception ex)
            {
                return new GetAllDashboard()
                {
                    ErrorMsg = ex.Message,
                    TotalOfAllContact = 0,
                    TotalOfClose = 0,
                    TotalOfSendMessages = 0,
                    TotalOfOrders = 0,
                    Bandel = 0,
                    RemainingConversation = 0
                };
            }
        }

        private static DateTime SwitchFilter(Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date, ref DateTime today)
        {
            DateTime dateselecte;
            if (date == null)
            {
                dateselecte = today.AddYears(-10);
                return dateselecte;
            }
            switch (date.IncomeStatisticsDateInterval.ToString())
            {
                case "None":

                    string timeStart = "";
                    string timeEnd = "";
                    try
                    {
                        timeStart = date.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                        timeEnd = date.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {

                        timeStart = date.EndDate.Day.ToString() + "/" + date.EndDate.Month.ToString() + "/" + date.EndDate.Year;
                        timeEnd = date.StartDate.Day.ToString() + "/" + date.StartDate.Month.ToString() + "/" + date.StartDate.Year;


                        timeStart = date.EndDate.ToString(timeStart, CultureInfo.InvariantCulture);
                        timeEnd = date.StartDate.ToString(timeEnd, CultureInfo.InvariantCulture);

                    }




                    var resulttimeStart = Convert.ToDateTime(timeStart, CultureInfo.InvariantCulture);
                    var resulttimeEnd = Convert.ToDateTime(timeEnd, CultureInfo.InvariantCulture);

                    today = resulttimeStart;
                    dateselecte = resulttimeEnd;
                    break;
                case "Daily":
                    today = DateTime.Today;
                    dateselecte = DateTime.Today;
                    break;
                case "Weekly":
                    dateselecte = today.AddDays(-7);
                    break;
                case "Monthly":
                    dateselecte = today.AddMonths(-1);
                    break;
                case "Yearly":
                    dateselecte = today.AddYears(-1);
                    break;
                default:
                    dateselecte = today;
                    break;
            }

            return dateselecte;
        }

        public async Task<GetBarChartInfoOutput> GetBarChartInfo()
        {

            try
            {
                //get all order 
                var orderlist = _orderRepository.GetAll().Where(x => x.TenantId == AbpSession.TenantId && x.orderStatus == OrderStatusEunm.Done).ToList();
                BarChartInfoModel barChartInfoModel = new BarChartInfoModel();
                var timeList = new List<string>() { "1:00 AM - 2:00 AM", "2:00 AM - 3:00 AM", "3:00 AM - 4:00 AM", "4:00 AM - 5:00 AM", "5:00 AM- 6:00 AM", "6:00 AM - 7:00 AM", "7:00 AM - 8:00 AM", "8:00 AM - 9:00 AM", "9:00 AM - 10:00 AM", "10:00 AM - 11:00 AM", "11:00 AM - 12:00 PM", "12:00 PM - 1:00 PM", "1:00 PM - 2:00 PM", "2:00 PM - 3:00 PM", "3:00 PM - 4:00 PM", "4:00 PM - 5:00 PM", "5:00 PM - 6:00 PM", "6:00 PM - 7:00 PM", "7:00 PM - 8:00 PM", "8:00 PM - 9:00 PM", "9:00 PM - 10:00 PM", "10:00 PM - 11:00 PM", "11:00 PM -12:00 AM", "12:00 AM - 1:00 AM" };

                var timeListNew = new List<string>();
                List<string> labels = new List<string>();
                List<int> data = new List<int>();
                List<string> backgroundColor = new List<string>();

                foreach (var time in timeList)
                {

                    //var firstAM = time.Replace("AM", "");
                    //var firstPM = firstAM.Replace("PM", "");

                    var second = time.Split("-").ToList();
                    string timeStart = second[0].Trim();
                    string timeEnd = second[1].Trim();


                    var resulttimeStart = Convert.ToDateTime(timeStart);
                    var resulttimeEnd = Convert.ToDateTime(timeEnd);


                    var OrderCountInThisTime = 0;

                    foreach (var order in orderlist)
                    {

                        string orderTime = order.CreationTime.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);

                        var resulttimeorde = Convert.ToDateTime(orderTime, CultureInfo.InvariantCulture);


                        if ((resulttimeStart <= resulttimeorde && resulttimeorde <= resulttimeEnd))
                        {


                            OrderCountInThisTime = OrderCountInThisTime + 1;
                        }



                        //    var orderCount = orderlist.Where(x => x.TenantId == AbpSession.TenantId && x.orderStatus != OrderStatusEunm.Done && Convert.ToDateTime(x.CreationTime.ToString("hh:mm tt")) <= resulttimeStart && Convert.ToDateTime(x.CreationTime.ToString("hh:mm tt")) >= resulttimeEnd).Count();
                        // labels.Add();

                    }



                    data.Add(OrderCountInThisTime);



                }
                var num = 0;
                foreach (var time in timeList)
                {
                    num = num + 1;
                    //time.Replace(":00", "");

                    timeListNew.Add(num.ToString());
                }

                barChartInfoModel.labels = timeListNew;


                barChartInfoModel.data = data;

                return new GetBarChartInfoOutput
                {
                    barChartInfoModel = barChartInfoModel
                };

            }
            catch
            {

                return new GetBarChartInfoOutput
                {
                    barChartInfoModel = new BarChartInfoModel
                    {
                        backgroundColor = new List<string> { "error" },
                        data = new List<int> { 0 },
                        labels = new List<string> { "error" }

                    }
                };

            }


        }
        public async Task<GetBarChartInfoOutput> GetBarChartInfoArea(int id)
        {

            try
            {
                BarChartInfoModel barChartInfoModel = new BarChartInfoModel();

                List<string> labels = new List<string>();
                List<int> data = new List<int>();
                List<string> backgroundColor = new List<string>();

                var Area = GetLocationsByParentId(id);

                foreach (var area in Area)
                {


                    labels.Add(area.LocationName);

                    data.Add(1);
                }

                barChartInfoModel.labels = labels;
                barChartInfoModel.data = data;

                return new GetBarChartInfoOutput
                {
                    barChartInfoModel = barChartInfoModel
                };

            }
            catch
            {
                return new GetBarChartInfoOutput
                {
                    barChartInfoModel = new BarChartInfoModel
                    {
                        backgroundColor = new List<string> { "error" },
                        data = new List<int> { 0 },
                        labels = new List<string> { "error" }

                    }
                };

            }


        }
        private ConversationMeasurementsModel GetConversationMeasurements(int TenantId, int Year, int Month)
        {
            try
            {

                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ConversationMeasurements] where TenantId=" + TenantId + " and Year = " + Year + " and Month= " + Month;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                ConversationMeasurementsModel branches = new ConversationMeasurementsModel();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    branches = new ConversationMeasurementsModel
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        BusinessInitiatedCount = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BusinessInitiatedCount"].ToString()),
                        ReferralConversionCount = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ReferralConversionCount"].ToString()),
                        TotalFreeConversation = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalFreeConversation"]),
                        UserInitiatedCount = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserInitiatedCount"].ToString()),
                        Month = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Month"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        Year = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Year"]),



                    };
                }

                conn.Close();
                da.Dispose();

                return branches;
            }
            catch (Exception)
            {

                return null;
            }

        }

        private List<LocationInfoModelD> GetAllCitty(int? LocationId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Locations] where Id= " + LocationId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<LocationInfoModelD> location = new List<LocationInfoModelD>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                //OrderStatusEunm MyStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), "sadas", true);

                location.Add(new LocationInfoModelD
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                    //ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                    //GoogleURL = dataSet.Tables[0].Rows[i]["GoogleURL"].ToString(),
                    LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                    //LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString(),
                });



            }

            conn.Close();
            da.Dispose();

            return location;

        }


        private List<LocationInfoModelD> GetLocationsByParentId(int parentId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            //AbpSession.TenantId
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@ParentId",parentId)
                     };


            IList<LocationInfoModelD> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.LocationsGetByParentId",
                       sqlParameters.ToArray(),
                       MapLocation,
                       connString);




            return result.ToList();
        }

        private static LocationInfoModelD MapLocation(IDataReader dataReader)
        {
            LocationInfoModelD model = new LocationInfoModelD
            {
                Id = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                LocationName = SqlDataHelper.GetValue<string>(dataReader, "LocationName"),
                LevelId = SqlDataHelper.GetValue<int>(dataReader, "LevelId"),
                ParentId = SqlDataHelper.GetValue<int>(dataReader, "ParentId"),
                GoogleURL = SqlDataHelper.GetValue<string>(dataReader, "GoogleURL"),
                LocationNameEn = SqlDataHelper.GetValue<string>(dataReader, "LocationNameEn"),
                AreaName = SqlDataHelper.GetValue<string>(dataReader, "AreaName"),
                CityName = SqlDataHelper.GetValue<string>(dataReader, "CityName"),
                DeliveryCost = SqlDataHelper.GetValue<string>(dataReader, "DeliveryCost"),
                AreaId = SqlDataHelper.GetValue<int>(dataReader, "AreaId"),
                CityId = SqlDataHelper.GetValue<int>(dataReader, "CityId"),


            };
            return model;
        }

        //    private async Task<GetAllDashboard> GetDashboardStastic
        //    {
        //        return new GetAllDashboard()
        //    {
        //                TotalOfAllContact = contact,
        //                TotalOfClose = Close,
        //                TotalOfSendMessages = SendMessage,
        //                TotalOfOrders = order,
        //                TotalOfRating = double.Parse(DRatingFormat),
        //                Bandel = Bandel,
        //                RemainingConversation = RemainingConversation

        //            };


        //}


        private GetAllDashboard GetTenantDashboardStatistic(int year, int month, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var tenantId = AbpSession.TenantId ?? 0;

                var sqlParameters = new Npgsql.NpgsqlParameter[]
                {
                    new Npgsql.NpgsqlParameter("p_tenant_id", tenantId),
                    new Npgsql.NpgsqlParameter("p_year", year),
                    new Npgsql.NpgsqlParameter("p_month", month),
                    new Npgsql.NpgsqlParameter("p_datefrom", dateFrom),
                    new Npgsql.NpgsqlParameter("p_dateto", dateTo)
                };

                var functionName = "dbo.tenant_dashboard_statistic_get";

                // Get the PostgreSQL connection string from configuration
                string connectionString = _postgresConnection;

                // Execute function using PostgresDataHelper
                var result = PostgresDataHelper.ExecuteFunction(functionName, sqlParameters, MapTenantDashboardStatistic, connectionString)
                                               .FirstOrDefault();

                if (result != null)
                {
                    // Keep the original calculation logic
                    result.RemainingUIConversation = result.TotalUIConversation - result.TotalUsageUIConversation;
                    result.RemainingBIConversation = result.TotalBIConversation - result.TotalUsageBIConversation;
                    result.RemainingFreeConversation = result.TotalFreeConversationWA - result.TotalUsageFreeConversation;
                }

                return result;
            }
            catch (Exception ex)
            {
                // Best practice: preserve stack trace
                throw;
            }
        }


        private GetAllDashboard MapTenantDashboardStatistic(IDataReader dataReader)
        {
            GetAllDashboard GetAllDashboard = new GetAllDashboard();
            GetAllDashboard.TotalOfAllContact = SqlDataHelper.GetValue<int>(dataReader, "TotalOfAllContact");
            GetAllDashboard.TotalOfOrders = SqlDataHelper.GetValue<int>(dataReader, "TotalOfOrders");
            GetAllDashboard.Bandel = SqlDataHelper.GetValue<int>(dataReader, "ConversationBundle");

            GetAllDashboard.TotalOfRating = SqlDataHelper.GetValue<double>(dataReader, "TotalOfRating");
            GetAllDashboard.TotalFreeConversationWA = SqlDataHelper.GetValue<decimal>(dataReader, "TotalFreeConversationWA");
            GetAllDashboard.TotalUsageFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversationWA");
            GetAllDashboard.TotalUsageFreeUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeUIWA");
            GetAllDashboard.TotalUsageFreeBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeBIWA");
            GetAllDashboard.TotalUsagePaidConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidConversationWA");
            GetAllDashboard.TotalUsagePaidUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidUIWA");
            GetAllDashboard.TotalUsagePaidBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidBIWA");
            GetAllDashboard.TotalUsageFreeEntry = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeEntry");
            GetAllDashboard.TotalUsageFreeConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageFreeConversation");
            GetAllDashboard.TotalUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUIConversation");
            GetAllDashboard.TotalUsageUIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageUIConversation");
            GetAllDashboard.TotalBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalBIConversation");
            GetAllDashboard.TotalUsageBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageBIConversation");
            //GetAllDashboard.TotalUsageBIConversation = SqlDataHelper.GetValue<decimal>(dataReader, "TotalUsageBIConversation");


            GetAllDashboard.RemainingConversation = (int)GetAllDashboard.TotalFreeConversationWA - GetAllDashboard.TotalUsageFreeConversationWA;
            return GetAllDashboard;
        }



        private List<TenantModel> GetTenantList()
        {

            //var x = GetContactId("962779746365", "28");


            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<TenantModel> order = new List<TenantModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                order.Add(new TenantModel
                {
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    ZohoCustomerId = dataSet.Tables[0].Rows[i]["ZohoCustomerId"].ToString(),
                    D360Key = dataSet.Tables[0].Rows[i]["D360Key"].ToString(),
                    CautionDays = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CautionDays"]),
                    WarningDays = Convert.ToInt32(dataSet.Tables[0].Rows[i]["WarningDays"]),
                    AccessToken = dataSet.Tables[0].Rows[i]["AccessToken"].ToString(),
                    WhatsAppAccountID = dataSet.Tables[0].Rows[i]["WhatsAppAccountID"].ToString(),
                });

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private async Task<DataTable> GetUsageConversation(TenantModel tenant, DataTable tbl)
        {
            try
            {
                Dictionary<string, int> conversations = new Dictionary<string, int>();

                //string userInitiatedDirection = Enum.GetName(typeof(WhatsAppConversationDirectionEnum), WhatsAppConversationDirectionEnum.USER_INITIATED);
                //string businessInitiatedDirection = Enum.GetName(typeof(WhatsAppConversationDirectionEnum), WhatsAppConversationDirectionEnum.BUSINESS_INITIATED);
                //string unknownDirection = Enum.GetName(typeof(WhatsAppConversationDirectionEnum), WhatsAppConversationDirectionEnum.UNKNOWN);

                string freeTierConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_TIER);
                string freeEntryPointConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_ENTRY_POINT);
                string unknownConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.UNKNOWN);


                string serviceCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.SERVICE);
                string utilityCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.UTILITY);
                string authenticationCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.AUTHENTICATION);
                string marketingCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.MARKETING);

                int totalUsageFreeUI = 0;
                int totalUsageFreeBI = 0;
                int totalUsagePaidUI = 0;
                int totalUsagePaidBI = 0;
                int totalUsageFreeEntry = 0;

                DateTime date = DateTime.UtcNow;
                int year = date.Year;
                int month = date.Month;
                DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1).AddHours(-AppSettingsModel.AddHour);
                //DateTime currentDayOfMonth = new DateTime(date.Year, date.Month, date.Day,date.Hour);

                WhatsAppAnalyticsModel objWhatsAppAnalytic = await getWhatsAppAnalyticAsync(firstDayOfMonth, DateTime.UtcNow, tenant);

                Conversation_Analytics conversation_Analytics = new Conversation_Analytics();
                List<Datum> data = new List<Datum>();
                List<Data_Points> data_Points = new List<Data_Points>();
                conversation_Analytics = objWhatsAppAnalytic.conversation_analytics;

                if (conversation_Analytics != null)
                {
                    data = conversation_Analytics.data;
                    foreach (var item in data)
                    {
                        data_Points = item.data_points;
                    }
                    foreach (var item2 in data_Points)
                    {
                        if (item2.conversation_category == serviceCategory && item2.conversation_type == freeTierConversation)
                        {
                            totalUsageFreeUI += item2.conversation;
                        }
                        if ((item2.conversation_category == utilityCategory || item2.conversation_category == authenticationCategory || item2.conversation_category == marketingCategory) && item2.conversation_type == freeTierConversation)
                        {
                            totalUsageFreeBI += item2.conversation;
                        }
                        if (item2.conversation_category == serviceCategory && item2.cost > 0)
                        {
                            totalUsagePaidUI += item2.conversation;
                        }
                        if ((item2.conversation_category == utilityCategory || item2.conversation_category == authenticationCategory || item2.conversation_category == marketingCategory) && item2.cost > 0)
                        {
                            totalUsagePaidBI += item2.conversation;
                        }
                        if (item2.conversation_type == freeEntryPointConversation)
                        {
                            totalUsageFreeEntry += item2.conversation;
                        }
                    }

                    int totalUsageFree = totalUsageFreeUI + totalUsageFreeBI;
                    int tatalUsagePaid = totalUsagePaidUI + totalUsagePaidBI;


                    DataRow dr = tbl.NewRow();
                    dr["TotalUsageFreeConversationWA"] = totalUsageFree;
                    dr["TotalUsageFreeUIWA"] = totalUsageFreeUI;
                    dr["TotalUsageFreeBIWA"] = totalUsageFreeBI;
                    dr["TotalUsagePaidConversationWA"] = tatalUsagePaid;
                    dr["TotalUsagePaidUIWA"] = totalUsagePaidUI;
                    dr["TotalUsagePaidBIWA"] = totalUsagePaidBI;
                    dr["TotalUsageFreeEntry"] = totalUsageFreeEntry;
                    dr["Year"] = year;
                    dr["Month"] = month;
                    dr["TenantId"] = tenant.TenantId;
                    tbl.Rows.Add(dr);
                }
                return tbl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void UpdateStatistics(DataTable tbl)
        {
            try
            {

                var SP_Name = "[dbo].[ConversationMeasurementBulkUpdate]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@ConversationMeasurementTable", tbl) };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        private async Task<WhatsAppAnalyticsModel> getWhatsAppAnalyticAsync(DateTime start, DateTime end, TenantModel tenant)
        {
            int startTime, endTime = 0;

            WhatsAppAnalyticsModel objWhatsAppAnalytic = new WhatsAppAnalyticsModel();

            using (var httpClient = new HttpClient())
            {
                string[] obj = { "CONVERSATION_DIRECTION", "CONVERSATION_TYPE", "COUNTRY", "PHONE", "CONVERSATION_CATEGORY" };

                startTime = (int)ConvertDatetimeToUnixTimeStamp(start);
                endTime = (int)ConvertDatetimeToUnixTimeStamp(end);

                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "?fields=conversation_analytics.start(" + startTime + ").end(" + endTime + ").granularity(DAILY).dimensions(" + obj[0] + "," + obj[1] + "," + obj[2] + "," + obj[3] + "," + obj[4] + ")";

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        var WhatsAppAnalytic = await content.ReadAsStringAsync();
                        objWhatsAppAnalytic = JsonConvert.DeserializeObject<WhatsAppAnalyticsModel>(WhatsAppAnalytic);

                    }
                }

                return objWhatsAppAnalytic;

            }
        }
        private long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - originDate;
            return (long)Math.Floor(diff.TotalSeconds);
        }


        #region public NewDashboard
        public async Task<DashbardModel> GetDashoardInfo(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                DashbardModel dashbardModel = new DashbardModel();
                dashbardModel.bundleModel = await GetUsageConversationNew(start, end, TenantId);
                dashbardModel.walletModel = WalletGetByTenantId(TenantId);
                if (dashbardModel.walletModel == null)
                {
                    _walletAppService.CreateWallet(TenantId);
                    dashbardModel.walletModel = WalletGetByTenantId(TenantId);
                }
                dashbardModel.userPerformanceOrderModel = GetPerformanceOrder(start, end, TenantId);
                dashbardModel.userPerformanceTicketModel = GetPerformanceTeckits(start, end, TenantId);
                dashbardModel.userPerformanceBookingModel = GetPerformanceBooking(start, end, TenantId);

                return dashbardModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BundleModel> GetUsageConversationNew(DateTime start, DateTime end, int TenantId)
        {
            return await getUsageConversationNewAsync(start, end, TenantId);
        }
        public TenantModelDash GetTenantById(int TenantId)
        {
            return getTenantById(TenantId);
        }
        public async Task<string> WalletDeposit(InvoicesWalletModel invoiseModel)
        {
            try
            {
                long ZohoId = 0 ;
                string zoho= getZohoCustomerIds(invoiseModel.TenantId);
                if (zoho != null && zoho != "")
                {
                   // ZohoId = long.Parse(getZohoCustomerIds(invoiseModel.TenantId));
                    ZohoId = long.Parse(zoho);
                }
                else
                { return null; }

                CreateInvoicesDashbordModel models = new CreateInvoicesDashbordModel();
                models.ZohoCustomerIds = zoho;
                models.UserId = invoiseModel.UserId;
                models.WalletId = invoiseModel.WalletId;
                models.TenantId = invoiseModel.TenantId;
                models.TotalAmount = invoiseModel.TotalAmount;
                models.DepositDate = invoiseModel.DepositDate;
                models.Country = invoiseModel.Country;

                UpdateInvoicesModel updateInvoicesModel = new UpdateInvoicesModel();
                //createInvoicesDashbordModel.ZohoCustomerIds = 3741872000000075184;
                if (invoiseModel.TotalAmount > 0 && ZohoId > 0)
                {
                    updateInvoicesModel = await _zohoAppService.InvoicesCreateAsync(models);
                }
                else
                {
                    return null;
                }
                if(updateInvoicesModel == null )
                    return null;

                WalletModel model = new WalletModel();
                
                model = WalletGetByTenantId(invoiseModel.TenantId);
                if (model != null && invoiseModel.UserId > 0)
                {

                    UsersDashModel usersDashModel = new UsersDashModel();
                    usersDashModel = GetUserInfo(invoiseModel.UserId);
                     //Add in transaction table 
                    TransactionModel transactionModel = new TransactionModel();

                    transactionModel.DoneBy = usersDashModel.Name;
                    transactionModel.TotalTransaction = invoiseModel.TotalAmount;
                    transactionModel.TransactionDate = DateTime.UtcNow;
                    transactionModel.CategoryType = Enum.GetName(typeof(TransactionType), TransactionType.Deposit);
                    transactionModel.TotalRemaining = model.TotalAmount + invoiseModel.TotalAmount;
                    transactionModel.WalletId = invoiseModel.WalletId;
                    transactionModel.Country = invoiseModel.Country;
                    transactionModel.TenantId = invoiseModel.TenantId;
                    transactionModel.invoiceId = updateInvoicesModel.invoice.invoice_id;
                    transactionModel.invoiceUrl = " ";
                    transactionModel.IsPayed = false;
                    transactionModel.Note = "Paid by User";

                    AddTransaction(transactionModel);

                    return updateInvoicesModel.invoice.invoice_url;

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public WalletModel WalletGetByTenantId(int TenantId)
        {
            try
            {
                return walletGetByTenantId(TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long AddTransaction(TransactionModel model)
        {
            return addTransaction(model);
        }
        public List<LastFourTransactionModel> TransactionGetLastFour(int TenantId)
        {
            try
            {
                List<LastFourTransactionModel> lastFourTransactionModels = new List<LastFourTransactionModel>();    
                List<TransactionModel> model = new List<TransactionModel>();
                model = transactionGetLastFour(TenantId);

                if (model != null)
                {
                    foreach (var Mo in model)
                    {
                        switch (Mo.CategoryType)
                        {
                            case "Deposit":
                                lastFourTransactionModels.Add(new LastFourTransactionModel
                                {
                                    icon = "bi-check2",
                                    CategoryType = Mo.CategoryType,
                                    Total ="+$"+ Mo.TotalTransaction.ToString()
                                });
                                break;
                            case "Marketing Conversations":
                                lastFourTransactionModels.Add(new LastFourTransactionModel
                                {
                                    icon = "bi-shield-check",
                                    CategoryType = Mo.CategoryType,
                                    Total = "-$" + Mo.TotalTransaction.ToString()
                                });
                                break;
                            case "Utility Conversations":
                                lastFourTransactionModels.Add(new LastFourTransactionModel
                                {
                                    icon = "bi-shield-check",
                                    CategoryType = Mo.CategoryType,
                                    Total = "-$" + Mo.TotalTransaction.ToString()
                                });
                                break;
                            case "Service Conversation":
                                lastFourTransactionModels.Add(new LastFourTransactionModel
                                {
                                    icon = "bi-currency-dollar",
                                    CategoryType = Mo.CategoryType,
                                    Total = "-$" + Mo.TotalTransaction.ToString()
                                });
                                break;
                        }
                    }
                    return lastFourTransactionModels; 
                }
                else
                { return lastFourTransactionModels; }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> CountryGetAll(int TenantId)
        {
            try
            {
                List<string> strings = new List<string>();
                List<CountryCodeModel> countryCodeModels = new List<CountryCodeModel>();
                countryCodeModels = countryGetAll(TenantId);
                if (countryCodeModels != null)
                {
                    foreach (var countryModel in countryCodeModels)
                    {
                        strings.Add(countryModel.Country);
                    }
                }
                return strings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ConversationPriceModel GetConvarsationPrice(ConversationPriceModel model, int TenantId)
        {
            try
            {
                return getConvarsationPrice(model, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UsersDashModel> GetAllUser(int TenantId)
        {
            try
            {
                return getAllUser(TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //User Performance (Orders)
        public OrderDashbordModel OrdersGetAll(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                return ordersGetAll(start, end, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserPerformanceOrderGenarecModel GetPerformanceOrder(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                return getPerformanceOrder(start, end, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //User Performance (Tickits)
        public TickitsDashbordModel TickitsGetAll(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                return tickitsGetAll(start, end, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserPerformanceTicketGenarecModel GetPerformanceTeckits(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                return getPerformanceTeckits(start, end , TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //User Performance (Booking)
        public BookingDashbordModel BookingGetAll(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                return bookingGetAll(start, end, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserPerformanceBookingGenarecModel GetPerformanceBooking(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                return getPerformanceBooking(start, end, TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        //Orders Statistics (Order)
        public OrderStatisticsModel OrdersStatisticsGet(DateTime start, DateTime end, int TenantId, long BranchId = 0)
        {
            try
            {
                OrderStatisticsModel orderStatisticsModel = new OrderStatisticsModel();
                orderStatisticsModel=ordersStatisticsGet(start, end, TenantId, BranchId);
                decimal Total= orderStatisticsModel.TotalOrder;
                if (Total > 0)
                {
                    orderStatisticsModel.PercentagePending = (int)((orderStatisticsModel.TotalOrderPending / Total) * 100);
                    orderStatisticsModel.PercentageCompleted = (int)((orderStatisticsModel.TotalOrderCompleted / Total) *100);
                    orderStatisticsModel.PercentageDeleted = (int)((orderStatisticsModel.TotalOrderDeleted / Total) *100);
                    orderStatisticsModel.PercentageCanceled = (int)((orderStatisticsModel.TotalOrderCanceled / Total) *100);
                    orderStatisticsModel.PercentagePreOrder = (int)((orderStatisticsModel.TotalOrderPreOrder / Total) *100);
                }

                return orderStatisticsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Booking Statistics (Booking)
        public BookingStatisticsModel BookingStatisticsGet(DateTime start, DateTime end, int TenantId, long UserId = 0)
        {
            try
            {
                // PostgreSQL function name
                string functionName = "dbo.booking_statistics_get";

                // Prepare PostgreSQL parameters
                var npgsqlParameters = new Npgsql.NpgsqlParameter[]
                {
            new Npgsql.NpgsqlParameter("p_tenantid", TenantId),
            new Npgsql.NpgsqlParameter("p_start", start),
            new Npgsql.NpgsqlParameter("p_end", end),
            new Npgsql.NpgsqlParameter("p_userid", UserId)
                };

                // Execute function using your helper
                var result = PostgresDataHelper.ExecuteFunction(
                    functionName,
                    npgsqlParameters,
                    DataReaderMapper.MapBookingStatisticsPSQL,
                    _postgresConnection
                ).FirstOrDefault() ?? new BookingStatisticsModel();

                // Calculate percentages (same logic as before)
                decimal Total = result.TotalAppointments;
                if (Total > 0)
                {
                    result.PercentageBooked = (int)((result.TotalBooked / Total) * 100);
                    result.PercentageConfirmed = (int)((result.TotalConfirmed / Total) * 100);
                    result.PercentageCanceled = (int)((result.TotalCancelled / Total) * 100);
                    result.PercentagePending = (int)((result.TotalPending / Total) * 100);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Tickets Statistics (Tickets)
        public TicketsStatisticsModel TicketsStatisticsGet(DateTime start, DateTime end,int TenantId)
        {
            try
            {

                TicketsStatisticsModel ticketsStatisticsModel = new TicketsStatisticsModel();
                ticketsStatisticsModel = ticketsStatisticsGet(start, end, TenantId);
                decimal Total = ticketsStatisticsModel.TotalTickets;
                if (Total > 0)
                {
                   ticketsStatisticsModel.PercentagePending = (int)((ticketsStatisticsModel.TotalPending / Total) * 100);
                   ticketsStatisticsModel.PercentageOpened = (int)((ticketsStatisticsModel.TotalOpened / Total) * 100);
                   ticketsStatisticsModel.PercentageClosed = (int)((ticketsStatisticsModel.TotalClosed / Total) * 100);
                   ticketsStatisticsModel.PercentageExpired = (int)((ticketsStatisticsModel.TotalExpired / Total) * 100);

                   decimal minutes = ticketsStatisticsModel.AvgResolutionTime; // Replace with your desired number of minutes
                   decimal hours = minutes / 60;
                   ticketsStatisticsModel.AvgResolutionTime = Math.Round(hours,2);
                }
                return ticketsStatisticsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    

        //Campaign Statistics (Campaign)
        [HttpGet]
        public async Task<CampaignStatisticsModel> CampaignStatisticsGet(DateTime start, DateTime end, int TenantId, long CampaignId = 0)
        {
            try
            {
                return await campaignStatisticsGet(start, end, TenantId, CampaignId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Contact Statistics (Contact)
        public ContactStatisticsModel ContactStatisticsGet(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                return contactStatisticsGet(start ,end,TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BranchsModel> BranchsGetAll(int TenantId)
        {
            try
            {
                return branchsGetAll(TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public List<CampaignDashModel> GetAllCampaign(int TenantId)
        {
            try
            {
                return getAllCampaign(TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BestSellingModel> GetBestSellingItems(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                return getBestSellingItems(start,end ,TenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void CreateDepocit(int TenantId, string invoiceId, decimal Total)
        {
            try
            {
                WalletModel walletModel = new WalletModel();

                // her shod get the wallet for same Tenant
                walletModel = WalletGetByTenantId(TenantId);

                if (walletModel != null)
                {
                    long result = walletDeposit(walletModel, Total);


                    // update Tenant bundle
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantId);
                    tenant.IsBundleActive = true;
                    await itemsCollection.UpdateItemAsync(tenant._self, tenant);

                    //Add in transaction table 
                    TransactionModel transactionModel = new TransactionModel();
                    transactionModel = GetTransaction(TenantId, invoiceId);
                    //transactionModel.IsPayed = true;
                    
                    UpdeteTransaction(transactionModel.WalletId, transactionModel.invoiceId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public UsageDetailsGenericModel GetUsageDetails( int TenantId, long? CampaignId = 0, string GroupBy = "", int? pageNumber = 0, int? pageSize = 10, DateTime? start = null, DateTime? end = null)
        {
            try
            {
                return getUsageDetails(TenantId, CampaignId, GroupBy, pageNumber, pageSize, start, end);
                //if (start != null)
                //{ return getUsageDetails(TenantId, CampaignId, GroupBy, pageNumber, pageSize, start.Value, end.Value); }
                //else
                //{ return getUsageDetails(TenantId, CampaignId, GroupBy, pageNumber, pageSize, start, end); }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public UsageStatisticsModel GetUsageStatistics(int TenantId, long CampaignId)
        {
            try
            {
                return getUsageStatistics(TenantId, CampaignId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileDto GetUsageDetailsToExcel(int TenantId, long? CampaignId = 0, string GroupBy = "", DateTime? start = null, DateTime? end = null)
        {
            try
            {
                FileDto fileDto = new FileDto();    
                int pageNumber = 0;
                int pageSize = 2147483647;

                UsageDetailsGenericModel itemes = getUsageDetails(TenantId, CampaignId, GroupBy, pageNumber, pageSize,start ,end);
                if (itemes != null)
                {
                    return _usageDetailsExcelExport.ExportToFile(itemes.usageDetails);
                }
                return fileDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region private NewDashboard 
        private async Task<BundleModel> getUsageConversationNewAsync(DateTime start, DateTime end, int tenantId)
        {
            try
            {
                BundleModel bundleModel = new BundleModel();

                string freeTierConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_TIER);
                string freeEntryPointConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_ENTRY_POINT);
                string regularConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.REGULAR);


                string marketingCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.MARKETING);
                string serviceCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.SERVICE);
                string utilityCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.UTILITY);

                int totalFreeTier = 0;
                int totalfreeEntryPoint = 0;

                int totalMarketing = 0;
                int totalService = 0;
                int totalUtility = 0;

                float totalMarketingCharge = 0;
                float totalServiceCharge = 0;
                float totalUtilityCharge = 0;

                //DateTime date = DateTime.UtcNow;
                //int year = date.Year;
                //int month = date.Month;
                //DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1).AddHours(-AppSettingsModel.AddHour);
                TenantModelDash tenant = new TenantModelDash();
                tenant = GetTenantById(tenantId);

                WhatsAppBundleModel whatsAppBundelModel = await getBundelAsync(start, end, tenant);
                if (whatsAppBundelModel.conversation_analytics == null)
                    return bundleModel;

                Conversation_Analytics1 conversation_Analytics = new Conversation_Analytics1();
                Datum1[] data;
                Data_Points1[] data_Points = null;

                conversation_Analytics = whatsAppBundelModel.conversation_analytics;


                if (conversation_Analytics != null)
                {
                    data = conversation_Analytics.data;
                    foreach (var item in data)
                    {
                        data_Points = item.data_points;
                    }
                    if (data_Points != null)
                    {
                        foreach (var item2 in data_Points)
                        {
                            if (item2.conversation_type == freeEntryPointConversation || item2.conversation_type == freeTierConversation)
                            {
                                if (item2.conversation_type == freeTierConversation)
                                {
                                    totalFreeTier += item2.conversation;
                                }
                                else if (item2.conversation_type == freeEntryPointConversation)
                                {
                                    totalfreeEntryPoint += item2.conversation;
                                }
                            }
                            else
                            {
                                if (item2.cost > 0)
                                {
                                    if (item2.conversation_category == marketingCategory)
                                    {
                                        totalMarketing += item2.conversation;
                                        totalMarketingCharge = (float)(totalMarketingCharge + (item2.cost * 1.2));
                                    }
                                    else if (item2.conversation_category == serviceCategory)
                                    {
                                        totalService += item2.conversation;
                                        totalServiceCharge = (float)(totalServiceCharge + (item2.cost * 1.2));

                                    }
                                    else if (item2.conversation_category == utilityCategory)
                                    {
                                        totalUtility += item2.conversation;
                                        totalUtilityCharge = (float)(totalUtilityCharge + (item2.cost * 1.2));

                                    }
                                }
                            }
                        }
                    }

                    bundleModel.TenantId = tenantId;
                    bundleModel.TotalFreeConversation = totalFreeTier + totalfreeEntryPoint;
                    bundleModel.TotalFreeTier = totalFreeTier;
                    bundleModel.TotalFacebookEntry = totalfreeEntryPoint;

                    bundleModel.TotalPaidConversation = totalUtility + totalService + totalMarketing;
                    bundleModel.TotalMarketingConversation = totalMarketing;
                    bundleModel.TotalUtilityConversation = totalUtility;
                    bundleModel.TotalServicesConversation = totalService;

                    bundleModel.TotalCharge = totalUtilityCharge + totalServiceCharge + totalMarketingCharge;
                    bundleModel.TotalMarketingCharge = totalMarketingCharge;
                    bundleModel.TotalUtilityCharge = totalUtilityCharge;
                    bundleModel.TotalServicesCharge = totalServiceCharge;

                    bundleModel.TotalConversation = bundleModel.TotalFreeConversation + bundleModel.TotalPaidConversation;
                }
                return bundleModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private async Task<WhatsAppBundleModel> getBundelAsync(DateTime start, DateTime end, TenantModelDash tenant)
        {

            if (tenant == null)
                return null;
            //start = new DateTime(2023, 09, 1, 0, 0, 0, DateTimeKind.Utc); // October 1, 2023, 12:00:00 AM UTC
            //end = new DateTime(2023, 09, 30, 23, 59, 59, DateTimeKind.Utc); // October 31, 2023, 11:59:59 PM UTC
          
            int startTime, endTime = 0;
            startTime = (int)ConvertDatetimeToUnixTimeStamp(start);
            endTime = (int)ConvertDatetimeToUnixTimeStamp(end);

            using (var httpClient = new HttpClient())
            {
                string[] obj = { "CONVERSATION_DIRECTION", "CONVERSATION_CATEGORY", "CONVERSATION_TYPE", "COUNTRY", "PHONE" };

                var requestUrl = Constants.WhatsAppTemplates.WhatsAppApiUrlNew + tenant.WhatsAppAccountID +
                    "?fields=conversation_analytics.start(" + startTime + ").end(" + endTime + ").granularity(DAILY).phone_numbers([]).dimensions(" +
                    obj[0] + "," + obj[1] + "," + obj[2] + "," + obj[3] + "," + obj[4] + ")&access_token=" + tenant.AccessToken;

                try
                {
                    var response = await httpClient.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var objWhatsApp = JsonConvert.DeserializeObject<WhatsAppBundleModel>(responseContent);

                    // You can work with objWhatsAppAnalytic here
                    if (objWhatsApp.conversation_analytics != null)
                    {
                        return objWhatsApp;
                    }
                    return objWhatsApp;
                }
                catch (HttpRequestException ex)
                {
                    // Handle the HTTP request exception
                    //Console.WriteLine("HTTP Request Exception: " + ex.Message);
                    return new WhatsAppBundleModel();
                }
                catch (JsonException ex)
                {
                    // Handle JSON deserialization exception
                    //Console.WriteLine("JSON Deserialization Exception: " + ex.Message);
                    return new WhatsAppBundleModel();
                }
            }
        }
        private TenantModelDash getTenantById(int tenantId)
        {
            try
            {
                var functionName = "dbo.tenants_by_id_get"; // your new PostgreSQL function name

                // Define PostgreSQL parameters
                var npgsqlParameters = new Npgsql.NpgsqlParameter[]
                {
                    new Npgsql.NpgsqlParameter("p_id", tenantId),
                    new Npgsql.NpgsqlParameter("p_contact_id", DBNull.Value) // pass contactId if needed, else NULL
                };

                // Execute function and map result
                var tenant = PostgresDataHelper.ExecuteFunction(
                    functionName,
                    npgsqlParameters,
                    DataReaderMapper.MapTenantForDash, 
                    _postgresConnection
                ).FirstOrDefault();

                return tenant ?? new TenantModelDash();
            }
            catch (Exception ex)
            {
                // Better: log exception instead of rethrowing directly
                throw new Exception("Error fetching tenant data", ex);
            }
        }

        private long walletDeposit(WalletModel model, decimal DepositAmount)
        {
            try
            {
                if (model.TotalAmount < 0)
                {
                    return 0;
                }

                // PostgreSQL function name
                var functionName = "dbo.wallet_deposit"; // should match "dbo.wallet_deposit"

                // Prepare parameters for the function
                var npgsqlParams = new Npgsql.NpgsqlParameter[]
                {
                    new Npgsql.NpgsqlParameter("p_walletid", model.WalletId),
                    new Npgsql.NpgsqlParameter("p_tenantid", model.TenantId),
                    new Npgsql.NpgsqlParameter("p_totalamount", model.TotalAmount),
                    new Npgsql.NpgsqlParameter("p_depositamount", DepositAmount),
                    new Npgsql.NpgsqlParameter("p_depositdate", DateTime.UtcNow)
                };

                // Execute the PostgreSQL function and get the WalletId (DepositId)
                long depositId = PostgresDataHelper.ExecuteScalarFunction<long>(
                    functionName,
                    npgsqlParams,
                    _postgresConnection // using your PostgreSQL connection string
                );

                // Check the returned value
                if (depositId == model.WalletId)
                {
                    return depositId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private WalletModel walletGetByTenantId(int TenantId)
        {
            try
            {
                WalletModel walletModel = new WalletModel();

                var functionName = "dbo.wallet_get";

                // PostgreSQL parameters
                var npgsqlParams = new Npgsql.NpgsqlParameter[]
                {
                   new Npgsql.NpgsqlParameter("p_tenantid", TenantId)
                };

                // Execute the PostgreSQL function
                walletModel = PostgresDataHelper.ExecuteFunction(
                    functionName,
                    npgsqlParams,
                    DataReaderMapper.MapWallet,
                    _postgresConnection // Use your PostgreSQL connection string
                ).FirstOrDefault();

                if (walletModel != null)
                {
                    walletModel.TotalAmountSAR = (walletModel.TotalAmount > 0)
                        ? Math.Round(walletModel.TotalAmount * 3.75m, 3)
                        : 0;
                }
                else
                {
                    // Handle the case where no wallet data is returned
                    walletModel = new WalletModel();
                }

                return walletModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private long addTransaction(TransactionModel model)
        {
            try
            {
                long outId = 0;

                // Use your PostgreSQL connection string
                using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
                {
                    conn.Open();

                    using (var cmd = new Npgsql.NpgsqlCommand("SELECT dbo.transaction_add(@DoneBy, @TotalTransaction, @TransactionDate, @CategoryType, @TotalRemaining, @WalletId, @Country, @TenantId, @InvoiceId, @InvoiceUrl, @IsPayed, @Note);", conn))
                    {
                        cmd.Parameters.AddWithValue("@DoneBy", NpgsqlTypes.NpgsqlDbType.Text, model.DoneBy ?? "");
                        cmd.Parameters.AddWithValue("@TotalTransaction", NpgsqlTypes.NpgsqlDbType.Numeric, model.TotalTransaction);
                        cmd.Parameters.AddWithValue("@TransactionDate", NpgsqlTypes.NpgsqlDbType.Timestamp, model.TransactionDate);
                        cmd.Parameters.AddWithValue("@CategoryType", NpgsqlTypes.NpgsqlDbType.Text, model.CategoryType ?? "");
                        cmd.Parameters.AddWithValue("@TotalRemaining", NpgsqlTypes.NpgsqlDbType.Numeric, model.TotalRemaining);
                        cmd.Parameters.AddWithValue("@WalletId", NpgsqlTypes.NpgsqlDbType.Bigint, model.WalletId);
                        cmd.Parameters.AddWithValue("@Country", NpgsqlTypes.NpgsqlDbType.Text, model.Country ?? "");
                        cmd.Parameters.AddWithValue("@TenantId", NpgsqlTypes.NpgsqlDbType.Integer, model.TenantId);
                        cmd.Parameters.AddWithValue("@InvoiceId", NpgsqlTypes.NpgsqlDbType.Text, model.invoiceId ?? "");
                        cmd.Parameters.AddWithValue("@InvoiceUrl", NpgsqlTypes.NpgsqlDbType.Text, model.invoiceUrl ?? "");
                        cmd.Parameters.AddWithValue("@IsPayed", NpgsqlTypes.NpgsqlDbType.Boolean, model.IsPayed);
                        cmd.Parameters.AddWithValue("@Note", NpgsqlTypes.NpgsqlDbType.Text, model.Note ?? "");

                        // Execute function and get returned transaction ID
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            outId = Convert.ToInt64(result);
                        }
                    }

                    conn.Close();
                }

                return outId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private CountryCodeModel GetCountryCode(string PhoneNumber)
        {
            // not used
            try
            {
                string phoneNumberWithCountryCode = "+" + PhoneNumber;

                // Create a PhoneNumberUtil instance
                PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

                // Specify the default region (e.g., "US" for the United States) based on your requirements
                string defaultRegion = "US"; // Adjust as needed

                // Parse the phone number with the specified default region
                PhoneNumber parsedNumber = phoneNumberUtil.Parse(phoneNumberWithCountryCode, defaultRegion);

                // Extract the country code
                int Code = parsedNumber.CountryCode;

                string CountryCode = Code.ToString();

                CountryCodeModel countryCodeModel = new CountryCodeModel();
                int TenantId = AbpSession.TenantId.Value;
                var SP_Name = Constants.Country.SP_CountryCodeGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CountryCode",CountryCode)
                };

                countryCodeModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCountryCode, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return countryCodeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UsersDashModel GetUserInfo(long UserId)
        {
            try
            {
                return getUserInfo(UserId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private UsersDashModel getUserInfo(long UserId)
        {
            try
            {
                UsersDashModel usersDashModel = new UsersDashModel();

                var SP_Name = Constants.User.SP_UsersGetInfo;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@UserId",UserId)
                };

                usersDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapUserInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return usersDashModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<TransactionModel> transactionGetLastFour(int TenantId)
        {
            try
            {
                List<TransactionModel> transactionModel = new List<TransactionModel>();

                // PostgreSQL function name
                var functionName = "dbo.transaction_get_last_four";

                using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
                {
                    conn.Open();
                    using (var cmd = new Npgsql.NpgsqlCommand(functionName, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add PostgreSQL parameter
                        cmd.Parameters.AddWithValue("p_tenantid", NpgsqlTypes.NpgsqlDbType.Integer, TenantId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactionModel.Add(DataReaderMapper.MapTransactionInfo(reader));
                            }
                        }
                    }
                }

                return transactionModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<CountryCodeModel> countryGetAll(int TenantId)
        {
            try
            {
                List<CountryCodeModel> countryCodeModel = new List<CountryCodeModel>();

                if (TenantId > 0)
                {
                    // PostgreSQL function name
                    string functionName = "dbo.country_get_all";

                    // PostgreSQL function has no parameters
                    var npgsqlParameters = new Npgsql.NpgsqlParameter[] { };

                    // Execute the PostgreSQL function using your helper
                    countryCodeModel = PostgresDataHelper.ExecuteFunction(
                        functionName,
                        npgsqlParameters,
                        DataReaderMapper.MapCountry,
                        _postgresConnection
                    ).ToList();

                    return countryCodeModel;
                }

                return countryCodeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ConversationPriceModel getConvarsationPrice(ConversationPriceModel model, int TenantId)
        {
            try
            {
                ConversationPriceModel conversationPriceModel = new ConversationPriceModel();
                
                conversationPriceModel.TotalDeposit = model.TotalDeposit;
                conversationPriceModel.Country = model.Country;
                conversationPriceModel.TotalMarketingCount = (int)(model.TotalDeposit / 0.014);
                conversationPriceModel.TotalUtilityCount = (int)(model.TotalDeposit / 0.014);
                conversationPriceModel.TotalServicesCount = 0;

                //CountryCodeModel countryCodeModel = new CountryCodeModel();
                //countryCodeModel = countryGetAll(TenantId).Where(x => x.Country == model.Country).FirstOrDefault();
                //if (countryCodeModel != null)
                //{
                    
                //        //(int)(model.TotalDeposit / countryCodeModel.ServicePrice);
                //}
                return conversationPriceModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<UsersDashModel> getAllUser(int TenantId)
        {
            try
            {
                List<UsersDashModel> usersDashModel = new List<UsersDashModel>();

                if (TenantId > 0)
                {
                    // PostgreSQL function name as string
                    string functionName = "dbo.users_get_all";

                    // Prepare parameters for Postgres as array
                    var npgsqlParameters = new Npgsql.NpgsqlParameter[]
                    {
                new Npgsql.NpgsqlParameter("p_tenantid", TenantId)
                    };

                    // Call Postgres function using helper
                    usersDashModel = PostgresDataHelper
                                        .ExecuteFunction(functionName, npgsqlParameters, DataReaderMapper.MapUserInfo, _postgresConnection)
                                        .ToList();
                }

                return usersDashModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //User Performance (Order)
        private OrderDashbordModel ordersGetAll(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                OrderDashbordModel orderDashModel = new OrderDashbordModel();
                //int TenantId = AbpSession.TenantId.Value;

                var SP_Name = Constants.Order.SP_OrdersGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Start",start.AddHours(AppSettingsModel.AddHour))
                    ,new System.Data.SqlClient.SqlParameter("@End",end.AddHours(AppSettingsModel.AddHour))

                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalOrders",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                orderDashModel.orderDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetOrderAllDash, AppSettingsModel.ConnectionStrings).ToList();
                orderDashModel.TotalOrdrs = (long)OutputParameter.Value;

                return orderDashModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private UserPerformanceOrderGenarecModel getPerformanceOrder(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                DateTime StartDate = start;
                DateTime EndDate = end;


                var user = GetAllUser(TenantId);
                var orderDashModel = OrdersGetAll(StartDate, EndDate, TenantId);

                UserPerformanceOrderGenarecModel model = new UserPerformanceOrderGenarecModel();
                var TotalOrders = orderDashModel.TotalOrdrs;
                model.totalOrders = TotalOrders;

                List<UserPerformanceOrderModel> userPerformanceOrderModels = new List<UserPerformanceOrderModel>();

                foreach (var us in user)
                {
                    var ls = orderDashModel.orderDashModel.Where(x => x.AgentId == us.Id).ToList();

                    if (ls.Count > 0 && ls != null)
                    {
                        decimal CountDone = 0;
                        decimal CountDelete = 0;
                        //long totalOrders = 0;
                        var totalmen = 0;
                        int hours = 0;
                        int remainingMinutes = 0;
                        string Avg_ActionTime = "";
                        try
                        {
                            var donemode = ls.Where(x => x.OrderStatus == 2).FirstOrDefault();
                            //totalOrders = donemode.totalOrders;

                           
                            if (donemode != null && TotalOrders != 0)
                            {
                                CountDone = donemode.Count_OrderStatus_2;
                                CountDone = Math.Round(((CountDone / TotalOrders) * 100), 2);

                                totalmen = totalmen + donemode.Avg_ActionTime_Minutes / orderDashModel.orderDashModel.Count;

                                hours = totalmen / 60; // Calculate the hours by dividing totalMinutes by 60
                                remainingMinutes = totalmen % 60;

                                if (hours > 0 && remainingMinutes > 0)
                                { Avg_ActionTime = $"{hours} hours {remainingMinutes} minutes"; }
                                else if (hours > 0 && remainingMinutes == 0)
                                { Avg_ActionTime = $"{hours} hours"; }
                                else
                                { Avg_ActionTime = $"{remainingMinutes} minutes"; }
                            }

                        }
                        catch { }
                        try
                        {
                            var deletemode = ls.Where(x => x.OrderStatus == 3).FirstOrDefault();
                            //totalOrders = deletemode.totalOrders;
                            if (deletemode != null && TotalOrders != 0)
                            { 
                                CountDelete = deletemode.Count_OrderStatus_3;
                                CountDelete = Math.Round(((CountDelete / TotalOrders) * 100), 2);

                                totalmen = totalmen + deletemode.Avg_ActionTime_Minutes;

                                hours = hours + (totalmen / 60); // Calculate the hours by dividing totalMinutes by 60
                                remainingMinutes = remainingMinutes + (totalmen % 60);
                                if (hours > 0 && remainingMinutes > 0)
                                { Avg_ActionTime = $"{hours} hours {remainingMinutes} minutes"; }
                                else if (hours > 0 && remainingMinutes == 0)
                                { Avg_ActionTime = $"{hours} hours"; }
                                else
                                { Avg_ActionTime = $"{remainingMinutes} minutes"; }
                            }
                        }
                        catch { }

                        userPerformanceOrderModels.Add(new UserPerformanceOrderModel
                        {
                            Id = ls.FirstOrDefault().Id,
                            UserName = ls.FirstOrDefault().UserName,
                            EmailAddress = ls.FirstOrDefault().EmailAddress,
                            CountDone = CountDone,
                            CountDelete = CountDelete,
                            Avg_ActionTime_Minutes = totalmen,
                            Avg_ActionTime = Avg_ActionTime
                        });

                    }
                    else
                    {
                        userPerformanceOrderModels.Add(new UserPerformanceOrderModel
                        {
                            Id = us.Id,
                            UserName = us.UserName,
                            EmailAddress = us.EmailAddress,
                            CountDone = 0,
                            CountDelete = 0,
                            Avg_ActionTime_Minutes = 0,
                            Avg_ActionTime = "0 minutes"
                        });
                    }
                }
                model.userPerformanceOrderModel = userPerformanceOrderModels;
                return model;
                //List<UsersDashModel> usersDashModels = getAllUser();
                //List<OrderDashModel> orderDashModel = ordersGetAll(StartDate, EndDate);
                //List<UserPerformanceOrderModel> Model = usersDashModels
                //.Zip(orderDashModel, (user, order) => new { User = user, Order = order })
                //.Where(pair => pair.Order.AgentId == pair.User.Id)
                //.Select(pair => new UserPerformanceOrderModel
                //{
                //    usersDashModel = pair.User,
                //    urderDashModel = pair.Order
                //})
                //.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //User Performance (tickits)
        private TickitsDashbordModel tickitsGetAll(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                TickitsDashbordModel tickitsDashbordModel = new TickitsDashbordModel();
                //int TenantId = AbpSession.TenantId.Value;

                string functionName = "dbo.tickets_get_all"; // PostgreSQL function name as string

                // Create NpgsqlParameter array
                Npgsql.NpgsqlParameter[] npgsqlParameters = new Npgsql.NpgsqlParameter[]
                {
            new Npgsql.NpgsqlParameter("p_tenantid", TenantId),
            new Npgsql.NpgsqlParameter("p_start", start),
            new Npgsql.NpgsqlParameter("p_end", end)
                };

                // Execute function
                tickitsDashbordModel.tickitDashModel = PostgresDataHelper.ExecuteFunction(
                    functionName,
                    npgsqlParameters,
                    DataReaderMapper.MapGetTicketsAllDashPSQL,
                    _postgresConnection
                ).ToList();

                // Compute total tickets
                tickitsDashbordModel.TotalTickits = tickitsDashbordModel.tickitDashModel != null
                ? tickitsDashbordModel.tickitDashModel.Sum(x => x.TotalOpen + x.TotalClose + x.TotalPending)
                   : 0;

                return tickitsDashbordModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private UserPerformanceTicketGenarecModel getPerformanceTeckits(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                DateTime StartDate = start;
                DateTime EndDate = end;

                var user = GetAllUser(TenantId);
                var TickitsDashModel = TickitsGetAll(StartDate, EndDate, TenantId);

                UserPerformanceTicketGenarecModel model = new UserPerformanceTicketGenarecModel();
                var TotalTickets = TickitsDashModel.TotalTickits;
                model.totalTickets = TotalTickets;

                List<UserPerformanceTicketModel> userPerformanceTicketModel = new List<UserPerformanceTicketModel>();
                if (user != null)
                {
                    foreach (var us in user)
                    {
                        var ls = TickitsDashModel.tickitDashModel.Where(x => x.AgentId == us.Id).ToList();

                        if (ls.Count > 0 && ls != null)
                        {
                            decimal CountOpen = 0;
                            decimal CountComplet = 0;
                            decimal CountPending = 0;
                            var totalmen = 0;
                            int hours = 0;
                            int remainingMinutes = 0;
                            string Avg_ActionTime = "";
                            try
                            {
                                var mode = ls.FirstOrDefault();
                                //totalOrders = donemode.totalOrders;
                                if (mode != null && TotalTickets != 0)
                                {
                                    CountOpen = mode.TotalOpen;
                                   // CountOpen = Math.Round(((CountOpen / TotalTickets) * 100), 2);

                                    CountComplet = mode.TotalClose;
                                    //  CountComplet = Math.Round(((CountComplet / TotalTickets) * 100), 2);

                                    CountPending = mode.TotalPending;
                                    totalmen = (int)(mode.AvgTimeMinutes);

                                    hours = totalmen / 60; // Calculate the hours by dividing totalMinutes by 60
                                    remainingMinutes = totalmen % 60;
                                    if (hours > 0 && remainingMinutes > 0)
                                    { Avg_ActionTime = $"{hours} hours {remainingMinutes} minutes"; }
                                    else if (hours > 0 && remainingMinutes == 0)
                                    { Avg_ActionTime = $"{hours} hours"; }
                                    else
                                    { Avg_ActionTime = $"{remainingMinutes} minutes"; }
                                }
                            }
                            catch { }

                            userPerformanceTicketModel.Add(new UserPerformanceTicketModel
                            {
                                AgentId = us.Id,
                                UserName = us.UserName,
                                EmailAddress = us.EmailAddress,
                                TotalOpen = CountOpen,
                                TotalClose = CountComplet,
                                TotalPending = CountPending,
                                AvgTimeMinutes = totalmen,
                                Avg_ActionTime = Avg_ActionTime
                            });

                        }
                        else
                        {
                            userPerformanceTicketModel.Add(new UserPerformanceTicketModel
                            {
                                AgentId = us.Id,
                                UserName = us.UserName,
                                EmailAddress = us.EmailAddress,
                                TotalOpen = 0,
                                TotalClose = 0,
                                TotalPending = 0,
                                AvgTimeMinutes = 0,
                                Avg_ActionTime = "0 minutes"
                            });
                        }
                    }
                }
                else
                {
                    return model;
                }
                model.userPerformanceTicketModel = userPerformanceTicketModel;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private BookingDashbordModel bookingGetAll(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                var model = new BookingDashbordModel();
                string functionName = "dbo.booking_get_all"; // PostgreSQL function name

                // PostgreSQL function parameters
                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_tenantid", TenantId),
                    new NpgsqlParameter("p_start", start),
                    new NpgsqlParameter("p_end", end)
                };

                // Execute function
                var rows = PostgresDataHelper.ExecuteFunction(
                    functionName,
                    parameters,
                    DataReaderMapper.MapGetBookingAllDashPSQL,
                    _postgresConnection
                ).ToList();

                model.bookingDashModel = rows;

                // Compute TotalBooking exactly like SQL Server:
                // Sum of all rows (TotalBooked + TotalConfirmed + TotalCancelled + TotalDeleted + TotalPending)
                model.TotalBooking = rows.Sum(r => r.TotalBooked + r.TotalConfirmed + r.TotalCancelled + r.TotalDeleted + r.TotalPending);

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private UserPerformanceBookingGenarecModel getPerformanceBooking(DateTime start, DateTime end,int TenantId)
        {
            try
            {
                DateTime StartDate = start;
                DateTime EndDate = end;

                var user = GetAllUser(TenantId);
                var BookingDashModel = BookingGetAll(StartDate, EndDate, TenantId);

                UserPerformanceBookingGenarecModel model = new UserPerformanceBookingGenarecModel();
                var totalBooking = BookingDashModel.TotalBooking;
                model.TotalBooking = totalBooking;

                List<UserPerformanceBookingModel> userPerformanceBookingModel = new List<UserPerformanceBookingModel>();

                foreach (var us in user)
                {
                    var ls = BookingDashModel.bookingDashModel.Where(x => x.UserId == us.Id).ToList();

                    if (ls.Count > 0 && ls != null)
                    {
                        decimal TotalBooked = 0;
                        decimal TotalConfirmed = 0;
                        decimal TotalCancelled = 0;
                        decimal TotalDeleted = 0;
                        decimal TotalPending = 0;
                        
                        try
                        {
                            var mode = ls.FirstOrDefault();
                            if (mode != null && totalBooking != 0)
                            {
                                TotalBooked = mode.TotalBooked;
                                TotalBooked = Math.Round(((TotalBooked / totalBooking) * 100), 2);

                                TotalConfirmed = mode.TotalConfirmed;
                                TotalConfirmed = Math.Round(((TotalConfirmed / totalBooking) * 100), 2);

                                TotalCancelled = mode.TotalCancelled;
                                TotalCancelled = Math.Round(((TotalCancelled / totalBooking) * 100), 2);

                                TotalDeleted = mode.TotalDeleted;
                                TotalDeleted = Math.Round(((TotalDeleted / totalBooking) * 100), 2);

                                TotalPending = mode.TotalPending;
                                TotalPending = Math.Round(((TotalPending / totalBooking) * 100), 2);
                            }
                        }
                        catch { }

                        userPerformanceBookingModel.Add(new UserPerformanceBookingModel
                        {
                            AgentId = us.Id,
                            UserName = us.UserName,
                            EmailAddress = us.EmailAddress,
                            TotalBooked = TotalBooked ,
                            TotalConfirmed = TotalConfirmed ,
                            TotalCancelled = TotalCancelled ,
                            TotalDeleted = TotalDeleted ,
                            TotalPending = TotalPending 
                        });

                    }
                    else
                    {
                        userPerformanceBookingModel.Add(new UserPerformanceBookingModel
                        {
                            AgentId = us.Id,
                            UserName = us.UserName,
                            EmailAddress = us.EmailAddress,
                            TotalBooked = 0,
                            TotalConfirmed = 0,
                            TotalCancelled = 0,
                            TotalDeleted = 0,
                            TotalPending = 0
                        });
                    }
                }
                model.userPerformanceBookingModel = userPerformanceBookingModel;
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private OrderStatisticsModel ordersStatisticsGet(DateTime start, DateTime end, int TenantId, long BranchId = 0)
        {
            try
            {
                OrderStatisticsModel model = new OrderStatisticsModel();

                // Function name as variable (similar to SP_Name in SQL Server)
                var functionName = "dbo.orders_statistics_get";

                using (var conn = new Npgsql.NpgsqlConnection(_postgresConnection))
                using (var cmd = new Npgsql.NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.Text;

                    // Pass the function name dynamically
                    cmd.CommandText = $"SELECT * FROM {functionName}(@p_tenantid, @p_start, @p_end, @p_branchid);";

                    cmd.Parameters.AddWithValue("@p_tenantid", TenantId);
                    cmd.Parameters.AddWithValue("@p_start", start.AddHours(AppSettingsModel.AddHour));
                    cmd.Parameters.AddWithValue("@p_end", end.AddHours(AppSettingsModel.AddHour));
                    cmd.Parameters.AddWithValue("@p_branchid", BranchId);

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = DataReaderMapper.MapOrderGetStatistics(reader);
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// get all booking Statistics Number in same tenant used in dashboard
        /// </summary>
        /// <param name="start">start date </param>
        /// <param name="end">end date </param>
        /// <param name="TenantId">tenant id</param>
        /// <param name="UserId">User id for filtering</param>
        /// <returns></returns>
        private BookingStatisticsModel bookingStatisticsGet(DateTime start, DateTime end, int TenantId, long UserId = 0)
        {
            try
            {
                BookingStatisticsModel model = new BookingStatisticsModel();

                string functionName = "dbo.booking_statistics_get"; // PostgreSQL function name

                // Create NpgsqlParameter array
                NpgsqlParameter[] npgsqlParameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("p_tenantid", TenantId),
                    new NpgsqlParameter("p_start", start),
                    new NpgsqlParameter("p_end", end),
                    new NpgsqlParameter("p_userid", UserId)
                };

                // Execute the function using PostgresDataHelper
                model = PostgresDataHelper.ExecuteFunction<BookingStatisticsModel>(
                     functionName,
                     npgsqlParameters,
                     new Converter<IDataReader, BookingStatisticsModel>(DataReaderMapper.MapBookingGetStatisticsPSQL),
                     _postgresConnection
                 ).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// get all tickets Statistics Number in same tenant used in dashboard
        /// </summary>
        /// <param name="start">start date </param>
        /// <param name="end">end date </param>
        /// <param name="TenantId">tenant id</param>
        /// <returns></returns>
        private TicketsStatisticsModel ticketsStatisticsGet(DateTime start, DateTime end , int TenantId)
        {
            try
            {
                TicketsStatisticsModel model = new TicketsStatisticsModel();
                //int TenantId = AbpSession.TenantId.Value;

                var SP_Name = Constants.LiveChat.SP_TicketsStatisticsGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Start",start)
                    ,new System.Data.SqlClient.SqlParameter("@End",end)

                };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTicketsGetStatistics, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get all contact statistics in the same tenant (PostgreSQL version)
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="TenantId">Tenant ID</param>
        /// <returns>ContactStatisticsModel</returns>
        private ContactStatisticsModel contactStatisticsGet(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                ContactStatisticsModel model = new ContactStatisticsModel();

                // PostgreSQL function name
                var functionName = "dbo.contacts_statistics_get";

                // Map SQL Server parameters to Npgsql parameters
                var sqlParameters = new Npgsql.NpgsqlParameter[]
                {
                    new Npgsql.NpgsqlParameter("p_tenantid", TenantId),
                    new Npgsql.NpgsqlParameter("p_start", start),
                    new Npgsql.NpgsqlParameter("p_end", end)
                };

                // Use PostgresDataHelper to execute the function
                string connectionString = _postgresConnection; // Make sure this is your PostgreSQL connection string

                model = PostgresDataHelper.ExecuteFunction(functionName, sqlParameters, DataReaderMapper.MapContactGetStatisticsPSQL, connectionString)
                                           .FirstOrDefault();

                return model;
            }
            catch (Exception)
            {
                throw; // Keep stack trace intact
            }
        }
        /// <summary>
        /// get all campaign Statistics Number in same tenant used in dashboard
        /// </summary>
        /// <param name="start">start date </param>
        /// <param name="end">end date </param>
        /// <param name="TenantId">tenant id</param>
        /// <param name="CampaignId">campain id for filtering</param>
        /// <returns></returns>
        private async Task<CampaignStatisticsModel> campaignStatisticsGet(DateTime start, DateTime end, int TenantId, long CampaignId = 0)
        {
            try
            {
                CampaignStatisticsModel campaignStatisticsModel = new CampaignStatisticsModel();



                List<CampaginMongoModel> model = new List<CampaginMongoModel>();
                try
                {
                    // Connection string from Azure Cosmos DB for MongoDB (vCore)
                    string connectionString = AppSettingsModel.connectionStringMongoDB;

                    // MongoDB database and collection details

                    string databaseName = AppSettingsModel.databaseName;
                    string collectionName = AppSettingsModel.collectionName;


                    // Initialize the MongoDB client
                    var client = new MongoClient(connectionString);

                    // Get the database
                    var database = client.GetDatabase(databaseName);

                    // Get the collection
                    var collection = database.GetCollection<CampaginMongoModel>(collectionName);


                    if (CampaignId!=0)
                    {


                        // Build the filter
                        var filter = Builders<CampaginMongoModel>.Filter.Eq("campaignId", CampaignId);
                        try
                        {
                            // Find the first matching document
                            var filterResult = await collection.Find(filter).ToListAsync();

                            model=filterResult;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }

                        try
                        {
                            campaignStatisticsModel.TotalRead+=model.Where(x => x.is_read).ToList().Count();
                            campaignStatisticsModel.TotalDelivered+=model.Where(x => x.is_delivered).ToList().Count();
                            campaignStatisticsModel.TotalSent+=model.Where(x => x.is_sent).ToList().Count();
                            campaignStatisticsModel.TotalFailed+=model.Where(x => !x.is_read &&!x.is_delivered &&!x.is_sent).ToList().Count();
                            campaignStatisticsModel.TotalReplied=0;
                            campaignStatisticsModel.TotalContact +=model.Where(x => x.is_accepted).ToList().Count();



                            //a.sendTime >= start && a.sendTime <= end)

                        }
                        catch
                        {

                        }

                    }
                    else
                    {
                        var campList = GetCampaignFun(TenantId, start, end);

                        foreach (var cp in campList)
                        {



                            // Build the filter
                            var filter = Builders<CampaginMongoModel>.Filter.Eq("campaignId", cp.campaignId);

                            try
                            {
                                // Find the first matching document
                                var filterResult = await collection.Find(filter).ToListAsync();

                                model=filterResult;

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }



                            try
                            {
                                campaignStatisticsModel.TotalRead+=model.Where(x => x.is_read).ToList().Count();
                                campaignStatisticsModel.TotalDelivered+=model.Where(x => x.is_delivered).ToList().Count();
                                campaignStatisticsModel.TotalSent+=model.Where(x => x.is_sent).ToList().Count();
                                campaignStatisticsModel.TotalFailed+=model.Where(x => !x.is_read &&!x.is_delivered &&!x.is_sent).ToList().Count();
                                campaignStatisticsModel.TotalReplied=0;
                                campaignStatisticsModel.TotalContact +=model.Where(x => x.is_accepted).ToList().Count();



                                //a.sendTime >= start && a.sendTime <= end)

                            }
                            catch
                            {

                            }



                        }


                    }

             







                }
                catch (Exception ex)
                {

                }


                return campaignStatisticsModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<CampaginModel> GetCampaignFun(long TenantId, DateTime start ,DateTime end)
        {
            try
            {
                var SP_Name = "GetSendCampaignNowByTenantId";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId", TenantId),
                    new System.Data.SqlClient.SqlParameter("@StartDate", start),
                    new System.Data.SqlClient.SqlParameter("@EndDate", end)
                };
                List<CampaginModel> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<CampaginModel>();
            }
        }
        private static CampaginModel MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                CampaginModel model = new CampaginModel
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),

                };

                try
                {

                    model.model=System.Text.Json.JsonSerializer.Deserialize<MessageTemplateModel>(SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"));
                }
                catch
                {


                }
                try
                {

                    model.templateVariablles=System.Text.Json.JsonSerializer.Deserialize<TemplateVariablles>(SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"));
                }
                catch
                {
                    model.templateVariablles=new TemplateVariablles();

                }

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new CampaginModel();
            }
        }



        /// <summary>
        /// get all branchs name in same tenant used for filter the Order statistic in dashboard
        /// </summary>
        /// <param name="TenantId">Tenant ID</param>
        /// <returns></returns>
        private List<BranchsModel> branchsGetAll(int TenantId)
        {
            try
            {
                List<BranchsModel> Model = new List<BranchsModel>();
                //int TenantId = AbpSession.TenantId.Value;
                var SP_Name = Constants.Area.SP_BranchsGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                Model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBranchGeeAll, AppSettingsModel.ConnectionStrings).ToList();

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// get all campaign name in same tenant used for filter the campaign statistic in dashboard
        /// </summary>
        /// <param name="TenantId"></param>
        /// <returns></returns>
        private List<CampaignDashModel> getAllCampaign(int TenantId)
        {
            try
            {
                List<CampaignDashModel> Model = new List<CampaignDashModel>();
                var SP_Name = Constants.WhatsAppCampaign.SP_CampaignGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                Model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCampaignGetAll, AppSettingsModel.ConnectionStrings).ToList();

                return Model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BestSellingModel> getBestSellingItems(DateTime start, DateTime end, int TenantId)
        {
            try
            {
                List<BestSellingModel> model = new List<BestSellingModel>();
                var SP_Name = Constants.Dashboard.SP_BestSellingGetTen;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Start",start.AddHours(AppSettingsModel.AddHour))
                    ,new System.Data.SqlClient.SqlParameter("@End",end.AddHours(AppSettingsModel.AddHour))
                };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBestSellingItems, AppSettingsModel.ConnectionStrings).ToList();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private TransactionModel GetTransaction(int TenantId, string invoiceId)
        {
            try
            {
                TransactionModel transactionModel = new TransactionModel();
                var SP_Name = Constants.Transaction.SP_TransactionGetByInvoceId;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@invoice_id",invoiceId)
                };
                transactionModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTransactionInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return transactionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdeteTransaction(long WalletId, string invoiceId)
        {
            try
            {
                bool tr = true;
                var SP_Name = Constants.Transaction.SP_UpdeteTransaction;
               
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@WalletId",WalletId)
                    ,new System.Data.SqlClient.SqlParameter("@invoice_id",invoiceId)
                    ,new System.Data.SqlClient.SqlParameter("@IsPayed",tr)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@out",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private UsageDetailsGenericModel getUsageDetails(int TenantId, long? CampaignId = 0, string GroupBy = "", int? pageNumber = 0, int? pageSize = 10, DateTime? start = null, DateTime? end = null)
        {
            try
            {
                UsageDetailsGenericModel model = new UsageDetailsGenericModel();
                var SP_Name = Constants.Dashboard.UsageDetailsGet;

                //pageNumber = pageNumber * pageSize;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber)
                    ,new System.Data.SqlClient.SqlParameter("@pageSize",pageSize)
                    ,new System.Data.SqlClient.SqlParameter("@tenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Start",start)
                    ,new System.Data.SqlClient.SqlParameter("@End",end)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                    ,new System.Data.SqlClient.SqlParameter("@GroupBy",GroupBy)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                model.usageDetails = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetUsageDetails, AppSettingsModel.ConnectionStrings).ToList();
                model.Total = (long)OutputParameter.Value;
                if (model.usageDetails != null)
                {                   
                    foreach (var Useg in model.usageDetails)
                    {
                        List<string> countryArray = new List<string>();
                        Dictionary<string, int> countryCounts = new Dictionary<string, int>();
                        if (Useg.Country != null && Useg.Country != "-" && Useg.Country != "" && Useg.Country != "--" && Useg.Country != "----" && Useg.CategoryType != "SuccessSentCampaign")
                        {
                            countryArray = Useg.Country?.Split(',')?.ToList() ?? new List<string>();
                        }
                        else
                        { 
                            Useg.Country = "-";
                            if(Useg.CategoryType == "SuccessSentCampaign")
                            {
                                Useg.CategoryType = "Success Sent Campaign";
                            }
                        }
                        if (Useg.CategoryType == "Deposit")
                        {
                            Useg.Country = "-";
                        }
                        if (countryArray.Count > 0)
                        {
                            foreach (var country in countryArray)
                            {
                                if (country != "")
                                {
                                    if (countryCounts.ContainsKey(country))
                                    {
                                        countryCounts[country]++;
                                    }
                                    else
                                    {
                                        countryCounts[country] = 1;
                                    }
                                }
                            }
                            Useg.Country = "";
                            foreach (var kvp in countryCounts)
                            {
                                if (countryCounts.Count == 1)
                                { Useg.Country += kvp.Key + " " + kvp.Value; }
                                else
                                { Useg.Country += kvp.Key + " " + kvp.Value + ","; }
                            }
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private UsageStatisticsModel getUsageStatistics(int TenantId, long CampaignId)
        {
            try
            {
                UsageStatisticsModel model = new UsageStatisticsModel();
                var SP_Name = Constants.Dashboard.UsageStatisticsGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                };

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetUsageStatistics, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getZohoCustomerIds(int TenantId)
        {
            try
            {
                var SP_Name = Constants.Tenant.ZohoCustomerIdsGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",TenantId)
                };
                string ZohoCustomerId;
                ZohoCustomerId = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTenantForZohoCustomerId, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return ZohoCustomerId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


    }
}

using Azure.Core;
using InfoSeedAzureFunction.AppFunEntities;
using Microsoft.Azure.WebJobs;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebJobEntities;
using static InfoSeedAzureFunction.Model.WhatsAppEnums;

namespace InfoSeedAzureFunction
{
    public static class TenantStatisticsSyncFunction
    {
        //[FunctionName("TenantStatisticsSyncFunction")]
        //public static void Run([TimerTrigger("0 0 1 * * * ", RunOnStartup = false)] TimerInfo myTimer)//On the 1st day of the month.
        //{
        //    //log.LogInformation($"C# Queue trigger function processed:");
        //    //TenantSyncMessage obj = JsonConvert.DeserializeObject<TenantSyncMessage>(message);
        //    //DateTime localDateTime, univDateTime;
        //    //localDateTime = DateTime.Parse(strDateTime);
        //    //univDateTime = localDateTime.ToUniversalTime();


        //    Sync().Wait();
        //}

        public static async Task Sync()
        {

            var tenant = GetTenantList();
            //foreach (var TenantItem in tenant)
            //{
            //    if (!string.IsNullOrEmpty(TenantItem.AccessToken))
            //    {
            //         MoveStatistics(TenantItem.Id.Value);
            //    }
            //}

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
                if (!string.IsNullOrEmpty(TenantItem.AccessToken))
                {
                    try
                    {
                        await GetUsageConversation(TenantItem, tbl);

                    }
                    catch
                    {

                    }
                }
            }
            UpdateStatistics(tbl);
        }

        private static List<TenantModel> GetTenantList()
        {

            //var x = GetContactId("962779746365", "28");


            string connString = Constant.ConnectionString;
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
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
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
        private static async Task<DataTable> GetUsageConversation(TenantModel tenant, DataTable tbl)
        {
            try
            {
                Dictionary<string, int> conversations = new Dictionary<string, int>();
                int totalUsageFreeUI = 0;
                int totalUsageFreeBI = 0;
                int totalUsagePaidUI = 0;
                int totalUsagePaidBI = 0;
                int totalUsageFreeEntry = 0;

                string freeTierConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_TIER);
                string freeEntryPointConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.FREE_ENTRY_POINT);
                string unknownConversation = Enum.GetName(typeof(WhatsAppConversationTypeEnum), WhatsAppConversationTypeEnum.UNKNOWN);

                string serviceCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.SERVICE);
                string utilityCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.UTILITY);
                string authenticationCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.AUTHENTICATION);
                string marketingCategory = Enum.GetName(typeof(WhatsAppConversationCategoryEnum), WhatsAppConversationCategoryEnum.MARKETING);

                DateTime date = DateTime.UtcNow;
                int year = date.Year;
                int month = date.Month;
                DateTime firstDayOfMonth = new DateTime(date.Year, date.Month-1, 1);//.AddHours(-Constant.AddHour);
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
                    dr["TenantId"] = tenant.Id;
                    tbl.Rows.Add(dr);
                }
                return tbl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static void MoveStatistics(int tenantId)
        {
            try
            {
                int perviousMonth = DateTime.Now.AddMonths(-1).Month;
                int currentMonth = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                var SP_Name = "[dbo].[MoveStatisticsConversation]";

                var sqlParameters = new List<SqlParameter> { 
                    new SqlParameter("@tenantId", tenantId),
                    new SqlParameter("@perviousMonth", perviousMonth),
                    new SqlParameter("@currentMonth", currentMonth),
                     new SqlParameter("@year", year),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        private static void UpdateStatistics(DataTable tbl)
        {
            try
            {

                var SP_Name = "[dbo].[ConversationMeasurementBulkUpdate]";

                var sqlParameters = new List<SqlParameter> { new SqlParameter("@ConversationMeasurementTable", tbl) };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        private static async Task<WhatsAppAnalyticsModel> getWhatsAppAnalyticAsync(DateTime start, DateTime end, TenantModel tenant)
        {
            int startTime, endTime = 0;

            WhatsAppAnalyticsModel objWhatsAppAnalytic = new WhatsAppAnalyticsModel();

            using (var httpClient = new HttpClient())
            {
                string[] obj = { "CONVERSATION_DIRECTION", "CONVERSATION_TYPE", "COUNTRY", "PHONE" };

                startTime = (int)ConvertDatetimeToUnixTimeStamp(start);
                endTime = (int)ConvertDatetimeToUnixTimeStamp(end);

                var postUrl = Constant.WhatsAppApiUrl + tenant.WhatsAppAccountID + "?fields=conversation_analytics.start(" + startTime + ").end(" + endTime + ").granularity(DAILY).dimensions(" + obj[0] + "," + obj[1] + "," + obj[2] + "," + obj[3] + ")";

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
        private static long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - originDate;
            return (long)Math.Floor(diff.TotalSeconds);
        }
        private static ConversationMeasurements getMeasurementForCampaign(TenantModel tenant)
        {
            try
            {
                ConversationMeasurements Measurement = new ConversationMeasurements();
                var SP_Name = "[dbo].[ConversationMeasurementsGet]";
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@Year",DateTime.Now.Year)
                    ,new SqlParameter("@Month",DateTime.Now.Month)
                    ,new SqlParameter("@TenantId",tenant.TenantId.Value)
                };
                Measurement = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapDashboardStatistic, Constants.ConnectionString).FirstOrDefault();
                return Measurement;

            }
            catch (Exception ex)
            {
                return new ConversationMeasurements()
                {
                    ErrorMsg = ex.Message,
                    TotalFreeConversationWA = 0,
                    TotalUsageFreeConversation = 0,
                    TotalUIConversation = 0,
                    TotalUsageUIConversation = 0,
                    TotalBIConversation = 0,
                    TotalUsageBIConversation = 0
                };
            }
        }
        private static ConversationMeasurements MapDashboardStatistic(IDataReader dataReader)
        {
            ConversationMeasurements Measurement = new ConversationMeasurements();

            Measurement.TotalFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalFreeConversationWA");
            Measurement.TotalUsageFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversationWA");
            Measurement.TotalUsageFreeUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeUIWA");
            Measurement.TotalUsageFreeBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeBIWA");
            Measurement.TotalUsageFreeConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversation");
            Measurement.TotalUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUIConversation");
            Measurement.TotalUsageUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageUIConversation");
            Measurement.TotalBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalBIConversation");
            Measurement.TotalUsageBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageBIConversation");
            Measurement.RemainingConversationWA = SqlDataHelper.GetValue<int>(dataReader, "RemainingConversationWA");
            return Measurement;
        }
    }
}

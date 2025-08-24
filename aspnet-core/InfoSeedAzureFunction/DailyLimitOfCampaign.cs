using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public class DailyLimitOfCampaign
    {
        [FunctionName("ChecDailyLimitFunction")]
        public static void Run([TimerTrigger("0 0 0 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)// Runs every day at midnight
        {
            //The cron expression "0 0 0 * * *" corresponds to midnight(00:00:00) in Coordinated Universal Time(UTC).
            CheckFunction(log).Wait();
        }
        public static async Task CheckFunction(ILogger log)
        {
            try
            {
                var tenants = TenantGetAll();

                foreach (var tenant in tenants) 
                {
                    // from Meta 
                    //int DailyLimit = await GetDailyLimitAsync(tenant.AccessToken, tenant.WhatsAppAccountID);
                    //if (DailyLimit == 0)
                    //{ continue;}


                    int tenantId = UpdateTenantDailyLimit(tenant.Id , tenant.BIDailyLimit );
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region private
        /// <summary>
        /// Get Tenant info
        /// </summary>
        /// <returns>tenant id and WhatsAppAccountID and AccessToken </returns>
        private static List<TenantModel> TenantGetAll()
        {
            try
            {
                List<TenantModel> model = new List<TenantModel>();
                var SP_Name = Constants.Tenant.SP_TenantGetInfo;

                model = SqlDataHelper.ExecuteReader(SP_Name, new List<System.Data.SqlClient.SqlParameter>().ToArray(), MapTenantInfo, Constants.ConnectionString).ToList();
                return model;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get DailyLimit from meta
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="WhatsAppAccountID"></param>
        /// <returns>DailyLimit</returns>
        private static async Task<int> GetDailyLimitAsync(string AccessToken, string WhatsAppAccountID)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://business.facebook.com/api/graphql/");
                request.Headers.Add("accept", "*/*");
                request.Headers.Add("accept-language", "en-US,en;q=0.9,ar-JO;q=0.8,ar-AE;q=0.7,ar;q=0.6,en-GB;q=0.5");
                request.Headers.Add("cookie", "datr=-kD6YwsHOTWI2hvtlFtLyPdF; sb=I8n9Yx8y0-FbsBP1wotuekRw; locale=en_US; c_user=100093542498596; xs=40%3Aj9EA_ziZeDHT9w%3A2%3A1706001680%3A-1%3A14142; fr=1LL9gs9BT0gosWKan.AWUUOSkHjt8aqhIwrso5LfPXPJc.BlorO7.NE.AAA.0.0.Blr4US.AWWv5d2a9hU; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1706001688546%2C%22v%22%3A1%7D; dpr=1; wd=1920x484; usida=eyJ2ZXIiOjEsImlkIjoiQXM3cGphOTRtYmFvbiIsInRpbWUiOjE3MDYwMDIwNzF9; c_user=100093542498596; fr=1ZK3oe15Y1lrqyjqk.AWV7frk80tGLeoUChkJBY4aefmQ.BlsiGa.NE.AAA.0.0.BlsiGa.AWVrV92tNS4; xs=40%3Aj9EA_ziZeDHT9w%3A2%3A1706001680%3A-1%3A14142%3A%3AAcUb8JABd6lZQGfwFZ-4JbyPug2EBli1wAOfvwL3LQ");
                request.Headers.Add("sec-fetch-site", "same-origin");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                request.Headers.Add("Authorization", "Bearer "+ AccessToken);

                var collection = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("fb_dtsg", "NAcPnmjXoN8yV91l5HMbE1Cdx56YSrsvFlsQ96mYza3oLcTozVRgmXg:40:1706001680"),
                    new KeyValuePair<string, string>("variables", "{\"waba_id\":\""+WhatsAppAccountID+"\"}"),
                    new KeyValuePair<string, string>("doc_id", "5967640690007388")
                };
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(result);

                int totalConversations = json["data"]["whatsapp_manager_phone_numbers_table_data"][0]["total_conversations"].Value<int>();

                return totalConversations;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Update Tenant Daily Limit
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="DailyLimit"></param>
        /// <returns></returns>
        private static int UpdateTenantDailyLimit(int? tenantId, int BIDailyLimit)
        {
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantUpdateDailyLimit;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                    new System.Data.SqlClient.SqlParameter("@bIDailyLimit",BIDailyLimit)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@OutputParameter",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Mapper
        public static TenantModel MapTenantInfo(IDataReader dataReader)
        {
            try
            {
                TenantModel model = new TenantModel();

                model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
                model.BIDailyLimit = SqlDataHelper.GetValue<int>(dataReader, "BIDailyLimit");
                model.DailyLimit = SqlDataHelper.GetValue<int?>(dataReader, "DailyLimit") ?? 0;

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

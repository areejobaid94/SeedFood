using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public static class ConversationNumberSyncFunction
    {
        //[FunctionName("ConversationNumberSyncFunction")]
        //public static void Run([TimerTrigger("0 0 1 * * * ", RunOnStartup = false)] TimerInfo myTimer)
        //{

        //    Sync().Wait();
        //}

        public static async Task Sync()
        {
            var tenant = GetTenantList();

            foreach (var TenantItem in tenant)
            {
                try
                {
                    UpdateStatistics(TenantItem.Id);

                }
                catch
                {

                }
            }
        }

        private static List<TenantModel> GetTenantList()
        {
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

        private static void UpdateStatistics(int? tenantId)
        {
            try
            {
                int perviousMonth = DateTime.Now.AddMonths(-1).Month;
                int currentMonth = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                int TotalBIConversation = 0;
                int TotalUsageBIConversation = 0;   
                int TotalCreditBIConversation = 0;
                int TotalCreditUIConversation = 0;

                var SP_Name = "[dbo].[ConversationNumberUpdate]";

                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@tenantId",tenantId)
                    ,new SqlParameter("@perviousMonth",perviousMonth)
                    ,new SqlParameter("@currentMonth",currentMonth)
                    ,new SqlParameter("@year",year)
                    ,new SqlParameter("@TotalBIConversation",TotalBIConversation)
                    ,new SqlParameter("@TotalUsageBIConversation",TotalUsageBIConversation)
                    ,new SqlParameter("@TotalCreditBIConversation",TotalCreditBIConversation)
                    ,new SqlParameter("@TotalCreditUIConversation",TotalCreditUIConversation)         
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
 
    }
}

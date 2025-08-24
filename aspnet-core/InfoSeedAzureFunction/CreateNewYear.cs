using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using InfoSeedAzureFunction.Model;
using System.Linq;

namespace InfoSeedAzureFunction
{
    public static class CreateNewYear
    {
        //[FunctionName("CreateNewYear")]
        //public static void Run([TimerTrigger("0 0 1 * * * ", RunOnStartup = false)] TimerInfo myTimer)
        //{

        //    //Sync().Wait();
        //}

        public static async Task Sync()
        {
            var tenant = GetTenantList();//.Where(x=>x.Id!=27);



            foreach (var TenantItem in tenant)
            {
                try
                {
                    
                    for (int i = 1; i <= 12; i++)
                    {
                        ConversationMeasurementsTenantModel conservationMeasurementsModel = new ConversationMeasurementsTenantModel();

                        conservationMeasurementsModel.TenantId = TenantItem.Id;
                        conservationMeasurementsModel.Month = i;


                        AddYear(conservationMeasurementsModel);
                        //lstconservation.Add(conservationMeasurementsModel);

                    }

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




        private static void AddYear(ConversationMeasurementsTenantModel tenantId)
        {
            try
            {

                var SP_Name = "[dbo].[AddYear]";

                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@TenantId",tenantId.TenantId)
                    ,new SqlParameter("@Month",tenantId.Month)
                   
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

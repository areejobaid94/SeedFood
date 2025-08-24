using ConservationMeasurementsJob.Entities;
using Framework.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConservationMeasurementsJob.DAL
{
    public class ConversationRepository
    {
        public static void LogConversationMasurements(ConservationMeasurement entity, TextWriter log)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity.ConversationId))
                {
                    // int AddHour = int.Parse(ConfigurationManager.AppSettings["AddHour"]);
                    // DateTime dateTime = DateTime.UtcNow.AddHours(AddHour);
                    var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId",entity.TenantId),
                    new SqlParameter("@CommunicationInitiated",(int)entity.CommunicationInitiated),
                    new SqlParameter("@ConversationId",entity.ConversationId),
                    new SqlParameter("@PhoneNumber",entity.PhoneNumber),
                    new SqlParameter("@MessageId",entity.MessageId),
                    new SqlParameter("@MessageStatusId",entity.MessageStatusId),
                };



                    SqlParameter OutsqlParameter = new SqlParameter();
                    OutsqlParameter.ParameterName = "@IsBundleActive";
                    OutsqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
                    OutsqlParameter.Direction = System.Data.ParameterDirection.Output;
                    sqlParameters.Add(OutsqlParameter);


                    SqlDataHelper.ExecuteNoneQuery(
                                   Constants.SP_ConservationMeasurementUpdate,
                                   sqlParameters.ToArray(), ConfigurationManager.ConnectionStrings["InfoseedDB"].ConnectionString);


                    if (!(bool)OutsqlParameter.Value)
                    {
                        UpdateConversationBundle(entity.TenantId);
                    }
                }
                else
                {
                    var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@MessageId",entity.MessageId),
                    new SqlParameter("@MessageStatusId",entity.MessageStatusId),
                    new SqlParameter("@TenantId",entity.TenantId),
                };
                    SqlDataHelper.ExecuteNoneQuery(
                                   Constants.SP_ContactsCampaignMessageUpdate,
                                   sqlParameters.ToArray(), ConfigurationManager.ConnectionStrings["InfoseedDB"].ConnectionString);


                }

            }
            catch (SqlException sqlex)
            {
                log.Write("SQL ERROR WHILE  UPDATING ConservationMeasurement: " + sqlex.StackTrace);

                throw sqlex;
            }
            catch (Exception ex)
            {
                log.Write("UNEXPECTED EXCEPTION WHILE UPDATING ConservationMeasurement: " + ex.StackTrace);
                throw ex;
            }
        }



        //public static bool CheckConversationBundleValidate(ConservationMeasurement entity, TextWriter log)
        //{
        //    try
        //    {

        //        int AddHour =int.Parse( ConfigurationManager.AppSettings["AddHour"]);
        //        DateTime dateTime = DateTime.UtcNow.AddHours(AddHour);
        //        var sqlParameters = new List<SqlParameter> {
        //            new SqlParameter("@TenantId",entity.TenantId),
        //            new SqlParameter("@DateTime",dateTime),
               
        //        };



        //        SqlDataHelper.ExecuteNoneQuery(
        //                   Constants.SP_ConversationBundleValidateGet,
        //                   sqlParameters.ToArray(), ConfigurationManager.ConnectionStrings["InfoseedDB"].ConnectionString);
        //        SqlParameter OutsqlParameter = new SqlParameter();
        //        OutsqlParameter.ParameterName = "@Result";
        //        OutsqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
        //        OutsqlParameter.Direction = System.Data.ParameterDirection.Output;
        //        return (bool)OutsqlParameter.Value; ;
        //    }

        //    catch (SqlException sqlex)
        //    {
        //        log.Write("SQL ERROR WHILE  Check Conversation Bundle Validate: " + sqlex.StackTrace);

        //        throw sqlex;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Write("UNEXPECTED EXCEPTION WHILE Check Conversation Bundle Validate: " + ex.StackTrace);
        //        throw ex;
        //    }
        //}


        public static async void UpdateConversationBundle(int tenantId)
        {
   
            var itemsCollection = new DocumentDBHelper<TenantEntity>(CollectionTypes.ItemsCollection);
            TenantEntity tenant = await itemsCollection.GetItemAsync(a => a.ItemType ==InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);
            tenant.IsBundleActive = false;

            var result =  itemsCollection.UpdateItemAsync(tenant._self, tenant);
        }

      
    }
}

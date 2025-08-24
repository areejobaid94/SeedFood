using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Web.Models.ConservationMeasurements;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConservationMeasurementsController :  MessagingPortalControllerBase
    {

        public ConservationMeasurementsController()
        {

        }

        [Route("SecriptTen")]
        [HttpGet]
        public void SecriptTen( int Year)
        {
            var list = GeAllTenant();

            foreach(var ten in list)
            {
                ConservationMeasurementsModel conservationMeasurementsModel = new ConservationMeasurementsModel();

                for(int i = 1; i <= 12; i++)
                {
                    conservationMeasurementsModel.TenantId = ten.Id;
                    conservationMeasurementsModel.Year = Year;
                    conservationMeasurementsModel.Month = i;
                    conservationMeasurementsModel.BusinessInitiatedCount = 0;
                    conservationMeasurementsModel.UserInitiatedCount = 0;
                    conservationMeasurementsModel.ReferralConversionCount = 0;
                    conservationMeasurementsModel.TotalFreeConversation = 1000;
                    conservationMeasurementsModel.LastUpdatedDate = DateTime.Now;
                    conservationMeasurementsModel.CreationDate = DateTime.Now;


                    CreateItemWithImage(conservationMeasurementsModel);
                }



            }
        }

        #region
        private List<TenantListDto> GeAllTenant()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants]";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<TenantListDto> itemDtos = new List<TenantListDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemDtos.Add(new TenantListDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                         TenancyName= dataSet.Tables[0].Rows[i]["TenancyName"].ToString()

                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemDtos;

        }

        private void CreateItemWithImage(ConservationMeasurementsModel item)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO ConservationMeasurements "
                            + " (TenantId ,Year ,Month ,BusinessInitiatedCount ,UserInitiatedCount ,ReferralConversionCount ,TotalFreeConversation ,LastUpdatedDate ,CreationDate) VALUES "
                            + " (@TenantId ,@Year ,@Month ,@BusinessInitiatedCount ,@UserInitiatedCount ,@ReferralConversionCount ,@TotalFreeConversation ,@LastUpdatedDate ,@CreationDate) ";

                        command.Parameters.AddWithValue("@TenantId", item.TenantId);
                        command.Parameters.AddWithValue("@Year", item.Year);
                        command.Parameters.AddWithValue("@Month", item.Month);
                        command.Parameters.AddWithValue("@BusinessInitiatedCount", item.BusinessInitiatedCount);
                        command.Parameters.AddWithValue("@UserInitiatedCount", item.UserInitiatedCount);
                        command.Parameters.AddWithValue("@ReferralConversionCount", item.ReferralConversionCount);
                        command.Parameters.AddWithValue("@TotalFreeConversation", item.TotalFreeConversation);
                        command.Parameters.AddWithValue("@LastUpdatedDate", item.LastUpdatedDate);
                        command.Parameters.AddWithValue("@CreationDate", item.CreationDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        #endregion
    }
}

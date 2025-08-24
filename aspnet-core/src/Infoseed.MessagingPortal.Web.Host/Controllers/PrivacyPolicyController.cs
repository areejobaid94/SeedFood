using Infoseed.MessagingPortal.Logic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivacyPolicyController : MessagingPortalControllerBase
    {
     
        public PrivacyPolicyController()
        {
          
        }


        #region public 


        [Route("GetPrivacyPolicy")]
        [HttpGet]
        public string GetPrivacyPolicy(string local)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[PrivacyPolicy] where Local= N'" + local+"'";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);



            string res = "";

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                res= dataSet.Tables[0].Rows[i]["Text"].ToString();
            }

            conn.Close();
            da.Dispose();

            return res;
        }



        #endregion
    }
}

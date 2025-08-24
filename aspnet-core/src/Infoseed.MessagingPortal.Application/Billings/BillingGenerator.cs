using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.AccountBillings;
using Infoseed.MessagingPortal.TenantServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Billings
{
    public class BillingGenerator
    {
        private readonly IRepository<Billing> _billingRepository;
       // private readonly IRepository<SalesUserCreate.SalesUserCreate> _salesUserCreateRepository;
        public BillingGenerator(IRepository<Billing> billingRepository)
        {
            _billingRepository = billingRepository;
           // _salesUserCreateRepository = salesUserCreateRepository;
        }
        public async Task SealsUserUpdateSync()
        {
            var list = GetSalesUserCreate();
            List<string> dayList = new List<string>() { "sunday", "monday", "saturday" };
            var dayName = DateTime.Now.ToString("dddd");
            foreach (var item in list)
            {
                var day = dayList.Where(x => x == dayName.ToLower()).FirstOrDefault();
                if (day != null)
                {
                    SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
                    {
                        Id = item.Id,
                        IsActiveSubmitButton = true,
                        IsActiveButton = true,
                        TenantId = item.TenantId,
                        TotalCreate = item.TotalCreate,
                        UserId = 0,
                        UserName = item.UserName
                    };
                    UpdateSalesUserCreate(salesUserCreate);
                }


            }

        }
        public async Task Create()
        {
         


            Billing billing = new Billing();
            //await _accountBillingRepository.InsertAsync(accountBilling);
            var contactAc = GetContactPerMonth();
            var tenantId = new List<int>();

            foreach (var item in contactAc)
            {

                var found = tenantId.Where(x => x == item.TenantId).ToList();
                if (found.Count() == 0)
                {

                    tenantId.Add(item.TenantId);
                }


            }

            foreach (var item in tenantId)
            {

                var activeList = new List<ActiveContactPerMonthModel>();
                var infoseedSID = GetTenantService().Where(x => x.TenantId == item).FirstOrDefault();

                var found = contactAc.Where(x => x.TenantId == item).ToList();

                foreach(var ac in found)
                {
                    var fo = activeList.Where(x => x.Month == ac.Month && x.Year == ac.Year).FirstOrDefault();
                    if(fo==null)
                    {
                        activeList.Add(ac);
                    }

                }



                foreach(var active in activeList)
                {
                    var billingFound = GetBilling().Where(x => x.TenantId == item && x.CreationTime.Month == active.Month && x.CreationTime.Year == active.Year).FirstOrDefault();
                    if (billingFound == null)
                    {
                        string timeNow = active.LastMessageDateTime.ToString();
                        var now = Convert.ToDateTime(timeNow);
                        var startDate = new DateTime(now.Year, now.Month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);


                        var found2 = found.Where(x => x.Month == now.Month && x.Year == now.Year).ToList();

                        if (found2.Count() > 0)
                        {
                            billing.CreationTime = DateTime.Now;
                            billing.TenantId = item;
                            billing.IsDeleted = false;
                            billing.TotalAmount = infoseedSID.ServiceFees * found2.Count();
                            billing.CreationTime = now;
                            billing.CurrencyId = 1;
                            billing.BillPeriodFrom = startDate;
                            billing.BillPeriodTo = endDate;
                            billing.IsPayed = false;
                            //billing.TenantServiceId = infoseedSID.Id;
                            billing.BillingID = Guid.NewGuid().ToString();

                            await _billingRepository.InsertAsync(billing);

                        }

                    }
                }

    


            }



        }
        private void UpdateSalesUserCreate(SalesUserCreate.SalesUserCreate salesUserCreate)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "UPDATE SalesUserCreate SET TenantId = @TenantId ,UserId =@UserId , UserName = @UserName , TotalCreate = @TotalCreate ,IsActiveButton = @IsActiveButton, IsActiveSubmitButton=@IsActiveSubmitButton  Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", salesUserCreate.Id);
                        command.Parameters.AddWithValue("@TenantId", salesUserCreate.TenantId);
                        command.Parameters.AddWithValue("@UserId", salesUserCreate.UserId);
                        command.Parameters.AddWithValue("@UserName", salesUserCreate.UserName);
                        command.Parameters.AddWithValue("@TotalCreate", salesUserCreate.TotalCreate);
                        command.Parameters.AddWithValue("@IsActiveButton", salesUserCreate.IsActiveButton);
                        command.Parameters.AddWithValue("@IsActiveSubmitButton", salesUserCreate.IsActiveSubmitButton);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception)
                {


                }

        }
        private List<SalesUserCreate.SalesUserCreate> GetSalesUserCreate()
        {
            string connString = AppSettingsModel.ConnectionStrings;
         

            string query = "select * from [dbo].[SalesUserCreate] ";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<SalesUserCreate.SalesUserCreate>   sales = new  List<SalesUserCreate.SalesUserCreate>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                sales.Add( new SalesUserCreate.SalesUserCreate
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                     IsActiveButton = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsActiveButton"].ToString()),
                     IsActiveSubmitButton = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsActiveSubmitButton"].ToString()),
                      TenantId= Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                       TotalCreate= Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalCreate"].ToString()),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"].ToString()),
                         UserName = dataSet.Tables[0].Rows[i]["UserName"].ToString()

                });
            }

            conn.Close();
            da.Dispose();

            return sales;

        }
        private List<ActiveContactPerMonthModel> GetContactPerMonth()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ActiveContactPerMonth]";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ActiveContactPerMonthModel> activeContactPerMonthModels = new List<ActiveContactPerMonthModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                activeContactPerMonthModels.Add(new ActiveContactPerMonthModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    ContactID = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactID"]),
                    Month = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Month"]),
                    Year = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Year"]),
                    LastMessageDateTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["LastMessageDateTime"].ToString())



                });
            }

            conn.Close();
            da.Dispose();

            return activeContactPerMonthModels;
        }

        private List<TenantService> GetTenantService()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[TenantServices]";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<TenantService>  tenantServices = new List<TenantService>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                tenantServices.Add(new TenantService
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    InfoSeedServiceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InfoSeedServiceId"]),
                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                    ServiceFees =  decimal.Parse( dataSet.Tables[0].Rows[i]["ServiceFees"].ToString()),

                });
            }

            conn.Close();
            da.Dispose();

            return tenantServices;
        }



        public async Task BillingUpdateSync()
        {
            var connString = AppSettingsModel.ConnectionStrings;

          //  var now = DateTime.Now.AddMonths(1);
            var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@BillDate",DateTime.Now)
                     };

            SqlDataHelper.ExecuteNoneQuery(
                      "dbo.BillingsGenerate",
                      sqlParameters.ToArray(),
                      connString);

        }

        private List<Billing> GetBilling()
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Billings]";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Billing> billings = new List<Billing>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                billings.Add(new Billing
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),

                });
            }

            conn.Close();
            da.Dispose();

            return billings;
        }
    }
}

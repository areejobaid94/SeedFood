using Abp.Domain.Repositories;
using Framework.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.AccountBillings
{
    public class AccountBillingGenerator
    {
        private readonly IRepository<AccountBilling> _accountBillingRepository;
        public AccountBillingGenerator(IRepository<AccountBilling> accountBillingRepository)
        {
            _accountBillingRepository = accountBillingRepository;
        }


        public async Task Create()
        {
            string timeNow = DateTime.Now.ToString("hh:mm:ss tt");
            var now = Convert.ToDateTime(timeNow);
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);



            AccountBilling accountBilling = new AccountBilling();
            //await _accountBillingRepository.InsertAsync(accountBilling);
            var contactAc = GetContactPerMonth();
            var tenantId = new List<int>();

            foreach(var item in contactAc)
            {

               var found= tenantId.Where(x => x == item.TenantId).ToList();
                if (found.Count()==0 )
                {

                    tenantId.Add(item.TenantId);
                }
                

            }

            foreach (var item in tenantId)
            {

                var found = contactAc.Where(x => x.TenantId == item).ToList();

                if (now == endDate)
                {
                    var found2 = found.Where(x => x.Month == now.Month && x.Year == now.Year).ToList();
                    if (found2.Count() > 0)
                    {

                        accountBilling.TenantId = item;
                        accountBilling.IsDeleted = false;
                        accountBilling.BillAmount = found2.Count();
                        accountBilling.CreationTime = now;

                        accountBilling.BillDateFrom = startDate;
                        accountBilling.BillDateTo = endDate;
                        accountBilling.BillID = Guid.NewGuid().ToString();

                        await _accountBillingRepository.InsertAsync(accountBilling);

                    }

                }

              

            }



        }
        public void Execute(IConfigurationRoot _appConfiguration)
        {
            var sqlParameters = new List<SqlParameter>
            {
            };
            SqlDataHelper.ExecuteNoneQuery(
                "dbo.AccountBillingsGenerate",
                sqlParameters.ToArray(), _appConfiguration.GetConnectionString("Default")
                );
        }
        public void Execute(string connectionString)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                       .AddJsonFile("appsettings.json"/*, optional: true, reloadOnChange: true*/)
                       //.AddEnvironmentVariables()
                       //.AddCommandLine(args)
                       .Build();
            var sqlParameters = new List<SqlParameter>
            {
            };
            SqlDataHelper.ExecuteNoneQuery(
                "dbo.AccountBillingsGenerate",
                sqlParameters.ToArray(), connectionString
                );
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

            List<ActiveContactPerMonthModel>  activeContactPerMonthModels = new List<ActiveContactPerMonthModel>();

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

    }
}

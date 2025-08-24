using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Newtonsoft.Json;
using System.Linq;
using RestSharp;
using InfoSeedAzureFunction.Model;
using System.Net;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http;

namespace InfoSeedAzureFunction
{
    public class CheckZohoInvoices
    {

        private static AccessTokenModel accessTokenModel { get; set; }

        //[FunctionName("CheckZohoInvoicesFunction")]
        //public static void Run([TimerTrigger("0 0 1 * * * ", RunOnStartup = false)] TimerInfo myTimer, ILogger log)//every day (daily)
        //{
        //    //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        //    CheckInvoicesFunction(log).Wait();
        //}
        public static async Task<string> CheckInvoicesFunction(ILogger log)
        {
            try
            {

                accessTokenModel = getZohoAccessToken();

                var ListModel = GetTenantList();

                foreach (var tenant in ListModel)
                {
                    if (!string.IsNullOrEmpty(tenant.ZohoCustomerId) )
                    {

                        try
                        {


                            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

                            var tenantCD = itemsCollection.GetItemAsync(p => p.TenantId == tenant.Id && p.ItemType == ContainerItemTypes.Tenant).Result;


                            var invous = GetAllInvoices(tenant.ZohoCustomerId);
                            if (invous.invoices != null)
                            {
                                foreach (var item in invous.invoices)
                                {
                                    addBilling(item, tenant.Id.Value);//add to DB
                                }

                                var invCAUTION = invous.invoices.Where(x => x.status == "sent").ToList();// InvoicesGet(tenant.ZohoCustomerId, "sent");// status CAUTION!
                                if (invCAUTION.Count() > 0)
                                {

                                    var InvBefore = invCAUTION.Where(x => ExtractNumber(x.due_days) <= tenant.CautionDays).ToArray();
                                    if (InvBefore.Length > 0)
                                    {
                                        await addInvoicesHistoryAsync(InvBefore, tenant.Id, "CAUTION", tenant.D360Key);

                                        tenantCD.IsCaution = true;
                                    }
                                    else
                                    {

                                        await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "CAUTION", false, tenant.D360Key);//on CAUTION

                                        tenantCD.IsCaution = false;
                                    }

                                }
                                else
                                {

                                    await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "CAUTION", false, tenant.D360Key);//on CAUTION

                                    tenantCD.IsCaution = false;
                                }


                                var invWARNING = invous.invoices.Where(x => x.status == "overdue").ToList();// InvoicesGet(tenant.ZohoCustomerId, "overdue");// status WARNING
                                if (invWARNING.Count > 0)
                                {
                                    var bef = invWARNING.Where(x => ExtractNumber(x.due_days) >= tenant.WarningDays).ToArray();

                                    if (bef.Length > 0)
                                    {
                                        await addInvoicesHistoryAsync(bef, tenant.Id, "WARNING", tenant.D360Key);

                                        tenantCD.IsPaidInvoice = false;
                                    }
                                    else
                                    {

                                        await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "WARNING", true, tenant.D360Key);//on WARNING 

                                        tenantCD.IsPaidInvoice = true;
                                        tenantCD.IsCaution = true;
                                    }

                                }
                                else
                                {
                                    await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "WARNING", true, tenant.D360Key);//on WARNING 

                                    tenantCD.IsPaidInvoice = true;
                                }


                                await itemsCollection.UpdateItemAsync(tenantCD._self, tenantCD);

                            }



                        }
                        catch
                        {

                        }


                    }
                    try
                    {
                        await Task.Delay(1000);

                    }
                    catch
                    {

                    }
                }

            }
            catch (Exception ex)
            {

                log.LogInformation(ex.Message);

            }

            return "";
        }

        private static void addBilling(InvoicesModel.Invoice item, int tenantId)
        {
            try
            {

                var SP_Name = "BillingAdd";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@BillingID",item.invoice_id)
                    ,new System.Data.SqlClient.SqlParameter("@BillingDate",DateTime.Parse(item.date))
                    ,new System.Data.SqlClient.SqlParameter("@TotalAmount",decimal.Parse(item.total.ToString()))
                    //,new System.Data.SqlClient.SqlParameter("@BillPeriodTo",Date)
                    //,new System.Data.SqlClient.SqlParameter("@BillPeriodFrom",messageTemplateModel.id)
                    ,new System.Data.SqlClient.SqlParameter("@DueDate",DateTime.Parse(item.due_date))
                    //,new System.Data.SqlClient.SqlParameter("@IsPayed",false)
                    ,new System.Data.SqlClient.SqlParameter("@Status",item.status)
                    ,new System.Data.SqlClient.SqlParameter("@CustomerId",item.customer_id)
                     ,new System.Data.SqlClient.SqlParameter("@InvoiceJson",JsonConvert.SerializeObject(item))
                };



                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);
                //return (long)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static List<TenantModel> GetTenantList()
        {

            //var x = GetContactId("962779746365", "28");


            string connString = Constant.ConnectionString;
            string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0 and ZohoCustomerId IS NOT NULL AND ZohoCustomerId <> ''";


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
                    WarningDays = Convert.ToInt32(dataSet.Tables[0].Rows[i]["WarningDays"])
                });

            }

            conn.Close();
            da.Dispose();

            return order;

        }

        public static InvoicesModel InvoicesGet(string ZohoCustomerId, string status)
        {
            var client = new RestClient("https://invoice.zoho.com/api/v3/invoices?customer_id=" + ZohoCustomerId + "&&status=" + status);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessTokenModel.access_token);
            //   request.AddHeader("Cookie", "BuildCookie_800001748=1; 0d082fb755=622080e4a2a522d402965665137144e9; JSESSIONID=B6196252D559333C3B0DB909391DCBA8; _zcsr_tmp=436c8351-0058-47a9-ba3d-a270a0d7b3ac; zbcscook=436c8351-0058-47a9-ba3d-a270a0d7b3ac");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new InvoicesModel();
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                RefreshToken();
                return InvoicesGet(ZohoCustomerId, status);
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var rez = JsonConvert.DeserializeObject<InvoicesModel>(response.Content);

                rez.invoices = rez.invoices.Where(x => x.status != "draft").ToArray();

                return rez;
            }
            else
            {
                return new InvoicesModel();
            }

        }
        public static InvoicesModel GetAllInvoices(string ZohoCustomerId)
        {

            InvoicesModel rez = new InvoicesModel();
            List<InvoicesModel.Invoice> list = new List<InvoicesModel.Invoice>();
            try
            {
                bool isnext = true;
                int count = 1;
                while (isnext)
                {

                    var client = new RestClient("https://invoice.zoho.com/api/v3/invoices?customer_id=" + ZohoCustomerId+"&?page="+count);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Authorization", "Bearer " + accessTokenModel.access_token);
                    IRestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return new InvoicesModel();
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        RefreshToken();
                        return GetAllInvoices(ZohoCustomerId);
                    }
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var rez2 = JsonConvert.DeserializeObject<InvoicesModel>(response.Content);


                        list.AddRange(rez2.invoices.Where(x => x.status != "draft"));





                        isnext =rez2.page_context.has_more_page;
                        count++;


                    }
                    else
                    {
                        return new InvoicesModel();
                    }


                }

                rez.invoices = list.ToArray();
            }
            catch
            {

            }
            return rez;

        }

        private static AccessTokenModel getZohoAccessToken()
        {
            try
            {
                AccessTokenModel liveChatEntity = new AccessTokenModel();
                var SP_Name = "GetZohoAccessToken";

                var sqlParameters = new List<SqlParameter>
                {
                };

                liveChatEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapAccessToken, Constant.ConnectionString).FirstOrDefault();


                return liveChatEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static AccessTokenModel MapAccessToken(IDataReader dataReader)
        {
            try
            {
                AccessTokenModel model = new AccessTokenModel();
                model.access_token = SqlDataHelper.GetValue<string>(dataReader, "access_token");
                model.refresh_token = SqlDataHelper.GetValue<string>(dataReader, "refresh_token");
                model.client_secret = SqlDataHelper.GetValue<string>(dataReader, "client_secret");
                model.client_id = SqlDataHelper.GetValue<string>(dataReader, "client_id");
                model.redirect_uri = SqlDataHelper.GetValue<string>(dataReader, "redirect_uri");
                model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void RefreshToken()
        {

            var client = new RestClient("https://accounts.zoho.com/oauth/v2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AlwaysMultipartFormData = true;
            request.AddParameter("client_id", accessTokenModel.client_id);
            request.AddParameter("client_secret", accessTokenModel.client_secret);
            request.AddParameter("refresh_token", accessTokenModel.refresh_token);
            request.AddParameter("grant_type", "refresh_token");
            IRestResponse response = client.Execute(request);



            var rez = JsonConvert.DeserializeObject<AccessTokenModel>(response.Content);
            accessTokenModel.access_token = rez.access_token;
            UpdateZohoAccessToken();

        }
        private static void UpdateZohoAccessToken()
        {
            try
            {

                var SP_Name = "ZohoAccessTokenUpdate";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@access_token",accessTokenModel.access_token),
                     new System.Data.SqlClient.SqlParameter("@refresh_token",accessTokenModel.refresh_token),
                     new System.Data.SqlClient.SqlParameter("@expires_in",3600),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static async Task addInvoicesHistoryAsync(InvoicesModel.Invoice[] item, int? tenantId, string Status, string D360Key)
        {
            try
            {
                var SP_Name = "InvoicesHistoryAdd";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Month",DateTime.UtcNow.Month)
                    ,new System.Data.SqlClient.SqlParameter("@Year",DateTime.UtcNow.Year)
                    ,new System.Data.SqlClient.SqlParameter("@Status",Status)
                     ,new System.Data.SqlClient.SqlParameter("@InvoicesJson",JsonConvert.SerializeObject(item))
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

                try
                {
                    var url = Constant.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync(url))
                        {
                            var resultrespons = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                            }
                        }
                    }

                }
                catch (Exception ex)
                {


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task UpdateInvoicesHistoryAsync(string customer_id, string Status, bool IsStatus, string D360Key)
        {
            try
            {
                var SP_Name = "InvoicesHistoryUpdate";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@ZohoCustomerId",customer_id)
                    ,new System.Data.SqlClient.SqlParameter("@Status",Status)
                     ,new System.Data.SqlClient.SqlParameter("@IsStatus",IsStatus)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constant.ConnectionString);

                try
                {
                    var url = Constant.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync(url))
                        {
                            var resultrespons = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                            }
                        }
                    }

                }
                catch (Exception ex)
                {


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static int ExtractNumber(string str1)
        {
            string str2 = string.Empty;
            int val = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (Char.IsDigit(str1[i]))
                    str2 += str1[i];
            }
            if (str2.Length > 0)
                val = int.Parse(str2);
            return val;
        }
        private static TenantModel MapTenantModel(IDataReader dataReader)
        {
            TenantModel catalogue = new TenantModel
            {
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                SmoochAppID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAppID"),
                SmoochSecretKey = SqlDataHelper.GetValue<string>(dataReader, "SmoochSecretKey"),
                SmoochAPIKeyID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAPIKeyID"),

                DirectLineSecret = SqlDataHelper.GetValue<string>(dataReader, "DirectLineSecret"),
                botId = SqlDataHelper.GetValue<string>(dataReader, "botId"),
                IsBotActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBotActive"),

                IsCancelOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsCancelOrder"),
                CancelTime = SqlDataHelper.GetValue<int>(dataReader, "CancelTime"),

                //StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate"),
                //EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate"),
                //WorkText = SqlDataHelper.GetValue<string>(dataReader, "WorkText"),
                IsWorkActive = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActive"),
                IsBellOn = SqlDataHelper.GetValue<bool>(dataReader, "IsBellOn"),
                IsBellContinues = SqlDataHelper.GetValue<bool>(dataReader, "IsBellContinues"),
                workModel = new WorkModel
                {
                    IsWorkActiveSun = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSun"),
                    IsWorkActiveFri = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveFri"),
                    IsWorkActiveMon = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveMon"),
                    IsWorkActiveSat = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSat"),
                    IsWorkActiveThurs = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveThurs"),
                    IsWorkActiveTues = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveTues"),
                    IsWorkActiveWed = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveWed"),

                    WorkTextMon = SqlDataHelper.GetValue<string>(dataReader, "WorkTextMon"),
                    WorkTextSat = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSat"),
                    WorkTextSun = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSun"),
                    WorkTextThurs = SqlDataHelper.GetValue<string>(dataReader, "WorkTextThurs"),
                    WorkTextTues = SqlDataHelper.GetValue<string>(dataReader, "WorkTextTues"),
                    WorkTextWed = SqlDataHelper.GetValue<string>(dataReader, "WorkTextWed"),
                    WorkTextFri = SqlDataHelper.GetValue<string>(dataReader, "WorkTextFri"),

                    StartDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSat"),
                    StartDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateFri"),
                    StartDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateMon"),
                    StartDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSun"),
                    StartDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateThurs"),
                    StartDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateTues"),
                    StartDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateWed"),


                    EndDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateFri"),
                    EndDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateMon"),
                    EndDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSat"),
                    EndDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSun"),
                    EndDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateThurs"),
                    EndDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateTues"),
                    EndDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateWed"),

                    HasSPSat = SqlDataHelper.GetValue<bool>(dataReader, "HasSPSat"),
                    HasSPSun = SqlDataHelper.GetValue<bool>(dataReader, "HasSPSun"),
                    HasSPMon = SqlDataHelper.GetValue<bool>(dataReader, "HasSPMon"),
                    HasSPTues = SqlDataHelper.GetValue<bool>(dataReader, "HasSPTues"),
                    HasSPWed = SqlDataHelper.GetValue<bool>(dataReader, "HasSPWed"),
                    HasSPThurs = SqlDataHelper.GetValue<bool>(dataReader, "HasSPThurs"),
                    HasSPFri = SqlDataHelper.GetValue<bool>(dataReader, "HasSPFri"),


                },

                IsEvaluation = SqlDataHelper.GetValue<bool>(dataReader, "IsEvaluation"),
                EvaluationText = SqlDataHelper.GetValue<string>(dataReader, "EvaluationText"),
                EvaluationTime = SqlDataHelper.GetValue<int>(dataReader, "EvaluationTime"),
                D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key"),

                isOrderOffer = SqlDataHelper.GetValue<bool>(dataReader, "isOrderOffer"),

                IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint"),
                Points = SqlDataHelper.GetValue<int>(dataReader, "Points "),

                PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                Image = SqlDataHelper.GetValue<string>(dataReader, "Image"),
                ImageBg = SqlDataHelper.GetValue<string>(dataReader, "ImageBg"),
                ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId"),
                id = SqlDataHelper.GetValue<string>(dataReader, "id"),
                _rid = SqlDataHelper.GetValue<string>(dataReader, "_rid"),
                _self = SqlDataHelper.GetValue<string>(dataReader, "_self"),
                _etag = SqlDataHelper.GetValue<string>(dataReader, "_etag"),
                _attachments = SqlDataHelper.GetValue<string>(dataReader, "_attachments"),
                _ts = SqlDataHelper.GetValue<int>(dataReader, "_ts"),
                AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken"),
                WhatsAppAccountID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAccountID"),
                WhatsAppAppID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAppID"),
                BotTemplateId = SqlDataHelper.GetValue<int>(dataReader, "BotTemplateId"),
                BIDailyLimit = SqlDataHelper.GetValue<int>(dataReader, "BIDailyLimit"),
                CurrencyCode = SqlDataHelper.GetValue<string>(dataReader, "CurrencyCode"),
                TimeZoneId = SqlDataHelper.GetValue<string>(dataReader, "TimeZoneId"),
                IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder"),
                IsPickup = SqlDataHelper.GetValue<bool>(dataReader, "IsPickup"),
                IsDelivery = SqlDataHelper.GetValue<bool>(dataReader, "IsDelivery"),
                BookingCapacity = SqlDataHelper.GetValue<int>(dataReader, "BookingCapacity"),
                ReminderBookingHour = SqlDataHelper.GetValue<int>(dataReader, "ReminderBookingHour"),
                IsCaution = SqlDataHelper.GetValue<bool>(dataReader, "IsCaution"),
                IsPaidInvoice = SqlDataHelper.GetValue<bool>(dataReader, "IsPaidInvoice"),

                CautionDays = SqlDataHelper.GetValue<int>(dataReader, "CautionDays"),
                WarningDays = SqlDataHelper.GetValue<int>(dataReader, "WarningDays"),
                RejectRequestText = SqlDataHelper.GetValue<string>(dataReader, "RejectRequestText"),
                ConfirmRequestText = SqlDataHelper.GetValue<string>(dataReader, "ConfirmRequestText"),
            };
            return catalogue;
        }

    }
}

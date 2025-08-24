using Abp.Json;
using Framework.Data;
using Framework.Payment.Implementation.Zoho;
using Framework.Payment.Interfaces.Zoho;
using Framework.Payment.Model;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Zoho.Dto;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Zoho.Dto.InvoicesModel;
using SqlParameter = Microsoft.Azure.Documents.SqlParameter;

namespace Infoseed.MessagingPortal.Zoho
{
    public class ZohoAppService : MessagingPortalAppServiceBase, IZohoAppService
    {
        IInvoices _invoices;
        private IConfiguration _appConfiguration;
        private IGeneralAppService _generalAppService;
        private static AccessTokenModel accessTokenModel { get; set; }
        private readonly IDocumentClient _IDocumentClient;
        public ZohoAppService(IGeneralAppService generalAppService, IConfiguration appConfiguration, IDocumentClient IDocumentClient)
        {
            _appConfiguration=appConfiguration;
            _generalAppService=generalAppService;
            _invoices = new InvoicesApi(appConfiguration);
            _IDocumentClient=IDocumentClient;
        }

        public ZohoContactsModel GetContacts(int? TenantId)
        {

            if (TenantId==null)
            {
                TenantId=AbpSession.TenantId.Value;
            }
            var tenant = _generalAppService.GetTenantById(TenantId.Value);
            //tenant.PhoneNumber="962781399319";
            var model = _invoices.GetContactsByPhoneNumber(tenant.PhoneNumber);
            var ContactsModel = JsonConvert.DeserializeObject<ZohoContactsModel>(model);
            return ContactsModel;
        }
        public StatementsModel GetStatementsFillter(string fillter)
        {
            var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);
            var model = _invoices.GetStatements(tenant.ZohoCustomerId, fillter);
            var StatementsModel = JsonConvert.DeserializeObject<StatementsModel>(model);

            return StatementsModel;

        }


        public BillingStatusModel GetBillingStatus()
        {
            BillingStatusModel BillingModelReturn = new BillingStatusModel();
            //var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);

            //var DateBeforeNow = DateTime.UtcNow.AddDays(-2);
            //var DateAfterNow = DateTime.UtcNow.AddDays(2);

            //var invWARNING = _invoices.GetInvoicesStatus(tenant.ZohoCustomerId, "overdue");// status WARNING
            //var invWARNINGModel = JsonConvert.DeserializeObject<InvoicesModel>(invWARNING);
            //if (invWARNINGModel.invoices.Length>0)
            //{
            //    var bef = invWARNINGModel.invoices.Where(x => DateBeforeNow>=DateTime.Parse(x.date)).ToArray();
            //    if (bef.Length>0)
            //    {
            //        BillingModelReturn.Status="WARNING";
            //        BillingModelReturn.Massage="you have an unpaid bill, pay it now to avoid disconnecting the service";
            //        BillingModelReturn.IsActive=true;
            //        return BillingModelReturn;
            //    }

            //}

            //var invCAUTION = _invoices.GetInvoicesStatus(tenant.ZohoCustomerId, "sent");// status CAUTION!
            //var invCAUTIONModel = JsonConvert.DeserializeObject<InvoicesModel>(invCAUTION);
            //if (invCAUTIONModel.invoices.Length>0)
            //{
            //    var InvBefore = invCAUTIONModel.invoices.Where(x => DateAfterNow>=DateTime.Parse(x.due_date)).ToArray();
            //    if (InvBefore.Length>0)
            //    {
            //        BillingModelReturn.Status="CAUTION";
            //        BillingModelReturn.Massage="You have an unpaid bill, pay it before disconnecting the service PAY NOW ";
            //        BillingModelReturn.IsActive=true;
            //        return BillingModelReturn;
            //    }

            //}


            //BillingModelReturn.Status="";
            //BillingModelReturn.Massage="";
            //BillingModelReturn.IsActive=false;
            return BillingModelReturn;
        }
        public GenerateAccessTokenModel GenerateAccessToken(string code)
        {

            var model = _invoices.GenerateAccessToken(code);
            var rez = JsonConvert.DeserializeObject<GenerateAccessTokenModel>(model);
            UpdateZohoAccessToken(rez);
            return rez;
        }

        public InvoicesModel InvoicesGet(int pageNumber , int pageSize )
        {

            //pageNumber=pageNumber*pageSize;

            //var tenant = _generalAppService.GetTenantById(AbpSession.TenantId.Value);
            //var model = _invoices.GetInvoices(long.Parse(tenant.ZohoCustomerId), pageNumber, pageSize);
            //var rez = JsonConvert.DeserializeObject<InvoicesModel>(model);

            //rez.invoices= rez.invoices.Where(x => x.status!="draft").ToArray();
            //return rez;
            int total=0;
            InvoicesModel invoicesModel = new InvoicesModel();
            List<BillingDto> model = getBilling(pageNumber, pageSize, out total);
            List<Invoice> invo = new List<Invoice>();

            foreach (var bll in model)
            {
                var conInvoice = JsonConvert.DeserializeObject<Invoice>(bll.InvoiceJson);

                invo.Add(conInvoice);


            }

            invoicesModel.invoices=invo.ToArray();
            invoicesModel.TenantId=AbpSession.TenantId.Value;
            invoicesModel.page_context=new Page_Context() { total =total };
            return invoicesModel;

           
        }

        public async Task SyncBillingAsync(int pageNumber = 1, int pageSize = 100, int? tenantId = null)
        {





            try
            {
                accessTokenModel = getZohoAccessToken();

                tenantId ??= AbpSession.TenantId.Value;

                var tenant = _generalAppService.GetTenantById(tenantId.Value);

                var invous = GetAllInvoices(tenant.ZohoCustomerId);
                if (invous.invoices != null)
                {
                    int count = 0;
                    foreach (var item in invous.invoices)
                    {
                        if (count==40)
                        {

                        }
                        addBilling(item, tenantId.Value);//add to DB
                        count++;
                    }

                    var invCAUTION = invous.invoices.Where(x => x.status == "sent").ToList();// InvoicesGet(tenant.ZohoCustomerId, "sent");// status CAUTION!
                    if (invCAUTION.Count() > 0)
                    {

                        var InvBefore = invCAUTION.Where(x => ExtractNumber(x.due_days) <= tenant.CautionDays).ToArray();
                        if (InvBefore.Length > 0)
                        {
                            await addInvoicesHistoryAsync(InvBefore, tenant.TenantId, "CAUTION", tenant.D360Key);

                            tenant.IsCaution = true;
                        }
                        else
                        {

                            await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "CAUTION", false, tenant.D360Key);//on CAUTION

                            tenant.IsCaution = false;
                        }

                    }
                    else
                    {

                        await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "CAUTION", false, tenant.D360Key);//on CAUTION

                        tenant.IsCaution = false;
                    }


                    var invWARNING = invous.invoices.Where(x => x.status == "overdue").ToList();// InvoicesGet(tenant.ZohoCustomerId, "overdue");// status WARNING
                    if (invWARNING.Count > 0)
                    {
                        var bef = invWARNING.Where(x => ExtractNumber(x.due_days) >= tenant.WarningDays).ToArray();

                        if (bef.Length > 0)
                        {
                            await addInvoicesHistoryAsync(bef, tenant.TenantId, "WARNING", tenant.D360Key);

                            tenant.IsPaidInvoice = false;
                        }
                        else
                        {

                            await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "WARNING", true, tenant.D360Key);//on WARNING 

                            tenant.IsPaidInvoice = true;
                            tenant.IsCaution = true;
                        }

                    }
                    else
                    {
                        await UpdateInvoicesHistoryAsync(tenant.ZohoCustomerId, "WARNING", true, tenant.D360Key);//on WARNING 

                        tenant.IsPaidInvoice = true;
                    };
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var tenantCD = itemsCollection.GetItemAsync(p => p.TenantId == tenantId && p.ItemType == InfoSeedContainerItemTypes.Tenant).Result;

                    tenantCD.IsPaidInvoice=tenant.IsPaidInvoice;
                    tenantCD.IsCaution=tenant.IsCaution;

                    await itemsCollection.UpdateItemAsync(tenantCD._self, tenantCD);

                }



            }
            catch
            {

            }











            //////////

           
            //var tenant = _generalAppService.GetTenantById(tenantId.Value);
            //var modelInvoices = _invoices.GetInvoices(long.Parse(tenant.ZohoCustomerId), 1, 500);
            //var rez = JsonConvert.DeserializeObject<InvoicesModel>(modelInvoices);

            //foreach (var item in rez.invoices)
            //{

            //    addBilling(item, tenantId.Value);
            //    //if (!model.Any(x => x.BillingID == item.invoice_id))
            //    //{
            //    //    addBilling(item);
            //    //}
            //}

        }

        private void addBilling(InvoicesModel.Invoice item, int tenantId)
        {
            try
            {
                //int tenantId = AbpSession.TenantId.Value;

                var SP_Name = Constants.Billing.SP_BillingAdd;

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



                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                //return (long)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<BillingDto> getBilling(int pageNumber , int pageSize ,  out int TotalCount)
        {
            try
            {
                List<BillingDto> billingDto = new List<BillingDto>();
                var SP_Name = Constants.Billing.SP_BillingGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                  new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                   new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize),
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                billingDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBilling, AppSettingsModel.ConnectionStrings).ToList();


                TotalCount = Convert.ToInt32(OutputParameter.Value);
                return billingDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void UpdateZohoAccessToken(GenerateAccessTokenModel item)
        {
            try
            {

                var SP_Name = Constants.Billing.SP_ZohoAccessTokenUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@access_token",item.access_token),
                     new System.Data.SqlClient.SqlParameter("@refresh_token",item.refresh_token),
                     new System.Data.SqlClient.SqlParameter("@expires_in",item.expires_in),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
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

            InvoicesModel rez=new InvoicesModel();
            List<Invoice> list = new List<Invoice>();

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

            return rez;



        }

        private static AccessTokenModel getZohoAccessToken()
        {
            try
            {
                AccessTokenModel liveChatEntity = new AccessTokenModel();
                var SP_Name = "GetZohoAccessToken";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                };

                liveChatEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapAccessToken, AppSettingsModel.ConnectionStrings).FirstOrDefault();


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

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

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

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                try
                {
                    var url = AppSettingsModel.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
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

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                try
                {
                    var url = AppSettingsModel.EngineAPIURL + "api/WhatsAppAPI/DeleteCache?phoneNumberId=" + D360Key + "&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges";
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

        private long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - originDate;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        private async Task<ContactsInfo.Rootobject> GetListcontacts()
        {
            try
            {
                ContactsInfo.Rootobject contactsInfo = new ContactsInfo.Rootobject();

                List<ContactsInfo.Contact> list = new List<ContactsInfo.Contact>();

                bool isnext = true;
                int count = 1;
                while (isnext)
                {
                   
                    accessTokenModel = getZohoAccessToken();
                    if (accessTokenModel == null)
                        return contactsInfo;

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://invoice.zoho.com/api/v3/contacts?page="+count);
                    request.Headers.Add("Authorization", "Bearer " + accessTokenModel.access_token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    // Deserialize the response content into ContactsInfo.Rootobject
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    contactsInfo = JsonConvert.DeserializeObject<ContactsInfo.Rootobject>(jsonContent);


                    list.AddRange(contactsInfo.contacts.ToList());


                    isnext =contactsInfo.page_context.has_more_page;
                    count++;
                }

                contactsInfo.contacts=list.ToArray();

                return contactsInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<UpdateInvoicesModel> InvoicesCreateAsync(CreateInvoicesDashbordModel createInvoicesDashbordModel)
        {
            try
            {
                accessTokenModel = getZohoAccessToken();
                if (accessTokenModel == null)
                    return new UpdateInvoicesModel();

                CreateInvoicesModel invoicesModel = new CreateInvoicesModel();

                InvoicesModel invoicesModel1 = new InvoicesModel();
                ///////////////*************hasan**********///////////////
                var contactsInfo = await GetListcontacts();
                //if (contactsInfo.page_context.has_more_page)
                //{
                //    var contactsInfo2 = await GetListcontacts();

                //}
               
                // Assuming ZohoCustomerIds is a string
                var targetContactId = createInvoicesDashbordModel.ZohoCustomerIds.ToString();

                // Filter contacts based on contact_id
                var matchingContact = contactsInfo.contacts.FirstOrDefault(contact => contact.contact_id == targetContactId);
                if (matchingContact == null)
                    return new UpdateInvoicesModel();

                //invoicesModel1 = GetAllInvoices(createInvoicesDashbordModel.ZohoCustomerIds.ToString());
                //if (invoicesModel1.invoices == null)
                //    return new UpdateInvoicesModel();

                Line_Items[] line_Items = new Line_Items[]
                {
                   new Line_Items
                   {
                       name = "Deposit",
                       description = "Action of transferring funds into your wallet account to use this fund for campaigns and service conversation",
                       item_order = 5,
                       quantity = 1,
                       discount_amount = 0,
                       discount = 0,
                       rate = createInvoicesDashbordModel.TotalAmount,
                       item_total = createInvoicesDashbordModel.TotalAmount,
                       unit =" "
                   }
                };
                Payment_Gateways[] payment_Gateways = new Payment_Gateways[]
                {
                   new Payment_Gateways
                   {
                       configured = true,
                       additional_field1 = "standard",
                       gateway_name = "paytabs"
                   }
                };

                if (invoicesModel.payment_options == null)
                {
                    invoicesModel.payment_options = new Payment_Options();
                }

                DateTime dateTime = DateTime.UtcNow;
                long time = 0;

                DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan diff = dateTime.ToUniversalTime() - originDate;
                time = (long)Math.Floor(diff.TotalSeconds);

                invoicesModel.invoice_number = "INV-DEPOSIT-"+ time;
                invoicesModel.date = dateTime.ToString("yyyy-MM-dd");
                invoicesModel.status = "partially_paid";
                invoicesModel.discount = 0;
                invoicesModel.discount_type = "item_level";
                invoicesModel.is_inclusive_tax = false;
                invoicesModel.is_viewed_by_client = false;
                invoicesModel.has_attachment = false;
                invoicesModel.payment_terms_label = "Net 15";
                dateTime = dateTime.AddMonths(12);
                invoicesModel.due_date = dateTime.ToString("yyyy-MM-dd");
                invoicesModel.customer_id = long.Parse(createInvoicesDashbordModel.ZohoCustomerIds);
                invoicesModel.payment_expected_date = " ";
                invoicesModel.last_payment_date = " ";
                invoicesModel.reference_number = " ";
                invoicesModel.client_viewed_time = " ";
                invoicesModel.last_reminder_sent_date = " ";
                invoicesModel.attachment_name = " ";

                //invoicesModel.customer_name = invoicesModel1.invoices[0].customer_name;
                invoicesModel.customer_name = matchingContact.customer_name;
                try
                {
                    invoicesModel.currency_id = long.Parse(matchingContact.currency_id);
                }
                catch  { }  
                invoicesModel.currency_code = matchingContact.currency_code;

                invoicesModel.line_items = line_Items;

                //invoicesModel.salesperson_id = invoicesModel1.invoices[0].salesperson_id;
                invoicesModel.salesperson_name = "Infoseed";
                invoicesModel.total = 0;
                invoicesModel.payment_reminder_enabled = true;
                invoicesModel.payment_made = (float)26.91;
                invoicesModel.credits_applied = (float)22.43;
                invoicesModel.tax_amount_withheld = 0;
                invoicesModel.balance = 0;
                invoicesModel.write_off_amount = 0;
                invoicesModel.allow_partial_payments = true;
                invoicesModel.price_precision = 2;

                invoicesModel.payment_options.payment_gateways = payment_Gateways;

                invoicesModel.is_emailed = false;
                invoicesModel.reminders_sent = 1;
                invoicesModel.notes = "Looking forward for your business.";
                invoicesModel.terms = "Terms & Conditions apply";
                invoicesModel.can_send_in_mail = true;
                invoicesModel.callback = "waapi.info-seed.com";
                invoicesModel._return = "waapi.info-seed.com";

                if (invoicesModel == null)
                    return new UpdateInvoicesModel();

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://invoice.zoho.com/api/v3/invoices");
                request.Headers.Add("Authorization", "Bearer " + accessTokenModel.access_token);
                //request.Headers.Add("Cookie", "BuildCookie_800001748=1; 0d082fb755=92b70414bd68d4fed9777b061c51b27c; JSESSIONID=F542B11D7944C39A9028841B023F98BA; _zcsr_tmp=60af00d0-c025-49ca-b7f7-52fe7ef6c7a8; zbcscook=60af00d0-c025-49ca-b7f7-52fe7ef6c7a8; JSESSIONID=A515273556929085573B6EBF60B9EADF");
                var content = new StringContent(invoicesModel.ToJsonString());
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return new UpdateInvoicesModel();
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    RefreshToken();
                    return await InvoicesCreateAsync(createInvoicesDashbordModel);
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var rez = JsonConvert.DeserializeObject<InvoicesModel>(await response.Content.ReadAsStringAsync());

                    rez.invoices = rez.invoices.Where(x => x.status != "draft").ToArray();

                    return new UpdateInvoicesModel();
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var ModelResult = JsonConvert.DeserializeObject<UpdateInvoicesModel>(responseContent);

                    // You can work with objWhatsAppAnalytic here
                    if (ModelResult.invoice != null)
                    {
                        var clients = new HttpClient();
                        var requests = new HttpRequestMessage(HttpMethod.Post, "https://invoice.zoho.com/api/v3/invoices/"+ ModelResult.invoice.invoice_id + "/status/sent");
                        requests.Headers.Add("Authorization", "Bearer " + accessTokenModel.access_token);
                        var responses = await client.SendAsync(requests);
                        responses.EnsureSuccessStatusCode();

                        //var responseContents = await responses.Content.ReadAsStringAsync();
                        //var ModelResults = JsonConvert.DeserializeObject<UpdateInvoicesModel>(responseContents);
                        if (responses == null) 
                            return new UpdateInvoicesModel();


                        return ModelResult;
                    }
                    return new UpdateInvoicesModel();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

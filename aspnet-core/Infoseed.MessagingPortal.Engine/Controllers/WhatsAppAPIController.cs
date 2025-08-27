using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Abp.Web.Models;
using Azure;
using Azure.Communication.Email;
using Framework.Data;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Framework.Integration.Model;
using Hangfire.Storage;
using Infoseed.MessagingPortal.Engine.Model;
using Infoseed.MessagingPortal.ExtraOrderDetails;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.OrderDetails;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Dashboard;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Driver;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using PayPalCheckoutSdk.Payments;
using RestSharp;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.TwiML;
using WebJobEntities;
using static Framework.Integration.Model.CreateContactMg;
using static Infoseed.MessagingPortal.Engine.Model.SallaInfoModel;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;
using CosmosSql = Microsoft.Azure.Documents;
using SqlClient = System.Data.SqlClient;

namespace Infoseed.MessagingPortal.Engine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppAPIController : MessagingPortalControllerBase
    {
        private ILiveChatAppService _iliveChat;
        //private string fbToken = "EAAKTbPZAaKtYBAGxYaZAa3JckCvl7QDd1wbK8w9MNrwjsvMkUDKBFDoHKVKgE9JaYNkUc4C1IvdxQgn73nLPQ81zW6bhbfflnfZC2xpG7ofzGqP2T7YXCSu7LbWccPVJVdafiCFw5UnyikowudxmYW9VLzEcvbobqlW4ZBqe47IUHid3IbZBC6SmC9GyiJBGkrV1cAmuZAEQZDZD";
        //private string postUrl = "https://graph.facebook.com/v17.0/103674912368849/messages";
        public string URLG = "https://4d715913baa1.ngrok-free.app";
        //public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindbqa.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";

        //  public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindb.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";
        IDBService _dbService;
        // private IHubContext<SignalR.TeamInboxHub> _hub;
        public IContactsAPI _contactsAPI;
        public ITicketsAPI _ticketsAPI;
        private readonly ICacheManager _cacheManager;

        private readonly IDocumentClient _IDocumentClient;
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        private readonly IOrdersAppService _IOrdersAppService;
        private TelemetryClient _telemetry;

        public WhatsAppAPIController(
            IDBService dbService
              , ICacheManager cacheManager
            , IDocumentClient iDocumentClient
            , ILiveChatAppService iliveChat
            , TenantDashboardAppService tenantDashboardAppService
            , IOrdersAppService IOrdersAppService
             ,TelemetryClient telemetry

           // IHubContext<SignalR.TeamInboxHub> hub
           )
        {
            //   _hub = hub;
            _dbService = dbService;
            _contactsAPI = new ContactsAPI(SettingsModel.MgUrl, SettingsModel.MgKey);
            _cacheManager = cacheManager;
            _IDocumentClient = iDocumentClient;
            _iliveChat = iliveChat;
            _tenantDashboardAppService = tenantDashboardAppService;
            _IOrdersAppService = IOrdersAppService;
            _telemetry = telemetry;

        }
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing");
            }
            using var client = new HttpClient();

            var clientId = "81d603b4-a742-48fb-98e6-0efb00c07088";
            var clientSecret = "5aadbe9baf4b2ad207a3a9ef6bd40fe0";
            var redirectUri = "https://infoseedengineapi-stg.azurewebsites.net/api/WhatsAppAPI/callback";

            var tokenRequest = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"redirect_uri", redirectUri}
            };

            var requestContent = new FormUrlEncodedContent(tokenRequest);
            var response = await client.PostAsync("https://accounts.salla.sa/oauth2/token", requestContent);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, responseContent);
            }

            var tokenResponse = JsonConvert.DeserializeObject<SallaTokenResponse>(responseContent);

            // You can store the token securely (e.g., in DB)
            return Ok(tokenResponse);
        }


        [HttpGet("TestAddMongoDBAsync")]
        public async Task<string> TestAddMongoDBAsync(string campaignId)
        {
            try
            {


                // Connection string from Azure Cosmos DB for MongoDB (vCore)
                string connectionString = "mongodb+srv://infoseed:P%40ssw0rd@campagindb.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";// AppSettingsModel.connectionStringMongoDB;

                // MongoDB database and collection details

                string databaseName = AppSettingsModel.databaseName;
                string collectionName = AppSettingsModel.collectionName;


                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Get the database
                var database = client.GetDatabase(databaseName);

                // Get the collection
                var collection = database.GetCollection<CampaginMDRez>(collectionName);


                // Build the filter
                var filter = Builders<CampaginMDRez>.Filter.Eq(x => x.campaignId, campaignId);

                try
                {
                    // Find the first matching document
                    var filterResult = await collection.Find(filter).ToListAsync();

                    if (filterResult != null)
                    {

                    }
                    else
                    {
                        Console.WriteLine("No document found with the specified campaignId.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }




            }
            catch (Exception ex)
            {

            }

            return "";
        }

        private async Task<string> TestUpdateMongoDBAsync(WebHookModel model, DocumentCosmoseDB<CustomerModel> documentCosmoseDB)
        {
            try
            {


                string PhoneNumber = model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
                var status = model.whatsApp.Entry[0].Changes[0].Value.statuses[0].status;
                var jsonResult = JsonConvert.SerializeObject(model.whatsApp, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var MassageId = model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id;
                var TenantId = model.tenant.TenantId;
                var StatusCode = (status == "failed" ? 400 : 200);
                var FaildDetails = "";
                var DetailsJosn = jsonResult;








                // Connection string from Azure Cosmos DB for MongoDB (vCore)
                string connectionString = AppSettingsModel.connectionStringMongoDB;

                // MongoDB database and collection details
                string databaseName = AppSettingsModel.databaseName;
                string collectionName = AppSettingsModel.collectionName;

                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Get the database
                var database = client.GetDatabase(databaseName);

                // Get the collection
                var collection = database.GetCollection<CampaginMDRez>(collectionName);



                var filter = Builders<CampaginMDRez>.Filter.Eq(x => x.messageId, MassageId);
                var filterresult = await collection.Find(filter).FirstOrDefaultAsync();


                if (filterresult == null)//Insert
                {
                    // 1. Add a document
                    var newDocument = new CampaginMDRez
                    {
                        tenantId = TenantId.Value,
                        campaignId = "1",
                        phoneNumber = PhoneNumber,
                        messageId = MassageId,
                        status = status,
                        statusCode = StatusCode,
                        failedDetails = "",
                        is_accepted = false,
                        is_delivered = (status == "delivered" ? true : false),
                        is_read = (status == "read" ? true : false),
                        is_sent = (status == "sent" ? true : false),
                        delivered_detailsJson = (status == "delivered" ? DetailsJosn : ""),
                        read_detailsJson = (status == "read" ? DetailsJosn : ""),
                        sent_detailsJson = (status == "sent" ? DetailsJosn : ""),
                    };


                    await collection.InsertOneAsync(newDocument);

                }
                else//update
                {

                    // CampaginMDRez model = JsonConvert.DeserializeObject<CampaginMDRez>(filterresult);

                    if (status == "sent")
                    {

                        var update = Builders<CampaginMDRez>.Update
                            .Set(x => x.status, status)
                            .Set(x => x.is_sent, true)
                            .Set(x => x.sent_detailsJson, jsonResult)
                            .Set(x => x.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);







                        try
                        {
                            var camp = GetCampaignFun(long.Parse(filterresult.campaignId)).FirstOrDefault();


                            model.customer.templateId = camp.templateId.ToString();
                            model.customer.CampaignId = filterresult.campaignId;
                            model.customer.IsTemplateFlow = true;
                            model.customer.TemplateFlowDate = DateTime.UtcNow;
                            model.customer.getBotFlowForViewDto = new BotFlow.Dtos.GetBotFlowForViewDto();

                            var Result = await documentCosmoseDB.UpdateItemAsync(model.customer._self, model.customer);



                            string type = "";
                            string mediaUrl = "";
                            string msg = prepareMessageTemplateText(camp.model, out type, out mediaUrl);

                            // show campaign on teamInbox
                            var itemsCollectionCh = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                            CustomerChat customerChat = new CustomerChat()
                            {
                                messageId = MassageId,
                                TenantId = TenantId.Value,
                                userId = model.customer.userId,
                                text = msg,
                                type = "text",
                                CreateDate = DateTime.Now,
                                status = (int)Messagestatus.New,
                                sender = MessageSenderType.TeamInbox,
                                ItemType = InfoSeedContainerItemTypes.ConversationItem,
                                // mediaUrl = campaignCosmo.mediaUrl,
                                UnreadMessagesCount = 0,
                                agentName = "admin",
                                agentId = "",
                            };

                            var Resultchat = await itemsCollectionCh.CreateItemAsync(customerChat);
                        }
                        catch
                        {


                        }





                    }
                    else if (status == "delivered")
                    {


                        var update = Builders<CampaginMDRez>.Update
                            .Set(x => x.status, status)
                            .Set(x => x.is_delivered, true)
                            .Set(x => x.is_sent, true)
                            .Set(x => x.delivered_detailsJson, jsonResult)
                            //   .Set(x => x.sent_detailsJson, jsonResult)
                            .Set(x => x.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);

                    }
                    else if (status == "read")
                    {


                        var update = Builders<CampaginMDRez>.Update
                            .Set(x => x.status, status)
                            .Set(x => x.is_read, true)
                            .Set(x => x.is_delivered, true)
                            .Set(x => x.is_sent, true)
                            .Set(x => x.read_detailsJson, jsonResult)
                            // .Set(x => x.delivered_detailsJson, jsonResult)
                            //.Set(x => x.sent_detailsJson, jsonResult)
                            .Set(x => x.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);
                        var x = _dbService.UpdateCustomerChatStatusNew(model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id, TenantId.Value);
                    }
                    else
                    {

                        if (model.whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors.Count > 0)
                        {
                            foreach (var error in model.whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors)
                            {
                                var update = Builders<CampaginMDRez>.Update
                                    .Set(x => x.statusCode, error.code)
                                    .Set(x => x.failedDetails, error.error_data.details.ToString())
                                    .Set(x => x.updatedAt, DateTime.UtcNow);

                                var result = await collection.UpdateOneAsync(filter, update);


                            }
                        }







                    }


                }




            }
            catch (Exception ex)
            {

            }

            return "";
        }
        private static string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type, out string mediaUrl)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                mediaUrl = "";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {


                            type = item.format.ToLower();

                            if (type == "document")
                            {


                                if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                {
                                    if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                    {

                                        var media = objWhatsAppTemplateModel.mediaLink.Split(",")[1];
                                        try
                                        {
                                            type = "application";
                                            result += media + "\n\r";

                                            mediaUrl = media;
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    else
                                    {
                                        try
                                        {
                                            type = "application";
                                            result += objWhatsAppTemplateModel.mediaLink + "\n\r";

                                            mediaUrl = objWhatsAppTemplateModel.mediaLink;
                                        }
                                        catch
                                        {

                                        }

                                    }




                                }
                                else
                                {
                                    try
                                    {
                                        type = "application";
                                        result += item.example.header_handle[0] + "\n\r";

                                        mediaUrl = item.example.header_handle[0];
                                    }
                                    catch
                                    {

                                    }


                                }



                            }

                        }
                        if (item.type.Equals("BUTTONS"))
                        {
                            for (int i = 0; i < item.buttons.Count; i++)
                            {
                                result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
                            }
                        }
                        result += item.text;

                    }

                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static List<CampaginModel> GetCampaignFun(long CampaignId)
        {
            try
            {
                var SP_Name = "GetSendCampaignNowById";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId", CampaignId)
                };
                List<CampaginModel> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<CampaginModel>();
            }
        }
        private static CampaginModel MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                CampaginModel model = new CampaginModel
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),

                };

                try
                {

                    model.model = System.Text.Json.JsonSerializer.Deserialize<MessageTemplateModel>(SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"));
                }
                catch
                {


                }
                try
                {

                    model.templateVariablles = System.Text.Json.JsonSerializer.Deserialize<TemplateVariablles>(SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"));
                }
                catch
                {
                    model.templateVariablles = new TemplateVariablles();

                }

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new CampaginModel();
            }
        }



        [HttpGet("DeleteCache")]
        [DontWrapResult]
        public string ClearCache(string phoneNumberId, string apiKey)
        {
            if (apiKey.Equals("oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges"))
            {
                _cacheManager.GetCache("CacheTenant").Remove(phoneNumberId.ToString());
                return "Done";

            }
            else
                return apiKey;
        }

        [HttpGet("ChackCosmose")]
        [DontWrapResult]
        public string ChackCosmoseAsync(string userId, int tenantId)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
            var Customer = customerResult.Result;
            return "";
        }

        [HttpPost("SallaWebhook")]
        public async void SallaWebhook(dynamic jsonData2)
        {
            string jsonData23 = JsonConvert.SerializeObject(jsonData2);

            var aaaa = JsonConvert.SerializeObject(jsonData2);
            try
            {
                SallaModel jsonData = JsonConvert.DeserializeObject<SallaModel>(aaaa);

                AddToSallaLog(jsonData.merchant.ToString(), jsonData23);
                TenantModel Tenant = await _dbService.GetTenantByMerchantId(jsonData.merchant.ToString());

                if (jsonData._event == "app.store.authorize")
                {
                    AddOrUpdateSallaToken(jsonData.merchant.ToString(), jsonData.data.access_token, jsonData.data.refresh_token);

                    var info = await SendEmailAsync(jsonData.data.access_token);

                    await SendEmailtoInfoseed(jsonData.merchant.ToString(), jsonData.data.access_token, jsonData.data.refresh_token, info);


                }
                else if (jsonData._event == "abandoned.cart" || jsonData._event == "abandoned.cart.update")
                {

                    try
                    {
                        var model = GetTenantById(Tenant.TenantId.Value).FirstOrDefault();
                        //var UserModel = GetUserByName("admin", Tenant.TenantId.Value);
                        // var TemplatesInfo = GetTemplatesInfo("sallasend", Tenant.TenantId.Value);

                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                        request.Headers.Add("accept", "text/plain");
                        request.Headers.Add("tenancy", model.TenancyName);
                        request.Headers.Add("username", "admin");
                        request.Headers.Add("password", "123qwee");
                        request.Headers.Add("templateName", "salla_message");
                        var content = new StringContent("{\"reciverPhoneNumber\":\"" + jsonData.data.customer.mobile + "\",\"reciverName\":\"FromSalla" + jsonData.data.customer.name + "\",\"mssageContent\":\"" + Tenant.MenuReminderMessage + "\",\"isPDF\":false,\"linkPDF\":\"\",\"fileName\":\"\"}", null, "application/json-patch+json");

                        request.Content = content;
                        var response = await client.SendAsync(request);
                        string responseBody = await response.Content.ReadAsStringAsync();

                        response.EnsureSuccessStatusCode();
                        Console.WriteLine(await response.Content.ReadAsStringAsync());

                    }
                    catch
                    {

                        AddToSallaLog("error", jsonData23);

                    }


                }
                else if (jsonData._event == "order.created")
                {
                    var model = GetTenantById(Tenant.TenantId.Value).FirstOrDefault();

                    if (jsonData.data.payment_method == "cod")
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                    
                        request.Headers.Add("accept", "text/plain");
                        request.Headers.Add("tenancy", model.TenancyName);
                        request.Headers.Add("username", "admin");
                        request.Headers.Add("password", "123qwee");
                        request.Headers.Add("templateName", "cod");

                        var content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            reciverPhoneNumber = jsonData.data.customer.mobile_code + jsonData.data.customer.mobile.ToString(),
                            reciverName = "FromSalla " + jsonData.data.customer.full_name.ToString(),
                            mssageContent = jsonData.data.customer.full_name.ToString(),
                            mssageContent2 = jsonData.data.reference_id.ToString(),
                            mssageContent3 = jsonData.data.amounts.total.amount.ToString(),
                            mssageContent16 = jsonData.data.id.ToString(),
                        }), Encoding.UTF8, "application/json");



                        request.Content = content;

                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();


                    }
                }
                else if (jsonData._event == "order.status.updated")
                {

                    try
                    {
                        var model = GetTenantById(Tenant.TenantId.Value).FirstOrDefault();

                        if (jsonData.data.order == null)
                        {
                            if (jsonData.data.customized.translations?.ar.name == "تم التوصيل"
                                  || jsonData.data.status == "تم التوصيل")
                            {
                                //send evaluation template
                                var client = new HttpClient();
                                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                                request.Headers.Add("accept", "text/plain");
                                request.Headers.Add("tenancy", model.TenancyName);
                                request.Headers.Add("username", "admin");
                                request.Headers.Add("password", "123qwee");
                                request.Headers.Add("templateName", "evaluation_2");
                                //var content = new StringContent("{\"reciverPhoneNumber\":\"" + jsonData.data.order.customer.mobile + "\",\"reciverName\":\"FromSalla" + jsonData.data.order.customer.name + "\",\"mssageContent\":\"" + jsonData.data.status + "\",\"isPDF\":false,\"linkPDF\":\"\",\"fileName\":\"\"}", null, "application/json-patch+json");
                                var content = new StringContent(JsonConvert.SerializeObject(new
                                {
                                    reciverPhoneNumber = jsonData.data.customer.mobile.ToString(),
                                    reciverName = "FromSalla" + jsonData.data.customer.full_name.ToString(),
                                    mssageContent = jsonData.data.customer.full_name.ToString(),
                                    mssageContent2 = jsonData.data.id.ToString(),
                                    mssageContent16 = jsonData.data.id.ToString()
                                }), Encoding.UTF8, "application/json");


                                request.Content = content;
                                var response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();
                                Console.WriteLine(await response.Content.ReadAsStringAsync());


                                //send invoice template
                                var invoice = await GetInvoice(jsonData.merchant, jsonData.data.order.id);
                                //var invoice2 = await GetInvoice(jsonData.merchant, jsonData.data.order.reference_id);

                                string url = "";
                                if (invoice != null)
                                {
                                    url = invoice.data.url;

                                }

                                client = new HttpClient();
                                request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");

                                request.Headers.Add("accept", "text/plain");
                                request.Headers.Add("tenancy", model.TenancyName);
                                request.Headers.Add("username", "admin");
                                request.Headers.Add("password", "123qwee");
                                request.Headers.Add("templateName", "invoice_2");

                                var contentJson = new
                                {
                                    reciverPhoneNumber = jsonData.data.customer.mobile.ToString(),
                                    reciverName = "FromSalla " + jsonData.data.customer.full_name,
                                    isPDF = true,
                                    linkPDF = url,
                                    fileName = "invoice.pdf"
                                };

                                string jsonContent = JsonConvert.SerializeObject(contentJson);
                                content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                                request.Content = content;

                                response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();
                            }

                            //var UserModel = GetUserByName("admin", Tenant.TenantId.Value);
                            // var TemplatesInfo = GetTemplatesInfo("sallasend", Tenant.TenantId.Value);
                            else
                            {
                                try
                                {
                                    //var UserModel = GetUserByName("admin", Tenant.TenantId.Value);
                                    // var TemplatesInfo = GetTemplatesInfo("sallasend", Tenant.TenantId.Value);
                                    var client = new HttpClient();
                                    var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                                    request.Headers.Add("accept", "text/plain");
                                    request.Headers.Add("tenancy", model.TenancyName);
                                    request.Headers.Add("username", "admin");
                                    request.Headers.Add("password", "123qwee");
                                    request.Headers.Add("templateName", "salla_status");
                                    var content = new StringContent("{\"reciverPhoneNumber\":\"" + jsonData.data.order.customer.mobile + "\",\"reciverName\":\"FromSalla" + jsonData.data.order.customer.name + "\",\"mssageContent\":\"" + jsonData.data.status + "\",\"isPDF\":false,\"linkPDF\":\"\",\"fileName\":\"\"}", null, "application/json-patch+json");

                                    request.Content = content;
                                    var response = await client.SendAsync(request);
                                    response.EnsureSuccessStatusCode();
                                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                                }
                                catch
                                {

                                    AddToSallaLog("error", jsonData23);

                                }

                            }
                        }
                        else
                        {
                            if (jsonData.data.customized.translations?.ar.name == "تم التوصيل"
                                    || jsonData.data.status == "تم التوصيل")
                            {
                                var client = new HttpClient();
                                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                                request.Headers.Add("accept", "text/plain");
                                request.Headers.Add("tenancy", model.TenancyName);
                                request.Headers.Add("username", "admin");
                                request.Headers.Add("password", "123qwee");
                                request.Headers.Add("templateName", "evaluation_2");
                                var content = new StringContent(JsonConvert.SerializeObject(new
                                {
                                    reciverPhoneNumber = jsonData.data.order.customer.mobile,
                                    reciverName = "FromSalla" + jsonData.data.order.customer.name,
                                    mssageContent = jsonData.data.order.customer.name,
                                    mssageContent2 = jsonData.data.order.id.ToString(),
                                    mssageContent16 = jsonData.data.order.reference_id.ToString()
                                }), Encoding.UTF8, "application/json");
                                request.Content = content;
                                var response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();

                                var invoice = await GetInvoice(jsonData.merchant, jsonData.data.order.id);

                                string url = "";
                                if (invoice != null)
                                {
                                    url = invoice.data.url;

                                }

                                client = new HttpClient();
                                request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");

                                request.Headers.Add("accept", "text/plain");
                                request.Headers.Add("tenancy", model.TenancyName);
                                request.Headers.Add("username", "admin");
                                request.Headers.Add("password", "123qwee");
                                request.Headers.Add("templateName", "invoice_2");

                                var contentJson = new
                                {
                                    reciverPhoneNumber = jsonData.data.order.customer.mobile,
                                    reciverName = "FromSalla " + jsonData.data.order.customer.name,
                                    isPDF = true,
                                    linkPDF = url,
                                    fileName = "invoice.pdf"
                                };

                                string jsonContent = JsonConvert.SerializeObject(contentJson);
                                content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                                request.Content = content;

                                response = await client.SendAsync(request);
                                response.EnsureSuccessStatusCode();
                            }

                            else
                            {
                                try
                                {

                                    var client = new HttpClient();
                                    var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");
                                    request.Headers.Add("accept", "text/plain");
                                    request.Headers.Add("tenancy", model.TenancyName);
                                    request.Headers.Add("username", "admin");
                                    request.Headers.Add("password", "123qwee");
                                    request.Headers.Add("templateName", "salla_status");
                                    var content = new StringContent("{\"reciverPhoneNumber\":\"" + jsonData.data.order.customer.mobile + "\",\"reciverName\":\"FromSalla" + jsonData.data.order.customer.name + "\",\"mssageContent\":\"" + jsonData.data.status + "\",\"isPDF\":false,\"linkPDF\":\"\",\"fileName\":\"\"}", null, "application/json-patch+json");

                                    request.Content = content;
                                    var response = await client.SendAsync(request);
                                    response.EnsureSuccessStatusCode();
                                    Console.WriteLine(await response.Content.ReadAsStringAsync());

                                }
                                catch
                                {

                                    AddToSallaLog("error", jsonData23);

                                }

                            }
                        }

                    }
                    catch
                    {

                        AddToSallaLog("error", jsonData23);

                    }
                }
                //else if (jsonData._event == "product.updated")
                //{
                //    var productData = jsonData.data;

                //    if (productData != null)
                //    {
                //        int quantity = productData.product.quantity;
                //        bool isAvailable = productData.product.is_available;
                //        string productName = productData.product.name;

                //        if (quantity > 0 && isAvailable)
                //        {
                //            var model = GetTenantById(Tenant.TenantId.Value).FirstOrDefault();

                //            var messageContent = $"📦 المنتج {productName} أصبح متوفر الآن! أسرع بالشراء.";

                //            var client = new HttpClient();
                //            var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedintegrationserverprod.azurewebsites.net/api/SendCampaign/SendMessage");

                //            request.Headers.Add("accept", "text/plain");
                //            request.Headers.Add("tenancy", model.TenancyName);
                //            request.Headers.Add("username", "admin");
                //            request.Headers.Add("password", "123qwee");
                //            request.Headers.Add("templateName", "item_back_in_stock");

                //            var contentJson = new
                //            {
                //                reciverPhoneNumber = "رقم الزبون",
                //                reciverName = "عميلنا العزيز",
                //                mssageContent = jsonData.data.customer.name,
                //                mssageContent2 = messageContent,
                //                isPDF = false,
                //                linkPDF = "",
                //                fileName = ""
                //            };

                //            string jsonContent = JsonConvert.SerializeObject(contentJson);
                //            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //            request.Content = content;
                //            var response = await client.SendAsync(request);
                //            response.EnsureSuccessStatusCode();
                //        }
                //    }
                //}

            }
            catch
            {
                AddToSallaLog("error", jsonData23);
            }

        }



        static async Task<InvoiceResponse> GetInvoice(long merchantId, long orderId)
        {
            var accessTokens = GetAccessTokenByMerchantID(merchantId);

            if (accessTokens == null || accessTokens.Count == 0)
            {
                Console.WriteLine("No access token found for this merchant ID.");
                return null;
            }

            var accessToken = accessTokens[0];

            using (var client = new HttpClient())
            {
                var requestUrl = $"https://api.salla.dev/admin/v2/orders/{orderId}/print-invoice";

                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON string to InvoiceResponse object
                var invoiceResponse = System.Text.Json.JsonSerializer.Deserialize<InvoiceResponse>(responseBody);

                return invoiceResponse;
            }
        }


        public static SallaTokenResponse MapAccessToken(IDataReader dataReader)
        {
            try
            {
                SallaTokenResponse SallaToken = new SallaTokenResponse
                {
                    AccessToken = SqlDataHelper.GetValue<string>(dataReader, "Access_token"),
                    RefreshToken = SqlDataHelper.GetValue<string>(dataReader, "Refresh_token")
                };

                return SallaToken;
            }
            catch (Exception ex)
            {
                // Optionally log ex
                throw;
            }
        }

        private static List<SallaTokenResponse> GetAccessTokenByMerchantID(long merchantId)
        {
            var SP_Name = Constants.Salla.SP_GetSallaTokenByMerchantId;

            var sqlParameters = new List<SqlClient.SqlParameter>
                {
                    new SqlClient.SqlParameter("@MerchantId", merchantId)
                };

            return SqlDataHelper.ExecuteReader(
                SP_Name,
                sqlParameters.ToArray(),
                MapAccessToken,
                AppSettingsModel.ConnectionStrings
            ).ToList();
        }



        //public byte[] GenerateInvoice(string customerName, string mobile, List<(string productName, int quantity, double price)> items)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        var document = new PdfDocument();
        //        var page = document.AddPage();
        //        var gfx = XGraphics.FromPdfPage(page);
        //        var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
        //        var fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
        //        var fontBody = new XFont("Arial", 12, XFontStyle.Regular);

        //        int yPoint = 40;

        //        // Title
        //        gfx.DrawString("INVOICE / فاتورة", fontTitle, XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopCenter);
        //        yPoint += 40;

        //        // Store and Date
        //        gfx.DrawString($"Store: Demo Store", fontBody, XBrushes.Black, new XPoint(40, yPoint));
        //        gfx.DrawString($"Date: {DateTime.Now:yyyy-MM-dd}", fontBody, XBrushes.Black, new XPoint(350, yPoint));
        //        yPoint += 25;

        //        // Customer
        //        gfx.DrawString($"Customer: {customerName}", fontBody, XBrushes.Black, new XPoint(40, yPoint));
        //        gfx.DrawString($"Mobile: {mobile}", fontBody, XBrushes.Black, new XPoint(40, yPoint + 20));
        //        yPoint += 50;

        //        // Table Headers
        //        gfx.DrawString("Product", fontHeader, XBrushes.Black, new XPoint(40, yPoint));
        //        gfx.DrawString("Qty", fontHeader, XBrushes.Black, new XPoint(250, yPoint));
        //        gfx.DrawString("Price", fontHeader, XBrushes.Black, new XPoint(300, yPoint));
        //        gfx.DrawString("Total", fontHeader, XBrushes.Black, new XPoint(400, yPoint));
        //        yPoint += 20;

        //        double grandTotal = 0;

        //        foreach (var item in items)
        //        {
        //            double total = item.price * item.quantity;
        //            grandTotal += total;

        //            gfx.DrawString(item.productName, fontBody, XBrushes.Black, new XPoint(40, yPoint));
        //            gfx.DrawString(item.quantity.ToString(), fontBody, XBrushes.Black, new XPoint(250, yPoint));
        //            gfx.DrawString($"{item.price:0.00}", fontBody, XBrushes.Black, new XPoint(300, yPoint));
        //            gfx.DrawString($"{total:0.00}", fontBody, XBrushes.Black, new XPoint(400, yPoint));
        //            yPoint += 20;
        //        }

        //        yPoint += 20;
        //        gfx.DrawLine(XPens.Black, 40, yPoint, 500, yPoint);
        //        yPoint += 20;

        //        gfx.DrawString($"Total: {grandTotal:0.00} SAR", fontHeader, XBrushes.Black, new XPoint(350, yPoint));
        //        yPoint += 40;

        //        gfx.DrawString("Thank you for your purchase! / شكراً لتسوقك معنا", fontBody, XBrushes.Black, new XPoint(40, yPoint));

        //        document.Save(stream, false);
        //        return stream.ToArray();
        //    }
        //}

        private static List<TenantModel> GetTenantById(long id)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where Id =" + id;


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
                    // Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenancyName = dataSet.Tables[0].Rows[i]["TenancyName"].ToString()
                });

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private TemplatesInfo GetTemplatesInfo(string templatesName, int tenantId)
        {
            try
            {
                TemplatesInfo templatesInfo = new TemplatesInfo();
                var SP_Name = Constants.WhatsAppTemplates.SP_IntegrationGetTemplatesInfo;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@templatesName",templatesName),
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                templatesInfo = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplatesIslamicInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return templatesInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Tenants.Dashboard.Dto.UsersDashModel GetUserByName(string UserName, int tenantId)
        {
            Tenants.Dashboard.Dto.UsersDashModel UserInfoDto = new Tenants.Dashboard.Dto.UsersDashModel();
            try
            {
                var SP_Name = Constants.User.SP_GetUserByName;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                     new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };

                UserInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapUserInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return UserInfoDto;
            }
            catch
            {
                return UserInfoDto;
            }
        }



        [HttpGet("webhook")]
        [DontWrapResult]
        public string Webhook(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.challenge")] string challenge,
        [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            string currentDattime = "InfoSeed-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            if (verify_token.Equals(currentDattime) && mode.Equals("subscribe"))
            {
                return challenge;
            }
            else
            {
                return null;
            }
        }


        [HttpPost("webhook")]
        public async void Webhook(dynamic jsonData2)
        {

            try
            {
                //return;

                var aaaa = JsonConvert.SerializeObject(jsonData2);
                WhatsAppModel jsonData = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);

                //testtAsync(jsonData);
                //return;


                string userId = string.Empty;
                var phoneNumberId = jsonData.Entry[0].Changes[0].Value.Metadata.phone_number_id;

                TenantModel Tenant = new TenantModel();

                var objTenant = _cacheManager.GetCache("CacheTenant").Get(phoneNumberId.ToString(), cache => cache);
                if (objTenant.Equals(phoneNumberId.ToString()))
                {
                    Tenant = await _dbService.GetTenantByKey("", phoneNumberId.ToString());
                    _cacheManager.GetCache("CacheTenant").Set(phoneNumberId.ToString(), Tenant);
                }
                else
                {
                    Tenant = (TenantModel)objTenant;
                }




                if (jsonData.Entry[0].Changes[0].Value.Messages == null && jsonData.Entry[0].Changes[0].Value.statuses == null)
                {
                    return;
                }


                //try
                //{
                //    if (Tenant.TenantId.Value==27)
                //    {

                //        MoveToStg(jsonData);
                //        return;
                //    }

                //}
                //catch
                //{

                //}

                //try
                //{
                //    if (Tenant.TenantId.Value==156)
                //    {

                //        MoveToQa(jsonData);
                //        return;
                //    }

                //}
                //catch
                //{

                //}





                if (jsonData.Entry[0].Changes[0].Value.statuses != null)
                {
                    var from2 = jsonData.Entry[0].Changes[0].Value.statuses[0].recipient_id; // extract the phone number from the webhook payload

                    WebHookModel model = new WebHookModel();

                    model.tenant=Tenant;
                    model.whatsApp=jsonData;
                    SetStatusInQueue(model);
                    //SetStatusInQueuestg(model);

                    return;

                    //try
                    //{
                    //    if (from2=="962779746365")
                    //    {

                    //        testtAsync(jsonData);
                    //        return;
                    //    }

                    //}
                    //catch
                    //{

                    //}

                    //await Task.Delay(2000);

                    //if (from2=="962779746365"|| from2=="962795735683")
                    //{

                    //    testtAsync(jsonData);
                    //    return;
                    //}







                    if (Tenant.IsBundleActive)
                    {
                        string PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
                        var name = PhoneNumber;
                        if (jsonData.Entry[0].Changes[0].Value.Contacts != null)
                        {

                            name = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;
                        }
                        userId = (Tenant.TenantId + "_" + PhoneNumber).ToString();
                        var status = jsonData.Entry[0].Changes[0].Value.statuses[0].status;


                        //var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        //var campaignCosmoResult = campaignCosmoDBModel.GetItemAsync(a => a.itemType == 5 && a.messagesId == jsonData.Entry[0].Changes[0].Value.statuses[0].id  && a.tenantId == Tenant.TenantId);
                        //var campaignCosmo = campaignCosmoResult.Result;

                        var itemsCollection3 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                        var Customer3 = await customerResult3;



                        if (status == "sent")
                        {
                            if (Customer3 == null)
                            {

                                Customer3 = _dbService.CreateNewCustomer(PhoneNumber, name, "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                                Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");

                                //Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                //Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;

                                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                                var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                            }
                            else
                            {
                                Customer3.channel = "Whatsapp";

                                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;

                                // Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                // Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;
                                // Get current UTC time as Unix timestamp
                                //Customer3.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                                //// Add 1 day (86400 seconds) to the creation timestamp
                                //Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                                if (Customer3.IsBlock)
                                {
                                    return;
                                }
                                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                                Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());

                                try
                                {
                                    if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                                    {
                                        Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                        Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                                    }
                                }
                                catch
                                {
                                    Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                    Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                                }


                                try
                                {
                                    if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                                    {
                                        Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                        Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                                    }
                                }
                                catch
                                {
                                    Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                    Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                                }

                                if (string.IsNullOrEmpty(Customer3.ContactID))
                                {

                                    var getCustomer3 = _dbService.GetCustomerfromDB(PhoneNumber, "", "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                                    Customer3.ContactID = getCustomer3.Id.ToString();


                                }


                                var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                            }


                        }


                        try
                        {

                            if (jsonData.Entry[0].Changes[0].Value.statuses[0].status == "read" || jsonData.Entry[0].Changes[0].Value.statuses[0].status == "failed")
                            {

                                WebHookModel webHookModel = new WebHookModel();
                                webHookModel.tenant = Tenant;
                                webHookModel.whatsApp = jsonData;
                                webHookModel.customer = Customer3;
                                // SetStatusInQueuestg(webHookModel);
                                await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);
                            }
                            else
                            {
                                if (jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "marketing" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "utility" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToUpper() == "AUTHENTICATION")
                                {
                                    WebHookModel webHookModel = new WebHookModel();
                                    webHookModel.tenant = Tenant;
                                    webHookModel.whatsApp = jsonData;
                                    webHookModel.customer = Customer3;
                                    //SetStatusInQueuestg(webHookModel);
                                    await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);


                                }
                            }



                        }
                        catch
                        {

                        }

                    }
                    return;
                }

                if (jsonData.Entry[0].Changes[0].Value.Messages != null)
                {
                    if (jsonData.Entry[0].Changes[0].Value.Messages[0].context != null)
                    {

                        ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
                        {
                            TenantId = Tenant.TenantId.Value,
                            MessageStatusId = 4,
                            MessageDateTime = DateTime.Now,
                            MessageId = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id

                        };
                        SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

                        var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;
                    }



                }


                var name2 = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;




                var massageId = jsonData.Entry[0].Changes[0].Value.Messages[0].Id;
                var from = jsonData.Entry[0].Changes[0].Value.Messages[0].From; // extract the phone number from the webhook payload
                string medaiUrl = string.Empty;
                userId = (Tenant.TenantId + "_" + from).ToString();
                var type = jsonData.Entry[0].Changes[0].Value.Messages[0].Type;
                string interactiveId = string.Empty;



                //if (from=="962779746365")
                //{

                //    testtAsync(jsonData);
                //    return;
                //}
                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();



                string msgBody = whatsAppAppService.MassageTypeText(jsonData.Entry[0].Changes[0].Value.Messages, Tenant.AccessToken, tAttachments, ref medaiUrl, attachmentMessageModel, out interactiveId);



                if (jsonData.Entry[0].Changes[0].Value.Messages[0].button != null)
                {
                    msgBody = jsonData.Entry[0].Changes[0].Value.Messages[0].button.text;

                }




                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                var Customer = await customerResult;



                if (Customer == null)
                {
                    Customer = _dbService.CreateNewCustomer(from, name2, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                    Customer.customerChat.text = msgBody;

                }

                Customer.avatarUrl = "avatar3";
                Customer.channel = "Whatsapp";

                if (Customer.expiration_timestamp != 0)
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Check if the expiration time has passed
                    if (currentTimestamp > Customer.expiration_timestamp)
                    {
                        Customer.IsLockedByAgent = false;
                        Customer.IsOpen = false;
                        Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };
                        Customer.IsHumanhandover = false;
                        //Console.WriteLine("The time has expired.");
                    }
                    else
                    {
                        //Console.WriteLine("The time has not expired yet.");
                    }

                }




                if (Customer.CustomerStepModel == null)
                {
                    Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };

                }
                Customer.CustomerStepModel.UserParmeter.Remove("ContactID");
                Customer.CustomerStepModel.UserParmeter.Add("ContactID", Customer.ContactID.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("TenantId");
                Customer.CustomerStepModel.UserParmeter.Add("TenantId", Customer.TenantId.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                Customer.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer.phoneNumber.ToString());




                Customer.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Customer.expiration_timestamp = Customer.creation_timestamp + 86400;



                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Name");
                        Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Name");
                    Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                }


                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Location");
                        Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Location");
                    Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                }
                if (string.IsNullOrEmpty(Customer.displayName))
                {
                    Customer.displayName = name2;
                }

                if (Customer.IsBlock)
                {
                    return;
                }

                if (Tenant == null)
                {
                    return;
                }
                if (!Tenant.IsBundleActive)
                {
                    return;
                }
                if (!Tenant.IsBotActive)
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = from;
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = Tenant.MassageIfBotNotActive;
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        Content message = new Content();
                        message.text = Tenant.MassageIfBotNotActive;
                        message.agentName = Tenant.botId;
                        message.agentId = "1000000";
                        message.type = "text";
                        //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId, Customer);
                        //Customer.CreateDate = CustomerChat.CreateDate;
                        //Customer.customerChat = CustomerChat;


                        var CustomerSendChat2 = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, message.text, message.type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.TeamInbox, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);

                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        //_hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                        // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                    }
                    return;

                }
                if (!Tenant.IsPaidInvoice)
                {
                    return;
                }

                try
                {
                    TimeSpan timeSpan = DateTime.UtcNow - Customer.TemplateFlowDate.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours <= 24)
                    {


                    }
                    else
                    {
                        Customer.IsTemplateFlow = false;
                        Customer.templateId = "";
                    }

                }
                catch
                {
                    Customer.IsTemplateFlow = false;
                    Customer.templateId = "";

                }


                if (attachmentMessageModel.IsHasAttachment)
                {
                    try
                    {
                        await Sync(attachmentMessageModel, Customer, massageId);

                    }
                    catch
                    {


                    }
                    var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
                    attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                    //var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
                    //attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                    ////try
                    ////{
                    ////    await Sync(attachmentMessageModel, Customer, massageId);

                    ////}
                    ////catch
                    ////{


                    ////}


                    //SetAttachmentMessageInQueue(attachmentMessageModel);
                }
                else
                {

                    var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                    Customer.customerChat = CustomerSendChat;

                    //if (Customer.IsOpen && Customer.customerChat.sender==MessageSenderType.Customer)
                    //{

                    //    try
                    //    {
                    //        Customer.ConversationsCount = Customer.ConversationsCount+1;
                    //    }
                    //    catch
                    //    {
                    //        Customer.ConversationsCount = 1;
                    //    }


                    //}
                    //else
                    //{
                    //    Customer.ConversationsCount = 0;
                    //}
                    if (Customer.IsConversationExpired)
                    {

                        Customer.IsConversationExpired = false;
                    }

                }







                if (Tenant.IsWorkActive && !Customer.IsOpen)
                {
                    string SFormat = string.Empty;
                    /// out of working hours  
                    if (!checkIsInService(Tenant.workModel, out SFormat))
                    {
                        PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                        postWhatsAppMessageModel.type = "text";
                        postWhatsAppMessageModel.to = from;
                        postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                        postWhatsAppMessageModel.text.body = SFormat;
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {
                            Content message = new Content();
                            message.text = SFormat;
                            message.agentName = Tenant.botId;
                            message.agentId = "1000000";
                            message.type = "text";
                            var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                            Customer.CreateDate = CustomerChat.CreateDate;
                            Customer.customerChat = CustomerChat;
                            // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        }

                        return;
                    }
                }



                if (Customer.customerChat != null)
                {
                    if (Customer.IsOpen && Customer.customerChat.sender == MessageSenderType.Customer)
                    {

                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, true, Customer.ConversationsCount, Customer.creation_timestamp, Customer.expiration_timestamp);
                    }
                    else
                    {
                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, false, 0, Customer.creation_timestamp, Customer.expiration_timestamp);

                    }
                }



                if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
                {
                    await PrepareBotChatWithCustomer(from, Tenant, Customer, msgBody, tAttachments, interactiveId);

                }
                else
                {
                    SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                    return;
                }
                
                //if the customer message type is order then create a new order
                if (jsonData.Entry[0].Changes[0].Value.Messages[0].Type == WhatsContentTypeEnum.order.ToString())
                {
                    var msg = jsonData.Entry[0].Changes[0].Value.Messages[0];
                    var cusId = int.Parse(Customer.id);
                    var orderModel = MapOrder(Tenant.TenantId.Value, cusId, msg);

                    await CreateOrder(orderModel);
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }

        [HttpPost("WebhookTest")]
        public async void WebhookTest(dynamic jsonData2)
        {
            try
            {
                return;

                var aaaa = JsonConvert.SerializeObject(jsonData2);
                WhatsAppModel jsonData = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);

                //testtAsync(jsonData);
                //return;


                string userId = string.Empty;
                var phoneNumberId = jsonData.Entry[0].Changes[0].Value.Metadata.phone_number_id;

                TenantModel Tenant = new TenantModel();

                var objTenant = _cacheManager.GetCache("CacheTenant").Get(phoneNumberId.ToString(), cache => cache);
                if (objTenant.Equals(phoneNumberId.ToString()))
                {
                    Tenant = await _dbService.GetTenantByKey("", phoneNumberId.ToString());
                    _cacheManager.GetCache("CacheTenant").Set(phoneNumberId.ToString(), Tenant);
                }
                else
                {
                    Tenant = (TenantModel)objTenant;
                }




                if (jsonData.Entry[0].Changes[0].Value.Messages == null && jsonData.Entry[0].Changes[0].Value.statuses == null)
                {
                    return;
                }


                try
                {
                    if (Tenant.TenantId.Value==27)
                    {

                        MoveToStg(jsonData);
                        return;
                    }

                }
                catch
                {

                }

                try
                {
                    if (Tenant.TenantId.Value==156)
                    {

                        MoveToQa(jsonData);
                        return;
                    }

                }
                catch
                {

                }





                if (jsonData.Entry[0].Changes[0].Value.statuses != null)
                {
                    var from2 = jsonData.Entry[0].Changes[0].Value.statuses[0].recipient_id; // extract the phone number from the webhook payload

                    WebHookModel model = new WebHookModel();

                    model.tenant=Tenant;
                    model.whatsApp=jsonData;
                    SetStatusInQueue(model);
                    //SetStatusInQueuestg(model);

                    return;

                    //try
                    //{
                    //    if (from2=="962779746365")
                    //    {

                    //        testtAsync(jsonData);
                    //        return;
                    //    }

                    //}
                    //catch
                    //{

                    //}

                    //await Task.Delay(2000);

                    //if (from2=="962779746365"|| from2=="962795735683")
                    //{

                    //    testtAsync(jsonData);
                    //    return;
                    //}







                    if (Tenant.IsBundleActive)
                    {
                        string PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
                        var name = PhoneNumber;
                        if (jsonData.Entry[0].Changes[0].Value.Contacts != null)
                        {

                            name = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;
                        }
                        userId = (Tenant.TenantId + "_" + PhoneNumber).ToString();
                        var status = jsonData.Entry[0].Changes[0].Value.statuses[0].status;


                        //var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        //var campaignCosmoResult = campaignCosmoDBModel.GetItemAsync(a => a.itemType == 5 && a.messagesId == jsonData.Entry[0].Changes[0].Value.statuses[0].id  && a.tenantId == Tenant.TenantId);
                        //var campaignCosmo = campaignCosmoResult.Result;

                        var itemsCollection3 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                        var Customer3 = await customerResult3;



                        if (status == "sent")
                        {
                            if (Customer3 == null)
                            {

                                Customer3 = _dbService.CreateNewCustomer(PhoneNumber, name, "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                                Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");

                                //Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                //Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;

                                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                                var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                            }
                            else
                            {
                                Customer3.channel = "Whatsapp";

                                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;

                                // Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                                // Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;
                                // Get current UTC time as Unix timestamp
                                //Customer3.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                                //// Add 1 day (86400 seconds) to the creation timestamp
                                //Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                                if (Customer3.IsBlock)
                                {
                                    return;
                                }
                                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                                Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                                Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());

                                try
                                {
                                    if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                                    {
                                        Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                        Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                                    }
                                }
                                catch
                                {
                                    Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                    Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                                }


                                try
                                {
                                    if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                                    {
                                        Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                        Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                                    }
                                }
                                catch
                                {
                                    Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                    Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                                }

                                if (string.IsNullOrEmpty(Customer3.ContactID))
                                {

                                    var getCustomer3 = _dbService.GetCustomerfromDB(PhoneNumber, "", "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                                    Customer3.ContactID = getCustomer3.Id.ToString();


                                }


                                var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                            }


                        }


                        try
                        {

                            if (jsonData.Entry[0].Changes[0].Value.statuses[0].status == "read" || jsonData.Entry[0].Changes[0].Value.statuses[0].status == "failed")
                            {

                                WebHookModel webHookModel = new WebHookModel();
                                webHookModel.tenant = Tenant;
                                webHookModel.whatsApp = jsonData;
                                webHookModel.customer = Customer3;
                                // SetStatusInQueuestg(webHookModel);
                                await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);
                            }
                            else
                            {
                                if (jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "marketing" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "utility" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToUpper() == "AUTHENTICATION")
                                {
                                    WebHookModel webHookModel = new WebHookModel();
                                    webHookModel.tenant = Tenant;
                                    webHookModel.whatsApp = jsonData;
                                    webHookModel.customer = Customer3;
                                    //SetStatusInQueuestg(webHookModel);
                                    await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);


                                }
                            }



                        }
                        catch
                        {

                        }

                    }
                    return;
                }

                if (jsonData.Entry[0].Changes[0].Value.Messages != null)
                {
                    if (jsonData.Entry[0].Changes[0].Value.Messages[0].context != null)
                    {

                        ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
                        {
                            TenantId = Tenant.TenantId.Value,
                            MessageStatusId = 4,
                            MessageDateTime = DateTime.Now,
                            MessageId = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id

                        };
                        SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

                        var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;
                    }



                }


                var name2 = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;




                var massageId = jsonData.Entry[0].Changes[0].Value.Messages[0].Id;
                var from = jsonData.Entry[0].Changes[0].Value.Messages[0].From; // extract the phone number from the webhook payload
                string medaiUrl = string.Empty;
                userId = (Tenant.TenantId + "_" + from).ToString();
                var type = jsonData.Entry[0].Changes[0].Value.Messages[0].Type;
                string interactiveId = string.Empty;



                //if (from=="962779746365")
                //{

                //    testtAsync(jsonData);
                //    return;
                //}
                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();



                string msgBody = whatsAppAppService.MassageTypeText(jsonData.Entry[0].Changes[0].Value.Messages, Tenant.AccessToken, tAttachments, ref medaiUrl, attachmentMessageModel, out interactiveId);



                if (jsonData.Entry[0].Changes[0].Value.Messages[0].button != null)
                {
                    msgBody = jsonData.Entry[0].Changes[0].Value.Messages[0].button.text;

                }




                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                var Customer = await customerResult;



                if (Customer == null)
                {
                    Customer = _dbService.CreateNewCustomer(from, name2, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                    Customer.customerChat.text = msgBody;

                }

                Customer.avatarUrl = "avatar3";
                Customer.channel = "Whatsapp";

                if (Customer.expiration_timestamp != 0)
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Check if the expiration time has passed
                    if (currentTimestamp > Customer.expiration_timestamp)
                    {
                        Customer.IsLockedByAgent = false;
                        Customer.IsOpen = false;
                        Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };
                        Customer.IsHumanhandover = false;
                        //Console.WriteLine("The time has expired.");
                    }
                    else
                    {
                        //Console.WriteLine("The time has not expired yet.");
                    }

                }




                if (Customer.CustomerStepModel == null)
                {
                    Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };

                }
                Customer.CustomerStepModel.UserParmeter.Remove("ContactID");
                Customer.CustomerStepModel.UserParmeter.Add("ContactID", Customer.ContactID.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("TenantId");
                Customer.CustomerStepModel.UserParmeter.Add("TenantId", Customer.TenantId.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                Customer.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer.phoneNumber.ToString());




                Customer.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Customer.expiration_timestamp = Customer.creation_timestamp + 86400;



                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Name");
                        Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Name");
                    Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                }


                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Location");
                        Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Location");
                    Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                }
                if (string.IsNullOrEmpty(Customer.displayName))
                {
                    Customer.displayName = name2;
                }

                if (Customer.IsBlock)
                {
                    return;
                }

                if (Tenant == null)
                {
                    return;
                }
                if (!Tenant.IsBundleActive)
                {
                    return;
                }
                if (!Tenant.IsBotActive)
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = from;
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = Tenant.MassageIfBotNotActive;
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        Content message = new Content();
                        message.text = Tenant.MassageIfBotNotActive;
                        message.agentName = Tenant.botId;
                        message.agentId = "1000000";
                        message.type = "text";
                        //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId, Customer);
                        //Customer.CreateDate = CustomerChat.CreateDate;
                        //Customer.customerChat = CustomerChat;


                        var CustomerSendChat2 = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, message.text, message.type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.TeamInbox, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);

                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        //_hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                        // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                    }
                    return;

                }
                if (!Tenant.IsPaidInvoice)
                {
                    return;
                }

                try
                {
                    TimeSpan timeSpan = DateTime.UtcNow - Customer.TemplateFlowDate.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours <= 24)
                    {


                    }
                    else
                    {
                        Customer.IsTemplateFlow = false;
                        Customer.templateId = "";
                    }

                }
                catch
                {
                    Customer.IsTemplateFlow = false;
                    Customer.templateId = "";

                }


                if (attachmentMessageModel.IsHasAttachment)
                {
                    try
                    {
                        await Sync(attachmentMessageModel, Customer, massageId);

                    }
                    catch
                    {


                    }
                    var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
                    attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                    //var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
                    //attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                    ////try
                    ////{
                    ////    await Sync(attachmentMessageModel, Customer, massageId);

                    ////}
                    ////catch
                    ////{


                    ////}


                    //SetAttachmentMessageInQueue(attachmentMessageModel);
                }
                else
                {

                    var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                    Customer.customerChat = CustomerSendChat;

                    //if (Customer.IsOpen && Customer.customerChat.sender==MessageSenderType.Customer)
                    //{

                    //    try
                    //    {
                    //        Customer.ConversationsCount = Customer.ConversationsCount+1;
                    //    }
                    //    catch
                    //    {
                    //        Customer.ConversationsCount = 1;
                    //    }


                    //}
                    //else
                    //{
                    //    Customer.ConversationsCount = 0;
                    //}
                    if (Customer.IsConversationExpired)
                    {

                        Customer.IsConversationExpired = false;
                    }

                }







                if (Tenant.IsWorkActive && !Customer.IsOpen)
                {
                    string SFormat = string.Empty;
                    /// out of working hours  
                    if (!checkIsInService(Tenant.workModel, out SFormat))
                    {
                        PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                        postWhatsAppMessageModel.type = "text";
                        postWhatsAppMessageModel.to = from;
                        postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                        postWhatsAppMessageModel.text.body = SFormat;
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {
                            Content message = new Content();
                            message.text = SFormat;
                            message.agentName = Tenant.botId;
                            message.agentId = "1000000";
                            message.type = "text";
                            var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                            Customer.CreateDate = CustomerChat.CreateDate;
                            Customer.customerChat = CustomerChat;
                            // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        }

                        return;
                    }
                }



                if (Customer.customerChat != null)
                {
                    if (Customer.IsOpen && Customer.customerChat.sender == MessageSenderType.Customer)
                    {

                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, true, Customer.ConversationsCount, Customer.creation_timestamp, Customer.expiration_timestamp);
                    }
                    else
                    {
                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, false, 0, Customer.creation_timestamp, Customer.expiration_timestamp);

                    }
                }



                if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
                {
                    await PrepareBotChatWithCustomer(from, Tenant, Customer, msgBody, tAttachments, interactiveId);

                }
                else
                {
                    SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                    return;
                }

                //if the customer message type is order then create a new order
                if (jsonData.Entry[0].Changes[0].Value.Messages[0].Type == WhatsContentTypeEnum.order.ToString())
                {
                    var msg = jsonData.Entry[0].Changes[0].Value.Messages[0];
                    var cusId = int.Parse(Customer.id);
                    var orderModel = MapOrder(Tenant.TenantId.Value, cusId, msg);

                    await CreateOrder(orderModel);
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }


        private async Task testtAsync(WhatsAppModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, URLG + "/api/WhatsAppAPI/WebhookTest");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }


        }
        private async Task MoveToStg(WhatsAppModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedengineapi-stg.azurewebsites.net/API/WhatsAppAPI/webhook");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }


        }

        private async Task MoveToQa(WhatsAppModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedengineapi-qa.azurewebsites.net/API/WhatsAppAPI/webhook");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }


        }
        private async Task SendToRestaurantsBot(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/RestaurantsChatBot/RestaurantsMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }




        }
        private async Task SendToBookingChatBott(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/BookingChatBot/BookingMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }




        }
        private async Task SendToFlowsChatBot(CustomerModel jsonData)
        {
            try
            {

                //if (jsonData.phoneNumber=="962779746365")
                //{

                //    AppSettingsModel.BotApi ="https://e787-176-29-133-35.ngrok-free.app/";
                //}
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/FlowsChatBot/FlowsBotMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }




        }
        private async Task<bool> PrepareBotChatWithCustomer(string from, TenantModel Tenant, CustomerModel Customer, string msg, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string interactiveId)
        {




            Customer.TennantPhoneNumberId = Tenant.D360Key;
            Customer.interactiveId = interactiveId;
            Customer.attachments = tAttachments;
            List<Activity> Bot = new List<Activity>();
            if (Tenant.botId == "RestaurantBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToRestaurantsBot(Customer);
                return true;
            }
            else if (Tenant.botId == "BookingBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToBookingChatBott(Customer);
                return true;
            }
            else if (Tenant.botId == "FlowsBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToFlowsChatBot(Customer);
                return true;
            }
            else
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                var aaaa = JsonConvert.SerializeObject(Customer.customerChat);

                if ((string.IsNullOrEmpty(Tenant.botId) || string.IsNullOrEmpty(Tenant.DirectLineSecret)) && (!Tenant.BotTemplateId.HasValue))
                {
                    return false;
                }
                DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
                var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
                Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments, Tenant.BotTemplateId, interactiveId).Result;
                List<Activity> botListMessages = new List<Activity>();

                foreach (var msgBot in Bot)
                {

                    if (msgBot.Text.Contains("The process cannot access the file") || msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
                    {

                    }
                    else
                    {
                        botListMessages.Add(msgBot);
                    }


                }

                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();


                foreach (var msgBot in botListMessages)
                {


                    List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, from, Tenant.botId);

                    foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                    {
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {

                            //var message = PrepareMessageContent(msgBot, Tenant.botId);
                            //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);

                            WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, Tenant.botId, Customer.userId, Customer.TenantId.Value, Customer.ConversationId);
                            var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);
                            var aaaa2 = JsonConvert.SerializeObject(CustomerChat);
                            Customer.customerChat = CustomerChat;
                            //await _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                            SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        }
                    }

                }
            }





            return true;
        }


        #region Private Method 
        private CreateOrderModel MapOrder(int tenantId, int customerId, WhatsAppMessageModel orderMsg)
        {
            var order = new CreateOrderModel();
            order.TenantId = tenantId;
            order.CustomerId = customerId;
            order.IsZeedlyOrder = true;
            order.Total = 0;

            order.CreateOrderDetailsModels = new List<CreateOrderDetailsModel>();
            foreach (var item in orderMsg.order.product_items)
            {
                var orderDetailsItem = new CreateOrderDetailsModel();
                orderDetailsItem.ItemId = Convert.ToInt32(item.product_retailer_id);
                orderDetailsItem.Quantity = Convert.ToInt32(item.quantity);
                orderDetailsItem.UnitPrice = Convert.ToDecimal(item.item_price);
                orderDetailsItem.Total = orderDetailsItem.Quantity * orderDetailsItem.UnitPrice;
                orderDetailsItem.IsCondiments = false;
                orderDetailsItem.IsDeserts = false;
                orderDetailsItem.IsCrispy = false;


                order.Total += orderDetailsItem.Total;
                order.CreateOrderDetailsModels.Add(orderDetailsItem);
            }

            return order;
        }
        private TenantModel GetTenantById2(int tenantId)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetTenantById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TenantModel
                                {
                                    BusinessId = reader["BusinessId"]?.ToString(),
                                    CatalogueAccessToken = reader["CatalogueAccessToken"]?.ToString()
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<CatalogueDto> GetCatalogue(int tenantId)
        {
            //retrieve business id and access token based on tenant id
            var tenant = GetTenantById2(tenantId);
            var businessId = tenant.BusinessId;
            var accessToken = tenant.CatalogueAccessToken;
            if (businessId.IsNullOrEmpty() || accessToken.IsNullOrEmpty())
            {
               throw new UserFriendlyException("Business Id or Access Token is missing");

            }
            var getUrl = $"https://graph.facebook.com/v22.0/{businessId}/owned_product_catalogs?access_token={accessToken}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(getUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error calling Facebook API: {response.StatusCode}");
            }
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = System.Text.Json.JsonSerializer.Deserialize<CatalogueDto>(json, options);

            return result;
        }
        private async Task<List<ProductItem>> GetCatalogueItems(int tenantId)
        {
            var tenant = GetTenantById2(tenantId);
            var accessToken = tenant.CatalogueAccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UserFriendlyException("Access Token is missing");
            }

            var catalogue = await GetCatalogue(tenantId);
            var catalogueId = catalogue.Data[0].Id;

            var fields = "id,name,retailer_id,description,price,currency,availability,image_url,url";
            var baseUrl = $"https://graph.facebook.com/v19.0/{catalogueId}/products?fields={fields}&access_token={accessToken}";

            var httpClient = new HttpClient();

            var allItems = new List<ProductItem>();
            var url = baseUrl;

            do
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error calling Facebook API: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = System.Text.Json.JsonSerializer.Deserialize<CatalogueItemsDto>(json, options);

                if (result?.Data != null)
                {
                    allItems.AddRange(result.Data);
                }

                url = result?.Paging?.Next;

            } while (!string.IsNullOrEmpty(url));

            return allItems;
        }

        //private async Task<CatalogueItemsDto> GetCatalogueItems(int tenantId)
        //{
        //    //retrieve access token based on tenant id
        //    var tenant = GetTenantById2(tenantId);
        //    var accessToken = tenant.CatalogueAccessToken;
        //    if (accessToken.IsNullOrEmpty())
        //    {
        //       throw new UserFriendlyException("Access Token is missing");
        //    }

        //    var catalogue = await GetCatalogue(tenantId);
        //    var catalogueId = catalogue.Data[0].Id;

        //    var fields = "id,name,retailer_id,description,price,currency,availability,image_url,url";
        //    var getUrl = $"https://graph.facebook.com/v19.0/{catalogueId}/products?fields={fields}&access_token={accessToken}";

        //    var httpClient = new HttpClient();
        //    var response = await httpClient.GetAsync(getUrl);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new HttpRequestException($"Error calling Facebook API: {response.StatusCode}");
        //    }
        //    var json = await response.Content.ReadAsStringAsync();
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    var result = System.Text.Json.JsonSerializer.Deserialize<CatalogueItemsDto>(json, options);
        //    return result;

        //}

        private async Task CreateOrder(CreateOrderModel createOrderModel)
        {
            var catalogueDto = await GetCatalogueItems(createOrderModel.TenantId.Value);

            var orderDetailsWithProductInfo = new List<object>();

            foreach (var orderDetail in createOrderModel.CreateOrderDetailsModels)
            {
                //var product = catalogue.Data.FirstOrDefault(p => p.Retailer_Id == orderDetail.ItemId.ToString());
                var product = catalogueDto.FirstOrDefault(p => p.Retailer_Id == orderDetail.ItemId.ToString());

                if (product != null)
                {
                    var detail = new
                    {
                        orderDetail.Quantity,
                        orderDetail.UnitPrice,
                        orderDetail.Total,
                        orderDetail.ItemId,
                        ProductId = product.Id,
                        Name = product.Name,
                        Retailer_Id = product.Retailer_Id,
                        Description = product.Description,
                        Price = product.Price,
                        Currency = product.Currency,
                        Availability = product.Availability,
                        Image_Url = product.Image_Url,
                        Url = product.Url
                    };

                    orderDetailsWithProductInfo.Add(detail);
                }
            }

            string jsonOrderDetailsCareem = System.Text.Json.JsonSerializer.Serialize(orderDetailsWithProductInfo);

            if (createOrderModel.IsZeedlyOrder.HasValue && createOrderModel.IsZeedlyOrder == true)
            {
                try
                {
                    Orders.Order order = new Orders.Order
                    {
                        TenantId = createOrderModel.TenantId,
                        ContactId = createOrderModel.CustomerId,
                        Total = createOrderModel.Total,
                        IsDeleted = false,
                        OrderTime = DateTime.Now,
                        OrderNumber = new Random().Next(0001, 9999),
                        CreationTime = DateTime.Now,
                        AgentId = -1,
                        IsLockByAgent = false,
                        orderStatus = OrderStatusEunm.Draft,
                        IsEvaluation = false,
                        IsZeedlyOrder = 1,
                        ZeedlyOrderStatus = ZeedlyOrderStatus.New,
                        OrderDetailsCareem = jsonOrderDetailsCareem
                    };
                

                    long orderId = 0;
                    try
                    {
                        orderId = await _IOrdersAppService.CreateNewOrder(JsonConvert.SerializeObject(order));

                        if (orderId == 0)
                        {
                            this._telemetry.TrackTrace("error insert order to data base ", SeverityLevel.Critical);
                        }

                    }
                    catch (Exception ex)
                    {
                        this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                    }

                    try
                    {
                        await InsertMethod(createOrderModel, null, orderId);
                    }
                    catch (Exception ex)
                    {
                        this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);

                    }

                    await SendToBotAsync(createOrderModel.CustomerId.Value, createOrderModel.TenantId.Value);

                }
                catch (Exception ex)
                {
                    this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                }
            }
        }

        private async Task InsertMethod(CreateOrderModel createOrderModel, List<CreateOrderDetailsModel> createOrderDetailsModels, long orderId)
        {


            if (createOrderModel.CreateOrderDetailsModels.Count() > 0)
            {
                foreach (var item in createOrderModel.CreateOrderDetailsModels)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        IsDeleted = false,
                        CreationTime = DateTime.Now,
                        Discount = item.Discount,

                        ItemId = item.IsCondiments || item.IsCrispy || item.IsDeserts ? -1 : item.ItemId,
                        OrderId = orderId,

                        Quantity = item.Quantity,

                        IsCondiments = item.IsCondiments,
                        IsCrispy = item.IsCrispy,
                        IsDeserts = item.IsDeserts,

                        Total = item.Total,
                        TenantId = createOrderModel.TenantId,

                        UnitPrice = item.UnitPrice

                    };
                    var orderDetailID = await _IOrdersAppService.CreateOrderDetails(JsonConvert.SerializeObject(orderDetail));

                    foreach (var ext in item.createExtraOrderDetailsModels)
                    {
                        ExtraOrderDetail extraOrderDetail = new ExtraOrderDetail
                        {
                            Name = ext.Name,
                            NameEnglish = ext.NameEnglish,
                            Quantity = ext.Quantity,
                            TenantId = createOrderModel.TenantId,
                            UnitPrice = ext.UnitPrice,
                            OrderDetailId = orderDetailID,
                            Total = ext.Total
                        };

                        var extID = await _IOrdersAppService.CreateOrderDetailsExtra(JsonConvert.SerializeObject(extraOrderDetail));

                    }

                    foreach (var ext in item.ItemSpecifications)
                    {

                        foreach (var axChoise in ext.SpecificationChoices)
                        {
                            ExtraOrderDetail extraOrderDetail = new ExtraOrderDetail();
                            if (createOrderModel.TenantId == 34)
                            {
                                extraOrderDetail = new ExtraOrderDetail
                                {
                                    Name = axChoise.SpecificationChoiceDescription,
                                    NameEnglish = axChoise.SpecificationChoiceDescriptionEnglish,
                                    Quantity = 1,
                                    TenantId = createOrderModel.TenantId,
                                    UnitPrice = axChoise.Price,
                                    OrderDetailId = orderDetailID,
                                    Total = axChoise.Price

                                };

                            }
                            else
                            {
                                extraOrderDetail = new ExtraOrderDetail
                                {
                                    Name = axChoise.SpecificationChoiceDescription,
                                    NameEnglish = axChoise.SpecificationChoiceDescriptionEnglish,
                                    Quantity = item.Quantity,
                                    TenantId = createOrderModel.TenantId,
                                    UnitPrice = axChoise.Price,
                                    OrderDetailId = orderDetailID,
                                    Total = axChoise.Price,
                                    SpecificationNameEnglish = ext.SpecificationDescriptionEnglish,
                                    SpecificationName = ext.SpecificationDescription,
                                    SpecificationUniqueId = ext.UniqueId,


                                };
                            }
                            var extID = await _IOrdersAppService.CreateOrderDetailsSpecifications(JsonConvert.SerializeObject(extraOrderDetail));
                        }
                    }
                }

            }
        }

        private async Task SendToBotAsync(int CustomerId, int tenantId)
        {
            try
            {
                var user = GetCustomerAzuerById(CustomerId.ToString());
                var tenant = await _dbService.GetTenantInfoById(tenantId);

                if (tenant != null && !string.IsNullOrEmpty(tenant.AccessToken))
                {
                    var result = await sendToWhatsApp(tenant, user);
                }

            }
            catch (Exception ex)
            {
                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
            }

        }

        private CustomerModel GetCustomerAzuerById(string contactId)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && contactId != null && a.ContactID == contactId);
            if (customerResult.IsCompletedSuccessfully)
            {
                return customerResult.Result;
            }

            return null;
        }

        private async Task<bool> sendToWhatsApp(TenantModel Tenant, CustomerModel user)
        {
            string from = user.phoneNumber;
            string userId = Tenant.TenantId + "_" + user.phoneNumber;
            DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, user.userId, Tenant.botId).Result;
            var Bot = directLineConnector.StartBotConversationD360(userId, user.ContactID.ToString(), micosoftConversationID.MicrosoftBotId, "testinfoseed", Tenant.DirectLineSecret, Tenant.botId, user.phoneNumber, user.TenantId.ToString(), user.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null).Result;

            List<Activity> botListMessages = new List<Activity>();

            foreach (var msgBot in Bot)
            {

                if (msgBot.Text.Contains("The process cannot access the file") || msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
                {

                }
                else
                {
                    botListMessages.Add(msgBot);
                }

            }

            foreach (var msgBot in botListMessages)
            {
                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await new WhatsAppAppService().BotChatWithCustomer(msgBot, from, Tenant.botId);
                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                {
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        var message = PrepareMessageContent(msgBot, Tenant.botId);

                        var CustomerChat = _dbService.UpdateCustomerChatD360(user.TenantId, message, user.userId, user.ConversationId);
                        user.customerChat = CustomerChat;
                        SocketIOManager.SendChat(user, user.TenantId.Value);
                    }
                }
            }

            return true;

        }
        [HttpPost("webhookyara")]
        public async void test(dynamic jsonData2)
        {
            try
            {
                var aaaa = JsonConvert.SerializeObject(jsonData2);
                WhatsAppModel jsonData = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);

                string userId = string.Empty;
                var phoneNumberId = jsonData.Entry[0].Changes[0].Value.Metadata.phone_number_id;

                TenantModel Tenant = new TenantModel();

                var objTenant = _cacheManager.GetCache("CacheTenant").Get(phoneNumberId.ToString(), cache => cache);
                if (objTenant.Equals(phoneNumberId.ToString()))
                {
                    Tenant = await _dbService.GetTenantByKey("", phoneNumberId.ToString());
                    _cacheManager.GetCache("CacheTenant").Set(phoneNumberId.ToString(), Tenant);
                }
                else
                {
                    Tenant = (TenantModel)objTenant;
                }


                if (jsonData.Entry[0].Changes[0].Value.Messages == null && jsonData.Entry[0].Changes[0].Value.statuses == null)
                {
                    return;
                }


                if (jsonData.Entry[0].Changes[0].Value.Messages != null)
                {
                    if (jsonData.Entry[0].Changes[0].Value.Messages[0].context != null)
                    {

                        ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
                        {
                            TenantId = Tenant.TenantId.Value,
                            MessageStatusId = 4,
                            MessageDateTime = DateTime.Now,
                            MessageId = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id

                        };
                        //SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

                        var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;
                    }
                }

                var name2 = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;


                var massageId = jsonData.Entry[0].Changes[0].Value.Messages[0].Id;
                var from = jsonData.Entry[0].Changes[0].Value.Messages[0].From; // extract the phone number from the webhook payload
                string medaiUrl = string.Empty;
                userId = (Tenant.TenantId + "_" + from).ToString();
                var type = jsonData.Entry[0].Changes[0].Value.Messages[0].Type;
                string interactiveId = string.Empty;

                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();

                string msgBody = whatsAppAppService.MassageTypeText(jsonData.Entry[0].Changes[0].Value.Messages, Tenant.AccessToken, tAttachments, ref medaiUrl, attachmentMessageModel, out interactiveId);

                if (jsonData.Entry[0].Changes[0].Value.Messages[0].button != null)
                {
                    msgBody = jsonData.Entry[0].Changes[0].Value.Messages[0].button.text;

                }

                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                var Customer = await customerResult;

                if (Customer == null)
                {
                    Customer = _dbService.CreateNewCustomer(from, name2, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                    Customer.customerChat.text = msgBody;

                }

                Customer.avatarUrl = "avatar3";
                Customer.channel = "Whatsapp";

                if (Customer.expiration_timestamp != 0)
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Check if the expiration time has passed
                    if (currentTimestamp > Customer.expiration_timestamp)
                    {
                        Customer.IsLockedByAgent = false;
                        Customer.IsOpen = false;
                        Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };
                        Customer.IsHumanhandover = false;
                    }
                    else
                    {
                    }

                }

                if (Customer.CustomerStepModel == null)
                {
                    Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };

                }
                Customer.CustomerStepModel.UserParmeter.Remove("ContactID");
                Customer.CustomerStepModel.UserParmeter.Add("ContactID", Customer.ContactID.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("TenantId");
                Customer.CustomerStepModel.UserParmeter.Add("TenantId", Customer.TenantId.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                Customer.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer.phoneNumber.ToString());

                Customer.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Customer.expiration_timestamp = Customer.creation_timestamp + 86400;

                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Name");
                        Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Name");
                    Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                }


                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Location");
                        Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Location");
                    Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                }
                if (string.IsNullOrEmpty(Customer.displayName))
                {
                    Customer.displayName = name2;
                }

                if (Customer.IsBlock)
                {
                    return;
                }

                if (Tenant == null)
                {
                    return;
                }
                if (!Tenant.IsBundleActive)
                {
                    return;
                }
                if (!Tenant.IsBotActive)
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = from;
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = Tenant.MassageIfBotNotActive;
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        Content message = new Content();
                        message.text = Tenant.MassageIfBotNotActive;
                        message.agentName = Tenant.botId;
                        message.agentId = "1000000";
                        message.type = "text";

                        var CustomerSendChat2 = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, message.text, message.type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.TeamInbox, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);

                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                    }
                    return;

                }
                if (!Tenant.IsPaidInvoice)
                {
                    return;
                }

                try
                {
                    TimeSpan timeSpan = DateTime.UtcNow - Customer.TemplateFlowDate.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours <= 24)
                    {


                    }
                    else
                    {
                        Customer.IsTemplateFlow = false;
                        Customer.templateId = "";
                    }

                }
                catch
                {
                    Customer.IsTemplateFlow = false;
                    Customer.templateId = "";

                }


                if (attachmentMessageModel.IsHasAttachment)
                {
                    try
                    {
                        //await Sync(attachmentMessageModel, Customer, massageId);

                    }
                    catch
                    {


                    }
                    var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
                    attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                }
                else
                {

                    var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                    Customer.customerChat = CustomerSendChat;

                    if (Customer.IsConversationExpired)
                    {

                        Customer.IsConversationExpired = false;
                    }

                }

                //if the customer message type is order then create a new order
                if (jsonData.Entry[0].Changes[0].Value.Messages[0].Type == WhatsContentTypeEnum.order.ToString())
                {
                    var msg = jsonData.Entry[0].Changes[0].Value.Messages[0];
                    var cusId = Convert.ToInt32(Customer.ContactID);
                    var orderModel = MapOrder(Tenant.TenantId.Value, cusId, msg);

                    await CreateOrder(orderModel);
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }



        private static CreateContactMg CreateContactMgFun(CreateContactFromInfoSeed model)
        {
            model.company = "infoseed";
            model.email = model.company + "-" + model.phone + "@infoseed.com";

            CreateContactMg createContactMg = new CreateContactMg();
            Property1 property1 = new Property1 { property = "email", value = model.email };
            Property1 firstname = new Property1 { property = "firstname", value = model.firstname };
            Property1 lastname = new Property1 { property = "lastname", value = model.lastname };
            //  Property1 website = new Property1 { property="website", value="http://hubspot.com" };
            Property1 company = new Property1 { property = "company", value = model.company };
            Property1 phone = new Property1 { property = "phone", value = model.phone };
            // Property1 address = new Property1 { property="address", value="25 First Street" };
            //  Property1 city = new Property1 { property="city", value="Cambridge" };
            //  Property1 state = new Property1 { property="state", value="MA" };
            // Property1 zip = new Property1 { property="zip", value="02139" };
            List<Property1> properties = new List<Property1>();

            properties.Add(property1);
            properties.Add(firstname);
            properties.Add(lastname);
            //  properties.Add(website);
            properties.Add(company);
            properties.Add(phone);
            // properties.Add(address);
            //  properties.Add(city);
            //  properties.Add(state);
            //  properties.Add(zip);

            createContactMg.properties = properties.ToArray();
            return createContactMg;
        }
        private Content PrepareMessageContent(Activity msgBot, string botId)
        {
            string tMessageToSend = string.Empty;
            List<CardAction> tOutActions = new List<CardAction>();
            int tOrder = 1;
            var optionlst = new Dictionary<int, string>();
            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
            {
                tOutActions.AddRange(msgBot.SuggestedActions.Actions);
            }

            foreach (var hc in tOutActions)
            {
                tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
                optionlst.Add(tOrder, hc.Title);
                tOrder++;
            }

            Content message = new Content
            {
                text = msgBot.Text + "\r\n" + tMessageToSend,
                type = "text",
                agentName = botId,
                agentId = "1000000"

            };

            return message;

        }

        [HttpGet("testnow")]
        public bool testnow()
        {
            DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
            DayOfWeek wk = currentDateTime.DayOfWeek;

            DateTime StartDate = getValidValue("2022-11-21T00:00:00+00:00");//.AddHours(AppSettingsModel.DivHour);
            DateTime EndDate = getValidValue("2022-11-21T02:00:00");//.AddHours(AppSettingsModel.DivHour);

            DateTime StartDateSP = getValidValue("2022-11-21T14:00:00");//.AddHours(AppSettingsModel.DivHour);
            DateTime EndDateSP = getValidValue("2022-11-21T23:59:00");//.AddHours(AppSettingsModel.DivHour);
            TimeSpan timeOfDay = currentDateTime.TimeOfDay;



            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

            {
                // result = true;
            }
            else
            {
                // outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                //            , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                // result = false;
            }


            return true;

        }

        private bool checkIsInService(Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel workModel, out string outWHMessage)
        {

            bool result = true;
            outWHMessage = string.Empty;
            DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
            DayOfWeek wk = currentDateTime.DayOfWeek;
            TimeSpan timeOfDay = currentDateTime.TimeOfDay;
            var options = new JsonSerializerOptions { WriteIndented = true };

            switch (wk)
            {
                case DayOfWeek.Saturday:
                    if (workModel.IsWorkActiveSat)
                    {
                        var StartDateSat = getValidValue(workModel.StartDateSat).AddHours(AppSettingsModel.AddHour);
                        var EndDateSat = getValidValue(workModel.EndDateSat).AddHours(AppSettingsModel.AddHour);

                        var StartDateSatSP = getValidValue(workModel.StartDateSatSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSatSP = getValidValue(workModel.EndDateSatSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }

                    break;
                case DayOfWeek.Sunday:
                    if (workModel.IsWorkActiveSun)
                    {
                        var StartDate = getValidValue(workModel.StartDateSun).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateSun).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateSunSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateSunSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextSun, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Monday:

                    if (workModel.IsWorkActiveMon)
                    {
                        var StartDate = getValidValue(workModel.StartDateMon).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateMon).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateMonSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateMonSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }

                    break;
                case DayOfWeek.Tuesday:
                    if (workModel.IsWorkActiveTues)
                    {
                        var StartDate = getValidValue(workModel.StartDateTues).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateTues).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateTuesSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateTuesSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextTues, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Wednesday:
                    if (workModel.IsWorkActiveWed)
                    {
                        var StartDate = getValidValue(workModel.StartDateWed).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateWed).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateWedSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateWedSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextWed, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Thursday:
                    if (workModel.IsWorkActiveThurs)
                    {
                        var StartDate = getValidValue(workModel.StartDateThurs).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateThurs).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateThursSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateThursSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextThurs, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Friday:
                    if (workModel.IsWorkActiveFri)
                    {
                        var StartDate = getValidValue(workModel.StartDateFri).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateFri).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateFriSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateFriSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextFri, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                default:

                    break;

            }



            return result;

        }

        private DateTime getValidValue(dynamic value)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                result = DateTime.Parse(value.ToString());
                return result;
            }
            catch (Exception)
            {
                return result;
                throw;
            }

        }
        private void SetStatusInQueue(WebHookModel message)
        {
            try
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("webhookstatusfuntest");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }
        private void SetStatusInQueuestg(WebHookModel message)
        {
            try
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=infoseedstoragestg;AccountKey=sc089/Ku+IUBAbCwGnlumuK72RultGBqL/TwHS36SJHlCx3uC9dtEKjJJJHPRiRrAMwrIs2FyP6Z+ASt8j6gWg==;EndpointSuffix=core.windows.net");
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("webhookstatusfuntest");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));


                //for(var i = 0; i<100; i++)
                //{

                queue.AddMessageAsync(queueMessage);

                //}

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }
        private void SetConversationMeasurmentsInQueue(ConservationMeasurementMessage message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("conservation-measurements");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }

        private void SetAttachmentMessageInQueue(AttachmentMessageModel message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("attachment-messages");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }
        private void SetMgMotorIntegrationInQueue(CreateContactMg ContactMg)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("mgcontacts-sync");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           ContactMg
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(ContactMg);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }

        private void UpdateContactConversationId(string ConversationId, string userId, int creation_timestamp, int expiration_timestamp)
        {




            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);//&& a.TenantId== TenantId

            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer == null)
                {

                }
                else
                {


                    if (customer.ConversationId != ConversationId)
                    {
                        customer.ConversationId = ConversationId;
                        customer.LastConversationStartDateTime = DateTime.Now;//.AddHours(AppSettingsModel.AddHour);

                        customer.expiration_timestamp = expiration_timestamp;
                        customer.creation_timestamp = creation_timestamp;

                        if (customer.creation_timestamp == 0 || customer.expiration_timestamp == 0)
                        {
                            var model = getConversationSessions(customer.TenantId.Value, customer.phoneNumber, ConversationId);

                            if (model != null && model.expiration_timestamp != 0 && model.creation_timestamp != 0)
                            {
                                customer.expiration_timestamp = model.expiration_timestamp;
                                customer.creation_timestamp = model.creation_timestamp;
                            }

                        }




                    }
                    else
                    {
                        customer.expiration_timestamp = expiration_timestamp;
                        customer.creation_timestamp = creation_timestamp;


                    }



                    // customer.LastConversationStartDateTime = ConversationId;
                    var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                }
            }
        }



        private static ConversationSessionsModel getConversationSessions(int TenantId, string PhoneNumber, string ConversationID)
        {
            try
            {
                ConversationSessionsModel ConversationSessionsEntity = new ConversationSessionsModel();
                var SP_Name = "GetConversationSessions";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@ConversationID",ConversationID)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                };

                ConversationSessionsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSessions, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return ConversationSessionsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
        {
            try
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "creation_timestamp");
                model.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "expiration_timestamp");
                ///model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = 0;
                model.expiration_timestamp = 0;

                return model;
            }
        }

        private void ExceptionLogAdd(string LogJson, int TenantId, string PhoneNumber)
        {
            try
            {
                var SP_Name = "ExceptionLogAdd";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@LogJson",LogJson)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                     ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                     ,new System.Data.SqlClient.SqlParameter("@Date",DateTime.UtcNow)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static CoutryTelCodeDashModel getCoutryBIRate(string phone)
        {
            List<CoutryTelCodeDashModel> TelCodes = new List<CoutryTelCodeDashModel>
                {
                    new CoutryTelCodeDashModel("93", "AF",1),
                    new CoutryTelCodeDashModel("355", "AL",1),
                    new CoutryTelCodeDashModel("213", "DZ",2),
                    new CoutryTelCodeDashModel("1-684", "AS",1),
                    new CoutryTelCodeDashModel("376", "AD",1),
                    new CoutryTelCodeDashModel("244", "AO",1),
                    new CoutryTelCodeDashModel("1-264", "AI",1),
                    new CoutryTelCodeDashModel("672", "AQ",1),
                    new CoutryTelCodeDashModel("1-268", "AG",1),
                    new CoutryTelCodeDashModel("54", "AR",1),
                    new CoutryTelCodeDashModel("374", "AM",1),
                    new CoutryTelCodeDashModel("297", "AW",1),
                    new CoutryTelCodeDashModel("61", "AU",1),
                    new CoutryTelCodeDashModel("43", "AT",(decimal)1.5),
                    new CoutryTelCodeDashModel("994", "AZ",1),
                    new CoutryTelCodeDashModel("1-242", "BS",1),
                    new CoutryTelCodeDashModel("973", "BH",1),
                    new CoutryTelCodeDashModel("880", "BD",1),
                    new CoutryTelCodeDashModel("1-246", "BB",1),
                    new CoutryTelCodeDashModel("375", "BY",(decimal)1.5),
                    new CoutryTelCodeDashModel("32", "BE",2),
                    new CoutryTelCodeDashModel("501", "BZ",1),
                    new CoutryTelCodeDashModel("229", "BJ",1),
                    new CoutryTelCodeDashModel("1-441", "BM",1),
                    new CoutryTelCodeDashModel("975", "BT",1),
                    new CoutryTelCodeDashModel("591", "BO",1),
                    new CoutryTelCodeDashModel("387", "BA",1),
                    new CoutryTelCodeDashModel("267", "BW",1),
                    new CoutryTelCodeDashModel("55", "BR",1),
                    new CoutryTelCodeDashModel("246", "IO",1),
                    new CoutryTelCodeDashModel("1-284", "VG",1),
                    new CoutryTelCodeDashModel("673", "BN",1),
                    new CoutryTelCodeDashModel("359", "BG",(decimal)1.5),
                    new CoutryTelCodeDashModel("226", "BF",1),
                    new CoutryTelCodeDashModel("257", "BI",1),
                    new CoutryTelCodeDashModel("855", "KH",1),
                    new CoutryTelCodeDashModel("237", "CM",1),
                    new CoutryTelCodeDashModel("1", "CA",(decimal)0.5),
                    new CoutryTelCodeDashModel("238", "CV",1),
                    new CoutryTelCodeDashModel("1-345", "KY",1),
                    new CoutryTelCodeDashModel("236", "CF",1),
                    new CoutryTelCodeDashModel("235", "TD",1),
                    new CoutryTelCodeDashModel("56", "CL",1),
                    new CoutryTelCodeDashModel("86", "CN",1),
                    new CoutryTelCodeDashModel("61", "CX",1),
                    new CoutryTelCodeDashModel("61", "CC",1),
                    new CoutryTelCodeDashModel("57", "CO",(decimal)0.5),
                    new CoutryTelCodeDashModel("269", "KM",1),
                    new CoutryTelCodeDashModel("682", "CK",1),
                    new CoutryTelCodeDashModel("506", "CR",1),
                    new CoutryTelCodeDashModel("385", "HR",(decimal)1.5),
                    new CoutryTelCodeDashModel("53", "CU",1),
                    new CoutryTelCodeDashModel("599", "CW",1),
                    new CoutryTelCodeDashModel("357", "CY",1),
                    new CoutryTelCodeDashModel("420", "CZ",(decimal)1.5),
                    new CoutryTelCodeDashModel("243", "CD",1),
                    new CoutryTelCodeDashModel("45", "DK",2),
                    new CoutryTelCodeDashModel("253", "DJ",1),
                    new CoutryTelCodeDashModel("1-767", "DM",1),
                    new CoutryTelCodeDashModel("1-809", "DO",1),
                    new CoutryTelCodeDashModel("1-829", "DO",1),
                    new CoutryTelCodeDashModel("1-849", "DO",1),
                    new CoutryTelCodeDashModel("670", "TL",1),
                    new CoutryTelCodeDashModel("593", "EC",1),
                    new CoutryTelCodeDashModel("20", "EG",(decimal)1.5),
                    new CoutryTelCodeDashModel("503", "SV",1),
                    new CoutryTelCodeDashModel("240", "GQ",1),
                    new CoutryTelCodeDashModel("291", "ER",1),
                    new CoutryTelCodeDashModel("372", "EE",(decimal)1.5),
                    new CoutryTelCodeDashModel("251", "ET",1),
                    new CoutryTelCodeDashModel("500", "FK",1),
                    new CoutryTelCodeDashModel("298", "FO",1),
                    new CoutryTelCodeDashModel("679", "FJ",1),
                    new CoutryTelCodeDashModel("358", "FI",1),
                    new CoutryTelCodeDashModel("33", "FR",2),
                    new CoutryTelCodeDashModel("689", "PF",1),
                    new CoutryTelCodeDashModel("241", "GA",1),
                    new CoutryTelCodeDashModel("220", "GM",1),
                    new CoutryTelCodeDashModel("995", "GE",1),
                    new CoutryTelCodeDashModel("49", "DE",2),
                    new CoutryTelCodeDashModel("233", "GH",1),
                    new CoutryTelCodeDashModel("350", "GI",1),
                    new CoutryTelCodeDashModel("30", "GR",1),
                    new CoutryTelCodeDashModel("299", "GL",(decimal)0.5),
                    new CoutryTelCodeDashModel("1-473", "GD",1),
                    new CoutryTelCodeDashModel("1-671", "GU",1),
                    new CoutryTelCodeDashModel("502", "GT",1),
                    new CoutryTelCodeDashModel("44-1481", "GG",1),
                    new CoutryTelCodeDashModel("224", "GN",1),
                    new CoutryTelCodeDashModel("245", "GW",1),
                    new CoutryTelCodeDashModel("592", "GY",1),
                    new CoutryTelCodeDashModel("509", "HT",1),
                    new CoutryTelCodeDashModel("504", "HN",1),
                    new CoutryTelCodeDashModel("852", "HK",1),
                    new CoutryTelCodeDashModel("36", "HU",(decimal)1.5),
                    new CoutryTelCodeDashModel("354", "IS",1),
                    new CoutryTelCodeDashModel("91", "IN",(decimal)0.5),
                    new CoutryTelCodeDashModel("62", "ID",(decimal)0.5),
                    new CoutryTelCodeDashModel("98", "IR",1),
                    new CoutryTelCodeDashModel("964", "IQ",1),
                    new CoutryTelCodeDashModel("353", "IE",1),
                    new CoutryTelCodeDashModel("44-1624", "IM",1),
                    new CoutryTelCodeDashModel("972", "IL",(decimal)0.5),
                    new CoutryTelCodeDashModel("39", "IT",1),
                    new CoutryTelCodeDashModel("225", "CI",1),
                    new CoutryTelCodeDashModel("1-876", "JM",1),
                    new CoutryTelCodeDashModel("81", "JP",1),
                    new CoutryTelCodeDashModel("44-1534", "JE",1),
                    new CoutryTelCodeDashModel("962", "JO",1),
                    new CoutryTelCodeDashModel("7", "KZ",1),
                    new CoutryTelCodeDashModel("254", "KE",1),
                    new CoutryTelCodeDashModel("686", "KI",1),
                    new CoutryTelCodeDashModel("383", "XK",1),
                    new CoutryTelCodeDashModel("965", "KW",1),
                    new CoutryTelCodeDashModel("996", "KG",1),
                    new CoutryTelCodeDashModel("856", "LA",1),
                    new CoutryTelCodeDashModel("371", "LV",1),
                    new CoutryTelCodeDashModel("961", "LB",1),
                    new CoutryTelCodeDashModel("266", "LS",1),
                    new CoutryTelCodeDashModel("231", "LR",1),
                    new CoutryTelCodeDashModel("218", "LY",2),
                    new CoutryTelCodeDashModel("423", "LI",1),
                    new CoutryTelCodeDashModel("370", "LT",1),
                    new CoutryTelCodeDashModel("352", "LU",1),
                    new CoutryTelCodeDashModel("853", "MO",1),
                    new CoutryTelCodeDashModel("389", "MK",1),
                    new CoutryTelCodeDashModel("261", "MG",1),
                    new CoutryTelCodeDashModel("265", "MW",1),
                    new CoutryTelCodeDashModel("60", "MY",1),
                    new CoutryTelCodeDashModel("960", "MV",1),
                    new CoutryTelCodeDashModel("223", "ML",1),
                    new CoutryTelCodeDashModel("356", "MT",1),
                    new CoutryTelCodeDashModel("692", "MH",1),
                    new CoutryTelCodeDashModel("222", "MR",1),
                    new CoutryTelCodeDashModel("230", "MU",1),
                    new CoutryTelCodeDashModel("262", "YT",1),
                    new CoutryTelCodeDashModel("52", "MX",(decimal)0.5),
                    new CoutryTelCodeDashModel("691", "FM",1),
                    new CoutryTelCodeDashModel("373", "MD",(decimal)1.5),
                    new CoutryTelCodeDashModel("377", "MC",1),
                    new CoutryTelCodeDashModel("976", "MN",1),
                    new CoutryTelCodeDashModel("382", "ME",1),
                    new CoutryTelCodeDashModel("1-664", "MS",1),
                    new CoutryTelCodeDashModel("212", "MA",1),
                    new CoutryTelCodeDashModel("258", "MZ",1),
                    new CoutryTelCodeDashModel("95", "MM",1),
                    new CoutryTelCodeDashModel("264", "NA",1),
                    new CoutryTelCodeDashModel("674", "NR",1),
                    new CoutryTelCodeDashModel("977", "NP",1),
                    new CoutryTelCodeDashModel("31", "NL",1),
                    new CoutryTelCodeDashModel("599", "AN",1),
                    new CoutryTelCodeDashModel("687", "NC",1),
                    new CoutryTelCodeDashModel("64", "NZ",1),
                    new CoutryTelCodeDashModel("505", "NI",1),
                    new CoutryTelCodeDashModel("227", "NE",1),
                    new CoutryTelCodeDashModel("234", "NG",1),
                    new CoutryTelCodeDashModel("683", "NU",1),
                    new CoutryTelCodeDashModel("850", "KP",1),
                    new CoutryTelCodeDashModel("1-670", "MP",1),
                    new CoutryTelCodeDashModel("47", "NO",1),
                    new CoutryTelCodeDashModel("968", "OM",1),
                    new CoutryTelCodeDashModel("92", "PK",1),
                    new CoutryTelCodeDashModel("680", "PW",1),
                    new CoutryTelCodeDashModel("970", "PS",1),
                    new CoutryTelCodeDashModel("507", "PA",1),
                    new CoutryTelCodeDashModel("675", "PG",1),
                    new CoutryTelCodeDashModel("595", "PY",1),
                    new CoutryTelCodeDashModel("51", "PE",1),
                    new CoutryTelCodeDashModel("63", "PH",1),
                    new CoutryTelCodeDashModel("64", "PN",1),
                    new CoutryTelCodeDashModel("48", "PL",(decimal)1.5),
                    new CoutryTelCodeDashModel("351", "PT",1),
                    new CoutryTelCodeDashModel("1-787", "PR",1),
                    new CoutryTelCodeDashModel("1-939", "PR",1),
                    new CoutryTelCodeDashModel("974", "QA",1),
                    new CoutryTelCodeDashModel("242", "CG",1),
                    new CoutryTelCodeDashModel("262", "RE",1),
                    new CoutryTelCodeDashModel("40", "RO",(decimal)1.5),
                    new CoutryTelCodeDashModel("7", "RU",1),
                    new CoutryTelCodeDashModel("250", "RW",1),
                    new CoutryTelCodeDashModel("590", "BL",1),
                    new CoutryTelCodeDashModel("290", "SH",1),
                    new CoutryTelCodeDashModel("1-869", "KN",1),
                    new CoutryTelCodeDashModel("1-758", "LC",1),
                    new CoutryTelCodeDashModel("590", "MF",1),
                    new CoutryTelCodeDashModel("508", "PM",(decimal)0.5),
                    new CoutryTelCodeDashModel("1-784", "VC",1),
                    new CoutryTelCodeDashModel("685", "WS",1),
                    new CoutryTelCodeDashModel("378", "SM",1),
                    new CoutryTelCodeDashModel("239", "ST",1),
                    new CoutryTelCodeDashModel("966", "SA",(decimal)0.5),
                    new CoutryTelCodeDashModel("221", "SN",1),
                    new CoutryTelCodeDashModel("381", "RS",1),
                    new CoutryTelCodeDashModel("248", "SC",1),
                    new CoutryTelCodeDashModel("232", "SL",1),
                    new CoutryTelCodeDashModel("65", "SG",1),
                    new CoutryTelCodeDashModel("1-721", "SX",1),
                    new CoutryTelCodeDashModel("421", "SK",(decimal)1.5),
                    new CoutryTelCodeDashModel("386", "SI",1),
                    new CoutryTelCodeDashModel("677", "SB",1),
                    new CoutryTelCodeDashModel("252", "SO",1),
                    new CoutryTelCodeDashModel("27", "ZA",(decimal)0.5),
                    new CoutryTelCodeDashModel("82", "KR",1),
                    new CoutryTelCodeDashModel("211", "SS",2),
                    new CoutryTelCodeDashModel("34", "ES",1),
                    new CoutryTelCodeDashModel("94", "LK",1),
                    new CoutryTelCodeDashModel("249", "SD",2),
                    new CoutryTelCodeDashModel("597", "SR",1),
                    new CoutryTelCodeDashModel("47", "SJ",1),
                    new CoutryTelCodeDashModel("268", "SZ",1),
                    new CoutryTelCodeDashModel("46", "SE",1),
                    new CoutryTelCodeDashModel("41", "CH",1),
                    new CoutryTelCodeDashModel("963", "SY",1),
                    new CoutryTelCodeDashModel("886", "TW",1),
                    new CoutryTelCodeDashModel("992", "TJ",1),
                    new CoutryTelCodeDashModel("255", "TZ",1),
                    new CoutryTelCodeDashModel("66", "TH",1),
                    new CoutryTelCodeDashModel("228", "TG",1),
                    new CoutryTelCodeDashModel("690", "TK",1),
                    new CoutryTelCodeDashModel("676", "TO",1),
                    new CoutryTelCodeDashModel("1-868", "TT",1),
                    new CoutryTelCodeDashModel("216", "TN",2),
                    new CoutryTelCodeDashModel("90", "TR",(decimal)0.5),
                    new CoutryTelCodeDashModel("993", "TM",1),
                    new CoutryTelCodeDashModel("1-649", "TC",1),
                    new CoutryTelCodeDashModel("688", "TV",1),
                    new CoutryTelCodeDashModel("1-340", "VI",1),
                    new CoutryTelCodeDashModel("256", "UG",1),
                    new CoutryTelCodeDashModel("380", "UA",(decimal)1.5),
                    new CoutryTelCodeDashModel("971", "AE",(decimal)0.5),
                    new CoutryTelCodeDashModel("44", "GB",1),
                    new CoutryTelCodeDashModel("1", "US",(decimal)0.5),
                    new CoutryTelCodeDashModel("598", "UY",1),
                    new CoutryTelCodeDashModel("998", "UZ",1),
                    new CoutryTelCodeDashModel("678", "VU",1),
                    new CoutryTelCodeDashModel("379", "VA",1),
                    new CoutryTelCodeDashModel("58", "VE",1),
                    new CoutryTelCodeDashModel("84", "VN",1),
                    new CoutryTelCodeDashModel("681", "WF",1),
                    new CoutryTelCodeDashModel("212", "EH",1),
                    new CoutryTelCodeDashModel("967", "YE",1),
                    new CoutryTelCodeDashModel("260", "ZM",1),
                    new CoutryTelCodeDashModel("263", "ZW",1),
            };
            CoutryTelCodeDashModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx)).FirstOrDefault();
            return result;

        }
        private static CountryCodeDashCoreModel CountryISOCodeGet(string ISOCodes)
        {
            try
            {
                CountryCodeDashCoreModel countryCodeDashModel = new CountryCodeDashCoreModel();
                var SP_Name = "[dbo].[CountryISOCodeGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@ISOCodes",ISOCodes)
                };

                countryCodeDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapCountryInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return countryCodeDashModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static CountryCodeDashCoreModel MapCountryInfo(IDataReader dataReader)
        {
            try
            {
                CountryCodeDashCoreModel countryCodeDashModel = new CountryCodeDashCoreModel();
                countryCodeDashModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                countryCodeDashModel.Country = SqlDataHelper.GetValue<string>(dataReader, "Country");
                countryCodeDashModel.Region = SqlDataHelper.GetValue<string>(dataReader, "Region");
                countryCodeDashModel.CountryCallingCode = SqlDataHelper.GetValue<string>(dataReader, "CountryCallingCode");
                countryCodeDashModel.Currency = SqlDataHelper.GetValue<string>(dataReader, "Currency");
                countryCodeDashModel.MarketingPrice = SqlDataHelper.GetValue<float>(dataReader, "MarketingPrice");
                countryCodeDashModel.UtilityPrice = SqlDataHelper.GetValue<float>(dataReader, "UtilityPrice");
                countryCodeDashModel.AuthenticationPrice = SqlDataHelper.GetValue<float>(dataReader, "AuthenticationPrice");
                countryCodeDashModel.ServicePrice = SqlDataHelper.GetValue<float>(dataReader, "ServicePrice");
                countryCodeDashModel.ISOCountryCodes = SqlDataHelper.GetValue<string>(dataReader, "ISOCountryCodes");

                return countryCodeDashModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private WalletDashModel walletGetByTenantId(int TenantId)
        {
            try
            {
                WalletDashModel walletModel = new WalletDashModel();

                var SP_Name = "[dbo].[WalletGet]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };

                walletModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapWallet, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return walletModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void updateTicket(int TenantId, string phonenumber, int create, int exp, bool isConvertaionExp)
        {
            try
            {

                var SP_Name = "[dbo].[UpdateAllTiketByContact]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@phonenumber",phonenumber),
                     new System.Data.SqlClient.SqlParameter("@create",create),
                    new System.Data.SqlClient.SqlParameter("@exp",exp),
                     new System.Data.SqlClient.SqlParameter("@isConvertaionExp",isConvertaionExp)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddToWebHookErorrLog(int TenantId, string Code, string Details, string Title, string JsonString)
        {
            try
            {

                var SP_Name = "[dbo].[AddToWebHookErorrLog]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@Code",Code),
                     new System.Data.SqlClient.SqlParameter("@Details",Details),
                    new System.Data.SqlClient.SqlParameter("@exp",Title),
                    new System.Data.SqlClient.SqlParameter("@JsonString",JsonString)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }
        private static WalletDashModel MapWallet(IDataReader dataReader)
        {
            try
            {
                WalletDashModel model = new WalletDashModel();
                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
                model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
                model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static long UnfreezeFundsAndCreateTransaction(UsageDetailsCoreModel usageDetailsModel)
        {
            try
            {

                var SP_Name = "[dbo].[ConversationUnfreezeFunds]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",usageDetailsModel.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@categoryType",usageDetailsModel.categoryType)
                    ,new System.Data.SqlClient.SqlParameter("@dateTime",usageDetailsModel.dateTime)
                    ,new System.Data.SqlClient.SqlParameter("@SentBy",usageDetailsModel.sentBy)
                    ,new System.Data.SqlClient.SqlParameter("@templateName",usageDetailsModel.templateName)
                    ,new System.Data.SqlClient.SqlParameter("@campaignName",usageDetailsModel.campaignName)
                    ,new System.Data.SqlClient.SqlParameter("@quantity",usageDetailsModel.quantity)
                    ,new System.Data.SqlClient.SqlParameter("@totalCost",usageDetailsModel.totalCost)
                    ,new System.Data.SqlClient.SqlParameter("@totalCreditRemaining",usageDetailsModel.totalCreditRemaining)
                    ,new System.Data.SqlClient.SqlParameter("@countries",usageDetailsModel.countries)
                    ,new System.Data.SqlClient.SqlParameter("@campaignId",usageDetailsModel.campaignId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Output",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "" || OutputParameter.Value.ToString() != null)
                {
                    return (long)OutputParameter.Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static CampaignInfoModel GetCampaignName(long CampaignId)
        {
            try
            {
                var SP_Name = "[dbo].[GetCampaignSomInfo]";
                CampaignInfoModel campaignInfoModel = new CampaignInfoModel();

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                };

                campaignInfoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapCampignInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return campaignInfoModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static CampaignInfoModel MapCampignInfo(IDataReader dataReader)
        {
            try
            {
                CampaignInfoModel campaignInfoModel = new CampaignInfoModel();

                campaignInfoModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                campaignInfoModel.campaignName = SqlDataHelper.GetValue<string>(dataReader, "Title");
                campaignInfoModel.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName");
                campaignInfoModel.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
                campaignInfoModel.Category = SqlDataHelper.GetValue<string>(dataReader, "Category");


                return campaignInfoModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static long UnfreezeFundsSuccessSentCampaign(UsageDetailsCoreModel usageDetailsModel)
        {
            try
            {

                var SP_Name = "[dbo].[ConversationSuccessSentCampaign]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",usageDetailsModel.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@categoryType",usageDetailsModel.categoryType)
                    ,new System.Data.SqlClient.SqlParameter("@dateTime",usageDetailsModel.dateTime)
                    ,new System.Data.SqlClient.SqlParameter("@SentBy",usageDetailsModel.sentBy)
                    ,new System.Data.SqlClient.SqlParameter("@templateName",usageDetailsModel.templateName)
                    ,new System.Data.SqlClient.SqlParameter("@campaignName",usageDetailsModel.campaignName)
                    ,new System.Data.SqlClient.SqlParameter("@quantity",usageDetailsModel.quantity)
                    ,new System.Data.SqlClient.SqlParameter("@totalCost",usageDetailsModel.totalCost)
                    ,new System.Data.SqlClient.SqlParameter("@totalCreditRemaining",usageDetailsModel.totalCreditRemaining)
                    ,new System.Data.SqlClient.SqlParameter("@countries",usageDetailsModel.countries)
                    ,new System.Data.SqlClient.SqlParameter("@campaignId",usageDetailsModel.campaignId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Output",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "" || OutputParameter.Value.ToString() != null)
                {
                    return (long)OutputParameter.Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static long CostForServiceConversation(UsageDetailsCoreModel usageDetailsModel)
        {
            try
            {

                var SP_Name = "[dbo].[CostForServiceConversation]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",usageDetailsModel.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@categoryType",usageDetailsModel.categoryType)
                    ,new System.Data.SqlClient.SqlParameter("@dateTime",usageDetailsModel.dateTime)
                    ,new System.Data.SqlClient.SqlParameter("@quantity",usageDetailsModel.quantity)
                    ,new System.Data.SqlClient.SqlParameter("@totalCost",usageDetailsModel.totalCost)
                    ,new System.Data.SqlClient.SqlParameter("@totalCreditRemaining",usageDetailsModel.totalCreditRemaining)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Output",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void UpdateTenantDailyLimit(int tenantId)
        {
            try
            {
                var SP_Name = "[dbo].[ReducingTenantDailyLimit]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Sync(AttachmentMessageModel attachmentMessageModel, CustomerModel customerModel, string massageId)
        {
            try
            {
                var RetrievingMedia = RetrievingMediaAsync(attachmentMessageModel.AttachmentId, attachmentMessageModel.FcToken).Result;

                var extention = "";
                if (RetrievingMedia.contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    extention = ".docx";

                }
                else if (RetrievingMedia.contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {

                    extention = ".xlsx";

                }
                else
                {
                    extention = "." + RetrievingMedia.mime_type.Split("/").LastOrDefault();

                }


                var type = RetrievingMedia.mime_type.Split("/")[0];
                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();

                AttachmentContent attachmentContent = new AttachmentContent()
                {
                    Content = RetrievingMedia.contentByte,
                    Extension = extention,
                    MimeType = RetrievingMedia.mime_type,
                    AttacmentName = RetrievingMedia.id.ToString()
                };

                var url = azureBlobProvider.Save(attachmentContent).Result;
                //  UpdateCustomerChatAsync(attachmentMessageModel.CustomerModel, url, type, massageId);


                var text = "";
                if (customerModel.customerChat != null)
                {
                    text = customerModel.customerChat.text;

                }

                CustomerChat customerChat = new CustomerChat()
                {
                    messageId = massageId,
                    TenantId = customerModel.TenantId,
                    userId = customerModel.userId,
                    text = text,
                    type = type,//hjhj
                    CreateDate = DateTime.Now.AddSeconds(-3),
                    status = (int)Messagestatus.New,
                    sender = MessageSenderType.Customer,
                    ItemType = InfoSeedContainerItemTypes.ConversationItem,
                    mediaUrl = url,
                    UnreadMessagesCount = 0,
                    agentName = "admin",
                    agentId = "",
                };

                customerModel.customerChat = customerChat;
            }
            catch
            {


            }


        }

        private static async Task<WhatsAppAttachmentModel> RetrievingMediaAsync(string mediaId, string fbToken)
        {
            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();

            WhatsAppAttachmentModel attachmentModel2 = new WhatsAppAttachmentModel();

            try
            {
                using (var httpClient = new HttpClient())
                {


                    var FBUrl = "https://graph.facebook.com/v17.0/" + mediaId;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                    using (var response = await httpClient.GetAsync(FBUrl))
                    {
                        using (var content = response.Content)
                        {
                            // WhatsAppMediaResponse whatsAppMediaResponse = new WhatsAppMediaResponse();

                            attachmentModel = JsonConvert.DeserializeObject<WhatsAppAttachmentModel>(await content.ReadAsStringAsync());
                            // attachmentModel.contentByte = response.Content.ReadAsByteArrayAsync().Result;
                            //  attachmentModel.contentType = response.Content.Headers.ContentType.MediaType;


                        }
                    }
                }



                attachmentModel2 = await DownloadMediaAsync(attachmentModel.url, fbToken);


                attachmentModel2.sha256 = attachmentModel.sha256;
                attachmentModel2.messaging_product = attachmentModel.messaging_product;
                attachmentModel2.mime_type = attachmentModel.mime_type;
                attachmentModel2.url = attachmentModel.url;
                attachmentModel2.file_size = attachmentModel.file_size;
                attachmentModel2.id = attachmentModel.id;






            }
            catch
            {


            }



            return attachmentModel2;


        }

        private static async Task<WhatsAppAttachmentModel> DownloadMediaAsync(string mediaurl, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();


            var client = new RestClient(mediaurl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + fbToken);
            IRestResponse response = client.Execute(request);

            attachmentModel.contentByte = response.RawBytes;
            attachmentModel.contentType = response.ContentType;


            return attachmentModel;

        }

        private async void UpdateCustomerChatAsync(string CustomerModel, string url, string type, string massageId)
        {
            try
            {
                CustomerModel customer = JsonConvert.DeserializeObject<CustomerModel>(CustomerModel);


                var text = "";
                if (customer.customerChat != null)
                {
                    text = customer.customerChat.text;

                }

                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                CustomerChat customerChat = new CustomerChat()
                {
                    messageId = massageId,
                    TenantId = customer.TenantId,
                    userId = customer.userId,
                    text = text,
                    type = type,//hjhj
                    CreateDate = DateTime.Now,
                    status = (int)Messagestatus.New,
                    sender = MessageSenderType.Customer,
                    ItemType = InfoSeedContainerItemTypes.ConversationItem,
                    mediaUrl = url,
                    UnreadMessagesCount = 0,
                    agentName = "admin",
                    agentId = "",
                };




                var Result = itemsCollection.CreateItemAsync(customerChat).Result;

                //customerModel.customerChat = customerChat;
                // var objCustomer = itemsCollectionCustomer.UpdateItemAsync(customerModel._self, customerModel).Result;

                //SocketIOManager.SendChat(customerChat, customerChat.TenantId.Value);

            }
            catch
            {


            }


        }

        private void AddToSallaLog(string MerchantID, string JsonString)
        {
            try
            {

                var SP_Name = "[dbo].[AddToSallaLog]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("MerchantID",MerchantID),
                    new System.Data.SqlClient.SqlParameter("@JsonString",JsonString)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }

        private void AddOrUpdateSallaToken(string MerchantID, string Access_token, string Refresh_token)
        {
            try
            {

                var SP_Name = "[dbo].[AddOrUpdateSallaToken]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("MerchantID",MerchantID),
                    new System.Data.SqlClient.SqlParameter("@Access_token",Access_token),
                    new System.Data.SqlClient.SqlParameter("@Refresh_token",Refresh_token),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }

        private async Task<SallaInfoModel> SendEmailAsync(string accessToken)
        {
            using var httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://accounts.salla.sa/oauth2/user/info");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<SallaInfoModel>(responseContent);


            var storName = model.data.name;
            var storEmail = model.data.email;
            var storPhonenumber = model.data.mobile;
            var storLogoUrl = model.data.merchant.avatar;


            var body = $@"
                               <html>
                               <body style='font-family:Tahoma, sans-serif;'>
                                   <div dir='rtl' style='text-align:right;'>
                                       <p>مرحبًا <strong>{storName}</strong>،</p>
                                       <p>شكرًا لاختيارك خدمة <strong>Infoseed</strong> عبر منصة سلة.</p>
                                       <p>يسعدنا انضمامك إلينا ونتطلع لدعم نمو متجرك.</p>
                                       <p>فريقنا جاهز لمساعدتك في إعداد وتفعيل شات بوت الواتساب بكل سهولة.</p>
                                       <p>نتطلع للعمل معك!</p>
                                       <p>سيتواصل معك فريقنا في أقرب وقت على رقم الجوال التالي: <strong>{storPhonenumber}</strong></p>
                                       <br />
                                       <p>تحياتنا،<br/>فريق Info-seed</p>
                                   </div>
                                   <hr />
                                   <div dir='ltr' style='margin-top:20px;'>
                                       <p>Hi <strong>{storName}</strong>,</p>
                                       <p>Thank you for choosing <strong>Infoseed</strong> via SALLA.</p>
                                       <p>We're excited to have you on board and look forward to supporting your store's growth.</p>
                                       <p>Our team is ready to help you set up and activate the WhatsApp chatbot with ease.</p>
                                       <p>Looking forward to working with you!</p>
                                       <p>Our team will contact you ASAP on the below mobile number: <strong>{storPhonenumber}</strong></p>
                                       <br />
                                       <p>The Info-seed Team</p>
                                   </div>
                               </body>
                               </html>";

            await SendEmailSalla(storEmail, storName, storPhonenumber, body);


            return model;


        }
        private async Task<IActionResult> SendEmailSalla(string toEmail, string merchantName, string phoneNumber, string body)
        {
            try
            {

                string connectionString = "endpoint=https://inoseedsendemail.unitedstates.communication.azure.com/;accesskey=B8Ah3Setud7u7kjs4W6P2asQjxzGmvvEEq17PZnQyvWnZvkSaqiBJQQJ99AGACULyCpNJ3J3AAAAAZCSCt0S";
                var emailClient = new EmailClient(connectionString);

                var emailContent = new EmailContent("Thanks From Infoseed")
                {
                    Html = body
                };

                var emailRecipients = new EmailRecipients(new[] { new EmailAddress(toEmail) });

                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@8d910114-fe9c-4e5d-b979-da4cd2848a34.azurecomm.net",
                    content: emailContent,
                    recipients: emailRecipients
                );

                EmailSendOperation operation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                // Log or return the error
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }


        private async Task<IActionResult> SendEmailtoInfoseed(string MerchantID, string Access_token, string Refresh_token, SallaInfoModel model)
        {
            try
            {


                var storName = model.data.name;
                var storEmail = model.data.email;
                var storPhonenumber = model.data.mobile;
                var storLogoUrl = model.data.merchant.avatar;

                var store_location = model.data.merchant.store_location;
                var plan = model.data.merchant.plan;
                var status = model.data.merchant.status;
                var domain = model.data.merchant.domain;
                var tax_number = model.data.merchant.tax_number;
                var commercial_number = model.data.merchant.commercial_number;
                var created_at = model.data.merchant.created_at;
                var username = model.data.merchant.username;



                var storRole = model.data.role;



                var toEmail = "Salla.Partner@info-seed.com";// "Hasan.snaid@info-seed.com";//infoseed

                string body = $@"
                                 <html>
                                 <body dir='rtl' style='font-family:Tahoma, sans-serif;'>
                                     <p>مرحبًا فريق إنفوسيد،</p>
                                 
                                     <p>نود إعلامكم أن المتجر <strong>{storName}</strong> قد قام باختيار خدمة إنفوسيد من خلال منصة سلة.</p>
                                 
                                     <p>تفاصيل المتجر:</p>
                                     <ul>
                                         <li><strong>Merchant ID:</strong> {MerchantID}</li>
                                         <li><strong>Access Token:</strong> {Access_token}</li>
                                         <li><strong>Refresh Token:</strong> {Refresh_token}</li>

  <li><strong>Name:</strong> {storName}</li>
  <li><strong>Email:</strong> {storEmail}</li>
  <li><strong>Phonenumber:</strong> {storPhonenumber}</li>


                                         <li><strong>username:</strong> {username}</li>
                                         <li><strong>avatar:</strong> {storLogoUrl}</li>
                                         <li><strong>store_location:</strong> {store_location}</li>
                                         <li><strong>plan:</strong> {plan}</li>
                                         <li><strong>status:</strong> {status}</li>
                                         <li><strong>domain:</strong> {domain}</li>
                                         <li><strong>Role:</strong> {storRole}</li>
                                         <li><strong>tax_number:</strong> {tax_number}</li>
                                         <li><strong>commercial_number:</strong> {commercial_number}</li>
                                         <li><strong>created_at:</strong> {created_at}</li>

                                     </ul>
                                 
                                     <p>يرجى متابعة إعداد الخدمة وتفعيل شات بوت الواتساب لهذا التاجر.</p>
                                 
                                     <br />
                                     <p>مع تحيات،<br/>
                                     نظام الربط التلقائي - Info-seed</p>
                                 </body>
                                 </html>";


                string connectionString = "endpoint=https://inoseedsendemail.unitedstates.communication.azure.com/;accesskey=B8Ah3Setud7u7kjs4W6P2asQjxzGmvvEEq17PZnQyvWnZvkSaqiBJQQJ99AGACULyCpNJ3J3AAAAAZCSCt0S";
                var emailClient = new EmailClient(connectionString);

                var emailContent = new EmailContent("Thanks From Infoseed")
                {
                    Html = body
                };

                var emailRecipients = new EmailRecipients(new[] { new EmailAddress(toEmail) });

                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@8d910114-fe9c-4e5d-b979-da4cd2848a34.azurecomm.net",
                    content: emailContent,
                    recipients: emailRecipients
                );

                EmailSendOperation operation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                // Log or return the error
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
        #endregion



    }
}

using Abp;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.UI;
using AutoMapper;
using Azure;
using Azure.Communication.Email;
using Azure.Storage.Blobs;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Framework.Data;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Framework.Integration.Model;
using Framework.Payment.Implementation.Zoho;
using Framework.Payment.Interfaces.Zoho;
using GeoCoordinatePortable;
using Google.Apis.Auth.OAuth2;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Asset;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Booking;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.AlSarhTravel;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.BotAPI.Models.Daffah;
using Infoseed.MessagingPortal.BotAPI.Models.Dto;
using Infoseed.MessagingPortal.BotAPI.Models.Firebase;
using Infoseed.MessagingPortal.BotAPI.Models.FlwosRestorant;
using Infoseed.MessagingPortal.BotAPI.Models.Location;
using Infoseed.MessagingPortal.BotAPI.Models.NewFolder;
using Infoseed.MessagingPortal.BotAPI.Models.RZ;
using Infoseed.MessagingPortal.BotAPI.Models.Sala;
using Infoseed.MessagingPortal.BotAPI.Models.Tania;
using Infoseed.MessagingPortal.Branches;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Careem_Express;
using Infoseed.MessagingPortal.Configuration.Tenants;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.ContactNotification;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Customers;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.DeliveryCost.Dto;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.Maintenance;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Notifications;
using Infoseed.MessagingPortal.OrderDetails;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.OrderOffers;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Spatial;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Trunking.V1;
using static Infoseed.MessagingPortal.BotAPI.Models.Tania.TaniaValidatePromocodePost;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;
using Item = Infoseed.MessagingPortal.BotAPI.Models.RZ.Item;
namespace Infoseed.MessagingPortal.BotAPI.Controllers
{
    public class BotFlowAPIController : MessagingPortalControllerBase
    {
        private static readonly object CurrentOrder = new object();
        private readonly IAppNotifier _appNotifier;
        private ICaptionBotAppService _captionBotAppService;
        private readonly IHubContext<LiveChatHub> _LiveChatHubhub;
        private readonly IHubContext<MaintenancesHub> _maintenanceshub;
        private readonly IHubContext<SellingRequestHub> _sellingRequestHub;
        private readonly IAreasAppService _iAreasAppService;
        private readonly IItemsAppService _itemsAppService;
        private readonly IUserAppService _iUserAppService;
        private readonly ISellingRequestAppService _iSellingRequestAppService;
        private readonly IAssetAppService _iAssetAppService;
        private readonly IOrdersAppService _iOrdersAppService;
        private readonly IDeliveryCostAppService _iDeliveryCostAppService;
        private readonly ILiveChatAppService _iliveChat;
        private readonly IMenusAppService _menusAppService;
        private readonly IDBService _dbService;
        private readonly IConfiguration _configuration;
        private readonly IEvaluationsAppService _evaluationsAppService;
        private readonly IMapper _mapper;
        private readonly ICustomerBehaviourAppService _cusomerBehaviourAppService;
        private readonly IWhatsAppMessageTemplateAppService _templateAppService;
        private readonly IGeneralAppService _generalAppService;
        public IContactsAPI _contactsAPI;
        public ITicketsAPI _ticketsAPI;
        private readonly IDocumentClient _IDocumentClient;
        private readonly ILoyaltyAppService _loyaltyAppService;
        private readonly ICacheManager _cacheManager;
        private readonly IBookingAppService _bookingAppService;
        private readonly IDashboardUIAppService _dashboardAppService;
        IInvoices _invoices;
        private IUserNotificationManager _userNotificationManager;
        private IContactNotification _contactNotification;
        private readonly RoleManager _roleManager;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IEmailSender _emailSender;
        private readonly ITenantSettingsAppService _tenantSettingsAppService;


        private static readonly string clientId = "82465cce-2e5e-4d82-9f2d-70990fdc6456";
        private static readonly string clientSecret = "c8b76e1ed897edf84f43ae9634d681a1";
        private static readonly string redirectUri = "https://672d-109-237-205-93.ngrok-free.app/callback";
        private static readonly string tokenUrl = "https://accounts.salla.sa/oauth2/token";

        private static readonly string baseUrl = "https://api.salla.dev/";

        private readonly BlobServiceClient _client;
        private readonly BlobContainerClient _blobClient;
        private readonly string containerName = ConfigurationManager.AppSettings["BlobStorageSubscription"].ToString();
        private readonly string connectionString = AppSettingsCoreModel.BlobServiceConnectionStrings;


        public IBotApis _botApis;

        public BotFlowAPIController(
            IEmailSender emailSender,
            IHubContext<SellingRequestHub> sellingRequestHub,
            IHubContext<LiveChatHub> LiveChatHubhub,
            IHubContext<MaintenancesHub> maintenanceshub,
            IConfiguration configuration,
            IDBService dbService,
            IMenusAppService menusAppService,
            IAreasAppService iAreasAppService,
            IUserAppService iUserAppService,
            IItemsAppService itemsAppService,
            ISellingRequestAppService iSellingRequestAppService,
            IAssetAppService iAssetAppService,
            IOrdersAppService iOrdersAppService,
            IDeliveryCostAppService iDeliveryCostAppService,
            ILiveChatAppService iliveChat,
            IEvaluationsAppService evaluationsAppService,
            IMapper mapper,
            ICustomerBehaviourAppService cusomerBehaviourAppService,
            IDocumentClient iDocumentClient,
            IGeneralAppService generalAppService,
            ILoyaltyAppService loyaltyAppService,
            ICacheManager cacheManager,
            IBookingAppService bookingAppService,
            RoleManager roleManager,
            IRepository<UserRole, long> userRoleRepository,
            IUserNotificationManager userNotificationManager,
            IContactNotification contactNotification,
            IAppNotifier appNotifier,
            ICaptionBotAppService captionBotAppService,
            IDashboardUIAppService dashboardAppService,
             IBotApis botApis
            )
        {
            _emailSender = emailSender;
            _menusAppService = menusAppService;
            _LiveChatHubhub = LiveChatHubhub;
            _sellingRequestHub = sellingRequestHub;
            _configuration = configuration;
            _dbService = dbService;
            _maintenanceshub = maintenanceshub;
            _iAreasAppService = iAreasAppService;
            _iUserAppService = iUserAppService;
            _itemsAppService = itemsAppService;
            _iSellingRequestAppService = iSellingRequestAppService;
            _iAssetAppService = iAssetAppService;
            _iOrdersAppService = iOrdersAppService;
            _iDeliveryCostAppService = iDeliveryCostAppService;
            _iliveChat = iliveChat;
            _evaluationsAppService = evaluationsAppService;
            _mapper = mapper;
            _cusomerBehaviourAppService = cusomerBehaviourAppService;
            _generalAppService = generalAppService;
            _contactsAPI = new ContactsAPI(SettingsModel.MgUrl, SettingsModel.MgKey);
            _ticketsAPI = new TicketsAPI(SettingsModel.MgUrl, SettingsModel.MgKey);
            _IDocumentClient = iDocumentClient;
            _cacheManager = cacheManager;
            _loyaltyAppService = loyaltyAppService;
            _invoices = new InvoicesApi(_configuration);
            _bookingAppService = bookingAppService;
            _userRoleRepository = userRoleRepository;

            _roleManager = roleManager;

            _userNotificationManager = userNotificationManager;
            _contactNotification = contactNotification;
            _appNotifier = appNotifier;
            _captionBotAppService = captionBotAppService;
            _dashboardAppService = dashboardAppService;

            _botApis = botApis;

        }


        #region Public




        [HttpPost("uploadfile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is missing.");

            try
            {
                var blobClient = new BlobContainerClient(connectionString, containerName);
                await blobClient.CreateIfNotExistsAsync();

                var blob = blobClient.GetBlobClient(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    await blob.UploadAsync(stream, overwrite: true);
                }

                return Ok(new { url = blob.Uri.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetProductDetailSalla")]

        public async Task<SallaProductDetailModel.Dataa2> GetProductDetailSalla(string TenantId, string sku)
        {
            try
            {
                var accessToken = GetSallaToken(TenantId);


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.salla.dev/admin/v2/products/sku/" + sku);
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<SallaProductDetailModel>(responseContent);

                return model.data;
            }
            catch
            {

                return null;

            }
        }

        [HttpGet("GetListOrderDetailsSalla")]

        public async Task<SallaOrderList.Datumu1> GetListOrderDetailsSalla(string TenantId, string referenceID)
        {
            try
            {
                var accessToken = GetSallaToken(TenantId);


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.salla.dev/admin/v2/orders?reference_id=" + referenceID);
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var model = JsonConvert.DeserializeObject<SallaOrderList>(responseContent);

                if (model.data.Length > 0 && model.data.Length == 1)
                {

                    return model.data.FirstOrDefault();

                }
                else
                {

                    return null;
                }


            }
            catch
            {

                return null;

            }

        }


        private string GetSallaToken(string TenantID)
        {
            string token = "";

            var mID = GetTenantMID(TenantID);

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[SallaToken] where MerchantId=N'" + mID + "'";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);


                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {


                    token = dataSet.Tables[0].Rows[i]["Access_token"].ToString();


                }

                conn.Close();
                da.Dispose();

                return token;
            }
            catch
            {

                return token;

            }


        }
        private string GetTenantMID(string TenantID)
        {
            string MerchantId = "";

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[AbpTenants] where Id=" + TenantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);


                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        MerchantId = dataSet.Tables[0].Rows[i]["MerchantID"].ToString();

                    }
                    catch
                    {

                        MerchantId = "";

                    }




                }

                conn.Close();
                da.Dispose();

                return MerchantId;
            }
            catch
            {

                return MerchantId;

            }

        }
        private async Task<SallaOrderDetailModel> RefreshTokenAsync(string refreshToken)
        {
            using var client = new HttpClient();

            var clientId = "81d603b4-a742-48fb-98e6-0efb00c07088";
            var clientSecret = "5aadbe9baf4b2ad207a3a9ef6bd40fe0";

            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var requestContent = new FormUrlEncodedContent(tokenRequest);
            var response = await client.PostAsync("https://accounts.salla.sa/oauth2/token", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<SallaOrderDetailModel>(responseContent);

            return model; // Or deserialize it to a model and return
        }
        [HttpPost("GetRandomString")]
        public async Task<IActionResult> GetRandomStringAsync([FromBody] TestModelMassage testModelMassage)
        {
            TestRezultMassage testRezultMassage = new TestRezultMassage();

            TestRezultMassage.Contact contact = new TestRezultMassage.Contact();
            List<TestRezultMassage.Contact> lcontact = new List<TestRezultMassage.Contact>();

            TestRezultMassage.Message message = new TestRezultMassage.Message();

            List<TestRezultMassage.Message> lmessage = new List<TestRezultMassage.Message>();



            ErrorTestModel errorTestModel = new ErrorTestModel();
            errorTestModel.error = new ErrorTestModel.Error();
            errorTestModel.error.error_data = new ErrorTestModel.Error_Data();
            errorTestModel.error.type = "OAuthException";
            errorTestModel.error.fbtrace_id = "A2kZDTkX7voPo231y-H1dqF";
            errorTestModel.error.error_data.details = "whatsapp";


            contact.input = testModelMassage.to;
            contact.wa_id = testModelMassage.to;

            lcontact.Add(contact);

            int length = 60;
            if (length <= 0)
            {
                return BadRequest("Length must be greater than 0.");
            }

            string randomString = GenerateRandomString(length);

            message.id = randomString;
            lmessage.Add(message);



            testRezultMassage.messaging_product = "whatsapp";
            testRezultMassage.contacts = lcontact.ToArray();
            testRezultMassage.messages = lmessage.ToArray();


            WebHookModel webHookModel = new WebHookModel();
            webHookModel.TenantId = 27;


            var payload1 = $@"
                             {{
                                     ""object"": ""whatsapp_business_account"",
                                     ""entry"": [
                                         {{
                                             ""id"": ""414976121690433"",
                                             ""changes"": [
                                                 {{
                                                     ""value"": {{
                                                         ""statuses"": [
                                                             {{
                                                                 ""conversation"": {{
                                                                     ""expiration_timestamp"": 0,
                                                                     ""id"": ""7515ec060e84d0e90866ff303d9c2521"",
                                                                     ""origin"": {{ ""billable"": null }}
                                                                 }},
                                                                 ""id"": ""{randomString}"",
                                                                 ""pricing"": {{
                                                                     ""billable"": true,
                                                                     ""category"": ""utility"",
                                                                     ""pricing_model"": ""CBP""
                                                                 }},
                                                                 ""recipient_id"": ""{contact.input}"",
                                                                 ""status"": ""send"",
                                                                 ""timestamp"": ""1737026054"",
                                                                 ""type"": null,
                                                                 ""errors"": null
                                                             }}
                                                         ],
                                                         ""messaging_product"": ""whatsapp"",
                                                         ""metadata"": {{
                                                             ""display_phone_number"": ""{contact.input}"",
                                                             ""phone_number_id"": ""395752056951374""
                                                         }},
                                                         ""contacts"": null,
                                                         ""messages"": null
                                                     }},
                                                     ""field"": ""messages""
                                                 }}
                                             ]
                                         }}
                                     ]
                                 }}";

            var payload2 = $@"
                             {{
                                     ""object"": ""whatsapp_business_account"",
                                     ""entry"": [
                                         {{
                                             ""id"": ""414976121690433"",
                                             ""changes"": [
                                                 {{
                                                     ""value"": {{
                                                         ""statuses"": [
                                                             {{
                                                                 ""conversation"": {{
                                                                     ""expiration_timestamp"": 0,
                                                                     ""id"": ""7515ec060e84d0e90866ff303d9c2521"",
                                                                     ""origin"": {{ ""billable"": null }}
                                                                 }},
                                                                 ""id"": ""{randomString}"",
                                                                 ""pricing"": {{
                                                                     ""billable"": true,
                                                                     ""category"": ""utility"",
                                                                     ""pricing_model"": ""CBP""
                                                                 }},
                                                                 ""recipient_id"": ""{contact.input}"",
                                                                 ""status"": ""delivered"",
                                                                 ""timestamp"": ""1737026054"",
                                                                 ""type"": null,
                                                                 ""errors"": null
                                                             }}
                                                         ],
                                                         ""messaging_product"": ""whatsapp"",
                                                         ""metadata"": {{
                                                             ""display_phone_number"": ""{contact.input}"",
                                                             ""phone_number_id"": ""395752056951374""
                                                         }},
                                                         ""contacts"": null,
                                                         ""messages"": null
                                                     }},
                                                     ""field"": ""messages""
                                                 }}
                                             ]
                                         }}
                                     ]
                                 }}";
            var payload3 = $@"
                             {{
                                     ""object"": ""whatsapp_business_account"",
                                     ""entry"": [
                                         {{
                                             ""id"": ""414976121690433"",
                                             ""changes"": [
                                                 {{
                                                     ""value"": {{
                                                         ""statuses"": [
                                                             {{
                                                                 ""conversation"": {{
                                                                     ""expiration_timestamp"": 0,
                                                                     ""id"": ""7515ec060e84d0e90866ff303d9c2521"",
                                                                     ""origin"": {{ ""billable"": null }}
                                                                 }},
                                                                 ""id"": ""{randomString}"",
                                                                 ""pricing"": {{
                                                                     ""billable"": true,
                                                                     ""category"": ""utility"",
                                                                     ""pricing_model"": ""CBP""
                                                                 }},
                                                                 ""recipient_id"": ""{contact.input}"",
                                                                 ""status"": ""read"",
                                                                 ""timestamp"": ""1737026054"",
                                                                 ""type"": null,
                                                                 ""errors"": null
                                                             }}
                                                         ],
                                                         ""messaging_product"": ""whatsapp"",
                                                         ""metadata"": {{
                                                             ""display_phone_number"": ""{contact.input}"",
                                                             ""phone_number_id"": ""395752056951374""
                                                         }},
                                                         ""contacts"": null,
                                                         ""messages"": null
                                                     }},
                                                     ""field"": ""messages""
                                                 }}
                                             ]
                                         }}
                                     ]
                                 }}";

            WhatsAppModel model1 = JsonConvert.DeserializeObject<WhatsAppModel>(payload1);
            WhatsAppModel model2 = JsonConvert.DeserializeObject<WhatsAppModel>(payload2);
            WhatsAppModel model3 = JsonConvert.DeserializeObject<WhatsAppModel>(payload3);









            //webHookModel.whatsApp=model1;
            //SetStatusInQueuestg(webHookModel);
            //webHookModel.whatsApp=model2;
            //SetStatusInQueuestg(webHookModel);
            //webHookModel.whatsApp=model3;
            //SetStatusInQueuestg(webHookModel);
            //await Task.Delay(100);
            switch (contact.input)
            {
                case "recipient_number_2":
                    errorTestModel.error.code = 131000;
                    errorTestModel.error.error_data.messaging_product = "something wrong";
                    return BadRequest(errorTestModel);

                case "recipient_number_10":
                    errorTestModel.error.code = 131016;
                    errorTestModel.error.error_data.messaging_product = "Service unavailable";
                    return BadRequest(errorTestModel);

                case "recipient_number_20":
                    errorTestModel.error.code = 130429;
                    errorTestModel.error.error_data.messaging_product = "Rate limit hit";
                    return BadRequest(errorTestModel);

                case "recipient_number_40":
                    errorTestModel.error.code = 132015;
                    errorTestModel.error.error_data.messaging_product = "Template is Paused";
                    return BadRequest(errorTestModel);//  Stop

                default:

                    return Ok(testRezultMassage);
            }


        }
        [HttpGet("GetTenantlistforTenantId")]

        public async Task<string> GetTenantlistforTenantId(bool TenantId = false)
        {

            var itemsCollection = new DocumentCosmoseDB<MultiTenancy.TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var tenants = await itemsCollection.GetItemListAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant);
            foreach (var tan in tenants)
            {
                if (TenantId)
                {
                    tan.TenantId = null;
                    await itemsCollection.UpdateItemAsync(tan._self, tan);
                }

            }

            return "";
        }

        private void SetStatusInQueuestg(WebHookModel message)
        {
            try
            {

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storgewebhookstg;AccountKey=vghTAbynNvoX1pE9yRcYu1TI2OIdClIg+bMPhmn4six6j2WVEIkDX5/0ksU8GHSZx7KAqAG+OvgC+AStHvRYOw==;EndpointSuffix=core.windows.net");
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("webhookstatusfun");
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
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        [Route("EndDialog")]
        [HttpGet]
        public async Task<string> EndDialog(string phonenumber, string teanantId)
        {
            try
            {


                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == teanantId + "_" + phonenumber);//&& a.TenantId== TenantId


                var Customer = customerResult.Result;
                if (Customer != null)
                {

                    Customer.IsTemplateFlow = false;
                    Customer.templateId = "";
                    Customer.CampaignId = "";
                    Customer.TemplateFlowDate = null;

                }

                var Result = itemsCollection.UpdateItemAsync(Customer._self, Customer).Result;

                return "ok";
            }
            catch (Exception ex)
            {
                return "no";
            }
        }



        [Route("SendEmail")]
        [HttpGet]
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // This code retrieves your connection string from an environment variable.
            string connectionString = "endpoint=https://inoseedsendemail.unitedstates.communication.azure.com/;accesskey=B8Ah3Setud7u7kjs4W6P2asQjxzGmvvEEq17PZnQyvWnZvkSaqiBJQQJ99AGACULyCpNJ3J3AAAAAZCSCt0S";
            var emailClient = new EmailClient(connectionString);


            EmailSendOperation emailSendOperation = emailClient.Send(
                WaitUntil.Completed,
                senderAddress: "DoNotReply@8d910114-fe9c-4e5d-b979-da4cd2848a34.azurecomm.net",
                recipientAddress: toEmail,
                subject: subject,
                htmlContent: "<html><h1>" + body + "</h1l></html>",
                plainTextContent: body);
        }

        [Route("NearestBranch")]
        [HttpGet]
        public async Task<string> NearestBranch(string longandlat, string teanantId)
        {
            try
            {
                var log = "";
                var lat = "";

                if (longandlat.Contains("maps.google.com"))
                {

                    var str = longandlat.Replace("https://maps.google.com/?q=", "");
                    log = str.Split(",")[0];
                    lat = str.Split(",")[1];

                }
                else
                {
                    log = longandlat.Split(",")[0];
                    lat = longandlat.Split(",")[1];


                }


                var area = getNearbyArea(int.Parse(teanantId), double.Parse(log), double.Parse(lat));

                return area.AreaName + " \n  " + "https://maps.google.com/?q=" + area.Latitude + "," + area.Longitude;
            }
            catch
            {
                return "حدث خطأ";

            }

        }
        private static List<SallaTokenResponse> GetAccessTokenByMerchantID(string merchantId)
        {
            var SP_Name = Constants.Salla.SP_GetSallaTokenByMerchantId;

            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@MerchantId", merchantId)
                };

            return SqlDataHelper.ExecuteReader(
                SP_Name,
                sqlParameters.ToArray(),
                MapAccessToken,
                AppSettingsModel.ConnectionStrings
            ).ToList();
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

        public Campaincs GetCampaignsByCampaignId(int campaignId)
        {
            try
            {
                string SP_Name = Constants.WhatsAppCampaign.SP_GetCampaignsByCampaignId;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                new System.Data.SqlClient.SqlParameter("@CampaignId", campaignId)
                };

                return SqlDataHelper.ExecuteReader(SP_Name,
                                               sqlParameters.ToArray(),
                                               ConvertToCampaignDto,
                                               AppSettingsModel.ConnectionStrings)
                                  .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetCampaignsByCampaignId: " + ex.Message);

                throw;
            }
        }

        private static Campaincs ConvertToCampaignDto(IDataReader dataReader)
        {
            return new Campaincs
            {
                //Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                //TenantId = SqlDataHelper.GetValue<long>(dataReader, "TenantId"),
                CampaignId = SqlDataHelper.GetValue<int>(dataReader, "CampaignId"),
                //TemplateId = SqlDataHelper.GetValue<int>(dataReader, "TemplateId")
                ContactsJson = SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"),
                CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                //UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId"),
                IsExternalContact = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                //JobName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                CampaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),
                TemplateJson = SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"),
                TemplateVariables = SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"),
                HeaderVariablesTemplate = SqlDataHelper.GetValue<string>(dataReader, "HeaderVariablesTemplate"),
                URLButton1VariablesTemplate = SqlDataHelper.GetValue<string>(dataReader, "URLButton1VariablesTemplate"),
                URLButton2VariablesTemplate = SqlDataHelper.GetValue<string>(dataReader, "URLButton2VariablesTemplate"),
                CarouselVariablesTemplate = SqlDataHelper.GetValue<string>(dataReader, "carouselVariabllesTemplate")
            };
        }

        public TenantEditDto GetTenantById(int tenantId)
        {
            try
            {
                string SP_Name = Constants.Tenant.SP_GetTenantById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
        {
            new System.Data.SqlClient.SqlParameter("@Id", tenantId)
        };

                return SqlDataHelper.ExecuteReader(
                            SP_Name,
                            sqlParameters.ToArray(),
                            ConvertToTenantDto,
                            AppSettingsModel.ConnectionStrings)
                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static TenantEditDto ConvertToTenantDto(IDataReader dataReader)
        {
            return new TenantEditDto
            {
                Id = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                ConnectionString = SqlDataHelper.GetValue<string>(dataReader, "ConnectionString"),
                IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive"),
                Name = SqlDataHelper.GetValue<string>(dataReader, "Name"),
                TenancyName = SqlDataHelper.GetValue<string>(dataReader, "TenancyName"),
                MerchantID = SqlDataHelper.GetValue<string>(dataReader, "MerchantID"),

            };
        }



        [Route("UpdateOrderStatus")]
        [HttpPost]
        public async Task<string> UpdateOrderStatusAsync(string phoneNumber, string tenantId, string newStatusSlug)
        {
            var userId = tenantId + "_" + phoneNumber;

            //var userId = "1234";

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == tenantId + "_" + phoneNumber);

            int tenantId_INT = int.Parse(tenantId);
            int campaignId;
            if (!int.TryParse(customerResult.CampaignId, out campaignId))
            {
                throw new Exception("CampaignId is not a valid int.");
            }
            long campaignId2 = long.Parse(customerResult.CampaignId);

            var campaignDetails = GetCampaignsByCampaignId(campaignId);
            var x = 0;

            var templateVariables = JsonConvert.DeserializeObject<TemplateVariables>(campaignDetails.TemplateVariables);

            var telantModel = GetTenantById(tenantId_INT);


            var accessToken = GetAccessTokenByMerchantID(telantModel.MerchantID).FirstOrDefault();


            //int orderId= int.Parse(templateVariables.VarTwo);VarSixteen

            var url = $"https://api.salla.dev/admin/v2/orders/{templateVariables.VarSixteen}/status";

            var jsonBody = $"{{ \"slug\": \"{newStatusSlug}\" }}";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);


            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return $"✅ Order updated to '{newStatusSlug}' successfully.";
                    }
                    else
                    {
                        return $"❌ Failed to update order. StatusCode: {(int)response.StatusCode}. Response: {responseContent}";
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return $"❌ Exception occurred: {ex.Message}";
            }
        }

        #endregion

        #region JoPetrol
        /// <summary>
        /// Intgreation with Jo Petrol For Calculet the prise  (Jo Petrol Bot)
        /// </summary>
        /// <param name="liters">Input User Liters</param>
        /// <param name="fuelname">Input User fuelname</param>
        /// <returns></returns>
        [Route("CalculetPriseJoPetrol")]
        [HttpGet]
        public async Task<string> CalculetPriseJoPetrol(string liters, string fuelname)
        {
            try
            {
                bool isInteger = int.TryParse(liters, out _);

                if (isInteger)
                {
                    if (int.Parse(liters) >= 500 && int.Parse(liters) <= 16000 && fuelname != null)
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, "https://loyalty.jppmc.jo/loyalty/public/api/user/fuelprice");
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        //Console.WriteLine(await response.Content.ReadAsStringAsync());
                        double totalPrise = 0;

                        switch (fuelname)
                        {
                            case "كاز / KER":
                                fuelname = "KEROSINE";
                                break;
                            case "سولار / Diesel":
                                fuelname = "DIESEL";
                                break;
                            case "بنزين 95 / OCT95":
                                fuelname = "GAZOLINE 95";
                                break;
                            case "بنزين 90 / OCT90":
                                fuelname = "GAZOLINE 90";
                                break;

                            case "Gas / KER":
                                fuelname = "KEROSINE";
                                break;
                            case "Diesel (Solar)":
                                fuelname = "DIESEL";
                                break;
                            case "Benzene 95 / OCT95":
                                fuelname = "GAZOLINE 95";
                                break;
                            case "Benzene 90 / OCT90":
                                fuelname = "GAZOLINE 90";
                                break;



                            default:
                                fuelname = null;
                                break;
                        }

                        if (response.IsSuccessStatusCode && fuelname != null)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var LastResponse = JsonConvert.DeserializeObject<JoPetrolPriceModel.Rootobject>(responseContent);

                            // Access Fuelprice array within the LastResponse
                            foreach (var fuelPrice in LastResponse.Fuelprice)
                            {
                                if (fuelname == fuelPrice.fuelname)
                                {
                                    totalPrise = int.Parse(liters) * (0.015 + double.Parse(fuelPrice.price));
                                    totalPrise = Math.Round(totalPrise, 2);
                                    return totalPrise.ToString();
                                }
                            }

                        }
                    }


                    return "0";
                }



                return "-1";
                //Quantity in liters = 500 L
                //total price = Quantity in liters(0.015 + The daily price of oil)
                //total price = 500 * (0.015 + 0.715)
            }
            catch (Exception ex)
            {
                return "-1";

            }
        }

        [Route("GetDateJoPetrol")]
        [HttpGet]
        public async Task<string> GetDateJoPetrol()
        {
            CultureInfo arabicCulture = new CultureInfo("ar-EG");

            // Define the date
            DateTime date = DateTime.UtcNow.AddHours(-3);

            // Format the date as "MMMM-yyyy" (e.g., "February-2024") using Arabic culture
            return date.ToString("MM-yyyy", arabicCulture);

        }
        [Route("GetLastFiveRequestJoPetrol")]
        [HttpGet]
        public string GetLastFiveRequestJoPetrol(int contactId, int tenantId)

        {
            try
            {
                return getLastFiveRequest(contactId, tenantId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Route("FormatEmailJoPetrol")]
        [HttpGet]
        public async Task<string> FormatEmailJoPetrol(string str)
        {
            try
            {

                var inpouts = str.Split(",");



                // Split the string by comma
                var items = inpouts.ToList();

                // Get the last item
                string lastItem2 = items.Last();

                // Remove the last item
                items.RemoveAt(items.Count - 1);


                string lastItem1 = items.Last();

                // Remove the last item
                items.RemoveAt(items.Count - 1);


                string email = "\n\n";

                foreach (var item in items)
                {
                    email = email + item + "\n";
                }



                if (IsLongitude(lastItem2))
                {
                    email = email + lastItem1 + "," + lastItem2 + "\n";

                }
                else
                {
                    email = email + lastItem1 + "\n" + lastItem2 + "\n";
                }


                email = email + "\n";



                return email;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Route("GasOrdering")]
        [HttpGet]
        public async Task<string> GasOrdering(string phonenumber, string lang = "ar-EG")

        {
            try
            {
                var str = "";
                if (phonenumber.StartsWith("962"))
                {
                    phonenumber = "0" + phonenumber.Substring(3);
                }



                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "http://212.118.10.213:5050/Gas_Ordering_Data_API/api/Order/GetData?CompanyID=1&CustTelNo=" + phonenumber);
                request.Headers.Add("accept", "text/plain");
                request.Headers.Add("Authorization", "Bearer " + GetTokenAsync().Result);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                var responseContent = await response.Content.ReadAsStringAsync();

                var LastResponse = JsonConvert.DeserializeObject<GasOrderingModel.Rootobject>(responseContent);

                if (LastResponse != null)
                {
                    if (LastResponse.dataList.Length > 0)
                    {
                        var list = LastResponse.dataList.ToList();


                        str = "-------------------\n";

                        foreach (var ord in list)
                        {

                            if (lang == "ar-EG")
                            {

                                str = str + "*النوع* :" + ord.itemName + "\n";
                                str = str + "*الكمية باللتر* :" + ord.delivered_Qty + "\n";
                                //str=str+"*السعر* :"+ord.itemPrice+"\n";//Price on the date of the customer's request
                                str = str + "*سعر اللتر شامل أجور النقل* :" + string.Format("{0:0.000}", ord.literPriceWithCarryCost) + "\n";
                                str = str + "*المبلغ الإجمالي* :" + string.Format("{0:0.000}", ord.totalPriceWithCarryCost) + "\n";
                                str = str + "*تاريخ وصول الطلب* :" + ord.order_DeliveryDateTime.ToString("yyyy-MM-dd") + "\n";


                            }
                            else
                            {


                                str = str + "*Type* :" + ord.itemName.Replace("سولار", "Solar") + "\n";
                                str = str + "*Quantity in liters* :" + ord.delivered_Qty + "\n";
                                //str=str+"*السعر* :"+ord.itemPrice+"\n";//Price on the date of the customer's request
                                str = str + "*Price Per Litter Including Transport* :" + string.Format("{0:0.000}", ord.literPriceWithCarryCost) + "\n";
                                str = str + "*Total Amount* :" + string.Format("{0:0.000}", ord.totalPriceWithCarryCost) + "\n";
                                str = str + "*Actual Delivery Date* :" + ord.order_DeliveryDateTime.ToString("yyyy-MM-dd") + "\n";
                            }




                            str = str + "-------------------\n";

                        }

                        str = str;// +"\n-------------------";
                    }
                    else
                    {
                        if (lang == "ar-EG")
                        {
                            return "لا يوجد طلبات سابقة";
                        }
                        else
                        {
                            return "No previous requests";
                        }

                    }


                }
                else
                {

                    if (lang == "ar-EG")
                    {
                        return "لا يوجد طلبات سابقة";
                    }
                    else
                    {
                        return "No previous requests";
                    }
                }



                return str;
            }
            catch (Exception ex)
            {
                return "لا يوجد طلبات سابقة";
            }
        }

        private async Task<string> GetTokenAsync()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://212.118.10.213:5050/Gas_Ordering_Data_API/api/Login");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent("{\r\n  \"userId\": \"infoseed\",\r\n  \"userPWD\": \"infoseed\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();


                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                return "";

            }

        }


        // Regex pattern for latitude (-90 to 90)
        private static readonly string latitudePattern = @"^(\+|-)?([1-8]?\d(\.\d+)?|90(\.0+)?)$";

        // Regex pattern for longitude (-180 to 180)
        private static readonly string longitudePattern = @"^(\+|-)?((1[0-7]\d(\.\d+)?|180(\.0+)?|[1-9]?\d(\.\d+)?))$";

        public static bool IsLatitude(string input)
        {
            return Regex.IsMatch(input, latitudePattern);
        }

        public static bool IsLongitude(string input)
        {
            return Regex.IsMatch(input, longitudePattern);
        }

        private string getLastFiveRequest(int contactId, int tenantId)
        {
            try
            {
                var SP_Name = Constants.JoPetrolBot.SP_JoPetrolGetLastFiveRequest;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@contactId",contactId),
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                var joPetrolPhoneModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.JoPetrolRequestDto, AppSettingsModel.ConnectionStrings).ToList();
                if (joPetrolPhoneModel.Count != 0)
                {
                    string laststring = "";

                    foreach (var model in joPetrolPhoneModel)
                    {

                        var str = model.Split(",");
                        foreach (var str2 in str)
                        {
                            laststring += str2 + Environment.NewLine + " ";
                        }
                        laststring += "----------" + Environment.NewLine;
                    }
                    return laststring;
                }
                return "لا يوجد لديك فواتير سابقه";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Daffah
        [Route("DaffahGetOrdersByPhoneNumber")]
        [HttpGet]
        public async Task<string> DaffahGetOrdersByPhoneNumber(string phonenumber)
        {
            try
            {
                // phonenumber="0541081205";
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://infoseed.daffah.sa/getAllOrders.php?telephone=" + phonenumber + "&accessToken=n9EX/Hd4GvX8Q5jtP1zOq7x1l5eOiMw5FikN8ZDbzgE=");
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var LastResponse = JsonConvert.DeserializeObject<DaffaOrdersModel>(json);
                string rez = "------------------ \r\n\r\n";

                if (LastResponse.orders != null)
                {
                    int count = 0;
                    foreach (var order in LastResponse.orders)
                    {
                        if (count > 3)
                        {
                            break;
                        }
                        rez = rez + "*Order number* :" + order.increment_id.ToString() + "\r\n";
                        rez = rez + "*Order Date* :" + order.order_date.ToString() + "\r\n";
                        rez = rez + "*Total* :" + order.total.ToString() + "\r\n";
                        rez = rez + "*Status* :" + order.status.ToString() + "\r\n\r\n";


                        rez = rez + "------------------ \r\n";
                        rez = rez + "------------------ \r\n\r\n";

                        count++;
                    }
                    rez = rez + "------------------";

                }
                else
                {
                    rez = "No orders found";

                }


                return rez;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [Route("DaffahGetLastOrdersByPhoneNumber")]
        [HttpGet]
        public async Task<string> DaffahGetLastOrdersByPhoneNumber(string phonenumber)
        {
            try
            {

                DaffaOrdersModel.Order order1 = new DaffaOrdersModel.Order();
                // phonenumber="0541081205";
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://infoseed.daffah.sa/getAllOrders.php?telephone=" + phonenumber + "&accessToken=n9EX/Hd4GvX8Q5jtP1zOq7x1l5eOiMw5FikN8ZDbzgE=");
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var LastResponse = JsonConvert.DeserializeObject<DaffaOrdersModel>(json);
                string rez = "";// "------------------ \r\n\r\n";

                if (LastResponse.orders != null)
                {
                    order1 = LastResponse.orders.LastOrDefault();
                    rez = "أهلاً بك عميلنا العزيز..✋ \r\n";
                    rez = rez + "*لديكم طلب برقم* :" + order1.increment_id.ToString() + "\r\n";
                    rez = rez + "*وحالة الطلب* :" + order1.status.ToString() + "\r\n\r\n";
                    rez = rez + "*تاريخ إنشاء الطلب* :" + order1.order_date.ToString() + "\r\n";
                    rez = rez + "*قيمة الطلب* :" + order1.total.ToString() + "\r\n";

                    order1.textorder = rez;
                }


                return order1.textorder;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        [Route("DaffahGetLastOrdersNumberByPhoneNumber")]
        [HttpGet]
        public async Task<string> DaffahGetLastOrdersNumberByPhoneNumber(string phonenumber)
        {
            try
            {

                DaffaOrdersModel.Order order1 = new DaffaOrdersModel.Order();
                // phonenumber="0541081205";
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://infoseed.daffah.sa/getAllOrders.php?telephone=" + phonenumber + "&accessToken=n9EX/Hd4GvX8Q5jtP1zOq7x1l5eOiMw5FikN8ZDbzgE=");
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var LastResponse = JsonConvert.DeserializeObject<DaffaOrdersModel>(json);
                string rez = "";// "------------------ \r\n\r\n";

                if (LastResponse.orders != null)
                {
                    order1 = LastResponse.orders.LastOrDefault();

                    rez = rez + "*Order number* :" + order1.increment_id.ToString() + "\r\n";
                    rez = rez + "*Order Date* :" + order1.order_date.ToString() + "\r\n";
                    rez = rez + "*Total* :" + order1.total.ToString() + "\r\n";
                    // rez=rez+"*Status* :"+order1.status.ToString()+"\r\n\r\n";

                    order1.textorder = rez;
                }


                return order1.increment_id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("DaffahUpdateOrderStatus")]
        [HttpGet]
        public async Task<string> DaffahUpdateOrderStatus(string phonenumber, string ordernumber, string status, string teanantId)
        {
            try
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseed.daffah.sa/updateOrderStatus.php");
                request.Headers.Add("Incrementid", ordernumber);
                request.Headers.Add("Status", status);
                request.Headers.Add("Accesstoken", "n9EX/Hd4GvX8Q5jtP1zOq7x1l5eOiMw5FikN8ZDbzgE=");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());


                return "ok";
            }
            catch (Exception ex)
            {
                return "no";
            }
        }



        #endregion


        #region salla

        [Route("RedirectToSalla")]
        [HttpGet]
        public IActionResult RedirectToSalla()
        {
            string clientId = "82465cce-2e5e-4d82-9f2d-70990fdc6456";
            string redirectUri = Uri.EscapeDataString("https://93ce-109-237-205-93.ngrok-free.app/callback");
            string scope = "orders.read";

            string authorizationUrl = $"https://accounts.salla.sa/oauth2/authorize?response_type=code" +
                                      $"&client_id={clientId}" +
                                      $"&redirect_uri={redirectUri}" +
                                      $"&scope={scope}";

            return Redirect(authorizationUrl);
        }


        [Route("SallaWebHook")]
        [HttpPost]
        public IActionResult SallaWebHook([FromBody] SallaModel jsonData)
        {
            if (jsonData == null)
            {
                return BadRequest("Invalid JSON payload");
            }

            Console.WriteLine($"Received event: {jsonData._event}");
            Console.WriteLine($"Data: {JsonConvert.SerializeObject(jsonData.data)}");


            var x = 0;
            var url = "https://s.salla.sa/orders/edit/";

            try
            {
                x = jsonData.data.skus.FirstOrDefault().product_id;
                url = url + x;
            }
            catch
            {

            }


            return Ok(new { Message = "Webhook received successfully", ReceivedData = url });
        }


        [Route("Callback")]
        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code is missing.");
            }

            var tokenResponse = await GetAccessToken(code);

            if (tokenResponse == null)
            {
                return BadRequest("Failed to retrieve access token.");
            }

            return Ok(tokenResponse); // This contains access_token and refresh_token
        }
        [Route("GetAccessToken")]
        [HttpGet]
        public async Task<string> GetAccessToken(string code)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "code", code }
            };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(tokenUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching access token: {responseString}");
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                return jsonResponse.access_token;
            }
        }


        public async Task<string> GetOrders(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"{baseUrl}admin/orders");
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching orders: {responseString}");
                }

                return responseString;
            }
        }
        #endregion

        #region Tania    
        [Route("TaniaCheckNumber")]
        [HttpGet]
        public async Task<bool> TaniaCheckNumber(string number)
        {
            if (double.TryParse(number, out double decimalResult) && decimalResult == 0 && decimalResult > 0)
            {
                return false; // It's 0
            }

            if (int.TryParse(number, out int result))
            {


                if (result <= 0)
                {

                    return false; // It's 0

                }
                return true; // It's a valid integer
            }



            return false; // It's a decimal number
        }

        [Route("TaniaPromoOffers")]
        [HttpGet]
        public async Task<TaniaPromoOffersModel.Offer> TaniaPromoOffers()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/promo-offers");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var LastResponse = JsonConvert.DeserializeObject<TaniaPromoOffersModel>(responseContent);


                return LastResponse.offers[0];

            }
            catch (Exception ex)
            {
                return new TaniaPromoOffersModel.Offer();
            }
        }


        [Route("TaniaCustomerCheck")]
        [HttpGet]
        public async Task<TaniaCustomerCheckModel> TaniaCustomerCheck(string phonenumber)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/customer-check");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{\r\n\"mobile\":\"" + phonenumber + "\"\r\n}\r\n");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var LastResponse = JsonConvert.DeserializeObject<TaniaCustomerCheckModel>(responseContent);


                return LastResponse;

            }
            catch (Exception ex)
            {
                return new TaniaCustomerCheckModel() { code = 500 };
            }
        }


        [Route("TaniaGetProductsByLoc")]
        [HttpGet]
        public async Task<string> TaniaGetProductsByLoc(string phonenumber, string LatAndLong, string addressId = "", int step = 1, string cat = "كرتون", string lang = "ar", bool IsSelectoffer = false)
        {
            try
            {
                var LastResponse = new TaniaGetProductsByLocModel() { code = 404 };

                if (addressId == "-1")
                {
                    var lat = LatAndLong.Split(",")[0];
                    var Long = LatAndLong.Split(",")[1];


                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                    HttpClientHandler handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                    };

                    using var client = new HttpClient(handler);

                    var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/get-products-by-loc");
                    request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");

                    // Correctly set content type to application/json with UTF-8 encoding
                    var content = new StringContent("{\r\n\"latitude\":\"" + lat + "\",\r\n\"longitude\":\"" + Long + "\",\r\n\"mobile\":\"" + phonenumber + "\"\r\n}");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = content;

                    try
                    {
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var responseContent = await response.Content.ReadAsStringAsync();
                        LastResponse = JsonConvert.DeserializeObject<TaniaGetProductsByLocModel>(responseContent);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request error: {ex.Message}");
                        Console.WriteLine(ex.InnerException?.Message);
                    }

                    //var responseContent = await response.Content.ReadAsStringAsync();
                    // LastResponse = JsonConvert.DeserializeObject<TaniaGetProductsByLocModel>(responseContent);

                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                    HttpClientHandler handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                    };

                    using var client = new HttpClient(handler);
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/get-products-by-loc");
                    request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                    var content = new StringContent("{\r\n\"address_id\":\"" + addressId + "\",\r\n\"mobile\":\"" + phonenumber + "\"\r\n}\r\n");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    LastResponse = JsonConvert.DeserializeObject<TaniaGetProductsByLocModel>(responseContent);
                }






                if (LastResponse.code == 200)
                {

                    if (step == 1)
                    {

                        //var str = "---------------------- \r\n\r\n";
                        //var count = 1;
                        //foreach (var item in LastResponse.products)
                        //{
                        //    if (lang=="ar")
                        //    {
                        //        str+=count+"-"+item.product_name_ar+"\r\n";

                        //    }
                        //    else
                        //    {
                        //        str+=count+"-"+item.product_name_en+"\r\n";
                        //    }

                        //    count++;
                        //}

                        //str=str+"---------------------- \r\n\r\n";

                        return LastResponse.address_id.ToString();

                    }
                    else if (step == 2)
                    {
                        var str = "";
                        foreach (var item in LastResponse.products)
                        {

                            if (lang == "ar")
                            {
                                str += item.category_name_ar + ",";

                            }
                            else
                            {
                                str += item.category_name_en + ",";
                            }



                        }

                        str = string.Join(",", str.Split(',').Distinct());

                        return str;

                    }
                    else
                    {

                        var str = "";
                        foreach (var item in LastResponse.products)
                        {
                            if (lang == "ar")
                            {
                                if (item.category_name_ar == cat)
                                {
                                    str += item.product_name_ar + ",";
                                }

                            }
                            else
                            {
                                if (item.category_name_en == cat)
                                {
                                    str += item.product_name_en + ",";

                                }

                            }



                        }


                        if (!IsSelectoffer)
                        {
                            if (lang == "ar" && cat == "كرتون")
                            {

                                str += "جالون";

                            }
                            if (lang == "ar" && cat == "جالون")
                            {

                                str += "كرتون";

                            }
                            if (lang == "en" && cat == "Carton")
                            {

                                str += "Gallon";

                            }
                            if (lang == "en" && cat == "Gallon")
                            {

                                str += "Carton";

                            }
                        }


                        return str;
                    }

                }
                else
                {
                    return "-1";

                }


            }
            catch (Exception ex)
            {
                return "-1";
            }
        }


        [Route("TaniaGetProductsId")]
        [HttpGet]
        public async Task<TaniaGetProductsByLocModel.Product> TaniaGetProductsId(string phonenumber, string addressId, string ProductName, string lang = "ar")
        {
            try
            {


                TaniaGetProductsByLocModel.Product prods = new TaniaGetProductsByLocModel.Product();
                var LastResponse = new TaniaGetProductsByLocModel() { code = 404 };


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/get-products-by-loc");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{\r\n\"address_id\":\"" + addressId + "\",\r\n\"mobile\":\"" + phonenumber + "\"\r\n}\r\n");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                LastResponse = JsonConvert.DeserializeObject<TaniaGetProductsByLocModel>(responseContent);
                Dictionary<string, string> productMappings = new Dictionary<string, string>
                         {
                             { "200ml*48-21.8SR", "Tania 200ml(1 Carton - 48 Bottles)" },
                             { "200ml*40-19.26SR", "Tania 200ml (1 Carton - 40 Bottles)" },
                             { "330ml*40-21.8SR", "Tania 330ml(1 Carton - 40 Bottles)" },
                             { "3.8L*4-16.1SR", "Tania 3.8L(1 Carton - 4 Bottles)" },
                             { "5L*4-18.09SR", "Tania 5L(1 Carton - 4 Bottles)" },
                             { "1500ml*6-8.98SR", "Tania 1500ml(1 Shrink - 6 Bottles)" },
                             { "330ml*20-10.35SR", "Tania 330ml (1 Shrink - 20 Bottles)" },
                             { "5GRefill-8.5SR", "Tania 5G Refill(18.9 Liters)" },
                             { "5GNew-13.8SR", "Tania 5G New(18.9 Liters)" },
                              { "MocabatICE-12.08SR", "Mocabat ICE(400ml*8)" },
                              { "600ml*24-18.18SR", "Tania 600 ml (1 Carton - 24 Bottles )" },

                              { "600مل*24-18.18رس", "تانيا 600 مل(1 كرتون - 24 عبوة)" },
                             { "200 مل*48-21.8رس", "تانيا 200 مل(1 كرتون - 48 عبوة)" },
                             { "200مل*40-19.26رس", "تانيا 200 مل (1 كرتون - 40 عبوة)" },
                             { "330مل*40-21.8رس", "تانيا 330 مل(1 كرتون - 40 عبوة)" },
                             { "3.8لتر*4-16.1رس", "تانيا 3.8 لتر(1 كرتون - 4 عبوة)" },
                             { "5لتر*4-18.09رس", "تانيا 5 لتر(1 كرتون - 4 عبوة)" },
                             { "1500مل*6-8.98رس", "تانيا 1500 مل(1 شرنك - 6 عبوة)" },
                             { "330مل*20-10.35رس", "تانيا 330 مل (1 شرنك - 20 عبوة)" },
                             { "5جالون-جديد-13.8رس", "تانيا 5 جالون - جديد" },
                             { "5جالون-تعبئة-8.5رس", "تانيا 5 جالون - إعادة تعبئة" },
                             { "مكعباتثلج-12.08رس", "مكعبات ثلج(1 كرتون - 8 شريحة)" }
                         };

                if (productMappings.ContainsKey(ProductName))
                {
                    ProductName = productMappings[ProductName];
                }

                if (lang == "ar")
                {
                    prods = LastResponse.products.Where(x => x.product_name_ar == ProductName).FirstOrDefault();

                }
                else
                {
                    prods = LastResponse.products.Where(x => x.product_name_en == ProductName).FirstOrDefault();

                }




                return prods;

            }
            catch (Exception ex)
            {
                return new TaniaGetProductsByLocModel.Product() { product_id = -1 };
            }
        }


        [Route("TaniaValidatePromocode")]
        [HttpGet]
        public async Task<TaniaValidatePromocodeModel> TaniaValidatePromocode(string cart, string addressid, string promocode)
        {
            try
            {
                if (cart.StartsWith(","))
                {
                    cart = cart.Substring(1);

                }
                cart = "[" + cart + "]";
                TaniaValidatePromocodePost taniaValidatePromocodePost = new TaniaValidatePromocodePost();
                var cartList = JsonConvert.DeserializeObject<List<TaniaValidatePromocodePost.Cart>>(cart);


                foreach (var car in cartList)
                {
                    try
                    {
                        double result = double.Parse(car.quantity);
                        car.quantity = Math.Floor(result).ToString();
                    }
                    catch
                    {

                    }


                }


                TaniaValidatePromocodeModel lastResponse = new TaniaValidatePromocodeModel();
                lastResponse.is_valid = false;



                taniaValidatePromocodePost.address_id = int.Parse(addressid);
                taniaValidatePromocodePost.promocode = promocode;
                taniaValidatePromocodePost.delivery_day = DateTime.Now.ToString("yyyy-MM-dd");
                taniaValidatePromocodePost.use_coupon = 0;
                taniaValidatePromocodePost.use_wallet = 0;
                taniaValidatePromocodePost.cart = cartList;



                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/validate-promocode");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5"); // Ensure "Bearer" prefix if required

                var jsonContent = JsonConvert.SerializeObject(taniaValidatePromocodePost);
                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = content;

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                lastResponse = JsonConvert.DeserializeObject<TaniaValidatePromocodeModel>(responseContent);



                return lastResponse;


            }
            catch (Exception ex)
            {
                return new TaniaValidatePromocodeModel() { is_valid = false };
            }
        }

        [Route("TaniaPlaceorder")]
        [HttpGet]
        public async Task<string> TaniaPlaceorder(string cart, string addressid, string promocode)
        {
            try
            {

                if (cart.StartsWith(","))
                {
                    cart = cart.Substring(1);

                }
                cart = "[" + cart + "]";
                TaniaValidatePromocodePost taniaValidatePromocodePost = new TaniaValidatePromocodePost();
                var cartList = JsonConvert.DeserializeObject<List<TaniaValidatePromocodePost.Cart>>(cart);

                foreach (var car in cartList)
                {
                    try
                    {
                        double result = double.Parse(car.quantity);
                        car.quantity = Math.Floor(result).ToString();
                    }
                    catch
                    {

                    }
                }



                taniaValidatePromocodePost.address_id = int.Parse(addressid);
                taniaValidatePromocodePost.promocode = promocode;
                taniaValidatePromocodePost.delivery_day = DateTime.Now.ToString("yyyy-MM-dd");
                taniaValidatePromocodePost.use_coupon = 0;
                taniaValidatePromocodePost.use_wallet = 0;
                taniaValidatePromocodePost.cart = cartList;


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/place-order");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5"); // Ensure "Bearer" prefix if required

                var jsonContent = JsonConvert.SerializeObject(taniaValidatePromocodePost);
                var content = new StringContent(jsonContent);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = content;

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var lastResponse = JsonConvert.DeserializeObject<TaniaPlaceorderModel>(responseContent);





                return lastResponse.order_number;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        [Route("TaniaCartString")]
        [HttpGet]
        public async Task<string> TaniaCartString(string cart, string lang = "ar", string total_before_discount = "0", string discount = "0", string total_after_discount = "0", string promotion_message_en = "", string promotion_message_ar = "")
        {
            try
            {
                List<TaniaCartString> taniaCartStringsList = new List<TaniaCartString>();
                if (cart.StartsWith(","))
                {
                    cart = cart.Substring(1);

                }
                cart = "[" + cart + "]";



                cart = cart.Replace("{\"productName\":", "").Replace("\"quantity\":", "").Replace("\"price\":", "");

                cart = cart.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");


                var listc = cart.Split(",");


                foreach (var item in listc)
                {
                    var listr = item.Split(";");
                    TaniaCartString taniaCartString = new TaniaCartString();
                    taniaCartString.productName = listr[0];
                    taniaCartString.quantity = listr[1];
                    taniaCartString.price = listr[2];

                    taniaCartStringsList.Add(taniaCartString);
                }





                var cartList = taniaCartStringsList;// JsonConvert.DeserializeObject<List<TaniaCartString>>(cart);


                foreach (var car in cartList)
                {
                    try
                    {
                        double result = double.Parse(car.quantity);
                        car.quantity = Math.Floor(result).ToString();
                    }
                    catch
                    {

                    }
                }

                var st = "";


                if (!string.IsNullOrEmpty(promotion_message_en) || !string.IsNullOrEmpty(promotion_message_ar))
                {
                    if (lang == "ar")
                    {


                        st = st + "*" + promotion_message_ar.Trim() + "*" + "\n";

                    }
                    else
                    {

                        st = st + "*" + promotion_message_en.Trim() + "*" + "\n";

                    }


                }


                if (lang == "ar")
                {

                    st = st + "---- ملخص الطلب ----\n";
                }
                else
                {

                    st = st + "---- Order Summary ----\n";
                }



                var total = 0.0;
                foreach (var it in cartList)
                {
                    if (int.Parse(it.quantity) > 0)
                    {

                        if (lang == "ar")
                        {
                            int result = int.Parse(it.quantity);
                            st = st + "*اسم المنتج* :" + it.productName + "\n";
                            st = st + "*العدد* :" + result + "\n";
                            st = st + "*السعر لكل قطعة* :" + it.price + "\n";

                        }
                        else
                        {
                            int result = int.Parse(it.quantity);
                            st = st + "*Product Name* :" + it.productName + "\n";
                            st = st + "*Number* :" + result + "\n";
                            st = st + "*Price per piece* :" + it.price + "\n";
                        }


                        st = st + "-------------------\n";

                    }




                }


                st = st + "-------------------\n";



                var TotalaFvat = (double.Parse(total_after_discount));
                if (double.Parse(discount) != 0)
                {

                    if (lang == "ar")
                    {

                        st = st + "*المجموع قبل الخصم* :" + total_before_discount + "\n";
                        st = st + "*المجموع بعد الخصم* :" + total_after_discount + "\n";
                        st = st + "*الخصم* :" + discount + "\n";
                        st = st + "*السعر الإجمالي بعد ضريبة* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";

                    }
                    else
                    {
                        st = st + "*Total Before Discount* :" + total_before_discount + "\n";
                        st = st + "*Total After Discount* :" + total_after_discount + "\n";
                        st = st + "*Discount* :" + discount + "\n";
                        st = st + "*Total Price After VAT* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";

                    }




                }
                else
                {


                    if (string.IsNullOrEmpty(promotion_message_en) || string.IsNullOrEmpty(promotion_message_ar))
                    {

                        if (lang == "ar")
                        {

                            st = st + "*المجموع* :" + string.Format("{0:0.00}", total_before_discount) + "\n";
                            st = st + "*السعر الإجمالي بعد ضريبة* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";

                        }
                        else
                        {
                            st = st + "*Total* :" + string.Format("{0:0.00}", total_before_discount) + "\n";
                            st = st + "*Total Price After VAT* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";

                        }
                    }
                    else
                    {

                        if (lang == "ar")
                        {

                            st = st + "*المجموع* :" + string.Format("{0:0.00}", total_before_discount) + "\n";
                            st = st + "*السعر الإجمالي بعد ضريبة* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";
                            //st=st+"*"+promotion_message_ar+"*"+"\n";

                        }
                        else
                        {
                            st = st + "*Total* :" + string.Format("{0:0.00}", total_before_discount) + "\n";
                            st = st + "*Total Price After VAT* :" + string.Format("{0:0.00}", TotalaFvat) + "\n";
                            // st=st+"*"+promotion_message_en+"*"+"\n";

                        }

                    }

                }



                return st;

            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        [Route("TaniaCreateComplaint")]
        [HttpGet]
        public async Task<TaniaCreateComplaintModel> TaniaCreateComplaint(string phonenumber, string addressid, string complaint)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/create-ticket");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{\r\n\"mobile\":" + phonenumber + ",\r\n\"order_id\":" + addressid + ",\r\n\"description\":\"" + complaint + "\"\r\n}\r\n\r\n");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                var LastResponse = JsonConvert.DeserializeObject<TaniaCreateComplaintModel>(responseContent);

                if (LastResponse.code == 200)
                {

                    return LastResponse;
                }
                else
                {

                    return new TaniaCreateComplaintModel() { code = 404 };
                }


            }
            catch (Exception ex)
            {
                return new TaniaCreateComplaintModel() { code = 404 };
            }
        }



        [Route("TaniaGetorderStatus")]
        [HttpGet]
        public async Task<TaniaGetorderStatusModel> TaniaGetorderStatus(string phonenumber, string ordarenumber)
        {
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/get-order-status");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{\r\n\"mobile\":" + phonenumber + ",\r\n\"order_number\":\"" + ordarenumber + "\"\r\n}", null, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());




                var responseContent = await response.Content.ReadAsStringAsync();
                var LastResponse = JsonConvert.DeserializeObject<TaniaGetorderStatusModel>(responseContent);


                switch (LastResponse.order_status_ar)
                {
                    case "جاهز للإستلام":
                        LastResponse.order_status_ar = "تم انشاء طلبك وجاري العمل عليه";
                        break;
                    case "تعيين":
                        LastResponse.order_status_ar = "تم التحقق من الطلب وجاري العمل عليه";
                        break;
                    case "في الطريق اليك":
                        LastResponse.order_status_ar = "طلبك في الطريق للتسليم";
                        break;

                }


                return LastResponse;

            }
            catch (Exception ex)
            {
                return new TaniaGetorderStatusModel() { code = 500 };
            }
        }

        [Route("TaniaTicketStatus")]
        [HttpGet]
        public async Task<TaniaTicketStatusModel> TaniaTicketStatus(string phonenumber, string ordarenumber)
        {
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Bypass certificate validation (for testing only)
                };

                using var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oms.taniawater.sa/oms/api/WhatsApp/ticket-status");
                request.Headers.Add("Authorization", "060fac9a80afec9b95eb292ad884c5f5");
                var content = new StringContent("{\r\n\"mobile\":" + phonenumber + ",\r\n\"order_id\":\"" + ordarenumber + "\"\r\n}", null, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());




                var responseContent = await response.Content.ReadAsStringAsync();
                var LastResponse = JsonConvert.DeserializeObject<TaniaTicketStatusModel>(responseContent);




                return LastResponse;

            }
            catch (Exception ex)
            {
                return new TaniaTicketStatusModel();
            }
        }


        #endregion


        #region privet
        private AreaDto getNearbyArea(int tenantID, double eLatitude, double eLongitude)
        {


            AreaDto areaDto = new AreaDto();
            var areas = _iAreasAppService.GetAllAreas(tenantID, true);
            List<AreaDto> lstAreaDto = new List<AreaDto>();

            if (areas != null)
            {
                foreach (var item in areas)
                {
                    if (checkIsInService(item.SettingJson))
                    {
                        lstAreaDto.Add(item);

                    }
                }
            }


            Dictionary<long, double> branch = new Dictionary<long, double>();

            //int
            if (lstAreaDto != null)
            {
                foreach (var item in lstAreaDto)
                {

                    var sCoord = new GeoCoordinate(item.Latitude.Value, item.Longitude.Value);
                    var eCoord = new GeoCoordinate(eLatitude, eLongitude);
                    var currentDistance = sCoord.GetDistanceTo(eCoord);



                    branch.Add(item.Id, currentDistance);


                }
            }
            var nearestAreaId = branch.OrderBy(x => x.Value).FirstOrDefault().Key;


            var area = lstAreaDto.Where(x => x.Id == nearestAreaId).FirstOrDefault();

            return area;
        }

        private bool checkIsInService(string workingHourSetting)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(workingHourSetting))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(workingHourSetting, options);

                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
                DayOfWeek wk = currentDateTime.DayOfWeek;
                TimeSpan timeOfDay = currentDateTime.TimeOfDay;

                switch (wk)
                {
                    case DayOfWeek.Saturday:
                        if (workModel.IsWorkActiveSat)
                        {
                            var StartDateSat = getValidValue(workModel.StartDateSat);
                            var EndDateSat = getValidValue(workModel.EndDateSat);

                            var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
                            var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

                            if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Sunday:
                        if (workModel.IsWorkActiveSun)
                        {
                            var StartDate = getValidValue(workModel.StartDateSun);
                            var EndDate = getValidValue(workModel.EndDateSun);

                            var StartDateSP = getValidValue(workModel.StartDateSunSP);
                            var EndDateSP = getValidValue(workModel.EndDateSunSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:

                        if (workModel.IsWorkActiveMon)
                        {
                            var StartDate = getValidValue(workModel.StartDateMon);
                            var EndDate = getValidValue(workModel.EndDateMon);

                            var StartDateSP = getValidValue(workModel.StartDateMonSP);
                            var EndDateSP = getValidValue(workModel.EndDateMonSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Tuesday:
                        if (workModel.IsWorkActiveTues)
                        {
                            var StartDate = getValidValue(workModel.StartDateTues);
                            var EndDate = getValidValue(workModel.EndDateTues);

                            var StartDateSP = getValidValue(workModel.StartDateTuesSP);
                            var EndDateSP = getValidValue(workModel.EndDateTuesSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (workModel.IsWorkActiveWed)
                        {
                            var StartDate = getValidValue(workModel.StartDateWed);
                            var EndDate = getValidValue(workModel.EndDateWed);

                            var StartDateSP = getValidValue(workModel.StartDateWedSP);
                            var EndDateSP = getValidValue(workModel.EndDateWedSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (workModel.IsWorkActiveThurs)
                        {
                            var StartDate = getValidValue(workModel.StartDateThurs);
                            var EndDate = getValidValue(workModel.EndDateThurs);

                            var StartDateSP = getValidValue(workModel.StartDateThursSP);
                            var EndDateSP = getValidValue(workModel.EndDateThursSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (workModel.IsWorkActiveFri)
                        {
                            var StartDate = getValidValue(workModel.StartDateFri);
                            var EndDate = getValidValue(workModel.EndDateFri);

                            var StartDateSP = getValidValue(workModel.StartDateFriSP);
                            var EndDateSP = getValidValue(workModel.EndDateFriSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    default:

                        break;

                }


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
        #endregion


        #region RZ
        [Route("RZPhoneVerificationRequest")]
        [HttpGet]
        public async Task<ActionResult<string>> VerifyPhoneNumber(string lang, string phonenumber, string userName = "RZ", string password = "Rz@2025")
        {
            try
            {

                if (phonenumber.StartsWith("962"))
                {
                    phonenumber = "0" + phonenumber.Substring(3);
                }
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://rz.natejerp.com/erp/natejerp/chatBot/getPhoneNumberVerification");

                var requestBody = $@"
                    {{
                        ""lang"": ""{lang}"",
                        ""mobile"": ""{phonenumber}"",
                        ""userName"": ""RZ"",
                        ""password"": ""Rz@2025""
                    }}";

                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("API request failed.");
                }
                var existingCustomer = JsonConvert.DeserializeObject<ExistingCustomerResponse>(responseString);
                if (existingCustomer != null && !string.IsNullOrEmpty(existingCustomer.clientName))
                {
                    return Ok(existingCustomer.clientName);
                }
                else
                {
                    return "-1";
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private List<Item> GetItemByPath(string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemByPath;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path)

                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<Item> GetItemByPath2(string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemByPath2;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path)

                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static Item ConvertItemsToDto(IDataReader dataReader)
        {
            Item items = new Item();
            items.itemCode = SqlDataHelper.GetValue<long>(dataReader, "itemCode");
            items.itemDescription = SqlDataHelper.GetValue<string>(dataReader, "itemDescription");
            items.path = SqlDataHelper.GetValue<int>(dataReader, "path");
            return items;
        }
        private List<Item> GetItemCode(string itemDescription, string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemCode;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path),
                new System.Data.SqlClient.SqlParameter("@itemDescription",itemDescription)
                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<Item> GetItemCode2(string itemDescription, string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemCode2;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path),
                new System.Data.SqlClient.SqlParameter("@itemDescription",itemDescription)
                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("getItemByPath")]
        public async Task<ActionResult<string>> GetItems(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemByPath(result);

            if (items == null || items.Count == 0)
            {
                return NotFound("");
            }

            string result2 = string.Join(",", items.Select(i => i.itemDescription));

            return Ok(result2);

        }
        [HttpGet("getItemByPath2")]
        public async Task<ActionResult<string>> GetItems2(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemByPath2(result);

            if (items == null || items.Count == 0)
            {
                return NotFound("");
            }

            string result2 = string.Join(",", items.Select(i => i.itemDescription));

            return Ok(result2);

        }
        [HttpGet("getItemCode")]
        public async Task<ActionResult<string>> GetItemCodeAPI(string itemDescription, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemCode(itemDescription, result).FirstOrDefault();

            return Ok(items.itemCode);

        }

        [HttpGet("getItemCode2")]
        public async Task<ActionResult<string>> GetItemCodeAPI2(string itemDescription, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemCode2(itemDescription, result).FirstOrDefault();

            return Ok(items.itemCode);

        }

        [HttpGet("RZsubmit-order")]
        public async Task<ActionResult<OrderResponseRZ>> SubmitOrder(
            string lang,
            string invNote,
            string cart)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cart))
                {
                    return BadRequest(new { success = false, message = "Cart cannot be empty." });
                }

                // Remove leading comma if exists
                if (cart.StartsWith(","))
                    cart = cart.Substring(1);

                // Quote itemCode if it's missing quotes
                cart = Regex.Replace(
                    cart,
                    @"""itemCode""\s*:\s*([A-Za-z0-9]+)(?=\s*,|\s*})",
                    @"""itemCode"":""$1"""
                );

                // Quote itemDesc safely (handles Arabic and special characters)
                cart = Regex.Replace(
                    cart,
                    @"""itemDesc""\s*:\s*(?![""])([^"",}\r\n]+)",
                    m => $@"""itemDesc"":""{m.Groups[1].Value.Trim()}"""
                );

                // Ensure it's wrapped as an array
                if (!cart.TrimStart().StartsWith("["))
                    cart = "[" + cart;
                if (!cart.TrimEnd().EndsWith("]"))
                    cart = cart + "]";

                List<OrderDetailRZ> cartList;
                try
                {
                    cartList = JsonConvert.DeserializeObject<List<OrderDetailRZ>>(cart);

                    if (cartList == null || cartList.Count == 0)
                    {
                        return BadRequest(new { success = false, message = "Cart is invalid or empty." });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = "Invalid cart format.", error = ex.Message });
                }

                // Floor the qty values and assign default unitCode if needed
                foreach (var item in cartList)
                {
                    item.qty = Math.Floor(item.qty);
                    if (item.unitCode == 0)
                        item.unitCode = 3;
                }

                var payloadCart = cartList.Select(item => new OrderDetailPayload
                {
                    itemCode = item.itemCode,
                    unitCode = item.unitCode,
                    qty = item.qty,
                    price = item.price
                }).ToList();

                using var client = new HttpClient();
                var apiUrl = "https://rz.natejerp.com/erp/natejerp/chatBot/orderSubmission";

                var payload = new
                {
                    userName = "RZ",
                    password = "Rz@2025",
                    lang = lang,
                    DATA = new[]
                    {
                new
                {
                    accNo = "0",
                    invNote = invNote,
                    DETAILS = payloadCart
                }
            }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        success = false,
                        message = "Failed to submit order.",
                        response = jsonResponse
                    });
                }

                var result = JsonConvert.DeserializeObject<OrderResponseRZ>(jsonResponse);

                // Build invoice-style summary
                var sb = new StringBuilder();
                sb.AppendLine("Cart Summary:");
                sb.AppendLine("-------------------------------------------------");
                sb.AppendLine("Item Code   | Quantity | Price  | Description");
                sb.AppendLine("--------------------------------------------------");

                foreach (var item in cartList)
                {
                    sb.AppendLine($"{item.itemCode.PadRight(11)}| {item.qty.ToString().PadRight(8)}| {item.price.ToString("0.00").PadRight(6)}| {item.itemDesc}");
                }

                sb.AppendLine("----------------------------------------------------");

                return Ok(new
                {
                    success = true,
                    message = "Order submitted successfully.",
                    orderResponse = result,
                    cartDetails = cartList.Select(x => new
                    {
                        x.itemCode,
                        x.qty,
                        x.price,
                        x.itemDesc
                    }),
                    cartSummary = sb.ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error.",
                    error = ex.Message
                });
            }
        }

        private List<Item> GetItemByPathEN(string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemByPath_EN;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path)

                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<Item> GetItemByPath2EN(string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetItemByPath_EN2;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path)

                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getItemByPathEN")]
        public async Task<ActionResult<string>> GetItemsEN(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemByPathEN(result);

            if (items == null || items.Count == 0)
            {
                return NotFound("");
            }

            string result2 = string.Join(",", items.Select(i => i.itemDescription));

            return Ok(result2);

        }
        [HttpGet("getItemByPath2EN")]
        public async Task<ActionResult<string>> GetItems2EN(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemByPath2EN(result);

            if (items == null || items.Count == 0)
            {
                return NotFound("");
            }

            string result2 = string.Join(",", items.Select(i => i.itemDescription));

            return Ok(result2);

        }

        private List<Item> GetItemCodeEN(string itemDescription, string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetRZItemCode_EN;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path),
                new System.Data.SqlClient.SqlParameter("@itemDescription",itemDescription)
                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<Item> GetItemCode2EN(string itemDescription, string path)
        {
            try
            {
                List<Item> items = new List<Item>();

                var SP_Name = Constants.RZIntegration.SP_GetRZItemCode_EN2;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@path",path),
                new System.Data.SqlClient.SqlParameter("@itemDescription",itemDescription)
                };
                items = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertItemsToDto, AppSettingsModel.ConnectionStrings).ToList();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("getItemCodeEN")]
        public async Task<ActionResult<string>> GetItemCodeAPIEN(string itemDescription, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemCodeEN(itemDescription, result).FirstOrDefault();

            return Ok(items.itemCode);

        }

        [HttpGet("getItemCode2EN")]
        public async Task<ActionResult<string>> GetItemCodeAPI2EN(string itemDescription, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Path cannot be empty");
            }

            string pattern = ",\"test\":";
            int firstOccurrence = path.IndexOf(pattern);

            if (firstOccurrence < 0)
            {
                return Ok(path);
            }

            string before = path.Substring(0, firstOccurrence);
            string after = path.Substring(firstOccurrence + pattern.Length);

            string processed = after.Replace(pattern, "-");

            string result = before + processed;

            var items = GetItemCode2EN(itemDescription, result).FirstOrDefault();

            return Ok(items.itemCode);

        }

        [HttpGet("RZProductDisplay")]
        public async Task<ActionResult<ProductDisplayResponse>> AddNewItemDetails(
         string lang,
         string itemCode
        )
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = "https://rz.natejerp.com/erp/natejerp/chatBot/ProductDisplay";

                    var requestBody = new
                    {
                        lang = lang,
                        accNo = 0,
                        pageNo = 0,
                        pageSize = 10000,
                        searchText = "",
                        userName = "RZ",
                        password = "Rz@2025",
                    };

                    string jsonRequest = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(apiUrl, content);
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, new { success = false, message = "API request failed.", response = jsonResponse });
                    }

                    var itemResponse = JsonConvert.DeserializeObject<List<ProductDisplayResponse>>(jsonResponse) ?? new List<ProductDisplayResponse>();

                    var matchedItem = itemResponse.FirstOrDefault(item => item.itemCode == itemCode);
                    if (matchedItem == null)
                    {
                        if (matchedItem == null)
                        {
                            return Ok(-1);
                        }
                    }
                    return Ok(matchedItem);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error.", error = ex.Message });
            }
        }




        #endregion

        #region alsarh
        private const string BaseUrl = "https://admin.alsarh.sa";

        [HttpGet("SearchAirports")]
        public async Task<ActionResult<string>> SearchAirports(string q = "")
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/airports/auto_complete");

                object requestObject;
                if (string.IsNullOrEmpty(q))
                {
                    requestObject = new { sort = new[] { "order:desc" } };
                }
                else
                {
                    requestObject = new { q = q, sort = new[] { "order:desc" } };
                }

                string jsonContent = JsonConvert.SerializeObject(requestObject);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Extract and return only the first 9 airport names (Arabic) in a comma-separated string.
                var formattedString = string.Join(",",
                    airportResponse?.Hits?
                        .Take(9)
                        .Where(hit => hit?.Name != null && hit.Name.ContainsKey("ar"))
                        .Select(hit => hit.Name["ar"])
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString=="")
                {
                    formattedString = "-1";
                }
                return Ok(formattedString);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling API: {ex.Message}");
            }
        }

        [HttpGet("SearchAirportsEN")]
        public async Task<ActionResult<string>> SearchAirportsEN(string q = "")
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/airports/auto_complete");

                object requestObject;
                if (string.IsNullOrEmpty(q))
                {
                    requestObject = new { sort = new[] { "order:desc" } };
                }
                else
                {
                    requestObject = new { q = q, sort = new[] { "order:desc" } };
                }

                string jsonContent = JsonConvert.SerializeObject(requestObject);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Extract and return only the first 9 airport names (Arabic) in a comma-separated string.
                var formattedString = string.Join(",",
                    airportResponse?.Hits?
                        .Take(9)
                        .Where(hit => hit?.Name != null && hit.Name.ContainsKey("en"))
                        .Select(hit => hit.Name["en"])
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString == "")
                {
                    formattedString = "-1";
                }
                return Ok(formattedString);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling API: {ex.Message}");
            }
        }

        [HttpGet("searchCountries")]
        public async Task<string> SearchCountries(string q = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/countries/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var formattedString = string.Join(",",
                    apiResponse?.Hits?
                        .Where(hit => hit?.Name != null && hit.Name.ContainsKey("ar"))
                        .Take(9)
                        .Select(hit => hit.Name["ar"])
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString == "")
                {
                    formattedString = "-1";
                }

                return formattedString;
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("searchCountriesEN")]
        public async Task<string> SearchCountriesEN(string q = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/countries/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var formattedString = string.Join(",",
                    apiResponse?.Hits?
                        .Where(hit => hit?.Name != null && hit.Name.ContainsKey("en"))
                        .Take(9)
                        .Select(hit => hit.Name["en"])
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString == "")
                {
                    formattedString = "-1";
                }
                return formattedString;
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }


        [HttpGet("hotel_cities1EN")]
        public async Task<string> hotel_citiesEN(string q = "")
        {
            try
            {


                DateTime now = DateTime.Now;

                DateTime currentWeekEnd = now;
                DateTime currentWeekStart = currentWeekEnd.AddDays(-7);

                DateTime lastMonthEnd = now;
                DateTime lastMonthStart = lastMonthEnd.AddMonths(-1);
                if (q == null || string.IsNullOrWhiteSpace(q))
                {
                    return ("Search query 'q' is required.  ..");
                }

                using var client = new HttpClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/hotel_cities/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var formattedString = string.Join(",",
                    apiResponse?.Hits?
                        .Where(hit => hit?.Name != null &&
                                     hit.Name.ContainsKey("ar") &&
                                     hit?.Country?.Name != null &&
                                     hit.Country.Name.ContainsKey("en"))
                        .Select(hit => $"{hit.Name["en"]}-({hit.Country.Name["en"]})")
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString == "")
                {
                    formattedString = "-1";
                }
                return formattedString;
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("hotel_cities1")]
        public async Task<string> hotel_cities(string q = "")
        {
            try
            {
                if (q == null || string.IsNullOrWhiteSpace(q))
                {
                    return ("Search query 'q' is required.  ..");
                }

                using var client = new HttpClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/hotel_cities/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var formattedString = string.Join(",",
                    apiResponse?.Hits?
                        .Where(hit => hit?.Name != null &&
                                     hit.Name.ContainsKey("en") &&
                                     hit?.Country?.Name != null &&
                                     hit.Country.Name.ContainsKey("ar"))
                        .Select(hit => $"{hit.Name["ar"]}-({hit.Country.Name["ar"]})")
                        .ToArray() ?? Array.Empty<string>());

                if (formattedString == "")
                {
                    formattedString = "-1";
                }
                return formattedString;
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("CheckStartReservationDate")]
        public string ValidateDate(string startReservation)
        {
            string[] dateFormats = new string[] { "yyyy-MM-dd", "yyyy-M-dd", "yyyy-M-d", "yyyy-MM-d" };

            DateTime inputDate;

            if (!DateTime.TryParseExact(startReservation, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out inputDate))
            {
                return "Invalid date format. Please use 'yyyy-MM-dd' or 'yyyy-M-dd' (e.g., 2025-04-26 or 2025-4-26).";
            }

            DateTime today = DateTime.Today;

            if (inputDate < today)
            {
                return "The date must be today or in the future.";
            }

            return "Valid date!";
        }

        [HttpGet("CheckEndReservationDate")]
        public string ValidateReservation(string reservationDate, string endReservationDate)
        {
            string[] dateFormats = new string[] { "yyyy-MM-dd", "yyyy-M-dd", "yyyy-M-d", "yyyy-MM-d" };
            DateTime startDate, endDate;

            if (!DateTime.TryParseExact(reservationDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(endReservationDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                return ("Invalid date format. Please use 'yyyy-MM-dd' (e.g., 2025-04-26).");
            }
            if (endDate <= startDate)
            {
                return ("End reservation date must be after the reservation date . please Enter end reservation once again.");
            }
            return ("True");
        }

        [HttpGet("GetAirportsCode")]
        public async Task<string> GetAirportsCode(string q = "")
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/airports/auto_complete");

                object requestObject;
                if (string.IsNullOrEmpty(q))
                {
                    requestObject = new { sort = new[] { "order:desc" } };
                }
                else
                {
                    requestObject = new { q = q, sort = new[] { "order:desc" } };
                }

                string jsonContent = JsonConvert.SerializeObject(requestObject);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Search for an airport where the Arabic name equals the provided query.
                var matchingAirport = airportResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("ar") &&
                                            hit.Name["ar"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (matchingAirport != null)
                {
                    return matchingAirport.IATA_CODE;
                }
                else
                {
                    return "No airport found matching the provided name.";
                }
            }
            catch (HttpRequestException ex)
            {
                return $" Error calling API: ";
            }
        }

        [HttpGet("GetAirportsCodeEN")]
        public async Task<string> GetAirportsCodeEN(string q = "")
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/airports/auto_complete");

                object requestObject;
                if (string.IsNullOrEmpty(q))
                {
                    requestObject = new { sort = new[] { "order:desc" } };
                }
                else
                {
                    requestObject = new { q = q, sort = new[] { "order:desc" } };
                }

                string jsonContent = JsonConvert.SerializeObject(requestObject);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var airportResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Search for an airport where the Arabic name equals the provided query.
                var matchingAirport = airportResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("en") &&
                                            hit.Name["en"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (matchingAirport != null)
                {
                    return matchingAirport.IATA_CODE;
                }
                else
                {
                    return "No airport found matching the provided name.";
                }
            }
            catch (HttpRequestException ex)
            {
                return $" Error calling API: ";
            }
        }


        [HttpGet("GetHotelCityCode")]
        public async Task<string> GetHotelCityCode(string q = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    q = q.Split('-')[0].Trim();
                }
                else
                {
                    return "Search query 'q' is required.";
                }
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/hotel_cities/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var match = apiResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("ar") &&
                                            hit.Name["ar"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return $"{match.Code}-{match.Country.Code}";

                }
                else
                {
                    return ("No matching record found.");
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("GetHotelCityCodeEN")]
        public async Task<string> GetHotelCityCodeEN(string q = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    q = q.Split('-')[0].Trim();
                }
                else
                {
                    return "Search query 'q' is required.";
                }
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/hotel_cities/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                var match = apiResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("en") &&
                                            hit.Name["en"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return $"{match.Code}-{match.Country.Code}";

                }
                else
                {
                    return ("No matching record found.");
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }
        [HttpGet("GetCountriesCode")]
        public async Task<string> GetCountriesCode(string q = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/countries/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Find the first record where the Arabic name exactly matches the provided query
                var match = apiResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("ar") &&
                                            hit.Name["ar"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return match.Code;
                }
                else
                {
                    return $"No country found with name '{q}'.";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("GetCountriesCodeEN")]
        public async Task<string> GetCountriesCodeEN(string q = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return "Search query 'q' is required.";
                }

                using var client = new HttpClient();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v2/countries/auto_complete");

                var requestBody = new { q = q };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<AirportResponse>(responseContent);

                // Find the first record where the Arabic name exactly matches the provided query
                var match = apiResponse?.Hits?
                    .FirstOrDefault(hit => hit?.Name != null &&
                                            hit.Name.ContainsKey("en") &&
                                            hit.Name["en"].Equals(q, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return match.Code;
                }
                else
                {
                    return $"No country found with name '{q}'.";
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }

        [HttpGet("FormatDate")]
        public ActionResult<string> FormatDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return BadRequest("Date parameter is required.");
            }

            // Define all acceptable formats
            string[] acceptedFormats = new[]
            {
                "yyyy-MM-dd",
                "yyyy-M-dd",
                "yyyy-MM-d",
                "yyyy-M-d"
            };

            if (DateTime.TryParseExact(date, acceptedFormats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime parsedDate))
            {
                string formattedDate = parsedDate.ToString("ddMMMyyyy", CultureInfo.InvariantCulture);
                return Ok(formattedDate);
            }
            else
            {
                return BadRequest("Invalid date format. Expected formats: yyyy-MM-dd, yyyy-M-dd, yyyy-MM-d, yyyy-M-d");
            }
        }


        [HttpGet("FormatDateRange")]
        public ActionResult<string> FormatDateRange(string startDate, string endDate)
        {
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
            {
                return BadRequest("Both startDate and endDate parameters are required.");
            }
            string[] acceptedFormats = new[]
            {
                  "yyyy-MM-dd",
                  "yyyy-M-dd",
                  "yyyy-MM-d",
                  "yyyy-M-d"
            };

            if (DateTime.TryParseExact(startDate, acceptedFormats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime start) &&
                DateTime.TryParseExact(endDate, acceptedFormats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime end))
            {
                var arabicCulture = new CultureInfo("ar-SA");
                arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();

                string formattedStart = start.ToString("dd MMMM yyyy", arabicCulture).Replace(" ", "");
                string formattedEnd = end.ToString("dd MMMM yyyy", arabicCulture).Replace(" ", "");

                return Ok($"{formattedStart}-{formattedEnd}");
            }
            else
            {
                return BadRequest("Invalid date format. Expected formats: yyyy-MM-dd, yyyy-M-dd, yyyy-MM-d, yyyy-M-d for both startDate and endDate.");
            }
        }

        [HttpGet("CancelOrder")]
        public void ChatStepCancelOrder(string SelectedAreaId,
        string ContactID,
        string LangString,
        string displayName,
        int tenantID,
        string PhoneNumbe,
        string OrderType,
        int langId,
        int orderId
    )
        {

            string userId = tenantID.ToString() + "_" + PhoneNumbe.ToString();

            _botApis.DeleteOrderDraft(tenantID, int.Parse(orderId.ToString()));
        }
        [HttpGet("GenerateLink")]
        public ActionResult<string> GenerateLink(
            string hotelCode,
            string countriesCode,
            string date,
            int numOfAdult = 1,
            int numOfChildren = 0)
        {
            if (string.IsNullOrWhiteSpace(hotelCode) ||
                string.IsNullOrWhiteSpace(countriesCode) ||
                string.IsNullOrWhiteSpace(date))
            {
                return BadRequest("All parameters (hotelCode, countriesCode, date) are required.");
            }

            string link = $"https://alsarh.sa/hotels/result/{hotelCode}/{date}/ADT-{numOfAdult}-CHI-?nationality={countriesCode}&stars=0&freeCancellation=false";
            return Ok(link);
        }



        [HttpGet("GenerateFlightLink")]
        public ActionResult<string> GenerateFlightLink(
        string airportCode1,
        string airportCode2,
        string date1,
        string date2,
        int numOfAdult,
        int numOfChild,
        int numOfInfants,
        string CabinClass)
        {
            if (string.IsNullOrWhiteSpace(airportCode1) || string.IsNullOrWhiteSpace(airportCode2)
                || string.IsNullOrWhiteSpace(date1) || string.IsNullOrWhiteSpace(date2))
            {
                return BadRequest("All parameters are required.");
            }

            string link = $"https://alsarh.sa/flights/result/{airportCode1}-{airportCode2}-{date1}|{airportCode2}-{airportCode1}-{date2}/R/{CabinClass}/ADT-{numOfAdult}|CHILD-{numOfChild}|INF-{numOfInfants}";
            return Ok(link);
        }

        [HttpGet("GenerateFlightOneWayLink")]
        public ActionResult<string> GenerateFlightLink(string AirportsCode1, string AirportsCode2, string date1, int numOfAdult = 1, int numOfChild = 0, int numOfInfants = 0, string CabinClass = "E")
        {
            if (string.IsNullOrWhiteSpace(AirportsCode1) ||
                string.IsNullOrWhiteSpace(AirportsCode2) ||
                string.IsNullOrWhiteSpace(date1) ||
                numOfAdult <= 0 ||
                numOfChild < 0 ||
                numOfInfants < 0 ||
                string.IsNullOrWhiteSpace(CabinClass))
            {
                return BadRequest("Invalid input. Please check your parameters.");
            }

            string formattedLink = $"https://alsarh.sa/flights/result/{AirportsCode1}-{AirportsCode2}-{date1}/O/{CabinClass}/ADT-{numOfAdult}|CHILD-{numOfChild}|INF-{numOfInfants}";

            return Ok(formattedLink);
        }

        #endregion

        #region NationalTaxi
        [HttpPost("NationalTaxi")]
        public async Task<IActionResult> SendInfoCustomer(
                 string phone,
                 string message,
                 string name,
                 string lang = "ar")
        {
            if (message != null && name != null)
            {
                string url = "https://taxialwatani.com/api/whatsappVerification";
                string token = "f3b89cd618b64770ad24353db73de3ccf24ef9a3d186f8c7fdf3793a97c821e6";

                using (var client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Add("lang", lang);
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                        var formData = new MultipartFormDataContent
                {
                    { new StringContent(phone), "phone" },
                    { new StringContent(name), "name" },
                    { new StringContent(message), "message" }
                };

                        HttpResponseMessage response = await client.PostAsync(url, formData);
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(responseBody);

                        return Content(responseBody, "application/json");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error: {ex.Message}");
                    }
                }
            }

            return StatusCode(500, "Error: Missing required fields.");
        }

        #endregion


        #region res
        [HttpGet("GenrateLink")]
        public string ChatStepLink(
                    string SelectedAreaId,
                    string ContactID,
                    string LangString,
                    string displayName,
                    int tenantID,
                    string PhoneNumbe,
                    int langId
                )
        {

            string text = "";
            if (LangString == "ar")
            {
                text = "الرجاء الضغط على رابط المنيو للاطلاع على الأصناف\n\n👇👇👇👇👇👇\n *https://menu.info-seed.com/index1?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}*\n👆👆👆👆👆👆";
            }
            else
            {
                text = "Kindly click on the menu link below to browse the available items:\n\n👇👇👇👇👇👇\n *https://menu.info-seed.com/index1?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}*\n👆👆👆👆👆👆";

            }

            text = text.Replace("{3}", SelectedAreaId.ToString()).Replace("{6}", "0");



            text = text.Replace("{0}", tenantID.ToString()).Replace("{1}", ContactID).Replace("{2}", "").Replace("{4}", langId.ToString()).Replace("{5}", LangString);
            MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel
            {
                ContactID = int.Parse(ContactID),

                Value = text
            };
            var link = _botApis.AddMenuContcatKey(menuContcatKeyModel);
            text = link;

            return link;
        }



        [HttpGet("GetlocationUserModel")]
        public Models.BotModel.GetLocationInfoModel GetlocationUserModel(int tenantID, string query, string lang)
        {
            if (query.StartsWith("https://maps.google.com/"))
            {
                var uri = new Uri(query);
                var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

                if (queryParams.TryGetValue("q", out var coordinates))
                {
                    query = coordinates.ToString();
                }
            }


            var input = new Models.BotModel.SendLocationUserModel
            {
                query = query,
                tenantID = tenantID,
                local = lang
            };
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                //var Customer = GetCustomer(tenantID + "_" + PhoneNumber);  //Get  Customer
                Models.BotModel.GetLocationInfoModel infoLocation = new Models.BotModel.GetLocationInfoModel();

                //Customer.customerChat.text = query;
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {


                    Web.Models.Sunshine.TenantModel tenant = GetTenantByIdRes(input.tenantID).Result;

                    input.isOrderOffer = tenant.isOrderOffer;
                    if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
                    {
                        Models.BotModel.GetLocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);


                        return getLocationInfoModel;
                    }
                    else
                    {
                        //GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                        string result = "-1";
                        string Country = "";
                        string City = "";
                        string Area = "";
                        string Distric = "";
                        string Route = "";

                        if (input.isChangeLocation)
                        {
                            Country = input.address.Split(" - ")[4].Replace("'", "").Trim();
                            City = input.address.Split(" - ")[3].Replace("'", "").Trim();
                            Area = input.address.Split(" - ")[2].Replace("'", "").Trim();
                            Distric = input.address.Split(" - ")[1].Replace("'", "").Trim();
                            Route = input.address.Split(" - ")[0].Replace("'", "").Trim();
                        }
                        else
                        {
                            var rez = GetLocation(input.query);
                            Country = rez.Country.Replace("'", "").Trim();
                            City = rez.City.Replace("'", "").Trim();
                            Area = rez.Area.Replace("'", "").Trim();
                            Distric = rez.Distric.Replace("'", "").Trim();
                            Route = rez.Route.Replace("'", "").Trim();
                        }


                        if (Distric == "Irbid Qasabah District" && Area == "")
                        {
                            Distric = Route;
                        }



                        try
                        {


                            decimal Longitude = decimal.Parse(input.query.Split(",")[0]);
                            decimal Latitude = decimal.Parse(input.query.Split(",")[1]);


                            using (var connection = new SqlConnection(connString))
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "dbo.GetDeliveryCost";

                                command.Parameters.AddWithValue("@Longitude", Longitude);
                                command.Parameters.AddWithValue("@Latitude", Latitude);
                                command.Parameters.AddWithValue("@City", City);
                                command.Parameters.AddWithValue("@Area", Area);
                                command.Parameters.AddWithValue("@Distric", Distric);
                                command.Parameters.AddWithValue("@TenantId", input.tenantID);

                                System.Data.SqlClient.SqlParameter returnValue = command.Parameters.Add("@DeliveryCost", SqlDbType.NVarChar);
                                returnValue.Direction = ParameterDirection.ReturnValue;

                                connection.Open();
                                command.ExecuteNonQuery();

                                result = returnValue.Value.ToString();


                            }

                            var spilt = result.Split(",");



                            if (spilt[0] == "-1" || spilt[0] == "")
                            {

                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;

                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;


                            }
                            else
                            {

                                try
                                {

                                    var locationList = GetAllLocationInfoModel();
                                    var cost = decimal.Parse(spilt[0]);
                                    var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

                                    var cityModel = locationList.Where(x => x.LocationNameEn == City).FirstOrDefault();
                                    var areaModel = locationList.Where(x => x.LocationNameEn == Area).FirstOrDefault();
                                    var districModel = locationList.Where(x => x.LocationNameEn == Distric).FirstOrDefault();

                                    var areaname = GetAreas(input.tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

                                    var cityName = "";
                                    var areaName = "";
                                    var districName = "";

                                    if (cityModel != null)
                                        cityName = cityModel.LocationName;
                                    if (areaModel != null)
                                        areaName = areaModel.LocationName;
                                    if (districModel != null)
                                        districName = districModel.LocationName;



                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = add;


                                    infoLocation.LocationId = int.Parse(spilt[1]);
                                    infoLocation.LocationAreaName = areaname.AreaName;




                                    if (!areaname.IsAvailableBranch)
                                    {


                                        infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                        infoLocation.DeliveryCostAfter = -1;
                                        infoLocation.DeliveryCostBefor = -1;
                                        infoLocation.LocationId = 0;

                                        return infoLocation;


                                    }

                                    OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);


                                    if (areaname != null)
                                    {
                                        if (input.local == "ar")
                                        {
                                            infoLocation.LocationAreaName = areaname.AreaName;
                                        }
                                        else
                                        {

                                            if (areaname.AreaNameEnglish == null)
                                            {

                                                infoLocation.LocationAreaName = areaname.AreaName;
                                            }
                                            else
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaNameEnglish;
                                            }

                                        }

                                    }
                                    infoLocation.DeliveryCostAfter = cost;
                                    if (infoLocation.DeliveryCostBefor == null)
                                    {
                                        infoLocation.DeliveryCostBefor = 0;
                                    }


                                    if (infoLocation.DeliveryCostBefor != -1)
                                    {
                                        //Customer.CustomerStepModel.LocationId = infoLocation.LocationId;
                                        //Customer.CustomerStepModel.IsLinkMneuStep = true;

                                        //Customer.CustomerStepModel.DeliveryCostAfter = infoLocation.DeliveryCostAfter;
                                        //Customer.CustomerStepModel.DeliveryCostBefor = infoLocation.DeliveryCostAfter;

                                        //Customer.CustomerStepModel.AddressLatLong = query;
                                        //Customer.CustomerStepModel.Address = infoLocation.Address;

                                        //Bot = ChatStepLink(model);
                                        //model.customerModel.CustomerStepModel.ChatStepId = 4;
                                        //model.customerModel.CustomerStepModel.ChatStepPervoiusId = 3;
                                        //UpdateCustomer(Customer);

                                    }
                                    return infoLocation;
                                }
                                catch
                                {
                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                    infoLocation.DeliveryCostAfter = -1;
                                    infoLocation.DeliveryCostBefor = -1;
                                    infoLocation.LocationId = 0;

                                    return infoLocation;


                                }


                            }


                        }
                        catch (Exception)
                        {
                            infoLocation.Country = Country;
                            infoLocation.City = City;
                            infoLocation.Area = Area;
                            infoLocation.Distric = Distric;

                            infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                            infoLocation.DeliveryCostAfter = -1;
                            infoLocation.DeliveryCostBefor = -1;
                            infoLocation.LocationId = 0;

                            return infoLocation;

                        }
                    }




                }
                else
                {
                    //GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                    infoLocation.Country = "";
                    infoLocation.City = "";
                    infoLocation.Area = "";
                    infoLocation.Distric = "";
                    infoLocation.Address = "";
                    infoLocation.DeliveryCostAfter = -1;
                    infoLocation.DeliveryCostBefor = -1;
                    infoLocation.LocationId = 0;


                    if (infoLocation.DeliveryCostBefor != -1)
                    {
                        //Customer.CustomerStepModel.LocationId = infoLocation.LocationId;
                        //Customer.CustomerStepModel.IsLinkMneuStep = true;

                        //Customer.CustomerStepModel.DeliveryCostAfter = infoLocation.DeliveryCostAfter;
                        //Customer.CustomerStepModel.DeliveryCostBefor = infoLocation.DeliveryCostAfter;

                        //Customer.CustomerStepModel.AddressLatLong = Customer.customerChat.text;
                        //Customer.CustomerStepModel.Address = infoLocation.Address;

                        //Bot = ChatStepLink(model);
                        //model.customerModel.CustomerStepModel.ChatStepId = 4;
                        //model.customerModel.CustomerStepModel.ChatStepPervoiusId = 3;
                        //UpdateCustomer(Customer);



                    }

                    return infoLocation;
                }



            }
            catch (Exception)
            {
                Models.BotModel.GetLocationInfoModel infoLocation = new Models.BotModel.GetLocationInfoModel();

                infoLocation.Country = "";
                infoLocation.City = "";
                infoLocation.Area = "";
                infoLocation.Distric = "";

                infoLocation.Address = "";
                infoLocation.DeliveryCostAfter = -1;
                infoLocation.DeliveryCostBefor = -1;
                infoLocation.LocationId = 0;

                return infoLocation;
            }
        }
        public async Task<Web.Models.Sunshine.TenantModel> GetTenantByIdRes(int? id)
        {

            var itemsCollection = new DocumentCosmoseDB<Web.Models.Sunshine.TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            Web.Models.Sunshine.TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }
        private Models.BotModel.GetLocationInfoModel getLocationInfoPerKiloMeter(int tenantID, string query)
        {
            try
            {
                Models.BotModel.GetLocationInfoModel getLocationInfoModel = new Models.BotModel.GetLocationInfoModel();

                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    double latitude = double.Parse(query.Split(",")[0]);
                    double longitude = double.Parse(query.Split(",")[1]);
                    var rez = GetLocation(query);


                    getLocationInfoModel.Country = rez.Country.Replace("'", "").Trim();
                    getLocationInfoModel.City = rez.City.Replace("'", "").Trim();
                    getLocationInfoModel.Area = rez.Area.Replace("'", "").Trim();
                    getLocationInfoModel.Distric = rez.Distric.Replace("'", "").Trim();
                    string Route = rez.Route.Replace("'", "").Trim();

                    getLocationInfoModel.DeliveryCostAfter = -1;
                    getLocationInfoModel.DeliveryCostBefor = -1;
                    getLocationInfoModel.LocationId = 0;
                    getLocationInfoModel.Address = Route + " - " + getLocationInfoModel.Distric + " - " + getLocationInfoModel.Area + " - " + getLocationInfoModel.City + " - " + getLocationInfoModel.Country;
                    double distance;
                    AreaDto AreaDto = getNearbyArea(tenantID, latitude, longitude, null, 0, out distance);


                    if (!AreaDto.IsAvailableBranch)
                    {

                        getLocationInfoModel = new Models.BotModel.GetLocationInfoModel();


                        return getLocationInfoModel;


                    }
                    if (AreaDto.Id > 0 && distance != -1)
                    {
                        DeliveryCostDto deliveryCostDto = _iDeliveryCostAppService.GetDeliveryCostByAreaId(tenantID, AreaDto.Id);

                        if (deliveryCostDto != null)
                        {

                            decimal value = -1;
                            if (deliveryCostDto.lstDeliveryCostDetailsDto != null)
                            {
                                distance = distance / 1000.00; // convert a meter to kilo-meter
                                DeliveryCostDetailsDto deliveryCostDetailsDto = new DeliveryCostDetailsDto();
                                foreach (var item in deliveryCostDto.lstDeliveryCostDetailsDto)
                                {
                                    if (deliveryCostDetailsDto.To <= item.To)
                                        deliveryCostDetailsDto = item;

                                    if ((double)item.From <= distance && distance <= (double)item.To)
                                    {
                                        value = item.Value;
                                        break;
                                    }

                                }
                                if (value == -1)
                                {

                                    // value = deliveryCostDto.AboveValue;

                                    value = (Math.Ceiling((decimal)distance - deliveryCostDetailsDto.To) * deliveryCostDto.AboveValue) + deliveryCostDetailsDto.Value;
                                }
                            }
                            getLocationInfoModel.DeliveryCostAfter = value;
                            getLocationInfoModel.DeliveryCostBefor = value;
                        }
                    }
                    getLocationInfoModel.LocationId = (int)AreaDto.Id;

                    //if (AreaDto.IsRestaurantsTypeAll)
                    //{
                    //    getLocationInfoModel.LocationId = 0;

                    //}
                    return getLocationInfoModel;
                }
                else
                {
                    return getLocationInfoModel;
                }

            }
            catch (Exception)
            {
                return new Models.BotModel.GetLocationInfoModel()
                {

                    DeliveryCostAfter = -1,
                    DeliveryCostBefor = -1,
                    LocationId = 0

                };

            }

        }
        private bool IsvalidLatLong(string latit, string longit)
        {
            double lon, lat;
            bool IsDoubleLat, IsDoubleLong, isValidLat, isValidLong;

            IsDoubleLat = double.TryParse(latit, out lat);
            IsDoubleLong = double.TryParse(longit, out lon);

            if (IsDoubleLat && IsDoubleLong)
            {
                if (lat < -90 || lat > 90)
                {
                    isValidLat = false;
                }
                else
                {
                    isValidLat = true;
                }
                if (lon < -180 || lon > 180)
                {
                    isValidLong = false;
                }
                else
                {
                    isValidLong = true;
                }
            }
            else
            {
                isValidLong = false;
                isValidLat = false;
            }

            return isValidLong && isValidLat;
        }
        private Models.Location.locationAddressModel GetLocation(string query)
        {
            try
            {
                var client = new HttpClient();
                string Key = _configuration["GoogleMapsKey:KeyMap"];
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key=" + Key;
                var response = client.GetAsync(url).Result;

                var result = response.Content.ReadAsStringAsync().Result;
                var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<Web.Models.Sunshine.GoogleMapModel>(result);


                ////
                Models.Location.locationAddressModel locationAddressModel = new Models.Location.locationAddressModel();
                var rez = resultO;

                foreach (var item in rez.results)
                {

                    //route
                    var route = item.types.Where(x => x == "street_address").FirstOrDefault();
                    if (route != null)
                    {

                        locationAddressModel.Route = item.formatted_address.Split(",")[0];
                        //break;
                    }
                    else
                    {
                        route = item.types.Where(x => x == "route").FirstOrDefault();
                        if (route != null)
                        {

                            locationAddressModel.Route = item.formatted_address.Split(",")[0];

                        }
                        else
                        {
                            //locationAddressModel.Route = "";
                        }
                    }

                    //Distric
                    var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
                    if (neighborhood != null)
                    {


                        locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;

                    }
                    else
                    {
                        neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
                        if (neighborhood != null && locationAddressModel.Distric == null)
                        {
                            locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;


                        }
                        else
                        {
                            //locationAddressModel.Distric = "";

                        }


                    }


                    //Area
                    var sublocality_level_1 = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
                    if (sublocality_level_1 != null)
                    {

                        locationAddressModel.Area = item.address_components.FirstOrDefault().long_name;

                    }
                    else
                    {
                        //locationAddressModel.Area = "";
                        ///

                    }


                    //city
                    var locality = item.types.Where(x => x == "locality").FirstOrDefault();
                    if (locality != null)
                    {

                        locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];

                    }
                    else
                    {


                        locality = item.types.Where(x => x == "administrative_area_level_1").FirstOrDefault();
                        if (locality != null)
                        {
                            if (item.address_components.FirstOrDefault().long_name.Split(",")[0] == "Jerash Governorate")
                            {
                                locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
                            }


                        }
                        //locationAddressModel.City = "";
                    }


                    //Country
                    var country = item.types.Where(x => x == "country").FirstOrDefault();
                    if (country != null)
                    {

                        locationAddressModel.Country = item.address_components.FirstOrDefault().long_name;
                        //break;
                    }


                }

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";


                return locationAddressModel;

            }
            catch (Exception)
            {
                Models.Location.locationAddressModel locationAddressModel = new Models.Location.locationAddressModel();

                if (locationAddressModel.Route == null)
                    locationAddressModel.Route = "";
                if (locationAddressModel.Distric == null)
                    locationAddressModel.Distric = "";
                if (locationAddressModel.Area == null)
                    locationAddressModel.Area = "";
                if (locationAddressModel.City == null)
                    locationAddressModel.City = "";
                if (locationAddressModel.Country == null)
                    locationAddressModel.Country = "";


                return locationAddressModel;
            }



            //   return resultO;
        }
        private AreaDto getNearbyArea(int tenantID, double eLatitude, double eLongitude, string city, long areaId, out double distance)
        {


            distance = -1;

            bool isInAmman = !string.IsNullOrEmpty(city) && city.Trim().ToLower().Equals("amman");
            AreaDto areaDto = new AreaDto();
            var areas = _iAreasAppService.GetAllAreas(tenantID, true);
            List<AreaDto> lstAreaDto = new List<AreaDto>();

            if (areas != null)
            {
                foreach (var item in areas)
                {
                    if (checkIsInService2(item.SettingJson))
                    {
                        lstAreaDto.Add(item);

                    }
                }
            }

            if (!isInAmman && tenantID == 42)// Macdonalds
            {
                if (lstAreaDto != null)
                    lstAreaDto = lstAreaDto.Where(x => x.Id == areaId).ToList();
            }
            //int
            if (lstAreaDto != null)
            {
                foreach (var item in lstAreaDto)
                {

                    var sCoord = new GeoCoordinate(item.Latitude.Value, item.Longitude.Value);
                    var eCoord = new GeoCoordinate(eLatitude, eLongitude);
                    var currentDistance = sCoord.GetDistanceTo(eCoord);


                    if ((distance == -1 && !isInAmman) || (isInAmman && currentDistance < 5000 && distance == -1))
                    {
                        areaDto = new AreaDto();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsAvailableBranch = item.IsAvailableBranch;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                        distance = currentDistance;

                    }
                    if ((distance > currentDistance && !isInAmman) || (isInAmman && currentDistance < 5000 && distance > currentDistance))
                    {
                        areaDto = new AreaDto();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.IsAvailableBranch = item.IsAvailableBranch;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                        distance = currentDistance;

                    }

                }
            }

            return areaDto;
        }
        private bool checkIsInService2(string workingHourSetting)
        {
            bool result = true;
            if (!string.IsNullOrEmpty(workingHourSetting))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(workingHourSetting, options);

                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
                DayOfWeek wk = currentDateTime.DayOfWeek;
                TimeSpan timeOfDay = currentDateTime.TimeOfDay;

                switch (wk)
                {
                    case DayOfWeek.Saturday:
                        if (workModel.IsWorkActiveSat)
                        {
                            var StartDateSat = getValidValue(workModel.StartDateSat);
                            var EndDateSat = getValidValue(workModel.EndDateSat);

                            var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
                            var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

                            if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Sunday:
                        if (workModel.IsWorkActiveSun)
                        {
                            var StartDate = getValidValue(workModel.StartDateSun);
                            var EndDate = getValidValue(workModel.EndDateSun);

                            var StartDateSP = getValidValue(workModel.StartDateSunSP);
                            var EndDateSP = getValidValue(workModel.EndDateSunSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:

                        if (workModel.IsWorkActiveMon)
                        {
                            var StartDate = getValidValue(workModel.StartDateMon);
                            var EndDate = getValidValue(workModel.EndDateMon);

                            var StartDateSP = getValidValue(workModel.StartDateMonSP);
                            var EndDateSP = getValidValue(workModel.EndDateMonSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }

                        break;
                    case DayOfWeek.Tuesday:
                        if (workModel.IsWorkActiveTues)
                        {
                            var StartDate = getValidValue(workModel.StartDateTues);
                            var EndDate = getValidValue(workModel.EndDateTues);

                            var StartDateSP = getValidValue(workModel.StartDateTuesSP);
                            var EndDateSP = getValidValue(workModel.EndDateTuesSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (workModel.IsWorkActiveWed)
                        {
                            var StartDate = getValidValue(workModel.StartDateWed);
                            var EndDate = getValidValue(workModel.EndDateWed);

                            var StartDateSP = getValidValue(workModel.StartDateWedSP);
                            var EndDateSP = getValidValue(workModel.EndDateWedSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (workModel.IsWorkActiveThurs)
                        {
                            var StartDate = getValidValue(workModel.StartDateThurs);
                            var EndDate = getValidValue(workModel.EndDateThurs);

                            var StartDateSP = getValidValue(workModel.StartDateThursSP);
                            var EndDateSP = getValidValue(workModel.EndDateThursSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (workModel.IsWorkActiveFri)
                        {
                            var StartDate = getValidValue(workModel.StartDateFri);
                            var EndDate = getValidValue(workModel.EndDateFri);

                            var StartDateSP = getValidValue(workModel.StartDateFriSP);
                            var EndDateSP = getValidValue(workModel.EndDateFriSP);

                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {

                                result = false;
                            }
                        }
                        break;
                    default:

                        break;

                }


            }
            return result;

        }
        private void OrderOfferFun(int tenantID, bool isOrderOffer, decimal OrderTotal, Models.BotModel.GetLocationInfoModel infoLocation, string ci, string ar, string dis, decimal costDistric)
        {
            if (isOrderOffer)
            {

                //ci=ci.Replace("(All)", "").Trim();
                //ar=ar.Replace("(All)", "").Trim();
                //dis=dis.Replace("(All)", "").Trim();




                var orderEffor = GetOrderOffer(tenantID);

                if (infoLocation.LocationAreaName == null)
                    infoLocation.LocationAreaName = "";

                var item = orderEffor.Where(x => (x.BranchesIds.Contains(infoLocation.LocationId.ToString()) && x.isPersentageDiscount == false));//.FirstOrDefault(); x.Area.Contains(ci) || x.Area.Contains(ar) || x.Area.Contains(dis) &&


                foreach (var areaEffor in item)
                {


                    if (areaEffor != null)
                    {

                        if (!areaEffor.isPersentageDiscount)
                        {

                            if (OrderTotal >= areaEffor.FeesStart && OrderTotal <= areaEffor.FeesEnd)
                            {
                                var DateNow = Convert.ToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                                var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

                                if (DateStart <= DateNow && DateEnd >= DateNow)
                                {
                                    var timeNow = Convert.ToDateTime(DateTime.UtcNow.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                                    var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
                                    var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));




                                    if ((timeStart <= timeNow && timeNow <= timeEnd))
                                    {


                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = areaEffor.NewFees;
                                        infoLocation.isOrderOfferCost = true;


                                        break;

                                    }
                                    else
                                    {
                                        infoLocation.DeliveryCostAfter = costDistric;
                                        infoLocation.DeliveryCostBefor = 0;
                                        infoLocation.isOrderOfferCost = false;

                                    }


                                }
                                else
                                {
                                    infoLocation.DeliveryCostAfter = costDistric;
                                    infoLocation.DeliveryCostBefor = 0;
                                    infoLocation.isOrderOfferCost = false;

                                }


                            }
                            else
                            {
                                infoLocation.DeliveryCostAfter = costDistric;
                                infoLocation.DeliveryCostBefor = 0;
                                infoLocation.isOrderOfferCost = false;

                            }



                        }
                        else
                        {
                            infoLocation.DeliveryCostAfter = costDistric;
                            infoLocation.DeliveryCostBefor = 0;
                            infoLocation.isOrderOfferCost = false;

                        }



                    }
                    else
                    {
                        infoLocation.DeliveryCostAfter = costDistric;
                        infoLocation.DeliveryCostBefor = 0;
                        infoLocation.isOrderOfferCost = false;

                    }


                }


            }
            else
            {
                infoLocation.DeliveryCostAfter = costDistric;
                infoLocation.DeliveryCostBefor = 0;
                infoLocation.isOrderOfferCost = false;

            }

        }
        private List<OrderOffers.OrderOffer> GetOrderOffer(int TenantID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderOffer] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<OrderOffers.OrderOffer> order = new List<OrderOffers.OrderOffer>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {


                    try
                    {
                        order.Add(new OrderOffers.OrderOffer
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),

                            BranchesName = dataSet.Tables[0].Rows[i]["BranchesName"].ToString(),
                            BranchesIds = dataSet.Tables[0].Rows[i]["BranchesIds"].ToString(),


                        });
                    }
                    catch
                    {
                        order.Add(new OrderOffers.OrderOffer
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
                            Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
                            FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
                            FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
                            NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
                            TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
                            isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
                            OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
                            OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
                            OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),//.AddHours(AppSettingsModel.AddHour),
                            isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),
                            isBranchDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isBranchDiscount"].ToString()),




                        });

                    }


                }

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private List<ItemDto> GetItem(int? TenantID, long id)
        {

            List<ItemDto> itemDtos = new List<ItemDto>();
            ItemDto item = _itemsAppService.GetItemInfoForBot(id, TenantID.Value);
            itemDtos.Add(item);
            return itemDtos;
        }
        private List<BotAPI.Models.Location.LocationInfoModel> GetAllLocationInfoModel()
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Locations] ";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<BotAPI.Models.Location.LocationInfoModel> locationInfoModel = new List<BotAPI.Models.Location.LocationInfoModel>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        locationInfoModel.Add(new BotAPI.Models.Location.LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                        });
                    }
                    catch
                    {
                        locationInfoModel.Add(new BotAPI.Models.Location.LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                        });

                    }


                }

                conn.Close();
                da.Dispose();

                return locationInfoModel;

            }
            catch
            {
                return null;

            }

        }
        private List<Area> GetAreas(int TenantID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + TenantID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            DataSet dataSet = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {

                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString(),
                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString()

                    });
                }
                catch
                {
                    branches.Add(new Area
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                    });

                }

            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        //second API when chhose الفرع
        public Area GetAreasID2(string TenantID = "27", string AreaName = "فرع الراس", int menu = 0, string local = "ar")
        {
            var list = GetAreasList(TenantID);

            var area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();

            if (local == "ar")
            {

                area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();
            }
            else
            {
                area = list.Where(x => (x.AreaNameEnglish).Contains(AreaName)).FirstOrDefault();
            }


            if (area == null)
            {
                Area area1 = new Area();
                area1.Id = 0;
                return area1;
            }


            if (area.SettingJson != null && area.SettingJson != "")
            {
                area.SettingJson = GetWorkingHour(area.SettingJson);
            }
            else
            {
                area.SettingJson = ("not found");

            }
            return area;
        }
        private List<Area> GetAreasList(string TenantID)
        {
            //TenantID = "31";
            var tenantID = int.Parse(TenantID);
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + tenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString());

                if (IsAvailableBranch)
                {
                    try
                    {
                        branches.Add(new Area
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
                            AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
                            AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
                            AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                            UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                            RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                            IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                            IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                            IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                            UserIds = (dataSet.Tables[0].Rows[i]["UserIds"].ToString()),
                            SettingJson = dataSet.Tables[0].Rows[i]["SettingJson"].ToString()


                        });
                    }
                    catch
                    {
                        branches.Add(new Area
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
                            AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
                            AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
                            AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                            BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
                            UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
                            RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                            IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
                            IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
                            IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                            UserIds = (dataSet.Tables[0].Rows[i]["UserIds"].ToString()),
                            //SettingJson = dataSet.Tables[0].Rows[i]["SettingJson"].ToString()


                        });
                    }


                }

            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        private string GetWorkingHour(string _AreaSettingJson)
        {

            // var List = new List<string>();

            string x = "";
            if (_AreaSettingJson != null && _AreaSettingJson != "")
            {
                Web.Models.Sunshine.WorkModelNBot workmodel = JsonConvert.DeserializeObject<Web.Models.Sunshine.WorkModelNBot>(_AreaSettingJson);

                if (workmodel.WorkTextAR != "" && workmodel.WorkTextEN != "")
                {
                    x = "[ " + workmodel.WorkTextAR + " /" + workmodel.WorkTextEN + " ]";
                }

                if (workmodel.WorkTextFri != "" && workmodel.IsWorkActiveFri)
                {
                    x = x + " -" + workmodel.WorkTextFri;
                }
                if (workmodel.WorkTextSat != "" && workmodel.IsWorkActiveSat)
                {

                    x = x + " -" + workmodel.WorkTextSat;
                }

                if (workmodel.WorkTextSun != "" && workmodel.IsWorkActiveSun)
                {
                    x = x + " -" + workmodel.WorkTextSun;

                }

                if (workmodel.WorkTextMon != "" && workmodel.IsWorkActiveMon)
                {
                    x = x + " -" + workmodel.WorkTextMon;
                }

                if (workmodel.WorkTextTues != "" && workmodel.IsWorkActiveTues)
                {
                    x = x + " -" + workmodel.WorkTextTues;
                }
                if (workmodel.WorkTextWed != "" && workmodel.IsWorkActiveWed)
                {
                    x = x + " -" + workmodel.WorkTextWed;
                }
                if (workmodel.WorkTextThurs != "" && workmodel.IsWorkActiveThurs)
                {
                    x = x + " -" + workmodel.WorkTextThurs;
                }



            }





            return x;

        }


        [HttpGet("GetOrderDetalisDelvery")]
        public OrderDetalisRes step5
                  (string SelectedAreaId,
                    string ContactID,
                    string LangString,
                    string displayName,
                    int tenantID,
                    string PhoneNumbe,
                    string OrderType,
                    int langId,
                    int LocationId,
                    decimal DeliveryCostBefor,
                    decimal DeliveryCostAfter,
                    decimal discount = 0
                 )
        {

            decimal tax = 0;
            Web.Models.Sunshine.TenantModel tenant = GetTenantByIdRes(tenantID).Result;
            if (tenant.TaxValue != null && tenant.IsTaxOrder)
            {
                tax = tenant.TaxValue;
            }
            else
            {
                tax = 0;
            }

                OrderAndDetailsModel Details = null;
            OrderDetalisRes orderDetalis = new OrderDetalisRes();

            if (true)
            //if (Customer.customerChat.text == "اختبار")
            {
                GetOrderAndDetailModel sendOrderAndDetailModel = new GetOrderAndDetailModel();

                if (LangString=="ar")
                {
                    sendOrderAndDetailModel.DeliveryCostTextTow = "";
                    sendOrderAndDetailModel.captionQuantityText = "العدد: ";
                    sendOrderAndDetailModel.captionAddtionText = "الاضافات";
                    sendOrderAndDetailModel.captionTotalText = "المجموع:   ";
                    sendOrderAndDetailModel.captionTotalOfAllText = "السعر الإجمالي للطلب";
                }
                else {
                    sendOrderAndDetailModel.DeliveryCostTextTow = "";
                    sendOrderAndDetailModel.captionQuantityText = "Number: ";
                    sendOrderAndDetailModel.captionAddtionText = "Additions";
                    sendOrderAndDetailModel.captionTotalText = "Total:   ";
                    sendOrderAndDetailModel.captionTotalOfAllText = "Total Order Price";
                }

                if (OrderType == "Pickup")
                {
                    sendOrderAndDetailModel.ContactId = int.Parse(ContactID);
                    sendOrderAndDetailModel.TenantID = tenantID;
                    sendOrderAndDetailModel.lang = LangString;
                    sendOrderAndDetailModel.MenuType = SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId = int.Parse(SelectedAreaId);
                    sendOrderAndDetailModel.isOrderOffer = false;
                    sendOrderAndDetailModel.Tax = tax;

                }
                else if (OrderType == "Delivery")
                {

                    sendOrderAndDetailModel.ContactId = int.Parse(ContactID);
                    sendOrderAndDetailModel.TenantID = tenantID;
                    sendOrderAndDetailModel.lang = LangString;
                    sendOrderAndDetailModel.MenuType = SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId = (int)LocationId;

                    sendOrderAndDetailModel.isOrderOffer = false;

                    sendOrderAndDetailModel.TypeChoes = OrderType;
                    sendOrderAndDetailModel.deliveryCostBefor = DeliveryCostBefor;
                    sendOrderAndDetailModel.deliveryCostAfter = DeliveryCostAfter;
                    sendOrderAndDetailModel.Cost = DeliveryCostAfter;
                    sendOrderAndDetailModel.Tax = tax;


                    sendOrderAndDetailModel.LocationInfo = null;


                }

                Details = _botApis.GetOrderAndDetailsFlowsBot(sendOrderAndDetailModel
                   );

                orderDetalis.detailText = Details.DetailText;


                if (Details != null)
                {
                    orderDetalis.detailText = Details.DetailText;
                    orderDetalis.OrderTotal = Details.total;

                    //Bot = ChatStepOrderDetails(model);

                    //orderDetalis.OrderCreationTime = Details.orderDetailDtos.CreationTime;

                    var firstDetail = Details.orderDetailDtos?.FirstOrDefault();
                    if (firstDetail != null)
                    {
                        orderDetalis.OrderCreationTime = firstDetail.CreationTime;
                    }

                    //Details.orderDetailDtos.CreationTime;

                    orderDetalis.OrderId = Details.orderId;


                    orderDetalis.Tax = tax;
                    orderDetalis.Discount = Details.Discount;




                    if (Details.IsDeliveryOffer)
                    {
                        //Customer.CustomerStepModel.DeliveryCostAfter = Details.GetLocationInfo.DeliveryCostAfter;
                        orderDetalis.DeliveryCostAfter = Details.GetLocationInfo.DeliveryCostAfter;

                        //  model.customerModel.CustomerStepModel.DeliveryCostBefor=Details.GetLocationInfo.DeliveryCostBefor;
                        orderDetalis.DeliveryCostBefor = Details.GetLocationInfo.DeliveryCostBefor;

                        //Customer.CustomerStepModel.IsDeliveryOffer = Details.IsDeliveryOffer;
                        orderDetalis.IsDeliveryOffer = Details.IsDeliveryOffer.ToString();

                        //Customer.CustomerStepModel.DeliveryOffer = Customer.CustomerStepModel.DeliveryCostBefor;
                        orderDetalis.DeliveryOffer = Details.GetLocationInfo.DeliveryCostBefor;
                    }
                    else
                    {
                        orderDetalis.IsDeliveryOffer = Details.IsDeliveryOffer.ToString();
                    }

                    if (Details.IsItemOffer)
                    {
                        //Customer.CustomerStepModel.IsItemOffer = Details.IsItemOffer;
                        orderDetalis.IsItemOffer = Details.IsItemOffer.ToString();

                        //Customer.CustomerStepModel.ItemOffer = Details.Discount;
                        orderDetalis.ItemOffer = Details.Discount;

                        //Customer.CustomerStepModel.Discount = Details.Discount;
                        orderDetalis.Discount = Details.Discount;
                    }
                    else
                    {
                        orderDetalis.IsItemOffer = "false";
                    }


                }

            }
            return orderDetalis;
        }


        private CustomerModel GetCustomer(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }

        [HttpGet("GetCustomer32")]

        public CustomerModel GetCustomer321(string id)
        {
            id = "27_962785495992";
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }
        public CustomerModel GetCustomer23(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }

        //create order
        [HttpGet("CreateOrder")]
        public string ChatStepCreateOrder(
                    string SelectedAreaId,
                    string ContactID,
                    string LangString,
                    string displayName,
                    int tenantID,
                    string PhoneNumbe,
                    string orderType,
                    int langId,
                    decimal deliveryCostBefor,
                    decimal deliveryCostAfter,
                    string addressLatLong,
                    string IsItemOffer,
                    string address,
                    string isDeliveryOffer,
                    int locationId,
                    decimal orderid,
                    decimal orderTotal,
                    decimal discount,
                    string IsPreOrder,
                    string SelectDay,
                    string SelectTime,
                    string BayType ,
                    decimal Tax ,
                    string DeliveryEstimation
            )

        {

            Web.Models.Sunshine.TenantModel tenant = GetTenantByIdRes(tenantID).Result;
            if (tenant.TaxValue != null && tenant.IsTaxOrder)
            {
                Tax = tenant.TaxValue;
            }
            else
            {
                Tax = 0;
            }
            bool IsItemOffer2 = bool.Parse(IsItemOffer);
            bool IsPreOrder2 = bool.Parse(IsPreOrder);
            bool isDeliveryOffer2 = bool.Parse(isDeliveryOffer);


            string userId = tenantID + "_" + PhoneNumbe;
            //var cacheKey = "Step_" + tenantID.ToString();
            //var objTenant1 = _cacheManager.GetCache("CacheTenant_CaptionStps").Get(cacheKey, cache => null);

            //List<CaptionDto> caption;
            //if (objTenant1 == null)
            //{
            //caption = _botApis.GetAllCaption(tenantID, LangString);
            //_cacheManager.GetCache("CacheTenant_CaptionStps").Set(cacheKey, caption);
            //}
            //else
            //{
            //    captionDtos = (List<CaptionDto>)objTenant1;
            //}
            UpdateOrderModel updateOrderModel = new UpdateOrderModel();

            if (LangString == "ar")
            {
                //updateOrderModel.captionOrderInfoText = caption.Where(x => x.TextResourceId == 22 && x.LanguageBotId == langId).FirstOrDefault().Text;//22
                updateOrderModel.captionOrderInfoText = "------------------ \r\n\r\nشكرا لك معلومات الطلب\r\nرقم الطلب :  {0}\r\nقيمة التوصيل :  {1}\r\nمن الموقع :  {2}\r\nمن الفرع : {3}\r\nالسعر الإجمالي للطلب: {4}\r\n\r\n------------------ \r\n\r\nسوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم  ";


                //updateOrderModel.captionOrderNumberText = caption.Where(x => x.TextResourceId == 24 && x.LanguageBotId == langId).FirstOrDefault().Text;//24
                updateOrderModel.captionOrderNumberText = "------------------\r\n\r\nشكرا لك معلومات الطلب\r\nرقم الطلب: {0}\r\nمن الفرع : {1}\r\nالسعر الإجمالي للطلب: {2}\r\n------------------\r\ntest\r\nسوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم  ";
                //updateOrderModel.captionTotalOfAllOrderText=caption.Where(x => x.TextResourceId==21 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//21
                //updateOrderModel.captionEndOrderText = caption.Where(x => x.TextResourceId == 25 && x.LanguageBotId == langId).FirstOrDefault().Text;//25
                updateOrderModel.captionEndOrderText = "------------------\r\n\r\nشكرا لك معلومات الطلب\r\nرقم الطلب :  {0}\r\nقيمة التوصيل :  {1}\r\nمن الموقع :  {2}\r\nمن الفرع : {3}\r\nالسعر الإجمالي للطلب: {4}\r\nالسعر بنقاط: {5}\r\ntest\r\n------------------\r\n\r\nسوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم  ";

                // updateOrderModel.aptionAreaNameText=caption.Where(x => x.TextResourceId==28 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//28
                //updateOrderModel.captionBranchCostText = caption.Where(x => x.TextResourceId == 26 && x.LanguageBotId == langId).FirstOrDefault().Text;//26
                updateOrderModel.captionBranchCostText = "------------------\r\n\r\nشكرا لك معلومات الطلب\r\nرقم الطلب: {0}\r\nمن الفرع : {1}\r\nالسعر الإجمالي للطلب: {2}\r\nالسعر بنقاط: {3}\r\ntest\r\n------------------\r\n\r\nسوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم   ";                                                                                                                              // updateOrderModel.captionFromLocationText=caption.Where(x => x.TextResourceId==27 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//27

            }
            else
            {
                updateOrderModel.captionOrderInfoText = "------------------ \r\n\r\nThank you for your order information\r\nOrder Number: {0}\r\nDelivery Cost: {1}\r\nFrom Location: {2}\r\nFrom Branch: {3}\r\nTotal Order Price: {4}\r\n\r\n------------------ \r\n\r\nYou will be notified when your order preparation starts.. Thank you.";
                updateOrderModel.captionOrderNumberText = "------------------\r\n\r\nThank you for your order information\r\nOrder Number: {0}\r\nFrom Branch: {1}\r\nTotal Order Price: {2}\r\n------------------\r\ntest\r\nYou will be notified when your order preparation starts.. Thank you.";
                updateOrderModel.captionEndOrderText = "------------------\r\n\r\nThank you for your order information\r\nOrder Number: {0}\r\nDelivery Cost: {1}\r\nFrom Location: {2}\r\nFrom Branch: {3}\r\nTotal Order Price: {4}\r\nPrice in Points: {5}\r\ntest\r\n------------------\r\n\r\nYou will be notified when your order preparation starts.. Thank you.";

                updateOrderModel.captionBranchCostText = "------------------\r\n\r\nThank you for your order information\r\nOrder Number: {0}\r\nFrom Branch: {1}\r\nTotal Order Price: {2}\r\nPrice in Points: {3}\r\ntest\r\n------------------\r\n\r\nYou will be notified when your order preparation starts.. Thank you.";

            }

            if (orderType == "Pickup")
            {
                updateOrderModel.ContactId = int.Parse(ContactID);
                updateOrderModel.TenantID = tenantID;
                updateOrderModel.OrderTotal = orderTotal;

                updateOrderModel.loyalityPoint = 0;
                updateOrderModel.OrderId = int.Parse(orderid.ToString());
                updateOrderModel.Tax = Tax;

                updateOrderModel.MenuId = int.Parse(SelectedAreaId.ToString());
                updateOrderModel.BranchId = int.Parse(SelectedAreaId);
                updateOrderModel.BranchName = "";
                updateOrderModel.TypeChoes = orderType;
                updateOrderModel.IsSpecialRequest = false;
                updateOrderModel.SpecialRequest = "";
                updateOrderModel.BotLocal = LangString;
                updateOrderModel.BuyType = BayType;

                updateOrderModel.IsItemOffer = IsItemOffer2;
                updateOrderModel.ItemOffer = discount;

                updateOrderModel.isOrderOfferCost = IsItemOffer2;

            }
            else
            {
                updateOrderModel.ContactId = int.Parse(ContactID);
                updateOrderModel.TenantID = tenantID;
                updateOrderModel.OrderTotal = orderTotal;
                updateOrderModel.loyalityPoint = 0;
                updateOrderModel.OrderId = int.Parse(orderid.ToString());

                updateOrderModel.MenuId = int.Parse(locationId.ToString());
                updateOrderModel.BranchId = updateOrderModel.MenuId;
                updateOrderModel.BranchName = "";
                updateOrderModel.TypeChoes = orderType;
                updateOrderModel.IsSpecialRequest = false;
                updateOrderModel.SpecialRequest = "";
                updateOrderModel.BotLocal = LangString;

                updateOrderModel.IsItemOffer = IsItemOffer2;
                updateOrderModel.ItemOffer = discount;
                updateOrderModel.DeliveryOffer = deliveryCostBefor;
                updateOrderModel.Tax = Tax;

                updateOrderModel.Address = address;
                updateOrderModel.DeliveryCostAfter = deliveryCostAfter;
                updateOrderModel.DeliveryCostBefor = deliveryCostBefor;
                //updateOrderModel.IsPreOrder = Customer.CustomerStepModel.IsPreOrder;
                updateOrderModel.IsPreOrder = IsPreOrder2;

                //updateOrderModel.SelectDay = Customer.CustomerStepModel.SelectDay;
                //updateOrderModel.SelectTime = Customer.CustomerStepModel.SelectTime;
                updateOrderModel.SelectDay = DateTime.Now.DayOfWeek.ToString(); // e.g., "Monday"
                updateOrderModel.SelectTime = DateTime.Now.ToString("HH:mm");   // e.g., "14:30"


                updateOrderModel.IsPreOrder = IsPreOrder2;
                //updateOrderModel.SelectDay = Customer.CustomerStepModel.SelectDay;
                //updateOrderModel.SelectTime = Customer.CustomerStepModel.SelectTime;

                updateOrderModel.LocationFrom = addressLatLong;
                updateOrderModel.BuyType = BayType;



                //updateOrderModel.IsItemOffer = IsItemOffer2;
                updateOrderModel.ItemOffer = discount;

                updateOrderModel.IsDeliveryOffer = isDeliveryOffer2;
                updateOrderModel.DeliveryOffer = deliveryCostBefor;

                updateOrderModel.isOrderOfferCost = IsItemOffer2;

            }
            //is important
            //updateOrderModel.loyalityPoint = Customer.CustomerStepModel.TotalPoints;
            updateOrderModel.DeliveryEstimation = DeliveryEstimation;
            var text = _botApis.UpdateOrderAsync(updateOrderModel);


            var area = GetAreasList(updateOrderModel.TenantID.ToString()).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();

            string areaName;
            if (LangString == "ar")
            {
                areaName = area.AreaName;
            }
            else
            {
                areaName = area.AreaNameEnglish;
            }
            return areaName;
        }

        private void UpdateCustomer(CustomerModel customer)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == customer.userId);

            var result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;

        }

        [HttpGet("GetBranchCount")]
        public AreaResponseDto GetAreasByTenant(int tenantId)
        {
            try
            {
                var SP_Name = "GetBranchCountByTenant";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId)
                };

                var areas = SqlDataHelper
                    .ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertAreaToDto, AppSettingsModel.ConnectionStrings)
                    .ToList();

                if (areas != null && areas.Count == 1)
                {
                    var area = areas[0];
                    return new AreaResponseDto
                    {
                        Count = areas.Count,
                        Id = area.Id,
                        AreaName = area.AreaName
                    };
                }

                return new AreaResponseDto
                {
                    Count = areas.Count
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private static AreaDto ConvertAreaToDto(IDataReader reader)
        {
            var area = new AreaDto();
            area.Id = SqlDataHelper.GetValue<int>(reader, "Id");
            area.AreaName = SqlDataHelper.GetValue<string>(reader, "AreaName");
            return area;
        }
        [HttpGet("GetBranchNamesEnglish")]
        public string GetBranchNamesEnglish(int TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "SELECT AreaNameEnglish FROM [dbo].[Areas] WHERE TenantID = " + TenantID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();
            da.Fill(dataSet);

            List<string> areaNamesEnglish = new List<string>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                string name = row["AreaNameEnglish"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    areaNamesEnglish.Add(name.Trim());
                }
            }

            conn.Close();
            da.Dispose();
            //string x = "14سم ميني كيترنج,30سم كيترنج,45سم كيترنج";
            //return x;
            // Join the names with commas
            return string.Join(", ", areaNamesEnglish);
        }


        [HttpGet("GetBranchID")]
        public decimal GetBranchID(int TenantID, string BranchName)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "SELECT Id FROM [dbo].[Areas] WHERE TenantID = @TenantID AND AreaNameEnglish = @BranchName";

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TenantID", TenantID);
                cmd.Parameters.AddWithValue("@BranchName", BranchName);

                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null && decimal.TryParse(result.ToString(), out decimal branchId))
                {
                    return branchId;
                }
                return -1;
            }
        }



        #endregion

        #region Careem 
        [HttpGet("GetEstimateDelivery")]
        public async Task<EstimateDeliveryResponse> GetEstimateDelivery(double customerLongitude, double customerLatitude, int tenantId)
        {

            EstimateDeliveryResponse DeliveryEstimation = new EstimateDeliveryResponse();
            Models.BotModel.SendLocationUserModel model = new Models.BotModel.SendLocationUserModel
            {
                query = $"{customerLatitude},{customerLongitude}",
                tenantID = tenantId,
                isOrderOffer = false,
                OrderTotal = 0,
                menu = 0,
                local = "ar",
                isChangeLocation = false,
                address = ""
            };

            var merchantLocation = GetlocationUserModel(model);
            AreaDto area = _iAreasAppService.GetAreaById(merchantLocation.LocationId, tenantId);
            var tenant = await GetTenantByIdPrivate(tenantId);

            EtimateDeliveryDTO estimateDelivery = new EtimateDeliveryDTO();
            var deliveryType = DeliveryType.LMD;
            if (tenant != null && tenant?.DeliveryType != null)
            {
                var deliveryTypes = tenant.DeliveryType.Split(",");
                deliveryType = deliveryTypes.Contains(((int)DeliveryType.LMD).ToString()) ? DeliveryType.LMD : DeliveryType.LMD_DELIVERY;
            }
            estimateDelivery.type = deliveryType.ToString();

            estimateDelivery.pickup = new EtimateDeliveryDTO.Pickup
            {
                coordinate = new EtimateDeliveryDTO.Coordinate
                {
                    latitude = (double)area.Latitude,
                    longitude = (double)area.Longitude
                }
            };

            estimateDelivery.dropoff = new EtimateDeliveryDTO.Dropoff
            {
                coordinate = new EtimateDeliveryDTO.Coordinate
                {
                    latitude = customerLatitude,
                    longitude = customerLongitude
                }
            };

            using (var client = new HttpClient())
            {
                var postUrl = "https://cnow-transporter-service.core.gw.prod.careem-rh.com/deliveries/estimate";

                var body = System.Text.Json.JsonSerializer.Serialize(estimateDelivery);
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var accessToken = string.Empty; //where to get it ??
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    var response = await client.PostAsync(postUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new UserFriendlyException($"Failed to Estimate delivery: {error}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    DeliveryEstimation = System.Text.Json.JsonSerializer.Deserialize<EstimateDeliveryResponse>(responseBody);
                    return DeliveryEstimation;
                }
                catch (HttpRequestException ex)
                {
                    throw;
                }
            }

        }

        #region private 

        private Models.BotModel.GetLocationInfoModel GetlocationUserModel(Models.BotModel.SendLocationUserModel input)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {
                    Web.Models.Sunshine.TenantModel tenant = GetTenantByIdPrivate(input.tenantID).Result;

                    input.isOrderOffer = tenant.isOrderOffer;
                    if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
                    {
                        Models.BotModel.GetLocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);
                        return getLocationInfoModel;
                    }
                    else
                    {
                        Models.BotModel.GetLocationInfoModel infoLocation = new Models.BotModel.GetLocationInfoModel();
                        string result = "-1";
                        string Country = "";
                        string City = "";
                        string Area = "";
                        string Distric = "";
                        string Route = "";

                        if (input.isChangeLocation)
                        {
                            Country = input.address.Split(" - ")[4].Replace("'", "").Trim();
                            City = input.address.Split(" - ")[3].Replace("'", "").Trim();
                            Area = input.address.Split(" - ")[2].Replace("'", "").Trim();
                            Distric = input.address.Split(" - ")[1].Replace("'", "").Trim();
                            Route = input.address.Split(" - ")[0].Replace("'", "").Trim();
                        }
                        else
                        {
                            var rez = GetLocation(input.query);
                            Country = rez.Country.Replace("'", "").Trim();
                            City = rez.City.Replace("'", "").Trim();
                            Area = rez.Area.Replace("'", "").Trim();
                            Distric = rez.Distric.Replace("'", "").Trim();
                            Route = rez.Route.Replace("'", "").Trim();
                        }

                        if (Distric == "Irbid Qasabah District" && Area == "")
                        {
                            Distric = Route;
                        }

                        if (Distric == "الجوادين" && Area == "")
                        {
                            Distric = "aljawadayn";
                        }
                        else if (Distric == "الساهرون" && Area == "")
                        {
                            Distric = "alsaahirun";
                        }
                        else if (Distric == "حي الضباط" && Area == "")
                        {
                            Distric = "hayu aldubaat";
                        }
                        else if (Distric == "الحي العسكري" && Area == "")
                        {
                            Distric = "alhayu aleaskariu";
                        }
                        else if (Distric == "البيضاء" && Area == "")
                        {
                            Distric = "albayda";
                        }
                        else if (Distric == "الرئاسة" && Area == "")
                        {
                            Distric = "alriyasa";
                        }
                        else if (Distric == "عدن" && Area == "")
                        {
                            Distric = "eadn";
                        }

                        try
                        {

                            decimal Latitude = decimal.Parse(input.query.Split(",")[0]);
                            decimal Longitude = decimal.Parse(input.query.Split(",")[1]);
                            try
                            {
                                using (var connection = new SqlConnection(connString))
                                using (var command = connection.CreateCommand())
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "dbo.GetDeliveryCost";

                                    command.Parameters.AddWithValue("@Longitude", Longitude);
                                    command.Parameters.AddWithValue("@Latitude", Latitude);
                                    command.Parameters.AddWithValue("@City", City);
                                    command.Parameters.AddWithValue("@Area", Area);
                                    command.Parameters.AddWithValue("@Distric", Distric);
                                    command.Parameters.AddWithValue("@TenantId", input.tenantID);

                                    System.Data.SqlClient.SqlParameter returnValue = command.Parameters.Add("@DeliveryCost", SqlDbType.NVarChar);
                                    returnValue.Direction = ParameterDirection.ReturnValue;

                                    connection.Open();
                                    command.ExecuteNonQuery();

                                    result = returnValue.Value.ToString();

                                }
                            }

                            catch (Exception ex)
                            {
                                throw ex;
                            }


                            var spilt = result.Split(",");
                            decimal va1 = -1;

                            try
                            {
                                va1 = decimal.Parse(spilt[0].ToString());
                            }
                            catch
                            {
                                throw;
                            }

                            if (va1 < 0)
                            {
                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;

                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;
                            }

                            if (spilt[0] == "-1" || spilt[0] == "")
                            {

                                infoLocation.Country = Country;
                                infoLocation.City = City;
                                infoLocation.Area = Area;
                                infoLocation.Distric = Distric;
                                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                infoLocation.DeliveryCostAfter = -1;
                                infoLocation.DeliveryCostBefor = -1;
                                infoLocation.LocationId = 0;

                                return infoLocation;

                            }
                            else
                            {
                                try
                                {

                                    var locationList = GetAllLocationInfoModel();
                                    var cost = decimal.Parse(spilt[0]);
                                    var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

                                    var cityModel = locationList.Where(x => x.LocationNameEn == City).FirstOrDefault();
                                    var areaModel = locationList.Where(x => x.LocationNameEn == Area).FirstOrDefault();
                                    var districModel = locationList.Where(x => x.LocationNameEn == Distric).FirstOrDefault();

                                    var areaname = GetAreas(input.tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

                                    var cityName = "";
                                    var areaName = "";
                                    var districName = "";

                                    if (cityModel != null)
                                        cityName = cityModel.LocationName;
                                    if (areaModel != null)
                                        areaName = areaModel.LocationName;
                                    if (districModel != null)
                                        districName = districModel.LocationName;

                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = add;

                                    infoLocation.LocationId = int.Parse(spilt[1]);
                                    infoLocation.LocationAreaName = areaname.AreaName;

                                    if (!areaname.IsAvailableBranch)
                                    {
                                        infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                        infoLocation.DeliveryCostAfter = -1;
                                        infoLocation.DeliveryCostBefor = -1;
                                        infoLocation.LocationId = 0;

                                        return infoLocation;
                                    }

                                    OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);

                                    if (areaname != null)
                                    {
                                        if (input.local == "ar")
                                        {
                                            infoLocation.LocationAreaName = areaname.AreaName;
                                        }
                                        else
                                        {
                                            if (areaname.AreaNameEnglish == null)
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaName;
                                            }
                                            else
                                            {
                                                infoLocation.LocationAreaName = areaname.AreaNameEnglish;
                                            }
                                        }
                                    }
                                    infoLocation.DeliveryCostAfter = cost;
                                    if (infoLocation.DeliveryCostBefor == null)
                                    {
                                        infoLocation.DeliveryCostBefor = 0;
                                    }
                                    return infoLocation;
                                }
                                catch
                                {
                                    infoLocation.Country = Country;
                                    infoLocation.City = City;
                                    infoLocation.Area = Area;
                                    infoLocation.Distric = Distric;

                                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                    infoLocation.DeliveryCostAfter = -1;
                                    infoLocation.DeliveryCostBefor = -1;
                                    infoLocation.LocationId = 0;

                                    return infoLocation;
                                }
                            }

                        }
                        catch (Exception)
                        {
                            infoLocation.Country = Country;
                            infoLocation.City = City;
                            infoLocation.Area = Area;
                            infoLocation.Distric = Distric;

                            infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                            infoLocation.DeliveryCostAfter = -1;
                            infoLocation.DeliveryCostBefor = -1;
                            infoLocation.LocationId = 0;

                            return infoLocation;
                        }
                    }
                }
                else
                {
                    Models.BotModel.GetLocationInfoModel infoLocation = new Models.BotModel.GetLocationInfoModel();
                    infoLocation.Country = "";
                    infoLocation.City = "";
                    infoLocation.Area = "";
                    infoLocation.Distric = "";
                    infoLocation.Address = "";
                    infoLocation.DeliveryCostAfter = -1;
                    infoLocation.DeliveryCostBefor = -1;
                    infoLocation.LocationId = 0;

                    return infoLocation;
                }
            }
            catch (Exception)
            {
                Models.BotModel.GetLocationInfoModel infoLocation = new Models.BotModel.GetLocationInfoModel();

                infoLocation.Country = "";
                infoLocation.City = "";
                infoLocation.Area = "";
                infoLocation.Distric = "";

                infoLocation.Address = "";
                infoLocation.DeliveryCostAfter = -1;
                infoLocation.DeliveryCostBefor = -1;
                infoLocation.LocationId = 0;

                return infoLocation;
            }

        }

        private async Task<Web.Models.Sunshine.TenantModel> GetTenantByIdPrivate(int? id)
        {
            var itemsCollection = new DocumentCosmoseDB<Web.Models.Sunshine.TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            Web.Models.Sunshine.TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }

        private Contact GetContactByPhoneNumber(string phoneNumber, int tenantId)
        {
            try
            {
                var result = new Contact();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetContactByPhoneNumber", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                result.TenantId = reader.GetInt32(reader.GetOrdinal("TenantId"));
                                result.DisplayName = reader["DisplayName"] as string;
                                result.PhoneNumber = reader["DisplayName"] as string;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #endregion

    }
}

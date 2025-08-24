using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Caching;
using AutoMapper;
using Framework.Data;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Framework.Integration.Model;
using Framework.Payment.Implementation.Zoho;
using Framework.Payment.Interfaces.Zoho;
using GeoCoordinatePortable;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Asset;
using Infoseed.MessagingPortal.Asset.Dto;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Booking;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.BotAPI.Models.Firebase;
using Infoseed.MessagingPortal.BotAPI.Models.Location;
using Infoseed.MessagingPortal.BotAPI.Models.plants;
using Infoseed.MessagingPortal.BotAPI.Models.Utrac;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.ContactNotification;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Customers;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.DashboardUI;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using Infoseed.MessagingPortal.DeliveryCost;
using Infoseed.MessagingPortal.DeliveryCost.Dto;
using Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos;
using Infoseed.MessagingPortal.Evaluations;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.Maintenance;
using Infoseed.MessagingPortal.Maintenance.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Notifications;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Orders.Exporting;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Infoseed.MessagingPortal.Zoho.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nancy.Json;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static Infoseed.MessagingPortal.Customers.Dtos.CustomerBehaviourEnums;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;
using static Infoseed.MessagingPortal.LiveChat.Dto.LiveChatEnums;
using CreateAppointmentMGModel = Framework.Integration.Model.CreateAppointmentMGModel;
using Match = System.Text.RegularExpressions.Match;
using SqlParameter = System.Data.SqlClient.SqlParameter;
using UserRoleDto = Infoseed.MessagingPortal.BotAPI.Models.UserRoleDto;

namespace Infoseed.MessagingPortal.Bot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotAPIController : MessagingPortalControllerBase
    {

        //  private string serverKey = "AAAAKInP3fI:APA91bHUaWCGK8ea8261aoGceJ4uGd7mJLHAl5vUMDiUFskG_t5iImtYqYMaLHZpiRHyFNAA2Cl4zEQ18Gnq6pRbP57PEhw1Z8DYIL9G07wqvZcaW1Gg7gbqqOvY403IzoJfQXx-CqSi";
        //  private string senderId = "174110793202";
        // private string webAddr = "https://fcm.googleapis.com/fcm/send";

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
        private readonly ITenantDashboardAppService _TenantDashboardAppService;

        public BotAPIController(
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
            IWhatsAppMessageTemplateAppService templateAppService
            , ITenantDashboardAppService iTenantDashboardAppService
            )
        {
            _menusAppService= menusAppService;
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
            _loyaltyAppService=loyaltyAppService;
            _invoices = new InvoicesApi(_configuration);
            _bookingAppService = bookingAppService;
            _userRoleRepository=userRoleRepository;

            _roleManager=roleManager;

            _userNotificationManager=userNotificationManager;
            _contactNotification=contactNotification;
            _appNotifier=appNotifier;
            _captionBotAppService=captionBotAppService;
            _dashboardAppService =dashboardAppService;
            _templateAppService = templateAppService;
            _TenantDashboardAppService = iTenantDashboardAppService;
        }


        public BotAPIController()
        {

        }




        [Route("TestLocation")]
        [HttpGet]

        public string TestLocation(string query ,int tenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
           
                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {


                    TenantModel tenant = GetTenantById(tenantID).Result;

                    GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                    string result = "-1";
                    string Country = "";
                    string City = "";
                    string Area = "";
                    string Distric = "";
                    string Route = "";

    
                        var rez = GetLocation(query);
                        Country = rez.Country.Replace("'", "").Trim();
                        City = rez.City.Replace("'", "").Trim();
                        Area = rez.Area.Replace("'", "").Trim();
                        Distric = rez.Distric.Replace("'", "").Trim();
                        Route = rez.Route.Replace("'", "").Trim();
                    


                    if (Distric=="Irbid Qasabah District" && Area=="")
                    {
                        Distric=Route;
                    }



                    try
                    {


                        decimal Longitude = decimal.Parse(query.Split(",")[0]);
                        decimal Latitude = decimal.Parse(query.Split(",")[1]);


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
                            command.Parameters.AddWithValue("@TenantId", tenantID);

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

                        return infoLocation.Address +" _______:"+"لا ندعم التوصيل للمنطقة ";


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

                                var areaname = GetAreas(tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

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
                                infoLocation.LocationAreaName=areaname.AreaName;




                                if (!areaname.IsAvailableBranch)
                                {


                                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                    infoLocation.DeliveryCostAfter = -1;
                                    infoLocation.DeliveryCostBefor = -1;
                                    infoLocation.LocationId = 0;

                                  return infoLocation.Address +" _______:"+"لا ندعم التوصيل للمنطقة ";


                                }

                                OrderOfferFun(tenantID, false, 0, infoLocation, cityName, areaName, districName, cost);


                                if (areaname != null)
                                {
                                    if (true)
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



                                return  "قيمة التوصيل :"+infoLocation.DeliveryCostAfter;
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

                              return infoLocation.Address +" _______:"+"لا ندعم التوصيل للمنطقة ";


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

                       return infoLocation.Address +" _______:"+"لا ندعم التوصيل للمنطقة ";

                    }
                }
                else
                {
                    GetLocationInfoModel infoLocation = new GetLocationInfoModel();
                    infoLocation.Country = "";
                    infoLocation.City = "";
                    infoLocation.Area = "";
                    infoLocation.Distric = "";
                    infoLocation.Address = "";
                    infoLocation.DeliveryCostAfter = -1;
                    infoLocation.DeliveryCostBefor = -1;
                    infoLocation.LocationId = 0;

                   return infoLocation.Address +" _______:"+"لا ندعم التوصيل للمنطقة ";
                }
          
        }


        [Route("BulkInser")]
        [HttpGet]
        public async Task BulkInser()
        {
            var campaigns = new List<CampaignCosmoDBModel>
            {
                    new CampaignCosmoDBModel
                    {
                        isSent = false,
                        tenantId = 277,
                        itemType = 5,
                        messagesId = "msg1",
                        campaignId = "camp1",
                        // other properties...
                    },
                    new CampaignCosmoDBModel
                    {
                        isSent = false,
                        tenantId = 277,
                        itemType = 5,
                        messagesId = "msg2",
                        campaignId = "camp2",
                        // other properties...
                    },
               // Add more CampaignCosmoDBModel instances as needed
           };

            var itemsCollection = new DocumentCosmoseDB<CampaignCosmoDBModel>(
                CollectionTypes.ItemsCollection,
                _IDocumentClient // Assume this is initialized elsewhere
            );

            var xx = new CampaignCosmoDBModel
            {
                isSent = false,
                tenantId = 33,
                itemType = 5,
                messagesId = "msg1",
                campaignId = "camp1",
                // other properties...
            };
            await itemsCollection.BulkInsertDocumentsAsync(xx);



        }

        [Route("DeleteCosmoseDB")]
        [HttpGet]
        public  async Task DeleteCosmoseDB(int TenantId, int item)
        {
            //var contact = await _contactRepository.FirstOrDefaultAsync((int)input);

            var itemsCollection = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            // delete contact caht 
            var queryString = "SELECT * From c WHERE c.itemType ="+item+" and c.tenantId="+TenantId;
            await itemsCollection.DeleteChatItem(queryString);


        }
        private static List<TenantModel> GetTenantList()
        {

            //var x = GetContactId("962779746365", "28");


            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where IsDeleted = 0 and D360Key IS NOT NULL AND D360Key <> ''";


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
                    D360Key = dataSet.Tables[0].Rows[i]["D360Key"].ToString()
                });

            }

            conn.Close();
            da.Dispose();

            return order;

        }

        [Route("GetLinkMenu")]
        [HttpGet]
        public async Task<string> GetLinkMenuAsync(int ContactID,int TenantId, string SelectedAreaId)
        {

            string accessToken = "EAAFLXHZBVtNQBO144RZAMJcJeBNm4ZBbUT5Q8GB5uLiEzlHq51ZAhAFKh5r4ctvqkZBZAZC3g4r4sS10b9WZBvHMf2DC1nO7q3azZBAZAZBP8mdEtjVgI71D0ZCrYf9ZBCXn6cYvfnfqQnnZCqOyuL3Bal8QwwLanzCZBz77U0zWyyqqqNKHj2fTLCqZBKEC8ZCxlt4mKxMNp";
            string url = "https://graph.facebook.com/v17.0/239558949250384/messages";

            var recipients = new string[] { "962779746365", "962797564443" };
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            foreach (var recipient in recipients)
            {
                var data = new
                {
                    messaging_product = "whatsapp",
                    to = recipient,
                    type = "template",
                    template = new
                    {
                        name = "var",
                        language = new { code = "en" },
                        components = new[] { new { type = "BODY" } }
                    }
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }
            //var caption = GetAllCaption(TenantId, "ar");
            //var text = caption.Where(x => x.TextResourceId==2 && x.LanguageBotId==1).FirstOrDefault().Text;
            ////  var text = "الرجاء الضغط على الرابط\r\n\r\n👇👇👇👇👇👇\r\n *https://infoseedordersystem-stg.azurewebsites.net/Index1?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}*\r\n👆👆👆👆👆👆";//2

            //if (true)//(model.customerModel.CustomerStepModel.OrderTypeId=="Pickup")
            //{
            //    text=text.Replace("{3}", SelectedAreaId).Replace("{6}", "0");
            //}
            //else
            //{
            //    text=text.Replace("{3}", SelectedAreaId).Replace("{6}", "1");
            //}

            //text=text.Replace("{0}", TenantId.ToString()).Replace("{1}", ContactID.ToString()).Replace("{2}", "").Replace("{4}", "1").Replace("{5}", "ar");
            //MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel
            //{
            //    ContactID=ContactID,

            //    Value=text
            //};
            //var link = AddMenuContcatKey(menuContcatKeyModel);

            return "";
        }

        [Route("GetLocationFromMap")]
        [HttpPost]
        public MapModel GetLocationFromMap(MapLocationModel mapLocationModel)
        {
            MapModel locationAddressModel = new MapModel();

            try
            {
                if (IsvalidLatLong(mapLocationModel.latitude.ToString(), mapLocationModel.longitude.ToString()))
                {
                    var rez = GetLocation(mapLocationModel.latitude.ToString()+","+mapLocationModel.longitude.ToString());



                    var Country = rez.Country.Replace("'", "").Trim();
                    var City = rez.City.Replace("'", "").Trim();
                    var Area = rez.Area.Replace("'", "").Trim();
                    var Distric = rez.Distric.Replace("'", "").Trim();
                    var Route = rez.Route.Replace("'", "").Trim();


                    locationAddressModel.Country = Country;
                    locationAddressModel.City = City;
                    locationAddressModel.Area = Area;
                    locationAddressModel.Distric = Distric;
                    locationAddressModel.Route = Route;
                    locationAddressModel.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;



                    locationAddressModel.AddressAR = Translate(locationAddressModel.Address);//translat text 



                    return locationAddressModel;
                }
                else
                {
                    return locationAddressModel;
                }
            }
            catch (Exception)
            {

                return locationAddressModel;
            }
        }



        //https://admin.mlapi.ai/api_key/5398
        //https://identify.plantnet.org/
        [Route("TestImage")]
        [HttpGet]
        public async Task<string> TestImage(string link)
        {
            GetBase64FromImageUrl("");
            var time = "12:00am-12:30pm";

            var timeS =DateTime.Parse(time.Split("-")[0]);
            var timeE = DateTime.Parse(time.Split("-")[1]);


            var x = timeS-timeE;

            return x.TotalMinutes.ToString();



            string apiKey = "4r6DS06VLYKQ4jJCdMjsEePYmVD1q0EXpnT0Pk0kGXToLyi1V9";
            string imageUrl = link;

            // Prepare the HTTP client
            HttpClient client = new HttpClient();

            // Build the request URL
            string requestUrl = $"https://api.plant.id/v2/identify";

            // Prepare the request body
            string imageBase64 = await GetBase64FromImageUrl(imageUrl);
            string requestBody = $"{{\"images\":[\"{imageBase64}\"]}}";
            client.DefaultRequestHeaders.Add("Api-Key", apiKey);

            // Make the HTTP POST request
            HttpResponseMessage response = await client.PostAsync(requestUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"));

            // Read the response
            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse the response and extract the plant name
            string plantName = ParsePlantNameFromResponse(responseContent);

            // Display the result
            Console.WriteLine($"The plant in the image is: {plantName}");



            //string apiKey = "9dbba256cf63421bb3fb459ef4e82992";
            //string imageUrl = link;

            //// Prepare the HTTP client
            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            //// Build the request URL
            //string requestUrl = "https://testservese.cognitiveservices.azure.com/vision/v3.0/analyze?visualFeatures=Description";

            //// Prepare the request body as JSON
            //string requestBody = $"{{\"url\":\"{imageUrl}\"}}";
            //HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            //// Make the HTTP POST request
            //HttpResponseMessage response = await client.PostAsync(requestUrl, content);

            //// Read the response
            //string responseContent = await response.Content.ReadAsStringAsync();

            //// Parse the response and extract the plant name
            //string plantName = ParsePlantNameFromResponse(responseContent);


            //// Display the result
            //Console.WriteLine($"The plant in the image is: {plantName}");
            return $"The plant in the image is: {plantName}";

        }
        static async Task<string> GetBase64FromImageUrl(string imageUrl)
        {
            //###########Send Payment###########

            string token = "rLtt6JWvbUHDDhsZnfpAhpYk4dxYDQkbcPTyGaKp2TYqQgG7FGZ5Th_WD53Oq8Ebz6A53njUoo1w3pjU1D4vs_ZMqFiz_j0urb_BH9Oq9VZoKFoJEDAbRZepGcQanImyYrry7Kt6MnMdgfG5jn4HngWoRdKduNNyP4kzcp3mRv7x00ahkm9LAK7ZRieg7k1PDAnBIOG3EyVSJ5kK4WLMvYr7sCwHbHcu4A5WwelxYK0GMJy37bNAarSJDFQsJ2ZvJjvMDmfWwDVFEVe_5tOomfVNt6bOg9mexbGjMrnHBnKnZR1vQbBtQieDlQepzTZMuQrSuKn-t5XZM7V6fCW7oP-uXGX-sMOajeX65JOf6XVpk29DP6ro8WTAflCDANC193yof8-f5_EYY-3hXhJj7RBXmizDpneEQDSaSz5sFk0sV5qPcARJ9zGG73vuGFyenjPPmtDtXtpx35A-BVcOSBYVIWe9kndG3nclfefjKEuZ3m4jL9Gg1h2JBvmXSMYiZtp9MR5I6pvbvylU_PP5xJFSjVTIz7IQSjcVGO41npnwIxRXNRxFOdIUHn0tjQ-7LwvEcTXyPsHXcMD8WtgBh-wxR8aKX7WPSsT1O8d8reb2aR7K3rkV3K82K_0OgawImEpwSvp9MNKynEAJQS6ZHe_J_l77652xwPNxMRTMASk1ZsJL"; //token value to be placed here;
            string baseURL = "https://apitest.myfatoorah.com";
            string url = baseURL + "/v2/SendPayment"; 
            HttpClient client = new HttpClient();
            byte[] cred = Encoding.UTF8.GetBytes("Bearer " + token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string json = "{\n \"NotificationOption\":\"ALL\",\n \"CustomerName\": \"Ahmed\",\n\"DisplayCurrencyIso\": \"KWD\",\n \"MobileCountryCode\":\"+965\",\n \"CustomerMobile\": \"12345678\",\n \"CustomerEmail\": \"xx@yy.com\",\n \"InvoiceValue\": 100,\n \"CallBackUrl\": \"https://google.com\",\n \"ErrorUrl\": \"https://google.com\",\n \"Language\": \"en\",\n \"CustomerReference\" :\"ref 1\",\n \"CustomerCivilId\":12345678,\n \"UserDefinedField\": \"Custom field\",\n \"ExpireDate\": \"\",\n \"CustomerAddress\" :{\n \"Block\":\"\",\n \"Street\":\"\",\n \"HouseBuildingNo\":\"\",\n \"Address\":\"\",\n \"AddressInstructions\":\"\"\n },\n \"InvoiceItems\": [\n {\n \"ItemName\": \"Product 01\",\n \"Quantity\": 1,\n \"UnitPrice\": 100\n }\n ]\n}\n\n"; 
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage messge = client.PostAsync(url, content).Result;
            if (messge.IsSuccessStatusCode) {
                string result = messge.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                   //content.Response.WriteAsync("\n" + result + "\n");
            }else {

                string result = messge.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //context.Response.WriteAsync("\n" + result + "\n");
            }

            return "";
            //    using (HttpClient client = new HttpClient())
            //    {
            //        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
            //        string base64String = Convert.ToBase64String(imageBytes);
            //        return base64String;
            //    }
        }
        static string ParsePlantNameFromResponse(string responseContent)
        {

            var model   = JsonConvert.DeserializeObject<PlantsModel>(responseContent);
           
            // Implement your own parsing logic based on the response structure
            // The JSON response will contain information about the image, including a description
            // Extract the plant name from the description or any relevant fields

            // Example implementation:
            // JObject responseJson = JObject.Parse(responseContent);
            // string plantName = responseJson["description"]["tags"][0].ToString();
            // return plantName;

            // Replace the following line with your own implementation
            return model.suggestions.FirstOrDefault().plant_name;
        }


        [Route("TestSendAccount")]
        [HttpGet]
        public async Task<string> TestSendAccount(int TenantId,long UserId)
        {
            AccountLoginModel accountLoginModel = new AccountLoginModel()
            {
                IsLogOut=true,
                TenantId=TenantId,
                UserId=UserId


            };


            SocketIOManager.SendAccount(accountLoginModel, TenantId);
            return "";

        }


        [Route("HtmlToPlainText")]
        [HttpGet]
        public async Task<string> HtmlToPlainTextAsync(string html)
        {

            AccountLoginModel accountLoginModel = new AccountLoginModel()
            {
                IsLogOut=true,
                TenantId=27,
                UserId=22


            };

            SocketIOManager.SendAccount(accountLoginModel, AbpSession.TenantId.Value);


            //string apiUrl = "https://api.cloudconvert.com/v2";
            //string apiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiZDc5Mzk0MGQ2MTg0MTVmYzYxNTNmN2Y1YzNlYjZlNDE3YWU5NTFlNmU4NWRjYmEzN2ZiZWRmMzE1YmI2YzYwYzcxMjc1NTIyZGViMzM1YmEiLCJpYXQiOjE2ODU0NTU2MDkuNjg5NTYsIm5iZiI6MTY4NTQ1NTYwOS42ODk1NjIsImV4cCI6NDg0MTEyOTIwOS42ODEyNywic3ViIjoiMjI3NjMzMDEiLCJzY29wZXMiOlsidXNlci5yZWFkIiwidXNlci53cml0ZSIsInRhc2sucmVhZCIsInRhc2sud3JpdGUiLCJ3ZWJob29rLnJlYWQiLCJ3ZWJob29rLndyaXRlIiwicHJlc2V0LnJlYWQiLCJwcmVzZXQud3JpdGUiXX0.RxDf3jNPnUNkdqHMQKoznIDYneWxJkxLmLVzjZ_M3PdbELS4Ry6tCXJ__svDUiWpexV0ZHmw8woOhrYQcz9UlwLPiM3_9Z8gsMTBamijRBeGYo0MqSoh0eqYslz7Tlh7Cf-19FFz2k-FoOggWiKGLOhE10eSFhV5I8l3omdnvdpvfH-AHKViC0qxpGPeJuYoiVKZ1_O9_BS7qqYM3ERjW3IHgnWrxv_BzFdtQGJV-98s0bUYEuZepj6SYU9CaqD8lkjutNfNIcdVWCwKVKaGnBR5qdHUdS3JDMXt131FLIZrrfxi3uLyHtogv2Ggfd6zrKFqiasswrmWKdOfO_N6MqHYFp6ySoTPxBLI2Mu98TkyP80-6MzkJ3Nldu86tLhEtSwOyU7-8sUpgvEVILP6dXwnkpG9_8LXWEMs8uMdqA-eCEvk4Tg2hRRBgfup_ntFLm_TyObybMLXt86msfblS3SvaNCaIm909gy88SY0Pe_PEibfZksnT7P4LwxUwyR_K-_lNa0rMEhAzQ68Kb1rqr3g2I2iNUTnj1KEs60BsjWwjqjyc-vOR32h7mM1QrH2VoNmFhl5Uz-ltl5hcLr6GrW0s64Ye7agD6gCk99bUhMZiUwr8TKpBihobe_4sFcysRHiPP5X_7M1LFIrJpbV7HbMewvuptqjCT1A0XnZOwY";
            //using (var httpClient = new HttpClient())
            //{
            //    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

            //    var response = await httpClient.GetAsync(apiUrl);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseContent = await response.Content.ReadAsStringAsync();
            //        // Key authentication successful
            //        Console.WriteLine("API key is valid.");
            //        Console.WriteLine("Response: " + responseContent);
            //    }
            //    else
            //    {
            //        // Key authentication failed
            //        Console.WriteLine("API key is invalid.");
            //        Console.WriteLine("Status code: " + response.StatusCode);
            //    }
            //}


            string apiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiZDc5Mzk0MGQ2MTg0MTVmYzYxNTNmN2Y1YzNlYjZlNDE3YWU5NTFlNmU4NWRjYmEzN2ZiZWRmMzE1YmI2YzYwYzcxMjc1NTIyZGViMzM1YmEiLCJpYXQiOjE2ODU0NTU2MDkuNjg5NTYsIm5iZiI6MTY4NTQ1NTYwOS42ODk1NjIsImV4cCI6NDg0MTEyOTIwOS42ODEyNywic3ViIjoiMjI3NjMzMDEiLCJzY29wZXMiOlsidXNlci5yZWFkIiwidXNlci53cml0ZSIsInRhc2sucmVhZCIsInRhc2sud3JpdGUiLCJ3ZWJob29rLnJlYWQiLCJ3ZWJob29rLndyaXRlIiwicHJlc2V0LnJlYWQiLCJwcmVzZXQud3JpdGUiXX0.RxDf3jNPnUNkdqHMQKoznIDYneWxJkxLmLVzjZ_M3PdbELS4Ry6tCXJ__svDUiWpexV0ZHmw8woOhrYQcz9UlwLPiM3_9Z8gsMTBamijRBeGYo0MqSoh0eqYslz7Tlh7Cf-19FFz2k-FoOggWiKGLOhE10eSFhV5I8l3omdnvdpvfH-AHKViC0qxpGPeJuYoiVKZ1_O9_BS7qqYM3ERjW3IHgnWrxv_BzFdtQGJV-98s0bUYEuZepj6SYU9CaqD8lkjutNfNIcdVWCwKVKaGnBR5qdHUdS3JDMXt131FLIZrrfxi3uLyHtogv2Ggfd6zrKFqiasswrmWKdOfO_N6MqHYFp6ySoTPxBLI2Mu98TkyP80-6MzkJ3Nldu86tLhEtSwOyU7-8sUpgvEVILP6dXwnkpG9_8LXWEMs8uMdqA-eCEvk4Tg2hRRBgfup_ntFLm_TyObybMLXt86msfblS3SvaNCaIm909gy88SY0Pe_PEibfZksnT7P4LwxUwyR_K-_lNa0rMEhAzQ68Kb1rqr3g2I2iNUTnj1KEs60BsjWwjqjyc-vOR32h7mM1QrH2VoNmFhl5Uz-ltl5hcLr6GrW0s64Ye7agD6gCk99bUhMZiUwr8TKpBihobe_4sFcysRHiPP5X_7M1LFIrJpbV7HbMewvuptqjCT1A0XnZOwY";

            var client = new RestClient("https://api.cloudconvert.com");
            client.Authenticator = new HttpBasicAuthenticator("Authorization", "Bearer " + apiKey);

            var request = new RestRequest("v2/convert", Method.POST);
            request.AddParameter("input", "download");
            request.AddParameter("file", html);
            request.AddParameter("outputformat", "mp3");

            var response = client.Execute(request);
            var content = response.Content;

            //Parse the response to retrieve the MP3 URL
            // The response format depends on the API you are using

            return content; // Return the MP3 URL


            html=html.Replace("<strong><em>", "*_");
            html=html.Replace("</em></strong>", "_*");


            html=html.Replace("<strong>", "*");
            html=html.Replace("</strong>", "*");

            html=html.Replace("<em>", "_");
            html=html.Replace("</em>", "_");

            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
        #region Booking

        [Route("SendNot")]
        [HttpPost]
        public async Task<string> SendNot(int TenantId,int UserId , string massage,NotificationSeverity notificationSeverity)
        {
           
            var result = _iUserAppService.GetUsersBot(TenantId).Result;

            var userids = "";

            foreach (var user in result)
            {
                userids=userids+","+user.Id.ToString();
            }

            var listuser = GetAdminIds(result, TenantId, userids).ToList();
            listuser.Add(UserId);

            listuser=listuser.Distinct().ToList();
            foreach (var userID in listuser)
            {
                UserNotification Notification = await SendNotfAsync(massage, userID, TenantId, notificationSeverity);
            }
  

            return "ok";
        }

        [Route("GetUserByBranches")]
        [HttpPost]
        public async Task<List<string>> GetUserByBranches(int TenantId, string UserIds)
        {
            List<string> vs = new List<string>();
            var result = _iUserAppService.GetUsersBot(TenantId, UserIds).Result;


            return FillRoleNames(result, TenantId, UserIds);
        }

        [Route("GetUserModelByBranches")]
        [HttpPost]
        public async Task<UserListDto> GetUserModelByBranches(int TenantId, string UserName)
        {
            var result = _iUserAppService.GetUsersBot(TenantId, null).Result;

            var x = result.Where(x => x.Name.Trim()==UserName.Trim()).FirstOrDefault();

            return x;
        }

        [Route("CreateBooking")]
        [HttpPost]
        public async Task<string> CreateBooking(BookingModel booking)
        {
                var aaaa = JsonConvert.SerializeObject(booking);
            //var  = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);
            booking.IsNew=true;
            string result = await _bookingAppService.CreateBookingAsync(booking);
            if (result == "booking_success")
            {
                SendMobileNotification(booking.TenantId, "booking", "create booking" ,true, booking.UserId.ToString());

                var massage = "";

                if (booking.LanguageId == 1)
                {
                    massage = booking.BookingDateTimeString + " في " +booking.UserName+" حجز الى ";                  
                }
                else
                {
                    massage = "Create Booking To :" +booking.UserName +" in " +booking.BookingDateTimeString;
                }

                try
                {
                    await SendNot(booking.TenantId, booking.UserId, massage, NotificationSeverity.Error);
                }
                catch
                {

                }

               
            }

           return result;
        }

        [Route("CancelBooking")]
        [HttpPost]
        public async Task<string> CancelBooking(string bookingId)
        {
            BookingModel booking = _bookingAppService.GetBookingById(long.Parse(bookingId));

            booking.IsNew=true;
            booking.StatusId = (int)BookingStatusEnum.Canceled;
            string result = await _bookingAppService.UpdateBooking(booking);
            if (result == "update_success")
            {
                SendMobileNotification(booking.TenantId, "booking", "cancel booking", false, booking.UserId.ToString());


                try
                {
                    var massage = "";

                    if (booking.LanguageId == 1)
                    {
                        massage = booking.BookingDateTimeString + " في " +booking.UserName+"  تم الغاء الموعد";
                    }
                    else
                    {
                        massage = "The appointment has been Cancelled :" +booking.UserName +" in " +booking.BookingDateTimeString;
                    }


                    try
                    {
                        await SendNot(booking.TenantId, booking.UserId, massage, NotificationSeverity.Warn);
                    }
                    catch
                    {

                    }

                    
                }
                catch
                {

                }
               




                if (booking.LanguageId == 1)
                {
                    return "تم الغاء الموعد";
                }
                else
                {
                    return "The appointment has been Cancelled";
                }
            }
            else
            {
                if (booking.LanguageId == 1)
                {
                    return "حدث خطأ";
                }
                else
                {
                    return "Failed";
                }
            }
        }

        [Route("ConfirmBooking")]
        [HttpPost]
        public async Task<string> ConfirmBooking(string bookingId)
        {
            BookingModel booking = _bookingAppService.GetBookingById(long.Parse(bookingId));
            booking.IsNew=true;
            booking.StatusId = (int)BookingStatusEnum.Booked;
            string result = await _bookingAppService.UpdateBooking(booking);
            if (result == "update_success")
            {
                SendMobileNotification(booking.TenantId, "booking", "booked booking", true, booking.UserId.ToString());
                try
                {
                    var massage = "";

                    if (booking.LanguageId == 1)
                    {
                        massage = booking.BookingDateTimeString + " في " +booking.UserName+"  تم تأكيد الموعد" ;
                    }
                    else
                    {
                        massage = "The appointment has been Confirmed :" +booking.UserName +" in " +booking.BookingDateTimeString;
                    }

                    try
                    {
                        await SendNot(booking.TenantId, booking.UserId, massage, NotificationSeverity.Info);
                    }
                    catch
                    {

                    }

                    
                }
                catch
                {

                }

                if (booking.LanguageId == 1)
                {
                    return "تم تأكيد الموعد";
                }
                else
                {
                    return "The appointment has been Confirmed";
                }
            }
            else
            {
                if (booking.LanguageId == 1)
                {
                    return "حدث خطأ";
                }
                else
                {
                    return "Failed";
                }
            }
        }
        [Route("GetBookingDay")]
        [HttpGet]
        public Dictionary<string, string> GetBookingDay(long userId, int languageId)
        {
            var days = _bookingAppService.GetBookingDay(userId, languageId);
            return days;
        }


        [Route("GetBookingTime")]
        [HttpGet]
        public List<string> GetBookingTime(string date, int tenantId, long userId)
        {
            var times = _bookingAppService.GetBookingTime(date, tenantId, userId);
            return times;
        }


        [Route("GetContactBooking")]
        [HttpGet]
        public List<BookingModel> GetContactBooking(int contactId, int tenantId, int languageId)
        {
            return _bookingAppService.GetContactBooking(contactId, tenantId, languageId);
        }

        #endregion




        [Route("hostedCheckout")]
        [HttpPost]
        public async Task<string> hostedCheckout(ZohoModel model)
        {

            if (model.payment_result!=null)
            {
                if (model.payment_result.response_status=="A")
                {

                    bool IsPaidInvoice = true;
                    bool IsCaution = false;

                    var invoices = _invoices.GetInvoicesByInvoiceId(model.cart_description);
                    var Convertinvoices = JsonConvert.DeserializeObject<InvoicesModel>(invoices);

                    var zohoCID = Convertinvoices.invoices.FirstOrDefault().customer_id;

                    var ten = GetTenantByZohoCustomerId(zohoCID);
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var tenantCD = itemsCollection.GetItemAsync(p => p.TenantId == ten.Id && p.ItemType == InfoSeedContainerItemTypes.Tenant).Result;


                    if (Convertinvoices.invoices[0].invoice_number.Contains("INV-DEPOSIT-"))
                    {
                        _TenantDashboardAppService.CreateDepocit((int)tenantCD.TenantId , Convertinvoices.invoices[0].invoice_id, (decimal)Convertinvoices.invoices[0].total);
                    }

                    var allInvo = JsonConvert.DeserializeObject<InvoicesModel>(_invoices.GetInvoicesByCustomerId(zohoCID));
                    foreach (var item in allInvo.invoices)
                    {
                        addBilling(item, ten.Id);//add to DB
                    }

                    var invCAUTION = allInvo.invoices.Where(x => x.status== "sent").ToList(); //_invoices.GetInvoicesStatus(zohoCID, "sent");// status CAUTION                  
                    if (invCAUTION.Count()>0)
                    {
                        var InvBefore = invCAUTION.Where(x => ExtractNumber(x.due_days)<=ten.CautionDays).ToArray();
                        if (InvBefore.Length>0)
                        {
                          await  UpdateInvoicesHistory(zohoCID, "CAUTION", true, ten.D360Key);//on CAUTION
                            IsCaution=true;

                            tenantCD.IsCaution=true;
                        }
                        else
                        {
                            await UpdateInvoicesHistory(zohoCID, "CAUTION", false, ten.D360Key);//on CAUTION
                            tenantCD.IsCaution=false;
                        }

                    }
                    else
                    {
                        await UpdateInvoicesHistory(zohoCID, "CAUTION", false, ten.D360Key);//on CAUTION
                        tenantCD.IsCaution=false;
                    }

                    var invWARNING = allInvo.invoices.Where(x => x.status== "overdue").ToList(); // _invoices.GetInvoicesStatus(zohoCID, "overdue");// status WARNING
               
                    if (invWARNING.Count()>0)
                    {
                        var bef = invWARNING.Where(x => ExtractNumber(x.due_days)>=ten.WarningDays).ToArray();
                        if (bef.Length>0)
                        {
                            IsPaidInvoice=false;
                            tenantCD.IsPaidInvoice=false;
                            await UpdateInvoicesHistory(zohoCID, "WARNING", false, ten.D360Key);//on WARNING 
                        }
                        else
                        {
                            tenantCD.IsPaidInvoice=true;
                            tenantCD.IsCaution=true;
                            await UpdateInvoicesHistory(zohoCID, "WARNING", true, ten.D360Key);//on WARNING 
                        }

                    }
                    else
                    {
                        await UpdateInvoicesHistory(zohoCID, "WARNING", true, ten.D360Key);//on WARNING 
                        tenantCD.IsPaidInvoice=true;
                    }

                    await itemsCollection.UpdateItemAsync(tenantCD._self, tenantCD);
                    Convertinvoices.IsCaution=IsCaution;
                    Convertinvoices.IsPaidInvoice=IsPaidInvoice;
                    Convertinvoices.TenantId=ten.Id;

                    SocketIOManager.SendInvoices(Convertinvoices, ten.Id);
                }

            }




            return "https://waapi.info-seed.com/";
        }


        [Route("AddMenuContcatKey")]
        [HttpPost]
        public string AddMenuContcatKey(MenuContcatKeyModel model)
        {
            try
            {
                var cap = model.Value;
                var url = extractURL(model.Value);
                model.Value=url;
                model.KeyMenu= RandomString(10);
                _menusAppService.MenuContactKeyAdd(model);

                var ret = cap.Replace(url, url.Split("?")[0]+"?"+model.KeyMenu);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        private string extractURL(string myString)
        {
            // string myString = "test =) https://google.com/";
            Match url = Regex.Match(myString, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return url.ToString();
        }
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }

        [Route("CancelUtracOrder")]
        [HttpPost]
        public void CancelUtracOrder(CancelUtracOrderModel model)
        {


        }
        [Route("QNAInfoseed")]
        [HttpPost]
        public string QNAInfoseed(string question)
        {
            var client = new RestClient("https://qnasupportinfoseed.cognitiveservices.azure.com/language/:query-knowledgebases?projectName=infoseedA&api-version=2021-10-01&deploymentName=test");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Ocp-Apim-Subscription-Key", "fc14e8bb1aef4111917424d1b5f78632");
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"question\" :\""+question+"\"}";// @"{""question"":question}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string resStr = response.Content.ToString();

            var rez = JsonConvert.DeserializeObject<QNAModel>(resStr);

            return rez.answers.FirstOrDefault().answer;


        }
        private static IList<UserTokenModel> GetFuncation(int? tenantId, string userIds = null)
        {

            string spName = "[dbo].[UserTokenGet]";
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@TenantId",tenantId),
                new SqlParameter("@UserIds",userIds)
            };


            IList<UserTokenModel> result = SqlDataHelper.ExecuteReader(spName, sqlParameters.ToArray(), MapUserToken, AppSettingsModel.ConnectionStrings);
            return result;
        }
        private static UserTokenModel MapUserToken(IDataReader dataReader)
        {
            UserTokenModel model = new UserTokenModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                DeviceId = SqlDataHelper.GetValue<string>(dataReader, "DeviceId"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                Token = SqlDataHelper.GetValue<string>(dataReader, "Token"),
                UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId")


            };
            return model;
        }

        [Route("hasantest")]
        [HttpPost]
        public async Task hasantestAsync(int? TenaentId, string title, string msg)
        {

            List<string> tokens = GetFuncation(TenaentId).Select(u => u.Token).ToList();
            foreach (var token in tokens)
            {
                var payload = new
                {
                    message = new
                    {
                        token = token,
                        notification = new
                        {
                            title = title,
                            body = msg
                        },
                        android = new
                        {
                            priority = "high",
                            notification = new
                            {
                                channel_id = "high_importance_channel_cancel",
                                sound = "sound.caf"
                            }
                        },
                        apns = new
                        {
                            headers = new
                            {
                                apns_priority = "10"
                            },
                            payload = new
                            {
                                aps = new
                                {
                                    sound = "sound.caf"
                                }
                            }
                        },
                        data = new
                        {
                            title = title,
                            body = msg,
                            sound = "sound.caf"
                        }
                    }
                };

                // Call your SendNotification method here with the current payload
                var Serializer = new JavaScriptSerializer();
                var json = Serializer.Serialize(payload);

                SendNotificationAsync(json);
            }
            //var payload = new
            //{
            //    messages = tokens.Select(token => new
            //    {
            //        token = token,
            //        notification = new
            //        {
            //            title = title,
            //            body = msg
            //        },
            //        android = new
            //        {
            //            priority = "high",
            //            notification = new
            //            {
            //                channel_id = "high_importance_channel_cancel",
            //                sound = "sound.caf"
            //            }
            //        },
            //        apns = new
            //        {
            //            headers = new
            //            {
            //                apns_priority = "10"
            //            },
            //            payload = new
            //            {
            //                aps = new
            //                {
            //                    sound = "sound.caf"
            //                }
            //            }
            //        },
            //        data = new
            //        {
            //            title = title,
            //            body = msg,
            //            sound = "sound.caf"
            //        }
            //    }).ToList()
            //};
          


            //var Serializer = new JavaScriptSerializer();
            //var json = Serializer.Serialize(payload);

            //SendNotificationAsync(json);
            // return Ok();

        }

        //done by hla for LabsBot

        //[Route("GetWorkingHour")]
        //[HttpGet]
        //public List<string> GetWorkingHour(string tenantID, int BranchId)
        //{

        //     var List= new List<string>();

        //    Area _Area = GetAreasID(BranchId);
        //    if (_Area != null) {
        //        if (_Area.SettingJson != null &&  _Area.SettingJson !="")
        //        {
        //            Web.Models.Sunshine.WorkModel workmodel = JsonConvert.DeserializeObject<Web.Models.Sunshine.WorkModel>(_Area.SettingJson);


        //            if (workmodel.WorkTextFri != "")
        //            {
        //                List.Add(workmodel.WorkTextFri);
        //            }
        //            if (workmodel.WorkTextSat != "")
        //            {
        //                List.Add(workmodel.WorkTextSat);
        //            }

        //            if (workmodel.WorkTextSun != "")
        //            {
        //                List.Add(workmodel.WorkTextSun);
        //            }

        //            if (workmodel.WorkTextMon != "")
        //            {
        //                List.Add(workmodel.WorkTextMon);
        //            }

        //            if (workmodel.WorkTextWed != "")
        //            {
        //                List.Add(workmodel.WorkTextWed);
        //            }
        //            if (workmodel.WorkTextThurs != "")
        //            {
        //                List.Add(workmodel.WorkTextThurs);
        //            }



        //        }
        //        else
        //        {
        //            List.Add("notFound");

        //        }

        //    }

        //    return List;

        //}

        [Route("GetBotReservedWords")]
        [HttpPost]
        public string GetBotReservedWords(int tenantID,string text)
        {
            var rez = "";
            try
            {
                var listmode = _templateAppService.GetBotReservedWords(0, 20, tenantID);
                foreach (var key in listmode.lstBotReservedWordsModel)
                {
                    var listbut = key.ButtonText.Split(",");

                    if (listbut.Contains(text.Trim()))
                    {
                        rez= key.ActionEn;

                    }

                }
            }
            catch
            {


            }
           

            return rez;
        }

        [Route("hasantest2")]
        [HttpPost]
        public void hasantest2(string city)
        {
            double xx;
            getNearbyLocation2(27, 3243, 2324, "", out xx);
        }
        [Route("GetNearbyLocations")]
        [HttpGet]
        public GetLocationInfoTowModel GetNearbyLocations(int tenantID, string query, string city)
        {
            try
            {

                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    var citymodel = GetLocationInfoModelByName(city);
                    GetLocationInfoTowModel locationsPinned = new GetLocationInfoTowModel();
                    if (citymodel!=null)
                    {
                        float lata = (float)Convert.ToDouble(query.Split(",")[0]);
                        float longt = (float)Convert.ToDouble(query.Split(",")[1]);

                        var result = _generalAppService.GetNearbyLocations(tenantID, lata, longt, citymodel.Id);
                        if (result != null)
                        {
                            locationsPinned = result;
                        }
                        else
                        {

                            locationsPinned.DeliveryCostAfter=-1;
                        }

                    }
                    else
                    {

                        locationsPinned.DeliveryCostAfter=-1;
                    }
                    return locationsPinned;

                }
                else
                {
                    return new GetLocationInfoTowModel() { DeliveryCostAfter=-1 };
                }
            }
            catch
            {
                return new GetLocationInfoTowModel() { DeliveryCostAfter=-1 };
            }
        }







        private AreaDto getNearbyLocation2(int tenantID, double eLatitude, double eLongitude, string city, out double distance)
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

        private async Task SendNotificationAsync(string data)
        {
            string accessToken = await GetAccessTokenAsync(); // Get Bearer token
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/v1/projects/info-seed/messages:send");
            request.Headers.Add("Authorization", "Bearer "+accessToken);
            var content = new StringContent(data, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var x= await response.Content.ReadAsStringAsync();


           // SendNotificationAsync(byteArray);
        }
        private async Task<string> GetAccessTokenAsync()
        {
            GoogleCredential credential;
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Firebase", "info-seed-firebase-adminsdk-nwflp-9dcbb1b151.json");

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        private async Task SendNotificationAsync(Byte[] byteArray)
        {

            try
            {
                string accessToken = await GetAccessTokenAsync(); // Get Bearer token

                WebRequest request = WebRequest.Create("https://fcm.googleapis.com/v1/projects/info-seed/messages:send");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add($"Authorization: Bearer {accessToken}");

                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        // Handle response if needed
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; // Handle or log the error
            }

        }


        [Route("SendMobileNotificationTest")]
        [HttpPost]
        public void SendMobileNotificationTest(int? TenaentId, string title, string msg,int type)
        {
            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
            httpWebRequest.Method = "POST";

            //var registrationTokens = new List<string>() { "caxMXQDT2E21k3icQZ2jCh:APA91bFTXnPM5YqvUPTiQfnQYxznV-c_uPQx_LoxZbklDf1cbq2IY5sloxF09gKVPMmetVhJBJBvu-ksWofxCnsDiaZz61pS-3_U4RvC5YS6yo-xXeAJPLwmlFGmv2lHLYERAIETazD2" };
            var payload = new
            {
                registration_ids = GetUserByTeneantId((int)TenaentId, "100,76"),
                data = new
                {
                    type= type,
                    body = msg,
                    title = title,
                    sound = "sound"
                },
                priority = "high",
                payload = new
                {
                    aps = new
                    {
                        sound = "sound"
                    }
                },
                android = new
                {
                    notification = new
                    {
                        channel_id = "high_importance_channel"
                    }
                },
                aps = new
                {
                    alert = title + " , " + msg,
                    sound = "sound.caf"
                }
            };

            //var payload2 = new
            //{

            //    to = GetUserByTeneantId((int)TenaentId), // iphone 6s test token
            //    data = new
            //    {
            //        body = "test",
            //        title = "test",
            //        pushtype = "events",



            //    },
            //    notification = new
            //    {
            //        body = "test",
            //        content_available = true,
            //        priority = "high",
            //        title = "C#"
            //    }
            //};

            //var payload3 = new
            //{
            //    registration_ids = GetUserByTeneantId((int)TenaentId),
            //    data = new
            //    {

            //        body = msg,
            //        title = title,
            //        sound = "sound"
            //    },
            //    priority = "high",
            //    payload = new
            //    {
            //        aps = new
            //        {
            //            sound = "sound"
            //        }
            //    },
            //    android = new
            //    {
            //        notification = new
            //        {
            //            channel_id = "high_importance_channel"
            //        }
            //    },
            //    aps = new
            //    {
            //        notification = new
            //        {
            //            body = msg,
            //            title = title,
            //        },

            //        alert = title + " , " + msg,
            //        sound = "sound.caf"
            //    },



            //    displayName = "Infoseed",
            //    state = "RUNNING",
            //    onStart = new { },
            //    notificationOptions = new
            //    {
            //        messageText = "Infoseed",
            //        messageTitle = "Infoseed",
            //        hasSound = false,
            //        imageUrl = "",
            //        registrationIds = GetUserByTeneantId((int)TenaentId),
            //    },
            //    dataBundle = new { }

            //};
            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

        }

        [Route("GetAssetByName")]
        [HttpPost]
        public List<GetListPDFModel> GetAssetByName(GetAssetByName model)
        {
            var lstLevels = _iAssetAppService.LoadLevels(model.TenantID);
            List<GetAssetModel> result = new List<GetAssetModel>();


            var levelOne = new AssetLevelOneDto();
            var levelTwo = new AssetLevelTwoDto();
            var levelThree = new AssetLevelThreeDto();


            if (model.Local.Equals("ar"))
            {
                levelOne = lstLevels.lstAssetLevelOneDto.Where(x => model.Brand.Contains(x.LevelOneNameAr)).FirstOrDefault();
                levelTwo = lstLevels.lstAssetLevelTwoDto.Where(x => model.Brand2.Contains(x.LevelTwoNameAr) && x.LevelOneId == levelOne.Id).FirstOrDefault();
                levelThree = lstLevels.lstAssetLevelThreeDto.Where(x => x.LevelThreeNameAr.Contains("عروض") && x.LevelTwoId == levelTwo.Id).FirstOrDefault();

            }
            else
            {

                levelOne = lstLevels.lstAssetLevelOneDto.Where(x => model.Brand.Contains(x.LevelOneNameEn)).FirstOrDefault();
                levelTwo = lstLevels.lstAssetLevelTwoDto.Where(x => model.Brand2.Contains(x.LevelTwoNameEn) && x.LevelOneId == levelOne.Id).FirstOrDefault();
                levelThree = lstLevels.lstAssetLevelThreeDto.Where(x => x.LevelThreeNameEn.Contains("Offer") && x.LevelTwoId == levelTwo.Id).FirstOrDefault();

            }

            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
            List<AssetDto> lstAsset = new List<AssetDto>();

            if (levelOne != null && levelTwo != null && levelThree != null)
            {
                lstAsset = _iAssetAppService.GetListOfAsset(model.TenantID.Value,
                    levelOne.Id,
                    levelTwo.Id,
                    0,
                   levelThree.Id,
                   0
                   , model.isOffer.Value);

            }
            else
            {
                lstAsset = null;

            }






            if (lstAsset != null)
                foreach (var item in lstAsset)
                {
                    if (item.lstAssetAttachmentDto != null)
                    {
                        foreach (var objitem in item.lstAssetAttachmentDto)
                        {

                            getListPDFModels.Add(new GetListPDFModel
                            {
                                AttachmentName = objitem.AttachmentName,
                                AttachmentType = objitem.AttachmentType,
                                AttachmentUrl = objitem.AttachmentUrl,
                                phoneNumber = model.PhoneNumber,
                                TenantID = model.TenantID.Value,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn,
                                IdOne = levelOne.Id,
                                IdTwo = levelTwo.Id


                            });

                            // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

                        }
                    }
                    else
                    {
                        getListPDFModels.Add(new GetListPDFModel
                        {
                            //AttachmentName = objitem.AttachmentName,
                            // AttachmentType = objitem.AttachmentType,
                            //AttachmentUrl = objitem.AttachmentUrl,
                            phoneNumber = model.PhoneNumber,
                            TenantID = model.TenantID.Value,
                            AssetDescriptionAr = item.AssetDescriptionAr,
                            AssetDescriptionEn = item.AssetDescriptionEn,
                            AssetNameAr = item.AssetNameAr,
                            AssetNameEn = item.AssetNameEn,
                            IdOne = levelOne.Id,
                            IdTwo = levelTwo.Id


                        });
                    }

                }








            return getListPDFModels;


        }

        [Route("UpdateCustomerBehavior")]
        [HttpPost]
        public void UpdateCustomerBehavior(CustomerBehaviourModel behaviourModel)
        {
            if (behaviourModel.Start)
            {
                behaviourModel.CustomerOPt = (int)CustomerBehaviourOptEnum.OptIn;
            }
            else if (behaviourModel.Stop)
            {
                behaviourModel.CustomerOPt = (int)CustomerBehaviourOptEnum.OptOut;
            }              
            _cusomerBehaviourAppService.UpdateCustomerBehavior(behaviourModel);
            UpdateCustomer(behaviourModel);

        }
        private void UpdateCustomer(CustomerBehaviourModel behaviourModel)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.ContactID == behaviourModel.ContactId.ToString()).Result;
            customerResult.CustomerOPT = behaviourModel.CustomerOPt;
            var Result = itemsCollection.UpdateItemAsync(customerResult._self, customerResult).Result;
        }

        [Route("CreateInterestedOf")]
        [HttpPost]
        public void CreateInterestedOf(CustomerInterestedOf interestedOf)
        {

            // var contactMG = _contactsAPI.GetContactsMg("962779746365").contacts.FirstOrDefault();


            _cusomerBehaviourAppService.CreateInterestedOf(interestedOf);

            try
            {

                if (interestedOf.TenantID == 59)
                {
                    CreateContactMg createContactMg = new CreateContactMg();
                    createContactMg.ContactId = interestedOf.ContactId;
                    createContactMg.levelTwoId = interestedOf.levelTwoId;
                    createContactMg.vid = "12312312";
                    SetMgMotorIntegrationInQueue(createContactMg);

                    //var con = GetContact(interestedOf.ContactId);

                    //var contactMGId = _contactsAPI.GetContactsMg(con.PhoneNumber).contacts.FirstOrDefault();

                    //CreateContactMg createContactMg = new CreateContactMg();

                    //createContactMg.ContactId=interestedOf.ContactId;
                    //createContactMg.levelTwoId=interestedOf.levelTwoId;

                    //var Interestedname = GetAssetLevelTwoName(interestedOf.levelTwoId);

                    //var liststring = contactMGId.properties.car__cloned__1.value;

                    //if (!liststring.ToLower().Contains(Interestedname.ToLower()))
                    //{


                    //    liststring=liststring+";"+Interestedname;
                    //}



                    //Property1 property1 = new Property1 { property="car__cloned__1", value=liststring };
                    //List<Property1> properties = new List<Property1>();

                    //properties.Add(property1);

                    //createContactMg.properties=properties.ToArray();
                    //createContactMg.vid=contactMGId.vid.ToString();
                    //SetMgMotorIntegrationInQueue(createContactMg);
                }


            }
            catch (Exception)
            {

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
            catch (Exception)
            {

                var Error = JsonConvert.SerializeObject(ContactMg);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }
        //[Route("CheckPhone")]

        //[HttpGet]
        //public bool CheckPhone(string phone)
        //{
        //   return _templateAppService.checkPhoneNumber(phone);
        //}

        [Route("GetCarModel")]
        [HttpGet]
        public List<string> GetCarModel(int? tenantID, int lvlOneId, int lvlTwoId)
        {

            List<string> vs = new List<string>();

            vs.Add("Hasan 1");
            vs.Add("Hasan 2");
            vs.Add("Hasan 3");
            vs.Add("Hasan 4");
            //var con = GetContact(ContactId);

            return vs;
        }

        [Route("GetCarAsset")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> GetCarAsset(int? TenantID)
        {


            List<GetListPDFModel> vs = new List<GetListPDFModel>();





            vs.Add(new GetListPDFModel
            {

                AttachmentName = "ss",
                AttachmentType = "image",
                AttachmentUrl = "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/4a767c02-1a89-4ad6-afd2-8be2dea42baa.jfif",
                phoneNumber = "962779746365",
                TenantID = 45
            });

            vs.Add(new GetListPDFModel
            {

                AttachmentName = "sd",
                AttachmentType = "image",
                AttachmentUrl = "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/4a767c02-1a89-4ad6-afd2-8be2dea42baa.jfif",
                phoneNumber = "962779746365",
                TenantID = 45
            });
            return vs;
        }

        [Route("UpdateNewLiveChat")]
        [HttpGet]
        public async Task<string> UpdateNewLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0)// DepartmentId
        {
            string SFormat = string.Empty;
            try
            {
                var Tenant = GetTenantById(TenantID).Result;

                var result = _dbService.UpdateLiveChat(TenantID + "_" + phoneNumber, 1, true).Result;

                result.LiveChatStatusName = "Pending";
                string type = Enum.GetName(typeof(TypeEnum), (int)TypeEnum.Ticket);
                var livechat = _iliveChat.AddNewLiveChat(TenantID, phoneNumber, TenantID + "_" + phoneNumber, result.displayName, 1, true, type, Department1, Department2, result.IsOpen, DepartmentId);

                if (Department1 != null)
                    result.Department = Department1 + "-" + Department2;

                if (Tenant.IsLiveChatWorkActive)
                {
                    if (!checkIsInServiceLiveChat(Tenant.LiveChatWorkingHours, out SFormat))
                    {
                        return SFormat;
                    }
                }
                if (result != null)
                {
                    try
                    {
                        var titl = "New Live Chat Request ";
                        var body = "From : " + result.displayName;

                        SendMobileNotification(TenantID.Value, titl, body, true, livechat.DepartmentUserIds);
                    }
                    catch (Exception){  }

                    SocketIOManager.SendLiveChat(livechat, TenantID.Value);
                }
                return SFormat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("UpdateLiveChat")]
        [HttpGet]
        public async Task<string> UpdateLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0)// DepartmentId
        {


            return await UpdateNewLiveChatAsync(TenantID, phoneNumber, Department1, Department2, DepartmentId);
            //string SFormat = string.Empty;
            //try
            //{
            //    var Tenant = GetTenantById(TenantID).Result;
            //    // var con = GetContact(contactId);
            //    var result = _dbService.UpdateLiveChat(TenantID + "_" + phoneNumber, 1, true).Result;

            //    result.LiveChatStatusName = "Pending";
            //    string type = Enum.GetName(typeof(TypeEnum), (int)TypeEnum.Ticket);
            //    var livechat = _iliveChat.AddNewLiveChat(TenantID, phoneNumber, TenantID + "_" + phoneNumber, result.displayName, 1, true, type, Department1, Department2, result.IsOpen, DepartmentId);

            //    if (Department1 != null)
            //        result.Department = Department1 + "-" + Department2;

            //    if (Tenant.IsLiveChatWorkActive)
            //    {

            //        if (!checkIsInServiceLiveChat(Tenant.LiveChatWorkingHours, out SFormat))
            //        {
            //            return SFormat;
            //        }

            //    }
            //    if (result != null)
            //    {
            //        //result.customerChat = null;
            //        try
            //        {
            //            var titl = "New Live Chat Request ";
            //            var body = "From : " + result.displayName;

            //            // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
            //            SendMobileNotification(TenantID.Value, titl, body, true, livechat.DepartmentUserIds);
            //        }
            //        catch (Exception)
            //        {

            //        }
            //        //await _LiveChatHubhub.Clients.All.SendAsync("brodCastBotLiveChat", result);
            //        SocketIOManager.SendLiveChat(livechat, TenantID.Value);
            //    }

            //    return SFormat;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    // return SFormat;
            //}
        }




        [Route("UpdateSaleOffer")]
        [HttpPost]
        public async Task UpdateSaleOfferAsync([FromBody] UpdateSaleOfferModel updateOrderModel)
        {
            // Audai Todo from Hassan 
            // FindFileByName(string fileName, string mimtype) 

            List<AttachmentBotAPIModel> AttachmetArray = new List<AttachmentBotAPIModel>();
            List<AttachmentBotAPIModel> AttachmetArrayTow = new List<AttachmentBotAPIModel>();
            if (updateOrderModel.AttachmetArray != null)
            {
                AttachmetArray = FillAttachmentsTicketData(updateOrderModel.AttachmetArray);

                // ticket.Attachments = Attachments;
            }
            if (updateOrderModel.AttachmetArrayTow != null)
            {

                AttachmetArrayTow = FillAttachmentsTicketData(updateOrderModel.AttachmetArrayTow);
                // ticket.Attachments = Attachments;
            }

            List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
            SellingRequestDto sellingRequestDto = await PreperSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto);

            var objSellingRequestDto = AddSellingRequest(sellingRequestDto);

            if (objSellingRequestDto.IsRequestForm)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                objSellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(sellingRequestDto.RequestDescription, options);
            }

            //result.customerChat = null;
            try
            {
                var titl = "New Selling Request ";
                var body = "From : " + updateOrderModel.ContactName;

                // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                SendMobileNotification(updateOrderModel.TenantID, titl, body);
            }
            catch (Exception)
            {

            }


            //await _sellingRequestHub.Clients.All.SendAsync("brodCastSellingRequest", objSellingRequestDto);

            SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);


        }
        [Route("SendPrescription")]
        [HttpPost]
        public async Task SendPrescription([FromBody] UpdateSaleOfferModel updateOrderModel , string JolInformation = null)
        {
          await  SendNewPrescription(updateOrderModel, JolInformation);

            //if (updateOrderModel.DepartmentId==null)
            //{
            //    updateOrderModel.DepartmentId=0;
            //}

            //if (string.IsNullOrEmpty(updateOrderModel.UserIds) || updateOrderModel.UserIds == "")
            //{
            //    updateOrderModel.UserIds = null;
            //}
            //if (updateOrderModel.AreaId == 0)
            //{
            //    updateOrderModel.AreaId = null;
            //}

            //List<AttachmentBotAPIModel> AttachmetArray = new List<AttachmentBotAPIModel>();
            //List<AttachmentBotAPIModel> AttachmetArrayTow = new List<AttachmentBotAPIModel>();
            //if (updateOrderModel.AttachmetArray != null)
            //{
            //    AttachmetArray = FillAttachmentsTicketData(updateOrderModel.AttachmetArray);

            //    // ticket.Attachments = Attachments;
            //}
            //if (updateOrderModel.AttachmetArrayTow != null)
            //{

            //    AttachmetArrayTow = FillAttachmentsTicketData(updateOrderModel.AttachmetArrayTow);
            //    // ticket.Attachments = Attachments;
            //}

            //List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
            //SellingRequestDto sellingRequestDto = await PreperSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto);
            //sellingRequestDto.AreaId = updateOrderModel.AreaId;
            //var objSellingRequestDto = AddSellingRequest(sellingRequestDto);
            ////var objSellingRequestDto = AddNewSellingRequest(sellingRequestDto);

            //if (objSellingRequestDto.IsRequestForm)
            //{
            //    var options = new JsonSerializerOptions { WriteIndented = true };

            //    objSellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(sellingRequestDto.RequestDescription, options);
            //}
            //objSellingRequestDto.AreaId = updateOrderModel.AreaId;
            ////await _sellingRequestHub.Clients.All.SendAsync("brodCastSellingRequest", objSellingRequestDto);
            //objSellingRequestDto.CreatedOn = objSellingRequestDto.CreatedOn.AddHours(AppSettingsModel.AddHour);
            //try
            //{
            //    var titl = "New Selling Request ";
            //    var body = "From : " + updateOrderModel.ContactName;

            //    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
            //    SendMobileNotification(updateOrderModel.TenantID, titl, body, false, objSellingRequestDto.DepartmentUserIds);
            //}
            //catch (Exception)
            //{

            //}
            //SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);

            //if (updateOrderModel.TenantID == 221)
            //{
            //    try
            //    {
            //        var client = new HttpClient();
            //        var request = new HttpRequestMessage(HttpMethod.Post, "https://loyalty.jppmc.jo/loyalty/public/api/user/send-mail?classification=" + JolInformation + "&name=" + updateOrderModel.ContactName + "&phone=" + updateOrderModel.PhoneNumber + "&information=" + updateOrderModel.information);
            //        request.Headers.Add("Authorization", "Bearer WhatsAppSendEmail");
            //        var response = await client.SendAsync(request);
            //        response.EnsureSuccessStatusCode();
            //        // Console.WriteLine(await response.Content.ReadAsStringAsync());
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
        }
        [Route("SendNewPrescription")]
        [HttpPost]
        public async Task SendNewPrescription([FromBody] UpdateSaleOfferModel updateOrderModel, string JolInformation = null)
        {
            if (updateOrderModel.DepartmentId == null)
            {
                updateOrderModel.DepartmentId = 0;
            }

            if (string.IsNullOrEmpty(updateOrderModel.UserIds) || updateOrderModel.UserIds == "")
            {
                updateOrderModel.UserIds = null;
            }
            if (updateOrderModel.AreaId == 0)
            {
                updateOrderModel.AreaId = null;
            }

            List<AttachmentBotAPIModel> AttachmetArray = new List<AttachmentBotAPIModel>();
            List<AttachmentBotAPIModel> AttachmetArrayTow = new List<AttachmentBotAPIModel>();
            if (updateOrderModel.AttachmetArray != null)
            {
                AttachmetArray = FillAttachmentsNewTicketData(updateOrderModel.AttachmetArray);

                // ticket.Attachments = Attachments;
            }
            if (updateOrderModel.AttachmetArrayTow != null)
            {

                AttachmetArrayTow = FillAttachmentsNewTicketData(updateOrderModel.AttachmetArrayTow);
                // ticket.Attachments = Attachments;
            }
            if (updateOrderModel.TenantID == 220 && JolInformation != null)
            {
                switch (JolInformation) 
                {
                    case "1":
                        updateOrderModel.information = "طلبات ديزل للمنازل والشركات" + "," + updateOrderModel.information;
                        break;
                    case "2":
                        updateOrderModel.information = "طلبات المحروقات / المحطات" + "," + updateOrderModel.information;
                        break;
                }
            }
                
            List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
            SellingRequestDto sellingRequestDto = await PreperNewSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto);
            sellingRequestDto.AreaId = updateOrderModel.AreaId;

            var objSellingRequestDto = AddNewSellingRequest(sellingRequestDto);

            if (objSellingRequestDto.IsRequestForm)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                objSellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(sellingRequestDto.RequestDescription, options);
            }
            objSellingRequestDto.AreaId = updateOrderModel.AreaId;
            //await _sellingRequestHub.Clients.All.SendAsync("brodCastSellingRequest", objSellingRequestDto);
            objSellingRequestDto.CreatedOn = objSellingRequestDto.CreatedOn.AddHours(AppSettingsModel.AddHour);
            try
            {
                var titl = "New Request";
                var body = "From : " + updateOrderModel.ContactName;

                // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                SendMobileNotification(updateOrderModel.TenantID, titl, body, false, objSellingRequestDto.DepartmentUserIds);
            }
            catch (Exception)
            {

            }
            //SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);

            if (updateOrderModel.TenantID == 220)
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://loyalty.jppmc.jo/loyalty/public/api/user/send-mail?classification=" + JolInformation + "&name=" + updateOrderModel.ContactName + "&phone=" + updateOrderModel.PhoneNumber + "&information=" + updateOrderModel.information);
                    request.Headers.Add("Authorization", "Bearer WhatsAppSendEmail");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    // Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [Route("CreateAppointmentMG")]
        [HttpPost]
        public async Task CreateAppointmentMG([FromBody] CreateAppointmentMGModel model)
        {


            CreateContactMg createContactMg = new CreateContactMg();

            createContactMg.createAppointmentMGModel=model;
            createContactMg.vid = "001100";
            SetMgMotorIntegrationInQueue(createContactMg);



        }
        private async Task<SellingRequestDto> PreperSellingRequestAsync(UpdateSaleOfferModel updateOrderModel, List<AttachmentBotAPIModel> AttachmetArray, List<AttachmentBotAPIModel> AttachmetArrayTow, List<SellingRequestDetailsDto> SellingRequestDetailsDto)
        {
            if (AttachmetArray != null)
            {
                var types = AppsettingsModel.AttacmentTypesAllowed;
                foreach (var item in AttachmetArray)
                {

                    //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                    //AttachmentContent attachmentContent = new AttachmentContent()
                    //{
                    //    Content = item.Base64,
                    //    Extension = Path.GetExtension(item.Filename),
                    //    MimeType = item.FileType,

                    //};

                    //string filepath = System.IO.Path.GetDirectoryName(item.Filename);
                    //var url = await azureBlobProvider.Save(attachmentContent);

                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
                    {
                        DocumentURL = item.Filename,
                        DocumentTypeId = 1,
                        TenantId = updateOrderModel.TenantID

                    });


                }

            }
            if (AttachmetArrayTow != null)
            {
                //var types = AppsettingsModel.AttacmentTypesAllowed;
                foreach (var item in AttachmetArrayTow)
                {

                    //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                    //AttachmentContent attachmentContent = new AttachmentContent()
                    //{
                    //    Content = item.Base64,
                    //    Extension = Path.GetExtension(item.Filename),
                    //    MimeType = item.FileType,

                    //};

                    //string filepath = System.IO.Path.GetDirectoryName(item.Filename);
                    //var url = await azureBlobProvider.Save(attachmentContent);

                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
                    {
                        DocumentURL = item.Filename,
                        DocumentTypeId = 2,
                        TenantId = updateOrderModel.TenantID

                    });


                }

            }
            SellingRequestDto sellingRequestDto = new SellingRequestDto();
            if (!updateOrderModel.IsRequestForm)
            {
                List<string> str = new List<string>();
                string newInformation = "";

                str.AddRange(updateOrderModel.information.Split(','));

                foreach (var item in str)
                {
                    // Trim leading and trailing spaces
                    string trimmedItem = item.Trim();
                    if (trimmedItem.Contains("Is_Location"))
                    {
                        trimmedItem = trimmedItem.Replace("Is_Location", " ");
                        trimmedItem = trimmedItem.Replace("الموقع", ""); 

                        var number1 = trimmedItem.Split(" ")[0];
                        var number2 = trimmedItem.Split(" ")[1];

                        if (double.TryParse(number1, out double parsedValue))
                        {
                            if (double.TryParse(number2, out double parsedValue2))
                            {
                                newInformation += "الموقع:https://maps.google.com/?q=" + trimmedItem + ",";
                            }
                            else
                            {
                                newInformation += "الموقع: " + trimmedItem + ",";
                            }
                        }
                        else
                        {
                            newInformation += "الموقع: " + trimmedItem + ",";
                        }
                    }
                    else
                    {
                        if (trimmedItem.Contains("الموقع"))
                        {
                            trimmedItem = trimmedItem.Replace("الموقع", "الموقع: ");
                            newInformation += trimmedItem + ",";
                        }
                        else
                        {
                            newInformation += trimmedItem + ",";
                        }
                    }
                }
                // Remove the trailing comma, if any
                if (newInformation.EndsWith(","))
                {
                    newInformation = newInformation.Substring(0, newInformation.Length - 1);
                }

                sellingRequestDto = new SellingRequestDto()
                {

                    TenantId = updateOrderModel.TenantID,
                    ContactId = updateOrderModel.ContactId,
                    ContactInfo = updateOrderModel.ContactInformation,
                    PhoneNumber = updateOrderModel.PhoneNumber,
                    CreatedOn = DateTime.UtcNow,
                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
                    SellingStatusId = 1,
                    RequestDescription = newInformation,
                    Price = updateOrderModel.Price,
                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
                    CreatedBy = updateOrderModel.ContactName,
                    IsRequestForm = updateOrderModel.IsRequestForm,
                    UserIds = updateOrderModel.UserIds,
                    DepartmentId= updateOrderModel.DepartmentId,
                };


            }
            else
            {
                sellingRequestDto = new SellingRequestDto()
                {
                    TenantId = updateOrderModel.TenantID,
                    ContactId = updateOrderModel.ContactId,
                    ContactInfo = updateOrderModel.ContactInformation,
                    PhoneNumber = updateOrderModel.PhoneNumber,
                    CreatedOn = DateTime.UtcNow,
                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
                    SellingStatusId = 1,
                    RequestDescription = JsonConvert.SerializeObject(updateOrderModel.RequestForm),
                    Price = updateOrderModel.Price,
                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
                    CreatedBy = updateOrderModel.ContactName,
                    IsRequestForm = updateOrderModel.IsRequestForm,
                    UserIds = updateOrderModel.UserIds,
                    DepartmentId= updateOrderModel.DepartmentId,
                };

            }


            return sellingRequestDto;
        }

        private async Task<SellingRequestDto> PreperNewSellingRequestAsync(UpdateSaleOfferModel updateOrderModel, List<AttachmentBotAPIModel> AttachmetArray, List<AttachmentBotAPIModel> AttachmetArrayTow, List<SellingRequestDetailsDto> SellingRequestDetailsDto)
        {
            if (AttachmetArray != null)
            {
                var types = AppsettingsModel.AttacmentTypesAllowed;
                foreach (var item in AttachmetArray)
                {
                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
                    {
                        DocumentURL = item.Filename,
                        DocumentTypeId = 1,
                        TenantId = updateOrderModel.TenantID

                    });
                }
            }
            if (AttachmetArrayTow != null)
            {
                //var types = AppsettingsModel.AttacmentTypesAllowed;
                foreach (var item in AttachmetArrayTow)
                {
                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
                    {
                        DocumentURL = item.Filename,
                        DocumentTypeId = 2,
                        TenantId = updateOrderModel.TenantID

                    });
                }
            }
            SellingRequestDto sellingRequestDto = new SellingRequestDto();
            if (!updateOrderModel.IsRequestForm)
            {
                List<string> str = new List<string>();
                string newInformation = "";

                str.AddRange(updateOrderModel.information.Split(','));

                foreach (var item in str)
                {
                    // Trim leading and trailing spaces
                    string trimmedItem = item.Trim();
                    if (trimmedItem.Contains("Is_Location"))
                    {
                        trimmedItem = trimmedItem.Replace("Is_Location", " ");
                        trimmedItem = trimmedItem.Replace("الموقع", "");

                        var number1 = trimmedItem.Split(" ")[0];
                        var number2 = trimmedItem.Split(" ")[1];

                        if (double.TryParse(number1, out double parsedValue))
                        {
                            if (double.TryParse(number2, out double parsedValue2))
                            {
                                newInformation += "الموقع:https://maps.google.com/?q=" + trimmedItem + ",";
                            }
                            else
                            {
                                newInformation += "الموقع: " + trimmedItem + ",";
                            }
                        }
                        else
                        {
                            newInformation += "الموقع: " + trimmedItem + ",";
                        }
                    }
                    else
                    {
                        if (trimmedItem.Contains("الموقع"))
                        {
                            trimmedItem = trimmedItem.Replace("الموقع", "الموقع: ");
                            newInformation += trimmedItem + ",";
                        }
                        else
                        {
                            newInformation += trimmedItem + ",";
                        }
                    }
                }
                // Remove the trailing comma, if any
                if (newInformation.EndsWith(","))
                {
                    newInformation = newInformation.Substring(0, newInformation.Length - 1);
                }

                sellingRequestDto = new SellingRequestDto()
                {

                    TenantId = updateOrderModel.TenantID,
                    ContactId = updateOrderModel.ContactId,
                    ContactInfo = updateOrderModel.ContactInformation,
                    PhoneNumber = updateOrderModel.PhoneNumber,
                    CreatedOn = DateTime.UtcNow,
                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
                    SellingStatusId = 1,
                    RequestDescription = newInformation,
                    Price = updateOrderModel.Price,
                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
                    CreatedBy = updateOrderModel.ContactName,
                    IsRequestForm = updateOrderModel.IsRequestForm,
                    UserIds = updateOrderModel.UserIds,
                    DepartmentId = updateOrderModel.DepartmentId,
                };
            }
            else
            {
                sellingRequestDto = new SellingRequestDto()
                {
                    TenantId = updateOrderModel.TenantID,
                    ContactId = updateOrderModel.ContactId,
                    ContactInfo = updateOrderModel.ContactInformation,
                    PhoneNumber = updateOrderModel.PhoneNumber,
                    CreatedOn = DateTime.UtcNow,
                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
                    SellingStatusId = 1,
                    RequestDescription = JsonConvert.SerializeObject(updateOrderModel.RequestForm),
                    Price = updateOrderModel.Price,
                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
                    CreatedBy = updateOrderModel.ContactName,
                    IsRequestForm = updateOrderModel.IsRequestForm,
                    UserIds = updateOrderModel.UserIds,
                    DepartmentId = updateOrderModel.DepartmentId,
                };
            }

            return sellingRequestDto;
        }
        [Route("GetAllCaption")]
        [HttpGet]
        public List<CaptionDto> GetAllCaption(int TenantID, string local)
        {
            int localID = 1;
            if (local == "en")
            {
                localID = 2;

            }
            else
            {
                localID = 1;
            }

            try
            {

                var cap = _captionBotAppService.GetCaption(TenantID, localID);
                return cap;
                //string connString = AppSettingsModel.ConnectionStrings;
                //string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + localID;


                //SqlConnection conn = new SqlConnection(connString);
                //SqlCommand cmd = new SqlCommand(query, conn);
                //conn.Open();

                //// create the DataSet 
                //DataSet dataSet = new DataSet();

                //// create data adapter
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //// this will query your database and return the result to your datatable
                //da.Fill(dataSet);

                //List<Caption> captions = new List<Caption>();

                //for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                //{

                //    captions.Add(new Caption
                //    {
                //        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                //        Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
                //        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                //        LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
                //        TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

                //    });
                //}

                //conn.Close();
                //da.Dispose();

                //return cap;

            }
            catch
            {
                return new List<CaptionDto>();

            }

        }



        [Route("UpdateCancelOrder")]
        [HttpGet]
        public CancelOrderModel UpdateCancelOrder(int? TenantID, string OrderNumber, int ContactId, string CanatCancelOrderText)
        {
            CancelOrderModel cancelOrderModel = new CancelOrderModel();

            try
            {


                var OrderModel = GetOrderListWithContact(TenantID, ContactId, OrderNumber).Where(x => x.OrderNumber == long.Parse(OrderNumber)).FirstOrDefault();

                if (OrderModel != null)
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID).Result;
                    if (tenant.IsCancelOrder)
                    {

                        TimeSpan timeSpan = DateTime.Now.AddHours(AppSettingsModel.AddHour) - OrderModel.CreationTime;
                        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);

                        if (totalMinutes >= tenant.CancelTime)
                        {

                            cancelOrderModel.CancelOrder = true;
                            cancelOrderModel.WrongOrder = false;
                            cancelOrderModel.IsTrueOrder = true;
                            cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                            return cancelOrderModel;

                        }
                    }
                    if (!_iOrdersAppService.CreateOrderStatusHistory(OrderModel.Id, (int)OrderStatusEunm.Canceled, TenantID))
                    {
                        cancelOrderModel.CancelOrder = true;
                        cancelOrderModel.WrongOrder = false;
                        cancelOrderModel.IsTrueOrder = true;
                        cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                        return cancelOrderModel;
                    }

                   

                    UpdateOrderAfterCancel(OrderNumber, ContactId, OrderModel, TenantID);
                    cancelOrderModel.CancelOrder = false;
                    cancelOrderModel.WrongOrder = false;
                    cancelOrderModel.IsTrueOrder = true;


                    _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                    {
                        TenantId = TenantID.Value,
                        TypeId = (int)DashboardTypeEnum.Order,
                        StatusId = (int)OrderStatusEunm.Canceled,
                        StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Canceled)
                    }) ;



                    decimal total = 0;
                    if (OrderModel.Total > 0)
                    {
                        decimal DeliveryCost = OrderModel.DeliveryCost.HasValue ? OrderModel.DeliveryCost.Value : 0;
                        total = (decimal)(OrderModel.Total - DeliveryCost);
                    }
                    else
                    {
                        total = 0;
                    }
                    if (total < 0)
                    {
                        total = 0;
                    }



                    ///////////////////////////////////////////////

                    var modelorder = GetOrder(OrderModel.Id);

                    if (modelorder.TotalPoints > 0 || modelorder.TotalCreditPoints > 0)
                    {
                        _loyaltyAppService.UpdateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                        {
                            OrderId = OrderModel.Id,
                            ContactId = ContactId,
                            CreatedBy = ContactId,
                            TransactionTypeId = (int)LoyaltyTransactionType.DeleteCancelOrder,
                        });

                    }

                    ////////////////////////////////////////////



                    //var Loyaltymodel = _loyaltyAppService.GetAll(TenantID);
                    //var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(total, TenantID);
                    //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    //var DateStart = Convert.ToDateTime(Loyaltymodel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    //var DateEnd = Convert.ToDateTime(Loyaltymodel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));


                    //if (Loyaltymodel.IsLoyalityPoint && Loyaltymodel.OrderType.Contains(((int)OrderModel.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                    //{

                    //        ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                    //        contactLoyalty.ContactId = ContactId;
                    //        contactLoyalty.LoyaltyDefinitionId = Loyaltymodel.Id;
                    //        contactLoyalty.OrderId = OrderModel.Id;
                    //        contactLoyalty.CreatedBy = ContactId;
                    //        contactLoyalty.Points = OrderModel.TotalPoints;
                    //        contactLoyalty.CardPoints = CardPoints;
                    //        contactLoyalty.TransactionTypeId = 2;
                    //        _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); // point

                    //        //contactLoyalty.Points =-updateOrderModel.loyalityPoint.Value;
                    //        // _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); //subtract points                      
                    //}


                    return cancelOrderModel;


                }
                else
                {

                    cancelOrderModel.CancelOrder = false;
                    cancelOrderModel.WrongOrder = true;
                    cancelOrderModel.IsTrueOrder = false;
                    return cancelOrderModel;

                }
            }
            catch
            {

                cancelOrderModel.CancelOrder = false;
                cancelOrderModel.WrongOrder = true;
                cancelOrderModel.IsTrueOrder = false;
                return cancelOrderModel;
            }


        }
        [Route("UpdateComplaint")]
        [HttpGet]
        public void UpdateComplaint(int contactId)
        {

            var con = _generalAppService.GetContactbyId(contactId);
            //var con = GetContact(contactId);
            var result = _dbService.UpdateComplaintBot(con.UserId, 0, true).Result;
            if (result != null)
            {
                result.customerChat = null;
                //    _hub2.Clients.All.SendAsync("brodCastEndUserMessage", result);
                SocketIOManager.SendContact(result, con.TenantId.HasValue ? con.TenantId.Value : 0);
            }

        }
        [Route("UpdateComplaintMG")]
        [HttpGet]
        public void UpdateComplaintMG(int contactId, string subject, string content)
        {

            try
            {
                var con = _generalAppService.GetContactbyId(contactId);
                //var con = GetContact(contactId);
                var result = _dbService.UpdateComplaintBot(con.UserId, 0, true).Result;
                if (result != null)
                {
                    result.customerChat = null;
                    //    _hub2.Clients.All.SendAsync("brodCastEndUserMessage", result);
                    SocketIOManager.SendContact(result, con.TenantId.HasValue ? con.TenantId.Value : 0);
                }

                //create new Tickets in MG
                if (con.TenantId == 59)
                {
                    SendTicketMg model = new SendTicketMg();

                    model.subject = subject;
                    model.content = content;
                    model.phoneNumber = con.PhoneNumber;


                    List<CreateTicket> properties = CreateFun(model);

                    var ticket = _ticketsAPI.Create(properties.ToArray());

                    var contactMGId = _contactsAPI.SearchContactsMg(model.phoneNumber);

                    UpdateFun(ticket, contactMGId);

                }
            }
            catch (Exception)
            {


            }


        }

        [Route("GetOrderAndDetails")]
        [HttpPost]
        public OrderAndDetailsModel GetOrderAndDetails([FromBody] GetOrderAndDetailModel input)
        {

            TenantModel tenant = GetTenantById(input.TenantID).Result;

            input.isOrderOffer=tenant.isOrderOffer;

            if (input.TypeChoes=="Delivery")
            {
                input.LocationId=input.LocationInfo.LocationId;

            }

            var order = _iOrdersAppService.GetOrderExtraDetails(input.TenantID, input.ContactId);

            List<OrderDetailDto> OrderDetailList = new List<OrderDetailDto>();

            List<ExtraOrderDetailsDto> getOrderDetailExtraList = new List<ExtraOrderDetailsDto>();

            if (!string.IsNullOrEmpty(order.OrderDetailDtoJson))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                OrderDetailList = System.Text.Json.JsonSerializer.Deserialize<List<OrderDetailDto>>(order.OrderDetailDtoJson, options);
            }
            foreach (var item in OrderDetailList)
            {
                if (item.extraOrderDetailsDtos != null)
                    getOrderDetailExtraList.AddRange(item.extraOrderDetailsDtos);


            }
            //var order = GetOrder(input.TenantID, input.ContactId);
            // var OrderDetailList = GetOrderDetail(input.TenantID, int.Parse(order.Id.ToString()));
            // var getOrderDetailExtraList = GetOrderDetailExtra(input.TenantID);


            // var itemList = GetItem(input.TenantID);

            var orderEffor = GetOrderOffer(input.TenantID);

            var areaEffor = orderEffor.Where(x => x.isAvailable == true && x.isPersentageDiscount == true).FirstOrDefault();
            bool isDiscount = false;
            decimal Discount = 0;

            if (areaEffor != null && input.isOrderOffer)
            {
                if (order.Total >= areaEffor.FeesStart && order.Total <= areaEffor.FeesEnd)
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

                            if (areaEffor.isBranchDiscount)
                            {
                                if (areaEffor.BranchesIds.Contains(input.LocationId.ToString()))
                                {
                                    order.Total = order.Total - (order.Total * areaEffor.NewFees) / 100;
                                    isDiscount = true;
                                    Discount = areaEffor.NewFees;
                                }
                            }
                            else
                            {
                                order.Total = order.Total - (order.Total * areaEffor.NewFees) / 100;
                                isDiscount = true;
                                Discount = areaEffor.NewFees;
                            }


                        }



                    }
                }
            }
            if (input.LocationInfo==null)
                input.LocationInfo=new GetLocationInfoModel() { DeliveryCostAfter=0, DeliveryCostBefor=0, LocationId=input.LocationId };

            var after = input.LocationInfo.DeliveryCostAfter;
            decimal? cost = 0;
            if (input.isOrderOffer)
            {
                if (string.IsNullOrEmpty(input.LocationInfo.City))
                {
                    input.LocationInfo.City="NotFound";
                }
                if (string.IsNullOrEmpty(input.LocationInfo.Area))
                {
                    input.LocationInfo.Area="NotFound";
                }
                if (string.IsNullOrEmpty(input.LocationInfo.Distric))
                {
                    input.LocationInfo.Distric="NotFound";
                }

                var locationList = GetAllLocationInfoModel();

                var cityModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.City).FirstOrDefault();
                var areaModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.Area).FirstOrDefault();
                var districModel = locationList.Where(x => x.LocationNameEn == input.LocationInfo.Distric).FirstOrDefault();

                var cityName = "NotFound";
                var areaName = "NotFound";
                var districName = "NotFound";

                if (cityModel != null)
                    cityName = cityModel.LocationName;
                if (areaModel != null)
                    areaName = areaModel.LocationName;
                if (districModel != null)
                    districName = districModel.LocationName;



                OrderOfferFun(input.TenantID, input.isOrderOffer, order.Total, input.LocationInfo, cityName, areaName, districName, cost.Value);




                if (input.LocationInfo.isOrderOfferCost)
                {
                    cost =input.LocationInfo.DeliveryCostBefor;
                    input.LocationInfo.DeliveryCostAfter=input.LocationInfo.DeliveryCostBefor;
                    input.LocationInfo.DeliveryCostBefor=after;

                    input.deliveryCostBefor=input.LocationInfo.DeliveryCostBefor;
                    input.deliveryCostAfter=input.LocationInfo.DeliveryCostAfter;

                    input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());


                    cost=input.deliveryCostAfter;

                    //  order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                }
                else
                {
                    input.LocationInfo.DeliveryCostAfter=after;
                    cost=input.deliveryCostAfter;
                }



            }
            else
            {
                cost= input.Cost;

                //  order.Total=order.Total+cost.Value;
            }


            // input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());

            var OrderString = GetOrderDetailString(input.TenantID, input.lang, order.Total, order.TotalPoints, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount, input.TypeChoes, input.DeliveryCostText, cost);


            //if (input.LocationInfo.isOrderOfferCost)
            //{
            //    order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
            //}

           // var totalPoint = order.TotalPoints;

            OrderAndDetailsModel orderAndDetailsModel = new OrderAndDetailsModel
            {
                order = order,
                DetailText = OrderString,
                orderId = int.Parse(order.Id.ToString()),
                total = order.Total,
                Discount = Discount,
                IsDiscount = isDiscount,
                GetLocationInfo=input.LocationInfo,
                IsItemOffer=isDiscount,
                ItemOffer=Discount,
                IsDeliveryOffer=input.LocationInfo.isOrderOfferCost,
                DeliveryOffer=after

            };



            return orderAndDetailsModel;
        }

        [Route("DeleteOrderDraft")]
        [HttpGet]
        public void DeleteOrderDraft(int tenantID, int orderId)
        {

            var orderDetail = GetOrderDetail(tenantID, orderId);
            var OrderDetailExtraList = GetOrderDetailExtra(tenantID);

            foreach (var deteal in orderDetail)
            {

                var GetOrderDetailExtraDraft = OrderDetailExtraList.Where(x => x.OrderDetailId == deteal.Id).ToList();

                foreach (var ExtraOrde in GetOrderDetailExtraDraft)
                {

                    DeleteExtraOrderDetail(ExtraOrde.Id);
                }

                DeleteOrderDetails(deteal.Id);

            }

            DeleteOrder(orderId);

        }

        [Route("GetContcatModel")]
        [HttpGet]
        public ContactDto GetContcatModel(int ContactId)
        {
            var con = _generalAppService.GetContactbyId(ContactId);

            //var con = GetContact(ContactId);

            return con;
        }

        [Route("TestCaption")]
        [HttpGet]
        public void TestCaption()
        {
            var locationList = GetAllLocationInfoModel();

            foreach (var item in locationList)
            {
                item.LocationNameEn = item.LocationNameEn.Trim();
                UpdateTextLocation(item);
            }

        }





        [Route("GetDeliveryBranch")]
        [HttpGet]
        public BotAPI.Models.Location.DeliveryLocationCost GetDeliveryBranch(int TenantID, string FromDistrichName, string FromAreaName, string ToDistrichName, string ToAreaName)
        {
            try
            {
                var LocationList = GetAllLocationInfoModel();

                var FromDistrich = LocationList.Where(x => x.LocationNameEn == FromDistrichName).FirstOrDefault();
                var FromArea = LocationList.Where(x => x.LocationNameEn == FromAreaName && x.LevelId == 3).FirstOrDefault();

                var ToDistrich = LocationList.Where(x => x.LocationNameEn == ToDistrichName).FirstOrDefault();
                var ToArea = LocationList.Where(x => x.LocationNameEn == ToAreaName && x.LevelId == 3).FirstOrDefault();



                if (FromDistrich == null || FromArea == null || ToDistrich == null || ToArea == null)
                {
                    BotAPI.Models.Location.DeliveryLocationCost branch = new BotAPI.Models.Location.DeliveryLocationCost();

                    branch.TenantId = TenantID;
                    branch.DeliveryCost = -1;
                    branch.Id = 0;
                    branch.BranchAreaId = 0;

                    return branch;

                }

                var costList = GetAllDeliveryLocationCost(TenantID);






                var Cos = costList.Where(x => (x.FromLocationId == FromDistrich.Id && x.ToLocationId == ToDistrich.Id) || (x.FromLocationId == ToDistrich.Id && x.ToLocationId == FromDistrich.Id)).FirstOrDefault();

                if (Cos == null)
                {

                    var Cos2 = costList.Where(x => (x.FromLocationId == FromArea.Id && x.ToLocationId == ToArea.Id) || (x.FromLocationId == ToArea.Id && x.ToLocationId == FromArea.Id)).FirstOrDefault();

                    if (Cos2 == null)
                    {
                        BotAPI.Models.Location.DeliveryLocationCost branch = new BotAPI.Models.Location.DeliveryLocationCost();

                        branch.TenantId = TenantID;
                        branch.DeliveryCost = -1;
                        branch.Id = 0;
                        branch.BranchAreaId = 0;

                        return branch;


                    }
                    else
                    {
                        BotAPI.Models.Location.DeliveryLocationCost branch = new BotAPI.Models.Location.DeliveryLocationCost();

                        branch.TenantId = TenantID;
                        branch.DeliveryCost = Cos2.DeliveryCost;
                        branch.Id = Cos2.Id;
                        branch.FromLocationId = FromArea.Id;
                        branch.ToLocationId = ToArea.Id;
                        return branch;



                    }


                }
                else
                {
                    BotAPI.Models.Location.DeliveryLocationCost branch = new BotAPI.Models.Location.DeliveryLocationCost();

                    branch.TenantId = TenantID;
                    branch.DeliveryCost = Cos.DeliveryCost;
                    branch.Id = Cos.Id;
                    branch.FromLocationId = FromDistrich.Id;
                    branch.ToLocationId = ToDistrich.Id;
                    return branch;

                }


            }
            catch
            {
                BotAPI.Models.Location.DeliveryLocationCost branch = new BotAPI.Models.Location.DeliveryLocationCost();
                branch.TenantId = TenantID;
                branch.DeliveryCost = -1;
                branch.Id = 0;
                branch.BranchAreaId = 0;

                return branch;


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



        [Route("GetNearbyLocationsPinned")]
        [HttpGet]
        public LocationsPinned GetNearbyLocationsPinned(int tenantID, string query)
        {


            if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
            {
                //  31.953255344085655, 35.847525840215276
                LocationsPinned locationsPinned = new LocationsPinned();

                float lata = (float)Convert.ToDouble(query.Split(",")[0]);
                float longt = (float)Convert.ToDouble(query.Split(",")[1]);

                var result = _generalAppService.GetNearbyLocationsPinned(tenantID, lata, longt);
                if (result != null)
                {
                    locationsPinned = result;
                }
                return locationsPinned;

            }
            else
            {
                return new LocationsPinned();
            }




        }



        [Route("GetNearbyArea")]
        [HttpGet]
        public locationAddressModel GetNearbyArea(int tenantID, string query)
        {

            try
            {
                

                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    //  31.953255344085655, 35.847525840215276
                    locationAddressModel locationAddressModel = new locationAddressModel();
                    var rez = GetLocation(query);

                    double lata = Convert.ToDouble(query.Split(",")[0]);
                    double longt = Convert.ToDouble(query.Split(",")[1]);



                    var Country = rez.Country.Replace("'", "").Trim();
                    var City = rez.City.Replace("'", "").Trim();
                    var Area = rez.Area.Replace("'", "").Trim();
                    var Distric = rez.Distric.Replace("'", "").Trim();
                    var Route = rez.Route.Replace("'", "").Trim();


                    Dictionary<int, string> lstLocation = new Dictionary<int, string>();
                    List<lstLocation> dlstLocation = new List<lstLocation>();

                    string locationName;
                    if (!string.IsNullOrEmpty(Distric))
                    {
                        lstLocation objlstLocation = new lstLocation();
                        objlstLocation.LevelId = 3;
                        objlstLocation.LocationName = Distric;
                        dlstLocation.Add(objlstLocation);

                    }
                    if (!string.IsNullOrEmpty(Area))
                    {
                        locationName = Area;
                        lstLocation objlstLocation = new lstLocation();
                        objlstLocation.LevelId = 2;
                        objlstLocation.LocationName = Area;
                        dlstLocation.Add(objlstLocation);
                    }
                    if (!string.IsNullOrEmpty(City))
                    {
                        lstLocation objlstLocation = new lstLocation();
                        objlstLocation.LevelId = 1;
                        objlstLocation.LocationName = City;
                        dlstLocation.Add(objlstLocation);
                    }
                    // locationName = City;





                    //  locationName = Distric + "," + Area + "," + City;
                    var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

                    var AddressEnglish = add;
                    var Address = Translate(add);//translat text 


                    LocationInfoModelDto locationInfoModelDto = _iAreasAppService.GetLocationDeliveryCost(tenantID, JsonConvert.SerializeObject(dlstLocation).ToString());

                    if (locationInfoModelDto != null && locationInfoModelDto.Id > 0)
                    {
                        locationAddressModel.Country = Country;
                        locationAddressModel.City = City;
                        locationAddressModel.Area = Area;
                        locationAddressModel.Distric = Distric;
                        locationAddressModel.Route = Route;
                        locationAddressModel.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                        locationAddressModel.AddressEnglish = locationAddressModel.Address;
                        locationAddressModel.Address = Translate(locationAddressModel.Address);//translat text 
                        double distance;
                        AreaDto areaDto = getNearbyArea(tenantID, lata, longt, City, locationInfoModelDto.AreaId, out distance);

                        if (areaDto.Id == 0)
                        {

                            return new locationAddressModel()
                            {
                                DeliveryCostAfter = -1,
                                DeliveryCostBefor = -1,
                            };
                        }

                        locationAddressModel.AreaNameEnglish = areaDto.AreaNameEnglish;
                        locationAddressModel.AreaName = areaDto.AreaName;
                        locationAddressModel.AreaCoordinate = areaDto.AreaCoordinate;
                        locationAddressModel.AreaCoordinateEnglish = areaDto.AreaCoordinateEnglish;

                        locationAddressModel.AreaId = (int)areaDto.Id;
                        locationAddressModel.DeliveryCostAfter = locationInfoModelDto.DeliveryCost.HasValue ? locationInfoModelDto.DeliveryCost.Value : 0;
                        locationAddressModel.DeliveryCostBefor = locationInfoModelDto.DeliveryCost.HasValue ? locationInfoModelDto.DeliveryCost.Value : 0;

                        return locationAddressModel;
                    }
                    else
                    {
                        var raz = GetlocationUserTowModel(tenantID, AddressEnglish);

                        return new locationAddressModel()
                        {
                            List=raz,
                            Address=Address,
                            AddressEnglish=AddressEnglish,
                            DeliveryCostAfter = -1,
                            DeliveryCostBefor = -1,

                        };
                    }
                }
                else
                {
                    return new locationAddressModel()
                    {


                        DeliveryCostAfter = -1,
                        DeliveryCostBefor = -1,
                    };
                }

            }
            catch (Exception)
            {
                return new locationAddressModel()
                {
                    DeliveryCostAfter = -1,
                    DeliveryCostBefor = -1,
                };
            }

        }


        private class lstLocation
        {
            public int LevelId { get; set; }
            public string LocationName { get; set; }
        }

        [Route("GetlocationUserDelivery")]
        [HttpGet]
        public locationAddressModel GetlocationUserDelivery(string query)
        {
            locationAddressModel locationAddressModel = new locationAddressModel();

            try
            {
                if (IsvalidLatLong(query.Split(",")[0], query.Split(",")[1]))
                {
                    var rez = GetLocation(query);



                    var Country = rez.Country.Replace("'", "").Trim();
                    var City = rez.City.Replace("'", "").Trim();
                    var Area = rez.Area.Replace("'", "").Trim();
                    var Distric = rez.Distric.Replace("'", "").Trim();
                    var Route = rez.Route.Replace("'", "").Trim();


                    locationAddressModel.Country = Country;
                    locationAddressModel.City = City;
                    locationAddressModel.Area = Area;
                    locationAddressModel.Distric = Distric;
                    locationAddressModel.Route = Route;
                    locationAddressModel.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;



                    locationAddressModel.Address = Translate(locationAddressModel.Address);//translat text 



                    return locationAddressModel;
                }
                else
                {
                    return locationAddressModel;
                }
            }
            catch (Exception)
            {

                return locationAddressModel;
            }


        }

        [Route("IsValidLocation")]
        [HttpPost]
        public bool IsValidLocation(string input)
        {
            try
            {
                return IsvalidLatLong(input.Split(",")[0], input.Split(",")[1]);

            }
            catch (Exception)
            {

                return false;
            }
        }
        //AddHours(-3)
        [Route("GetlocationUserModel")]
        [HttpPost]
        public GetLocationInfoModel GetlocationUserModel([FromBody] SendLocationUserModel input)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {


                    TenantModel tenant = GetTenantById(input.tenantID).Result;

                    input.isOrderOffer=tenant.isOrderOffer;
                    if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
                    {
                        GetLocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);


                        return getLocationInfoModel;
                    }
                    else
                    {
                        GetLocationInfoModel infoLocation = new GetLocationInfoModel();
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


                        if(Distric=="Irbid Qasabah District" && Area=="")
                        {
                            Distric=Route;
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
                            decimal va1 = -1;

                            try
                            {
                                va1=decimal.Parse(spilt[0].ToString());

                            }
                            catch
                            {

                            }

                            if (va1<0)
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
                                    infoLocation.LocationAreaName=areaname.AreaName;




                                    if (!areaname.IsAvailableBranch)
                                    {


                                        infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
                                        infoLocation.DeliveryCostAfter = -1;
                                        infoLocation.DeliveryCostBefor = -1;
                                        infoLocation.LocationId = 0;

                                        return infoLocation;


                                    }

                                    OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);



                                    //if (areaname.IsRestaurantsTypeAll)
                                    //{
                                    //    infoLocation.LocationId = 0;

                                    //}

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
                    GetLocationInfoModel infoLocation = new GetLocationInfoModel();
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
                GetLocationInfoModel infoLocation = new GetLocationInfoModel();

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

        //AddHours(-3)
        [Route("GetlocationUserTowModel")]
        [HttpGet]
        public List<string> GetlocationUserTowModel(int TenantID, string address)
        {
            string connString = AppSettingsModel.ConnectionStrings;

            var locationList = GetAllLocationInfoModel();
            var costList = GetAllLocationDeliveryCost(TenantID);
            List<GetLocationInfoModel> infoLocation = new List<GetLocationInfoModel>();
            List<string> vs = new List<string>();

            try
            {
                var Country = "";
                var City = "";
                try
                {
                    Country = address.Split(" - ")[4].Replace("'", "").Trim();
                    City = address.Split(" - ")[3].Replace("'", "").Trim();
                }
                catch
                {
                    Country = address.Split(" - ")[3].Replace("'", "").Trim();
                    City = address.Split(" - ")[2].Replace("'", "").Trim();

                }

                var Area = address.Split(" - ")[2].Replace("'", "").Trim();
                var Distric = address.Split(" - ")[1].Replace("'", "").Trim();
                var Route = address.Split(" - ")[0].Replace("'", "").Trim();

                if (City == "Jerash Governorate")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Jerash Governorate A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }

                }
                else if (City == "Ajloun")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Ajloun A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }
                else if (City == "Kufranjah")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Ajloun B").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }
                else if (City == "Al-Mafraq")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Al-Mafraq A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }
                else if (City == "As-Salt")
                {
                    //var result = _generalAppService.GetNearbyLocationsPinned(tenantID, lata, longt);

                    //infoLocation.Add(new GetLocationInfoModel
                    //{

                    //    LocationAreaName = item.LocationName,
                    //    DeliveryCostAfter = result.d,
                    //    DeliveryCostBefor = 0,
                    //    isOrderOfferCost = false,


                    //});

                    var loc = locationList.Where(x => x.LocationNameEn == "As-Salt A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }
                else if (City == "Ain Albasha District")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Ain Albasha District A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }
                else if (City == "Aqaba")
                {
                    var loc = locationList.Where(x => x.LocationNameEn == "Aqaba A").FirstOrDefault();
                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                    foreach (var item in listLoca)
                    {
                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                        if (FoindCost != null)
                        {

                            infoLocation.Add(new GetLocationInfoModel
                            {

                                LocationAreaName = item.LocationName,
                                DeliveryCostAfter = FoindCost.DeliveryCost,
                                DeliveryCostBefor = 0,
                                isOrderOfferCost = false,


                            });
                        }


                    }


                }



                List<decimal?> vs1 = new List<decimal?>();
                var OrderByCostList = infoLocation.OrderBy(x => x.DeliveryCostAfter);
                foreach (var it in OrderByCostList)
                {
                    vs1.Add(it.DeliveryCostAfter);

                }


                var myList = vs1.Distinct();

                foreach (var item in myList)
                {

                    var x = OrderByCostList.Where(x => x.DeliveryCostAfter == item);

                    var nameOp = "";
                    foreach (var item2 in x)
                    {

                        nameOp = nameOp + "-" + item2.LocationAreaName;

                    }
                    vs.Add(nameOp);

                }




                return vs;
            }
            catch (Exception)
            {
                return vs;
            }


        }


        [Route("GetlocationUserThreeModel")]
        [HttpGet]
        public decimal? GetlocationUserThreeModel(int TenantID, string address, string select)
        {
            string connString = AppSettingsModel.ConnectionStrings;

            var locationList = GetAllLocationInfoModel();
            var costList = GetAllLocationDeliveryCost(TenantID);
            List<GetLocationInfoModel> infoLocation = new List<GetLocationInfoModel>();
            List<string> vs = new List<string>();


            var Country = "";
            var City = "";
            try
            {
                Country = address.Split(" - ")[4].Replace("'", "").Trim();
                City = address.Split(" - ")[3].Replace("'", "").Trim();
            }
            catch
            {
                Country = address.Split(" - ")[3].Replace("'", "").Trim();
                City = address.Split(" - ")[2].Replace("'", "").Trim();

            }
            var Area = address.Split(" - ")[2].Replace("'", "").Trim();
            var Distric = address.Split(" - ")[1].Replace("'", "").Trim();
            var Route = address.Split(" - ")[0].Replace("'", "").Trim();

            if (City == "Jerash Governorate")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Jerash Governorate A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }

            }
            else if (City == "Ajloun")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Ajloun A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }

            }
            else if (City == "Kufranjah")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Ajloun B").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }


            }
            else if (City == "Al-Mafraq")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Al-Mafraq A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }


            }

            else if (City == "As-Salt")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "As-Salt A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }


            }
            else if (City == "Ain Albasha District")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Ain Albasha District A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }


            }
            else if (City == "Aqaba")
            {
                var loc = locationList.Where(x => x.LocationNameEn == "Aqaba A").FirstOrDefault();
                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

                foreach (var item in listLoca)
                {
                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
                    if (FoindCost != null)
                    {

                        infoLocation.Add(new GetLocationInfoModel
                        {

                            LocationAreaName = item.LocationName,
                            DeliveryCostAfter = FoindCost.DeliveryCost,
                            DeliveryCostBefor = 0,
                            isOrderOfferCost = false,


                        });
                    }


                }


            }

            List<decimal?> vs1 = new List<decimal?>();
            var OrderByCostList = infoLocation.OrderBy(x => x.DeliveryCostAfter);
            foreach (var it in OrderByCostList)
            {

                if (select.ToLower().Trim().Contains(it.LocationAreaName.ToLower().Trim()))
                {

                    return it.DeliveryCostAfter;
                }

            }







            return 0;

        }


        [Route("GetDay")]
        [HttpGet]
        public List<string> GetDay(string local)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i <= 6; i++)
            {
                var day = DateTime.Now.AddDays(i);
                string dayName = "";
                string date = "";
                if (local == "ar")
                {
                    dayName = day.ToString("dddd", new CultureInfo("ar-AE"));
                    date = day.ToString("dd/MM", new CultureInfo("ar-AE"));
                }
                else
                {
                    dayName = day.ToString("dddd", new CultureInfo("en-US"));
                    date = day.ToString("dd/MM", new CultureInfo("en-US"));
                }

                var st = dayName + "(" + date + ")";

                vs.Add(st);
            }

            return vs;
        }

        [Route("GetDayMg")]
        [HttpGet]
        public List<string> GetDayMg(string local, bool isMaintenance)
        {
            List<string> vs = new List<string>();
            if (isMaintenance)
            {
                int size = 8;
                int start = 4;
                for (int i = start; i <= size; i++)
                {
                    if (DateTime.Now.AddDays(i).ToString("dddd", new CultureInfo("en-US"))=="Friday" || DateTime.Now.AddDays(i).ToString("dddd", new CultureInfo("en-US"))=="Saturday")
                    {

                        size++;
                    }
                    else
                    {
                        var day = DateTime.Now.AddDays(i);
                        string dayName = "";
                        string date = "";
                        if (local == "ar")
                        {
                            dayName = day.ToString("dddd", new CultureInfo("ar-AE"));
                            date = day.ToString("dd/MM/yyyy", new CultureInfo("ar-AE"));
                        }
                        else
                        {
                            dayName = day.ToString("dddd", new CultureInfo("en-US"));
                            date = day.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                        }

                        var st = dayName + "," + date;

                        vs.Add(st);
                    }

                }

            }
            else
            {


                int size = 7;
                int start = 1;
                for (int i = start; i <= size; i++)
                {
                    if (DateTime.Now.AddDays(i).ToString("dddd", new CultureInfo("en-US"))=="Friday")
                    {

                        size++;
                    }
                    else
                    {
                        var day = DateTime.Now.AddDays(i);
                        string dayName = "";
                        string date = "";
                        if (local == "ar")
                        {
                            dayName = day.ToString("dddd", new CultureInfo("ar-AE"));
                            date = day.ToString("dd/MM/yyyy", new CultureInfo("ar-AE"));
                        }
                        else
                        {
                            dayName = day.ToString("dddd", new CultureInfo("en-US"));
                            date = day.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                        }

                        var st = dayName + "," + date;

                        vs.Add(st);
                    }

                }
            }


            return vs;
        }

        [Route("GetTimeMg")]
        [HttpGet]
        public List<string> GetTimeMg(int TenantID, bool isMaintenance)
        {
            List<string> vs = new List<string>();


            if (isMaintenance)
            {
                vs.Add("10:00 AM");
                vs.Add("10:15 AM");
                vs.Add("10:30 AM");

            }
            else
            {
                //vs.Add("10:00");
               // vs.Add("10:30");
                vs.Add("11:00");
                vs.Add("11:30");
                vs.Add("12:00");
                vs.Add("12:30");
                vs.Add("13:00");
                vs.Add("13:30");
                vs.Add("14:00");
                vs.Add("14:30");

            }


            return vs;

        }

        //AddHours(3)
        [Route("GetTime")]
        [HttpGet]
        public List<string> GetTime(int TenantID, string selectDay, string local)
        {
            if (TenantID == 34)
            {
                List<string> vs = new List<string>();

                vs.Add("8 AM - 11 AM");
                vs.Add("11 AM - 2 PM");
                vs.Add("2 PM - 5 PM");
                vs.Add("5 PM - 8 PM");


                return vs;
            }
            else if (TenantID == 42)
            {
                List<string> vs = new List<string>();
                if (local == "ar")
                {
                    vs.Add("18:30- 19:00 افطار");
                    vs.Add("19:15- 19:45 افطار");
                    vs.Add("سحور 2 - 3");
                    vs.Add("سحور 3 - 4");
                }
                else
                {
                    vs.Add("18:30-19:00 Iftar");
                    vs.Add("19:15-19:45 Iftar");
                    vs.Add("2 - 3 Sohoor");
                    vs.Add("3 - 4 Sohoor");
                }



                return vs;
            }
            else if (TenantID == 124123123)
            {



                List<int> listTime = new List<int>();

                List<string> listTimeString = new List<string>();
                var timeNow = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                string dayName = "";
                string date = "";
                if (local == "ar")
                {
                    dayName = DateTime.Now.ToString("dddd", new CultureInfo("ar-AE"));
                    date = DateTime.Now.ToString("dd/MM", new CultureInfo("ar-AE"));
                }
                else
                {
                    dayName = DateTime.Now.ToString("dddd", new CultureInfo("en-US"));
                    date = DateTime.Now.ToString("dd/MM", new CultureInfo("en-US"));
                }

                var st = dayName + "(" + date + ")";


                var resulttimeNow = Convert.ToDateTime(timeNow);
                int startTime = 15 - 1;//3:00 pm

                if (TenantID == 5)
                    startTime = 10;

                for (int i = 0; i < 10; i++)
                {
                    startTime++;
                    if (startTime > 24)
                        break;


                    if (selectDay.Contains(st))
                    {
                        if (startTime > resulttimeNow.Hour)
                            listTime.Add(startTime);

                    }
                    else
                    {
                        listTime.Add(startTime);

                    }

                }

                foreach (var item in listTime)
                {

                    listTimeString.Add(item.ToString() + ":00");
                }


                return listTimeString;
            }
            else if (TenantID == 59)
            {

                List<string> vs = new List<string>();


                vs.Add("9 AM - 10 AM");
                vs.Add("10 AM - 11 AM");
                vs.Add("11 AM - 12 PM");
                vs.Add("12 PM - 1 PM");

                return vs;

            }
            else
            {

                //List<string> vs = new List<string>();

                //vs.Add("11 AM");
                //vs.Add("12 PM");
                //vs.Add("1 PM");
                //vs.Add("2 PM");
                //vs.Add("3 PM");
                //vs.Add("4 PM");
                //vs.Add("5 PM");
                //vs.Add("6 PM");
                //vs.Add("7 PM");
                //vs.Add("8 PM");

                List<string> vs = new List<string>();

                vs.Add("8 AM - 9 AM");
                vs.Add("9 AM - 10 AM");
                vs.Add("10 AM - 11 AM");
                vs.Add("11 AM - 12 PM");
                vs.Add("12 PM - 1 PM");
                vs.Add("1 PM - 2 PM");
                vs.Add("2 PM - 3 PM");
                vs.Add("3 PM - 4:30 PM");
                return vs;

            }



        }


        [Route("GetAreasWithPage")]
        [HttpGet]
        public List<string> GetAreasWithPage(string TenantID, string local, int menu, int pageNumber, int pageSize, bool isDelivery)
        {

            List<string> vs = new List<string>();
            var list = _iAreasAppService.GetAllAreas(int.Parse(TenantID),true);// GetAreasList(TenantID);

            if (list.Count > 8)
            {
                var values = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                if (pageNumber >= 1)
                {
                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                    if (local == "ar")
                    {
                        values2.Add(new AreaDto
                        {
                            AreaName = "العودة",
                            AreaCoordinate = ""
                        });
                    }
                    else
                    {
                        values2.Add(new AreaDto
                        {
                            AreaNameEnglish = "Back",
                            AreaCoordinate = ""
                        });
                    }
                    if (list.Count - (values2.Count - 1) - (pageNumber*8)  > 0)
                    {
                        if (local == "ar")
                        {
                            values2.Add(new AreaDto
                            {
                                AreaName = "اخرى",
                                AreaCoordinate = ""
                            });
                        }
                        else
                        {
                            values2.Add(new AreaDto
                            {
                                AreaNameEnglish = "Others",
                                AreaCoordinate = ""
                            });
                        }
                    }
                    foreach (var item in values2)
                    {

                        if (local == "ar")
                        {
                            vs.Add(item.AreaName);
                        }
                        else
                        {
                            vs.Add(item.AreaNameEnglish);
                        }
                    }
                    return vs;
                }
                else
                {
                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();

                    if ((list.Count - values2.Count) > 0)
                    {
                        if (local == "ar")
                        {
                            values2.Add(new AreaDto
                            {
                                AreaName = "اخرى",
                                AreaCoordinate = ""
                            });
                        }
                        else
                        {
                            values2.Add(new AreaDto
                            {
                                AreaNameEnglish = "Others",
                                AreaCoordinate = ""
                            });
                        }
                    }
                    foreach (var item in values2)
                    {

                        if (local == "ar")
                        {
                            vs.Add(item.AreaName);
                        }
                        else
                        {
                            vs.Add(item.AreaNameEnglish);
                        }
                    }
                    return vs;
                }
            }
            else
            {

                foreach (var item in list)
                {

                    if (local == "ar")
                    {

                        vs.Add(item.AreaName);
                    }
                    else
                    {
                        vs.Add(item.AreaNameEnglish);
                    }
                }

                return vs;
            }

        }
        [Route("GetLastFiveRequest")]
        [HttpGet]
        public string GetLastFiveRequest(int contactId, int tenantId)
        
        {
            try
            {
                return getLastFiveRequest(contactId , tenantId);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private string getLastFiveRequest(int contactId ,int tenantId)
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
        /// <summary>
        /// this Api for Create List of 9 days starting today (Jo Petrol Bot)
        /// </summary>
        /// <returns></returns>
        [Route("GetNineDay")]
        [HttpGet]
        public List<string> GetNineDay()
        {
            try
            {
                List<string> nineDay = new List<string>();
                DateTime date = DateTime.Now;
                CultureInfo arabicCulture = new CultureInfo("ar-SA");
                nineDay.Add(date.ToString("dd-MM-yyyy") + " " + date.ToString("dddd", arabicCulture)); 
                for (int i =1 ; i < 9 ; i++)
                {
                    date = date.AddDays(1);
                    nineDay.Add(date.ToString("dd-MM-yyyy") + " " + date.ToString("dddd", arabicCulture));
                }
                nineDay.Add("إدخال التاريخ يدوياً");
                return nineDay;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// this Api for check the user enter valid format (Jo Petrol Bot)
        /// </summary>
        /// <param name="date">The date entered by the user</param>
        /// <returns></returns>
        [Route("IsValidDate")]
        [HttpGet]
        public bool IsValidDate(string date)
        {
            try
            {
                // 2023-08-28
                var DateSplite = date.Split("-").ToList();
                if (DateSplite.Count == 3)
                {
                    var dds = "";
                    var MMs = "";
                    DateTime DateNow = DateTime.Now;

                    bool IsNumberDD = int.TryParse(DateSplite[0], out int valueDD);
                    if (IsNumberDD && int.Parse(DateSplite[0]) <= 9 && int.Parse(DateSplite[0]) >= 1)
                    {
                        dds = "0" + valueDD;
                    }
                    else if (IsNumberDD && int.Parse(DateSplite[0]) <= 31 && int.Parse(DateSplite[0]) >= 1)
                    {
                        dds = DateSplite[0];
                    }
                    else
                        return false;

                    bool IsNumberMM = int.TryParse(DateSplite[1], out int valueMM);
                    if (IsNumberMM && int.Parse(DateSplite[1]) <= 9 && int.Parse(DateSplite[1]) >= 1)
                    {
                        MMs = "0" + valueMM;
                    }
                    else if (IsNumberDD && int.Parse(DateSplite[1]) <= 12 && int.Parse(DateSplite[1]) >= 1)
                    {
                        MMs =DateSplite[1];
                    }
                    else
                        return false;

                    bool IsNumberYY = int.TryParse(DateSplite[2], out int valueYY);
                    if (IsNumberYY && int.Parse(DateSplite[2]) >= DateNow.Year)
                    {
                        DateSplite[2] =  DateSplite[2];
                    }
                    else
                        return false;

                    var newdat = dds + "-" + MMs + "-" + DateSplite[2];

                    bool isValidDate = DateTime.TryParseExact(newdat, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime SendgDate);

                    if (isValidDate)
                    {
                        if (SendgDate.Year > DateNow.Year)
                        {
                            return true;
                        }
                        else if (SendgDate.Year == DateNow.Year &&  SendgDate.Month > DateNow.Month)
                        {
                            return true;
                        }
                        else 
                        {
                            if (SendgDate.Year == DateNow.Year && SendgDate.Month == DateNow.Month)
                            {
                                if (SendgDate.Day > DateNow.Day)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        return false;
                    }
                    else
                        return false;
                }
                else
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Intgreation with Jo Petrol For Calculet the prise  (Jo Petrol Bot)
        /// </summary>
        /// <param name="liters">Input User Liters</param>
        /// <param name="fuelname">Input User fuelname</param>
        /// <returns></returns>
        [Route("CalculetPriseAsync")]
        [HttpGet]
        public async Task<double> CalculetPriseAsync(int liters, string fuelname)
        {
            try
            {
                if (liters >= 500 && liters <= 16000 && fuelname !=null) 
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
                        default:
                            fuelname = null;
                            break;
                    }

                    if (response.IsSuccessStatusCode && fuelname !=null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var LastResponse = JsonConvert.DeserializeObject<JoPetrolPriceModel.Rootobject>(responseContent);

                        // Access Fuelprice array within the LastResponse
                        foreach (var fuelPrice in LastResponse.Fuelprice)
                        {
                            if (fuelname == fuelPrice.fuelname)
                            {
                                totalPrise = liters * (0.015 + double.Parse(fuelPrice.price));
                                totalPrise = Math.Round(totalPrise, 2);
                                return totalPrise;
                            }
                        }
                        
                    }
                }
                return 0;
                //Quantity in liters = 500 L
                //total price = Quantity in liters(0.015 + The daily price of oil)
                //total price = 500 * (0.015 + 0.715)
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Verify the user's number, whether it exists or not, in order to allow him to request maintenance
        /// </summary>
        /// <param name="Phone">User Input </param>
        /// <returns></returns>
        [Route("CheckUserCanComplete")]
        [HttpGet]
        public bool CheckUserCanComplete(string Phone)
        {
            try
            {
                if (checkUserCanComplete(Phone))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Verify the user's number, whether it exists or not, in order to allow him to request maintenance
        /// </summary>
        /// <param name="Phone">User Input </param>
        /// <returns></returns>
        private bool checkUserCanComplete(string Phone)
        {
            try
            {
                List<JoPetrolPhoneModel> joPetrolPhoneModel = new List<JoPetrolPhoneModel>();
                var SP_Name = Constants.JoPetrolBot.SP_JoPetrolGetAllPhoneNumber;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@PhoneNumber",Phone),
                };

                joPetrolPhoneModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertJoPetrolPhoneDto, AppSettingsModel.ConnectionStrings).ToList();
                if(joPetrolPhoneModel.Count != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get ten day 
        /// </summary>
        /// <param name="local">languge </param>
        /// <returns></returns>
        [Route("GetTenDay")]
        [HttpGet]
        public List<string> GetTenDay(string local)
        {
            try
            {
                List<string> nineDay = new List<string>();
                DateTime date = DateTime.Now;
                CultureInfo Culture;
                if (local == "ar")
                {
                    Culture = new CultureInfo("ar-SA");
                }
                else
                {
                    Culture = new CultureInfo("en");
                }
                
                nineDay.Add(date.ToString("dd-MM-yyyy") + " " + date.ToString("dddd", Culture));
                for (int i = 1; i < 10; i++)
                {
                    date = date.AddDays(1);
                    nineDay.Add(date.ToString("dd-MM-yyyy") + " " + date.ToString("dddd", Culture));
                }
                return nineDay;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get hours from 10 AM to 7 PM
        /// </summary>
        /// <param name="local">languge</param>
        /// <returns></returns>
        [Route("GetTenHours")]
        [HttpGet]
        public List<string> GetTenHours(string local, string Datetime)
        {
            try
            {
                string[] dateComponents = Datetime.Split('-');
                string[] dateYearComponents = dateComponents[2].Split(' ');

                DateTime dateNow = DateTime.Now;
                List<string> nineDay = new List<string>();
                string Time1 = "ص";
                string Time2 = "م";

                if (local == "en")
                {
                    Time1 = "AM";
                    Time2 = "PM";
                }
                int count = 0;
                if (int.Parse(dateComponents[0]) == dateNow.Day && int.Parse(dateComponents[1]) == dateNow.Month && int.Parse(dateYearComponents[0]) == dateNow.Year)
                {
                    int[] myNum = { 10, 11, 12, 13, 14, 15, 16, 17, 18 };

                    for (int i = 0; i < myNum.Length; i++)
                    {
                        if (myNum[i] < dateNow.Hour)
                        {
                            // If the element is less than the current hour, skip it
                            continue;
                        }
                        switch (myNum[i])
                        {
                            case 13:
                                myNum[i] = 1;
                                break;
                            case 14:
                                myNum[i] = 2;
                                break;
                            case 15:
                                myNum[i] = 3;
                                break;
                            case 16:
                                myNum[i] = 4;
                                break;
                            case 17:
                                myNum[i] = 5;
                                break;
                            case 18:
                                myNum[i] = 6;
                                break;
                        }

                        // Convert the integer to a string with ":00" and append the appropriate time
                        string timeString = (myNum[i] == 10 || myNum[i] == 11) ? myNum[i] + ":00" + Time1 : myNum[i] + ":00" + Time2;

                        // Add the resulting string to the list
                        nineDay.Add(timeString);
                    }
                   
                    return nineDay;
                }
                else
                {
                    int[] myNum = { 10, 11, 12, 1, 2, 3, 4, 5, 6 };
                    for (int i = 0; i < myNum.Length; i++)
                    {
                        if (myNum[i] == 10 || myNum[i] == 11)
                        {
                            nineDay.Add(myNum[i] + ":00" + Time1);
                        }
                        else
                        {
                            nineDay.Add(myNum[i] + ":00" + Time2);
                        }
                    }

                    return nineDay;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("IsCorrectHours")]
        [HttpGet]
        public bool IsCorrectHours(string Time , string Datetime)
        {
            try
            {
                string[] timeComponents = Time.Split(':');
                string[] dateComponents = Datetime.Split('-');
                string[] dateYearComponents = dateComponents[2].Split(' ');
                int hour = int.Parse(timeComponents[0]);
                DateTime dateNow = DateTime.Now;
                if (int.Parse(dateComponents[0]) == dateNow.Day && int.Parse(dateComponents[1]) == dateNow.Month && int.Parse(dateYearComponents[0]) == dateNow.Year)
                {
                    switch (hour)
                    {
                        case 1:
                            hour = 13;
                            break;
                        case 2:
                            hour = 14;
                            break;
                        case 3:
                            hour = 15;
                            break;
                        case 4:
                            hour = 16;
                            break;
                        case 5:
                            hour = 17;
                            break;
                        case 6:
                            hour = 18;
                            break;
                    }
                    if (hour < dateNow.Hour)
                    {
                        return false;
                    }
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        [Route("MgMotorsGetAllOfers")]
        [HttpGet]
        public List<GetAssetModel> MgMotorsGetAllOfers(int tenantId, string local)
        {
            List<GetAssetModel> result = new List<GetAssetModel>(); ;
            var lstLevels = _iAssetAppService.MgMotorsGetOfers(tenantId);
          
            if (lstLevels != null)
            {
                foreach (var item in lstLevels)
                {
                    GetAssetModel getAssetModel = new GetAssetModel();
                    if (local.Equals("ar"))
                    {
                        getAssetModel.Key = item.Id;
                        getAssetModel.Value = item.LevelTwoNameAr.Trim();
                        result.Add(getAssetModel);
                    }
                    else
                    {
                        getAssetModel.Key = item.Id;
                        getAssetModel.Value = item.LevelTwoNameEn.Trim();
                        result.Add(getAssetModel);
                    }
                }
                return result;
            }
            return result;
        }
        [Route("GetAssetLevel")]
        [HttpGet]
        public List<GetAssetModel> GetAssetLevel(int tenantId, string local, int stepId, int levelId = 0,bool  isOffer=false)
        {
            var lstLevels = _iAssetAppService.LoadLevels(tenantId);
            List<GetAssetModel> result = new List<GetAssetModel>(); ;
            if (lstLevels != null)
            {
                switch (stepId)
                {
                    case 1:
                        foreach (var item in lstLevels.lstAssetLevelOneDto)
                        {
                            GetAssetModel getAssetModel = new GetAssetModel();
                            if (local.Equals("ar"))
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelOneNameAr.Trim();
                                result.Add(getAssetModel);
                            }
                            else
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelOneNameEn.Trim();
                                result.Add(getAssetModel);
                            }
                        };
                        break;
                    case 2:
                        var levelTwo = lstLevels.lstAssetLevelTwoDto.Where(x => x.LevelOneId == levelId).ToList();
                        foreach (var item in levelTwo)
                        {
                            GetAssetModel getAssetModel = new GetAssetModel();
                            if (local.Equals("ar"))
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelTwoNameAr.Trim();
                                if (tenantId == 59 && (getAssetModel.Value == "عرض 1" || getAssetModel.Value == "عرض 2"))
                                {
                                }
                                else 
                                {
                                    result.Add(getAssetModel);
                                }
                            }
                            else
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelTwoNameEn.Trim();
                                if (tenantId == 59 && (getAssetModel.Value == "Offer 1" || getAssetModel.Value == "Offer 2"))
                                {
                                }
                                else
                                {
                                    result.Add(getAssetModel);
                                }
                            }

                        };
                        break;
                    case 3:
                        var levelThree = lstLevels.lstAssetLevelThreeDto.Where(x => x.LevelTwoId == levelId).ToList();
                        foreach (var item in levelThree)
                        {
                            GetAssetModel getAssetModel = new GetAssetModel();
                            if (local.Equals("ar"))
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelThreeNameAr.Trim();
                                result.Add(getAssetModel);
                            }
                            else
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelThreeNameEn.Trim();
                                result.Add(getAssetModel);
                            }

                        };
                        break;
                    case 4:

                        if (lstLevels.lstAssetLevelFourDto != null)
                        {
                            var levelfour = lstLevels.lstAssetLevelFourDto.Where(x => x.LevelThreeId == levelId).ToList();

                            foreach (var item in levelfour)
                            {
                                GetAssetModel getAssetModel = new GetAssetModel();
                                if (local.Equals("ar"))
                                {
                                    getAssetModel.Key = item.Id;
                                    getAssetModel.Value = item.LevelFourNameAr.Trim();
                                    result.Add(getAssetModel);
                                }
                                else
                                {
                                    getAssetModel.Key = item.Id;
                                    getAssetModel.Value = item.LevelFourNameEn.Trim();
                                    result.Add(getAssetModel);
                                }

                            };

                        }

                        break;
                }
            }


            if (tenantId==67 && stepId==3 && levelId==130)
            {

                result.RemoveAt(0);
            }


            return result;

        }
        [Route("IsOneMenu")]
        [HttpGet]
        public bool IsOneMenu(string TenantID)
        {
            var list = GetAreasList(TenantID).FirstOrDefault();

            return list.IsRestaurantsTypeAll;

        }
        [Route("Translate")]
        [HttpGet]
        public string Translate(string word)
        {

            //var list = word.Split(" - ");

            //var FirstWord = TransFun(list[0]);


            //var secWord = word.Substring(0, word.IndexOf(" - ", 4, StringComparison.Ordinal) - 1);

            //secWord = word.Replace(secWord, "");

            var secWord = TransFun(word);

            return secWord;


        }


        [Route("GetAreasID")]
        [HttpGet]
        public Area GetAreasID(string TenantID, string AreaName, int menu, string local)
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

            //if (area.IsRestaurantsTypeAll)
            //{
            //    area.Id = 0;
            //}

            return area;

        }


        [Route("GetAreasByID")]
        [HttpGet]
        public Area GetAreasByID(string TenantID, int AreaID)
        {
            try
            {
                var area = GetAreasID(AreaID);



                if (area == null)
                {
                    Area area1 = new Area();
                    area1.Id = 0;
                    return area1;
                }

                //if (area.IsRestaurantsTypeAll)
                //{
                //    area.Id = 0;
                //}

                return area;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        [Route("GetAreasID2")]
        [HttpGet]
        public Area GetAreasID2(string TenantID, string AreaName, int menu, string local)
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

        //AddHours(3)
        [Route("UpdateOrder")]
        [HttpPost]
        public async Task<string> UpdateOrderAsync([FromBody] UpdateOrderModel updateOrderModel)
        {


            if (updateOrderModel.BuyType == "No select")
                updateOrderModel.BuyType = "";

            var time = DateTime.Now;
            var timeAdd = time.AddHours(AppSettingsModel.AddHour);
            string connString = AppSettingsModel.ConnectionStrings;
            long number = 0;
            //var con = GetContact(updateOrderModel.ContactId);
            var con = _generalAppService.GetContactbyId(updateOrderModel.ContactId);

            lock (CurrentOrder)
            {
                number = UpateOrder(updateOrderModel.TenantID);

            }

            var captionTotalPointOfAllOrderText = "السعر الإجمالي لنقاط";

            if (updateOrderModel.BotLocal=="ar")
            {
                captionTotalPointOfAllOrderText="السعر الإجمالي لنقاط";
            }
            else
            {

                captionTotalPointOfAllOrderText="Total price for points";
            }


            if (updateOrderModel.TypeChoes == "PickUp")
            {

                var area = GetAreasList(updateOrderModel.TenantID.ToString()).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();
                Infoseed.MessagingPortal.Orders.Order order = new Infoseed.MessagingPortal.Orders.Order
                {

                    OrderLocal = updateOrderModel.BotLocal,
                    BranchId=updateOrderModel.BranchId,
                    HtmlPrint = "",
                    // SpecialRequestText = updateOrderModel.SpecialRequest,
                    // IsSpecialRequest= updateOrderModel.IsSpecialRequest,

                    AreaId = updateOrderModel.BranchId,
                    BranchAreaId= updateOrderModel.BranchId,
                    ContactId = updateOrderModel.ContactId,
                    OrderTime = timeAdd,
                    CreationTime = timeAdd,
                    Id = updateOrderModel.OrderId,
                    OrderNumber = number,
                    TenantId = updateOrderModel.TenantID,
                    orderStatus = OrderStatusEunm.Pending,
                    OrderType = OrderTypeEunm.Takeaway,
                    Total = updateOrderModel.OrderTotal,
                    IsDeleted = false,
                    AgentId = 0,
                    AgentIds = area.UserIds,
                    IsLockByAgent = false,
                    LockByAgentName="",
                    IsEvaluation =false,
                    Address="",
                    IsBranchArea=false,
                    BranchAreaName="",
                    LocationID=0,
                    FromLocationID=0,
                    ToLocationID=0,
                    FromLocationDescribation="",
                    FromLongitudeLatitude="",
                    OrderDescribation="",
                    AfterDeliveryCost=0,


                    IsPreOrder= updateOrderModel.IsPreOrder,
                    SelectDay= updateOrderModel.SelectDay,
                    SelectTime= updateOrderModel.SelectTime,
                    RestaurantName="",
                    BuyType="",
                    TotalPoints=updateOrderModel.loyalityPoint.Value,

                    IsItemOffer=updateOrderModel.IsItemOffer,
                    IsDeliveryOffer=updateOrderModel.IsDeliveryOffer,
                    ItemOffer=updateOrderModel.ItemOffer,
                    DeliveryOffer=updateOrderModel.DeliveryOffer


                };
                if (updateOrderModel.TenantID == 259) // OdrosJo
                {
                    order.SpecialRequestText = updateOrderModel.SpecialRequest;
                    order.IsSpecialRequest = updateOrderModel.IsSpecialRequest;
                }
                //_iOrdersAppService.UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                OrderSoket orderDto = new OrderSoket();
                orderDto = _iOrdersAppService.UpdateOrderSoket(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = orderDto.TenantId,
                    TypeId = (int)DashboardTypeEnum.Order,
                    StatusId = (int)OrderStatusEunm.Pending,
                    StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Pending)
                });

                //  updateOrderDB(updateOrderModel, timeAdd, connString, number, area);


                var ListString = "------------------ \r\n\r\n";

                ListString = ListString + updateOrderModel.captionOrderInfoText;
                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";
                ListString = ListString + updateOrderModel.aptionAreaNameText + updateOrderModel.BranchName + "\r\n";
                if (updateOrderModel.loyalityPoint.Value>0)
                {

                    ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint + "\r\n";

                }
                ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + updateOrderModel.OrderTotal + "\r\n";
                ListString = ListString + "------------------ \r\n\r\n";
                ListString = ListString + updateOrderModel.captionEndOrderText;




                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), orderDto.OrderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), orderDto.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);

                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
                getOrderForViewDto.Order = GetOrderMap;
                getOrderForViewDto.AreahName = updateOrderModel.BranchName;
                getOrderForViewDto.OrderStatusName = orderStatusName;
                getOrderForViewDto.OrderTypeName = orderTypeName;
                getOrderForViewDto.AgentIds = area.UserIds;
                getOrderForViewDto.CustomerID=con.UserId;
                getOrderForViewDto.IsAssginToAllUser = area.IsAssginToAllUser;
                getOrderForViewDto.IsAvailableBranch = area.IsAvailableBranch;
                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
                getOrderForViewDto.CreatDate = GetOrderMap.CreationTime.ToString("MM/dd/yyyy");
                getOrderForViewDto.CreatTime = GetOrderMap.CreationTime.ToString("hh:mm tt");
                getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");


                try
                {
                    var titl = "The Order Number: " + number.ToString();
                    var body = "Order Status :" + OrderTypeEunm.Takeaway.ToString() + " From :" + area.AreaName;
                    SendMobileNotification(orderDto.TenantId, titl, body, false, area.UserIds);
                }
                catch (Exception)
                {

                }



                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);

                //delete bot conversation
                //  DeleteConversation(usertodelete.SunshineConversationId);

                con.TotalOrder = con.TotalOrder + 1;
                con.TakeAwayOrder = con.TakeAwayOrder + 1;



                //////////////////////***************************//////////////////
                var loyaltyModel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);
                var modelorder = GetOrder(orderDto.Id);
                // var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(updateOrderModel.OrderTotal, updateOrderModel.TenantID);
                if (modelorder.TotalCreditPoints > 0 || orderDto.TotalPoints > 0)
                {
                    _loyaltyAppService.CreateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                    {
                        OrderId = orderDto.Id,
                        LoyaltyDefinitionId = loyaltyModel.Id,
                        ContactId = orderDto.ContactId,
                        Points= modelorder.TotalPoints,
                        CreditPoints= modelorder.TotalCreditPoints,
                        CreatedBy = orderDto.ContactId,
                        Year=DateTime.UtcNow.Year,
                        TransactionTypeId = (int)LoyaltyTransactionType.MakeOrder,
                    });

                }
                ////////////////////////*******************///////////////////////


                //con.loyalityPoint=con.loyalityPoint+(int)updateOrderModel.loyalityPoint.Value;


                //   var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(updateOrderModel.OrderTotal, updateOrderModel.TenantID);
                // var Loyaltymodel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);


                //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateStart = Convert.ToDateTime(Loyaltymodel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateEnd = Convert.ToDateTime(Loyaltymodel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));



                //if (Loyaltymodel.IsLoyalityPoint && Loyaltymodel.Id != 0 && Loyaltymodel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                //{
                //    ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                //    contactLoyalty.ContactId = updateOrderModel.ContactId;
                //    contactLoyalty.LoyaltyDefinitionId =Loyaltymodel.Id;
                //    contactLoyalty.OrderId = order.Id;
                //    contactLoyalty.CreatedBy=updateOrderModel.ContactId;
                //    contactLoyalty.Points = CardPoints;
                //    contactLoyalty.CardPoints= updateOrderModel.loyalityPoint.Value;
                //    contactLoyalty.TransactionTypeId=1;
                // _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); // point

                //    //contactLoyalty.Points =-updateOrderModel.loyalityPoint.Value;
                //   // _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); //subtract points

                //}







                // var contact = await _dbService.UpdateCustomerLocation(con);

                return ListString;

            }
            else
            {

                var area = new Area();
                try
                {
                    area = GetAreaId(updateOrderModel.MenuId);
                }
                catch
                {
                    area = null;

                }


                long agId = 0;
                if (area != null && area.Id != 0)
                {

                    if (area.UserId != 0)
                    {

                        agId = long.Parse(area.UserId.ToString());
                    }
                    else
                    {
                        area = GetAreaId(updateOrderModel.BranchId);
                        agId = updateOrderModel.BranchId;
                    }
                }
                else
                {
                    area = GetAreaId(updateOrderModel.BranchId);
                    agId = updateOrderModel.BranchId;
                }
                string AgentIds = area.UserIds;
                if (string.IsNullOrEmpty(AgentIds))
                    AgentIds = null;

                var BranchAreaName = updateOrderModel.BranchName;

                decimal totalWithBranchCost = 0;
                decimal Cost = 0;

                if (updateOrderModel.isOrderOfferCost)
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostBefor);
                    Cost = updateOrderModel.DeliveryCostBefor;
                }
                else
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostAfter);
                    Cost = updateOrderModel.DeliveryCostAfter;
                }

                var orderStatus = OrderStatusEunm.Pending;
                if (updateOrderModel.IsPreOrder)
                {
                    orderStatus = OrderStatusEunm.Pre_Order;
                }


                Infoseed.MessagingPortal.Orders.Order order = new Infoseed.MessagingPortal.Orders.Order
                {
                    OrderLocal = updateOrderModel.BotLocal,
                    BuyType = updateOrderModel.BuyType,
                    SelectDay = updateOrderModel.SelectDay,
                    SelectTime = updateOrderModel.SelectTime,
                    IsPreOrder = updateOrderModel.IsPreOrder,

                    //SpecialRequestText = updateOrderModel.SpecialRequest,
                    //IsSpecialRequest = updateOrderModel.IsSpecialRequest,
                    AfterDeliveryCost = updateOrderModel.DeliveryCostAfter,
                    AreaId = updateOrderModel.BranchId,

                    BranchAreaName = BranchAreaName,
                    BranchAreaId = updateOrderModel.BranchId,
                    Address = updateOrderModel.Address,
                    BranchId = updateOrderModel.BranchId,
                    ContactId = updateOrderModel.ContactId,
                    OrderTime = timeAdd,
                    CreationTime = timeAdd,
                    Id = updateOrderModel.OrderId,
                    OrderNumber = number,
                    TenantId = updateOrderModel.TenantID,
                    orderStatus = orderStatus,
                    OrderType = OrderTypeEunm.Delivery,
                    Total = totalWithBranchCost,
                    IsDeleted = false,
                    AgentId = 0,
                    AgentIds = AgentIds,
                    LocationID = updateOrderModel.BranchId,
                    FromLocationDescribation = "https://maps.google.com/?q=" + updateOrderModel.LocationFrom.Replace(" ", ""),
                    HtmlPrint = "",

                    IsLockByAgent = false,
                    LockByAgentName="",
                    IsEvaluation =false,


                    IsBranchArea=false,
                    FromLocationID=0,
                    ToLocationID=0,
                    FromLongitudeLatitude="",
                    OrderDescribation="",
                    RestaurantName="",
                    TotalPoints=updateOrderModel.loyalityPoint.Value,
                    DeliveryCost= Cost,

                    IsItemOffer=updateOrderModel.IsItemOffer,
                    IsDeliveryOffer=updateOrderModel.IsDeliveryOffer,
                    ItemOffer=updateOrderModel.ItemOffer,
                    DeliveryOffer=updateOrderModel.DeliveryOffer

                };






                //_iOrdersAppService.UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                OrderSoket orderDto = new OrderSoket();
                orderDto = _iOrdersAppService.UpdateOrderSoket(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = orderDto.TenantId,
                    TypeId = (int)DashboardTypeEnum.Order,
                    StatusId = (int)OrderStatusEunm.Pending,
                    StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Pending)
                });


                //var captionBranchCostText = GetCaptionFormat("BackEnd_Text_BranchCost", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//قيمة التوصيل :
                //var captionFromLocationText = GetCaptionFormat("BackEnd_Text_FromLocation", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//من الموقع :
                // UpdateDeliveryOrderDB(updateOrderModel, timeAdd, connString, number, BranchAreaName, area);



                var ListString = "------------------ \r\n\r\n";

                ListString = ListString + updateOrderModel.captionOrderInfoText;
                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";

                if (updateOrderModel.TenantID == 46)
                {
                    ListString = ListString + updateOrderModel.captionBranchCostText + ((int)Cost).ToString() + "\r\n";
                }
                else
                {
                    ListString = ListString + updateOrderModel.captionBranchCostText + Cost + "\r\n";
                }

                ListString = ListString + updateOrderModel.captionFromLocationText + updateOrderModel.Address + "\r\n";



                if (updateOrderModel.TenantID == 46)
                {
                    if (updateOrderModel.loyalityPoint.Value>0)
                    {
                        ListString = ListString + captionTotalPointOfAllOrderText + (updateOrderModel.loyalityPoint.Value).ToString() + "\r\n\r\n";

                    }
                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + ((int)totalWithBranchCost).ToString() + "\r\n\r\n";
                }
                else
                {
                    if (updateOrderModel.loyalityPoint.Value>0)
                    {
                        ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint.Value.ToString() + "\r\n\r\n";

                    }
                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + totalWithBranchCost + "\r\n\r\n";
                }



                ListString = ListString + "------------------ \r\n\r\n";
                ListString = ListString + updateOrderModel.captionEndOrderText;

                if (updateOrderModel.IsPreOrder)
                {
                    orderDto.OrderStatus = OrderStatusEunm.Pre_Order;
                }

                try
                {
                    var titl = "the order Number: " + number.ToString();
                    var body = "Order Status :" + orderDto.OrderStatus + " From :" + updateOrderModel.Address;

                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                    SendMobileNotification(orderDto.TenantId, titl, body, false, area.UserIds);
                }
                catch
                {

                }


                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), orderDto.OrderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), orderDto.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);
                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
                getOrderForViewDto.Order = GetOrderMap;
                getOrderForViewDto.OrderStatusName = orderStatusName;
                getOrderForViewDto.OrderTypeName = orderTypeName;
                getOrderForViewDto.AgentIds = AgentIds;
                getOrderForViewDto.CustomerID=con.UserId;
                getOrderForViewDto.BranchAreaName = BranchAreaName;
                //getOrderForViewDto.DeliveryCost = Cost;
                getOrderForViewDto.IsAssginToAllUser = true;// area.IsAssginToAllUser;
                getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
                getOrderForViewDto.CreatDate = GetOrderMap.CreationTime.ToString("MM/dd/yyyy");
                getOrderForViewDto.CreatTime = GetOrderMap.CreationTime.ToString("hh:mm tt");

                if (updateOrderModel.isOrderOfferCost)
                {
                    getOrderForViewDto.DeliveryCost = updateOrderModel.DeliveryCostBefor;
                }
                else
                {
                    getOrderForViewDto.DeliveryCost = updateOrderModel.DeliveryCostAfter;
                }


                getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");


                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);


                con.TotalOrder = con.TotalOrder + 1;
                con.DeliveryOrder = con.DeliveryOrder + 1;
                con.Description = updateOrderModel.Address;
                con.Website = updateOrderModel.AddressEn;
                con.EmailAddress = updateOrderModel.LocationFrom;

                con.StreetName = updateOrderModel.StreetName;
                con.BuildingNumber = updateOrderModel.BuildingNumber;
                con.FloorNo = updateOrderModel.FloorNo;
                con.ApartmentNumber = updateOrderModel.ApartmentNumber;

                //con.loyalityPoint=con.loyalityPoint+(int)updateOrderModel.loyalityPoint.Value;

                var contact = _dbService.UpdateCustomerLocation(con).Result;

                //////////////////////***************************//////////////////
                var loyaltyModel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);
                var modelorder = GetOrder(orderDto.Id);
                // var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(updateOrderModel.OrderTotal, updateOrderModel.TenantID);
                if (modelorder.TotalCreditPoints > 0 || orderDto.TotalPoints > 0)
                {
                    _loyaltyAppService.CreateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                    {
                        OrderId = orderDto.Id,
                        LoyaltyDefinitionId = loyaltyModel.Id,
                        ContactId = orderDto.ContactId,
                        Points= modelorder.TotalPoints,
                        CreditPoints= modelorder.TotalCreditPoints,
                        CreatedBy = orderDto.ContactId,
                        Year=DateTime.UtcNow.Year,
                        TransactionTypeId = (int)LoyaltyTransactionType.MakeOrder,
                    });

                }
                ////////////////////////*******************///////////////////////
                ///
                //decimal total = 0;
                //if (updateOrderModel.OrderTotal > 0)
                //{
                //    total = updateOrderModel.OrderTotal - Cost;
                //}
                //else
                //{
                //    total =0;
                //}
                //if (total < 0)
                //{
                //    total = 0;
                //}

                //var CardPoints = _loyaltyAppService.ConvertCustomerPriceToPoint(updateOrderModel.OrderTotal, updateOrderModel.TenantID);

                //var Loyaltymodel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);

                //var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateStart = Convert.ToDateTime(Loyaltymodel.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //var DateEnd = Convert.ToDateTime(Loyaltymodel.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));


                //if (Loyaltymodel.IsLoyalityPoint&& Loyaltymodel.OrderType.Contains(((int)order.OrderType).ToString())&& DateStart <= DateNow && DateEnd >= DateNow)
                //{
                //    ContactLoyaltyTransactionModel contactLoyalty = new ContactLoyaltyTransactionModel();
                //    contactLoyalty.ContactId = updateOrderModel.ContactId;
                //    contactLoyalty.LoyaltyDefinitionId =Loyaltymodel.Id;
                //    contactLoyalty.OrderId = order.Id;
                //    contactLoyalty.CreatedBy=updateOrderModel.ContactId;
                //    contactLoyalty.Points = CardPoints;
                //    contactLoyalty.CardPoints=updateOrderModel.loyalityPoint.Value;
                //    contactLoyalty.TransactionTypeId=1;
                //    _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); // point

                //    // contactLoyalty.Points =-updateOrderModel.loyalityPoint.Value;
                //    //  _loyaltyAppService.CreateContactLoyaltyTransaction(contactLoyalty); //subtract points  .AddHours(AppSettingsModel.AddHour)

                //}



                //contact.customerChat = null;
                //await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", contact);
                // SocketIOManager.SendContact(contact, contact.TenantId.HasValue ? contact.TenantId.Value : 0);
                //delete bot conversation
                // DeleteConversation(usertodelete.SunshineConversationId);
                return ListString;

            }



        }

        [Route("UpdateDeliveryOrder")]
        [HttpPost]
        public async Task<string> UpdateDeliveryOrderAsync([FromBody] OrderBotData orderBotData)
        {
            var time = DateTime.Now;

            var timeAdd = time.AddHours(AppSettingsModel.AddHour);
            string connString = AppSettingsModel.ConnectionStrings;
            //var con = GetContact(orderBotData.ContactId);
            var con = _generalAppService.GetContactbyId(orderBotData.ContactId);

            int modified = 0;
            long number = 0;

            lock (CurrentOrder)
            {
                number = UpateOrder(orderBotData.TenantID);

            }


            orderBotData.FromAddressEn = orderBotData.FromAddress;
            orderBotData.ToAddressEn = orderBotData.ToAddress;
            if (orderBotData.BotLocal == "ar")
            {

                orderBotData.FromAddress = Translate(orderBotData.FromAddress);//translat text 
                orderBotData.ToAddress = Translate(orderBotData.ToAddress);//translat text 

            }


            var ListString = string.Format(orderBotData.CaptionText, number.ToString(), orderBotData.FromAddress.Trim(), orderBotData.ToAddress.Trim(), orderBotData.FromLocationDescribatione, orderBotData.BranchCost);


            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "INSERT INTO Orders (IsDeleted, OrderTime, OrderNumber, CreationTime, IsLockByAgent, orderStatus, TenantId, ContactId, Total, AgentId, DeliveryCost, OrderType, IsEvaluation ,FromLocationID,ToLocationID,FromLocationDescribation,OrderDescribation)  VALUES (@IsDeleted, @OrderTime, @OrderNumber, @CreationTime, @IsLockByAgent, @orderStatus, @TenantId, @ContactId, @Total, @AgentId, @DeliveryCost , @OrderType , @IsEvaluation, @FromLocationID, @ToLocationID,@FromLocationDescribation,@OrderDescribation) ;SELECT SCOPE_IDENTITY(); ";

                command.Parameters.AddWithValue("@IsDeleted", false);
                command.Parameters.AddWithValue("@OrderTime", timeAdd);
                command.Parameters.AddWithValue("@OrderNumber", number);
                command.Parameters.AddWithValue("@CreationTime", timeAdd);
                command.Parameters.AddWithValue("@IsLockByAgent", false);
                command.Parameters.AddWithValue("@orderStatus", OrderStatusEunm.Pending);
                command.Parameters.AddWithValue("@TenantId", orderBotData.TenantID);
                command.Parameters.AddWithValue("@ContactId", orderBotData.ContactId);
                command.Parameters.AddWithValue("@Total", decimal.Parse(orderBotData.BranchCost));
                command.Parameters.AddWithValue("@AgentId", -1);


                command.Parameters.AddWithValue("@DeliveryCost", orderBotData.BranchCost);
                command.Parameters.AddWithValue("@OrderType", OrderTypeEunm.Delivery);
                command.Parameters.AddWithValue("@IsEvaluation", false);

                command.Parameters.AddWithValue("@FromLocationID", orderBotData.FromLocationID);
                command.Parameters.AddWithValue("@ToLocationID", orderBotData.ToLocationID);

                command.Parameters.AddWithValue("@FromLocationDescribation", orderBotData.FromLocationDescribatione);
                command.Parameters.AddWithValue("@OrderDescribation", orderBotData.OrderDescribation);

                connection.Open();


                modified = Convert.ToInt32(command.ExecuteScalar());
                if (connection.State == System.Data.ConnectionState.Open) connection.Close();

            }

            DeliveryOrderDetailsDto deliveryOrderDetailsDto = new DeliveryOrderDetailsDto
            {
                DeliveryCost = decimal.Parse(orderBotData.BranchCost),
                DeliveryCostString = orderBotData.BranchCost,
                FromAddress = orderBotData.FromAddress,
                FromLocationId = orderBotData.FromLocationID,
                FromGoogleURL = "https://maps.google.com/?q=" + orderBotData.LocationFrom,
                TenantId = orderBotData.TenantID,
                ToAddress = orderBotData.ToAddress,
                ToLocationId = orderBotData.ToLocationID,
                ToGoogleURL = "https://maps.google.com/?q=" + orderBotData.LocationTo,
                OrderId = modified,




            };


            Add(deliveryOrderDetailsDto);

            Infoseed.MessagingPortal.Orders.Order order = new Infoseed.MessagingPortal.Orders.Order
            {
                BranchAreaName = "BranchAreaName",
                BranchAreaId = orderBotData.BranchAreaId,
                Address = orderBotData.Address,
                BranchId = orderBotData.BranchID,
                ContactId = orderBotData.ContactId,
                OrderTime = timeAdd,
                CreationTime = timeAdd,
                Id = orderBotData.Id,
                OrderNumber = number,
                TenantId = orderBotData.TenantID,
                orderStatus = OrderStatusEunm.Pending,
                OrderType = OrderTypeEunm.Delivery,
                Total = decimal.Parse(orderBotData.BranchCost),
                IsDeleted = false,
                // AgentId = agId,
                IsLockByAgent = false,
                LocationID = orderBotData.LocationID
            };

            try
            {
                var titl = "the order Number: " + number.ToString();
                var body = "Order Status :" + order.orderStatus + " From :" + orderBotData.FromAddress + " To :" + orderBotData.ToAddress;

                await SendNotificationsAsync("fcm", orderBotData.TenantID.ToString(), titl, body);
            }
            catch
            {

            }



            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            //  var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
            var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);

            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.OrderTypeName = orderTypeName;

            getOrderForViewDto.BranchAreaName = "BranchAreaName";

            getOrderForViewDto.IsAssginToAllUser = true; //area.IsAssginToAllUser;
            getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
            getOrderForViewDto.TenantId = orderBotData.TenantID;
            //  await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);
            SocketIOManager.SendOrder(getOrderForViewDto, orderBotData.TenantID.Value);

            con.TotalOrder = con.TotalOrder + 1;
            con.DeliveryOrder = con.DeliveryOrder + 1;
            con.Description = orderBotData.Address;
            con.Website = orderBotData.FromAddressEn + "," + orderBotData.ToAddressEn;
            con.EmailAddress = orderBotData.LocationFrom;
            var contact = _dbService.UpdateCustomerLocation(con).Result;



            contact.customerChat = null;
            // await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", contact);
            SocketIOManager.SendContact(contact, contact.TenantId.HasValue ? contact.TenantId.Value : 0);
            //delete bot conversation
            // DeleteConversation(usertodelete.SunshineConversationId);
            return ListString;

        }

        //AddHours(3)
        [Route("CreateEvaluations")]
        [HttpGet]
        public void CreateEvaluations(int TenantId, string phoneNumber, string displayName, string EvaluationsText, string orderID, string EvaluationsReat)
        {
            var time = DateTime.Now;

            var timeAdd = time.AddHours(AppSettingsModel.AddHour);

            int evaluationRate = EvaluationsReat.ToCharArray().Count(c => c == '⭐');

            CreateOrEditEvaluationDto evaluation = new CreateOrEditEvaluationDto
            {
                ContactName = displayName,
                CreationTime = timeAdd,
                EvaluationsReat = EvaluationsReat,
                EvaluationsText = EvaluationsText.Replace("File upload:", "").Trim(),
                OrderNumber = int.Parse(orderID),
                EvaluationRate = evaluationRate > 0 ? evaluationRate : -1,
                PhoneNumber = phoneNumber,
                TenantId = TenantId

            };
            _evaluationsAppService.CreateOrEdit(evaluation);

            // _evaluationHub.Clients.All.SendAsync("brodCastBotEvaluation", evaluation);
            SocketIOManager.SendEvaluation(evaluation, TenantId);

        }

        [HttpGet("SetBundleActiveInfoseed")]
        public async Task<ActionResult> SetBundleActiveInfoseed(int TenantId, bool IsBundleActive = true, string phonenumber = null)
        {

            try
            {


                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantId);

                tenant.workModel = new Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel();

                tenant.IsBundleActive = IsBundleActive;
                var result = await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                await UpdateConversation(TenantId, phonenumber);
                _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());
            }
            catch
            {


            }

            return Ok(true);

        }

        [HttpGet("SetBundleActiveInfoseedStart")]
        public async Task<ActionResult> SetBundleActiveInfoseedStart(int TenantId)
        {

            try
            {

                bool IsBundleActive = true;
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantId);

                if (tenant.workModel == null)
                {
                    tenant.workModel = new Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel();
                }
                tenant.IsBundleActive = IsBundleActive;
                _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());

                var result = await itemsCollection.UpdateItemAsync(tenant._self, tenant);


            }
            catch
            {


            }


            return Ok(true);
        }
        [Route("TranslateText")]
        [HttpPost]
        public string TranslateText(string text, string lan)
        {
            try
            {
                var result = TranslatorFun(text, lan).Result;

                return result;
            }
            catch
            {

                return text;
            }


        }

        //[Route("UpdateContact")]
        //[HttpGet]
        //public int UpdateContact(string phoneNumber, int OldContcatId, int tenantId)
        //{


        //    //DB
        //    var con = GetContact(OldContcatId);
        //    //var con = _contactsAppService.GetContactbyId(OldContcatId);

        //    if (con != null)
        //    {
        //        var userIdDB = tenantId + "_" + con.PhoneNumber;
        //        var check = GetContactWithUserID(tenantId).Where(x => x.UserId == userIdDB).FirstOrDefault();
        //        if (check == null)
        //        {
        //            con.TenantId = tenantId;
        //            con.UserId = tenantId + "_" + con.PhoneNumber;
        //            var idcont = InsertContact(con);
        //             //_contactsAppService.CreateContact(con);
        //            //cosmodeb
        //            CosmoDbUpdate(phoneNumber, tenantId, idcont);
        //            return idcont;
        //        }
        //        return check.Id;

        //    }
        //    return 0;


        //}


        [Route("ChickDate")]
        [HttpGet]
        public bool ChickDate()
        {
            var timeNow = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture);


            if (int.Parse(timeNow.Split(":")[0]) >= 20)
            {
                return true;

            }
            else
            {
                return false;
            }


        }


        [Route("UpdateMaintenance")]
        [HttpPost]
        public async Task UpdateMaintenanceAsync([FromBody] BotAPI.Models.BotModel.CreateMaintenanceModel model)
        {
            var man = CreateMaintenance(model);
            await _maintenanceshub.Clients.All.SendAsync("MaintenancesBotOrder", man);
        }

        [Route("UpdateUserToken")]
        [HttpPost]
        public void UpdateUserToken([FromBody] UserTokenModel userTokenModel)
        {
            _iUserAppService.UpdateUserToken(userTokenModel);


        }


        #region private
        private void Add(DeliveryOrderDetailsDto deliveryLocationCost)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO DeliveryOrderDetails (FromLocationId , ToLocationId, TenantId, DeliveryCost, FromAddress, FromGoogleURL, ToAddress, ToGoogleURL, DeliveryCostString, OrderId) VALUES (@FromLocationId ,@ToLocationId, @TenantId, @DeliveryCost, @FromAddress, @FromGoogleURL, @ToAddress, @ToGoogleURL, @DeliveryCostString, @OrderId) ";

                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromLocationId);
                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToLocationId);
                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);

                        command.Parameters.AddWithValue("@FromAddress", deliveryLocationCost.FromAddress);
                        command.Parameters.AddWithValue("@FromGoogleURL", deliveryLocationCost.FromGoogleURL);
                        command.Parameters.AddWithValue("@ToAddress", deliveryLocationCost.ToAddress);
                        command.Parameters.AddWithValue("@ToGoogleURL", deliveryLocationCost.ToGoogleURL);
                        command.Parameters.AddWithValue("@DeliveryCostString", deliveryLocationCost.DeliveryCostString);
                        command.Parameters.AddWithValue("@OrderId", deliveryLocationCost.OrderId);


                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception)
                {


                }

        }
        private List<Area> GetAreas(int TenantID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where TenantID=" + TenantID;

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
        private string GetWorkingHour(string _AreaSettingJson)
        {

            // var List = new List<string>();

            string x = "";
            if (_AreaSettingJson != null && _AreaSettingJson != "")
            {
                Web.Models.Sunshine.WorkModelNBot workmodel = JsonConvert.DeserializeObject<Web.Models.Sunshine.WorkModelNBot>(_AreaSettingJson);

                if (workmodel.WorkTextAR != "" && workmodel.WorkTextEN!="")
                {
                    x = "[ " + workmodel.WorkTextAR + " /" + workmodel.WorkTextEN + " ]";
                }

                if (workmodel.WorkTextFri != "" && workmodel.IsWorkActiveFri)
                {
                    x = x + " -" + workmodel.WorkTextFri;
                }
                if (workmodel.WorkTextSat != "" && workmodel.IsWorkActiveSat)
                {

                    x = x +" -"+ workmodel.WorkTextSat;
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
        private Area GetAreasID(int ID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where Id=" + ID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Area branches = new Area();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {

                    branches = new Area
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
                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString(),
                        UserIds = dataSet.Tables[0].Rows[i]["UserIds"].ToString(),
                        SettingJson = dataSet.Tables[0].Rows[i]["SettingJson"].ToString(),
                    };
                }
                catch
                {
                    branches = new Area
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
                        UserIds = dataSet.Tables[0].Rows[i]["UserIds"].ToString(),


                    };

                }

            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        private void CosmoDbUpdate(string phoneNumber, int tenantId, int contactID)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == "22_" + phoneNumber).Result;//&& a.TenantId== TenantId

            if (customerResult != null)
            {

                var userIdCosmo = tenantId + "_" + customerResult.phoneNumber;

                var check = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userIdCosmo).Result;

                if (check == null)
                {
                    var customerChat = UpdateCustomerChatD360(tenantId, customerResult.phoneNumber, customerResult.customerChat.text, customerResult.customerChat.type, customerResult.customerChat.mediaUrl, 1);
                    customerResult.customerChat = customerChat;

                    var CustomerModel = new CustomerModel()
                    {
                        ContactID = contactID.ToString(),////
                        IsComplaint = customerResult.IsComplaint,
                        userId = userIdCosmo,
                        displayName = customerResult.displayName,
                        avatarUrl = customerResult.avatarUrl,
                        type = customerResult.type,
                        D360Key = customerResult.D360Key,
                        CreateDate = customerResult.CreateDate,
                        IsLockedByAgent = customerResult.IsLockedByAgent,
                        LockedByAgentName = customerResult.LockedByAgentName,
                        IsOpen = customerResult.IsOpen,
                        agentId = customerResult.agentId,
                        IsBlock = customerResult.IsBlock,
                        IsConversationExpired = customerResult.IsConversationExpired,
                        CustomerChatStatusID = customerResult.CustomerChatStatusID,
                        CustomerStatusID = customerResult.CustomerStatusID,
                        LastMessageData = customerResult.LastMessageData,
                        IsNew = customerResult.IsNew,
                        TenantId = tenantId,
                        phoneNumber = customerResult.phoneNumber,
                        UnreadMessagesCount = customerResult.UnreadMessagesCount,
                        IsNewContact = customerResult.IsNewContact,
                        IsBotChat = customerResult.IsBotChat,
                        IsBotCloseChat = customerResult.IsBotCloseChat,
                        loyalityPoint = customerResult.loyalityPoint,
                        TotalOrder = customerResult.TotalOrder,
                        TakeAwayOrder = customerResult.TakeAwayOrder,
                        DeliveryOrder = customerResult.DeliveryOrder,
                    };

                    var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
                }



            }
        }
        //private int InsertContact(Contact contact)
        //{


        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    int modified = 0;
        //    string connString = AppSettingsModel.ConnectionStrings;
        //    //return orderCount.Count().ToString();
        //    using (SqlConnection connection = new SqlConnection(connString))
        //    using (SqlCommand command = connection.CreateCommand())
        //    {


        //        command.CommandText = "INSERT INTO Contacts (TenantId, AvatarUrl, DisplayName, PhoneNumber, SunshineAppID, IsLockedByAgent, LockedByAgentName, IsOpen,Website,EmailAddress,Description,ChatStatuseId,ContactStatuseId,CreationTime,CreatorUserId,DeleterUserId,DeletionTime,IsDeleted,LastModificationTime,LastModifierUserId,UserId,IsConversationExpired,IsBlock) " +
        //            " VALUES (@TenantId, @AvatarUrl, @DisplayName, @PhoneNumber, @SunshineAppID, @IsLockedByAgent, @LockedByAgentName, @IsOpen , @Website, @EmailAddress, @Description, @ChatStatuseId, @ContactStatuseId, @CreationTime, @CreatorUserId, @DeleterUserId, @DeletionTime, @IsDeleted, @LastModificationTime, @LastModifierUserId, @UserId, @IsConversationExpired, @IsBlock) ;SELECT SCOPE_IDENTITY();";

        //        command.Parameters.AddWithValue("@TenantId", contact.TenantId);
        //        command.Parameters.AddWithValue("@AvatarUrl", contact.AvatarUrl ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@DisplayName", contact.DisplayName);
        //        command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
        //        command.Parameters.AddWithValue("@SunshineAppID", "");
        //        command.Parameters.AddWithValue("@IsLockedByAgent", contact.IsLockedByAgent);
        //        command.Parameters.AddWithValue("@LockedByAgentName", contact.LockedByAgentName ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@IsOpen", contact.IsOpen);
        //        command.Parameters.AddWithValue("@Website", contact.Website ?? Convert.DBNull);

        //        command.Parameters.AddWithValue("@EmailAddress", contact.EmailAddress ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@Description", contact.Description ?? Convert.DBNull);
        //        command.Parameters.AddWithValue("@ChatStatuseId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@ContactStatuseId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@CreationTime", contact.CreationTime);
        //        command.Parameters.AddWithValue("@CreatorUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@DeleterUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@DeletionTime", Convert.DBNull);
        //        command.Parameters.AddWithValue("@IsDeleted", contact.IsDeleted);

        //        command.Parameters.AddWithValue("@LastModificationTime", Convert.DBNull);
        //        command.Parameters.AddWithValue("@LastModifierUserId", Convert.DBNull);
        //        command.Parameters.AddWithValue("@UserId", contact.UserId);
        //        command.Parameters.AddWithValue("@IsConversationExpired", contact.IsConversationExpired);
        //        command.Parameters.AddWithValue("@IsBlock", contact.IsBlock);

        //        connection.Open();
        //        modified = Convert.ToInt32(command.ExecuteScalar());
        //        if (connection.State == System.Data.ConnectionState.Open) connection.Close();


        //        return modified;

        //    }


        //}
        private CustomerChat UpdateCustomerChatD360(int TenantId, string phonenumber, string textt, string typee, string mediaUrll, int count)
        {
            try
            {


                CustomerChat CustomerChat = new CustomerChat();
                string userId = TenantId + "_" + phonenumber;
                string text = textt;
                string type = typee;
                string mediaUrl = mediaUrll;
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

                if (type == "location")
                {

                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        TenantId = TenantId,
                        userId = userId,
                        text = "https://maps.google.com/?q=" + text,
                        type = "location",//type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.Customer,
                        mediaUrl = mediaUrl,
                        UnreadMessagesCount = count
                    };
                }
                else
                {
                    // Create your new conversation instance
                    CustomerChat = new CustomerChat()
                    {
                        TenantId = TenantId,
                        userId = userId,
                        text = text,
                        type = type,
                        CreateDate = DateTime.Now,
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.Customer,
                        mediaUrl = mediaUrl,
                        UnreadMessagesCount = count
                    };

                }




                var Result = itemsCollection.CreateItemAsync(CustomerChat).Result;
                return CustomerChat;
            }
            catch (Exception)
            {

                throw;
            }

        }
        private async Task SendNotificationsAsync(string pns, string to_tag, string title, string body)
        {
            var user = "admin";// HttpContext.User.Identity.Name;
            string[] userTag = new string[1];
            userTag[0] = to_tag;
            // userTag[1] = "from:" + user;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;
            try
            {
                switch (pns.ToLower())
                {
                    case "wns":
                        // Windows 8.1 / Windows Phone 8.1
                        var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                    "From " + user + ": " + "message" + "</text></binding></visual></toast>";
                        outcome = await BotAPI.Models.Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                        break;
                    case "apns":
                        // iOS
                        var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + "message" + "\"}}";
                        outcome = await BotAPI.Models.Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                        break;
                    case "fcm":
                        // Android
                        SendNotfificationModel sendNotfificationModel = new SendNotfificationModel
                        {
                            notification = new SendNotfificationModel.Notification
                            {
                                title = title,
                                body = body
                            },
                            data = new SendNotfificationModel.Data
                            {
                                property1 = "value1",
                                property2 = 42
                            }

                        };
                        var notif = JsonConvert.SerializeObject(sendNotfificationModel).ToString();
                        // var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + "message" + "\"}}";
                        outcome = await BotAPI.Models.Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
                        break;
                }
            }
            catch (Exception)
            {


            }


            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

        }


        private void SendMobileNotification(int TenaentId, string title, string msg, bool islivechat = false, string userIds = null, bool isCancelOrder = false)
        {
            
            var tenant = GetTenantById(TenaentId).Result;
            if (tenant.IsBellOn)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
                httpWebRequest.Method = "POST";


                var tokenuser = GetUserByTeneantId(TenaentId, userIds);

                string mainSoundIOS = string.Empty;
                string mainSoundAndroid = string.Empty;

                if (isCancelOrder)
                {
                    mainSoundIOS = "cancel.caf";
                    mainSoundAndroid = "cancel";

                    if (tenant.IsBellContinues)
                    {
                        mainSoundIOS = "cancel_con.caf";
                        mainSoundAndroid = "cancel_con";
                    }
                }
                else
                {
                    mainSoundIOS = "sound.caf";
                    mainSoundAndroid = "sound";
                    if (tenant.IsBellContinues)
                    {
                        mainSoundIOS = "sound_con.caf";
                        mainSoundAndroid = "sound_con";
                    }
                }

                if (tokenuser.Count > 0)
                {
                    var payload = new
                    {
                        registration_ids = tokenuser,
                        data = new
                        {

                            body = msg,
                            title = title,
                            sound = mainSoundAndroid,
                            apple = new
                            {
                                sound = mainSoundIOS
                            }
                        },
                        priority = "high",
                        payload = new
                        {
                            aps = new
                            {
                                sound = mainSoundIOS
                            }
                        },
                        android = new
                        {
                            notification = new
                            {
                                channel_id = $"high_importance_channel_{mainSoundAndroid}"
                            }
                        },
                        apns = new
                        {

                            header = new
                            {
                                priority = "10"
                            },
                            payload = new
                            {
                                aps = new
                                {
                                    sound = mainSoundIOS
                                }
                            }
                        }
                    ,
                        notification = new
                        {
                            body = msg,
                            content_available = true,
                            priority = "high",
                            title = title,
                            sound = mainSoundAndroid,
                            apns = new
                            {
                                payload = new
                                {
                                    aps = new
                                    {
                                        sound = mainSoundIOS
                                    }
                                }
                            }
                        },
                    };
                    var serializer = new JavaScriptSerializer();
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = serializer.Serialize(payload);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }


                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    var streamReader = new StreamReader(httpResponse.GetResponseStream());
                    var result = streamReader.ReadToEnd();
                }
            }
        }

        private void SendMobileCancelNotification(int? TenaentId, string title, string msg)
        {

            try
            {

                var result = "-1";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
                httpWebRequest.Method = "POST";

                //var payload = new
                //{
                //    registration_ids = GetUserByTeneantId((int)TenaentId),
                //    // to = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync("iyuiyu").Result,
                //    priority = "high",
                //    content_available = true,
                //    notification = new
                //    {
                //        body = msg,
                //        title = title,
                //        sound = "sound"
                //    },
                //};
                var payload = new
                {
                    registration_ids = GetUserByTeneantId((int)TenaentId),
                    data = new
                    {

                        body = msg,
                        title = title,
                        sound = "cancel.caf",
                        apple = new
                        {
                            sound = "cancel.caf"
                        }
                    },
                    priority = "high",
                    payload = new
                    {
                        aps = new
                        {
                            sound = "cancel.caf"
                        }
                    },
                    android = new
                    {
                        notification = new
                        {
                            channel_id = "high_importance_channel_cancel"
                        }
                    },
                    apns = new
                    {
                        //alert = title + " , " + msg,
                        //sound = "cancel.caf",

                        header = new
                        {
                            priority = "10"
                        },


                        payload = new
                        {
                            aps = new
                            {
                                sound = "cancel.caf"
                            }
                        }
                    }
        ,
                    notification = new
                    {
                        body = msg,
                        content_available = true,
                        priority = "high",
                        title = title,
                        sound = "cancel.caf",
                        apns = new
                        {
                            //alert = title + " , " + msg,
                            //sound = "cancel.caf",

                            //apple = new
                            //{
                            //    sound = "cancel.caf"
                            //},
                            payload = new
                            {
                                aps = new
                                {
                                    sound = "cancel.caf"
                                }
                            }
                        }
                    },




                };
                //var payload = new
                //{
                //    registration_ids = GetUserByTeneantId((int)TenaentId),
                //    data = new
                //    {

                //        body = msg,
                //        title = title,
                //        sound = "cancel"
                //    },
                //    priority = "high",
                //    payload = new
                //    {
                //        aps = new
                //        {
                //            sound = "cancel"
                //        }
                //    },
                //    android = new
                //    {
                //        notification = new
                //        {
                //            channel_id = "high_importance_channel_cancel"
                //        }
                //    },
                //    aps = new
                //    {
                //        alert = title + " , " + msg,
                //        sound = "cancel.caf"
                //    }
                //    ,
                //    notification = new
                //    {
                //        body = msg,
                //        content_available = true,
                //        priority = "high",
                //        title = title,
                //        sound = "cancel.caf"
                //    }

                //};

                var serializer = new JavaScriptSerializer();
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = serializer.Serialize(payload);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch
            {


            }
          

        }
        private void UpdateTextLocation(BotAPI.Models.Location.LocationInfoModel location)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {


                command.CommandText = "UPDATE Locations SET LocationNameEn = @LocationNameEn  Where Id = @Id";

                command.Parameters.AddWithValue("@Id", location.Id);
                command.Parameters.AddWithValue("@LocationNameEn", location.LocationNameEn);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void UpdateDeliveryOrderDB(UpdateOrderModel updateOrderModel, DateTime timeAdd, string connString, long number, string BranchAreaName, Area area)
        {
            try
            {
                decimal totalWithBranchCost = 0;
                decimal deleverfees = 0;

                if (updateOrderModel.BuyType == "No select")
                    updateOrderModel.BuyType = "";

                if (updateOrderModel.isOrderOfferCost)
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostBefor);
                    deleverfees = updateOrderModel.DeliveryCostBefor;
                }
                else
                {
                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostAfter);
                    deleverfees = updateOrderModel.DeliveryCostAfter;
                }
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    //valdet if the order exist or not 
                    command.CommandText = "UPDATE Orders SET OrderTime = @OrT, OrderStatus = @OrI ,CreationTime =@CreT ,OrderType = @OrTy, Total= @Tot ,Address=@Add ,DeliveryCost=@Dcos ,IsEvaluation=@IsEv ,BranchAreaId=@BranchAreaId , AreaId = @AreaId ,BranchAreaName=@BranchAreaName, OrderNumber =@OrderNumber ,LocationID=@LocationID ,FromLocationDescribation=@FromLocationDescribation, AfterDeliveryCost=@AfterDeliveryCost , SelectDay=@SelectDay, SelectTime=@SelectTime, IsPreOrder=@IsPreOrder, RestaurantName=@RestaurantName , IsSpecialRequest=@IsSpecialRequest,SpecialRequestText=@SpecialRequestText , BuyType=@BuyType ,OrderLocal=@OrderLocal ,AgentId=@AgentId,AgentIds=@AgentIds,StreetName=@StreetName,BuildingNumber=@BuildingNumber,FloorNo=@FloorNo,ApartmentNumber=@ApartmentNumber  Where Id = @Id";
                    command.Parameters.AddWithValue("@Id", updateOrderModel.OrderId);
                    command.Parameters.AddWithValue("@OrT", timeAdd);
                    command.Parameters.AddWithValue("@CreT", timeAdd);
                    command.Parameters.AddWithValue("@Tot", totalWithBranchCost);
                    command.Parameters.AddWithValue("@Add", updateOrderModel.Address);
                    command.Parameters.AddWithValue("@Dcos", deleverfees);
                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Delivery);

                    command.Parameters.AddWithValue("@OrderLocal", updateOrderModel.BotLocal);

                    command.Parameters.AddWithValue("@BuyType", updateOrderModel.BuyType);

                    if (updateOrderModel.IsPreOrder)
                    {
                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pre_Order);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
                    }



                    if (area != null && area.Id != 0)
                    {
                        command.Parameters.AddWithValue("@BranchAreaId", area.Id);
                        command.Parameters.AddWithValue("@AreaId", area.Id);
                        if (area.UserId != 0 && area.UserId != null)
                        {
                            command.Parameters.AddWithValue("@AgentId", area.UserId);

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AgentId", 0);
                        }
                        if (!string.IsNullOrEmpty(area.UserIds))
                        { command.Parameters.AddWithValue("@AgentIds", area.UserIds); }
                        else
                        {
                            command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
                        }

                    }
                    else
                    {

                        command.Parameters.AddWithValue("@AreaId", DBNull.Value);
                        command.Parameters.AddWithValue("@AgentId", 0);
                        command.Parameters.AddWithValue("@AgentIds", DBNull.Value);

                        command.Parameters.AddWithValue("@BranchAreaId", 0);
                    }




                    command.Parameters.AddWithValue("@IsEv", false);


                    command.Parameters.AddWithValue("@BranchAreaName", BranchAreaName);// orderBotData.BranchName);//نفيسة
                    command.Parameters.AddWithValue("@RestaurantName", updateOrderModel.BranchName);//دوار المدينة

                    command.Parameters.AddWithValue("@LocationID", updateOrderModel.BranchId);




                    command.Parameters.AddWithValue("@FromLocationDescribation", "https://maps.google.com/?q=" + updateOrderModel.LocationFrom.Replace(" ", ""));
                    command.Parameters.AddWithValue("@OrderNumber", number);

                    command.Parameters.AddWithValue("@AfterDeliveryCost", updateOrderModel.DeliveryCostAfter);

                    command.Parameters.AddWithValue("@SelectDay", updateOrderModel.SelectDay);
                    command.Parameters.AddWithValue("@SelectTime", updateOrderModel.SelectTime);
                    command.Parameters.AddWithValue("@IsPreOrder", updateOrderModel.IsPreOrder);

                    //  command.Parameters.AddWithValue("@HtmlPrint", htmlOrderD);



                    command.Parameters.AddWithValue("@IsSpecialRequest", updateOrderModel.IsSpecialRequest);
                    command.Parameters.AddWithValue("@SpecialRequestText", updateOrderModel.SpecialRequest);




                    if (updateOrderModel.StreetName != null)
                    {
                        command.Parameters.AddWithValue("@StreetName", updateOrderModel.StreetName);
                        command.Parameters.AddWithValue("@BuildingNumber", updateOrderModel.BuildingNumber);
                        command.Parameters.AddWithValue("@FloorNo", updateOrderModel.FloorNo);
                        command.Parameters.AddWithValue("@ApartmentNumber", updateOrderModel.ApartmentNumber);



                    }
                    else
                    {
                        command.Parameters.AddWithValue("@StreetName", DBNull.Value);
                        command.Parameters.AddWithValue("@BuildingNumber", DBNull.Value);
                        command.Parameters.AddWithValue("@FloorNo", DBNull.Value);
                        command.Parameters.AddWithValue("@ApartmentNumber", DBNull.Value);
                    }


                    connection.Open();
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception)
            {


            }

        }
        private void updateOrderDB(UpdateOrderModel updateOrderModel, DateTime timeAdd, string connString, long number, Area area)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {


                    command.CommandText = "UPDATE Orders SET Total = @Total , OrderTime = @OrT ,CreationTime =@CreT , OrderStatus = @OrI , OrderType = @OrTy ,AreaId = @ArI ,IsEvaluation=@IsEv , OrderNumber =@OrderNumber  ,IsSpecialRequest=@IsSpecialRequest,SpecialRequestText=@SpecialRequestText , OrderLocal=@OrderLocal ,AgentId=@AgentId, AgentIds=@AgentIds ,BranchAreaId=@BranchAreaId Where Id = @Id";
                    command.Parameters.AddWithValue("@OrderLocal", updateOrderModel.BotLocal);
                    command.Parameters.AddWithValue("@Id", updateOrderModel.OrderId);
                    command.Parameters.AddWithValue("@OrT", timeAdd);
                    command.Parameters.AddWithValue("@CreT", timeAdd);
                    command.Parameters.AddWithValue("@ArI", updateOrderModel.BranchId);
                    command.Parameters.AddWithValue("@SpecialRequestText", updateOrderModel.SpecialRequest);
                    command.Parameters.AddWithValue("@IsSpecialRequest", updateOrderModel.IsSpecialRequest);
                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Takeaway);
                    command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
                    command.Parameters.AddWithValue("@IsEv", false);
                    command.Parameters.AddWithValue("@OrderNumber", number);


                    if (area != null)
                    {
                        command.Parameters.AddWithValue("@BranchAreaId", area.Id);
                        if (area.UserId != 0 && area.UserId != null)
                        {
                            command.Parameters.AddWithValue("@AgentId", area.UserId);

                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AgentId", 0);
                        }
                        if (!string.IsNullOrEmpty(area.UserIds))
                            command.Parameters.AddWithValue("@AgentIds", area.UserIds);
                        else
                        {
                            command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
                    }



                    command.Parameters.AddWithValue("@Total", updateOrderModel.OrderTotal);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }

        }





        private List<Infoseed.MessagingPortal.Orders.Order> GetOrderListWithContact(int? TenantID, int ContactId, string OrderNumber)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and ContactId=" + ContactId +" and OrderNumber =" +OrderNumber +" and OrderStatus <> 4";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Infoseed.MessagingPortal.Orders.Order> order = new List<Infoseed.MessagingPortal.Orders.Order>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {

                    try
                    {

                        order.Add(new Infoseed.MessagingPortal.Orders.Order
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                            AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString(),
                            AreaId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            TotalPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalPoints"].ToString()),
                            ///AfterDeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["AfterDeliveryCost"].ToString()),
                            DeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                            OrderType=(OrderTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderType"]),

                        });

                    }
                    catch
                    {

                        order.Add(new Infoseed.MessagingPortal.Orders.Order
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString(),
                            AreaId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
                            TotalPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalPoints"].ToString()),
                            //AfterDeliveryCost= decimal.Parse(dataSet.Tables[0].Rows[i]["AfterDeliveryCost"].ToString()),
                            OrderType=(OrderTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderType"]),
                            DeliveryCost= 0,
                        });

                    }


                }

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private void UpdateOrderAfterCancel(string OrderNumber, int ContactId, Infoseed.MessagingPortal.Orders.Order OrderModel, int? TenantID)
        {
            var con = _generalAppService.GetContactbyId(ContactId);

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Orders SET  OrderStatus = @OrI, OrderRemarks=@Rema , ActionTime = @ActionTime   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", OrderModel.Id);
                command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Canceled);
                command.Parameters.AddWithValue("@Rema", "CancelByCustomer");
                command.Parameters.AddWithValue("@ActionTime", DateTime.UtcNow);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            OrderModel.OrderNumber = long.Parse(OrderNumber);
            OrderModel.ContactId = ContactId;
            OrderModel.orderStatus = OrderStatusEunm.Canceled;
            OrderModel.ActionTime =  DateTime.UtcNow;

            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), OrderModel.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), OrderModel.OrderType);
            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            var GetOrderMap = _mapper.Map(OrderModel, getOrderForViewDto.Order);

            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.OrderStatusName = orderStatusName;
            getOrderForViewDto.AgentIds = OrderModel.AgentIds;
            getOrderForViewDto.OrderTypeName = orderTypeName;

            getOrderForViewDto.CustomerCustomerName = con.DisplayName;
            getOrderForViewDto.TenantId = TenantID;
            getOrderForViewDto.DeliveryChangeDeliveryServiceProvider = OrderModel.CreationTime.ToString("MM/dd hh:mm tt");


            getOrderForViewDto.IsAssginToAllUser = true;


            try
            {
                var area = GetAreaId(int.Parse(OrderModel.AreaId.ToString()));

                if (area != null)
                {
                    getOrderForViewDto.AreahName = area.AreaName;
                }

            }
            catch (Exception)
            {

                getOrderForViewDto.AreahName = "";
            }
            SocketIOManager.SendOrder(getOrderForViewDto, TenantID.HasValue ? TenantID.Value : 0);
            var titl = "The Order Number: " + OrderNumber;
            var body = "Order Status :" + OrderStatusEunm.Canceled;

            SendMobileNotification(TenantID.Value, titl, body,false,null,true);

        }


        private Infoseed.MessagingPortal.Orders.Order GetOrder(long orderId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Orders] where Id=" + orderId;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Infoseed.MessagingPortal.Orders.Order order = new Infoseed.MessagingPortal.Orders.Order();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                if (!IsDeleted)
                {

                    order = new Infoseed.MessagingPortal.Orders.Order
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        TotalPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalPoints"].ToString()),
                        TotalCreditPoints = decimal.Parse(dataSet.Tables[0].Rows[i]["TotalCreditPoints"].ToString()),


                    };

                }

            }

            conn.Close();
            da.Dispose();

            return order;

        }
        private List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[OrderDetails] where TenantID=" + TenantID + " and OrderId=" + OrderId;




            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<OrderDetailDto> orderDetailDtos = new List<OrderDetailDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new OrderDetailDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private List<ExtraOrderDetailsDto> GetOrderDetailExtra(int? TenantID)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                orderDetailDtos.Add(new ExtraOrderDetailsDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



                });
            }

            conn.Close();
            da.Dispose();

            return orderDetailDtos;
        }
        private List<ItemDto> GetItem(int? TenantID, long id)
        {

            List<ItemDto> itemDtos = new List<ItemDto>();
            ItemDto item = _itemsAppService.GetItemInfoForBot(id, TenantID.Value);
            itemDtos.Add(item);
            return itemDtos;
        }

        private async Task<List<GetOrderDetailForViewDto>> GetAllDetail(int? TenantId, int orderId, string lang)
        {

            List<GetOrderDetailForViewDto> getOrderDetailForViewDto = new List<GetOrderDetailForViewDto>();
            var OrderDetail = GetOrderDetail(TenantId, orderId);

            //  var itemslist = GetItem(TenantId);

            foreach (var item in OrderDetail)
            {
                var items = GetItem(TenantId, long.Parse(item.ItemId.ToString())).FirstOrDefault();// itemslist.Where(x => x.Id == item.ItemId).FirstOrDefault();

                if (items != null)
                {
                    getOrderDetailForViewDto.Add(new GetOrderDetailForViewDto
                    {

                        OrderDetail = item,
                        ItemName = items.ItemName,
                        ItemNameEnglish = items.ItemNameEnglish
                    });
                }
                else
                {

                    //getOrderDetailForViewDto.Add(new GetOrderDetailForViewDto
                    //{

                    //    OrderDetail = new OrderDetailDto() {  },
                    //    ItemName = items.ItemName,
                    //    ItemNameEnglish = items.ItemNameEnglish
                    //});
                }




            }



            var exListList = GetOrderDetailExtra(TenantId);


            foreach (var item in getOrderDetailForViewDto)
            {

                var exList = exListList.Where(x => x.OrderDetailId == item.OrderDetail.Id).ToList();


                List<ExtraOrderDetailsDto> extraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
                foreach (var ext in exList)
                {

                    if (lang == "ar")
                    {
                        extraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                        {
                            Id = 0,
                            Name = ext.Name,
                            OrderDetailId = ext.OrderDetailId,
                            Quantity = ext.Quantity,
                            TenantId = ext.TenantId,
                            Total = ext.Total,
                            UnitPrice = ext.UnitPrice,

                        });
                    }
                    else
                    {
                        extraOrderDetailsDto.Add(new ExtraOrderDetailsDto
                        {
                            Id = 0,
                            Name = ext.NameEnglish,
                            OrderDetailId = ext.OrderDetailId,
                            Quantity = ext.Quantity,
                            TenantId = ext.TenantId,
                            Total = ext.Total,
                            UnitPrice = ext.UnitPrice,

                        });

                    }





                    // item.OrderDetail.extraOrderDetailsDtos.Add(extraOrderDetailsDto);




                }
                item.OrderDetail.extraOrderDetailsDtos = extraOrderDetailsDto;






            }




            return getOrderDetailForViewDto;

        }
        private string GetOrderDetailString(int TenantID, string lang, decimal? Total, decimal? TotalLoyaltyPoints, string captionQuantityText, string captionAddtionText, string captionTotalText, string captionTotalOfAllText, List<OrderDetailDto> OrderDetailList, List<ExtraOrderDetailsDto> getOrderDetailExtraList, decimal discount, bool isdiscount, string TypeChoes = "", string DeliveryCostText = "", decimal? Cost = 0)
        {

            //var captionQuantityText = GetCaptionFormat("BackEnd_Text_Quantity", lang, TenantID, "", "", 0);//العدد :
            //var captionAddtionText = GetCaptionFormat("BackEnd_Text_Addtion", lang, TenantID, "", "", 0);//الاضافات
            //var captionTotalText = GetCaptionFormat("BackEnd_Text_Total", lang, TenantID, "", "", 0);//المجموع:       
            //var captionTotalOfAllText = GetCaptionFormat("BackEnd_Text_TotalOfAll", lang, TenantID, "", "", 0);//السعر الكلي للصنف: 

            var captionTotalPointsOfAllText = "السعر الكلي لصنف بالنقاط :";
            var captionTotalPointsText = "المجموع بالنقاط :";
            if (lang == "ar")
            {
                captionTotalPointsOfAllText = "السعر الكلي لصنف بالنقاط :";
                captionTotalPointsText = "المجموع بالنقاط :";
            }
            else
            {
                captionTotalPointsOfAllText = "Price per item in points:";
                captionTotalPointsText = "Total in points:";
            }




            var listString = "-------------------------- \r\n";
            listString = listString + "-------------------------- \r\n\r\n";
            decimal? total = 0;

            foreach (var OrderD in OrderDetailList)
            {

                var getOrderDetailExtra = getOrderDetailExtraList.Where(x => x.OrderDetailId == OrderD.Id).ToList();
                var item = GetItem(TenantID, long.Parse(OrderD.ItemId.ToString())).FirstOrDefault();// itemList.Where(x => x.Id == OrderD.ItemId).FirstOrDefault();

                if (item != null)
                {
                    if (lang == "ar")
                    {
                        listString = listString + "*" + item.ItemName.Trim() + "*" + "\r\n";
                    }
                    else
                    {
                        listString = listString + "*" + item.ItemNameEnglish.Trim() + "*" + "\r\n";
                    }

                    if (getOrderDetailExtra.Count > 0)
                    {
                        listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

                    }
                }
                else
                {
                    if (lang == "ar")
                    {
                        if (OrderD.IsCrispy)
                        {
                            listString = listString + "*" + "كرسبي" + "*" + "\r\n";

                        }
                        else if (OrderD.IsDeserts)
                        {
                            listString = listString + "*" + "حلويات" + "*" + "\r\n";

                        }
                        else if (OrderD.IsCondiments)
                        {
                            listString = listString + "*" + "الصوص" + "*" + "\r\n";

                        }
                    }
                    else
                    {
                        if (OrderD.IsCrispy)
                        {
                            listString = listString + "*" + "Crispy" + "*" + "\r\n";

                        }
                        else if (OrderD.IsDeserts)
                        {
                            listString = listString + "*" + "Desserts" + "*" + "\r\n";

                        }
                        else if (OrderD.IsCondiments)
                        {
                            listString = listString + "*" + "Condiments" + "*" + "\r\n";
                        }

                    }

                    //if (getOrderDetailExtra.Count > 0)
                    //{
                    //    listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

                    //}

                }


                foreach (var ext in getOrderDetailExtra)
                {
                    if (ext.Quantity > 1)
                    {
                        if (lang == "ar")
                        {
                            listString = listString + ext.Name + "  (" + ext.Quantity + ")" + "\r\n";
                        }
                        else
                        {
                            listString = listString + ext.NameEnglish + "  (" + ext.Quantity + ")" + "\r\n";

                        }

                    }
                    else
                    {
                        if (lang == "ar")
                        {

                            listString = listString + ext.Name + "\r\n";
                        }
                        else
                        {
                            listString = listString + ext.NameEnglish + "\r\n";
                        }


                    }

                    //listString = listString + captionQuantityText + ext.Quantity + "\r\n";

                }




                listString = listString + "\r\n" + "*" + captionQuantityText + OrderD.Quantity + "*" + "\r\n";
                if (TenantID == 46)
                {
                    if (OrderD.TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsOfAllText + ((int)OrderD.TotalLoyaltyPoints).ToString() + "\r\n\r\n";
                    }

                    listString = listString + captionTotalOfAllText + ((int)OrderD.Total).ToString() + "\r\n\r\n";
                }
                else
                {

                    if (OrderD.TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsOfAllText + OrderD.TotalLoyaltyPoints + "\r\n\r\n";
                    }
                    listString = listString + captionTotalOfAllText + OrderD.Total + "\r\n\r\n";
                }



                total = total + OrderD.Total;// + Cost;

                listString = listString + "-------------------------- \r\n";
            }


            listString = listString + "-------------------------- \r\n\r\n";

            if (TypeChoes == "Delivery")
            {
                listString = listString + DeliveryCostText + "\r\n";

                listString = listString + "-------------------------- \r\n\r\n";
            }

            if (TypeChoes == "Delivery")
            {
                if (TenantID == 46)
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total + Cost).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + (Total + Cost).ToString() + "\r\n";
                }
            }
            else
            {
                if (TenantID == 46)
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints!=0)
                    {
                        listString = listString + captionTotalPointsText + TotalLoyaltyPoints.ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + Total.ToString() + "\r\n";
                }

            }



            listString = listString + "-------------------------- \r\n";
            if (isdiscount)
            {
                if (lang == "ar")
                {

                    listString = listString + "بعد خصم :" + discount + " %" + "\r\n\r\n";
                }
                else
                {
                    listString = listString + "After Discount :" + discount + " %" + "\r\n\r\n";
                }


            }

            return listString;
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

                            BranchesName= dataSet.Tables[0].Rows[i]["BranchesName"].ToString(),
                            BranchesIds= dataSet.Tables[0].Rows[i]["BranchesIds"].ToString(),


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
        private void OrderOfferFun(int tenantID, bool isOrderOffer, decimal OrderTotal, GetLocationInfoModel infoLocation, string ci, string ar, string dis, decimal costDistric)
        {
            if (isOrderOffer)
            {

                //ci=ci.Replace("(All)", "").Trim();
                //ar=ar.Replace("(All)", "").Trim();
                //dis=dis.Replace("(All)", "").Trim();




                var orderEffor = GetOrderOffer(tenantID);

                if (infoLocation.LocationAreaName == null)
                    infoLocation.LocationAreaName = "";

                var item = orderEffor.Where(x => (x.BranchesIds.Contains(infoLocation.LocationId.ToString())&& x.isPersentageDiscount == false));//.FirstOrDefault(); x.Area.Contains(ci) || x.Area.Contains(ar) || x.Area.Contains(dis) &&


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
        private void DeleteOrder(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM Orders   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteOrderDetails(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM OrderDetails   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        private void DeleteExtraOrderDetail(long? id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM ExtraOrderDetail   Where Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }




        private locationAddressModel GetLocation(string query)
        {
            try
            {
                var client = new HttpClient();
                string Key = _configuration["GoogleMapsKey:KeyMap"];
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key=" + Key;
                var response = client.GetAsync(url).Result;

                var result = response.Content.ReadAsStringAsync().Result;
                var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleMapModel>(result);



                ////
                locationAddressModel locationAddressModel = new locationAddressModel();
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
                locationAddressModel locationAddressModel = new locationAddressModel();

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


        private BotAPI.Models.Location.LocationInfoModel GetLocationInfoModelByName(string name)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Locations] where LocationNameEn LIKE N'"+name + "%'and LevelId = 2 ";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                BotAPI.Models.Location.LocationInfoModel locationInfoModel = new BotAPI.Models.Location.LocationInfoModel();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        locationInfoModel=new BotAPI.Models.Location.LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
                        };
                    }
                    catch
                    {
                        locationInfoModel=new BotAPI.Models.Location.LocationInfoModel
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
                        };

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

        private List<BotAPI.Models.Location.LocationInfoModel> GetAllLocationDeliveryCost(int TenantId)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[LocationDeliveryCost]  where TenantId=" + TenantId;


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

                    locationInfoModel.Add(new BotAPI.Models.Location.LocationInfoModel
                    {
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        LocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LocationId"]),
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                        BranchAreaId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"]),

                    });
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
        private long UpateOrder(int? tenantId)
        {
            string connString = AppSettingsModel.ConnectionStrings;

            //AbpSession.TenantId
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                            new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                     };

            System.Data.SqlClient.SqlParameter output = new System.Data.SqlClient.SqlParameter();

            output.ParameterName = "@CurrentOrderNumber";
            output.DbType = DbType.Int64;
            output.Direction = ParameterDirection.Output;
            sqlParameters.Add(output);



            var result = SqlDataHelper.ExecuteNoneQuery(
                       "dbo.GetCurrentOrderNumber",
                       sqlParameters.ToArray(),
                       connString);

            return Int64.Parse(output.Value.ToString());
        }
        private string GetHtmlD(UpdateOrderModel orderBotData, DateTime timeAdd, long number, Contact con, string BranchAreaName, string local)
        {
            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.OrderId, local).Result;
            var header = "";
            if (orderBotData.IsPreOrder)
            {
                header = "<div > <strong  style='font-size: large;'> Name : </strong>" + con.DisplayName + "</div>"
             + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
             + "<div > <strong style='font-size: large;' > Address : </strong>" + orderBotData.Address + "</div>"
             + "<div > <strong  style='font-size: large;' > Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
             + "<div >  <strong  style='font-size: large;' > Type : </strong>" + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Delivery) + "/  " + orderBotData.SelectTime + "-" + orderBotData.SelectDay + "</div>"
             + "<div > <strong style='font-size: large;'> Branch : </strong>" + orderBotData.BranchName + "/" + BranchAreaName + "</div>"
             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
             + "<hr  style='border-top: dotted 1px #000 !important;'>";

            }
            else
            {
                header = "<div > <strong  style='font-size: large;'> Name : </strong>" + con.DisplayName + "</div>"
             + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
             + "<div > <strong style='font-size: large;' > Address : </strong>" + orderBotData.Address + "</div>"
             + "<div > <strong  style='font-size: large;' > Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
             + "<div > <strong style='font-size: large;'> Branch : </strong>" + orderBotData.BranchName + "/" + BranchAreaName + "</div>"
             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
             + "<hr  style='border-top: dotted 1px #000 !important;'>";
            }


            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";


            foreach (var record in orderDetailsList)
            {

                if (local == "ar")
                {
                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName + "</strong></td>";
                }
                else
                {
                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemNameEnglish + "</strong></td>";
                }


                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Quantity + "</strong></td>";
                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Total + "</strong></td> </tr>";

                var extraOrderDetails = "";
                foreach (var group in record.OrderDetail.extraOrderDetailsDtos)
                {
                    extraOrderDetails = extraOrderDetails + "<tr ><td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Name + "</td>";
                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Quantity + "</td>";
                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller;'> " + group.Total + "</td> </tr>";

                }


                orderDetailsbody = orderDetailsbody + extraOrderDetails;




            }
            orderDetailsbody = orderDetailsbody + "</tbody></table> </div> <hr  style='border-top: dotted 1px #000 !important;'>  <hr  style='border-top: dotted 1px #000 !important;'>";

            // var xxx = (Math.Round(orderBotData.AfterBranchCost * 100) / 100).toFixed(2);

            orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> Delivery fees   : </strong> <strong style='font-size: medium;'>" + orderBotData.DeliveryCostAfter + " </strong> </div>";

            if (orderBotData.isOrderOfferCost)
            {
                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> instead of   : </strong> <strong style='font-size: medium;'>" + orderBotData.DeliveryCostBefor + " </strong> </div>";
            }


            if (orderBotData.IsSpecialRequest)
            {
                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium;'  > <strong style='font-size: x-large;'> Note   : </strong> <strong style='font-size: medium;'>" + orderBotData.SpecialRequest + " </strong> </div>";
            }

            if (orderBotData.isOrderOfferCost)
            {
                orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + ((orderBotData.OrderTotal + orderBotData.DeliveryCostBefor)) + " </p> </div>";
            }
            else
            {
                orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + ((orderBotData.OrderTotal + orderBotData.DeliveryCostAfter)) + " </p> </div>";
            }


            var htmlOrderD = header + orderDetailsbody;
            return htmlOrderD;
        }
        private string GetHtmlPrint(UpdateOrderModel orderBotData, DateTime timeAdd, long number, Contact con, Area area, string local)
        {
            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.OrderId, local).Result;

            var header = "<div > <strong  style='font-size:large;'> Name : </strong>" + con.DisplayName + "</div>"
               + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
               + "<div > <strong  style='font-size: large;'> Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
               + "<div >  <strong  style='font-size: large;'> Type : </strong>" + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Takeaway) + "</div>"
               + "<div > <strong  style='font-size: large;'> Branch : </strong>" + area.AreaName + "</div>"
               + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
               + "<hr  style='border-top: dotted 1px #000 !important;'><hr  style='border-top: dotted 1px #000 !important;'>";

            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";



            foreach (var record in orderDetailsList)
            {
                if (local == "ar")
                {
                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName + "</strong></td>";
                }
                else
                {
                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemNameEnglish + "</strong></td>";
                }



                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Quantity + "</strong></td>";
                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Total + "</strong></td> </tr>";

                var extraOrderDetails = "";
                foreach (var group in record.OrderDetail.extraOrderDetailsDtos)
                {
                    extraOrderDetails = extraOrderDetails + "<tr ><td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Name + "</td>";
                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Quantity + "</td>";
                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller;'> " + group.Total + "</td> </tr>";

                }


                orderDetailsbody = orderDetailsbody + extraOrderDetails;




            }
            orderDetailsbody = orderDetailsbody + "</tbody></table> </div> <hr  style='border-top: dotted 1px #000 !important;'>  <hr  style='border-top: dotted 1px #000 !important;'>";

            if (orderBotData.SpecialRequest != "NULLNOT" || orderBotData.SpecialRequest != "NULLORDER")
            {
                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium;'  > <strong style='font-size: x-large;'> Note   : </strong> <strong style='font-size: medium;'>" + orderBotData.SpecialRequest + " </strong> </div>";
            }
            orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + orderBotData.OrderTotal + " </p> </div>";

            var htmlOrder = header + orderDetailsbody;
            return htmlOrder;
        }
        private Contact GetContact(int id)
        {

            try
            {
                Contact contact = new Contact();

                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Contacts] where Id=" + id;


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
                        contact = new Contact
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),
                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
                            ContactDisplayName = dataSet.Tables[0].Rows[i]["ContactDisplayName"].ToString(),
                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
                            StreetName = dataSet.Tables[0].Rows[i]["StreetName"].ToString(),
                            BuildingNumber = dataSet.Tables[0].Rows[i]["BuildingNumber"].ToString(),
                            FloorNo = dataSet.Tables[0].Rows[i]["FloorNo"].ToString(),
                            ApartmentNumber = dataSet.Tables[0].Rows[i]["ApartmentNumber"].ToString(),
                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]) : 0,

                        };
                    }
                    catch
                    {
                        contact = new Contact
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]) : 0,
                        };
                    }

                }

                conn.Close();
                da.Dispose();

                return contact;

            }
            catch
            {
                return null;

            }

        }
        private Area GetAreaId(int id)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Areas] where Id=" + id;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Area branches = new Area();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                branches = new Area
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
                    UserIds = dataSet.Tables[0].Rows[i]["UserIds"].ToString(),
                    IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
                };
            }

            conn.Close();
            da.Dispose();

            return branches;
        }
        private List<Contact> GetContactWithUserID(int teantID)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Contacts] where TenantId=" + teantID;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<Contact> contact = new List<Contact>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    contact.Add(new Contact
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                        PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

                        Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
                        EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
                        Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

                        DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
                        TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
                        TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
                        DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
                        loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"])

                    });
                }

                conn.Close();
                da.Dispose();

                return contact;

            }
            catch (Exception)
            {
                return null;

            }

        }

        private string GetAssetLevelTwoName(int? Id)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[AssetLevelTwo] where Id=" + Id;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                string contact = "";

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    contact = dataSet.Tables[0].Rows[i]["LevelTwoNameEn"].ToString();
                }

                conn.Close();
                da.Dispose();

                return contact;

            }
            catch (Exception)
            {
                return "";

            }

        }
        //private async Task<UserNotification> SendNotfAsync(string message, long agentID, int? TenantID)
        //{
        //    var userIdentifier = ToUserIdentifier(TenantID, agentID);

        //    await _appNotifier.SendMessageAsync(userIdentifier, message);

        //    var notifications = await _userNotificationManager.GetUserNotificationsAsync(
        //      userIdentifier);

        //    return null;
        //}
        private UserIdentifier ToUserIdentifier(int? TargetTenantId, long TargetUserId)
        {
            return new UserIdentifier(TargetTenantId, TargetUserId);
        }

        private List<BotAPI.Models.Location.DeliveryLocationCost> GetAllDeliveryLocationCost(int TenantId)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[DeliveryLocationCost] where TenantId = " + TenantId;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<BotAPI.Models.Location.DeliveryLocationCost> locationInfoModel = new List<BotAPI.Models.Location.DeliveryLocationCost>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    locationInfoModel.Add(new BotAPI.Models.Location.DeliveryLocationCost
                    {
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        FromLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FromLocationId"]),
                        ToLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ToLocationId"]),
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
                        BranchAreaId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"]),

                    });
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
        private TenantEditDto GetTenantByID(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where id=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            TenantEditDto tenants = new TenantEditDto();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {


                tenants = new TenantEditDto
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    InsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InsideNumber"]),
                    OutsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OutsideNumber"]),


                };



            }

            conn.Close();
            da.Dispose();

            return tenants;
        }


        private TenantEditDto GetTenantByZohoCustomerId(string ZohoCustomerId)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where ZohoCustomerId=" + ZohoCustomerId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            TenantEditDto tenants = new TenantEditDto();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                try
                {
                    tenants = new TenantEditDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        D360Key= dataSet.Tables[0].Rows[i]["D360Key"].ToString(),
                        ZohoCustomerId= dataSet.Tables[0].Rows[i]["ZohoCustomerId"].ToString(),
                        CautionDays= Convert.ToInt32(dataSet.Tables[0].Rows[i]["CautionDays"]),
                        WarningDays= Convert.ToInt32(dataSet.Tables[0].Rows[i]["WarningDays"]),
                        //InsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InsideNumber"]),
                        //OutsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OutsideNumber"]),


                    };
                }
                catch
                {
                    tenants = new TenantEditDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        D360Key= dataSet.Tables[0].Rows[i]["D360Key"].ToString(),
                        ZohoCustomerId= dataSet.Tables[0].Rows[i]["ZohoCustomerId"].ToString(),
                        CautionDays=2,
                        WarningDays= 2,
                        //InsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InsideNumber"]),
                        //OutsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OutsideNumber"]),


                    };

                }





            }

            conn.Close();
            da.Dispose();

            return tenants;
        }
        private static async Task<string> TranslatorFun(string text, string lan)
        {
            string url = "https://infoseedtranslator.cognitiveservices.azure.com/translator/text/v3.0/translate?to=" + lan;


            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", AppSettingsModel.TranslateKey);
                    //request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Region", "switzerlandnorth");

                    request.Content = new StringContent("[{'Text':'" + text + "'}]");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
                    string Jso = response.Content.ReadAsStringAsync().Result;
                    var results = Jso.Substring(1, Jso.Length - 2);
                    var rez = JsonConvert.DeserializeObject<TranslatorModel>(results);
                    return rez.translations[0].text;
                }
            }


        }
        private string TransFun(string word)
        {
            var toLanguage = "ar";//English
            var fromLanguage = "en";//Deutsch
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {
                return "Error";
            }
        }
        private GetMaintenancesForViewDto CreateMaintenance(BotAPI.Models.BotModel.CreateMaintenanceModel create)
        {
            var timeAdd = DateTime.Now.AddHours(AppSettingsModel.AddHour);

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO Maintenance "
                            + " (TenantId ,CreationTime ,ContactId ,OrderStatus , Address, DeliveryCost, OrderLocal, CustomerName, PhoneNumber1, PhoneNumber2, PhoneType, Damage, IsLockByAgent, LockByAgentName, AgentId, phoneNumber, DisplayName, UserId, FromLongitudeLatitude) VALUES "
                            + " (@TenantId ,@CreationTime ,@ContactId ,@OrderStatus ,@Address, @DeliveryCost, @OrderLocal, @CustomerName, @PhoneNumber1, @PhoneNumber2, @PhoneType, @Damage, @IsLockByAgent, @LockByAgentName, @AgentId, @phoneNumber, @DisplayName, @UserId, @FromLongitudeLatitude) ";




                        command.Parameters.AddWithValue("@TenantId", create.TenantID);
                        command.Parameters.AddWithValue("@CreationTime", timeAdd);
                        command.Parameters.AddWithValue("@ContactId", create.ContactId);
                        command.Parameters.AddWithValue("@OrderStatus", OrderStatusEunm.Pending);
                        command.Parameters.AddWithValue("@Address", create.Address);
                        command.Parameters.AddWithValue("@DeliveryCost", create.DeliveryCostAfter);

                        //  command.Parameters.AddWithValue("@FromLongitudeLatitude", create.FromLongitudeLatitude);
                        command.Parameters.AddWithValue("@OrderLocal", create.BotLocal);
                        command.Parameters.AddWithValue("@CustomerName", create.CustomerName);
                        command.Parameters.AddWithValue("@PhoneNumber1", create.PhoneNumber1);
                        command.Parameters.AddWithValue("@PhoneNumber2", create.PhoneNumber2);
                        command.Parameters.AddWithValue("@PhoneType", create.PhoneType);

                        command.Parameters.AddWithValue("@Damage", create.Damage);

                        command.Parameters.AddWithValue("@isLockByAgent", false);
                        command.Parameters.AddWithValue("@LockByAgentName", "");
                        command.Parameters.AddWithValue("@AgentId", -1);

                        command.Parameters.AddWithValue("@phoneNumber", create.phoneNumber);
                        command.Parameters.AddWithValue("@DisplayName", create.DisplayName);
                        command.Parameters.AddWithValue("@UserId", create.UserId);

                        command.Parameters.AddWithValue("@FromLongitudeLatitude", "https://maps.google.com/?q=" + create.LocationFrom);




                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), OrderStatusEunm.Pending);

                    GetMaintenancesForViewDto getMaintenancesForViewDto = new GetMaintenancesForViewDto();

                    getMaintenancesForViewDto.TenantID = create.TenantID;
                    getMaintenancesForViewDto.CreationTime = timeAdd;
                    getMaintenancesForViewDto.ContactId = create.ContactId;
                    getMaintenancesForViewDto.OrderStatus = 1;
                    getMaintenancesForViewDto.Address = create.Address;
                    getMaintenancesForViewDto.DeliveryCost = create.DeliveryCostAfter;
                    getMaintenancesForViewDto.OrderLocal = create.BotLocal;
                    getMaintenancesForViewDto.CustomerName = create.CustomerName;
                    getMaintenancesForViewDto.PhoneNumber1 = create.PhoneNumber1;
                    getMaintenancesForViewDto.PhoneNumber2 = create.PhoneNumber2;
                    getMaintenancesForViewDto.PhoneType = create.PhoneType;
                    getMaintenancesForViewDto.Damage = create.Damage;
                    getMaintenancesForViewDto.isLockByAgent = false;
                    getMaintenancesForViewDto.LockByAgentName = "";
                    getMaintenancesForViewDto.AgentId = -1;
                    getMaintenancesForViewDto.phoneNumber = create.phoneNumber;
                    getMaintenancesForViewDto.DisplayName = create.DisplayName;
                    getMaintenancesForViewDto.UserId = create.UserId;
                    getMaintenancesForViewDto.LocationFrom = "https://maps.google.com/?q=" + create.LocationFrom;

                    getMaintenancesForViewDto.orderStatusName = orderStatusName;



                    return getMaintenancesForViewDto;

                }
                catch (Exception)
                {

                    GetMaintenancesForViewDto getMaintenancesForViewDto = new GetMaintenancesForViewDto();
                    return getMaintenancesForViewDto;
                }

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
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;

                        areaDto.IsAvailableBranch = item.IsAvailableBranch;
                        distance = currentDistance;

                    }
                    if ((distance > currentDistance && !isInAmman) || (isInAmman && currentDistance < 5000 && distance > currentDistance))
                    {
                        areaDto = new AreaDto();
                        areaDto.Id = item.Id;
                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
                        areaDto.AreaName = item.AreaName;
                        areaDto.AreaCoordinate = item.AreaCoordinate;
                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
                        areaDto.IsRestaurantsTypeAll = item.IsRestaurantsTypeAll;
                        areaDto.IsAssginToAllUser = item.IsAssginToAllUser;
                         areaDto.IsAvailableBranch = item.IsAvailableBranch;
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

        private bool checkIsInServiceLiveChat(Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel workModel, out string outWHMessage)
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
                            outWHMessage = string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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
                            outWHMessage = string.Format(workModel.WorkTextSun, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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
                            outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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
                            outWHMessage = string.Format(workModel.WorkTextTues, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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
                            outWHMessage = string.Format(workModel.WorkTextWed, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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
                            outWHMessage = string.Format(workModel.WorkTextThurs, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
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

        private bool checkIsInService(string menuSetting)
        {

            bool result = true;
            if (!string.IsNullOrEmpty(menuSetting))
            {
                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
                DayOfWeek wk = currentDateTime.DayOfWeek;
                TimeSpan timeOfDay = currentDateTime.TimeOfDay;
                var options = new JsonSerializerOptions { WriteIndented = true };
                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(menuSetting, options);

                switch (wk)
                {
                    case DayOfWeek.Saturday:
                        if (workModel.IsWorkActiveSat)
                        {
                            if (timeOfDay >= getValidValue(workModel.StartDateSat).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateSat).TimeOfDay)
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
                            if (timeOfDay >= getValidValue(workModel.StartDateSun).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateSun).TimeOfDay)
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
                            if (timeOfDay >= getValidValue(workModel.StartDateMon).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateMon).TimeOfDay)

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
                            if (timeOfDay >= getValidValue(workModel.StartDateTues).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateTues).TimeOfDay)
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
                            if (timeOfDay >= getValidValue(workModel.StartDateWed).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateWed).TimeOfDay)
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
                            if (timeOfDay >= getValidValue(workModel.StartDateThurs).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateThurs).TimeOfDay)
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
                            if (timeOfDay >= getValidValue(workModel.StartDateFri).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateFri).TimeOfDay)
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
        private List<string> GetUserByTeneantId(int TenaentId, string userIds = null)
        {

            List<string> lstUserToken = new List<string>();
            var list = _iUserAppService.GetUserToken(TenaentId, userIds);
            if (list != null)
            {
                foreach (var item in list)
                {
                    lstUserToken.Add(item.Token);
                }
            }

            return lstUserToken;
        }
        private async Task<TenantModel> GetTenantById(int? id)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }

        private List<AttachmentBotAPIModel> FillAttachmentsTicketData(Infoseed.MessagingPortal.BotAPI.Models.BotModel.AttachmentModel[] attachments)
        {
            List<AttachmentBotAPIModel> attachmentsList = new List<AttachmentBotAPIModel>();

            foreach (var item in attachments)
            {

                attachmentsList.Add(new AttachmentBotAPIModel
                {
                    FileType = item.contentType,
                    Filename = FindFileByName(item.contentName, item.contentType).Result,
                    //Base64 = tContent
                });

                //var ext = System.IO.Path.GetExtension(item.contentUrl);

                //item.contentName = "infoseed" + ext;
                ////if (item.contentName == "")
                ////{
                ////    return attachmentsList;
                ////}
                //Helper _helper = new Helper();
                //var type = _helper.GetContentType(item.contentName);

                //if (type != null)
                //{
                //    byte[] tContent;
                //    string tContentType;
                //    if (item.contentUrl != null && _helper.DownloadAttachment(item.contentUrl, out tContent, out tContentType, out long? _KBSize))
                //    {
                //        attachmentsList.Add(new AttachmentBotAPIModel
                //        {
                //            FileType = tContentType,
                //            Filename = item.contentName,
                //            Base64 = tContent
                //        });
                //    }

                //}

            }

            return attachmentsList;
        }

        private List<AttachmentBotAPIModel> FillAttachmentsNewTicketData(Infoseed.MessagingPortal.BotAPI.Models.BotModel.AttachmentModel[] attachments)
        {
            List<AttachmentBotAPIModel> attachmentsList = new List<AttachmentBotAPIModel>();

            foreach (var item in attachments)
            {

                attachmentsList.Add(new AttachmentBotAPIModel
                {
                    FileType = item.contentType,
                    Filename = FindFileByName(item.contentName, item.contentType).Result,
                    //Base64 = tContent
                });
             
            }

            return attachmentsList;
        }


        private async Task<string> FindFileByName(string fileName, string mimtype)
        {
            var type = "." + mimtype.Split("/")[1];
            string url = await _generalAppService.FindFileByName(fileName, type);


            return url;
        }
        #endregion

        #region Selling Request 


        [Route("GetListPDF")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> GetListPDF(int TenantID, string phoneNumber, long? RealEstateType, int? RealEstateResidentialType, long? RealEstateResidentialForSaleType, int? VillaType, bool isOffer = false)
        {

            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
            var lstAsset = _iAssetAppService.GetListOfAsset(TenantID, RealEstateType.Value, RealEstateResidentialForSaleType.Value, RealEstateResidentialType.Value, null, null, isOffer);

            if (lstAsset != null)
                foreach (var item in lstAsset)
                {
                    if (item.lstAssetAttachmentDto != null)
                    {
                        foreach (var objitem in item.lstAssetAttachmentDto)
                        {

                            getListPDFModels.Add(new GetListPDFModel
                            {
                                AttachmentName = objitem.AttachmentName,
                                AttachmentType = objitem.AttachmentType,
                                AttachmentUrl = objitem.AttachmentUrl,
                                phoneNumber = phoneNumber,
                                TenantID = TenantID,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn

                            });

                            // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

                        }
                    }

                }



            return getListPDFModels;
        }
        [Route("GetAsset")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> GetAsset(int tenantID, string phoneNumber, int? typeId = null, long? levleOneId = 0, long? levelTwoId = 0, long? levelThreeId = 0, long? levelFourId = 0, bool isOffer = false)
        {

            if (levleOneId == null)
                levleOneId = 0;
            if (levelTwoId == null)
                levelTwoId = 0;
            if (levelThreeId == null)
                levelThreeId = 0;
            if (levelFourId == null)
                levelFourId = 0;

            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();

            try
            {

                var lstAsset = _iAssetAppService.GetListOfAsset(tenantID,
                    levleOneId,
                    levelTwoId,
                    typeId,
                   levelThreeId,
                   levelFourId
                   , isOffer);


                if (lstAsset != null)
                    foreach (var item in lstAsset)
                    {
                        if (item.lstAssetAttachmentDto != null)
                        {
                            foreach (var objitem in item.lstAssetAttachmentDto)
                            {

                                getListPDFModels.Add(new GetListPDFModel
                                {
                                    AttachmentName = objitem.AttachmentName,
                                    AttachmentType = objitem.AttachmentType,
                                    AttachmentUrl = objitem.AttachmentUrl,
                                    phoneNumber = phoneNumber,
                                    TenantID = tenantID,
                                    AssetDescriptionAr = item.AssetDescriptionAr,
                                    AssetDescriptionEn = item.AssetDescriptionEn,
                                    AssetNameAr = item.AssetNameAr,
                                    AssetNameEn = item.AssetNameEn,


                                });

                                // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

                            }
                        }
                        else
                        {
                            getListPDFModels.Add(new GetListPDFModel
                            {
                                //AttachmentName = objitem.AttachmentName,
                                // AttachmentType = objitem.AttachmentType,
                                //AttachmentUrl = objitem.AttachmentUrl,
                                phoneNumber = phoneNumber,
                                TenantID = tenantID,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn


                            });
                        }

                    }



                return getListPDFModels;

            }
            catch (Exception)
            {
                return getListPDFModels;

            }

        }

        [Route("getListPDFName")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> getListPDFName(int tenantID, string phoneNumber, int? typeId = null, long? levleOneId = 0, long? levelTwoId = 0, long? levelThreeId = 0, long? levelFourId = 0, bool isOffer = false)
        {

            if (levleOneId == null)
                levleOneId = 0;
            if (levelTwoId == null)
                levelTwoId = 0;
            if (levelThreeId == null)
                levelThreeId = 0;
            if (levelFourId == null)
                levelFourId = 0;

            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();

            try
            {

                var lstAsset = _iAssetAppService.GetListOfAsset(tenantID,
                    levleOneId,
                    levelTwoId,
                    typeId,
                   levelThreeId,
                   levelFourId
                   , isOffer);


                if (lstAsset != null)
                    foreach (var item in lstAsset)
                    {
                        if (item.lstAssetAttachmentDto != null)
                        {
                            getListPDFModels.Add(new GetListPDFModel
                            {
                                //AttachmentName = objitem.AttachmentName,
                                // AttachmentType = objitem.AttachmentType,
                                //AttachmentUrl = objitem.AttachmentUrl,
                                phoneNumber = phoneNumber,
                                TenantID = tenantID,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn


                            });
                        }
                        else
                        {
                            getListPDFModels.Add(new GetListPDFModel
                            {
                                //AttachmentName = objitem.AttachmentName,
                                // AttachmentType = objitem.AttachmentType,
                                //AttachmentUrl = objitem.AttachmentUrl,
                                phoneNumber = phoneNumber,
                                TenantID = tenantID,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn


                            });
                        }

                    }



                return getListPDFModels;

            }
            catch (Exception)
            {
                return getListPDFModels;

            }

        }

        [Route("getListPDFOfferName")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> getListPDFOfferName(int tenantID, string phoneNumber = "")
        {
            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
            try
            {

                var offers = _iAssetAppService.GetOfferAsset(0, 50, tenantID);
                if (offers.lstAssetDto != null)
                    foreach (var item in offers.lstAssetDto)
                    {
                        if (item.lstAssetAttachmentDto != null)
                        {
                            getListPDFModels.Add(new GetListPDFModel
                            {
                                AttachmentName = "",
                                AttachmentType = "",
                                AttachmentUrl = "",
                                phoneNumber = phoneNumber,
                                TenantID = tenantID,
                                AssetDescriptionAr = item.AssetDescriptionAr,
                                AssetDescriptionEn = item.AssetDescriptionEn,
                                AssetNameAr = item.AssetNameAr,
                                AssetNameEn = item.AssetNameEn


                            });
                        }

                    }
                return getListPDFModels;

            }
            catch (Exception)
            {
                return getListPDFModels;
            }


        }


        [Route("GetAssetOffers")]
        [HttpGet]
        public async Task<List<GetListPDFModel>> GetAssetOffers(int tenantID, string phoneNumber = "")
        {
            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
            try
            {

                var offers = _iAssetAppService.GetOfferAsset(0, 50, tenantID);
                if (offers.lstAssetDto != null)
                    foreach (var item in offers.lstAssetDto)
                    {
                        if (item.lstAssetAttachmentDto != null)
                        {
                            foreach (var objitem in item.lstAssetAttachmentDto)
                            {

                                getListPDFModels.Add(new GetListPDFModel
                                {
                                    AttachmentName = objitem.AttachmentName,
                                    AttachmentType = objitem.AttachmentType,
                                    AttachmentUrl = objitem.AttachmentUrl,
                                    phoneNumber = phoneNumber,
                                    TenantID = tenantID,
                                    AssetDescriptionAr = item.AssetDescriptionAr,
                                    AssetDescriptionEn = item.AssetDescriptionEn,
                                    AssetNameAr = item.AssetNameAr,
                                    AssetNameEn = item.AssetNameEn


                                });

                                // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

                            }
                        }

                    }
                return getListPDFModels;

            }
            catch (Exception)
            {
                return getListPDFModels;
            }


        }
        [Route("sendAssetAttachmentBot")]
        [HttpPost]
        public async Task<string> sendAssetAttachmentBot([FromBody] GetListPDFModel getListPDFModel)
        {

            await sendAssetAttachment(getListPDFModel.TenantID, getListPDFModel.phoneNumber, getListPDFModel.AttachmentUrl, getListPDFModel.AttachmentName, getListPDFModel.AttachmentType);

            return "Done";
        }
        private SellingRequestDto AddNewSellingRequest(SellingRequestDto sellingRequestDto)
        {
            var jsonModel = JsonConvert.SerializeObject(sellingRequestDto).ToString();

            CustomerLiveChatModel customerLiveChatModel = new CustomerLiveChatModel();

            var result = _dbService.UpdateLiveChat(sellingRequestDto.TenantId + "_" + sellingRequestDto.PhoneNumber, 1, true).Result;
            result.LiveChatStatusName = Enum.GetName(typeof(LiveChatStatusEnum), (int)LiveChatStatusEnum.Pending);
            string Department1 = null;
            string Department2 = null;
            string type = Enum.GetName(typeof(TypeEnum), (int)TypeEnum.Request);

            customerLiveChatModel.IsRequestForm = sellingRequestDto.IsRequestForm;
            customerLiveChatModel.ContactId = sellingRequestDto.ContactId;
            customerLiveChatModel.ContactInfo = sellingRequestDto.ContactInfo;
            customerLiveChatModel.lstSellingRequestDetailsDto = sellingRequestDto.lstSellingRequestDetailsDto;
            customerLiveChatModel.RequestDescription = sellingRequestDto.RequestDescription;
            customerLiveChatModel.AreaId = sellingRequestDto.AreaId;

            var livechat = _iliveChat.AddNewLiveChat(sellingRequestDto.TenantId, sellingRequestDto.PhoneNumber, sellingRequestDto.TenantId + "_" + sellingRequestDto.PhoneNumber, result.displayName, 1, true, type, Department1, Department2, result.IsOpen, (int)sellingRequestDto.DepartmentId, "", customerLiveChatModel);


            livechat.TenantId = sellingRequestDto.TenantId;
            //models.
            SocketIOManager.SendLiveChat(livechat, sellingRequestDto.TenantId);



            SellingRequestDto sellingRequest = _iSellingRequestAppService.AddSellingRequest(sellingRequestDto);

            return sellingRequest;

        }
        private SellingRequestDto AddSellingRequest(SellingRequestDto sellingRequestDto)
        {
            var jsonModel = JsonConvert.SerializeObject(sellingRequestDto).ToString();        
            SellingRequestDto sellingRequest = _iSellingRequestAppService.AddSellingRequest(sellingRequestDto);
            return sellingRequest;
        }


        private async Task sendAssetAttachment(int tenantID, string phoneNumber, string mediaUrl, string name, string type)
        {
            var Tenant = GetTenantById(tenantID).Result;

            var customer = await _dbService.GetCustomerWithTenantId(tenantID + "_" + phoneNumber, tenantID);


            var content = new Content()
            {
                type = type,
                text = "",
                fileName = name,
                mediaUrl = mediaUrl,
                agentName = Tenant.botId,
                agentId = "1000000"
            };



            var masseges = new SendWhatsAppD360Model
            {

                mediaUrl = mediaUrl,
                to = phoneNumber,
                type = type == "file" ? "document" : type,
                fileName = name,
                document = new SendWhatsAppD360Model.Document
                {
                    link = mediaUrl
                }

            };


            //var result = await WhatsAppDialogConnector.PostMsgToSmooch(Tenant.D360Key, masseges, _telemetry, true);

            //if (result == HttpStatusCode.Created)
            //{

            //    var CustomerChat = _dbService.UpdateCustomerChatD360(tenantID, content, tenantID + "_" + phoneNumber, customer.ConversationId);
            //    customer.customerChat = CustomerChat;

            //    // await _hub2.Clients.All.SendAsync("brodCastAgentMessage", customer);
            //    SocketIOManager.SendChat(customer, tenantID);


            //}
        }

        #endregion


        #region Delivery cost Per KiloMeter
        private GetLocationInfoModel getLocationInfoPerKiloMeter(int tenantID, string query)
        {
            try
            {
                GetLocationInfoModel getLocationInfoModel = new GetLocationInfoModel();

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

                        getLocationInfoModel = new GetLocationInfoModel();


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
                return new GetLocationInfoModel()
                {

                    DeliveryCostAfter = -1,
                    DeliveryCostBefor = -1,
                    LocationId = 0

                };

            }

        }
        #endregion


        #region  Jeep  bot

        [Route("CheckIsFillDisplayName")]
        [HttpGet]
        public string checkIsFillDisplayName(int id)
        {
            try
            {
               var con = _generalAppService.GetContactbyId(id);

                //var con = GetContact(id);
                if (con != null)
                {
                    if (string.IsNullOrEmpty(con.ContactDisplayName))
                        return null;
                    else
                        return con.ContactDisplayName;
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

        [Route("UpdateContcatDisplayName")]
        [HttpGet]
        public void updateContcatDisplayName(int id, string contactDisplayName)
        {
            try
            {
                _cusomerBehaviourAppService.UpdateContactName(id, contactDisplayName);
            }
            catch
            {


            }
           
            //ContactDto contactDto = new ContactDto();
            //contactDto.Id = id;
            //contactDto.ContactDisplayName = contactDisplayName;
            //updateContactSP(contactDto);

        }
        [Route("UpdateContcatkinship")]
        [HttpGet]
        public void updateContcatkinship(int id, string contactkinship)
        {
            try
            {
                _cusomerBehaviourAppService.UpdateContactkinship(id, contactkinship);
            }
            catch
            {

            }
        }

        //private void updateContactSP(ContactDto contactDto)
        //{
        //    try
        //    {
        //        var SP_Name = Constants.Contacts.SP_ContactUpdate;
        //        var sqlParameters = new List<SqlParameter>
        //        {
        //       new SqlParameter("@Id",contactDto.Id)
        //       ,new SqlParameter("@ContactDisplayName",contactDto.ContactDisplayName)
        //         };

        //        SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        #endregion



        [Route("lstGetChat")]
        [HttpGet]
        public async Task<string> fillchat(int length)
        {

            SocketIOManager.SendChat("Audai Walla test", 27);
            return "fff";

        }



        [Route("SingalRGet")]
        [HttpGet]
        public async Task<string> SingalRGet(int length)
        {
            try
            {



                for (int i = 0; i < length; i++)
                {
                    TestSingalRModel testSingalRModel = new TestSingalRModel();
                    testSingalRModel.Id = i;
                    testSingalRModel.Name = "Admin" + i;
                    // await _TestSingalRhub.Clients.All.SendAsync("broadcastTestSingalR", testSingalRModel);

                }
                return "done";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        [Route("EncryptApi")]
        [HttpGet]
        public string EncryptApi(string clearText)
        {


            string url = Encrypt(clearText);


            return url;
        }
        [Route("DecryptApi")]
        [HttpGet]
        public string DecryptApi(string clearText)
        {

            string url = Decrypt(clearText);


            return url;
        }
        private static List<CreateTicket> CreateFun(SendTicketMg model)
        {
            CreateTicket subject = new CreateTicket { name = "subject", value = model.subject };
            CreateTicket content = new CreateTicket { name = "content", value = model.content };



            CreateTicket hs_pipeline = new CreateTicket { name = "hs_pipeline", value = "0" };
            CreateTicket hs_pipeline_stage = new CreateTicket { name = "hs_pipeline_stage", value = "2" };
            CreateTicket hs_ticket_priority = new CreateTicket { name = "hs_ticket_priority", value = "HIGH" };
            CreateTicket hs_ticket_category = new CreateTicket { name = "hs_ticket_category", value = "GENERAL_INQUIRY" };


            List<CreateTicket> properties = new List<CreateTicket>();

            properties.Add(subject);
            properties.Add(content);
            properties.Add(hs_pipeline);
            properties.Add(hs_pipeline_stage);
            properties.Add(hs_ticket_priority);
            properties.Add(hs_ticket_category);
            return properties;
        }

        private void UpdateFun(TicketMg ticket, int contactId)
        {
            UpdateTicketsMg updateTicketsMg = new UpdateTicketsMg();
            List<UpdateTicketsMg.Input> input = new List<UpdateTicketsMg.Input>();
            UpdateTicketsMg.Input inp = new UpdateTicketsMg.Input
            {

                from = new UpdateTicketsMg.From { id = contactId.ToString() },
                to = new UpdateTicketsMg.To { id = ticket.objectId.ToString() },
                type = "contact_to_ticket"

            };
            input.Add(inp);
            updateTicketsMg.inputs = input.ToArray();
            _ticketsAPI.UpdateTicketsMg(updateTicketsMg);
        }
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private async Task<string> UpdateConversation(int Id, string phonenumber)
        {


            string result = string.Empty;

            try
            {
                var userID = Id.ToString()+"_"+phonenumber;
                var conversationChat2 = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                if (conversationChat2 != null)
                {
                    var queryString2 = "WHERE c.ItemType = 1 and c.userId='"+userID+"'";
                    await conversationChat2.DeleteChatItem(queryString2);

                }

                //var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                //TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == Id);

                //tenant.workModel=new Web.Models.Sunshine.WorkModel();
                //await itemsCollection.UpdateItemAsync(tenant._self, tenant);

            }
            catch
            {

            }


            return result;
        }



        private static string MyURLShorten(string Longurl)
        {

            string APIkey = "AIzaSyB_4zgEPL_so7Uu3pUlD1nogcHvlk45zYw";
            string post = "{\"longUrl\": \"" + Longurl + "\"}";
            string MyshortUrl = Longurl;
            HttpWebRequest Myrequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + APIkey);

            try
            {
                Myrequest.ServicePoint.Expect100Continue = false;
                Myrequest.Method = "POST";
                Myrequest.ContentLength = post.Length;
                Myrequest.ContentType = "application/json";
                Myrequest.Headers.Add("Cache-Control", "no-cache");

                using (Stream requestStream =
                   Myrequest.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0,
                       postBuffer.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)Myrequest.GetResponse())
                {
                    using (Stream responseStream =
                       response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new
                           StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();
                            MyshortUrl = Regex.Match(json, @"""id"":?""(?<id>.+)""").Groups["id"].Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return MyshortUrl;
        }


        private static string shortenIt(string url)
        {
            UrlshortenerService service = new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyB_4zgEPL_so7Uu3pUlD1nogcHvlk45zYw",
                ApplicationName = "My First Project",
            });

            var m = new Google.Apis.Urlshortener.v1.Data.Url();
            m.LongUrl = url;
            return service.Url.Insert(m).Execute().Id;
        }





        public static string Shorten(string longUrl)
        {
            string apikey = "YOUR_API_KEY";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var url = "https://api-ssl.bitly.com/v4/shorten";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.Headers["Authorization"] = "Bearer " + apikey;
            httpRequest.ContentType = "application/json";
            var data = new { long_url = longUrl, domain = "bit.ly" };
            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            ShortLinkResponce result = JsonConvert.DeserializeObject<ShortLinkResponce>(streamReader.ReadToEnd());
            return result.link;
        }
        public class ShortLinkResponce
        {
            public DateTime created_at { get; set; }
            public string id { get; set; }
            public string link { get; set; }
            public object[] custom_bitlinks { get; set; }
            public string long_url { get; set; }
            public bool archived { get; set; }
            public object[] tags { get; set; }
            public object[] deeplinks { get; set; }
        }

        private LoyaltyModel GetAll(int? tenantId = null)
        {
            if (tenantId == null)
            {
                tenantId = AbpSession.TenantId;
            }

            LoyaltyModel model = new LoyaltyModel();

            var objLoyalty = _cacheManager.GetCache("CacheLoyalty").Get("Loyalty_"+ tenantId.ToString(), cache => cache);
            if (objLoyalty.Equals("Loyalty_"+ tenantId.ToString()))
            {
                model = LoyaltyGet(tenantId);
                if (model != null)
                    _cacheManager.GetCache("CacheLoyalty").Set("Loyalty_"+ tenantId.ToString(), model);
                else
                    model = new LoyaltyModel() { CustomerCurrencyValue=1, CustomerPoints=100, ItemsCurrencyValue=1, ItemsPoints=100, IsLoyalityPoint=true };
            }
            else
            {
                model = (LoyaltyModel)objLoyalty;
            }



            return model;
        }


        private LoyaltyModel LoyaltyGet(int? tenantId = null)
        {
            try
            {
                if (tenantId == null)
                {
                    tenantId = AbpSession.TenantId;
                }

                var SP_Name = Constants.Loyalty.SP_LoyaltyGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                   new System.Data.SqlClient.SqlParameter("@TenantId", tenantId)
                    };

                LoyaltyModel loyalty = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                  DataReaderMapper.ConvertToLoyaltyDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return loyalty;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal ConvertCustomerPriceToPoint(decimal? price, int? tenantId = null)

        {
            decimal result = price.Value;

            if (price.HasValue)
            {
                var LoyaltyModel = GetAll(tenantId);
                if (LoyaltyModel.IsLoyalityPoint)
                    result = (LoyaltyModel.CustomerPoints * price.Value)/(LoyaltyModel.CustomerCurrencyValue);
            }
            return result;
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



                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void createContactLoyaltyTransaction(ContactLoyaltyTransactionModel contactLoyalty)
        {
            try
            {
                int year = DateTime.UtcNow.Year;
                var SP_Name = Constants.Loyalty.SP_ContactLoyaltyTransactionAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>{
                     new System.Data.SqlClient.SqlParameter("@ContactId",contactLoyalty.ContactId)
                    ,new System.Data.SqlClient.SqlParameter("@Points",contactLoyalty.Points)
                    ,new System.Data.SqlClient.SqlParameter("@OrderId",contactLoyalty.OrderId)
                   // ,new SqlParameter("@CardPoints",contactLoyalty.CardPoints)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedBy",76)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@LoyaltyDefinitionId",contactLoyalty.LoyaltyDefinitionId)
                    ,new System.Data.SqlClient.SqlParameter("@Year",year)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private  async Task UpdateInvoicesHistory(string customer_id, string Status, bool IsStatus, string D360Key)
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

                _cacheManager.GetCache("CacheTenant").Remove(D360Key);

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

        private List<string> FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos, int TenantId, string UserIds)
        {

            List<string> vs = new List<string>();

            var role = GetRole(TenantId);
            var userRole = GetUserRole(TenantId, UserIds).Distinct();

            foreach (var item in userRole)
            {
                var uR = role.Where(x => x.Id==item.RoleId).FirstOrDefault();
                if (uR!=null)
                {
                    if (uR.Name!="Admin")
                    {
                        var doc = userListDtos.Where(x => x.Id==item.UserId).FirstOrDefault();

                        vs.Add(doc.Name.Trim());
                    }

                }

            }

            return vs;
        }

        private List<long> GetAdminIds(IReadOnlyCollection<UserListDto> userListDtos, int TenantId, string UserIds)
        {

            List<long> vs = new List<long>();

            var role = GetRole(TenantId);
            var userRole = GetUserRole(TenantId, UserIds).Distinct();

            foreach (var item in userRole)
            {
                var uR = role.Where(x => x.Id==item.RoleId).FirstOrDefault();
                if (uR!=null)
                {
                    if (uR.Name=="Admin")
                    {
                        var doc = userListDtos.Where(x => x.Id==item.UserId).FirstOrDefault();

                        vs.Add(doc.Id);
                    }

                }

            }

            return vs;
        }


        private List<RoleDto> GetRole(int TenantId)
        {

            var SP_Name = "GetRole";

            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                   new System.Data.SqlClient.SqlParameter("@TenantId", TenantId)
                    };

            List<RoleDto> roles = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertToRoleDto, AppSettingsModel.ConnectionStrings).ToList();
            return roles;
        }
        private List<UserRoleDto> GetUserRole(int TenantId, string UserIds)
        {

            var SP_Name = "GetUserRole";

            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>(){
                   new System.Data.SqlClient.SqlParameter("@TenantId", TenantId),
                    new System.Data.SqlClient.SqlParameter("@UserIds", UserIds)
                    };

            List<UserRoleDto> userroles = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), ConvertToUserRoleDto, AppSettingsModel.ConnectionStrings).ToList();
            return userroles;
        }

        private static RoleDto ConvertToRoleDto(IDataReader dataReader)
        {
            RoleDto role = new RoleDto();
            role.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            role.Name = SqlDataHelper.GetValue<string>(dataReader, "Name");

            return role;
        }


        private static UserRoleDto ConvertToUserRoleDto(IDataReader dataReader)
        {
            UserRoleDto userroles = new UserRoleDto();
            userroles.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            userroles.RoleId = SqlDataHelper.GetValue<int>(dataReader, "RoleId");
            userroles.UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId");
            return userroles;
        }
        private async Task<UserNotification> SendNotfAsync(string message, long agentID,int tenantId, NotificationSeverity notificationSeverity)
        {
            var userIdentifier = ToUserIdentifier(tenantId, agentID);

            await _appNotifier.SendMessageAsync(userIdentifier, message, notificationSeverity);

            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
              userIdentifier);

            notifications.Sort((x, y) => DateTime.Compare(x.Notification.CreationTime, y.Notification.CreationTime));

            var notf = notifications.ToArray().Last();

            return notf;
        }


     

    }





}
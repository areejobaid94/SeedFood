using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Caching;
using AutoMapper;
using Azure;
using Azure.Communication.Email;
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
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.BotAPI.Models.Firebase;
using Infoseed.MessagingPortal.BotAPI.Models.Location;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.ContactNotification;
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
using Infoseed.MessagingPortal.Notifications;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SealingReuest;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Customers.Dtos.CustomerBehaviourEnums;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;
using static Infoseed.MessagingPortal.LiveChat.Dto.LiveChatEnums;
using AttachmentModel = Infoseed.MessagingPortal.BotAPI.Models.BotModel.AttachmentModel;
using Match = System.Text.RegularExpressions.Match;
using UserRoleDto = Infoseed.MessagingPortal.BotAPI.Models.UserRoleDto;

namespace Infoseed.MessagingPortal.BotAPI.Controllers
{
    public class NewBotAPIController : IBotApis
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
        public NewBotAPIController(
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
            IDashboardUIAppService dashboardAppService

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
        }

        public async Task<string> EndDialog(string phonenumber, string teanantId)
        {
            try
            {


                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == teanantId+"_"+phonenumber);//&& a.TenantId== TenantId


                var Customer = customerResult.Result;
                if (Customer!=null)
                {

                    Customer.IsTemplateFlow = false;
                    Customer.templateId = "";
                    Customer.CampaignId="";
                    Customer.TemplateFlowDate=null;

                }

                var Result = itemsCollection.UpdateItemAsync(Customer._self, Customer).Result;

                return "ok";
            }
            catch (Exception ex)
            {
                return "no";
            }
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body,string userIds)
        {

            var emails = getEmails(userIds);

            foreach(var user in emails)
            {

                // This code retrieves your connection string from an environment variable.
                string connectionString = "endpoint=https://inoseedsendemail.unitedstates.communication.azure.com/;accesskey=B8Ah3Setud7u7kjs4W6P2asQjxzGmvvEEq17PZnQyvWnZvkSaqiBJQQJ99AGACULyCpNJ3J3AAAAAZCSCt0S";
                var emailClient = new EmailClient(connectionString);


                EmailSendOperation emailSendOperation = emailClient.Send(
                    WaitUntil.Completed,
                    senderAddress: "DoNotReply@8d910114-fe9c-4e5d-b979-da4cd2848a34.azurecomm.net",
                    recipientAddress: user.EmailAddress,
                    subject: subject,
                    htmlContent: "<html><h1>مرحبا من انفوسيد</h1l> </br> "+body+"</html>",
                    plainTextContent: body);

            }




           
        }

        private List<UserEmailsModel> getEmails(string userIds)
        {
            try
            {
                List<UserEmailsModel> emails = new List<UserEmailsModel>();

                var SP_Name = Constants.User.SP_GetUsersByIds;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@Ids",userIds)

                };
                emails = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),ConvertUserEmailsDto, AppSettingsModel.ConnectionStrings).ToList();

                return emails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static UserEmailsModel ConvertUserEmailsDto(IDataReader dataReader)
        {
            UserEmailsModel emails = new UserEmailsModel();
            emails.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
            emails.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
            emails.EmailAddress = SqlDataHelper.GetValue<int>(dataReader, "EmailAddress");


            return emails;
        }
        public void UpdaateDisplayName(string contactID, string displayName = "")
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactNameUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@contactID", contactID) ,
                    new System.Data.SqlClient.SqlParameter("@displayName",displayName) ,
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);;
            }
            catch
            {
            }
        }
        public void UpdaateLocation(string contactID, string location = "")
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactLocationUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@contactID", contactID) ,
                    new System.Data.SqlClient.SqlParameter("@location",location) ,
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings); ;
            }
            catch
            {
            }
        }

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

        public List<CaptionDto> GetAllCaption(int TenantID, string local)
        {
            int localID = 0;
            if (local == "en")
            {
                localID = 2;

            }
            if (local == "ar")
            {
                localID = 1;
            }

            try
            {
                var cap = _captionBotAppService.GetCaption(TenantID, null);
                return cap;
           
            }
            catch
            {
                return new List<CaptionDto>();

            }
        }

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

        public List<string> GetAreasWithPage(string TenantID, string local, int menu, int pageNumber, int pageSize, bool isDelivery)
        {
            List<string> vs = new List<string>();
            var list = _iAreasAppService.GetAllAreas(int.Parse(TenantID), true);// GetAreasList(TenantID);

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
        public GetLocationInfoModel GetlocationUserModel(SendLocationUserModel input)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            try
            {
                if (IsvalidLatLong(input.query.Split(",")[0], input.query.Split(",")[1]))
                {


                    TenantModel tenant = GetTenantById(input.tenantID).Result;

                    input.isOrderOffer = tenant.isOrderOffer;
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

        public GetLocationInfoModel GetlocationUserModeFlowsBot(SendLocationUserModel input)
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


                        if (Distric=="Irbid Qasabah District" && Area=="")
                        {
                            Distric=Route;
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

        public OrderAndDetailsModel GetOrderAndDetails(GetOrderAndDetailModel input)
        {
            try
            {
                TenantModel tenant = GetTenantById(input.TenantID).Result;

                input.isOrderOffer = tenant.isOrderOffer;

                if (input.TypeChoes == "Delivery")
                {
                    input.LocationId = input.LocationId;

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
                if (input.LocationInfo == null)
                    input.LocationInfo = new GetLocationInfoModel() { DeliveryCostAfter = 0, DeliveryCostBefor = 0, LocationId = input.LocationId };

                var after = input.LocationInfo.DeliveryCostAfter;
                decimal? cost = 0;
                if (input.isOrderOffer)
                {
                    if (string.IsNullOrEmpty(input.LocationInfo.City))
                    {
                        input.LocationInfo.City = "NotFound";
                    }
                    if (string.IsNullOrEmpty(input.LocationInfo.Area))
                    {
                        input.LocationInfo.Area = "NotFound";
                    }
                    if (string.IsNullOrEmpty(input.LocationInfo.Distric))
                    {
                        input.LocationInfo.Distric = "NotFound";
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
                        cost = input.LocationInfo.DeliveryCostBefor;
                        input.LocationInfo.DeliveryCostAfter = input.LocationInfo.DeliveryCostBefor;
                        input.LocationInfo.DeliveryCostBefor = after;

                        input.deliveryCostBefor = input.LocationInfo.DeliveryCostBefor;
                        input.deliveryCostAfter = input.LocationInfo.DeliveryCostAfter;

                        input.DeliveryCostText = input.DeliveryCostTextTow.Replace("{0}", cost.ToString());


                        cost = input.deliveryCostAfter;

                        //  order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                    }
                    else
                    {
                        input.LocationInfo.DeliveryCostAfter = after;
                        cost = input.deliveryCostAfter;


                    }



                }
                else
                {
                    cost = input.Cost;

                    //  order.Total=order.Total+cost.Value;
                }


                // input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());
                if (input.DeliveryCostTextTow != null)
                {
                    input.DeliveryCostTextTow = input.DeliveryCostTextTow.Replace("{0}", input.Cost.ToString());
                }

                if (input.DeliveryCostText == null)
                {

                    input.DeliveryCostText = input.DeliveryCostTextTow.Replace("{0}", cost.ToString());
                }

                var OrderString = GetOrderDetailString(input.TenantID, input.lang, order.Total, order.TotalPoints, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount, input.TypeChoes, input.DeliveryCostText, cost);


                //if (input.LocationInfo.isOrderOfferCost)
                //{
                //    order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                //}
                OrderAndDetailsModel orderAndDetailsModel = new OrderAndDetailsModel
                {
                    order = order,
                    DetailText = OrderString,
                    orderId = int.Parse(order.Id.ToString()),
                    total = order.Total,
                    Discount = Discount,
                    IsDiscount = isDiscount,
                    GetLocationInfo = input.LocationInfo,
                    IsItemOffer = isDiscount,
                    ItemOffer = Discount,
                    IsDeliveryOffer = input.LocationInfo.isOrderOfferCost,
                    DeliveryOffer = after

                };



                return orderAndDetailsModel;

            }
            catch
            {

                return null;
            }
        }

        public OrderAndDetailsModel GetOrderAndDetailsFlowsBot(GetOrderAndDetailModel input)
        {
            try
            {
                TenantModel tenant = GetTenantById(input.TenantID).Result;

                input.isOrderOffer = tenant.isOrderOffer;
                input.Tax = tenant.TaxValue;

                if (input.TypeChoes == "Delivery")
                {
                    input.LocationId = input.LocationId;

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
                                decimal taxValue;
                                decimal offerValue;
                                decimal deliveryCostAfter = input.deliveryCostAfter ?? 0m;
                                if (areaEffor.isBranchDiscount)
                                {
                                    if (areaEffor.BranchesIds.Contains(input.LocationId.ToString()))
                                    {

                                        if (input.Tax != 0)
                                        {

                                            if (input.deliveryCostAfter > 0)
                                            {
                                                taxValue = ((order.Total) + (input.deliveryCostAfter ?? 0m)) * ((input.Tax ?? 0m) / 100m);
                                                offerValue = (order.Total * (areaEffor.NewFees / 100));
                                                order.Total = order.Total + taxValue - offerValue;
                                                isDiscount = true;
                                                Discount = areaEffor.NewFees;
                                            }
                                            else
                                            {
                                                taxValue = order.Total * ((input.Tax ?? 0) / 100);
                                                offerValue = (order.Total * (areaEffor.NewFees / 100));
                                                order.Total = order.Total + taxValue - offerValue;
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
                                else
                                {
                                    if (input.Tax != 0)
                                    {
                                        if(input.deliveryCostAfter > 0)
                                        {
                                            taxValue = ((order.Total) + (input.deliveryCostAfter ?? 0m)) * ((input.Tax ?? 0m) / 100m);
                                            offerValue = (order.Total * (areaEffor.NewFees / 100));
                                            order.Total = order.Total + taxValue - offerValue;
                                            isDiscount = true;
                                            Discount = areaEffor.NewFees;
                                        }
                                        else
                                        {
                                            taxValue = order.Total * ((input.Tax ?? 0) / 100);
                                            offerValue = (order.Total * (areaEffor.NewFees / 100));
                                            order.Total = order.Total + taxValue - offerValue;
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
                        else if (input.Tax != 0)
                        {
                            decimal taxValue = ((order.Total) + (input.deliveryCostAfter ?? 0m)) * ((input.Tax ?? 0m) / 100m);
                            order.Total = order.Total + taxValue;
                        }

                    }
                }
                else if (input.Tax != 0)
                {
                    decimal taxValue = ((order.Total) + (input.deliveryCostAfter ?? 0m)) * ((input.Tax ?? 0m) / 100m);
                    order.Total = order.Total + taxValue;
                }
                if (input.LocationInfo == null)
                    input.LocationInfo = new GetLocationInfoModel() { DeliveryCostAfter = 0, DeliveryCostBefor = 0, LocationId = input.LocationId };

                var after = input.LocationInfo.DeliveryCostAfter;
                decimal? cost = 0;
                if (input.isOrderOffer)
                {
                    if (string.IsNullOrEmpty(input.LocationInfo.City))
                    {
                        input.LocationInfo.City = "NotFound";
                    }
                    if (string.IsNullOrEmpty(input.LocationInfo.Area))
                    {
                        input.LocationInfo.Area = "NotFound";
                    }
                    if (string.IsNullOrEmpty(input.LocationInfo.Distric))
                    {
                        input.LocationInfo.Distric = "NotFound";
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
                        cost = input.LocationInfo.DeliveryCostBefor;
                        input.LocationInfo.DeliveryCostAfter = input.LocationInfo.DeliveryCostBefor;
                        input.LocationInfo.DeliveryCostBefor = after;

                        input.deliveryCostBefor = input.LocationInfo.DeliveryCostBefor;
                        input.deliveryCostAfter = input.LocationInfo.DeliveryCostAfter;

                        input.DeliveryCostText = input.DeliveryCostTextTow.Replace("{0}", cost.ToString());


                        cost = input.deliveryCostAfter;

                        //  order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                    }
                    else
                    {
                        input.LocationInfo.DeliveryCostAfter = after;
                        cost = input.deliveryCostAfter;


                    }



                }
                else
                {
                    cost = input.Cost;

                    //  order.Total=order.Total+cost.Value;
                }


                // input.DeliveryCostText=input.DeliveryCostTextTow.Replace("{0}", cost.ToString());
                if (input.DeliveryCostTextTow != null)
                {
                    input.DeliveryCostTextTow = input.DeliveryCostTextTow.Replace("{0}", input.Cost.ToString());
                }

                if (input.DeliveryCostText == null)
                {

                    input.DeliveryCostText = input.DeliveryCostTextTow.Replace("{0}", cost.ToString());
                }

                var OrderString = GetOrderDetailStringFlowsBot(input.TenantID, input.lang, order.Total, input.Tax, order.TotalPoints, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount, input.TypeChoes, input.DeliveryCostText, cost);

                //var OrderString = GetOrderDetailString(input.TenantID, input.lang, order.Total, order.TotalPoints, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount, input.TypeChoes, input.DeliveryCostText, cost);

                //if (input.LocationInfo.isOrderOfferCost)
                //{
                //    order.Total=order.Total+input.LocationInfo.DeliveryCostAfter.Value;
                //}
                OrderAndDetailsModel orderAndDetailsModel = new OrderAndDetailsModel
                {
                    order = order,
                    DetailText = OrderString,
                    orderId = int.Parse(order.Id.ToString()),
                    total = order.Total,
                    Discount = Discount,
                    IsDiscount = isDiscount,
                    GetLocationInfo = input.LocationInfo,
                    IsItemOffer = isDiscount,
                    ItemOffer = Discount,
                    IsDeliveryOffer = input.LocationInfo.isOrderOfferCost,
                    DeliveryOffer = after

                };



                return orderAndDetailsModel;

            }
            catch
            {

                return null;
            }

        }

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

                            cancelOrderModel.CancelOrder = false;
                            cancelOrderModel.WrongOrder = false;
                            cancelOrderModel.IsTrueOrder = true;
                            cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                            return cancelOrderModel;

                        }
                    }
                    if (!_iOrdersAppService.CreateOrderStatusHistory(OrderModel.Id, (int)OrderStatusEunm.Canceled, TenantID))
                    {
                        cancelOrderModel.CancelOrder = false;
                        cancelOrderModel.WrongOrder = false;
                        cancelOrderModel.IsTrueOrder = true;
                        cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
                        return cancelOrderModel;
                    }

                   

                    var x = UpdateOrderAfterCancel(OrderNumber, ContactId, OrderModel, TenantID);
                    cancelOrderModel.CancelOrder = true;
                    cancelOrderModel.WrongOrder = false;
                    cancelOrderModel.IsTrueOrder = true;


                    _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                    {
                        TenantId = TenantID.Value,
                        TypeId = (int)DashboardTypeEnum.Order,
                        StatusId = (int)OrderStatusEunm.Canceled,
                        StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Canceled)
                    });



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

        public string UpdateOrderAsync(UpdateOrderModel updateOrderModel)
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


            if (updateOrderModel.TypeChoes == "Pickup")
            {

                var area = GetAreasList(updateOrderModel.TenantID.ToString()).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();
                Infoseed.MessagingPortal.Orders.Order order = new Infoseed.MessagingPortal.Orders.Order
                {

                    OrderLocal = updateOrderModel.BotLocal,
                    BranchId=updateOrderModel.BranchId,
                    BuyType = updateOrderModel.BuyType,
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

                    Tax= updateOrderModel.Tax,


                    IsPreOrder = updateOrderModel.IsPreOrder,
                    SelectDay= updateOrderModel.SelectDay,
                    SelectTime= updateOrderModel.SelectTime,
                    RestaurantName="",
                    TotalPoints=updateOrderModel.loyalityPoint.Value,

                    IsItemOffer=updateOrderModel.IsItemOffer,
                    IsDeliveryOffer=updateOrderModel.IsDeliveryOffer,
                    ItemOffer=updateOrderModel.ItemOffer,
                    DeliveryOffer=updateOrderModel.DeliveryOffer,
                    DeliveryEstimation = updateOrderModel.DeliveryEstimation


                };

                //_iOrdersAppService.UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                OrderSoket orderDto = new OrderSoket();
                orderDto = _iOrdersAppService.UpdateOrderSoket(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = order.TenantId.Value,
                    TypeId = (int)DashboardTypeEnum.Order,
                    StatusId = (int)OrderStatusEunm.Pending,
                    StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Pending)
                });



                //var ListString = "------------------ \r\n\r\n";

                //ListString = ListString + updateOrderModel.captionOrderInfoText;
                //ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";
                //ListString = ListString + updateOrderModel.aptionAreaNameText + area.AreaName + "\r\n";
                //if (updateOrderModel.loyalityPoint.Value>0)
                //{

                //    ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint + "\r\n";

                //}
                //ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + updateOrderModel.OrderTotal + "\r\n";
                //ListString = ListString + "------------------ \r\n\r\n";
                //ListString = ListString + updateOrderModel.captionEndOrderText;

                var ListString = "";



                if (updateOrderModel.loyalityPoint.Value>0)
                {
                    ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint.Value.ToString() + "\r\n\r\n";
                    if (updateOrderModel.BotLocal=="ar")
                    {
                        ListString = updateOrderModel.captionBranchCostText.Replace("{0}", number.ToString()).Replace("{1}", area.AreaName).Replace("{2}", updateOrderModel.OrderTotal.ToString()).Replace("{3}", updateOrderModel.loyalityPoint.Value.ToString());
                    }
                    else
                    {
                        ListString = updateOrderModel.captionBranchCostText.Replace("{0}", number.ToString()).Replace("{1}", area.AreaNameEnglish).Replace("{2}", updateOrderModel.OrderTotal.ToString()).Replace("{3}", updateOrderModel.loyalityPoint.Value.ToString());
                    }

                }
                else
                {
                    if (updateOrderModel.BotLocal=="ar")
                    {
                        ListString = updateOrderModel.captionOrderNumberText.Replace("{0}", number.ToString()).Replace("{1}", area.AreaName).Replace("{2}", updateOrderModel.OrderTotal.ToString());
                    }
                    else
                    {
                        ListString = updateOrderModel.captionOrderNumberText.Replace("{0}", number.ToString()).Replace("{1}", area.AreaNameEnglish).Replace("{2}", updateOrderModel.OrderTotal.ToString());
                    }
                }




                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);
                GetOrderMap.SpecialRequestText=orderDto.SpecialRequestText;
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
                    SendMobileNotification(order.TenantId.Value, titl, body, false, area.UserIds);
                }
                catch (Exception)
                {

                }



                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);

            

                con.TotalOrder = con.TotalOrder + 1;
                con.TakeAwayOrder = con.TakeAwayOrder + 1;



                var loyaltyModel = _loyaltyAppService.GetAll(updateOrderModel.TenantID);
                var modelorder = GetOrder(order.Id);
                if (modelorder.TotalCreditPoints > 0 || order.TotalPoints > 0)
                {
                    _loyaltyAppService.CreateContactLoyaltyTransaction(new ContactLoyaltyTransactionModel
                    {
                        OrderId = order.Id,
                        LoyaltyDefinitionId = loyaltyModel.Id,
                        ContactId = order.ContactId.Value,
                        Points= modelorder.TotalPoints,
                        CreditPoints= modelorder.TotalCreditPoints,
                        CreatedBy = order.ContactId.Value,
                        Year=DateTime.UtcNow.Year,
                        TransactionTypeId = (int)LoyaltyTransactionType.MakeOrder,
                    });

                }

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

                var BranchAreaName = "";
                if (updateOrderModel.BotLocal=="ar")
                {
                    BranchAreaName = area.AreaName;
                }
                else
                {
                    BranchAreaName = area.AreaNameEnglish;
                }
                decimal totalWithBranchCost = 0;
                decimal Cost = 0;

                if (false)//updateOrderModel.isOrderOfferCost
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

                    Tax= updateOrderModel.Tax,

                    IsDeleted = false,
                    AgentId = 0,
                    AgentIds = AgentIds,
                    LocationID = updateOrderModel.BranchId,
                    FromLocationDescribation = "https://maps.google.com/?q=" + updateOrderModel.LocationFrom.Replace(" ", "").Replace("https://maps.google.com/?q=",""),
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
                    DeliveryOffer=updateOrderModel.DeliveryOffer,
                    DeliveryEstimation = updateOrderModel.DeliveryEstimation

                };






                //_iOrdersAppService.UpdateOrder(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                OrderSoket orderDto = new OrderSoket();
                orderDto = _iOrdersAppService.UpdateOrderSoket(JsonConvert.SerializeObject(order), order.Id, order.TenantId.Value);
                _dashboardAppService.CreateDashboardNumber(new DashboardNumbers
                {
                    TenantId = order.TenantId.Value,
                    TypeId = (int)DashboardTypeEnum.Order,
                    StatusId = (int)OrderStatusEunm.Pending,
                    StatusName = Enum.GetName(typeof(OrderStatusEunm), (int)OrderStatusEunm.Pending)
                });


       

                //var ListString = "------------------ \r\n\r\n";

                //ListString = ListString + updateOrderModel.captionOrderInfoText;
                //ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";

                //if (updateOrderModel.TenantID == 46)
                //{
                //    ListString = ListString + updateOrderModel.captionBranchCostText + ((int)Cost).ToString() + "\r\n";
                //}
                //else
                //{
                //    ListString = ListString + updateOrderModel.captionBranchCostText + Cost + "\r\n";
                //}

                //ListString = ListString + updateOrderModel.captionFromLocationText + updateOrderModel.Address + "\r\n";



                //if (updateOrderModel.TenantID == 46)
                //{
                //    if (updateOrderModel.loyalityPoint.Value>0)
                //    {
                //        ListString = ListString + captionTotalPointOfAllOrderText + (updateOrderModel.loyalityPoint.Value).ToString() + "\r\n\r\n";

                //    }
                //    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + ((int)totalWithBranchCost).ToString() + "\r\n\r\n";
                //}
                //else
                //{
                //    if (updateOrderModel.loyalityPoint.Value>0)
                //    {
                //        ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint.Value.ToString() + "\r\n\r\n";

                //    }
                //    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + totalWithBranchCost + "\r\n\r\n";
                //}


                //ListString = ListString + "------------------ \r\n\r\n";
                //ListString = ListString + updateOrderModel.captionEndOrderText;




                var ListString = "";



                if (updateOrderModel.loyalityPoint.Value>0)
                {
                    ListString = ListString + captionTotalPointOfAllOrderText + updateOrderModel.loyalityPoint.Value.ToString() + "\r\n\r\n";
                    if (updateOrderModel.BotLocal=="ar")
                    {
                        ListString = updateOrderModel.captionEndOrderText.Replace("{0}", number.ToString()).Replace("{1}", Cost.ToString()).Replace("{2}", updateOrderModel.Address).Replace("{3}", area.AreaName).Replace("{4}", totalWithBranchCost.ToString()).Replace("{5}", updateOrderModel.loyalityPoint.Value.ToString());
                    }
                    else
                    {
                        ListString = updateOrderModel.captionEndOrderText.Replace("{0}", number.ToString()).Replace("{1}", Cost.ToString()).Replace("{2}", updateOrderModel.Address).Replace("{3}", area.AreaNameEnglish).Replace("{4}", totalWithBranchCost.ToString());
                    }

                }
                else
                {
                    if (updateOrderModel.BotLocal=="ar")
                    {
                        ListString = updateOrderModel.captionOrderInfoText.Replace("{0}", number.ToString()).Replace("{1}", Cost.ToString()).Replace("{2}", updateOrderModel.Address).Replace("{3}", area.AreaName).Replace("{4}", totalWithBranchCost.ToString());
                    }
                    else
                    {
                        ListString = updateOrderModel.captionOrderInfoText.Replace("{0}", number.ToString()).Replace("{1}", Cost.ToString()).Replace("{2}", updateOrderModel.Address).Replace("{3}", area.AreaNameEnglish).Replace("{4}", totalWithBranchCost.ToString());
                    }
                }

                if (updateOrderModel.IsPreOrder)
                {
                    order.orderStatus = OrderStatusEunm.Pre_Order;
                }

                try
                {
                    var titl = "the order Number: " + number.ToString();
                    var body = "Order Status :" + order.orderStatus + " From :" + updateOrderModel.Address;

                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                    SendMobileNotification(order.TenantId.Value, titl, body, false, area.UserIds);
                }
                catch
                {

                }


                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
                var GetOrderMap = _mapper.Map(order, getOrderForViewDto.Order);
                GetOrderMap.SpecialRequestText=orderDto.SpecialRequestText;
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
            UpdateCustomer(behaviourModel);
            _cusomerBehaviourAppService.UpdateCustomerBehavior(behaviourModel);
           

        }
        private void UpdateCustomer(CustomerBehaviourModel behaviourModel)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.ContactID == behaviourModel.ContactId.ToString()).Result;
            customerResult.CustomerOPT = behaviourModel.CustomerOPt;
            var Result = itemsCollection.UpdateItemAsync(customerResult._self, customerResult).Result;
        }
        public async Task<TenantModel> GetTenantAsync(int TenantID)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID);
            return tenant;
        }
        public async Task<string> UpdateNewLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0, string UserIds = "", string DepartmentInfo = "")// DepartmentId
        {
            
            string SFormat = string.Empty;
            try
            {
                var Tenant = GetTenantById(TenantID).Result;

                var result = _dbService.UpdateLiveChat(TenantID + "_" + phoneNumber, 1, true).Result;


                if (Department1 != null)
                    result.Department = Department1 + "-" + Department2;


                if (!DepartmentInfo.IsNullOrEmpty())
                {
                    result.Department=DepartmentInfo;
                    Department1=DepartmentInfo;
                }


                result.LiveChatStatusName = "Pending";
                string type = Enum.GetName(typeof(TypeEnum), (int)TypeEnum.Ticket);
                var livechat = _iliveChat.AddNewLiveChat(TenantID, phoneNumber, TenantID + "_" + phoneNumber, result.displayName, 1, true, type, Department1, Department2, result.IsOpen, DepartmentId, UserIds);

         

                if (Tenant.IsLiveChatWorkActive)
                {
                    if (!checkIsInServiceLiveChat(Tenant.LiveChatWorkingHours, out SFormat))
                    {
                        return SFormat;
                    }
                }

                if (result != null)
                {
                    //result.customerChat = null;
                    try
                    {
                        var titl = "New Live Chat Request ";
                        var body = "From : " + result.displayName;
                        if (DepartmentId==0&& !string.IsNullOrEmpty(UserIds))
                        {
                            livechat.DepartmentUserIds=UserIds;

                        }
                        // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                        SendMobileNotification(TenantID.Value, titl, body, true, livechat.DepartmentUserIds);
                    }
                    catch (Exception)
                    {

                    }
                    await _LiveChatHubhub.Clients.All.SendAsync("brodCastBotLiveChat", result);
                    SocketIOManager.SendLiveChat(livechat, TenantID.Value);
                }
                return SFormat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> UpdateLiveChatAsync(int? TenantID, string phoneNumber, string Department1 = null, string Department2 = null, int DepartmentId = 0, string UserIds = "")// DepartmentId
        {

            string SFormat = string.Empty;


            try
            {
                var Tenant = GetTenantById(TenantID).Result;
                // var con = GetContact(contactId);
                var result = _dbService.UpdateLiveChat(TenantID + "_" + phoneNumber, 1, true).Result;

                result.LiveChatStatusName = "Pending";
                var livechat = _iliveChat.AddLiveChat(TenantID, phoneNumber, TenantID + "_" + phoneNumber, result.displayName, 1, true, Department1, Department2, result.IsOpen, DepartmentId, UserIds);

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
                    //result.customerChat = null;
                    try
                    {
                        var titl = "New Live Chat Request ";
                        var body = "From : " + result.displayName;
                        if (DepartmentId==0&& !string.IsNullOrEmpty(UserIds))
                        {
                            livechat.DepartmentUserIds=UserIds;

                        }
                        // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                        SendMobileNotification(TenantID.Value, titl, body, true, livechat.DepartmentUserIds);
                    }
                    catch (Exception)
                    {

                    }
                    await _LiveChatHubhub.Clients.All.SendAsync("brodCastBotLiveChat", result);
                    SocketIOManager.SendLiveChat(livechat, TenantID.Value);
                }


                return SFormat;
            }
            catch (Exception ex)
            {
                throw ex;
                // return SFormat;
            }


        }
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
            else
            {


                List<string> vs = new List<string>();

        
                vs.Add("10 AM - 11 AM");
                vs.Add("11 AM - 12 PM");
                vs.Add("12 PM - 1 PM");
                vs.Add("1 PM - 2 PM");
                vs.Add("2 PM - 3 PM");
                vs.Add("3 PM - 4 PM");
                vs.Add("4 PM - 5 PM");
                vs.Add("6 PM - 7 PM");
                vs.Add("7 PM - 8 PM");
                vs.Add("8 PM - 9 PM");
                return vs;

            }



        }
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


        #region Public Booking
        public async Task<List<string>> GetUserByBranches(int TenantId, string UserIds)
        {
            var result = _iUserAppService.GetUsersBot(TenantId, UserIds).Result;

            return FillRoleNames(result, TenantId, UserIds);
        }
        public async Task<UserListDto> GetUserModelByBranches(int TenantId, string UserName)
        {
            var result = _iUserAppService.GetUsersBot(TenantId, null).Result;

            var x = result.Where(x => x.Name.Trim()==UserName.Trim()).FirstOrDefault();

            return x;
        }
        public Dictionary<string, string> GetBookingDay(long userId, int languageId)
        {
            var days = _bookingAppService.GetBookingDay(userId, languageId);
            return days;
        }
        public List<string> GetBookingTime(string date, int tenantId, long userId)
        {
            var times = _bookingAppService.GetBookingTime(date, tenantId, userId);
            return times;
        }

        public async Task<string> CreateBooking(BookingModel booking)
        {
            var aaaa = JsonConvert.SerializeObject(booking);
            //var  = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);
            booking.IsNew=true;
            string result = await _bookingAppService.CreateBookingAsync(booking);
            if (result == "booking_success")
            {
                SendMobileNotification(booking.TenantId, "booking", "create booking", true, booking.UserId.ToString());

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
        public List<BookingModel> GetContactBooking(int contactId, int tenantId, int languageId)
        {
            return _bookingAppService.GetContactBooking(contactId, tenantId, languageId);
        }
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
                        massage = booking.BookingDateTimeString + " في " +booking.UserName+"  تم تأكيد الموعد";
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
       
        #endregion

        #region privet
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
        public async Task<TenantModel> GetTenantById(int? id)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
            return tenant;
        }
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
                    if (OrderD.TotalLoyaltyPoints != 0)
                    {
                        listString = listString + captionTotalPointsOfAllText + ((int)OrderD.TotalLoyaltyPoints).ToString() + "\r\n\r\n";
                    }

                    listString = listString + captionTotalOfAllText + ((int)OrderD.Total).ToString() + "\r\n\r\n";
                }
                else
                {

                    if (OrderD.TotalLoyaltyPoints != 0)
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
                    if (TotalLoyaltyPoints != 0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total + Cost).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints != 0)
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
                    if (TotalLoyaltyPoints != 0)
                    {
                        listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
                    }
                    listString = listString + captionTotalText + ((int)Total).ToString() + "\r\n";
                }
                else
                {
                    if (TotalLoyaltyPoints != 0)
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
        private string GetOrderDetailStringFlowsBot(int TenantID, string lang, decimal? Total, decimal? Tax, decimal? TotalLoyaltyPoints, string captionQuantityText, string captionAddtionText, string captionTotalText, string captionTotalOfAllText, List<OrderDetailDto> OrderDetailList, List<ExtraOrderDetailsDto> getOrderDetailExtraList, decimal discount, bool isdiscount, string TypeChoes = "", string DeliveryCostText = "", decimal? Cost = 0)
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

            //if (TypeChoes == "Delivery")
            //{
            //  //  listString = listString + DeliveryCostText + "\r\n";

            //   // listString = listString + "-------------------------- \r\n\r\n";
            //}

            //if (TypeChoes == "Delivery")
            //{
            //    if (TenantID == 46)
            //    {
            //        if (TotalLoyaltyPoints!=0)
            //        {
            //            listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
            //        }
            //        listString = listString + captionTotalText + ((int)Total + Cost).ToString() + "\r\n";
            //    }
            //    else
            //    {
            //        if (TotalLoyaltyPoints!=0)
            //        {
            //            listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
            //        }
            //        listString = listString + captionTotalText + (Total + Cost).ToString() + "\r\n";
            //    }
            //}
            //else
            //{
            //    if (TenantID == 46)
            //    {
            //        if (TotalLoyaltyPoints!=0)
            //        {
            //            listString = listString + captionTotalPointsText + ((int)TotalLoyaltyPoints).ToString() + "\r\n";
            //        }
            //        listString = listString + captionTotalText + ((int)Total).ToString() + "\r\n";
            //    }
            //    else
            //    {
            //        if (TotalLoyaltyPoints!=0)
            //        {
            //            listString = listString + captionTotalPointsText + TotalLoyaltyPoints.ToString() + "\r\n";
            //        }
            //        listString = listString + captionTotalText + Total.ToString() + "\r\n";
            //    }

            //}



            //listString = listString + "-------------------------- \r\n";
            //if (isdiscount)
            //{
            //    if (lang == "ar")
            //    {

            //        listString = listString + "بعد خصم :" + discount + " %" + "\r\n\r\n";
            //    }
            //    else
            //    {
            //        listString = listString + "After Discount :" + discount + " %" + "\r\n\r\n";
            //    }


            //}

            return listString;
        }
        private List<ItemDto> GetItem(int? TenantID, long id)
        {

            List<ItemDto> itemDtos = new List<ItemDto>();
            ItemDto item = _itemsAppService.GetItemInfoForBot(id, TenantID.Value);
            itemDtos.Add(item);
            return itemDtos;
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
        private async Task UpdateOrderAfterCancel(string OrderNumber, int ContactId, Infoseed.MessagingPortal.Orders.Order OrderModel, int? TenantID)
        {
            //var con = GetContact(ContactId);
            var con = _generalAppService.GetContactbyId(ContactId);

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Orders SET  OrderStatus = @OrI, OrderRemarks=@Rema ,ActionTime=@ActionTime  Where Id = @Id";
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
            OrderModel.ActionTime =  DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);

            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), OrderModel.orderStatus);
            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), OrderModel.OrderType);
            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
            // var GetOrderMap = ObjectMapper.Map(OrderModel, getOrderForViewDto.Order);
            var GetOrderMap = _mapper.Map(OrderModel, getOrderForViewDto.Order);

            getOrderForViewDto.Order = GetOrderMap;
            getOrderForViewDto.ActionTime=OrderModel.ActionTime.Value.ToString("MM/dd hh:mm tt");

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

            //SendMobileCancelNotification(TenantID, titl, body);
            SendMobileNotification(TenantID.Value, titl, body, false, null, true);


        }
        private void SendMobileNotification(int TenaentId, string title, string msg, bool islivechat = false, string userIds = null, bool isCancelOrder = false)
        {



            var tenant = GetTenantById(TenaentId).Result;
            if (tenant.IsBellOn)
            {
                //var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
                //httpWebRequest.ContentType = "application/json";
                //httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
                //httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
                //httpWebRequest.Method = "POST";



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

               
                    var tokens = GetUserByTeneantId(TenaentId, userIds);
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
                                        channel_id = $"high_importance_channel_{mainSoundAndroid}",
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
                                            sound = mainSoundIOS
                                        }
                                    }
                                },
                                data = new
                                {
                                    title = title,
                                    body = msg,
                                    sound = mainSoundAndroid,
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
                    //    registration_ids = tokenuser,
                    //    data = new
                    //    {

                    //        body = msg,
                    //        title = title,
                    //        sound = mainSoundAndroid,
                    //        apple = new
                    //        {
                    //            sound = mainSoundIOS
                    //        }
                    //    },
                    //    priority = "high",
                    //    payload = new
                    //    {
                    //        aps = new
                    //        {
                    //            sound = mainSoundIOS
                    //        }
                    //    },
                    //    android = new
                    //    {
                    //        notification = new
                    //        {
                    //            channel_id = $"high_importance_channel_{mainSoundAndroid}"
                    //        }
                    //    },
                    //    apns = new
                    //    {

                    //        header = new
                    //        {
                    //            priority = "10"
                    //        },
                    //        payload = new
                    //        {
                    //            aps = new
                    //            {
                    //                sound = mainSoundIOS
                    //            }
                    //        }
                    //    }
                    //,
                    //    notification = new
                    //    {
                    //        body = msg,
                    //        content_available = true,
                    //        priority = "high",
                    //        title = title,
                    //        sound = mainSoundAndroid,
                    //        apns = new
                    //        {
                    //            payload = new
                    //            {
                    //                aps = new
                    //                {
                    //                    sound = mainSoundIOS
                    //                }
                    //            }
                    //        }
                    //    },
                    //};
                    //var serializer = new JavaScriptSerializer();
                    //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    //{
                    //    string json = serializer.Serialize(payload);
                    //    streamWriter.Write(json);
                    //    streamWriter.Flush();
                    //}


                    //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //var streamReader = new StreamReader(httpResponse.GetResponseStream());
                    //var result = streamReader.ReadToEnd();
                
            }
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
            var x = await response.Content.ReadAsStringAsync();


            // SendNotificationAsync(byteArray);
        }
        private async Task SendMobileNotificationAsync22(int TenaentId, string title, string msg, bool islivechat = false, string userIds = null, bool isCancelOrder = false)
        {
            try
            {
                var credential9 = GoogleCredential.FromFile("C:/Users/user/source/repos/SeedFood/aspnet-core/Infoseed.MessagingPortal.BotAPI/wwwroot/Firebase/info-seed-prod-firebase-adminsdk-eibos-ac0022c003.json")
                                                  .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                var token = await credential9.UnderlyingCredential.GetAccessTokenForRequestAsync();

            }
            catch (Exception ex)
            {

            }
            var tenant = GetTenantById(TenaentId).Result;
            if (tenant.IsBellOn)
            {
                // Load the service account key JSON file
                var credential = GoogleCredential.FromFile("C:/Users/user/source/repos/SeedFood/aspnet-core/Infoseed.MessagingPortal.BotAPI/wwwroot/Firebase/info-seed-prod-firebase-adminsdk-eibos-ac0022c003.json")
                                                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                // Obtain an access token
                var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
                        token = tokenuser.FirstOrDefault(),
                        notification = new
                        {
                            title = title,
                            body = msg,
                        },
                        android = new
                        {
                            notification = new
                            {
                                sound = mainSoundAndroid,
                                channel_id = $"high_importance_channel_{mainSoundAndroid}"
                            }
                        },
                        apns = new
                        {
                            payload = new
                            {
                                aps = new
                                {
                                    sound = mainSoundIOS
                                }
                            },
                            headers = new
                            {
                                apns_priority = "10"
                            }
                        },
                        data = new
                        {
                            body = msg,
                            title = title,
                            sound = mainSoundAndroid
                        }
                    };

                    var jsonPayload = JObject.FromObject(payload).ToString();
                    var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://fcm.googleapis.com/v1/projects/info-seed-prod/messages:send", content);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
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

        private List<AttachmentBotAPIModel> FillAttachmentsTicketData(AttachmentModel[] attachments)
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

                sellingRequestDto = new SellingRequestDto()
                {

                    TenantId = updateOrderModel.TenantID,
                    ContactId = updateOrderModel.ContactId,
                    ContactInfo = updateOrderModel.ContactInformation,
                    PhoneNumber = updateOrderModel.PhoneNumber,
                    CreatedOn = DateTime.UtcNow,
                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
                    SellingStatusId = 1,
                    RequestDescription = updateOrderModel.information,
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
        private SellingRequestDto AddSellingRequest(SellingRequestDto sellingRequestDto)
        {
            var jsonModel = JsonConvert.SerializeObject(sellingRequestDto).ToString();
            SellingRequestDto sellingRequest = _iSellingRequestAppService.AddSellingRequest(sellingRequestDto);

            return sellingRequest;

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
            }


        }
        #endregion


        #region privet Booking
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
        private async Task<string> SendNot(int TenantId, int UserId, string massage, NotificationSeverity notificationSeverity)
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
        private async Task<UserNotification> SendNotfAsync(string message, long agentID, int tenantId, NotificationSeverity notificationSeverity)
        {
            var userIdentifier = ToUserIdentifier(tenantId, agentID);

            await _appNotifier.SendMessageAsync(userIdentifier, message, notificationSeverity);

            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
              userIdentifier);

            notifications.Sort((x, y) => DateTime.Compare(x.Notification.CreationTime, y.Notification.CreationTime));

            var notf = notifications.ToArray().Last();

            return notf;
        }
        private UserIdentifier ToUserIdentifier(int? TargetTenantId, long TargetUserId)
        {
            return new UserIdentifier(TargetTenantId, TargetUserId);
        }
        #endregion


        #region Public Vicale
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
        public void updateContcatDisplayName(int id, string contactDisplayName)
        {
            try
            {
                _cusomerBehaviourAppService.UpdateContactName(id, contactDisplayName);
            }
            catch
            {


            }

        }
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
        public async Task SendPrescription(UpdateSaleOfferModel updateOrderModel)
        {


            if (updateOrderModel.DepartmentId==null)
            {
                updateOrderModel.DepartmentId=0;
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
            sellingRequestDto.AreaId = updateOrderModel.AreaId;
            var objSellingRequestDto = AddSellingRequest(sellingRequestDto);

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
                var titl = "New Selling Request ";
                var body = "From : " + updateOrderModel.ContactName;

                // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
                SendMobileNotification(updateOrderModel.TenantID, titl, body, false, objSellingRequestDto.DepartmentUserIds);
            }
            catch (Exception)
            {

            }
            SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);



        }
        public void SendNewPrescription([FromBody] UpdateSaleOfferModel updateOrderModel, string JolInformation = null)
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

            List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
            SellingRequestDto sellingRequestDto =  PreperNewSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto).Result;
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
                SendMobileNotification(updateOrderModel.TenantID, titl, body, false, objSellingRequestDto.UserIds);
            }
            catch (Exception)
            {

            }



            //SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);

            //if (updateOrderModel.TenantID == 220)
            //{
            //    try
            //    {
            //        var client = new HttpClient();
            //        var request = new HttpRequestMessage(HttpMethod.Post, "https://loyalty.jppmc.jo/loyalty/public/api/user/send-mail?classification=" + JolInformation + "&name=" + updateOrderModel.ContactName + "&phone=" + updateOrderModel.PhoneNumber + "&information=" + updateOrderModel.information);
            //        request.Headers.Add("Authorization", "Bearer WhatsAppSendEmail");
            //        var response = client.SendAsync(request).Result;
            //        response.EnsureSuccessStatusCode();
            //        // Console.WriteLine(await response.Content.ReadAsStringAsync());
            //    }
            //    catch (Exception ex)
            //    {
            //        //throw ex;
            //    }
            //}
        }
        public List<GetAssetModel> GetAssetLevel(int tenantId, string local, int stepId, int levelId = 0)
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
                                result.Add(getAssetModel);
                            }
                            else
                            {
                                getAssetModel.Key = item.Id;
                                getAssetModel.Value = item.LevelTwoNameEn.Trim();
                                result.Add(getAssetModel);
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
        public void CreateInterestedOf(CustomerInterestedOf interestedOf)
        {

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
                }


            }
            catch (Exception)
            {

            }


        }
        #endregion


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

                ///str.AddRange(updateOrderModel.information.Split(','));



                string input = updateOrderModel.information;

                string pattern = @"(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)";
                Match match = Regex.Match(input, pattern);

                if (match.Success )
                {
                    string latitude = match.Groups[1].Value;
                    string longitude = match.Groups[3].Value;

                    input=input.Replace("https://maps.google.com/?q=","");

                    newInformation ="الموقع :"+ "https://maps.google.com/?q="+latitude+"%20 "+longitude+",";



                    input=input.Replace(latitude+",", "");
                    input=input.Replace(longitude+",", "");



                    //Console.WriteLine($"Latitude: {latitude}");
                    // Console.WriteLine($"Longitude: {longitude}");
                }
                else
                {
                    //Console.WriteLine("The string does not match the coordinates format.");
                }


                var inputs = input.Split(",");

                foreach (var item in inputs)
                {

                    if (item!="")
                    {
                        newInformation += item + ",";

                    }

               

                }


                //Remove the trailing comma, if any
                  if (newInformation.EndsWith(","))
                  {
                      newInformation = newInformation.Substring(0, newInformation.Length - 1);
                  }




                //foreach (var item in str)
                //{
                //    // Trim leading and trailing spaces
                //    string trimmedItem = item.Trim();
                //    if (trimmedItem.Contains("Is_Location"))
                //    {
                //        trimmedItem = trimmedItem.Replace("Is_Location", " ");
                //        trimmedItem = trimmedItem.Replace("الموقع", "");

                //        var number1 = trimmedItem.Split(" ")[0];
                //        var number2 = trimmedItem.Split(" ")[1];

                //        if (double.TryParse(number1, out double parsedValue))
                //        {
                //            if (double.TryParse(number2, out double parsedValue2))
                //            {
                //                newInformation += "الموقع:https://maps.google.com/?q=" + trimmedItem + ",";
                //            }
                //            else
                //            {
                //                newInformation += "الموقع: " + trimmedItem + ",";
                //            }
                //        }
                //        else
                //        {
                //            newInformation += "الموقع: " + trimmedItem + ",";
                //        }
                //    }
                //    else
                //    {
                //        if (trimmedItem.Contains("الموقع"))
                //        {
                //            trimmedItem = trimmedItem.Replace("الموقع", "الموقع: ");
                //            newInformation += trimmedItem + ",";
                //        }
                //        else
                //        {
                //            newInformation += trimmedItem + ",";
                //        }
                //    }
                //}


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

            var livechat = _iliveChat.AddNewLiveChat(sellingRequestDto.TenantId, sellingRequestDto.PhoneNumber, sellingRequestDto.TenantId + "_" + sellingRequestDto.PhoneNumber, result.displayName, 1, true, type, Department1, Department2, result.IsOpen, (int)sellingRequestDto.DepartmentId, sellingRequestDto.UserIds, customerLiveChatModel);

            livechat.TenantId = sellingRequestDto.TenantId;
            SellingRequestDto sellingRequest = _iSellingRequestAppService.AddSellingRequest(sellingRequestDto);
           // Thread.Sleep(1000);

            SocketIOManager.SendLiveChat(livechat, sellingRequestDto.TenantId);

         

            return sellingRequest;

        }


        #region GoogleSheets

        #region private 
        private GoogleSheetConfigDto GoogleSheetConfigGet(int? tenantId)
        {
            try
            {
                var result = new GoogleSheetConfigDto();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GoogleSheetConfigGet", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                result.TenantId = reader.GetInt32(reader.GetOrdinal("TenantId"));
                                result.AccessToken = reader["AccessToken"] as string;
                                result.RefreshToken = reader["RefreshToken"] as string;
                                result.IsConnected = reader["IsConnected"] != DBNull.Value ? (bool)reader["IsConnected"] : (bool?)null;
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

        private void GoogleSheetConfigUpdate(string accessToken, string refreshToken, bool? isConnected, int tenantId)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GoogleSheetConfigUpdate";

                        command.Parameters.AddWithValue("@TenantId", (object)tenantId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);
                        command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                        command.Parameters.AddWithValue("@IsConnected", isConnected);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task RefreshAccessTokenAsync(int tenantId)
        {
            string clientId = AppSettingsModel.googleSheetClientId;
            string clientSecret = AppSettingsModel.googleSheetClientSecret;
            string tokenUrl = "https://oauth2.googleapis.com/token";

            var config = GoogleSheetConfigGet(tenantId);
            var refreshToken = config.RefreshToken;

            using (var httpClient = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                    };

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(tokenUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to refresh token: {responseBody}");
                }

                var json = JsonDocument.Parse(responseBody);
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var newRefreshToken = json.RootElement.TryGetProperty("refresh_token", out var rtElement)
                    ? rtElement.GetString()
                    : refreshToken;

                // Only update refresh token if it's actually new
                if (!string.IsNullOrEmpty(newRefreshToken) && newRefreshToken != refreshToken)
                {
                    GoogleSheetConfigUpdate(accessToken, newRefreshToken, true, tenantId);
                }
                else
                {
                    GoogleSheetConfigUpdate(accessToken, refreshToken, true, tenantId);
                }

            }
        }

        private bool IsFirstRowLikelyHeader(List<string> headers)
        {
            int validHeaderCount = headers.Count(h =>
                !string.IsNullOrWhiteSpace(h) &&
                !double.TryParse(h, out _) &&
                h.Length <= 50
            );

            return validHeaderCount >= 2;
        }

        #endregion

        //[HttpGet("GetSheetValues")]
        public async Task<List<IList<object>>> GetSheetValues(string spreadsheetId, string sheetName, string lookUpColumn, string filterValue, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{Uri.EscapeDataString(sheetName)}";

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{Uri.EscapeDataString(sheetName)}";

                        response = await httpClient.GetAsync(url);
                    }
                    else
                    {
                        throw new Exception($"Google Sheets API error: {await response.Content.ReadAsStringAsync()}");
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                dynamic sheetData = JsonConvert.DeserializeObject(content);

                var filteredRows = new List<IList<object>>();

                if (sheetData.values != null)
                {
                    var allRows = ((IEnumerable<dynamic>)sheetData.values)
                                    .Select(row => ((IEnumerable<dynamic>)row).Cast<object>().ToList())
                                    .ToList();

                    if (allRows.Count == 0)
                    {
                        return filteredRows;
                    }

                    var headerRow = allRows[0];
                    filteredRows.Add(headerRow);

                    int columnIndex = headerRow.FindIndex(h =>
                        h?.ToString().Equals(lookUpColumn, StringComparison.OrdinalIgnoreCase) == true);

                    foreach (var row in allRows.Skip(1))
                    {
                        if (row.Count > columnIndex &&
                            row[columnIndex]?.ToString().Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            filteredRows.Add(row);
                        }
                    }
                }

                return filteredRows;
            }
        }

        //[HttpPost("InsertRow")]
        public async Task<string> InsertRow([FromBody] InsertSheetRowDto rowDto)
        {
            var config = GoogleSheetConfigGet(rowDto.tenantId);
            var accessToken = config.AccessToken;

            using (var httpClient = new HttpClient())
            {
                //first get all column names ((headers))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!1:1";

                var headerResponse = await httpClient.GetAsync(headerUrl);

                if (!headerResponse.IsSuccessStatusCode)
                {
                    if (headerResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(rowDto.tenantId);
                        config = GoogleSheetConfigGet(rowDto.tenantId);
                        accessToken = config.AccessToken;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!1:1";
                        headerResponse = await httpClient.GetAsync(headerUrl);

                        if (!headerResponse.IsSuccessStatusCode)
                        {
                            return $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}";
                        }
                    }
                    else
                    {
                        return $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}";
                    }
                }

                var headerContent = await headerResponse.Content.ReadAsStringAsync();
                dynamic headerData = JsonConvert.DeserializeObject(headerContent);
                var headers = ((IEnumerable<dynamic>)headerData.values[0]).Select(h => h.ToString()).ToList();

                //order the row to match the order of column names
                var orderedRow = headers.Select(h => rowDto.row.ContainsKey(h) ? rowDto.row[h] : "").ToList();

                var appendUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{rowDto.spreadsheetId}/values/{rowDto.sheetName}!A1:append?valueInputOption=RAW";

                var body = new
                {
                    values = new List<IList<object>> { orderedRow }
                };
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var appendResponse = await httpClient.PostAsync(appendUrl, content);

                var appendResult = await appendResponse.Content.ReadAsStringAsync();

                if (!appendResponse.IsSuccessStatusCode)
                {
                    return $"Insert failed: {appendResult}";
                }

                return "Row inserted successfully.";

            }
        }

        //[HttpGet("GetWorkSheets")]
        public async Task<List<string>> GetWorkSheets(string spreadsheetId, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            var url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RefreshAccessTokenAsync(tenantId);
                    config = GoogleSheetConfigGet(tenantId);
                    accessToken = config.AccessToken;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}";

                    response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new List<string> { $"Google Sheets API error: {await response.Content.ReadAsStringAsync()}" };
                    }
                }
                else
                {
                    return new List<string> { $"Google Sheets API error: {await response.Content.ReadAsStringAsync()}" };
                }
            }

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);

            var sheetTitles = doc.RootElement
                                 .GetProperty("sheets")
                                 .EnumerateArray()
                                 .Select(sheet => sheet.GetProperty("properties").GetProperty("title").GetString())
                                 .ToList();

            return sheetTitles;
        }

        //[HttpGet("GetLookupHeaders")]
        public async Task<List<string>> GetLookupHeaders(string spreadsheetId, string sheetName, int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var headerUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{sheetName}!1:1";

                HttpResponseMessage headerResponse = await httpClient.GetAsync(headerUrl);

                if (!headerResponse.IsSuccessStatusCode)
                {
                    if (headerResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        headerResponse = await httpClient.GetAsync(headerUrl);
                    }

                    if (!headerResponse.IsSuccessStatusCode)
                    {
                        return new List<string> { $"Failed to get headers: {await headerResponse.Content.ReadAsStringAsync()}" };
                    }
                }

                var headerContent = await headerResponse.Content.ReadAsStringAsync();
                var headerData = JsonConvert.DeserializeObject<SheetValueResponse>(headerContent);
                var headers = headerData?.Values?.FirstOrDefault()?.Select(h => h?.ToString())?.ToList() ?? new List<string>();

                if (!IsFirstRowLikelyHeader(headers))
                {
                    return new List<string> { "Invalid sheet structure: Expected column names in the first row." };
                }

                return headers;
            }
        }

        //[HttpGet("GetSpreadSheets")]
        public async Task<GetSpreadSheetsDto> GetSpreadSheets(int tenantId)
        {
            var config = GoogleSheetConfigGet(tenantId);
            var accessToken = config.AccessToken;
            var spreadSheetResult = new GetSpreadSheetsDto();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var url = "https://www.googleapis.com/drive/v3/files?q=mimeType='application/vnd.google-apps.spreadsheet'";

                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        await RefreshAccessTokenAsync(tenantId);
                        config = GoogleSheetConfigGet(tenantId);
                        accessToken = config.AccessToken;

                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        url = "https://www.googleapis.com/drive/v3/files?q=mimeType='application/vnd.google-apps.spreadsheet'";

                        response = await httpClient.GetAsync(url);

                        if (!response.IsSuccessStatusCode)
                        {
                            spreadSheetResult.ErrorMsg = $"Google Sheets API error: {await response.Content.ReadAsStringAsync()}";
                            return spreadSheetResult;
                        }
                    }
                    else
                    {
                        spreadSheetResult.ErrorMsg = $"{await response.Content.ReadAsStringAsync()}";
                        return spreadSheetResult;
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonConvert.DeserializeObject<DriveApiResponse>(content);
                    spreadSheetResult.Files = json?.Files ?? new List<GoogleDriveFile>();
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    spreadSheetResult.ErrorMsg = $"Invalid JSON returned from API: {content}";
                }

                return spreadSheetResult;
            }
        }

        #endregion

    }
}

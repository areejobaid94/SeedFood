//using Infoseed.MessagingPortal.Branches;
//using Infoseed.MessagingPortal.Web.Models.Sunshine;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Infoseed.MessagingPortal.Web.Sunshine;
//using Infoseed.MessagingPortal.OrderDetails.Dtos;
//using Infoseed.MessagingPortal.Orders;
//using Infoseed.MessagingPortal.Items.Dtos;
//using Infoseed.MessagingPortal.Web.Models;
//using Infoseed.MessagingPortal.Orders.Dtos;
//using Microsoft.AspNetCore.SignalR;
//using Infoseed.MessagingPortal.Contacts;
//using Abp.Domain.Repositories;
//using Infoseed.MessagingPortal.Areas;
//using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
//using Infoseed.MessagingPortal.Web.Models.Location;
//using Framework.Data;
//using Infoseed.MessagingPortal.CaptionBot;
//using Abp.Notifications;
//using Abp;
//using Infoseed.MessagingPortal.Notifications;

//using Infoseed.MessagingPortal.Evaluations;
//using System.Net;
//using Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos;
//using Microsoft.Extensions.Configuration;
//using System.Globalization;
//using Infoseed.MessagingPortal.OrderDetails;

//using System.Linq.Dynamic.Core;

//using Infoseed.MessagingPortal.Web.Models.BotModel;
//using Infoseed.MessagingPortal.Booking;
//using Infoseed.MessagingPortal.MultiTenancy.Dto;

//using System.Net.Http.Headers;
//using Newtonsoft.Json;
//using System.Web;
//using Infoseed.MessagingPortal.Maintenance;
//using Infoseed.MessagingPortal.Maintenance.Dtos;
//using Infoseed.MessagingPortal.Areas.Dtos;
//using GeoCoordinatePortable;
//using Nancy.Json;
//using System.IO;
//using Infoseed.MessagingPortal.Authorization.Users;
//using Infoseed.MessagingPortal.Web.Models.Firebase;
//using Infoseed.MessagingPortal.Authorization.Users.Dto;
//using Infoseed.MessagingPortal.Items;
//using System.Text.Json;
//using Microsoft.ApplicationInsights;
//using Infoseed.MessagingPortal.Web.WhatsAppDialog;
//using Infoseed.MessagingPortal.LiveChat;
//using Infoseed.MessagingPortal.Asset;
//using Infoseed.MessagingPortal.SealingReuest;
//using Infoseed.MessagingPortal.SealingReuest.Dto;
//using Abp.Specifications;
//using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
//using Infoseed.MessagingPortal.DeliveryCost;
//using Infoseed.MessagingPortal.DeliveryCost.Dto;
//using Infoseed.MessagingPortal.Orders.Exporting;
//using Infoseed.MessagingPortal.Contacts.Dtos;
//using Infoseed.MessagingPortal.Web.Helper;
//using Infoseed.MessagingPortal.SocketIOClient;
//using Infoseed.MessagingPortal.Configuration.Tenants;
//using Infoseed.MessagingPortal.MgSystem;

//namespace Infoseed.MessagingPortal.Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NewBotAPIController : MessagingPortalControllerBase
//    {
//        private string serverKey = "AAAAKInP3fI:APA91bHUaWCGK8ea8261aoGceJ4uGd7mJLHAl5vUMDiUFskG_t5iImtYqYMaLHZpiRHyFNAA2Cl4zEQ18Gnq6pRbP57PEhw1Z8DYIL9G07wqvZcaW1Gg7gbqqOvY403IzoJfQXx-CqSi";
//        private string senderId = "174110793202";
//        private string webAddr = "https://fcm.googleapis.com/fcm/send";
//        private readonly IRepository<Items.Item, long> _lookup_ItemRepository;
//        private readonly IRepository<OrderDetail, long> _orderDetailRepository;
//        private static readonly object CurrentOrder = new object();
//        private IAppNotifier _appNotifier;
//        private IUserNotificationManager _userNotificationManager;
//        private IRepository<Contact> _contactRepos;
//        //private IHubContext<OrderHub> _hub;
//      //  private IHubContext<SignalR.TeamInboxHub> _hub2;
//        private IHubContext<SignalR.TestSingalRhub> _TestSingalRhub;

//        private IHubContext<LiveChatHub> _LiveChatHubhub;
//        //private IHubContext<EvaluationHub> _evaluationHub;
//        private IHubContext<MaintenancesHub> _maintenanceshub;
//        private IHubContext<SellingRequestHub> _sellingRequestHub;
        
//        private IAreasAppService _iAreasAppService;
//        private IItemsAppService _itemsAppService;

//        private IUserAppService _iUserAppService;

//        private TelemetryClient _telemetry;
//        private ISellingRequestAppService _iSellingRequestAppService;

//        private IAssetAppService _iAssetAppService;
//        private IOrdersAppService _iOrdersAppService;
//        private IDeliveryCostAppService _iDeliveryCostAppService;
//        private IContactsAppService _iContactsAppService;
//        private ILiveChatAppService  _iliveChat;

//        IDBService _dbService;
//        private readonly IRepository<Evaluation, long> _evaluationRepository;
//        private IConfiguration _configuration;
//        private readonly IRepository<Bookings.Booking> _bookingRepository;


//        public  IRepository<Caption, long> _captionRepository;
//        public  IRepository<OrderOffers.OrderOffer, long> _orderOfferRepository;

//        public NewBotAPIController(IHubContext<SellingRequestHub> sellingRequestHub,IHubContext<LiveChatHub> LiveChatHubhub, TelemetryClient telemetry, IHubContext<MaintenancesHub> maintenanceshub, IRepository<Items.Item, long> lookup_ItemRepository, IRepository<OrderDetail, long> orderDetailRepository,
//            IConfiguration configuration, IDBService dbService, 
//            //IHubContext<OrderHub> hub,
//            IRepository<Contact> contactRepos, IAppNotifier appNotifier, IUserNotificationManager userNotificationManager, IRepository<Evaluation, long> evaluationRepository,
//           /// IHubContext<EvaluationHub> evaluationHub,
//           // IHubContext<SignalR.TeamInboxHub> hub2,
//            IRepository<Bookings.Booking> bookingRepository
//            , IAreasAppService iAreasAppService,
//            IUserAppService iUserAppService,
//            IItemsAppService itemsAppService,
//            ISellingRequestAppService iSellingRequestAppService,
//            IAssetAppService  iAssetAppService,
//            IOrdersAppService iOrdersAppService,
//            IDeliveryCostAppService  iDeliveryCostAppService,
//            ILiveChatAppService iliveChat,
//        IHubContext<SignalR.TestSingalRhub> TestSingalRhub,
//        IContactsAppService iContactsAppService,
//        IRepository<Caption, long> captionRepository,
//        IRepository<OrderOffers.OrderOffer, long> orderOfferRepository

//            )
//        {
//            _captionRepository=captionRepository;
//            _orderOfferRepository=orderOfferRepository;
//            _LiveChatHubhub = LiveChatHubhub;
//            _sellingRequestHub = sellingRequestHub;
//            _telemetry = telemetry;
//            _lookup_ItemRepository = lookup_ItemRepository;
//            _orderDetailRepository = orderDetailRepository;
//            _orderDetailRepository = orderDetailRepository;
//            _configuration = configuration;
//            _userNotificationManager = userNotificationManager;
//            _appNotifier = appNotifier;
//            _contactRepos = contactRepos;
//            _dbService = dbService;
//           // _hub = hub;
//           // _hub2 = hub2;
//           // _evaluationHub = evaluationHub;
//            _evaluationRepository = evaluationRepository;
//            _bookingRepository = bookingRepository;
//            _maintenanceshub = maintenanceshub;
//            _iAreasAppService = iAreasAppService;
//            _iUserAppService = iUserAppService;
//            _itemsAppService = itemsAppService;
//            _iSellingRequestAppService = iSellingRequestAppService;
//            _iAssetAppService = iAssetAppService;
//            _iOrdersAppService = iOrdersAppService;
//            _iDeliveryCostAppService = iDeliveryCostAppService;
//            _TestSingalRhub = TestSingalRhub;
//            _iContactsAppService = iContactsAppService;
//            _iliveChat=iliveChat;

//        }



//        #region public 


//        [Route("UpdateCustomerBehavior")]
//        [HttpPost]
//        public void UpdateCustomerBehavior(int? TenantID, int ContactId, bool Stop, bool Start)
//        {
//            try
//            {
//                var SP_Name = Constants.Contacts.SP_CustomerBehaviourUpdate;

//                var sqlParameters = new List<SqlParameter> {
//                    new SqlParameter("@Stop", Stop),
//                    new SqlParameter("@Start", Start),
//                    new SqlParameter("@ContactId", ContactId),
//                    new SqlParameter("@TenantID", TenantID),
//                };

//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }

//        }
//        [Route("CreateInterestedOf")]
//        [HttpPost]
//        public void CreateInterestedOf(int? TenantID, int ContactId, int levleOneId, int? levelTwoId = 0 , int? levelThreeId = 0)
//        {
//            try
//            {
//                var SP_Name = Constants.Contacts.SP_ContactsInterrestedOfAdd;

//                var sqlParameters = new List<SqlParameter> { 
//                    new SqlParameter("@levleOneId", levleOneId),
//                    new SqlParameter("@levelTwoId", levelTwoId),
//                    new SqlParameter("@levelThreeId", levelThreeId),
//                    new SqlParameter("@ContactId", ContactId),
//                    new SqlParameter("@TenantID", TenantID),
//                };

//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }
//        }

//        [HttpPost]
//        [Route("UpdateGeneralSettingsMobile")]
//        public async Task UpdateGeneralSettingsMobile(TenantInformationDto input)
//        {
//            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

//            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == input.TenantId);
//            switch ((EunmFlagSetting)input.flag)
//            {
//                case EunmFlagSetting.GeneralSettings:
//                    {

//                        tenant.CancelTime = input.CancelTime;
//                        tenant.IsCancelOrder = input.IsCancelOrder;

//                        tenant.IsBotActive = input.IsBotActive;

//                        tenant.IsEvaluation = input.IsEvaluation;
//                        tenant.EvaluationText = input.EvaluationText;

//                        tenant.EvaluationTime = input.EvaluationTime;
//                        tenant.DeliveryCostTypeId=input.DeliveryCostTypeId;
//                        await itemsCollection.UpdateItemAsync(tenant._self, tenant);
//                        break;
//                    }
//                case EunmFlagSetting.WorkingHour:
//                    {

//                        tenant.IsWorkActive = input.IsWorkActive;
//                        tenant.workModel = new Models.Sunshine.WorkModel
//                        {
//                            EndDateFri = input.workModel.EndDateFri.AddHours(AppSettingsModel.AddHour),
//                            EndDateMon = input.workModel.EndDateMon.AddHours(AppSettingsModel.AddHour),
//                            EndDateSat = input.workModel.EndDateSat.AddHours(AppSettingsModel.AddHour),
//                            EndDateSun = input.workModel.EndDateSun.AddHours(AppSettingsModel.AddHour),
//                            EndDateThurs = input.workModel.EndDateThurs.AddHours(AppSettingsModel.AddHour),
//                            EndDateTues = input.workModel.EndDateTues.AddHours(AppSettingsModel.AddHour),
//                            EndDateWed = input.workModel.EndDateWed.AddHours(AppSettingsModel.AddHour),

//                            EndDateFriSP = input.workModel.EndDateFriSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateMonSP = input.workModel.EndDateMonSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateSatSP = input.workModel.EndDateSatSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateSunSP = input.workModel.EndDateSunSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateThursSP = input.workModel.EndDateThursSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateTuesSP = input.workModel.EndDateTuesSP.AddHours(AppSettingsModel.AddHour),
//                            EndDateWedSP = input.workModel.EndDateWedSP.AddHours(AppSettingsModel.AddHour),



//                            IsWorkActiveFri = input.workModel.IsWorkActiveFri,
//                            IsWorkActiveMon = input.workModel.IsWorkActiveMon,
//                            IsWorkActiveSat = input.workModel.IsWorkActiveSat,
//                            IsWorkActiveSun = input.workModel.IsWorkActiveSun,
//                            IsWorkActiveThurs = input.workModel.IsWorkActiveThurs,
//                            IsWorkActiveTues = input.workModel.IsWorkActiveTues,
//                            IsWorkActiveWed = input.workModel.IsWorkActiveWed,

//                            StartDateFri = input.workModel.StartDateFri.AddHours(AppSettingsModel.AddHour),
//                            StartDateMon = input.workModel.StartDateMon.AddHours(AppSettingsModel.AddHour),
//                            StartDateSat = input.workModel.StartDateSat.AddHours(AppSettingsModel.AddHour),
//                            StartDateSun = input.workModel.StartDateSun.AddHours(AppSettingsModel.AddHour),
//                            StartDateThurs = input.workModel.StartDateThurs.AddHours(AppSettingsModel.AddHour),
//                            StartDateTues = input.workModel.StartDateTues.AddHours(AppSettingsModel.AddHour),
//                            StartDateWed = input.workModel.StartDateWed.AddHours(AppSettingsModel.AddHour),

//                            StartDateFriSP = input.workModel.StartDateFriSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateMonSP = input.workModel.StartDateMonSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateSatSP = input.workModel.StartDateSatSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateSunSP = input.workModel.StartDateSunSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateThursSP = input.workModel.StartDateThursSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateTuesSP = input.workModel.StartDateTuesSP.AddHours(AppSettingsModel.AddHour),
//                            StartDateWedSP = input.workModel.StartDateWedSP.AddHours(AppSettingsModel.AddHour),


//                            WorkTextFri = input.workModel.WorkTextFri,
//                            WorkTextMon = input.workModel.WorkTextMon,
//                            WorkTextSat = input.workModel.WorkTextSat,
//                            WorkTextSun = input.workModel.WorkTextSun,
//                            WorkTextThurs = input.workModel.WorkTextThurs,
//                            WorkTextTues = input.workModel.WorkTextTues,
//                            WorkTextWed = input.workModel.WorkTextWed

//                        };
//                        await itemsCollection.UpdateItemAsync(tenant._self, tenant);
//                        break;
//                    }
//                case EunmFlagSetting.LiveChatWorkingHour:
//                    {

//                        tenant.IsLiveChatWorkActive = input.IsLiveChatWorkActive;
//                        tenant.LiveChatWorkingHours = new Models.Sunshine.WorkModel
//                        {
//                            // EndDateFri = input.workModel.EndDateFri.AddHours(AppSettingsModel.AddHour),
//                            EndDateMon = input.LiveChatWorkingHours.EndDateMon.AddHours(AppSettingsModel.AddHour),
//                            EndDateSat = input.LiveChatWorkingHours.EndDateSat.AddHours(AppSettingsModel.AddHour),
//                            EndDateSun = input.LiveChatWorkingHours.EndDateSun.AddHours(AppSettingsModel.AddHour),
//                            EndDateThurs = input.LiveChatWorkingHours.EndDateThurs.AddHours(AppSettingsModel.AddHour),
//                            EndDateTues = input.LiveChatWorkingHours.EndDateTues.AddHours(AppSettingsModel.AddHour),
//                            EndDateWed = input.LiveChatWorkingHours.EndDateWed.AddHours(AppSettingsModel.AddHour),

//                            IsWorkActiveFri = input.LiveChatWorkingHours.IsWorkActiveFri,
//                            IsWorkActiveMon = input.LiveChatWorkingHours.IsWorkActiveMon,
//                            IsWorkActiveSat = input.LiveChatWorkingHours.IsWorkActiveSat,
//                            IsWorkActiveSun = input.LiveChatWorkingHours.IsWorkActiveSun,
//                            IsWorkActiveThurs = input.LiveChatWorkingHours.IsWorkActiveThurs,
//                            IsWorkActiveTues = input.LiveChatWorkingHours.IsWorkActiveTues,
//                            IsWorkActiveWed = input.LiveChatWorkingHours.IsWorkActiveWed,

//                            //    StartDateFri = input.workModel.StartDateFri.AddHours(AppSettingsModel.AddHour),
//                            StartDateMon = input.LiveChatWorkingHours.StartDateMon.AddHours(AppSettingsModel.AddHour),
//                            StartDateSat = input.LiveChatWorkingHours.StartDateSat.AddHours(AppSettingsModel.AddHour),
//                            StartDateSun = input.LiveChatWorkingHours.StartDateSun.AddHours(AppSettingsModel.AddHour),
//                            StartDateThurs = input.LiveChatWorkingHours.StartDateThurs.AddHours(AppSettingsModel.AddHour),
//                            StartDateTues = input.LiveChatWorkingHours.StartDateTues.AddHours(AppSettingsModel.AddHour),
//                            StartDateWed = input.LiveChatWorkingHours.StartDateWed.AddHours(AppSettingsModel.AddHour),

//                            WorkTextFri = input.LiveChatWorkingHours.WorkTextFri,
//                            WorkTextMon = input.LiveChatWorkingHours.WorkTextMon,
//                            WorkTextSat = input.LiveChatWorkingHours.WorkTextSat,
//                            WorkTextSun = input.LiveChatWorkingHours.WorkTextSun,
//                            WorkTextThurs = input.LiveChatWorkingHours.WorkTextThurs,
//                            WorkTextTues = input.LiveChatWorkingHours.WorkTextTues,
//                            WorkTextWed = input.LiveChatWorkingHours.WorkTextWed

//                        };
//                        await itemsCollection.UpdateItemAsync(tenant._self, tenant);
//                        break;
//                    }
//                case EunmFlagSetting.BotCaption:
//                    {


//                        Caption caption = new Caption
//                        {
//                            Text= input.OneCaption.Text.Replace(" \n", "\r\n"),
//                            Id= input.OneCaption.Id,
//                            TenantId= input.OneCaption.TenantId,
//                            LanguageBotId= input.OneCaption.LanguageBotId,
//                            TextResourceId= input.OneCaption.TextResourceId,
//                             HeaderText=input.OneCaption.HeaderText,

//                        };

//                         await _captionRepository.UpdateAsync(caption);

//                        break;
//                    }
//                case EunmFlagSetting.Condations:
//                    {



//                        OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
//                        {
//                            Id = input.OneOrderOffern.Id,
//                            FeesEnd = input.OneOrderOffern.FeesEnd,
//                            FeesStart = input.OneOrderOffern.FeesStart,
//                            Area = input.OneOrderOffern.Area,
//                            Cities = input.OneOrderOffern.Cities,
//                            NewFees = input.OneOrderOffern.NewFees,
//                            isAvailable = input.OneOrderOffern.isAvailable,
//                            OrderOfferDateEnd = input.OneOrderOffern.OrderOfferDateEnd,
//                            OrderOfferDateStart = input.OneOrderOffern.OrderOfferDateStart,
//                            isPersentageDiscount = input.OneOrderOffern.isPersentageDiscount,
//                            OrderOfferEnd = input.OneOrderOffern.OrderOfferEnd.AddHours(AppSettingsModel.AddHour),
//                            OrderOfferStart = input.OneOrderOffern.OrderOfferStart.AddHours(AppSettingsModel.AddHour),
//                            TenantId= input.TenantId

//                        };

//                         await _orderOfferRepository.UpdateAsync(orderOffer);
//                        break;
//                    }

//            }



//        }


//        [Route("GetCarModel")]
//        [HttpGet]
//        public List<string> GetCarModel(int? tenantID, int lvlOneId, int lvlTwoId)
//        {
           
//            List<string> vs = new List<string>();

//            vs.Add("Hasan 1");
//            vs.Add("Hasan 2");
//            vs.Add("Hasan 3");
//            vs.Add("Hasan 4");
//            //var con = GetContact(ContactId);

//            return vs;
//        }

//        [Route("GetCarAsset")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> GetCarAsset(int? TenantID)
//        {
            
            
//            List<GetListPDFModel> vs = new List<GetListPDFModel>();





//            vs.Add(new GetListPDFModel {
            
//               AttachmentName="ss",
//               AttachmentType="image",
//               AttachmentUrl="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/4a767c02-1a89-4ad6-afd2-8be2dea42baa.jfif",
//               phoneNumber="962779746365",
//               TenantID=45
//            });

//            vs.Add(new GetListPDFModel
//            {

//                AttachmentName="sd",
//                AttachmentType="image",
//                AttachmentUrl="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/4a767c02-1a89-4ad6-afd2-8be2dea42baa.jfif",
//                phoneNumber="962779746365",
//                TenantID=45
//            });
//            return vs;
//        }

      
//        [Route("UpdateLiveChat")]
//        [HttpGet]
//        public async Task<string> UpdateLiveChatAsync(int? TenantID, string phoneNumber, string Department1=null, string Department2=null)
//        {

//            string SFormat = string.Empty;


//            try
//            {
//                var Tenant = GetTenantById(TenantID).Result;
//                // var con = GetContact(contactId);
//                var result = _dbService.UpdateLiveChat(TenantID + "_" + phoneNumber, 1, true).Result;

//                result.LiveChatStatusName ="Pending";
//                var x = _iliveChat.AddLiveChat(TenantID, phoneNumber, TenantID + "_" + phoneNumber, result.displayName, 1, true, Department1, Department2, result.IsOpen);

//                if (Department1!=null)
//                    result.Department=Department1+"-"+Department2;


//                if (Tenant.IsLiveChatWorkActive)
//                {

//                    if (!checkIsInServiceLiveChat(Tenant.LiveChatWorkingHours, out SFormat))
//                    {
//                        return SFormat;
//                    }

//                }






//                if (result != null)
//                {
//                    //result.customerChat = null;
//                    try
//                    {
//                        var titl = "New Live Chat Request ";
//                        var body = "From : "+result.displayName;

//                        // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
//                        SendMobileNotification(TenantID, titl, body);
//                    }
//                    catch (Exception ex)
//                    {

//                    }
//                    await _LiveChatHubhub.Clients.All.SendAsync("brodCastBotLiveChat", result);
//                    SocketIOManager.SendLiveChat(x, TenantID.Value);
//                }


//                return SFormat;
//            }
//            catch(Exception ex)
//            {
//                throw ex;
//               // return SFormat;
//            }
       

//        }

      

//        [Route("UpdateSaleOffer")]
//        [HttpPost]
//        public async Task UpdateSaleOfferAsync([FromBody] UpdateSaleOfferModel updateOrderModel)
//        {
         


//                List<Attachment> AttachmetArray = new List<Attachment>();
//                List<Attachment> AttachmetArrayTow = new List<Attachment>();
//                if (updateOrderModel.AttachmetArray != null )
//                {
//                    AttachmetArray = FillAttachmentsTicketData(updateOrderModel.AttachmetArray);
                    
//                    // ticket.Attachments = Attachments;
//                }
//                if (  updateOrderModel.AttachmetArrayTow != null)
//                {
                    
//                    AttachmetArrayTow = FillAttachmentsTicketData(updateOrderModel.AttachmetArrayTow);
//                    // ticket.Attachments = Attachments;
//                }

//                List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
//                SellingRequestDto sellingRequestDto = await PreperSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto);

//                var objSellingRequestDto = await AddSellingRequest(sellingRequestDto);

//            if (objSellingRequestDto.IsRequestForm)
//            {
//                var options = new JsonSerializerOptions { WriteIndented = true };

//                objSellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(sellingRequestDto.RequestDescription, options);
//            }

//            //result.customerChat = null;
//            try
//            {
//                var titl = "New Selling Request ";
//                var body = "From : "+updateOrderModel.ContactName;

//                // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
//                SendMobileNotification(updateOrderModel.TenantID , titl, body);
//            }
//            catch (Exception ex)
//            {

//            }


//            //await _sellingRequestHub.Clients.All.SendAsync("brodCastSellingRequest", objSellingRequestDto);

//               SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);


//        }
//        [Route("SendPrescription")]
//        [HttpPost]
//        public async Task SendPrescription([FromBody] UpdateSaleOfferModel updateOrderModel)
//        {



//            List<Attachment> AttachmetArray = new List<Attachment>();
//            List<Attachment> AttachmetArrayTow = new List<Attachment>();
//            if (updateOrderModel.AttachmetArray != null)
//            {
//                AttachmetArray = FillAttachmentsTicketData(updateOrderModel.AttachmetArray);

//                // ticket.Attachments = Attachments;
//            }
//            if (updateOrderModel.AttachmetArrayTow != null)
//            {

//                AttachmetArrayTow = FillAttachmentsTicketData(updateOrderModel.AttachmetArrayTow);
//                // ticket.Attachments = Attachments;
//            }

//            List<SellingRequestDetailsDto> SellingRequestDetailsDto = new List<SellingRequestDetailsDto>();
//            SellingRequestDto sellingRequestDto = await PreperSellingRequestAsync(updateOrderModel, AttachmetArray, AttachmetArrayTow, SellingRequestDetailsDto);

//            var objSellingRequestDto = await AddSellingRequest(sellingRequestDto);

//            if (objSellingRequestDto.IsRequestForm)
//            {
//                var options = new JsonSerializerOptions { WriteIndented = true };

//                objSellingRequestDto.RequestForm = System.Text.Json.JsonSerializer.Deserialize<SellingRequestFormModel>(sellingRequestDto.RequestDescription, options);
//            }

//            //await _sellingRequestHub.Clients.All.SendAsync("brodCastSellingRequest", objSellingRequestDto);
//            objSellingRequestDto.CreatedOn = objSellingRequestDto.CreatedOn.AddHours(AppSettingsModel.AddHour);
//            SocketIOManager.SendSellingRequest(objSellingRequestDto, updateOrderModel.TenantID);



//        }
//        private async Task<SellingRequestDto> PreperSellingRequestAsync(UpdateSaleOfferModel updateOrderModel, List<Attachment> AttachmetArray, List<Attachment> AttachmetArrayTow, List<SellingRequestDetailsDto> SellingRequestDetailsDto)
//        {
//            if (AttachmetArray != null)
//            {
//                var types = AppsettingsModel.AttacmentTypesAllowed;
//                foreach (var item in AttachmetArray)
//                {

//                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
//                    AttachmentContent attachmentContent = new AttachmentContent()
//                    {
//                        Content = item.Base64,
//                        Extension = Path.GetExtension(item.Filename),
//                        MimeType = item.FileType,

//                    };

//                    string filepath = System.IO.Path.GetDirectoryName(item.Filename);
//                    var url = await azureBlobProvider.Save(attachmentContent);

//                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
//                    {
//                        DocumentURL = url,
//                        DocumentTypeId = 1,
//                        TenantId = updateOrderModel.TenantID

//                    });


//                }

//            }
//            if ( AttachmetArrayTow != null)
//            {
//                var types = AppsettingsModel.AttacmentTypesAllowed;
//                foreach (var item in AttachmetArrayTow)
//                {

//                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
//                    AttachmentContent attachmentContent = new AttachmentContent()
//                    {
//                        Content = item.Base64,
//                        Extension = Path.GetExtension(item.Filename),
//                        MimeType = item.FileType,

//                    };

//                    string filepath = System.IO.Path.GetDirectoryName(item.Filename);
//                    var url = await azureBlobProvider.Save(attachmentContent);

//                    SellingRequestDetailsDto.Add(new SellingRequestDetailsDto
//                    {
//                        DocumentURL = url,
//                        DocumentTypeId = 2,
//                        TenantId = updateOrderModel.TenantID

//                    });


//                }

//            }
//            SellingRequestDto sellingRequestDto = new SellingRequestDto();
//            if (!updateOrderModel.IsRequestForm)
//            {

//                 sellingRequestDto = new SellingRequestDto()
//                {
//                    TenantId = updateOrderModel.TenantID,
//                    ContactId = updateOrderModel.ContactId,
//                    ContactInfo = updateOrderModel.ContactInformation,
//                    PhoneNumber = updateOrderModel.PhoneNumber,
//                    CreatedOn = DateTime.UtcNow,
//                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
//                    SellingStatusId = 1,
//                    RequestDescription = updateOrderModel.information,
//                    Price = updateOrderModel.Price,
//                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
//                    CreatedBy= updateOrderModel.ContactName,
//                    IsRequestForm= updateOrderModel.IsRequestForm,
//                 };


//            }
//            else
//            {
//                sellingRequestDto = new SellingRequestDto()
//                {
//                    TenantId = updateOrderModel.TenantID,
//                    ContactId = updateOrderModel.ContactId,
//                    ContactInfo = updateOrderModel.ContactInformation,
//                    PhoneNumber = updateOrderModel.PhoneNumber,
//                    CreatedOn = DateTime.UtcNow,
//                    lstSellingRequestDetailsDto = SellingRequestDetailsDto,
//                    SellingStatusId = 1,
//                    RequestDescription = JsonConvert.SerializeObject(updateOrderModel.RequestForm),
//                    Price = updateOrderModel.Price,
//                    UserId = updateOrderModel.TenantID + "_" + updateOrderModel.PhoneNumber,
//                    CreatedBy= updateOrderModel.ContactName,
//                    IsRequestForm= updateOrderModel.IsRequestForm,
//                };

//            }


//            return sellingRequestDto;
//        }

//        [Route("GetAllCaption")]
//        [HttpGet]
//        public List<Caption> GetAllCaption(int TenantID, string local)
//        {
//            int localID = 1;
//            if (local == "en")
//            {
//                localID = 2;

//            }
//            else
//            {
//                localID = 1;
//            }

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + localID;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                List<Caption> captions = new List<Caption>();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    captions.Add(new Caption
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
//                        TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

//                    });
//                }

//                conn.Close();
//                da.Dispose();

//                return captions;

//            }
//            catch
//            {
//                return null;

//            }

//        }



//        [Route("UpdateCancelOrder")]
//        [HttpGet]
//        public CancelOrderModel UpdateCancelOrder(int? TenantID, string OrderNumber, int ContactId, string CanatCancelOrderText)
//        {
//            CancelOrderModel cancelOrderModel = new CancelOrderModel();

//            try
//            {


//                var OrderModel = GetOrderListWithContact(TenantID, ContactId).Where(x => x.OrderNumber == long.Parse(OrderNumber)).FirstOrDefault();

//                if (OrderModel != null)
//                {
//                    var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
//                    TenantModel tenant = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID).Result;


//                    if (tenant.IsCancelOrder)
//                    {

//                        TimeSpan timeSpan = DateTime.Now.AddHours(AppSettingsModel.AddHour) - OrderModel.CreationTime;
//                        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);

//                        if (totalMinutes >= tenant.CancelTime)
//                        {

//                            cancelOrderModel.CancelOrder = true;
//                            cancelOrderModel.WrongOrder = false;
//                            cancelOrderModel.IsTrueOrder = true;
//                            cancelOrderModel.TextCancelOrder = CanatCancelOrderText;
//                            return cancelOrderModel;

//                        }


//                    }

//                    var x = UpdateOrderAfterCancel(OrderNumber, ContactId, OrderModel, TenantID);
//                    cancelOrderModel.CancelOrder = false;
//                    cancelOrderModel.WrongOrder = false;
//                    cancelOrderModel.IsTrueOrder = true;
//                    return cancelOrderModel;


//                }
//                else
//                {

//                    cancelOrderModel.CancelOrder = false;
//                    cancelOrderModel.WrongOrder = true;
//                    cancelOrderModel.IsTrueOrder = false;
//                    return cancelOrderModel;

//                }
//            }
//            catch
//            {

//                cancelOrderModel.CancelOrder = false;
//                cancelOrderModel.WrongOrder = true;
//                cancelOrderModel.IsTrueOrder = false;
//                return cancelOrderModel;
//            }


//        }


//        [Route("UpdateComplaint")]
//        [HttpGet]
//        public void UpdateComplaint(int contactId)
//        {
//            var con = GetContact(contactId);
//            var result = _dbService.UpdateComplaint(con.UserId, 0, true).Result;
//            if (result != null)
//            {
//                result.customerChat = null;
//            //    _hub2.Clients.All.SendAsync("brodCastEndUserMessage", result);
//                SocketIOManager.SendContact(result, con.TenantId.HasValue? con.TenantId.Value :0);
//            }

//        }
//        [Route("UpdateComplaintMG")]
//        [HttpGet]
//        public void UpdateComplaintMG(int contactId,string subject, string content)
//        {
//            try
//            {
//                var con = GetContact(contactId);
//                var result = _dbService.UpdateComplaint(con.UserId, 0, true).Result;
//                if (result != null)
//                {
//                    result.customerChat = null;
//                    //    _hub2.Clients.All.SendAsync("brodCastEndUserMessage", result);
//                    SocketIOManager.SendContact(result, con.TenantId.HasValue ? con.TenantId.Value : 0);
//                }

//                //create new Tickets in MG
//                if (con.TenantId==59)
//                {
//                    SendTicketMg model = new SendTicketMg();

//                    model.subject=subject;
//                    model.content=content;
//                    model.phoneNumber=con.PhoneNumber;


//                    MGApiController mGApiController = new MGApiController();

//                    mGApiController.CreateTicketsMg(model);
//                }
//            }
//            catch(Exception ex)
//            {


//            }
           

//        }

//        [Route("GetOrderAndDetails")]
//        [HttpPost]
//        public OrderAndDetailsModel GetOrderAndDetails([FromBody] GetOrderAndDetailModel input)
//        {



//            var order = _iOrdersAppService.GetOrderExtraDetails(input.TenantID, input.ContactId);
           
//            List<OrderDetailDto> OrderDetailList = new List<OrderDetailDto>();

//            List<ExtraOrderDetailsDto> getOrderDetailExtraList = new List<ExtraOrderDetailsDto>();

//            if (!string.IsNullOrEmpty(order.OrderDetailDtoJson))
//            {
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                OrderDetailList = System.Text.Json.JsonSerializer.Deserialize<List<OrderDetailDto>>(order.OrderDetailDtoJson, options);
//            }
//            foreach (var item in OrderDetailList)
//            {
//                if (item.extraOrderDetailsDtos != null)
//                 getOrderDetailExtraList.AddRange(item.extraOrderDetailsDtos);
               

//            }
//            //var order = GetOrder(input.TenantID, input.ContactId);
//           // var OrderDetailList = GetOrderDetail(input.TenantID, int.Parse(order.Id.ToString()));
//           // var getOrderDetailExtraList = GetOrderDetailExtra(input.TenantID);


//            // var itemList = GetItem(input.TenantID);

//            var orderEffor = GetOrderOffer(input.TenantID);

//            var areaEffor = orderEffor.Where(x => x.isAvailable == true && x.isPersentageDiscount == true).FirstOrDefault();
//            bool isDiscount = false;
//            decimal Discount = 0;

//            if (areaEffor != null)
//            {
//                if (order.Total >= areaEffor.FeesStart && order.Total <= areaEffor.FeesEnd)
//                {


//                    var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
//                    var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
//                    var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

//                    if (DateStart <= DateNow && DateEnd >= DateNow)
//                    {
//                        var timeNow = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
//                        var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
//                        var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));

//                        if ((timeStart <= timeNow && timeNow <= timeEnd))
//                        {

//                            order.Total = order.Total - (order.Total * areaEffor.NewFees) / 100;
//                            isDiscount = true;
//                            Discount = areaEffor.NewFees;
//                        }



//                    }


//                }

//            }


//            var OrderString = GetOrderDetailString(input.TenantID, input.lang, order.Total, input.captionQuantityText, input.captionAddtionText, input.captionTotalText, input.captionTotalOfAllText, OrderDetailList, getOrderDetailExtraList, Discount, isDiscount);
//            OrderAndDetailsModel orderAndDetailsModel = new OrderAndDetailsModel
//            {
//                order = order,
//                DetailText = OrderString,
//                orderId = int.Parse(order.Id.ToString()),
//                total = order.Total,
//                Discount = Discount,
//                IsDiscount = isDiscount

//            };



//            return orderAndDetailsModel;
//        }

//        [Route("DeleteOrderDraft")]
//        [HttpGet]
//        public void DeleteOrderDraft(int tenantID, int orderId)
//        {

//            var orderDetail = GetOrderDetail(tenantID, orderId);
//            var OrderDetailExtraList = GetOrderDetailExtra(tenantID);

//            foreach (var deteal in orderDetail)
//            {

//                var GetOrderDetailExtraDraft = OrderDetailExtraList.Where(x => x.OrderDetailId == deteal.Id).ToList();

//                foreach (var ExtraOrde in GetOrderDetailExtraDraft)
//                {

//                    DeleteExtraOrderDetail(ExtraOrde.Id);
//                }

//                DeleteOrderDetails(deteal.Id);

//            }

//            DeleteOrder(orderId);

//        }

//        [Route("GetContcatModel")]
//        [HttpGet]
//        public Contact GetContcatModel(int ContactId)
//        {
//            var con = GetContact(ContactId);

//            return con;
//        }

//        [Route("TestCaption")]
//        [HttpGet]
//        public void TestCaption()
//        {
//            var locationList = GetAllLocationInfoModel();

//            foreach (var item in locationList)
//            {
//                item.LocationNameEn = item.LocationNameEn.Trim();
//                UpdateTextLocation(item);
//            }

//        }





//        [Route("GetDeliveryBranch")]
//        [HttpGet]
//        public Models.Location.DeliveryLocationCost GetDeliveryBranch(int TenantID, string FromDistrichName, string FromAreaName, string ToDistrichName, string ToAreaName)
//        {
//            try
//            {
//                var LocationList = GetAllLocationInfoModel();

//                var FromDistrich = LocationList.Where(x => x.LocationNameEn == FromDistrichName).FirstOrDefault();
//                var FromArea = LocationList.Where(x => x.LocationNameEn == FromAreaName && x.LevelId == 3).FirstOrDefault();

//                var ToDistrich = LocationList.Where(x => x.LocationNameEn == ToDistrichName).FirstOrDefault();
//                var ToArea = LocationList.Where(x => x.LocationNameEn == ToAreaName && x.LevelId == 3).FirstOrDefault();



//                if (FromDistrich == null || FromArea == null || ToDistrich == null || ToArea == null)
//                {
//                    Models.Location.DeliveryLocationCost branch = new Models.Location.DeliveryLocationCost();

//                    branch.TenantId = TenantID;
//                    branch.DeliveryCost = -1;
//                    branch.Id = 0;
//                    branch.BranchAreaId = 0;

//                    return branch;

//                }

//                var costList = GetAllDeliveryLocationCost(TenantID);






//                var Cos = costList.Where(x => (x.FromLocationId == FromDistrich.Id && x.ToLocationId == ToDistrich.Id) || (x.FromLocationId == ToDistrich.Id && x.ToLocationId == FromDistrich.Id)).FirstOrDefault();

//                if (Cos == null)
//                {

//                    var Cos2 = costList.Where(x => (x.FromLocationId == FromArea.Id && x.ToLocationId == ToArea.Id) || (x.FromLocationId == ToArea.Id && x.ToLocationId == FromArea.Id)).FirstOrDefault();

//                    if (Cos2 == null)
//                    {
//                        Models.Location.DeliveryLocationCost branch = new Models.Location.DeliveryLocationCost();

//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = -1;
//                        branch.Id = 0;
//                        branch.BranchAreaId = 0;

//                        return branch;


//                    }
//                    else
//                    {
//                        Models.Location.DeliveryLocationCost branch = new Models.Location.DeliveryLocationCost();

//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = Cos2.DeliveryCost;
//                        branch.Id = Cos2.Id;
//                        branch.FromLocationId = FromArea.Id;
//                        branch.ToLocationId = ToArea.Id;
//                        return branch;



//                    }


//                }
//                else
//                {
//                    Models.Location.DeliveryLocationCost branch = new Models.Location.DeliveryLocationCost();

//                    branch.TenantId = TenantID;
//                    branch.DeliveryCost = Cos.DeliveryCost;
//                    branch.Id = Cos.Id;
//                    branch.FromLocationId = FromDistrich.Id;
//                    branch.ToLocationId = ToDistrich.Id;
//                    return branch;

//                }


//            }
//            catch
//            {
//                Models.Location.DeliveryLocationCost branch = new Models.Location.DeliveryLocationCost();
//                branch.TenantId = TenantID;
//                branch.DeliveryCost = -1;
//                branch.Id = 0;
//                branch.BranchAreaId = 0;

//                return branch;


//            }


//        }


//        [Route("GetNearbyArea")]
//        [HttpGet]
//        public locationAddressModel GetNearbyArea(int tenantID, string query)
//        {
//            try
//            {

//                //  31.953255344085655, 35.847525840215276
//                locationAddressModel locationAddressModel = new locationAddressModel();
//                var rez = GetLocation(query);

//                double lata = Convert.ToDouble(query.Split(",")[0]);
//                double longt = Convert.ToDouble(query.Split(",")[1]);

//                var Country = rez.Country.Replace("'", "").Trim();
//                var City = rez.City.Replace("'", "").Trim();
//                var Area = rez.Area.Replace("'", "").Trim();
//                var Distric = rez.Distric.Replace("'", "").Trim();
//                var Route = rez.Route.Replace("'", "").Trim();

//                var result = "-1";

//                Dictionary<int, string> lstLocation = new Dictionary<int, string>();
//                List<lstLocation> dlstLocation = new List<lstLocation>();

//                string locationName;
//                if (!string.IsNullOrEmpty(Distric))
//                {
//                    lstLocation objlstLocation = new lstLocation();
//                    objlstLocation.LevelId = 3;
//                    objlstLocation.LocationName = Distric;
//                    dlstLocation.Add(objlstLocation);

//                }
//                if (!string.IsNullOrEmpty(Area))
//                {
//                    locationName = Area;
//                    lstLocation objlstLocation = new lstLocation();
//                    objlstLocation.LevelId = 2;
//                    objlstLocation.LocationName = Area;
//                    dlstLocation.Add(objlstLocation);
//                }
//                if (!string.IsNullOrEmpty(City))
//                {
//                    lstLocation objlstLocation = new lstLocation();
//                    objlstLocation.LevelId = 1;
//                    objlstLocation.LocationName = City;
//                    dlstLocation.Add(objlstLocation);
//                }
//                // locationName = City;





//                //  locationName = Distric + "," + Area + "," + City;


//                LocationInfoModelDto locationInfoModelDto = _iAreasAppService.GetLocationDeliveryCost(tenantID, JsonConvert.SerializeObject(dlstLocation).ToString());

//                if (locationInfoModelDto != null && locationInfoModelDto.Id > 0)
//                {




//                    locationAddressModel.Country = Country;
//                    locationAddressModel.City = City;
//                    locationAddressModel.Area = Area;
//                    locationAddressModel.Distric = Distric;
//                    locationAddressModel.Route = Route;
//                    locationAddressModel.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
//                    locationAddressModel.AddressEnglish = locationAddressModel.Address;
//                    locationAddressModel.Address = Translate(locationAddressModel.Address);//translat text 
//                    double distance;
//                    AreaDto areaDto = getNearbyArea(tenantID, lata, longt, City, locationInfoModelDto.AreaId, out  distance);

//                    if (areaDto.Id == 0)
//                    {

//                        return new locationAddressModel()
//                        {
//                            DeliveryCostAfter = -1,
//                            DeliveryCostBefor = -1,
//                        };
//                    }
//                    locationAddressModel.AreaNameEnglish = areaDto.AreaNameEnglish;
//                    locationAddressModel.AreaName = areaDto.AreaName;
//                    locationAddressModel.AreaCoordinate = areaDto.AreaCoordinate;
//                    locationAddressModel.AreaCoordinateEnglish = areaDto.AreaCoordinateEnglish;

//                    locationAddressModel.AreaId = areaDto.Id;
//                    locationAddressModel.DeliveryCostAfter = locationInfoModelDto.DeliveryCost.HasValue ? locationInfoModelDto.DeliveryCost.Value : 0;
//                    locationAddressModel.DeliveryCostBefor = locationInfoModelDto.DeliveryCost.HasValue ? locationInfoModelDto.DeliveryCost.Value : 0;

//                    return locationAddressModel;
//                }
//                else
//                {
//                    return new locationAddressModel()
//                    {
//                        DeliveryCostAfter = -1,
//                        DeliveryCostBefor = -1,
                        
//                    };
//                }


//            }
//            catch (Exception)
//            {


//                return new locationAddressModel()
//                {
//                    DeliveryCostAfter = -1,
//                    DeliveryCostBefor = -1,
//                };
//            }

//        }


//        private class lstLocation
//        {
//            public int LevelId { get; set; }
//            public string LocationName { get; set; }
//        }

//        [Route("GetlocationUserDelivery")]
//        [HttpGet]
//        public locationAddressModel GetlocationUserDelivery(string query)
//        {
//            locationAddressModel locationAddressModel = new locationAddressModel();

//            try
//            {
//                var rez = GetLocation(query);


//                var result = "-1";

//                var Country = rez.Country.Replace("'", "").Trim();
//                var City = rez.City.Replace("'", "").Trim();
//                var Area = rez.Area.Replace("'", "").Trim();
//                var Distric = rez.Distric.Replace("'", "").Trim();
//                var Route = rez.Route.Replace("'", "").Trim();


//                locationAddressModel.Country = Country;
//                locationAddressModel.City = City;
//                locationAddressModel.Area = Area;
//                locationAddressModel.Distric = Distric;
//                locationAddressModel.Route = Route;
//                locationAddressModel.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;



//                locationAddressModel.Address = Translate(locationAddressModel.Address);//translat text 



//                return locationAddressModel;
//            }
//            catch(Exception ex)
//            {

//                return locationAddressModel;
//            }
          

//        }


//        //AddHours(-3)
//        [Route("GetlocationUserModel")]
//        [HttpPost]
//        public GetLocationInfoModel GetlocationUserModel([FromBody] SendLocationUserModel input)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            TenantModel tenant = GetTenantById(input.tenantID).Result;
//            if (tenant.DeliveryCostType == DeliveryCostType.PerKiloMeter)
//            {
//                GetLocationInfoModel getLocationInfoModel = getLocationInfoPerKiloMeter(input.tenantID, input.query);


//                return getLocationInfoModel; 
//            }
//            else
//            {
//            GetLocationInfoModel infoLocation = new GetLocationInfoModel();
//            var result = "-1";
//            var Country = "";
//            var City = "";
//            var Area = "";
//            var Distric = "";
//            var Route = "";

//                if (input.isChangeLocation)
//                {
//                    Country = input.address.Split(" - ")[4].Replace("'", "").Trim();
//                    City = input.address.Split(" - ")[3].Replace("'", "").Trim();
//                    Area = input.address.Split(" - ")[2].Replace("'", "").Trim();
//                    Distric = input.address.Split(" - ")[1].Replace("'", "").Trim();
//                    Route = input.address.Split(" - ")[0].Replace("'", "").Trim();
//                }
//                else
//                {
//                    var rez = GetLocation(input.query);
//                    Country = rez.Country.Replace("'", "").Trim();
//                    City = rez.City.Replace("'", "").Trim();
//                    Area = rez.Area.Replace("'", "").Trim();
//                    Distric = rez.Distric.Replace("'", "").Trim();
//                    Route = rez.Route.Replace("'", "").Trim();
//                }


//            try
//            {


//                decimal Longitude = decimal.Parse(input.query.Split(",")[0]);
//                decimal Latitude = decimal.Parse(input.query.Split(",")[1]);


//                using (var connection = new SqlConnection(connString))
//                using (var command = connection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = "dbo.GetDeliveryCost";

//                    command.Parameters.AddWithValue("@Longitude", Longitude);
//                    command.Parameters.AddWithValue("@Latitude", Latitude);
//                    command.Parameters.AddWithValue("@City", City);
//                    command.Parameters.AddWithValue("@Area", Area);
//                    command.Parameters.AddWithValue("@Distric", Distric);
//                    command.Parameters.AddWithValue("@TenantId", input.tenantID);

//                    SqlParameter returnValue = command.Parameters.Add("@DeliveryCost", SqlDbType.NVarChar);
//                    returnValue.Direction = ParameterDirection.ReturnValue;

//                    connection.Open();
//                    command.ExecuteNonQuery();

//                    result = returnValue.Value.ToString();


//                }

//                var spilt = result.Split(",");



//                if (spilt[0] == "-1" || spilt[0] == "")
//                {

//                    infoLocation.Country = Country;
//                    infoLocation.City = City;
//                    infoLocation.Area = Area;
//                    infoLocation.Distric = Distric;

//                    infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
//                    infoLocation.DeliveryCostAfter = -1;
//                    infoLocation.DeliveryCostBefor = -1;
//                    infoLocation.LocationId = 0;

//                    return infoLocation;


//                }
//                else
//                {

//                    try
//                    {

//                        var locationList = GetAllLocationInfoModel();
//                        var cost = decimal.Parse(spilt[0]);
//                        var add = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;

//                        var cityModel = locationList.Where(x => x.LocationNameEn == City).FirstOrDefault();
//                        var areaModel = locationList.Where(x => x.LocationNameEn == Area).FirstOrDefault();
//                        var districModel = locationList.Where(x => x.LocationNameEn == Distric).FirstOrDefault();

//                        var areaname = GetAreas(input.tenantID).Where(x => x.Id == int.Parse(spilt[1])).FirstOrDefault();

//                        var cityName = "";
//                        var areaName = "";
//                        var districName = "";

//                        if (cityModel != null)
//                            cityName = cityModel.LocationName;
//                        if (areaModel != null)
//                            areaName = areaModel.LocationName;
//                        if (districModel != null)
//                            districName = districModel.LocationName;





//                        OrderOfferFun(input.tenantID, input.isOrderOffer, input.OrderTotal, infoLocation, cityName, areaName, districName, cost);

//                        infoLocation.Country = Country;
//                        infoLocation.City = City;
//                        infoLocation.Area = Area;
//                        infoLocation.Distric = Distric;

//                        infoLocation.Address = add;


//                        infoLocation.LocationId = int.Parse(spilt[1]);

//                            if (areaname.IsRestaurantsTypeAll)
//                            {
//                                infoLocation.LocationId=0;

//                            }

//                        if (areaname != null)
//                        {
//                            if (input.local == "ar")
//                            {
//                                infoLocation.LocationAreaName = areaname.AreaName;
//                            }
//                            else
//                            {

//                                if (areaname.AreaNameEnglish == null)
//                                {

//                                    infoLocation.LocationAreaName = areaname.AreaName;
//                                }
//                                else
//                                {
//                                    infoLocation.LocationAreaName = areaname.AreaNameEnglish;
//                                }

//                            }

//                        }
//                        infoLocation.DeliveryCostAfter = cost;




//                        return infoLocation;
//                    }
//                    catch
//                    {
//                        infoLocation.Country = Country;
//                        infoLocation.City = City;
//                        infoLocation.Area = Area;
//                        infoLocation.Distric = Distric;

//                        infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
//                        infoLocation.DeliveryCostAfter = -1;
//                        infoLocation.DeliveryCostBefor = -1;
//                        infoLocation.LocationId = 0;

//                        return infoLocation;


//                    }


//                }


//            }
//            catch (Exception rx)
//            {
//                infoLocation.Country = Country;
//                infoLocation.City = City;
//                infoLocation.Area = Area;
//                infoLocation.Distric = Distric;

//                infoLocation.Address = Route + " - " + Distric + " - " + Area + " - " + City + " - " + Country;
//                infoLocation.DeliveryCostAfter = -1;
//                infoLocation.DeliveryCostBefor = -1;
//                infoLocation.LocationId = 0;

//                return infoLocation;

//            }
//            }

//        }

//        //AddHours(-3)
//        [Route("GetlocationUserTowModel")]
//        [HttpGet]
//        public List<string> GetlocationUserTowModel(int TenantID, string address)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;

//            var locationList = GetAllLocationInfoModel();
//            var costList = GetAllLocationDeliveryCost(TenantID);
//            List<GetLocationInfoModel> infoLocation = new List<GetLocationInfoModel>();
//            List<string> vs = new List<string>();

//            try
//            {
//                var Country = address.Split(" - ")[4].Replace("'", "").Trim();
//                var City = address.Split(" - ")[3].Replace("'", "").Trim();
//                var Area = address.Split(" - ")[2].Replace("'", "").Trim();
//                var Distric = address.Split(" - ")[1].Replace("'", "").Trim();
//                var Route = address.Split(" - ")[0].Replace("'", "").Trim();

//                if (City == "Jerash Governorate")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Jerash Governorate A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }

//                }
//                else if (City == "Ajloun")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Ajloun A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }
//                else if (City == "Kufranjah")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Ajloun B").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }
//                else if (City == "Al-Mafraq")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Al-Mafraq A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }
//                else if (City == "As-Salt")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "As-Salt A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }
//                else if (City == "Ain Albasha District")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Ain Albasha District A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }
//                else if (City == "Aqaba")
//                {
//                    var loc = locationList.Where(x => x.LocationNameEn == "Aqaba A").FirstOrDefault();
//                    var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                    foreach (var item in listLoca)
//                    {
//                        var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                        if (FoindCost != null)
//                        {

//                            infoLocation.Add(new GetLocationInfoModel
//                            {

//                                LocationAreaName = item.LocationName,
//                                DeliveryCostAfter = FoindCost.DeliveryCost,
//                                DeliveryCostBefor = 0,
//                                isOrderOfferCost = false,


//                            });
//                        }


//                    }


//                }



//                List<decimal?> vs1 = new List<decimal?>();
//                var OrderByCostList = infoLocation.OrderBy(x => x.DeliveryCostAfter);
//                foreach (var it in OrderByCostList)
//                {
//                    vs1.Add(it.DeliveryCostAfter);

//                }


//                var myList = vs1.Distinct();

//                foreach (var item in myList)
//                {

//                    var x = OrderByCostList.Where(x => x.DeliveryCostAfter == item);

//                    var nameOp = "";
//                    foreach (var item2 in x)
//                    {

//                        nameOp = nameOp + "-" + item2.LocationAreaName;

//                    }
//                    vs.Add(nameOp);

//                }




//                return vs;
//            }
//            catch(Exception ex)
//            {
//                return vs;
//            }


//        }


//        [Route("GetlocationUserThreeModel")]
//        [HttpGet]
//        public decimal? GetlocationUserThreeModel(int TenantID, string address, string select)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;

//            var locationList = GetAllLocationInfoModel();
//            var costList = GetAllLocationDeliveryCost(TenantID);
//            List<GetLocationInfoModel> infoLocation = new List<GetLocationInfoModel>();
//            List<string> vs = new List<string>();


//            var Country = address.Split(" - ")[4].Replace("'", "").Trim();
//            var City = address.Split(" - ")[3].Replace("'", "").Trim();
//            var Area = address.Split(" - ")[2].Replace("'", "").Trim();
//            var Distric = address.Split(" - ")[1].Replace("'", "").Trim();
//            var Route = address.Split(" - ")[0].Replace("'", "").Trim();

//            if (City == "Jerash Governorate")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Jerash Governorate A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }

//            }
//            else if (City == "Ajloun")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Ajloun A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }

//            }
//            else if (City == "Kufranjah")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Ajloun B").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }


//            }
//            else if (City == "Al-Mafraq")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Al-Mafraq A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }


//            }

//            else if (City == "As-Salt")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "As-Salt A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }


//            }
//            else if (City == "Ain Albasha District")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Ain Albasha District A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }


//            }
//            else if (City == "Aqaba")
//            {
//                var loc = locationList.Where(x => x.LocationNameEn == "Aqaba A").FirstOrDefault();
//                var listLoca = locationList.Where(x => x.ParentId == loc.Id);

//                foreach (var item in listLoca)
//                {
//                    var FoindCost = costList.Where(x => x.LocationId == item.Id).FirstOrDefault();
//                    if (FoindCost != null)
//                    {

//                        infoLocation.Add(new GetLocationInfoModel
//                        {

//                            LocationAreaName = item.LocationName,
//                            DeliveryCostAfter = FoindCost.DeliveryCost,
//                            DeliveryCostBefor = 0,
//                            isOrderOfferCost = false,


//                        });
//                    }


//                }


//            }

//            List<decimal?> vs1 = new List<decimal?>();
//            var OrderByCostList = infoLocation.OrderBy(x => x.DeliveryCostAfter);
//            foreach (var it in OrderByCostList)
//            {

//                if (select.Contains(it.LocationAreaName))
//                {

//                    return it.DeliveryCostAfter;
//                }

//            }







//            return 0;

//        }


//        [Route("GetDay")]
//        [HttpGet]
//        public List<string> GetDay(string local)
//        {
//            List<string> vs = new List<string>();
//            for (int i = 0; i <= 6; i++)
//            {
//                var day = DateTime.Now.AddDays(i);
//                string dayName = "";
//                string date = "";
//                if (local == "ar")
//                {
//                    dayName = day.ToString("dddd", new CultureInfo("ar-AE"));
//                    date = day.ToString("dd/MM", new CultureInfo("ar-AE"));
//                }
//                else
//                {
//                    dayName = day.ToString("dddd", new CultureInfo("en-US"));
//                    date = day.ToString("dd/MM", new CultureInfo("en-US"));
//                }

//                var st = dayName + "(" + date + ")";

//                vs.Add(st);
//            }

//            return vs;
//        }

//        //AddHours(3)
//        [Route("GetTime")]
//        [HttpGet]
//        public List<string> GetTime(int TenantID, string selectDay, string local)
//        {
//            if (TenantID == 34)
//            {
//                List<string> vs = new List<string>();

//                vs.Add("8 AM - 11 AM");
//                vs.Add("11 AM - 2 PM");
//                vs.Add("2 PM - 5 PM");
//                vs.Add("5 PM - 8 PM");


//                return vs;
//            }
//            else if (TenantID == 42)
//            {
//                List<string> vs = new List<string>();
//                if (local == "ar")
//                {
//                    vs.Add("18:30- 19:00 افطار");
//                    vs.Add("19:15- 19:45 افطار");
//                    vs.Add("سحور 2 - 3");
//                    vs.Add("سحور 3 - 4");
//                }
//                else
//                {
//                    vs.Add("18:30-19:00 Iftar");
//                    vs.Add("19:15-19:45 Iftar");
//                    vs.Add("2 - 3 Sohoor");
//                    vs.Add("3 - 4 Sohoor");
//                }



//                return vs;
//            }
//            else if (TenantID == 124123123)
//            {



//                List<int> listTime = new List<int>();

//                List<string> listTimeString = new List<string>();
//                var timeNow = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture);
//                string dayName = "";
//                string date = "";
//                if (local == "ar")
//                {
//                    dayName = DateTime.Now.ToString("dddd", new CultureInfo("ar-AE"));
//                    date = DateTime.Now.ToString("dd/MM", new CultureInfo("ar-AE"));
//                }
//                else
//                {
//                    dayName = DateTime.Now.ToString("dddd", new CultureInfo("en-US"));
//                    date = DateTime.Now.ToString("dd/MM", new CultureInfo("en-US"));
//                }

//                var st = dayName + "(" + date + ")";


//                var resulttimeNow = Convert.ToDateTime(timeNow);
//                int startTime = 15 - 1;//3:00 pm

//                if (TenantID == 5)
//                    startTime = 10;

//                for (int i = 0; i < 10; i++)
//                {
//                    startTime++;
//                    if (startTime > 24)
//                        break;


//                    if (selectDay.Contains(st))
//                    {
//                        if (startTime > resulttimeNow.Hour)
//                            listTime.Add(startTime);

//                    }
//                    else
//                    {
//                        listTime.Add(startTime);

//                    }

//                }

//                foreach (var item in listTime)
//                {

//                    listTimeString.Add(item.ToString() + ":00");
//                }


//                return listTimeString;
//            }
//            else if (TenantID == 59) {

//                List<string> vs = new List<string>();

               
//                vs.Add("9 AM - 10 AM");
//                vs.Add("10 AM - 11 AM");
//                vs.Add("11 AM - 12 PM");
//                vs.Add("12 PM - 1 PM");
               
//                return vs;

//            }
//            else
//            {

//                //List<string> vs = new List<string>();

//                //vs.Add("11 AM");
//                //vs.Add("12 PM");
//                //vs.Add("1 PM");
//                //vs.Add("2 PM");
//                //vs.Add("3 PM");
//                //vs.Add("4 PM");
//                //vs.Add("5 PM");
//                //vs.Add("6 PM");
//                //vs.Add("7 PM");
//                //vs.Add("8 PM");

//                List<string> vs = new List<string>();

//                vs.Add("8 AM - 9 AM");
//                vs.Add("9 AM - 10 AM");
//                vs.Add("10 AM - 11 AM");
//                vs.Add("11 AM - 12 PM");
//                vs.Add("12 PM - 1 PM");
//                vs.Add("1 PM - 2 PM");
//                vs.Add("2 PM - 3 PM");
//                vs.Add("3 PM - 4:30 PM");
//                return vs;

//            }



//        }


//        [Route("GetAreasWithPage")]
//        [HttpGet]
//        public List<string> GetAreasWithPage(string TenantID, string local, int menu, int pageNumber, int pageSize, bool isDelivery)
//        {

//            List<string> vs = new List<string>();
//            var list = GetAreasList(TenantID);

//            if (list.Count > 10)
//            {
//                var values = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();
//                if (pageNumber >= 1)
//                {
//                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();


//                    //if (local == "ar")
//                    //{
//                    //    values2.Add(new Area
//                    //    {
//                    //        AreaName = "العودة",
//                    //        AreaCoordinate = "<---"


//                    //    });
//                    //}
//                    //else
//                    //{
//                    //    values2.Add(new Area
//                    //    {
//                    //        AreaName = "Back",
//                    //        AreaCoordinate = "<---"


//                    //    });
//                    //}

//                    foreach (var item in values2)
//                    {

//                        if (local == "ar")
//                        {

//                            vs.Add(item.AreaName);
//                        }
//                        else
//                        {
//                            vs.Add(item.AreaNameEnglish);
//                        }
//                    }
//                    return vs;
//                }


//                if (local == "ar")
//                {
//                    if (TenantID!="56")
//                    {
//                        //values.Add(new Area
//                        //{
//                        //    AreaName = "اخرى",
//                        //    AreaCoordinate = "--->"


//                        //});
//                    }
                  
//                }
//                else
//                {
//                    if (TenantID!="56")
//                    {
//                        //values.Add(new Area
//                        //{
//                        //    AreaNameEnglish = "Others",
//                        //    AreaCoordinateEnglish = "--->"


//                        //});
//                    }

//                }


//                foreach (var item in values)
//                {
//                    if (local == "ar")
//                    {

//                        vs.Add(item.AreaName);
//                    }
//                    else
//                    {
//                        vs.Add(item.AreaNameEnglish);
//                    }

//                }




//                return vs;
//            }
//            else
//            {

//                foreach (var item in list)
//                {

//                    if (local == "ar")
//                    {

//                        vs.Add(item.AreaName);
//                    }
//                    else
//                    {
//                        vs.Add(item.AreaNameEnglish);
//                    }
//                }

//                return vs;
//            }

//        }

//        [Route("GetAssetLevel")]
//        [HttpGet]
//        public List<GetAssetModel> GetAssetLevel(int tenantId,string local,  int stepId, int levelId =0)
//        {
//            var lstLevels = _iAssetAppService.LoadLevels(tenantId);
//            List<GetAssetModel> result = new List<GetAssetModel>(); ;
//            if (lstLevels != null)
//            {
//                switch (stepId)
//                {
//                    case 1:
//                        foreach (var item in lstLevels.lstAssetLevelOneDto)
//                        {
//                            GetAssetModel getAssetModel = new GetAssetModel();  
//                            if (local.Equals("ar"))
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelOneNameAr.Trim();
//                                result.Add(getAssetModel);
//                            }
//                            else
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelOneNameEn.Trim();
//                                result.Add(getAssetModel);                               
//                            }
                               
//                        } ;
//                        break;
//                    case 2:
//                        var levelTwo = lstLevels.lstAssetLevelTwoDto.Where(x => x.LevelOneId == levelId).ToList();
//                        foreach (var item in levelTwo)
//                        {
//                            GetAssetModel getAssetModel = new GetAssetModel();
//                            if (local.Equals("ar"))
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelTwoNameAr.Trim();
//                                result.Add(getAssetModel);
//                            }
//                            else
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelTwoNameEn.Trim();
//                                result.Add(getAssetModel);
//                            }
  
//                        };
//                        break;
//                    case 3:
//                        var levelThree = lstLevels.lstAssetLevelThreeDto.Where(x => x.LevelTwoId == levelId).ToList();
//                        foreach (var item in levelThree)
//                        {
//                            GetAssetModel getAssetModel = new GetAssetModel();
//                            if (local.Equals("ar"))
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelThreeNameAr.Trim();
//                                result.Add(getAssetModel);
//                            }
//                            else
//                            {
//                                getAssetModel.Key = item.Id;
//                                getAssetModel.Value = item.LevelThreeNameEn.Trim();
//                                result.Add(getAssetModel);
//                            }

//                        };
//                        break;
//                    case 4:

//                        if (lstLevels.lstAssetLevelFourDto!=null)
//                        {
//                            var levelfour = lstLevels.lstAssetLevelFourDto.Where(x => x.LevelThreeId == levelId).ToList();

//                            foreach (var item in levelfour)
//                            {
//                                GetAssetModel getAssetModel = new GetAssetModel();
//                                if (local.Equals("ar"))
//                                {
//                                    getAssetModel.Key = item.Id;
//                                    getAssetModel.Value = item.LevelFourNameAr.Trim();
//                                    result.Add(getAssetModel);
//                                }
//                                else
//                                {
//                                    getAssetModel.Key = item.Id;
//                                    getAssetModel.Value = item.LevelFourNameEn.Trim();
//                                    result.Add(getAssetModel);
//                                }

//                            };

//                        }
                      
//                        break;
//                }
//            }


//            return result;

//        }
//        [Route("IsOneMenu")]
//        [HttpGet]
//        public bool IsOneMenu(string TenantID)
//        {
//            var list = GetAreasList(TenantID).FirstOrDefault();

//            return list.IsRestaurantsTypeAll;

//        }
//        [Route("Translate")]
//        [HttpGet]
//        public string Translate(string word)
//        {

//            //var list = word.Split(" - ");

//            //var FirstWord = TransFun(list[0]);


//            //var secWord = word.Substring(0, word.IndexOf(" - ", 4, StringComparison.Ordinal) - 1);

//            //secWord = word.Replace(secWord, "");

//            var secWord = TransFun(word);

//            return secWord;


//        }


//        [Route("GetAreasID")]
//        [HttpGet]
//        public Area GetAreasID(string TenantID, string AreaName, int menu, string local)
//        {

//            var list = GetAreasList(TenantID);

//            var area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();

//            if (local == "ar")
//            {

//                area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();
//            }
//            else
//            {
//                area = list.Where(x => (x.AreaNameEnglish).Contains(AreaName)).FirstOrDefault();
//            }


//            if (area == null)
//            {
//                Area area1 = new Area();
//                area1.Id = 0;
//                return area1;
//            }

//            if (area.IsRestaurantsTypeAll)
//            {
//                area.Id = 0;
//            }

//            return area;

//        }


//        [Route("GetAreasByID")]
//        [HttpGet]
//        public Area GetAreasByID(string TenantID, int AreaID)
//        {

//            var area = GetAreasID(AreaID);

         

//            if (area == null)
//            {
//                Area area1 = new Area();
//                area1.Id = 0;
//                return area1;
//            }

//            if (area.IsRestaurantsTypeAll)
//            {
//                area.Id = 0;
//            }

//            return area;

//        }
//        [Route("GetAreasID2")]
//        [HttpGet]
//        public Area GetAreasID2(string TenantID, string AreaName, int menu, string local)
//        {

//            var list = GetAreasList(TenantID);

//            var area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();

//            if (local == "ar")
//            {

//                area = list.Where(x => (x.AreaName).Contains(AreaName)).FirstOrDefault();
//            }
//            else
//            {
//                area = list.Where(x => (x.AreaNameEnglish).Contains(AreaName)).FirstOrDefault();
//            }


//            if (area == null)
//            {
//                Area area1 = new Area();
//                area1.Id = 0;
//                return area1;
//            }



//            return area;

//        }

//        //AddHours(3)
//        [Route("UpdateOrder")]
//        [HttpPost]
//        public async Task<string> UpdateOrderAsync([FromBody] UpdateOrderModel updateOrderModel)
//        {
//            if (updateOrderModel.BuyType == "No select")
//                updateOrderModel.BuyType = "";

//            var time = DateTime.Now;
//            var timeAdd = time.AddHours(AppSettingsModel.AddHour);
//            string connString = AppSettingsModel.ConnectionStrings;
//            long number = 0;
//            var con = GetContact(updateOrderModel.ContactId);
//            lock (CurrentOrder)
//            {
//                number = UpateOrder(updateOrderModel.TenantID);

//            }


//            if (updateOrderModel.TypeChoes == "PickUp")
//            {
//                var area = GetAreasList(updateOrderModel.TenantID.ToString()).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();

//                //  string htmlOrder = GetHtmlPrint(updateOrderModel, timeAdd, number, con, area, updateOrderModel.BotLocal);

//                var captionAreaNameText = updateOrderModel.aptionAreaNameText;//من الفرع :

//                updateOrderDB(updateOrderModel, timeAdd, connString, number, area);


//                var ListString = "------------------ \r\n\r\n";

//                ListString = ListString + updateOrderModel.captionOrderInfoText;
//                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";
//                ListString = ListString + captionAreaNameText + updateOrderModel.BranchName + "\r\n";
//                ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + updateOrderModel.OrderTotal + "\r\n";

//                ListString = ListString + "------------------ \r\n\r\n";
//                ListString = ListString + updateOrderModel.captionEndOrderText;





//                long agId = 0;

//                //Notification
//                if (updateOrderModel.BranchId != 0)
//                {

//                    // Notification
//                    //try
//                    //{
//                    //    if (area != null && !string.IsNullOrEmpty(area.UserIds))
//                    //    {
//                    //        var arry = area.UserIds.Split(',');
//                    //        foreach (var item in arry)
//                    //        {
//                    //            var message = "The order AssignTo :" + area.AreaName + "(" + area.AreaCoordinate + ")";
//                    //            UserNotification Notification = await SendNotfAsync(message, Convert.ToInt64(item), updateOrderModel.TenantID);
//                    //        }

//                    //        agId = Convert.ToInt64(area.UserId);
//                    //    }


//                    //}
//                    //catch
//                    //{


//                    //}



//                }

//                Order order = new Order
//                {
//                    OrderLocal = updateOrderModel.BotLocal,
//                    HtmlPrint = "",
//                    SpecialRequestText = updateOrderModel.SpecialRequest,
//                    AreaId = updateOrderModel.BranchId,
//                    ContactId = updateOrderModel.ContactId,
//                    OrderTime = timeAdd,
//                    CreationTime = timeAdd,
//                    Id = updateOrderModel.OrderId,
//                    OrderNumber = number,
//                    TenantId = updateOrderModel.TenantID,
//                    orderStatus = OrderStatusEunm.Pending,
//                    OrderType = OrderTypeEunm.Takeaway,
//                    Total = updateOrderModel.OrderTotal,
//                    IsDeleted = false,
//                    AgentId = agId,
//                    AgentIds = area.UserIds,
//                    IsLockByAgent = agId > 0,
//                };

//                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
//                getOrderForViewDto.Order = GetOrderMap;
//                getOrderForViewDto.AreahName = updateOrderModel.BranchName;
//                getOrderForViewDto.OrderStatusName = orderStatusName;
//                getOrderForViewDto.OrderTypeName = orderTypeName;
//                getOrderForViewDto.AgentIds = area.UserIds;

//                getOrderForViewDto.IsAssginToAllUser = area.IsAssginToAllUser;
//                getOrderForViewDto.IsAvailableBranch = area.IsAvailableBranch;
//                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
//                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
//                getOrderForViewDto.CreatDate = GetOrderMap.CreationTime.ToString("MM/dd/yyyy");
//                getOrderForViewDto.CreatTime = GetOrderMap.CreationTime.ToString("hh:mm tt");

//                //if (updateOrderModel.TenantID==46)
//                //{
//                //    getOrderForViewDto.Order.StringTotal = ((int)GetOrderMap.Total).ToString();
//                //}
//                //else
//                //{
//                //    getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");
//                //}
//                getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");
//                try
//                {
//                    var titl = "The Order Number: " + number.ToString();
//                    var body = "Order Status :" + OrderTypeEunm.Takeaway.ToString() + " From :" + area.AreaName;

//                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
//                    SendMobileNotification(order.TenantId, titl, body);
//                }
//                catch (Exception ex)
//                {

//                }



//               // await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);
//                SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);

//                //delete bot conversation
//                //  DeleteConversation(usertodelete.SunshineConversationId);



//                con.TotalOrder = con.TotalOrder + 1;
//                con.TakeAwayOrder = con.TakeAwayOrder + 1;

//                var contact =await _dbService.UpdateCustomerLocation(con);

//                return ListString;

//            }
//            else
//            {
//                //updateOrderModel.AddressEn = updateOrderModel.Address;
//                //if (updateOrderModel.BotLocal == "ar")
//                //{

//                //    updateOrderModel.Address = Translate(updateOrderModel.AddressEn);//translat text 

//                //}
//                var area = new Area();
//                try
//                {
//                    area = GetAreaId(updateOrderModel.MenuId);
//                }
//                catch
//                {
//                    area = null;

//                }

//                //  var area = GetAreas(updateOrderModel.TenantID.ToString()).Where(x => x.Id == updateOrderModel.BranchId).FirstOrDefault();

//                long agId = 0;
//                if (area != null && area.Id != 0)
//                {

//                    if (area.UserId != 0)
//                    {

//                        agId = long.Parse(area.UserId.ToString());
//                    }
//                    else
//                    {
//                        area = GetAreaId(updateOrderModel.BranchId);
//                        agId = updateOrderModel.BranchId;
//                    }
//                }
//                else
//                {
//                    area = GetAreaId(updateOrderModel.BranchId);
//                    agId = updateOrderModel.BranchId;
//                }
//                string AgentIds = area.UserIds;
//                if (string.IsNullOrEmpty(AgentIds))
//                    AgentIds =null;

//                var BranchAreaName = updateOrderModel.BranchName;

//                // Notification
//                //try
//                //{
//                //    if (!string.IsNullOrEmpty(AgentIds)) { 
//                //    var arry = AgentIds.Split(',');
//                //    foreach (var item in arry)
//                //    {
//                //        var message = "The order AssignTo :" + "(" + BranchAreaName + ")";
//                //        UserNotification Notification = await SendNotfAsync(message, Convert.ToInt64(item), updateOrderModel.TenantID);
//                //    }
//                //    }
//                //    //var message = "The order AssignTo :" + "(" + BranchAreaName + ")";
//                //    //UserNotification Notification = await SendNotfAsync(message, Convert.ToInt64(agId), updateOrderModel.TenantID);
//                //    agId = Convert.ToInt64(agId);

//                //}
//                //catch
//                //{

//                //}




//                //   string htmlOrderD = GetHtmlD(updateOrderModel, timeAdd, number, con, BranchAreaName, updateOrderModel.BotLocal);


//                //var captionBranchCostText = GetCaptionFormat("BackEnd_Text_BranchCost", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//قيمة التوصيل :
//                //var captionFromLocationText = GetCaptionFormat("BackEnd_Text_FromLocation", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//من الموقع :
//                UpdateDeliveryOrderDB(updateOrderModel, timeAdd, connString, number, BranchAreaName, area);

//                decimal totalWithBranchCost = 0;
//                decimal Cost = 0;

//                if (updateOrderModel.isOrderOfferCost)
//                {
//                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostBefor);
//                    Cost = updateOrderModel.DeliveryCostBefor;
//                }
//                else
//                {
//                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostAfter);
//                    Cost = updateOrderModel.DeliveryCostAfter;
//                }


//                var ListString = "------------------ \r\n\r\n";

//                ListString = ListString + updateOrderModel.captionOrderInfoText;
//                ListString = ListString + updateOrderModel.captionOrderNumberText + number + "\r\n";

//                if (updateOrderModel.TenantID==46)
//                {
//                    ListString = ListString + updateOrderModel.captionBranchCostText + ((int)Cost).ToString() + "\r\n";
//                }
//                else
//                {
//                    ListString = ListString + updateOrderModel.captionBranchCostText + Cost + "\r\n";
//                }
                
//                ListString = ListString + updateOrderModel.captionFromLocationText + updateOrderModel.Address + "\r\n";



//                if (updateOrderModel.TenantID==46)
//                {
//                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + ((int)totalWithBranchCost).ToString() + "\r\n\r\n";
//                }
//                else
//                {
//                    ListString = ListString + updateOrderModel.captionTotalOfAllOrderText + totalWithBranchCost + "\r\n\r\n";
//                }
               


//                ListString = ListString + "------------------ \r\n\r\n";
//                ListString = ListString + updateOrderModel.captionEndOrderText;



//                Order order = new Order
//                {
//                    OrderLocal = updateOrderModel.BotLocal,
//                    BuyType = updateOrderModel.BuyType,
//                    SelectDay = updateOrderModel.SelectDay,
//                    SelectTime = updateOrderModel.SelectTime,
//                    IsPreOrder = updateOrderModel.IsPreOrder,

//                    SpecialRequestText = updateOrderModel.SpecialRequest,
//                    AfterDeliveryCost = updateOrderModel.DeliveryCostAfter,
                   
//                    BranchAreaName = BranchAreaName,
//                    BranchAreaId = updateOrderModel.BranchId,
//                    Address = updateOrderModel.Address,
//                    BranchId = updateOrderModel.BranchId,
//                    ContactId = updateOrderModel.ContactId,
//                    OrderTime = timeAdd,
//                    CreationTime = timeAdd,
//                    Id = updateOrderModel.OrderId,
//                    OrderNumber = number,
//                    TenantId = updateOrderModel.TenantID,
//                    orderStatus = OrderStatusEunm.Pending,
//                    OrderType = OrderTypeEunm.Delivery,
//                    Total = totalWithBranchCost,
//                    IsDeleted = false,
//                    AgentId = agId,
//                    AgentIds = AgentIds,
//                    IsLockByAgent = agId > 0,
//                    LocationID = updateOrderModel.BranchId,
//                    FromLocationDescribation = "https://maps.google.com/?q=" + updateOrderModel.LocationFrom,
//                    HtmlPrint = ""
//                };

//                if (updateOrderModel.IsPreOrder)
//                {
//                    order.orderStatus = OrderStatusEunm.Pre_Order;
//                }

//                try
//                {
//                    var titl = "the order Number: " + number.ToString();
//                    var body = "Order Status :" + order.orderStatus + " From :" + updateOrderModel.Address;

//                    // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
//                    SendMobileNotification(order.TenantId, titl, body);
//                }
//                catch
//                {

//                }




//                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
//                getOrderForViewDto.CustomerMobile = con.PhoneNumber;
//                getOrderForViewDto.Order = GetOrderMap;
//                getOrderForViewDto.OrderStatusName = orderStatusName;
//                getOrderForViewDto.OrderTypeName = orderTypeName;
//                getOrderForViewDto.AgentIds = AgentIds;

//                getOrderForViewDto.BranchAreaName = BranchAreaName;

//                getOrderForViewDto.IsAssginToAllUser = true;// area.IsAssginToAllUser;
//                getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
//                getOrderForViewDto.TenantId = updateOrderModel.TenantID;
//                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
//                getOrderForViewDto.CreatDate = GetOrderMap.CreationTime.ToString("MM/dd/yyyy");
//                getOrderForViewDto.CreatTime = GetOrderMap.CreationTime.ToString("hh:mm tt");

//                if (updateOrderModel.isOrderOfferCost)
//                {
//                    getOrderForViewDto.DeliveryCost=updateOrderModel.DeliveryCostBefor;
//                }
//                else
//                {
//                    getOrderForViewDto.DeliveryCost=updateOrderModel.DeliveryCostAfter;
//                }
                   

//                //if (updateOrderModel.TenantID==46)
//                //{
//                //    getOrderForViewDto.Order.StringTotal = ((int)GetOrderMap.Total).ToString();
//                //}
//                //else
//                //{
//                //    getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");
//                //}
//                getOrderForViewDto.Order.StringTotal = (Math.Round(GetOrderMap.Total * 100) / 100).ToString("N2");

//               // await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);

//                 SocketIOManager.SendOrder(getOrderForViewDto, updateOrderModel.TenantID);



//                con.TotalOrder = con.TotalOrder + 1;
//                con.DeliveryOrder = con.DeliveryOrder + 1;
//                con.Description = updateOrderModel.Address;
//                con.Website = updateOrderModel.AddressEn;
//                con.EmailAddress = updateOrderModel.LocationFrom;

//                con.StreetName = updateOrderModel.StreetName;
//                con.BuildingNumber = updateOrderModel.BuildingNumber;
//                con.FloorNo = updateOrderModel.FloorNo;
//                con.ApartmentNumber = updateOrderModel.ApartmentNumber;

//                var contact = _dbService.UpdateCustomerLocation(con).Result;

//                contact.customerChat = null;
//                //await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", contact);
//                // SocketIOManager.SendContact(contact, contact.TenantId.HasValue ? contact.TenantId.Value : 0);
//                //delete bot conversation
//                // DeleteConversation(usertodelete.SunshineConversationId);
//                return ListString;

//            }



//        }

//        [Route("UpdateDeliveryOrder")]
//        [HttpPost]
//        public async Task<string> UpdateDeliveryOrderAsync([FromBody] OrderBotData orderBotData)
//        {
//            var time = DateTime.Now;

//            var timeAdd = time.AddHours(AppSettingsModel.AddHour);
//            string connString = AppSettingsModel.ConnectionStrings;
//            var con = GetContact(orderBotData.ContactId);

//            int modified = 0;
//            long number = 0;

//            lock (CurrentOrder)
//            {
//                number = UpateOrder(orderBotData.TenantID);

//            }


//            orderBotData.FromAddressEn = orderBotData.FromAddress;
//            orderBotData.ToAddressEn = orderBotData.ToAddress;
//            if (orderBotData.BotLocal == "ar")
//            {

//                orderBotData.FromAddress = Translate(orderBotData.FromAddress);//translat text 
//                orderBotData.ToAddress = Translate(orderBotData.ToAddress);//translat text 

//            }


//            var ListString = string.Format(orderBotData.CaptionText, number.ToString(), orderBotData.FromAddress.Trim(), orderBotData.ToAddress.Trim(), orderBotData.FromLocationDescribatione, orderBotData.BranchCost);


//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {

//                command.CommandText = "INSERT INTO Orders (IsDeleted, OrderTime, OrderNumber, CreationTime, IsLockByAgent, orderStatus, TenantId, ContactId, Total, AgentId, DeliveryCost, OrderType, IsEvaluation ,FromLocationID,ToLocationID,FromLocationDescribation,OrderDescribation)  VALUES (@IsDeleted, @OrderTime, @OrderNumber, @CreationTime, @IsLockByAgent, @orderStatus, @TenantId, @ContactId, @Total, @AgentId, @DeliveryCost , @OrderType , @IsEvaluation, @FromLocationID, @ToLocationID,@FromLocationDescribation,@OrderDescribation) ;SELECT SCOPE_IDENTITY(); ";

//                command.Parameters.AddWithValue("@IsDeleted", false);
//                command.Parameters.AddWithValue("@OrderTime", timeAdd);
//                command.Parameters.AddWithValue("@OrderNumber", number);
//                command.Parameters.AddWithValue("@CreationTime", timeAdd);
//                command.Parameters.AddWithValue("@IsLockByAgent", false);
//                command.Parameters.AddWithValue("@orderStatus", OrderStatusEunm.Pending);
//                command.Parameters.AddWithValue("@TenantId", orderBotData.TenantID);
//                command.Parameters.AddWithValue("@ContactId", orderBotData.ContactId);
//                command.Parameters.AddWithValue("@Total", decimal.Parse(orderBotData.BranchCost));
//                command.Parameters.AddWithValue("@AgentId", -1);


//                command.Parameters.AddWithValue("@DeliveryCost", orderBotData.BranchCost);
//                command.Parameters.AddWithValue("@OrderType", OrderTypeEunm.Delivery);
//                command.Parameters.AddWithValue("@IsEvaluation", false);

//                command.Parameters.AddWithValue("@FromLocationID", orderBotData.FromLocationID);
//                command.Parameters.AddWithValue("@ToLocationID", orderBotData.ToLocationID);

//                command.Parameters.AddWithValue("@FromLocationDescribation", orderBotData.FromLocationDescribatione);
//                command.Parameters.AddWithValue("@OrderDescribation", orderBotData.OrderDescribation);

//                connection.Open();


//                modified = Convert.ToInt32(command.ExecuteScalar());
//                if (connection.State == System.Data.ConnectionState.Open) connection.Close();

//            }

//            DeliveryOrderDetailsDto deliveryOrderDetailsDto = new DeliveryOrderDetailsDto
//            {
//                DeliveryCost = decimal.Parse(orderBotData.BranchCost),
//                DeliveryCostString = orderBotData.BranchCost,
//                FromAddress = orderBotData.FromAddress,
//                FromLocationId = orderBotData.FromLocationID,
//                FromGoogleURL = "https://maps.google.com/?q=" + orderBotData.LocationFrom,
//                TenantId = orderBotData.TenantID,
//                ToAddress = orderBotData.ToAddress,
//                ToLocationId = orderBotData.ToLocationID,
//                ToGoogleURL = "https://maps.google.com/?q=" + orderBotData.LocationTo,
//                OrderId = modified,




//            };


//            Add(deliveryOrderDetailsDto);

//            Order order = new Order
//            {
//                BranchAreaName = "BranchAreaName",
//                BranchAreaId = orderBotData.BranchAreaId,
//                Address = orderBotData.Address,
//                BranchId = orderBotData.BranchID,
//                ContactId = orderBotData.ContactId,
//                OrderTime = timeAdd,
//                CreationTime = timeAdd,
//                Id = orderBotData.Id,
//                OrderNumber = number,
//                TenantId = orderBotData.TenantID,
//                orderStatus = OrderStatusEunm.Pending,
//                OrderType = OrderTypeEunm.Delivery,
//                Total = decimal.Parse(orderBotData.BranchCost),
//                IsDeleted = false,
//                // AgentId = agId,
//                IsLockByAgent = false,
//                LocationID = orderBotData.LocationID
//            };

//            try
//            {
//                var titl = "the order Number: " + number.ToString();
//                var body = "Order Status :" + order.orderStatus + " From :" + orderBotData.FromAddress + " To :" + orderBotData.ToAddress;

//                await SendNotificationsAsync("fcm", orderBotData.TenantID.ToString(), titl, body);
//            }
//            catch
//            {

//            }



//            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//            var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//            getOrderForViewDto.Order = GetOrderMap;
//            getOrderForViewDto.OrderStatusName = orderStatusName;
//            getOrderForViewDto.OrderTypeName = orderTypeName;

//            getOrderForViewDto.BranchAreaName = "BranchAreaName";

//            getOrderForViewDto.IsAssginToAllUser = true; //area.IsAssginToAllUser;
//            getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
//            getOrderForViewDto.TenantId = orderBotData.TenantID;
//          //  await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);
//             SocketIOManager.SendOrder(getOrderForViewDto, orderBotData.TenantID.Value);

//            con.TotalOrder = con.TotalOrder + 1;
//            con.DeliveryOrder = con.DeliveryOrder + 1;
//            con.Description = orderBotData.Address;
//            con.Website = orderBotData.FromAddressEn + "," + orderBotData.ToAddressEn;
//            con.EmailAddress = orderBotData.LocationFrom;
//            var contact = _dbService.UpdateCustomerLocation(con).Result;

//            contact.customerChat = null;
//           // await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", contact);
//            SocketIOManager.SendContact(contact, contact.TenantId.HasValue ? contact.TenantId.Value : 0);
//            //delete bot conversation
//            // DeleteConversation(usertodelete.SunshineConversationId);
//            return ListString;

//        }

//        //AddHours(3)
//        [Route("CreateEvaluations")]
//        [HttpGet]
//        public void CreateEvaluations(int TenantId, string phoneNumber, string displayName, string EvaluationsText, string orderID, string EvaluationsReat)
//        {
//            var time = DateTime.Now;

//            var timeAdd = time.AddHours(AppSettingsModel.AddHour);

//            int evaluationRate = EvaluationsReat.ToCharArray().Count(c => c == '⭐');

//            Evaluation evaluation = new Evaluation
//            {
//                ContactName = displayName,
//                CreationTime = timeAdd,
//                EvaluationsReat = EvaluationsReat,
//                EvaluationsText = EvaluationsText.Replace("File upload:", "").Trim(),
//                OrderNumber = int.Parse(orderID),
//                EvaluationRate = evaluationRate > 0 ? evaluationRate : -1,
//                PhoneNumber = phoneNumber,
//                TenantId = TenantId

//            };
//            _evaluationRepository.InsertAsync(evaluation);

//           // _evaluationHub.Clients.All.SendAsync("brodCastBotEvaluation", evaluation);
//            SocketIOManager.SendEvaluation(evaluation, TenantId);

//        }

//        [Route("CreateBooking")]
//        [HttpPost]
//        public async Task<string> CreateBooking([FromBody] CreateBookingModel input)
//        {
//            var listBooking = GetBookingList(input.TenantID);
//            var tenentInfo = GetTenantByID(input.TenantID);
//            DateTime resulttimeNow = DateTime.Now.Date;



//            Bookings.Booking booking = new Bookings.Booking
//            {

//                bookingTypeEunm = BookingTypeEunm.Inside,
//                BookTableDate = DateTime.Now.Date,
//                UserId = "",
//                UserName = "",
//                UserPhone = "",
//                TableNumber = "",
//                BreakfastUserId = "",
//                BreakfastUserName = "",
//                BreakfastUserPhone = "",
//                DinnerUserId = "",
//                DinnerUserName = "",
//                DinnerUserPhone = "",
//                IsBreakfast = false,
//                IsDinner = false,
//                IsLunch = false,
//                LunchUserId = "",
//                LunchUserName = "",
//                LunchUserPhone = "",
//                NumberPersonsBreakfast = 0,
//                NumberPersonsBreakfastString = "",
//                NumberPersonsDinner = 0,
//                NumberPersonsDinnerString = "",
//                NumberPersonsLunch = 0,
//                NumberPersonsLunchString = "",
//                TenantId = 0


//            };

//            if (input.IsNow)
//            {

//                booking.BookTableDate = resulttimeNow;
//            }
//            else
//            {
//                var oldeDate = (input.SelectBookingDay.Substring(input.SelectBookingDay.LastIndexOf('(') + 1).Replace(")", "")).Trim();

//                var day = oldeDate.Split("/")[1];
//                var month = oldeDate.Split("/")[0];
//                var yeres = resulttimeNow.Year.ToString();
//                var date = Convert.ToDateTime(day + "/" + month + "/" + yeres);
//                booking.BookTableDate = date;
//            }


//            var type = 0;
//            var tablenumber = 0;
//            if (input.SelectBookingHall == "داخل الصالة")
//            {
//                var boo = listBooking.Where(x => x.TenantId == input.TenantID && x.bookingTypeEunm.ToString() == BookingTypeEunm.Inside.ToString() && x.BookTableDate == booking.BookTableDate).ToList();

//                //if(boo.Count()>= (tenentInfo.InsideNumber*3))
//                //{

//                //    return "نعتذر لا يوجد مكان متاح داخل الصالة ";
//                //}

//                tablenumber = tenentInfo.InsideNumber;
//                booking.bookingTypeEunm = BookingTypeEunm.Inside;

//                type = 1;
//            }
//            else
//            {
//                var boo = listBooking.Where(x => x.TenantId == input.TenantID && x.bookingTypeEunm == BookingTypeEunm.Outside && x.BookTableDate == booking.BookTableDate).ToList();
//                //if (boo.Count() >= (tenentInfo.OutsideNumber * 3))
//                //{

//                //    return "نعتذر لا يوجد مكان متاح خارج الصالة ";
//                //}
//                tablenumber = tenentInfo.OutsideNumber;
//                booking.bookingTypeEunm = BookingTypeEunm.Outside;
//                type = 2;
//            }

//            booking.TableNumber = UpateBooking((int)input.TenantID).ToString();
//            booking.TenantId = input.TenantID;
//            booking.UserId = input.UserId;
//            booking.UserName = input.DisplayName;
//            booking.UserPhone = input.phoneNumber;


//            //create
//            if (input.SelectBookingTime.Contains("فطور"))
//            {
//                var boo = listBooking.Where(x => x.TenantId == input.TenantID && x.IsBreakfast && x.BookTableDate == booking.BookTableDate && x.bookingTypeEunm == booking.bookingTypeEunm).ToList();
//                if (boo.Count() >= tablenumber)
//                {

//                    return "نعتذر الفطور غير متاح في هذا الوقت  الرجاء تغير وقت الحجز ";
//                }
//                booking.IsBreakfast = true;
//                booking.BreakfastUserId = input.UserId;
//                booking.BreakfastUserName = input.DisplayName;
//                booking.BreakfastUserPhone = input.phoneNumber;
//                booking.NumberPersonsBreakfastString = input.SelectBookingPeople;



//            }
//            else if (input.SelectBookingTime.Contains("غداء"))
//            {
//                var boo = listBooking.Where(x => x.TenantId == input.TenantID && x.IsLunch && x.BookTableDate == booking.BookTableDate && x.bookingTypeEunm == booking.bookingTypeEunm).ToList();
//                if (boo.Count() >= tablenumber)
//                {

//                    return "نعتذر الغداء غير متاح في هذا الوقت  الرجاء تغير وقت الحجز ";
//                }
//                booking.IsLunch = true;
//                booking.LunchUserId = input.UserId;
//                booking.LunchUserName = input.DisplayName;
//                booking.LunchUserPhone = input.phoneNumber;
//                booking.NumberPersonsLunchString = input.SelectBookingPeople;

//            }
//            else
//            {
//                var boo = listBooking.Where(x => x.TenantId == input.TenantID && x.IsDinner && x.BookTableDate == booking.BookTableDate && x.bookingTypeEunm == booking.bookingTypeEunm).ToList();
//                if (boo.Count() >= tablenumber)
//                {

//                    return "نعتذر العشاء غير متاح في هذا الوقت  الرجاء تغير وقت الحجز ";
//                }
//                booking.IsDinner = true;
//                booking.DinnerUserId = input.UserId;
//                booking.DinnerUserName = input.DisplayName;
//                booking.DinnerUserPhone = input.phoneNumber;
//                booking.NumberPersonsDinnerString = input.SelectBookingPeople;

//            }

//            CreateBooking(booking);

//            //  var captionText = "شكرا لكم 😊 . معلومات الحجز  : \r\n رقم الحجز :{0} \r\n الاسم :{1} \r\n الرقم :{2} \r\n تاريخ الحجز :{3} \r\n وقت الحجز :{4} \r\n عدد الاشخاص : {5} \r\n مكان الطاولة :{6} \r\n";

//            var text = string.Format(input.CaptionText.Replace("\\r\\n", "\r\n"), booking.TableNumber, input.DisplayName, input.phoneNumber, booking.BookTableDate.ToString("MM/dd/yyyy"), input.SelectBookingTime, input.SelectBookingPeople, input.SelectBookingHall);






//            return text;




//        }


//        [Route("TranslateText")]
//        [HttpPost]
//        public string TranslateText(string text, string lan)
//        {
//            try
//            {
//                var result = TranslatorFun(text, lan).Result;

//                return result;
//            }
//            catch
//            {

//                return text;
//            }


//        }

//        [Route("UpdateContact")]
//        [HttpGet]
//        public int UpdateContact(string phoneNumber, int OldContcatId, int tenantId)
//        {


//            //DB
//            var con = GetContact(OldContcatId);

//            if (con != null)
//            {
//                var userIdDB = tenantId + "_" + con.PhoneNumber;
//                var check = GetContactWithUserID(tenantId).Where(x => x.UserId == userIdDB).FirstOrDefault();
//                if (check == null)
//                {
//                    con.TenantId = tenantId;
//                    con.UserId = tenantId + "_" + con.PhoneNumber;
//                    var idcont = InsertContact(con);
//                    //cosmodeb
//                    CosmoDbUpdate(phoneNumber, tenantId, idcont);
//                    return idcont;
//                }
//                return check.Id;

//            }
//            return 0;


//        }



//        [Route("GetBookingTime")]
//        [HttpGet]
//        public List<string> GetBookingTime(int tenantId, string local, string SelectBookingHall, string SelectBookingDay, bool IsNow)
//        {
//            List<string> vs = new List<string>();
//            var dateNowWithTime = DateTime.Now.AddHours(AppSettingsModel.AddHour);
//            var dateNow = DateTime.Now.Date;
//            bool isNowDatea = false;
//            var date = DateTime.Now.Date;
//            var listBooking = GetBookingList(tenantId);
//            var tenentInfo = GetTenantByID(tenantId);
//            if (!IsNow)
//            {

//                var oldeDate = (SelectBookingDay.Substring(SelectBookingDay.LastIndexOf('(') + 1).Replace(")", "")).Trim();

//                var day = oldeDate.Split("/")[1];
//                var month = oldeDate.Split("/")[0];
//                var yeres = date.Year.ToString();
//                date = Convert.ToDateTime(day + "/" + month + "/" + yeres);


//                if (dateNow == date)
//                {

//                    isNowDatea = true;
//                }

//            }
//            else
//            {
//                isNowDatea = true;
//            }


//            var tablenumber = 0;
//            BookingTypeEunm bookingTypeEunm;
//            if (SelectBookingHall == "داخل الصالة")
//            {
//                var boo = listBooking.Where(x => x.TenantId == tenantId && x.bookingTypeEunm.ToString() == BookingTypeEunm.Inside.ToString() && x.BookTableDate == date).ToList();
//                tablenumber = tenentInfo.InsideNumber;
//                bookingTypeEunm = BookingTypeEunm.Inside;
//            }
//            else
//            {
//                var boo = listBooking.Where(x => x.TenantId == tenantId && x.bookingTypeEunm == BookingTypeEunm.Outside && x.BookTableDate == date).ToList();
//                tablenumber = tenentInfo.OutsideNumber;
//                bookingTypeEunm = BookingTypeEunm.Outside;
//            }



//            var booBreakfast = listBooking.Where(x => x.TenantId == tenantId && x.IsBreakfast && x.BookTableDate == date && x.bookingTypeEunm == bookingTypeEunm).ToList();


//            var booLunch = listBooking.Where(x => x.TenantId == tenantId && x.IsLunch && x.BookTableDate == date && x.bookingTypeEunm == bookingTypeEunm).ToList();
//            var booDinner = listBooking.Where(x => x.TenantId == tenantId && x.IsDinner && x.BookTableDate == date && x.bookingTypeEunm == bookingTypeEunm).ToList();

//            if (tablenumber > booBreakfast.Count)
//            {

//                if (isNowDatea)
//                {
//                    if (dateNowWithTime.Hour <= 12)
//                    {
//                        if (local == "ar")
//                        {
//                            vs.Add("فطور (9-12)");

//                        }
//                        else
//                        {
//                            vs.Add("breakfast (9-12)");

//                        }

//                    }


//                }
//                else
//                {
//                    if (local == "ar")
//                    {
//                        vs.Add("فطور (9-12)");

//                    }
//                    else
//                    {
//                        vs.Add("breakfast (9-12)");

//                    }
//                }


//            }
//            if (tablenumber > booLunch.Count)
//            {



//                if (isNowDatea)
//                {
//                    if (dateNowWithTime.Hour <= 16)
//                    {
//                        if (local == "ar")
//                        {
//                            vs.Add("غداء (1-4)");

//                        }
//                        else
//                        {
//                            vs.Add("lunch (1-4)");

//                        }

//                    }


//                }
//                else
//                {
//                    if (local == "ar")
//                    {
//                        vs.Add("غداء (1-4)");

//                    }
//                    else
//                    {
//                        vs.Add("lunch (1-4)");

//                    }
//                }





//            }
//            if (tablenumber > booDinner.Count)
//            {

//                if (isNowDatea)
//                {
//                    if (dateNowWithTime.Hour <= 24)
//                    {
//                        if (local == "ar")
//                        {
//                            vs.Add("عشاء (5-12)");

//                        }
//                        else
//                        {
//                            vs.Add("Dinner (5-12)");

//                        }

//                    }


//                }
//                else
//                {
//                    if (local == "ar")
//                    {
//                        vs.Add("عشاء (5-12)");

//                    }
//                    else
//                    {
//                        vs.Add("Dinner (5-12)");

//                    }
//                }


//            }

//            return vs;


//        }

//        [Route("CancelBooking")]
//        [HttpGet]
//        public bool CancelBooking(int TenantID, string BookingNumber, string UserId)
//        {
//            var listBooking = GetBookingList(TenantID).Where(x => x.UserId == UserId && x.TableNumber == BookingNumber).FirstOrDefault();

//            if (listBooking != null)
//            {

//                DeleteBooking(listBooking.Id);

//                return true;
//            }
//            else
//            {

//                return false;

//            }

//        }



//        [Route("ChickDate")]
//        [HttpGet]
//        public bool ChickDate()
//        {
//            var timeNow = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("HH:mm:ss", CultureInfo.InvariantCulture);


//            if (int.Parse(timeNow.Split(":")[0]) >= 20)
//            {
//                return true;

//            }
//            else
//            {
//                return false;
//            }


//        }


//        [Route("UpdateMaintenance")]
//        [HttpPost]
//        public async Task UpdateMaintenanceAsync([FromBody] Models.BotModel.CreateMaintenanceModel model)
//        {
//            var man = CreateMaintenance(model);
//            await _maintenanceshub.Clients.All.SendAsync("MaintenancesBotOrder", man);
//        }

//        [Route("UpdateUserToken")]
//        [HttpPost]
//        public void UpdateUserToken([FromBody] UserTokenModel userTokenModel)
//        {
//            _iUserAppService.UpdateUserToken(userTokenModel);


//        }

//        #endregion

//        #region private
//        private void Add(DeliveryOrderDetailsDto deliveryLocationCost)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "INSERT INTO DeliveryOrderDetails (FromLocationId , ToLocationId, TenantId, DeliveryCost, FromAddress, FromGoogleURL, ToAddress, ToGoogleURL, DeliveryCostString, OrderId) VALUES (@FromLocationId ,@ToLocationId, @TenantId, @DeliveryCost, @FromAddress, @FromGoogleURL, @ToAddress, @ToGoogleURL, @DeliveryCostString, @OrderId) ";

//                        command.Parameters.AddWithValue("@Id", deliveryLocationCost.Id);
//                        command.Parameters.AddWithValue("@FromLocationId", deliveryLocationCost.FromLocationId);
//                        command.Parameters.AddWithValue("@ToLocationId", deliveryLocationCost.ToLocationId);
//                        command.Parameters.AddWithValue("@TenantId", deliveryLocationCost.TenantId);
//                        command.Parameters.AddWithValue("@DeliveryCost", deliveryLocationCost.DeliveryCost);

//                        command.Parameters.AddWithValue("@FromAddress", deliveryLocationCost.FromAddress);
//                        command.Parameters.AddWithValue("@FromGoogleURL", deliveryLocationCost.FromGoogleURL);
//                        command.Parameters.AddWithValue("@ToAddress", deliveryLocationCost.ToAddress);
//                        command.Parameters.AddWithValue("@ToGoogleURL", deliveryLocationCost.ToGoogleURL);
//                        command.Parameters.AddWithValue("@DeliveryCostString", deliveryLocationCost.DeliveryCostString);
//                        command.Parameters.AddWithValue("@OrderId", deliveryLocationCost.OrderId);


//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception e)
//                {


//                }

//        }
//        private List<Area> GetAreas(int TenantID)
//        {

//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Areas] where TenantID=" + TenantID;

//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<Area> branches = new List<Area>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                try
//                {

//                    branches.Add(new Area
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString(),
//                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString()

//                    });
//                }
//                catch
//                {
//                    branches.Add(new Area
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }

//        private Area GetAreasID(int ID)
//        {

//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Areas] where Id=" + ID;

//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            Area branches = new Area();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                try
//                {

//                    branches=new Area
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString(),
//                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString()

//                    };
//                }
//                catch
//                {
//                    branches=new Area
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                    };

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }
//        private void CosmoDbUpdate(string phoneNumber, int tenantId, int contactID)
//        {
//            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
//            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == "22_" + phoneNumber).Result;//&& a.TenantId== TenantId

//            if (customerResult != null)
//            {

//                var userIdCosmo = tenantId + "_" + customerResult.phoneNumber;

//                var check = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userIdCosmo).Result;

//                if (check == null)
//                {
//                    var customerChat = UpdateCustomerChatD360(tenantId, customerResult.phoneNumber, customerResult.customerChat.text, customerResult.customerChat.type, customerResult.customerChat.mediaUrl, 1);
//                    customerResult.customerChat = customerChat;

//                    var CustomerModel = new CustomerModel()
//                    {
//                        ContactID = contactID.ToString(),////
//                        IsComplaint = customerResult.IsComplaint,
//                        userId = userIdCosmo,
//                        displayName = customerResult.displayName,
//                        avatarUrl = customerResult.avatarUrl,
//                        type = customerResult.type,
//                        D360Key = customerResult.D360Key,
//                        CreateDate = customerResult.CreateDate,
//                        IsLockedByAgent = customerResult.IsLockedByAgent,
//                        LockedByAgentName = customerResult.LockedByAgentName,
//                        IsOpen = customerResult.IsOpen,
//                        agentId = customerResult.agentId,
//                        IsBlock = customerResult.IsBlock,
//                        IsConversationExpired = customerResult.IsConversationExpired,
//                        CustomerChatStatusID = customerResult.CustomerChatStatusID,
//                        CustomerStatusID = customerResult.CustomerStatusID,
//                        LastMessageData = customerResult.LastMessageData,
//                        IsNew = customerResult.IsNew,
//                        TenantId = tenantId,
//                        phoneNumber = customerResult.phoneNumber,
//                        UnreadMessagesCount = customerResult.UnreadMessagesCount,
//                        IsNewContact = customerResult.IsNewContact,
//                        IsBotChat = customerResult.IsBotChat,
//                        IsBotCloseChat = customerResult.IsBotCloseChat,
//                        loyalityPoint = customerResult.loyalityPoint,
//                        TotalOrder = customerResult.TotalOrder,
//                        TakeAwayOrder = customerResult.TakeAwayOrder,
//                        DeliveryOrder = customerResult.DeliveryOrder,
//                    };

//                    var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
//                }



//            }
//        }
//        private int InsertContact(Contact contact)
//        {

//            int modified = 0;
//            string connString = AppSettingsModel.ConnectionStrings;
//            //return orderCount.Count().ToString();
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "INSERT INTO Contacts (TenantId, AvatarUrl, DisplayName, PhoneNumber, SunshineAppID, IsLockedByAgent, LockedByAgentName, IsOpen,Website,EmailAddress,Description,ChatStatuseId,ContactStatuseId,CreationTime,CreatorUserId,DeleterUserId,DeletionTime,IsDeleted,LastModificationTime,LastModifierUserId,UserId,IsConversationExpired,IsBlock) " +
//                    " VALUES (@TenantId, @AvatarUrl, @DisplayName, @PhoneNumber, @SunshineAppID, @IsLockedByAgent, @LockedByAgentName, @IsOpen , @Website, @EmailAddress, @Description, @ChatStatuseId, @ContactStatuseId, @CreationTime, @CreatorUserId, @DeleterUserId, @DeletionTime, @IsDeleted, @LastModificationTime, @LastModifierUserId, @UserId, @IsConversationExpired, @IsBlock) ;SELECT SCOPE_IDENTITY();";

//                command.Parameters.AddWithValue("@TenantId", contact.TenantId);
//                command.Parameters.AddWithValue("@AvatarUrl", contact.AvatarUrl ?? Convert.DBNull);
//                command.Parameters.AddWithValue("@DisplayName", contact.DisplayName);
//                command.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
//                command.Parameters.AddWithValue("@SunshineAppID", "");
//                command.Parameters.AddWithValue("@IsLockedByAgent", contact.IsLockedByAgent);
//                command.Parameters.AddWithValue("@LockedByAgentName", contact.LockedByAgentName ?? Convert.DBNull);
//                command.Parameters.AddWithValue("@IsOpen", contact.IsOpen);
//                command.Parameters.AddWithValue("@Website", contact.Website ?? Convert.DBNull);

//                command.Parameters.AddWithValue("@EmailAddress", contact.EmailAddress ?? Convert.DBNull);
//                command.Parameters.AddWithValue("@Description", contact.Description ?? Convert.DBNull);
//                command.Parameters.AddWithValue("@ChatStatuseId", Convert.DBNull);
//                command.Parameters.AddWithValue("@ContactStatuseId", Convert.DBNull);
//                command.Parameters.AddWithValue("@CreationTime", contact.CreationTime);
//                command.Parameters.AddWithValue("@CreatorUserId", Convert.DBNull);
//                command.Parameters.AddWithValue("@DeleterUserId", Convert.DBNull);
//                command.Parameters.AddWithValue("@DeletionTime", Convert.DBNull);
//                command.Parameters.AddWithValue("@IsDeleted", contact.IsDeleted);

//                command.Parameters.AddWithValue("@LastModificationTime", Convert.DBNull);
//                command.Parameters.AddWithValue("@LastModifierUserId", Convert.DBNull);
//                command.Parameters.AddWithValue("@UserId", contact.UserId);
//                command.Parameters.AddWithValue("@IsConversationExpired", contact.IsConversationExpired);
//                command.Parameters.AddWithValue("@IsBlock", contact.IsBlock);

//                connection.Open();
//                modified = Convert.ToInt32(command.ExecuteScalar());
//                if (connection.State == System.Data.ConnectionState.Open) connection.Close();


//                return modified;

//            }


//        }
//        private CustomerChat UpdateCustomerChatD360(int TenantId, string phonenumber, string textt, string typee, string mediaUrll, int count)
//        {
//            try
//            {


//                CustomerChat CustomerChat = new CustomerChat();
//                string userId = TenantId + "_" + phonenumber;
//                string text = textt;
//                string type = typee;
//                string mediaUrl = mediaUrll;
//                var itemsCollection = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);

//                if (type == "location")
//                {

//                    // Create your new conversation instance
//                    CustomerChat = new CustomerChat()
//                    {
//                        TenantId = TenantId,
//                        userId = userId,
//                        text = "https://maps.google.com/?q=" + text,
//                        type = "location",//type,
//                        CreateDate = DateTime.Now,
//                        status = (int)Messagestatus.New,
//                        sender = MessageSenderType.Customer,
//                        mediaUrl = mediaUrl,
//                        UnreadMessagesCount = count
//                    };
//                }
//                else
//                {
//                    // Create your new conversation instance
//                    CustomerChat = new CustomerChat()
//                    {
//                        TenantId = TenantId,
//                        userId = userId,
//                        text = text,
//                        type = type,
//                        CreateDate = DateTime.Now,
//                        status = (int)Messagestatus.New,
//                        sender = MessageSenderType.Customer,
//                        mediaUrl = mediaUrl,
//                        UnreadMessagesCount = count
//                    };

//                }




//                var Result = itemsCollection.CreateItemAsync(CustomerChat).Result;
//                return CustomerChat;
//            }
//            catch (Exception ex)
//            {

//                throw;
//            }

//        }
//        private async Task SendNotificationsAsync(string pns, string to_tag, string title, string body)
//        {
//            var user = "admin";// HttpContext.User.Identity.Name;
//            string[] userTag = new string[1];
//            userTag[0] = to_tag;
//            // userTag[1] = "from:" + user;

//            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
//            HttpStatusCode ret = HttpStatusCode.InternalServerError;
//            try
//            {
//                switch (pns.ToLower())
//                {
//                    case "wns":
//                        // Windows 8.1 / Windows Phone 8.1
//                        var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
//                                    "From " + user + ": " + "message" + "</text></binding></visual></toast>";
//                        outcome = await Models.Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
//                        break;
//                    case "apns":
//                        // iOS
//                        var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + "message" + "\"}}";
//                        outcome = await Models.Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
//                        break;
//                    case "fcm":
//                        // Android
//                        SendNotfificationModel sendNotfificationModel = new SendNotfificationModel
//                        {
//                            notification = new SendNotfificationModel.Notification
//                            {
//                                title = title,
//                                body = body
//                            },
//                            data = new SendNotfificationModel.Data
//                            {
//                                property1 = "value1",
//                                property2 = 42
//                            }

//                        };
//                        var notif = JsonConvert.SerializeObject(sendNotfificationModel).ToString();
//                        // var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + "message" + "\"}}";
//                        outcome = await Models.Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
//                        break;
//                }
//            }
//            catch (Exception ex)
//            {


//            }


//            if (outcome != null)
//            {
//                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
//                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
//                {
//                    ret = HttpStatusCode.OK;
//                }
//            }

//        }


//        private void SendMobileNotification(int? TenaentId, string title, string msg)
//        {
//            var result = "-1";
//            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
//            httpWebRequest.ContentType = "application/json";
//            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
//            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
//            httpWebRequest.Method = "POST";

//            //var payload = new
//            //{
//            //    registration_ids = GetUserByTeneantId((int)TenaentId),
//            //    // to = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync("iyuiyu").Result,
//            //    priority = "high",
//            //    content_available = true,
//            //    notification = new
//            //    {
//            //        body = msg,
//            //        title = title,
//            //        sound = "sound"
//            //    },
//            //};

//            var payload = new
//            {
//                registration_ids = GetUserByTeneantId((int)TenaentId),
//                data = new
//                {

//                    body = msg,
//                    title = title,
//                    sound = "sound"
//                },
//                priority = "high",
//                payload = new
//                {
//                    aps = new
//                    {
//                        sound = "sound"
//                    }
//                },
//                android = new
//                {
//                    notification = new
//                    {
//                        channel_id = "high_importance_channel"
//                    }
//                },
//                aps= new
//                     {
//                      alert= title+" , "+msg,
//                      sound="sound.caf"
//                     }
//                };

//            var serializer = new JavaScriptSerializer();
//            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
//            {
//                string json = serializer.Serialize(payload);
//                streamWriter.Write(json);
//                streamWriter.Flush();
//            }


//            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
//            {
//                result = streamReader.ReadToEnd();
//            }

//        }

//        private void SendMobileCancelNotification(int? TenaentId, string title, string msg)
//        {
//            var result = "-1";
//            var httpWebRequest = (HttpWebRequest)WebRequest.Create(FcmNotificationSetting.webAddr);
//            httpWebRequest.ContentType = "application/json";
//            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", FcmNotificationSetting.ServerKey));
//            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", FcmNotificationSetting.SenderId));
//            httpWebRequest.Method = "POST";

//            //var payload = new
//            //{
//            //    registration_ids = GetUserByTeneantId((int)TenaentId),
//            //    // to = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync("iyuiyu").Result,
//            //    priority = "high",
//            //    content_available = true,
//            //    notification = new
//            //    {
//            //        body = msg,
//            //        title = title,
//            //        sound = "sound"
//            //    },
//            //};

//            var payload = new
//            {
//                registration_ids = GetUserByTeneantId((int)TenaentId),
//                data = new
//                {

//                    body = msg,
//                    title = title,
//                    sound = "cancel"
//                },
//                priority = "high",
//                payload = new
//                {
//                    aps = new
//                    {
//                        sound = "cancel"
//                    }
//                },
//                android = new
//                {
//                    notification = new
//                    {
//                        channel_id = "high_importance_channel_cancel"
//                    }
//                },
//                aps = new
//                {
//                    alert = title+" , "+msg,
//                    sound = "cancel.caf"
//                }
           
//        };

//            var serializer = new JavaScriptSerializer();
//            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
//            {
//                string json = serializer.Serialize(payload);
//                streamWriter.Write(json);
//                streamWriter.Flush();
//            }


//            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
//            {
//                result = streamReader.ReadToEnd();
//            }

//        }
//        private void UpdateTextLocation(LocationInfoModel location)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "UPDATE Locations SET LocationNameEn = @LocationNameEn  Where Id = @Id";

//                command.Parameters.AddWithValue("@Id", location.Id);
//                command.Parameters.AddWithValue("@LocationNameEn", location.LocationNameEn);
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//        }
//        private void UpdateDeliveryOrderDB(UpdateOrderModel updateOrderModel, DateTime timeAdd, string connString, long number, string BranchAreaName, Area area)
//        {
//            try
//            {
//                decimal totalWithBranchCost = 0;
//                decimal deleverfees = 0;

//                if (updateOrderModel.BuyType == "No select")
//                    updateOrderModel.BuyType = "";

//                if (updateOrderModel.isOrderOfferCost)
//                {
//                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostBefor);
//                    deleverfees = updateOrderModel.DeliveryCostBefor;
//                }
//                else
//                {
//                    totalWithBranchCost = (updateOrderModel.OrderTotal + updateOrderModel.DeliveryCostAfter);
//                    deleverfees = updateOrderModel.DeliveryCostAfter;
//                }
//                using (SqlConnection connection = new SqlConnection(connString))
//                using (SqlCommand command = connection.CreateCommand())
//                {
//                    //valdet if the order exist or not 
//                    command.CommandText = "UPDATE Orders SET OrderTime = @OrT, OrderStatus = @OrI ,CreationTime =@CreT ,OrderType = @OrTy, Total= @Tot ,Address=@Add ,DeliveryCost=@Dcos ,IsEvaluation=@IsEv ,BranchAreaId=@BranchAreaId  ,BranchAreaName=@BranchAreaName, OrderNumber =@OrderNumber ,LocationID=@LocationID ,FromLocationDescribation=@FromLocationDescribation, AfterDeliveryCost=@AfterDeliveryCost , SelectDay=@SelectDay, SelectTime=@SelectTime, IsPreOrder=@IsPreOrder, RestaurantName=@RestaurantName , IsSpecialRequest=@IsSpecialRequest,SpecialRequestText=@SpecialRequestText , BuyType=@BuyType ,OrderLocal=@OrderLocal ,AgentId=@AgentId,AgentIds=@AgentIds,StreetName=@StreetName,BuildingNumber=@BuildingNumber,FloorNo=@FloorNo,ApartmentNumber=@ApartmentNumber  Where Id = @Id";
//                    command.Parameters.AddWithValue("@Id", updateOrderModel.OrderId);
//                    command.Parameters.AddWithValue("@OrT", timeAdd);
//                    command.Parameters.AddWithValue("@CreT", timeAdd);
//                    command.Parameters.AddWithValue("@Tot", totalWithBranchCost);
//                    command.Parameters.AddWithValue("@Add", updateOrderModel.Address);
//                    command.Parameters.AddWithValue("@Dcos", deleverfees);
//                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Delivery);

//                    command.Parameters.AddWithValue("@OrderLocal", updateOrderModel.BotLocal);

//                    command.Parameters.AddWithValue("@BuyType", updateOrderModel.BuyType);

//                    if (updateOrderModel.IsPreOrder)
//                    {
//                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pre_Order);
//                    }
//                    else
//                    {
//                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
//                    }



//                    if (area != null && area.Id != 0)
//                    {
//                        command.Parameters.AddWithValue("@BranchAreaId", area.Id);
//                        if (area.UserId != 0 && area.UserId != null)
//                        {
//                            command.Parameters.AddWithValue("@AgentId", area.UserId);

//                        }
//                        else
//                        {
//                            command.Parameters.AddWithValue("@AgentId", 0);
//                        }
//                        if (!string.IsNullOrEmpty(area.UserIds))
//                        { command.Parameters.AddWithValue("@AgentIds", area.UserIds); }
//                        else
//                        {
//                            command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
//                        }

//                    }
//                    else
//                    {
//                        command.Parameters.AddWithValue("@AgentId", 0);
//                        command.Parameters.AddWithValue("@AgentIds", DBNull.Value);

//                        command.Parameters.AddWithValue("@BranchAreaId", 0);
//                    }




//                    command.Parameters.AddWithValue("@IsEv", false);


//                    command.Parameters.AddWithValue("@BranchAreaName", BranchAreaName);// orderBotData.BranchName);//نفيسة
//                    command.Parameters.AddWithValue("@RestaurantName", updateOrderModel.BranchName);//دوار المدينة

//                    command.Parameters.AddWithValue("@LocationID", updateOrderModel.BranchId);




//                    command.Parameters.AddWithValue("@FromLocationDescribation", "https://maps.google.com/?q=" + updateOrderModel.LocationFrom.Replace(" ", ""));
//                    command.Parameters.AddWithValue("@OrderNumber", number);

//                    command.Parameters.AddWithValue("@AfterDeliveryCost", updateOrderModel.DeliveryCostAfter);

//                    command.Parameters.AddWithValue("@SelectDay", updateOrderModel.SelectDay);
//                    command.Parameters.AddWithValue("@SelectTime", updateOrderModel.SelectTime);
//                    command.Parameters.AddWithValue("@IsPreOrder", updateOrderModel.IsPreOrder);

//                    //  command.Parameters.AddWithValue("@HtmlPrint", htmlOrderD);



//                    command.Parameters.AddWithValue("@IsSpecialRequest", updateOrderModel.IsSpecialRequest);
//                    command.Parameters.AddWithValue("@SpecialRequestText", updateOrderModel.SpecialRequest);




//                    if (updateOrderModel.StreetName != null)
//                    {
//                        command.Parameters.AddWithValue("@StreetName", updateOrderModel.StreetName);
//                        command.Parameters.AddWithValue("@BuildingNumber", updateOrderModel.BuildingNumber);
//                        command.Parameters.AddWithValue("@FloorNo", updateOrderModel.FloorNo);
//                        command.Parameters.AddWithValue("@ApartmentNumber", updateOrderModel.ApartmentNumber);



//                    }
//                    else
//                    {
//                        command.Parameters.AddWithValue("@StreetName", DBNull.Value);
//                        command.Parameters.AddWithValue("@BuildingNumber", DBNull.Value);
//                        command.Parameters.AddWithValue("@FloorNo", DBNull.Value);
//                        command.Parameters.AddWithValue("@ApartmentNumber", DBNull.Value);
//                    }


//                    connection.Open();
//                    command.ExecuteNonQuery();

//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {


//            }

//        }
//        private void updateOrderDB(UpdateOrderModel updateOrderModel, DateTime timeAdd, string connString, long number, Area area)
//        {
//            try
//            {
//                using (SqlConnection connection = new SqlConnection(connString))
//                using (SqlCommand command = connection.CreateCommand())
//                {


//                    command.CommandText = "UPDATE Orders SET Total = @Total , OrderTime = @OrT ,CreationTime =@CreT , OrderStatus = @OrI , OrderType = @OrTy ,AreaId = @ArI ,IsEvaluation=@IsEv , OrderNumber =@OrderNumber  ,IsSpecialRequest=@IsSpecialRequest,SpecialRequestText=@SpecialRequestText , OrderLocal=@OrderLocal ,AgentId=@AgentId, AgentIds=@AgentIds ,BranchAreaId=@BranchAreaId Where Id = @Id";

//                    command.Parameters.AddWithValue("@OrderLocal", updateOrderModel.BotLocal);
//                    command.Parameters.AddWithValue("@Id", updateOrderModel.OrderId);
//                    command.Parameters.AddWithValue("@OrT", timeAdd);
//                    command.Parameters.AddWithValue("@CreT", timeAdd);
//                    command.Parameters.AddWithValue("@ArI", updateOrderModel.BranchId);
//                    command.Parameters.AddWithValue("@SpecialRequestText", updateOrderModel.SpecialRequest);
//                    command.Parameters.AddWithValue("@IsSpecialRequest", updateOrderModel.IsSpecialRequest);
//                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Takeaway);
//                    command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
//                    command.Parameters.AddWithValue("@IsEv", false);
//                    command.Parameters.AddWithValue("@OrderNumber", number);
//                    // command.Parameters.AddWithValue("@HtmlPrint", htmlOrder);


//                    if (area != null)
//                    {
//                        command.Parameters.AddWithValue("@BranchAreaId", area.Id);
//                        if (area.UserId != 0 && area.UserId != null)
//                        {
//                            command.Parameters.AddWithValue("@AgentId", area.UserId);

//                        }
//                        else
//                        {
//                            command.Parameters.AddWithValue("@AgentId", 0);
//                        }
//                        if(!string.IsNullOrEmpty(area.UserIds))
//                         command.Parameters.AddWithValue("@AgentIds", area.UserIds);
//                         else
//                        {
//                            command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
//                        }
//                    }
//                    else
//                    {
//                        command.Parameters.AddWithValue("@AgentIds", DBNull.Value);
//                    }



//                    command.Parameters.AddWithValue("@Total", updateOrderModel.OrderTotal);
//                    connection.Open();
//                    command.ExecuteNonQuery();
//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {

//            }

//        }
//        private List<Order> GetOrderListWithContact(int? TenantID, int ContactId)
//        {

//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and ContactId=" + ContactId;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<Order> order = new List<Order>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
//                if (!IsDeleted)
//                {

//                    try
//                    {

//                        order.Add(new Order
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
//                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
//                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
//                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
//                            AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
//                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString()

//                        });

//                    }
//                    catch
//                    {

//                        order.Add(new Order
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
//                            Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                            ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
//                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
//                            OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
//                            AgentIds = dataSet.Tables[0].Rows[i]["AgentIds"].ToString()


//                        });

//                    }


//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }
//        private async Task UpdateOrderAfterCancel(string OrderNumber, int ContactId, Order OrderModel, int? TenantID)
//        {
//            var con = GetContact(ContactId);

//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {
//                command.CommandText = "UPDATE Orders SET  OrderStatus = @OrI, OrderRemarks=@Rema  Where Id = @Id";
//                command.Parameters.AddWithValue("@Id", OrderModel.Id);
//                command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Canceled);
//                command.Parameters.AddWithValue("@Rema", "CancelByCustomer");

//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }

//            OrderModel.OrderNumber = long.Parse(OrderNumber);
//            OrderModel.ContactId = ContactId;
//            OrderModel.orderStatus = OrderStatusEunm.Canceled;

//            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), OrderModel.orderStatus);
//            var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), OrderModel.OrderType);
//            GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//            var GetOrderMap = ObjectMapper.Map(OrderModel, getOrderForViewDto.Order);

//            getOrderForViewDto.Order = GetOrderMap;
//            getOrderForViewDto.OrderStatusName = orderStatusName;
//            getOrderForViewDto.AgentIds = OrderModel.AgentIds;
//            getOrderForViewDto.OrderTypeName = orderTypeName;

//            getOrderForViewDto.CustomerCustomerName = con.DisplayName;
//            getOrderForViewDto.TenantId = TenantID;
//            getOrderForViewDto.DeliveryChangeDeliveryServiceProvider = OrderModel.CreationTime.ToString("MM/dd hh:mm tt");


//            getOrderForViewDto.IsAssginToAllUser = true;


//            try
//            {
//                var area = GetAreaId(int.Parse(OrderModel.AreaId.ToString()));

//                if (area != null)
//                {
//                    getOrderForViewDto.AreahName = area.AreaName;
//                }

//            }
//            catch
//            {

//                getOrderForViewDto.AreahName = "";
//            }



//            //GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//            // var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);
//            // GetOrderMap.StringTotal = stringTotall;
//            // getOrderForViewDto.Order = GetOrderMap;
//            //  getOrderForViewDto.OrderStatusName = orderStatusName;


//            //  getOrderForViewDto.TenantId = AbpSession.TenantId;
//            // getOrderForViewDto.ActionTime = DateTime.Now.AddHours(AppSettingsModel.AddHour).ToString("hh:mm tt");




//            //await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);

//             SocketIOManager.SendOrder(getOrderForViewDto, TenantID.HasValue? TenantID.Value:0);
//            var titl = "The Order Number: " + OrderNumber;
//            var body = "Order Status :" + OrderStatusEunm.Canceled;

//            // await SendNotificationsAsync("fcm", updateOrderModel.TenantID.ToString(), titl, body);
//            SendMobileCancelNotification(TenantID, titl, body);


//            // Notification
//            //try
//            //{
//            //    var message = titl+" "+ body;
//            //    UserNotification Notification = await SendNotfAsync(message, OrderModel.AgentId, TenantID);


//            //}
//            //catch
//            //{

//            //}
//        }


//        private Order GetOrder(int? TenantID, int ContactId)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and ContactId=" + ContactId;

//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            Order order = new Order();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
//                if (!IsDeleted)
//                {

//                    order = new Order
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
//                        Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                        ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
//                        CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
//                        OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),

//                    };

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }
//        private List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[OrderDetails] where TenantID=" + TenantID + " and OrderId=" + OrderId;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<OrderDetailDto> orderDetailDtos = new List<OrderDetailDto>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                orderDetailDtos.Add(new OrderDetailDto
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    Discount = decimal.Parse(dataSet.Tables[0].Rows[i]["Discount"].ToString()),
//                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
//                    OrderId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderId"]),
//                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
//                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



//                });
//            }

//            conn.Close();
//            da.Dispose();

//            return orderDetailDtos;
//        }
//        private List<ExtraOrderDetailsDto> GetOrderDetailExtra(int? TenantID)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<ExtraOrderDetailsDto> orderDetailDtos = new List<ExtraOrderDetailsDto>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                orderDetailDtos.Add(new ExtraOrderDetailsDto
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    OrderDetailId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderDetailId"]),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                    Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Quantity"]),
//                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
//                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
//                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



//                });
//            }

//            conn.Close();
//            da.Dispose();

//            return orderDetailDtos;
//        }
//        private List<ItemDto> GetItem(int? TenantID, long id)
//        {

//            List<ItemDto> itemDtos = new List<ItemDto>();
//            ItemDto item = _itemsAppService.GetItemInfoForBot(id, TenantID.Value);
//            itemDtos.Add(item);
//            return itemDtos;


//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Items] where  TenantID=" + TenantID + " and Id = " + id;
//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);



//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

//                if (!IsDeleted)
//                {
//                    itemDtos.Add(new ItemDto
//                    {
//                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
//                        CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
//                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
//                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
//                        IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
//                        ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
//                        ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
//                        ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
//                        ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
//                        Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
//                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"])

//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return itemDtos;

//        }

//        private async Task<List<GetOrderDetailForViewDto>> GetAllDetail(int? TenantId, int orderId, string lang)
//        {

//            List<GetOrderDetailForViewDto> getOrderDetailForViewDto = new List<GetOrderDetailForViewDto>();
//            var OrderDetail = GetOrderDetail(TenantId, orderId);

//            //  var itemslist = GetItem(TenantId);

//            foreach (var item in OrderDetail)
//            {
//                var items = GetItem(TenantId, long.Parse(item.ItemId.ToString())).FirstOrDefault();// itemslist.Where(x => x.Id == item.ItemId).FirstOrDefault();

//                if (items != null)
//                {
//                    getOrderDetailForViewDto.Add(new GetOrderDetailForViewDto
//                    {

//                        OrderDetail = item,
//                        ItemName = items.ItemName,
//                        ItemNameEnglish = items.ItemNameEnglish
//                    });
//                }
//                else
//                {

//                    //getOrderDetailForViewDto.Add(new GetOrderDetailForViewDto
//                    //{

//                    //    OrderDetail = new OrderDetailDto() {  },
//                    //    ItemName = items.ItemName,
//                    //    ItemNameEnglish = items.ItemNameEnglish
//                    //});
//                }




//            }



//            var exListList = GetOrderDetailExtra(TenantId);


//            foreach (var item in getOrderDetailForViewDto)
//            {

//                var exList = exListList.Where(x => x.OrderDetailId == item.OrderDetail.Id).ToList();


//                List<ExtraOrderDetailsDto> extraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
//                foreach (var ext in exList)
//                {

//                    if (lang == "ar")
//                    {
//                        extraOrderDetailsDto.Add(new ExtraOrderDetailsDto
//                        {
//                            Id = 0,
//                            Name = ext.Name,
//                            OrderDetailId = ext.OrderDetailId,
//                            Quantity = ext.Quantity,
//                            TenantId = ext.TenantId,
//                            Total = ext.Total,
//                            UnitPrice = ext.UnitPrice,

//                        });
//                    }
//                    else
//                    {
//                        extraOrderDetailsDto.Add(new ExtraOrderDetailsDto
//                        {
//                            Id = 0,
//                            Name = ext.NameEnglish,
//                            OrderDetailId = ext.OrderDetailId,
//                            Quantity = ext.Quantity,
//                            TenantId = ext.TenantId,
//                            Total = ext.Total,
//                            UnitPrice = ext.UnitPrice,

//                        });

//                    }





//                    // item.OrderDetail.extraOrderDetailsDtos.Add(extraOrderDetailsDto);




//                }
//                item.OrderDetail.extraOrderDetailsDtos = extraOrderDetailsDto;






//            }




//            return getOrderDetailForViewDto;

//        }
//        private string GetOrderDetailString(int TenantID, string lang, decimal? Total, string captionQuantityText, string captionAddtionText, string captionTotalText, string captionTotalOfAllText, List<OrderDetailDto> OrderDetailList, List<ExtraOrderDetailsDto> getOrderDetailExtraList, decimal discount, bool isdiscount)
//        {

//            //var captionQuantityText = GetCaptionFormat("BackEnd_Text_Quantity", lang, TenantID, "", "", 0);//العدد :
//            //var captionAddtionText = GetCaptionFormat("BackEnd_Text_Addtion", lang, TenantID, "", "", 0);//الاضافات
//            //var captionTotalText = GetCaptionFormat("BackEnd_Text_Total", lang, TenantID, "", "", 0);//المجموع:       
//            //var captionTotalOfAllText = GetCaptionFormat("BackEnd_Text_TotalOfAll", lang, TenantID, "", "", 0);//السعر الكلي للصنف: 



//            var listString = "-------------------------- \r\n";
//            listString = listString + "-------------------------- \r\n\r\n";
//            decimal? total = 0;

//            foreach (var OrderD in OrderDetailList)
//            {

//                var getOrderDetailExtra = getOrderDetailExtraList.Where(x => x.OrderDetailId == OrderD.Id).ToList();
//                var item = GetItem(TenantID, long.Parse(OrderD.ItemId.ToString())).FirstOrDefault();// itemList.Where(x => x.Id == OrderD.ItemId).FirstOrDefault();

//                if (item != null)
//                {
//                    if (lang == "ar")
//                    {
//                        listString = listString + "*" + item.ItemName.Trim() + "*" + "\r\n";
//                    }
//                    else
//                    {
//                        listString = listString + "*" + item.ItemNameEnglish.Trim() + "*" + "\r\n";
//                    }

//                    if (getOrderDetailExtra.Count > 0)
//                    {
//                        listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

//                    }
//                }
//                else
//                {
//                    if (lang == "ar")
//                    {
//                        if (OrderD.IsCrispy)
//                        {
//                            listString = listString + "*" + "كرسبي" + "*" + "\r\n";

//                        }
//                        else if (OrderD.IsDeserts)
//                        {
//                            listString = listString + "*" + "حلويات" + "*" + "\r\n";

//                        }
//                        else if (OrderD.IsCondiments)
//                        {
//                            listString = listString + "*" + "الصوص" + "*" + "\r\n";

//                        }
//                    }
//                    else
//                    {
//                        if (OrderD.IsCrispy)
//                        {
//                            listString = listString + "*" + "Crispy" + "*" + "\r\n";

//                        }
//                        else if (OrderD.IsDeserts)
//                        {
//                            listString = listString + "*" + "Desserts" + "*" + "\r\n";

//                        }
//                        else if (OrderD.IsCondiments)
//                        {
//                            listString = listString + "*" + "Condiments" + "*" + "\r\n";
//                        }
                       
//                    }

//                    //if (getOrderDetailExtra.Count > 0)
//                    //{
//                    //    listString = listString + "*" + captionAddtionText.Trim() + "*" + "\r\n";

//                    //}

//                }


//                foreach (var ext in getOrderDetailExtra)
//                {
//                    if (ext.Quantity > 1)
//                    {
//                        if (lang == "ar")
//                        {
//                            listString = listString + ext.Name + "  (" + ext.Quantity + ")" + "\r\n";
//                        }
//                        else
//                        {
//                            listString = listString + ext.NameEnglish + "  (" + ext.Quantity + ")" + "\r\n";

//                        }

//                    }
//                    else
//                    {
//                        if (lang == "ar")
//                        {

//                            listString = listString + ext.Name + "\r\n";
//                        }
//                        else
//                        {
//                            listString = listString + ext.NameEnglish + "\r\n";
//                        }


//                    }

//                    //listString = listString + captionQuantityText + ext.Quantity + "\r\n";

//                }

//                listString = listString + "\r\n" + "*" + captionQuantityText + OrderD.Quantity + "*" + "\r\n";
//                if (TenantID==46)
//                {
//                    listString = listString + captionTotalOfAllText + ((int)OrderD.Total).ToString() + "\r\n\r\n";
//                }
//                else
//                {
//                    listString = listString + captionTotalOfAllText + OrderD.Total + "\r\n\r\n";
//                }

               







//                total = total + OrderD.Total;

//                listString = listString + "-------------------------- \r\n";
//            }

//            listString = listString + "-------------------------- \r\n\r\n";
//            if (TenantID==46)
//            {
//                listString = listString + captionTotalText + ((int)Total).ToString() + "\r\n";
//            }
//            else
//            {
//                listString = listString + captionTotalText + Total.ToString() + "\r\n";
//            }
           
//            listString = listString + "-------------------------- \r\n";
//            if (isdiscount)
//            {
//                if (lang == "ar")
//                {

//                    listString = listString + "بعد خصم :" + discount + " %" + "\r\n\r\n";
//                }
//                else
//                {
//                    listString = listString + "After Discount :" + discount + " %" + "\r\n\r\n";
//                }


//            }

//            return listString;
//        }

//        private List<OrderOffers.OrderOffer> GetOrderOffer(int TenantID)
//        {

//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[OrderOffer] where TenantID=" + TenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<OrderOffers.OrderOffer> order = new List<OrderOffers.OrderOffer>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
//                if (!IsDeleted)
//                {

//                    order.Add(new OrderOffers.OrderOffer
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
//                        Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
//                        FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
//                        FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
//                        NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
//                        TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
//                        isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
//                        OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
//                        OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
//                        OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),
//                        OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString()),
//                        isPersentageDiscount = bool.Parse(dataSet.Tables[0].Rows[i]["isPersentageDiscount"].ToString()),




//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }
//        private void OrderOfferFun(int tenantID, bool isOrderOffer, decimal OrderTotal, GetLocationInfoModel infoLocation, string ci, string ar, string dis, decimal costDistric)
//        {
//            if (isOrderOffer)
//            {

//                var orderEffor = GetOrderOffer(tenantID);

//                var item = orderEffor.Where(x => (x.Area.Contains(ci) || x.Area.Contains(ar) || x.Area.Contains(dis) && x.isPersentageDiscount == false));//.FirstOrDefault();


//                foreach (var areaEffor in item)
//                {


//                    if (areaEffor != null)
//                    {

//                        if (!areaEffor.isPersentageDiscount)
//                        {

//                            if (OrderTotal >= areaEffor.FeesStart && OrderTotal <= areaEffor.FeesEnd)
//                            {
//                                var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
//                                var DateStart = Convert.ToDateTime(areaEffor.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
//                                var DateEnd = Convert.ToDateTime(areaEffor.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

//                                if (DateStart <= DateNow && DateEnd >= DateNow)
//                                {
//                                    var timeNow = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
//                                    var timeStart = Convert.ToDateTime(areaEffor.OrderOfferStart.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));
//                                    var timeEnd = Convert.ToDateTime(areaEffor.OrderOfferEnd.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture));

//                                    if ((timeStart <= timeNow && timeNow <= timeEnd))
//                                    {
//                                        infoLocation.DeliveryCostAfter = costDistric;
//                                        infoLocation.DeliveryCostBefor = areaEffor.NewFees;
//                                        infoLocation.isOrderOfferCost = true;

//                                        break;

//                                    }
//                                    else
//                                    {
//                                        infoLocation.DeliveryCostAfter = costDistric;
//                                        infoLocation.DeliveryCostBefor = 0;
//                                        infoLocation.isOrderOfferCost = false;

//                                    }


//                                }
//                                else
//                                {
//                                    infoLocation.DeliveryCostAfter = costDistric;
//                                    infoLocation.DeliveryCostBefor = 0;
//                                    infoLocation.isOrderOfferCost = false;

//                                }


//                            }
//                            else
//                            {
//                                infoLocation.DeliveryCostAfter = costDistric;
//                                infoLocation.DeliveryCostBefor = 0;
//                                infoLocation.isOrderOfferCost = false;

//                            }



//                        }
//                        else
//                        {
//                            infoLocation.DeliveryCostAfter = costDistric;
//                            infoLocation.DeliveryCostBefor = 0;
//                            infoLocation.isOrderOfferCost = false;

//                        }



//                    }
//                    else
//                    {
//                        infoLocation.DeliveryCostAfter = costDistric;
//                        infoLocation.DeliveryCostBefor = 0;
//                        infoLocation.isOrderOfferCost = false;

//                    }


//                }


//            }
//            else
//            {
//                infoLocation.DeliveryCostAfter = costDistric;
//                infoLocation.DeliveryCostBefor = 0;
//                infoLocation.isOrderOfferCost = false;

//            }

//        }
//        private void DeleteOrder(long? id)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {

//                command.CommandText = "DELETE FROM Orders   Where Id = @Id";
//                command.Parameters.AddWithValue("@Id", id);
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }

//        }
//        private void DeleteOrderDetails(long? id)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {

//                command.CommandText = "DELETE FROM OrderDetails   Where Id = @Id";
//                command.Parameters.AddWithValue("@Id", id);
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }

//        }
//        private void DeleteExtraOrderDetail(long? id)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {

//                command.CommandText = "DELETE FROM ExtraOrderDetail   Where Id = @Id";
//                command.Parameters.AddWithValue("@Id", id);
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }

//        }


       

//        private locationAddressModel GetLocation(string query)
//        {
//            try
//            {
//                var client = new HttpClient();
//                string Key = _configuration["GoogleMapsKey:KeyMap"];
//                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key=" + Key;
//                var response = client.GetAsync(url).Result;

//                var result = response.Content.ReadAsStringAsync().Result;
//                var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleMapModel>(result);



//                ////
//                locationAddressModel locationAddressModel = new locationAddressModel();
//                var rez = resultO;

//                foreach (var item in rez.results)
//                {

//                    //route
//                    var route = item.types.Where(x => x == "street_address").FirstOrDefault();
//                    if (route != null)
//                    {

//                        locationAddressModel.Route = item.formatted_address.Split(",")[0];
//                        //break;
//                    }
//                    else
//                    {
//                        route = item.types.Where(x => x == "route").FirstOrDefault();
//                        if (route != null)
//                        {

//                            locationAddressModel.Route = item.formatted_address.Split(",")[0];

//                        }
//                        else
//                        {
//                            //locationAddressModel.Route = "";
//                        }
//                    }

//                    //Distric
//                    var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
//                    if (neighborhood != null)
//                    {


//                        locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;

//                    }
//                    else
//                    {
//                        neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
//                        if (neighborhood != null && locationAddressModel.Distric == null)
//                        {
//                            locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;


//                        }
//                        else
//                        {
//                            //locationAddressModel.Distric = "";

//                        }


//                    }


//                    //Area
//                    var sublocality_level_1 = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
//                    if (sublocality_level_1 != null)
//                    {

//                        locationAddressModel.Area = item.address_components.FirstOrDefault().long_name;

//                    }
//                    else
//                    {
//                        //locationAddressModel.Area = "";
//                        ///

//                    }


//                    //city
//                    var locality = item.types.Where(x => x == "locality").FirstOrDefault();
//                    if (locality != null)
//                    {

//                        locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];

//                    }
//                    else
//                    {


//                        locality = item.types.Where(x => x == "administrative_area_level_1").FirstOrDefault();
//                        if (locality != null)
//                        {
//                            if (item.address_components.FirstOrDefault().long_name.Split(",")[0] == "Jerash Governorate")
//                            {
//                                locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
//                            }


//                        }
//                        //locationAddressModel.City = "";
//                    }


//                    //Country
//                    var country = item.types.Where(x => x == "country").FirstOrDefault();
//                    if (country != null)
//                    {

//                        locationAddressModel.Country = item.address_components.FirstOrDefault().long_name;
//                        //break;
//                    }


//                }

//                if (locationAddressModel.Route == null)
//                    locationAddressModel.Route = "";
//                if (locationAddressModel.Distric == null)
//                    locationAddressModel.Distric = "";
//                if (locationAddressModel.Area == null)
//                    locationAddressModel.Area = "";
//                if (locationAddressModel.City == null)
//                    locationAddressModel.City = "";
//                if (locationAddressModel.Country == null)
//                    locationAddressModel.Country = "";


//                return locationAddressModel;

//            }
//            catch(Exception ex)
//            {
//                locationAddressModel locationAddressModel = new locationAddressModel();

//                if (locationAddressModel.Route == null)
//                    locationAddressModel.Route = "";
//                if (locationAddressModel.Distric == null)
//                    locationAddressModel.Distric = "";
//                if (locationAddressModel.Area == null)
//                    locationAddressModel.Area = "";
//                if (locationAddressModel.City == null)
//                    locationAddressModel.City = "";
//                if (locationAddressModel.Country == null)
//                    locationAddressModel.Country = "";


//                return locationAddressModel;
//            }



//            //   return resultO;
//        }
//        private List<LocationInfoModel> GetAllLocationInfoModel()
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[Locations] ";


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                List<LocationInfoModel> locationInfoModel = new List<LocationInfoModel>();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {
//                    try
//                    {
//                        locationInfoModel.Add(new LocationInfoModel
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
//                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
//                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
//                            ParentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ParentId"].ToString()),
//                        });
//                    }
//                    catch
//                    {
//                        locationInfoModel.Add(new LocationInfoModel
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
//                            LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
//                            LevelId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"].ToString()),
//                        });

//                    }


//                }

//                conn.Close();
//                da.Dispose();

//                return locationInfoModel;

//            }
//            catch
//            {
//                return null;

//            }

//        }
//        private List<LocationInfoModel> GetAllLocationDeliveryCost(int TenantId)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[LocationDeliveryCost]  where TenantId=" + TenantId;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                List<LocationInfoModel> locationInfoModel = new List<LocationInfoModel>();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    locationInfoModel.Add(new LocationInfoModel
//                    {
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        LocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LocationId"]),
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
//                        BranchAreaId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"]),

//                    });
//                }

//                conn.Close();
//                da.Dispose();

//                return locationInfoModel;

//            }
//            catch
//            {
//                return null;

//            }

//        }
//        private List<Area> GetAreasList(string TenantID)
//        {
//            //TenantID = "31";
//            var tenantID = int.Parse(TenantID);
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Areas] where TenantID=" + tenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<Area> branches = new List<Area>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                var IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString());

//                if (IsAvailableBranch)
//                {
//                    branches.Add(new Area
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString() ?? "",
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString() ?? "",
//                        AreaCoordinateEnglish = dataSet.Tables[0].Rows[i]["AreaCoordinateEnglish"].ToString() ?? "",
//                        AreaNameEnglish = dataSet.Tables[0].Rows[i]["AreaNameEnglish"].ToString() ?? "",
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                        IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                        UserIds = (dataSet.Tables[0].Rows[i]["UserIds"].ToString()),


//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }
//        private long UpateOrder(int? tenantId)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;

//            //AbpSession.TenantId
//            var sqlParameters = new List<SqlParameter> {
//                            new SqlParameter("@TenantId",tenantId)
//                     };

//            SqlParameter output = new SqlParameter();

//            output.ParameterName = "@CurrentOrderNumber";
//            output.DbType = DbType.Int64;
//            output.Direction = ParameterDirection.Output;
//            sqlParameters.Add(output);



//            var result = SqlDataHelper.ExecuteNoneQuery(
//                       "dbo.GetCurrentOrderNumber",
//                       sqlParameters.ToArray(),
//                       connString);

//            return Int64.Parse(output.Value.ToString());
//        }
//        private int UpateBooking(int? tenantId)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;

//            //AbpSession.TenantId
//            var sqlParameters = new List<SqlParameter> {
//                            new SqlParameter("@TenantId",tenantId)
//                     };

//            SqlParameter output = new SqlParameter();

//            output.ParameterName = "@CurrentBookingNumber";
//            output.DbType = DbType.Int64;
//            output.Direction = ParameterDirection.Output;
//            sqlParameters.Add(output);



//            var result = SqlDataHelper.ExecuteNoneQuery(
//                       "dbo.GetCurrentBookingNumber",
//                       sqlParameters.ToArray(),
//                       connString);

//            return int.Parse(output.Value.ToString());
//        }
//        private string GetHtmlD(UpdateOrderModel orderBotData, DateTime timeAdd, long number, Contact con, string BranchAreaName, string local)
//        {
//            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.OrderId, local).Result;
//            var header = "";
//            if (orderBotData.IsPreOrder)
//            {
//                header = "<div > <strong  style='font-size: large;'> Name : </strong>" + con.DisplayName + "</div>"
//             + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
//             + "<div > <strong style='font-size: large;' > Address : </strong>" + orderBotData.Address + "</div>"
//             + "<div > <strong  style='font-size: large;' > Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
//             + "<div >  <strong  style='font-size: large;' > Type : </strong>" + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Delivery) + "/  " + orderBotData.SelectTime + "-" + orderBotData.SelectDay + "</div>"
//             + "<div > <strong style='font-size: large;'> Branch : </strong>" + orderBotData.BranchName + "/" + BranchAreaName + "</div>"
//             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
//             + "<hr  style='border-top: dotted 1px #000 !important;'>";

//            }
//            else
//            {
//                header = "<div > <strong  style='font-size: large;'> Name : </strong>" + con.DisplayName + "</div>"
//             + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
//             + "<div > <strong style='font-size: large;' > Address : </strong>" + orderBotData.Address + "</div>"
//             + "<div > <strong  style='font-size: large;' > Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
//             + "<div > <strong style='font-size: large;'> Branch : </strong>" + orderBotData.BranchName + "/" + BranchAreaName + "</div>"
//             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
//             + "<hr  style='border-top: dotted 1px #000 !important;'>";
//            }


//            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";


//            foreach (var record in orderDetailsList)
//            {

//                if (local == "ar")
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName + "</strong></td>";
//                }
//                else
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemNameEnglish + "</strong></td>";
//                }


//                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Quantity + "</strong></td>";
//                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Total + "</strong></td> </tr>";

//                var extraOrderDetails = "";
//                foreach (var group in record.OrderDetail.extraOrderDetailsDtos)
//                {
//                    extraOrderDetails = extraOrderDetails + "<tr ><td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Name + "</td>";
//                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Quantity + "</td>";
//                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller;'> " + group.Total + "</td> </tr>";

//                }


//                orderDetailsbody = orderDetailsbody + extraOrderDetails;




//            }
//            orderDetailsbody = orderDetailsbody + "</tbody></table> </div> <hr  style='border-top: dotted 1px #000 !important;'>  <hr  style='border-top: dotted 1px #000 !important;'>";

//            // var xxx = (Math.Round(orderBotData.AfterBranchCost * 100) / 100).toFixed(2);

//            orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> Delivery fees   : </strong> <strong style='font-size: medium;'>" + orderBotData.DeliveryCostAfter + " </strong> </div>";

//            if (orderBotData.isOrderOfferCost)
//            {
//                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> instead of   : </strong> <strong style='font-size: medium;'>" + orderBotData.DeliveryCostBefor + " </strong> </div>";
//            }


//            if (orderBotData.IsSpecialRequest)
//            {
//                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium;'  > <strong style='font-size: x-large;'> Note   : </strong> <strong style='font-size: medium;'>" + orderBotData.SpecialRequest + " </strong> </div>";
//            }

//            if (orderBotData.isOrderOfferCost)
//            {
//                orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + ((orderBotData.OrderTotal + orderBotData.DeliveryCostBefor)) + " </p> </div>";
//            }
//            else
//            {
//                orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + ((orderBotData.OrderTotal + orderBotData.DeliveryCostAfter)) + " </p> </div>";
//            }


//            var htmlOrderD = header + orderDetailsbody;
//            return htmlOrderD;
//        }
//        private string GetHtmlPrint(UpdateOrderModel orderBotData, DateTime timeAdd, long number, Contact con, Area area, string local)
//        {
//            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.OrderId, local).Result;

//            var header = "<div > <strong  style='font-size:large;'> Name : </strong>" + con.DisplayName + "</div>"
//               + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
//               + "<div > <strong  style='font-size: large;'> Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
//               + "<div >  <strong  style='font-size: large;'> Type : </strong>" + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Takeaway) + "</div>"
//               + "<div > <strong  style='font-size: large;'> Branch : </strong>" + area.AreaName + "</div>"
//               + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
//               + "<hr  style='border-top: dotted 1px #000 !important;'><hr  style='border-top: dotted 1px #000 !important;'>";

//            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";



//            foreach (var record in orderDetailsList)
//            {
//                if (local == "ar")
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName + "</strong></td>";
//                }
//                else
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemNameEnglish + "</strong></td>";
//                }



//                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Quantity + "</strong></td>";
//                orderDetailsbody = orderDetailsbody + "<td style='text-align: center; width: 100px; '> <strong>" + record.OrderDetail.Total + "</strong></td> </tr>";

//                var extraOrderDetails = "";
//                foreach (var group in record.OrderDetail.extraOrderDetailsDtos)
//                {
//                    extraOrderDetails = extraOrderDetails + "<tr ><td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Name + "</td>";
//                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Quantity + "</td>";
//                    extraOrderDetails = extraOrderDetails + "<td style='text-align: center; width: 100px; font-size: smaller;'> " + group.Total + "</td> </tr>";

//                }


//                orderDetailsbody = orderDetailsbody + extraOrderDetails;




//            }
//            orderDetailsbody = orderDetailsbody + "</tbody></table> </div> <hr  style='border-top: dotted 1px #000 !important;'>  <hr  style='border-top: dotted 1px #000 !important;'>";

//            if (orderBotData.SpecialRequest != "NULLNOT" || orderBotData.SpecialRequest != "NULLORDER")
//            {
//                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium;'  > <strong style='font-size: x-large;'> Note   : </strong> <strong style='font-size: medium;'>" + orderBotData.SpecialRequest + " </strong> </div>";
//            }
//            orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + orderBotData.OrderTotal + " </p> </div>";

//            var htmlOrder = header + orderDetailsbody;
//            return htmlOrder;
//        }
//        private Contact GetContact(int id)
//        {

//            try
//            {


//                //ContactDto objContact = _iContactsAppService.GetContactbyId(id);

//                Contact contact = new Contact();

//                //contact = new Contact
//                //{
//                //    Id = objContact.Id,
//                //    UserId = objContact.UserId,
//                //    PhoneNumber = objContact.PhoneNumber,

//                //    Description = objContact.Description,
//                //    EmailAddress = objContact.EmailAddress,
//                //    Website = objContact.Website,

//                //    DisplayName = objContact.DisplayName,
//                //    TotalOrder = objContact.TotalOrder,
//                //    TakeAwayOrder = objContact.TakeAwayOrder,
//                //    DeliveryOrder = objContact.DeliveryOrder,
//                //    loyalityPoint = objContact.loyalityPoint,
//                //    StreetName = objContact.StreetName,
//                //    BuildingNumber = objContact.BuildingNumber,
//                //    FloorNo = objContact.FloorNo,
//                //    ApartmentNumber = objContact.ApartmentNumber,
//                //    ContactDisplayName = objContact.ContactDisplayName


//                //};



//                //return contact; 

//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[Contacts] where Id=" + id;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);


//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    try
//                    {
//                        contact = new Contact
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
//                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

//                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
//                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
//                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

//                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
//                            ContactDisplayName = dataSet.Tables[0].Rows[i]["ContactDisplayName"].ToString(),
//                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
//                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
//                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
//                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
//                            StreetName = dataSet.Tables[0].Rows[i]["StreetName"].ToString(),
//                            BuildingNumber = dataSet.Tables[0].Rows[i]["BuildingNumber"].ToString(),
//                            FloorNo = dataSet.Tables[0].Rows[i]["FloorNo"].ToString(),
//                            ApartmentNumber = dataSet.Tables[0].Rows[i]["ApartmentNumber"].ToString(),
//                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ?Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]):0,

//                        };
//                    }
//                    catch
//                    {
//                        contact = new Contact
//                        {
//                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                            UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
//                            PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

//                            Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
//                            EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
//                            Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

//                            DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
//                            TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
//                            TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
//                            DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
//                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"]),
//                            TenantId = dataSet.Tables[0].Rows[i]["TenantId"] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]) : 0,
//                        };
//                    }

//                }
                
//                conn.Close();
//                da.Dispose();

//                return contact;

//            }
//            catch 
//            {
//                return null;

//            }

//        }
//        private Area GetAreaId(int id)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Areas] where Id=" + id;

//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            Area branches = new Area();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                branches = new Area
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                    AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                    BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                    UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                    RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                    IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                    IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),
//                    UserIds = dataSet.Tables[0].Rows[i]["UserIds"].ToString(),
//                     IsRestaurantsTypeAll = bool.Parse(dataSet.Tables[0].Rows[i]["IsRestaurantsTypeAll"].ToString()),
//                };
//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }
//        private List<Contact> GetContactWithUserID(int teantID)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[Contacts] where TenantId=" + teantID;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                List<Contact> contact = new List<Contact>();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    contact.Add(new Contact
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
//                        PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

//                        Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
//                        EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
//                        Website = dataSet.Tables[0].Rows[i]["Website"].ToString(),

//                        DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
//                        TotalOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
//                        TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
//                        DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
//                        loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"])

//                    });
//                }

//                conn.Close();
//                da.Dispose();

//                return contact;

//            }
//            catch (Exception ex)
//            {
//                return null;

//            }

//        }
//        private async Task<UserNotification> SendNotfAsync(string message, long agentID, int? TenantID)
//        {
//            var userIdentifier = ToUserIdentifier(TenantID, agentID);

//            await _appNotifier.SendMessageAsync(userIdentifier, message);

//            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
//              userIdentifier);

//            return null;
//        }
//        private UserIdentifier ToUserIdentifier(int? TargetTenantId, long TargetUserId)
//        {
//            return new UserIdentifier(TargetTenantId, TargetUserId);
//        }
//        private List<Bookings.Booking> GetBookingList(int TenantID)
//        {
//            //TenantID = "31";
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Booking] where TenantID=" + TenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<Bookings.Booking> bookings = new List<Bookings.Booking>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {
//                BookingTypeEunm bookingTypeEunm;

//                Enum.TryParse(dataSet.Tables[0].Rows[i]["bookingTypeEunm"].ToString(), out bookingTypeEunm);
//                bookings.Add(new Bookings.Booking
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                    BookTableDate = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BookTableDate"].ToString()),
//                    bookingTypeEunm = bookingTypeEunm,
//                    TableNumber = dataSet.Tables[0].Rows[i]["TableNumber"].ToString(),
//                    IsBreakfast = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsBreakfast"].ToString()),
//                    IsDinner = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDinner"].ToString()),
//                    IsLunch = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsLunch"].ToString()),
//                    UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString()

//                });



//            }

//            conn.Close();
//            da.Dispose();

//            return bookings;
//        }
//        private void DeleteBooking(int id)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {

//                command.CommandText = "DELETE FROM Booking   Where Id = @Id";
//                command.Parameters.AddWithValue("@Id", id);
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }

//        }
//        private void updateBreakfast(Bookings.Booking booking)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "UPDATE Booking SET "
//                    + "IsBreakfast = @IsBreakfast ,"
//                    + "BreakfastUserId = @BreakfastUserId ,"
//                    + "BreakfastUserName = @BreakfastUserName ,"
//                    + "BreakfastUserPhone = @BreakfastUserPhone ,"
//                     + "NumberPersonsBreakfastString = @NumberPersonsBreakfastString "
//                    + " Where Id = @Id";

//                command.Parameters.AddWithValue("@Id", booking.Id);
//                command.Parameters.AddWithValue("@IsBreakfast", booking.IsBreakfast);
//                command.Parameters.AddWithValue("@BreakfastUserId", booking.BreakfastUserId);
//                command.Parameters.AddWithValue("@BreakfastUserName", booking.BreakfastUserName);
//                command.Parameters.AddWithValue("@BreakfastUserPhone", booking.BreakfastUserPhone);
//                command.Parameters.AddWithValue("@NumberPersonsBreakfastString", booking.NumberPersonsBreakfastString);

//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//        }
//        private void updateLunch(Bookings.Booking booking)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "UPDATE Booking SET "
//                    + "IsLunch = @IsLunch ,"
//                    + "LunchUserId = @LunchUserId ,"
//                    + "LunchUserName = @LunchUserName ,"
//                    + "LunchUserPhone = @LunchUserPhone ,"
//                     + "NumberPersonsLunchString = @NumberPersonsLunchString "
//                    + " Where Id = @Id";

//                command.Parameters.AddWithValue("@Id", booking.Id);
//                command.Parameters.AddWithValue("@IsLunch", booking.IsLunch);
//                command.Parameters.AddWithValue("@LunchUserId", booking.LunchUserId);
//                command.Parameters.AddWithValue("@LunchUserName", booking.LunchUserName);
//                command.Parameters.AddWithValue("@LunchUserPhone", booking.LunchUserPhone);
//                command.Parameters.AddWithValue("@NumberPersonsLunchString", booking.NumberPersonsLunchString);

//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//        }
//        private void updateDinner(Bookings.Booking booking)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "UPDATE Booking SET "
//                    + "IsDinner = @IsDinner ,"
//                    + "DinnerUserId = @DinnerUserId ,"
//                    + "DinnerUserName = @DinnerUserName ,"
//                    + "DinnerUserPhone = @DinnerUserPhone ,"
//                     + "NumberPersonsDinnerString = @NumberPersonsDinnerString "
//                    + " Where Id = @Id";

//                command.Parameters.AddWithValue("@Id", booking.Id);
//                command.Parameters.AddWithValue("@IsDinner", booking.IsDinner);
//                command.Parameters.AddWithValue("@DinnerUserId", booking.DinnerUserId);
//                command.Parameters.AddWithValue("@DinnerUserName", booking.DinnerUserName);
//                command.Parameters.AddWithValue("@DinnerUserPhone", booking.DinnerUserPhone);
//                command.Parameters.AddWithValue("@NumberPersonsDinnerString", booking.NumberPersonsDinnerString);

//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//        }
//        private void CreateBooking(Bookings.Booking booking)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "INSERT INTO Booking "
//                            + " (UserPhone ,UserName ,UserId ,IsBreakfast , BreakfastUserId, BreakfastUserName, BreakfastUserPhone, NumberPersonsBreakfast, NumberPersonsBreakfastString, IsLunch, LunchUserId, LunchUserName, LunchUserPhone, NumberPersonsLunch, NumberPersonsLunchString, IsDinner, DinnerUserId, DinnerUserName, DinnerUserPhone, NumberPersonsDinner, NumberPersonsDinnerString, TableNumber, BookTableDate, bookingTypeEunm, TenantId) VALUES "
//                            + " (@UserPhone ,@UserName ,@UserId ,@IsBreakfast ,@BreakfastUserId, @BreakfastUserName, @BreakfastUserPhone, @NumberPersonsBreakfast, @NumberPersonsBreakfastString, @IsLunch, @LunchUserId, @LunchUserName, @LunchUserPhone, @NumberPersonsLunch, @NumberPersonsLunchString, @IsDinner, @DinnerUserId, @DinnerUserName, @DinnerUserPhone, @NumberPersonsDinner, @NumberPersonsDinnerString, @TableNumber, @BookTableDate, @bookingTypeEunm, @TenantId) ";

//                        command.Parameters.AddWithValue("@IsBreakfast", booking.IsBreakfast);
//                        command.Parameters.AddWithValue("@BreakfastUserId", booking.BreakfastUserId);
//                        command.Parameters.AddWithValue("@BreakfastUserName", booking.BreakfastUserName);
//                        command.Parameters.AddWithValue("@BreakfastUserPhone", booking.BreakfastUserPhone);
//                        command.Parameters.AddWithValue("@NumberPersonsBreakfast", booking.NumberPersonsBreakfast);
//                        command.Parameters.AddWithValue("@NumberPersonsBreakfastString", booking.NumberPersonsBreakfastString);

//                        command.Parameters.AddWithValue("@IsLunch", booking.IsLunch);
//                        command.Parameters.AddWithValue("@LunchUserId", booking.LunchUserId);
//                        command.Parameters.AddWithValue("@LunchUserName", booking.LunchUserName);
//                        command.Parameters.AddWithValue("@LunchUserPhone", booking.LunchUserPhone);
//                        command.Parameters.AddWithValue("@NumberPersonsLunch", booking.NumberPersonsLunch);
//                        command.Parameters.AddWithValue("@NumberPersonsLunchString", booking.NumberPersonsLunchString);

//                        command.Parameters.AddWithValue("@IsDinner", booking.IsDinner);
//                        command.Parameters.AddWithValue("@DinnerUserId", booking.DinnerUserId);
//                        command.Parameters.AddWithValue("@DinnerUserName", booking.DinnerUserName);
//                        command.Parameters.AddWithValue("@DinnerUserPhone", booking.DinnerUserPhone);
//                        command.Parameters.AddWithValue("@NumberPersonsDinner", booking.NumberPersonsDinner);
//                        command.Parameters.AddWithValue("@NumberPersonsDinnerString", booking.NumberPersonsDinnerString);


//                        command.Parameters.AddWithValue("@TableNumber", booking.TableNumber);
//                        command.Parameters.AddWithValue("@BookTableDate", booking.BookTableDate);
//                        command.Parameters.AddWithValue("@bookingTypeEunm", booking.bookingTypeEunm);
//                        command.Parameters.AddWithValue("@TenantId", booking.TenantId);
//                        command.Parameters.AddWithValue("@UserId", booking.UserId);
//                        command.Parameters.AddWithValue("@UserName", booking.UserName);
//                        command.Parameters.AddWithValue("@UserPhone", booking.UserPhone);



//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception e)
//                {


//                }

//        }
//        private List<Models.Location.DeliveryLocationCost> GetAllDeliveryLocationCost(int TenantId)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[DeliveryLocationCost] where TenantId = " + TenantId;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                List<Models.Location.DeliveryLocationCost> locationInfoModel = new List<Models.Location.DeliveryLocationCost>();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    locationInfoModel.Add(new Models.Location.DeliveryLocationCost
//                    {
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        FromLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FromLocationId"]),
//                        ToLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ToLocationId"]),
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
//                        BranchAreaId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BranchAreaId"]),

//                    });
//                }

//                conn.Close();
//                da.Dispose();

//                return locationInfoModel;

//            }
//            catch
//            {
//                return null;

//            }

//        }
//        private TenantEditDto GetTenantByID(int TenantID)
//        {
//            //TenantID = "31";
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[AbpTenants] where id=" + TenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            TenantEditDto tenants = new TenantEditDto();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {


//                tenants = new TenantEditDto
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    InsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InsideNumber"]),
//                    OutsideNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OutsideNumber"]),


//                };



//            }

//            conn.Close();
//            da.Dispose();

//            return tenants;
//        }
//        private static async Task<string> TranslatorFun(string text, string lan)
//        {
//            string url = "https://infoseedtranslator.cognitiveservices.azure.com/translator/text/v3.0/translate?to=" + lan;


//            using (var httpClient = new HttpClient())
//            {
//                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
//                {
//                    request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", AppSettingsModel.TranslateKey);
//                    //request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Region", "switzerlandnorth");

//                    request.Content = new StringContent("[{'Text':'" + text + "'}]");
//                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

//                    var response = await httpClient.SendAsync(request);
//                    string Jso = response.Content.ReadAsStringAsync().Result;
//                    var results = Jso.Substring(1, Jso.Length - 2);
//                    var rez = JsonConvert.DeserializeObject<TranslatorModel>(results);
//                    return rez.translations[0].text;
//                }
//            }


//        }
//        private string TransFun(string word)
//        {
//            var toLanguage = "ar";//English
//            var fromLanguage = "en";//Deutsch
//            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
//            var webClient = new WebClient
//            {
//                Encoding = System.Text.Encoding.UTF8
//            };
//            var result = webClient.DownloadString(url);
//            try
//            {
//                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
//                return result;
//            }
//            catch
//            {
//                return "Error";
//            }
//        }
//        private GetMaintenancesForViewDto CreateMaintenance(Models.BotModel.CreateMaintenanceModel create)
//        {
//            var timeAdd = DateTime.Now.AddHours(AppSettingsModel.AddHour);

//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "INSERT INTO Maintenance "
//                            + " (TenantId ,CreationTime ,ContactId ,OrderStatus , Address, DeliveryCost, OrderLocal, CustomerName, PhoneNumber1, PhoneNumber2, PhoneType, Damage, IsLockByAgent, LockByAgentName, AgentId, phoneNumber, DisplayName, UserId, FromLongitudeLatitude) VALUES "
//                            + " (@TenantId ,@CreationTime ,@ContactId ,@OrderStatus ,@Address, @DeliveryCost, @OrderLocal, @CustomerName, @PhoneNumber1, @PhoneNumber2, @PhoneType, @Damage, @IsLockByAgent, @LockByAgentName, @AgentId, @phoneNumber, @DisplayName, @UserId, @FromLongitudeLatitude) ";




//                        command.Parameters.AddWithValue("@TenantId", create.TenantID);
//                        command.Parameters.AddWithValue("@CreationTime", timeAdd);
//                        command.Parameters.AddWithValue("@ContactId", create.ContactId);
//                        command.Parameters.AddWithValue("@OrderStatus", OrderStatusEunm.Pending);
//                        command.Parameters.AddWithValue("@Address", create.Address);
//                        command.Parameters.AddWithValue("@DeliveryCost", create.DeliveryCostAfter);

//                        //  command.Parameters.AddWithValue("@FromLongitudeLatitude", create.FromLongitudeLatitude);
//                        command.Parameters.AddWithValue("@OrderLocal", create.BotLocal);
//                        command.Parameters.AddWithValue("@CustomerName", create.CustomerName);
//                        command.Parameters.AddWithValue("@PhoneNumber1", create.PhoneNumber1);
//                        command.Parameters.AddWithValue("@PhoneNumber2", create.PhoneNumber2);
//                        command.Parameters.AddWithValue("@PhoneType", create.PhoneType);

//                        command.Parameters.AddWithValue("@Damage", create.Damage);

//                        command.Parameters.AddWithValue("@isLockByAgent", false);
//                        command.Parameters.AddWithValue("@LockByAgentName", "");
//                        command.Parameters.AddWithValue("@AgentId", -1);

//                        command.Parameters.AddWithValue("@phoneNumber", create.phoneNumber);
//                        command.Parameters.AddWithValue("@DisplayName", create.DisplayName);
//                        command.Parameters.AddWithValue("@UserId", create.UserId);

//                        command.Parameters.AddWithValue("@FromLongitudeLatitude", "https://maps.google.com/?q=" + create.LocationFrom);




//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }

//                    var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), OrderStatusEunm.Pending);

//                    GetMaintenancesForViewDto getMaintenancesForViewDto = new GetMaintenancesForViewDto();

//                    getMaintenancesForViewDto.TenantID = create.TenantID;
//                    getMaintenancesForViewDto.CreationTime = timeAdd;
//                    getMaintenancesForViewDto.ContactId = create.ContactId;
//                    getMaintenancesForViewDto.OrderStatus = 1;
//                    getMaintenancesForViewDto.Address = create.Address;
//                    getMaintenancesForViewDto.DeliveryCost = create.DeliveryCostAfter;
//                    getMaintenancesForViewDto.OrderLocal = create.BotLocal;
//                    getMaintenancesForViewDto.CustomerName = create.CustomerName;
//                    getMaintenancesForViewDto.PhoneNumber1 = create.PhoneNumber1;
//                    getMaintenancesForViewDto.PhoneNumber2 = create.PhoneNumber2;
//                    getMaintenancesForViewDto.PhoneType = create.PhoneType;
//                    getMaintenancesForViewDto.Damage = create.Damage;
//                    getMaintenancesForViewDto.isLockByAgent = false;
//                    getMaintenancesForViewDto.LockByAgentName = "";
//                    getMaintenancesForViewDto.AgentId = -1;
//                    getMaintenancesForViewDto.phoneNumber = create.phoneNumber;
//                    getMaintenancesForViewDto.DisplayName = create.DisplayName;
//                    getMaintenancesForViewDto.UserId = create.UserId;
//                    getMaintenancesForViewDto.LocationFrom = "https://maps.google.com/?q=" + create.LocationFrom;

//                    getMaintenancesForViewDto.orderStatusName = orderStatusName;



//                    return getMaintenancesForViewDto;

//                }
//                catch (Exception e)
//                {

//                    GetMaintenancesForViewDto getMaintenancesForViewDto = new GetMaintenancesForViewDto();
//                    return getMaintenancesForViewDto;
//                }

//        }

//        private AreaDto getNearbyArea(int tenantID, double eLatitude, double eLongitude, string city, long areaId, out double distance)
//        {


//             distance = -1;

//            bool isInAmman = !string.IsNullOrEmpty(city) && city.Trim().ToLower().Equals("amman");
//            AreaDto areaDto = new AreaDto();
//            var areas = _iAreasAppService.GetAllAreas(tenantID, true);
//            List<AreaDto> lstAreaDto = new List<AreaDto>();

//            if (areas != null)
//            {
//                foreach (var item in areas)
//                {
//                    if (checkIsInService2(item.SettingJson))
//                    {
//                        lstAreaDto.Add(item);

//                    }
//                }
//            }

//            if (!isInAmman && tenantID == 42)// Macdonalds
//            {
//                if (lstAreaDto != null)
//                lstAreaDto = lstAreaDto.Where(x => x.Id == areaId).ToList();
//            }
//            //int
//            if (lstAreaDto != null)
//            {
//                foreach (var item in lstAreaDto)
//                {

//                    var sCoord = new GeoCoordinate(item.Latitude.Value, item.Longitude.Value);
//                    var eCoord = new GeoCoordinate(eLatitude, eLongitude);
//                    var currentDistance = sCoord.GetDistanceTo(eCoord);


//                    if ((distance == -1 && !isInAmman) || (isInAmman && currentDistance < 5000 && distance == -1))
//                    {
//                        areaDto = new AreaDto();
//                        areaDto.Id = item.Id;
//                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
//                        areaDto.AreaName = item.AreaName;
//                        areaDto.AreaCoordinate = item.AreaCoordinate;
//                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
//                        distance = currentDistance;

//                    }
//                    if ((distance > currentDistance && !isInAmman) || (isInAmman && currentDistance < 5000 && distance > currentDistance))
//                    {
//                        areaDto = new AreaDto();
//                        areaDto.Id = item.Id;
//                        areaDto.AreaNameEnglish = item.AreaNameEnglish;
//                        areaDto.AreaName = item.AreaName;
//                        areaDto.AreaCoordinate = item.AreaCoordinate;
//                        areaDto.AreaCoordinateEnglish = item.AreaCoordinateEnglish;
//                        distance = currentDistance;

//                    }

//                }
//            }

//            return areaDto;
//        }


//        private bool checkIsInService2(string workingHourSetting)
//        {
//            bool result = true;
//            if (!string.IsNullOrEmpty(workingHourSetting))
//            {
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(workingHourSetting, options);

//                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
//                DayOfWeek wk = currentDateTime.DayOfWeek;
//                TimeSpan timeOfDay = currentDateTime.TimeOfDay;

//                switch (wk)
//                {
//                    case DayOfWeek.Saturday:
//                        if (workModel.IsWorkActiveSat)
//                        {
//                            var StartDateSat = getValidValue(workModel.StartDateSat);
//                            var EndDateSat = getValidValue(workModel.EndDateSat);

//                            var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
//                            var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

//                            if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }

//                        break;
//                    case DayOfWeek.Sunday:
//                        if (workModel.IsWorkActiveSun)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateSun);
//                            var EndDate = getValidValue(workModel.EndDateSun);

//                            var StartDateSP = getValidValue(workModel.StartDateSunSP);
//                            var EndDateSP = getValidValue(workModel.EndDateSunSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Monday:

//                        if (workModel.IsWorkActiveMon)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateMon);
//                            var EndDate = getValidValue(workModel.EndDateMon);

//                            var StartDateSP = getValidValue(workModel.StartDateMonSP);
//                            var EndDateSP = getValidValue(workModel.EndDateMonSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }

//                        break;
//                    case DayOfWeek.Tuesday:
//                        if (workModel.IsWorkActiveTues)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateTues);
//                            var EndDate = getValidValue(workModel.EndDateTues);

//                            var StartDateSP = getValidValue(workModel.StartDateTuesSP);
//                            var EndDateSP = getValidValue(workModel.EndDateTuesSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Wednesday:
//                        if (workModel.IsWorkActiveWed)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateWed);
//                            var EndDate = getValidValue(workModel.EndDateWed);

//                            var StartDateSP = getValidValue(workModel.StartDateWedSP);
//                            var EndDateSP = getValidValue(workModel.EndDateWedSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Thursday:
//                        if (workModel.IsWorkActiveThurs)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateThurs);
//                            var EndDate = getValidValue(workModel.EndDateThurs);

//                            var StartDateSP = getValidValue(workModel.StartDateThursSP);
//                            var EndDateSP = getValidValue(workModel.EndDateThursSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Friday:
//                        if (workModel.IsWorkActiveFri)
//                        {
//                            var StartDate = getValidValue(workModel.StartDateFri);
//                            var EndDate = getValidValue(workModel.EndDateFri);

//                            var StartDateSP = getValidValue(workModel.StartDateFriSP);
//                            var EndDateSP = getValidValue(workModel.StartDateFriSP);

//                            if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                            {
//                                result = true;
//                            }
//                            else
//                            {

//                                result = false;
//                            }
//                        }
//                        break;
//                    default:

//                        break;

//                }


//            }
//            return result;
            
//        }

//        private bool checkIsInServiceLiveChat(Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel workModel, out string outWHMessage)
//        {

//            bool result = true;
//            outWHMessage = string.Empty;
//            DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
//            DayOfWeek wk = currentDateTime.DayOfWeek;
//            TimeSpan timeOfDay = currentDateTime.TimeOfDay;
//            var options = new JsonSerializerOptions { WriteIndented = true };

//            switch (wk)
//            {
//                case DayOfWeek.Saturday:
//                    if (workModel.IsWorkActiveSat)
//                    {
//                        var StartDateSat = getValidValue(workModel.StartDateSat);
//                        var EndDateSat = getValidValue(workModel.EndDateSat);

//                        var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
//                        var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

//                        if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }

//                    break;
//                case DayOfWeek.Sunday:
//                    if (workModel.IsWorkActiveSun)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateSun);
//                        var EndDate = getValidValue(workModel.EndDateSun);

//                        var StartDateSP = getValidValue(workModel.StartDateSunSP);
//                        var EndDateSP = getValidValue(workModel.EndDateSunSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextSun, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Monday:

//                    if (workModel.IsWorkActiveMon)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateMon);
//                        var EndDate = getValidValue(workModel.EndDateMon);

//                        var StartDateSP = getValidValue(workModel.StartDateMonSP);
//                        var EndDateSP = getValidValue(workModel.EndDateMonSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }

//                    break;
//                case DayOfWeek.Tuesday:
//                    if (workModel.IsWorkActiveTues)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateTues);
//                        var EndDate = getValidValue(workModel.EndDateTues);

//                        var StartDateSP = getValidValue(workModel.StartDateTuesSP);
//                        var EndDateSP = getValidValue(workModel.EndDateTuesSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextTues, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Wednesday:
//                    if (workModel.IsWorkActiveWed)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateWed);
//                        var EndDate = getValidValue(workModel.EndDateWed);

//                        var StartDateSP = getValidValue(workModel.StartDateWedSP);
//                        var EndDateSP = getValidValue(workModel.EndDateWedSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextWed, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Thursday:
//                    if (workModel.IsWorkActiveThurs)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateThurs);
//                        var EndDate = getValidValue(workModel.EndDateThurs);

//                        var StartDateSP = getValidValue(workModel.StartDateThursSP);
//                        var EndDateSP = getValidValue(workModel.EndDateThursSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextThurs, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Friday:
//                    if (workModel.IsWorkActiveFri)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateFri);
//                        var EndDate = getValidValue(workModel.EndDateFri);

//                        var StartDateSP = getValidValue(workModel.StartDateFriSP);
//                        var EndDateSP = getValidValue(workModel.StartDateFriSP);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextFri, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                default:

//                    break;

//            }



//            return result;

//        }

//        private bool checkIsInService(string menuSetting)
//        {

//            bool result = true;
//            if (!string.IsNullOrEmpty(menuSetting))
//            {
//                DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
//                DayOfWeek wk = currentDateTime.DayOfWeek;
//                TimeSpan timeOfDay = currentDateTime.TimeOfDay;
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                var workModel = System.Text.Json.JsonSerializer.Deserialize<MessagingPortal.Configuration.Tenants.Dto.WorkModel>(menuSetting, options);

//                switch (wk)
//                {
//                    case DayOfWeek.Saturday:
//                        if (workModel.IsWorkActiveSat)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateSat).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateSat).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }

//                        break;
//                    case DayOfWeek.Sunday:
//                        if (workModel.IsWorkActiveSun)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateSun).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateSun).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Monday:

//                        if (workModel.IsWorkActiveMon)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateMon).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateMon).TimeOfDay)

//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }

//                        break;
//                    case DayOfWeek.Tuesday:
//                        if (workModel.IsWorkActiveTues)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateTues).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateTues).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Wednesday:
//                        if (workModel.IsWorkActiveWed)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateWed).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateWed).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Thursday:
//                        if (workModel.IsWorkActiveThurs)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateThurs).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateThurs).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }
//                        break;
//                    case DayOfWeek.Friday:
//                        if (workModel.IsWorkActiveFri)
//                        {
//                            if (timeOfDay >= getValidValue(workModel.StartDateFri).TimeOfDay && timeOfDay <= getValidValue(workModel.EndDateFri).TimeOfDay)
//                            {
//                                result = true;
//                            }
//                            else
//                            {
//                                result = false;
//                            }
//                        }
//                        break;
//                    default:

//                        break;

//                }

//            }

//            return result;

//        }
//        private DateTime getValidValue(dynamic value)
//        {
//            DateTime result = DateTime.MinValue;
//            try
//            {
//                result = DateTime.Parse(value.ToString());
//                return result;
//            }
//            catch (Exception)
//            {
//                return result;
//                throw;
//            }

//        }
//        private List<string> GetUserByTeneantId(int TenaentId)
//        {

//            List<string> lstUserToken = new List<string>();
//            var list = _iUserAppService.GetUserToken(TenaentId);
//            if (list != null)
//            {
//                foreach (var item in list)
//                {
//                    lstUserToken.Add(item.Token);
//                }
//            }

//            return lstUserToken;
//        }
//        private async Task<TenantModel> GetTenantById(int? id)
//        {

//            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

//            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == id);
//            return tenant;
//        }

//        private List<Attachment> FillAttachmentsTicketData(AttachmentModel[] attachments)
//        {
//            List<Attachment> attachmentsList = new List<Attachment>();

//            foreach (var item in attachments)
//            {
//                var ext = System.IO.Path.GetExtension(item.contentUrl);

//                item.contentName = "infoseed" + ext;
//                //if (item.contentName == "")
//                //{
//                //    return attachmentsList;
//                //}
//                Helper.Helper _helper = new Helper.Helper();
//                var type = _helper.GetContentType(item.contentName);

//                if (type != null)
//                {
//                    byte[] tContent;
//                    string tContentType;
//                    if (item.contentUrl != null && _helper.DownloadAttachment(item.contentUrl, out tContent, out tContentType, out long? _KBSize))
//                    {
//                        attachmentsList.Add(new Attachment
//                        {
//                            FileType = tContentType,
//                            Filename = item.contentName,
//                            Base64 = tContent
//                        });
//                    }

//                }

//            }

//            return attachmentsList;
//        }

       


//        #endregion

//        #region Selling Request 


//        [Route("GetListPDF")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> GetListPDF(int TenantID, string phoneNumber, long? RealEstateType, int? RealEstateResidentialType, long? RealEstateResidentialForSaleType, int? VillaType, bool isOffer = false)
//        {

//            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
//            var lstAsset = _iAssetAppService.GetListOfAsset(TenantID, RealEstateType.Value, RealEstateResidentialForSaleType.Value, RealEstateResidentialType.Value,null,null, isOffer);

//            if (lstAsset != null)
//                foreach (var item in lstAsset)
//                {
//                    if (item.lstAssetAttachmentDto != null)
//                    {
//                        foreach (var objitem in item.lstAssetAttachmentDto)
//                        {

//                            getListPDFModels.Add(new GetListPDFModel
//                            {
//                                AttachmentName = objitem.AttachmentName,
//                                AttachmentType = objitem.AttachmentType,
//                                AttachmentUrl = objitem.AttachmentUrl,
//                                phoneNumber = phoneNumber,
//                                TenantID = TenantID,
//                                AssetDescriptionAr = item.AssetDescriptionAr,
//                                AssetDescriptionEn = item.AssetDescriptionEn,
//                                 AssetNameAr=item.AssetNameAr,
//                                  AssetNameEn=item.AssetNameEn

//                            });

//                           // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

//                        }
//                    }

//                }

            

//            return getListPDFModels;
//        }
//        [Route("GetAsset")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> GetAsset(int tenantID, string phoneNumber, int? typeId = null, long? levleOneId = 0, long? levelTwoId = 0,  long? levelThreeId = 0, long? levelFourId = 0, bool isOffer = false)
//        {

//            if (levleOneId==null)
//                levleOneId=0;
//            if (levelTwoId==null)
//                levelTwoId=0;
//            if (levelThreeId==null)
//                levelThreeId=0;
//            if (levelFourId==null)
//                levelFourId=0;

//            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();

//            try {

//                var lstAsset = _iAssetAppService.GetListOfAsset(tenantID,
//                    levleOneId,
//                    levelTwoId,
//                    typeId,
//                   levelThreeId,
//                   levelFourId
//                   ,isOffer);


//                if (lstAsset != null)
//                    foreach (var item in lstAsset)
//                    {
//                        if (item.lstAssetAttachmentDto != null)
//                        {
//                            foreach (var objitem in item.lstAssetAttachmentDto)
//                            {

//                                getListPDFModels.Add(new GetListPDFModel
//                                {
//                                    AttachmentName = objitem.AttachmentName,
//                                    AttachmentType = objitem.AttachmentType,
//                                    AttachmentUrl = objitem.AttachmentUrl,
//                                    phoneNumber = phoneNumber,
//                                    TenantID = tenantID,
//                                    AssetDescriptionAr=item.AssetDescriptionAr,
//                                    AssetDescriptionEn=item.AssetDescriptionEn,
//                                     AssetNameAr=item.AssetNameAr,
//                                      AssetNameEn=item.AssetNameEn,


//                                });

//                                // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

//                            }
//                        }
//                        else
//                        {
//                            getListPDFModels.Add(new GetListPDFModel
//                            {
//                                //AttachmentName = objitem.AttachmentName,
//                                // AttachmentType = objitem.AttachmentType,
//                                //AttachmentUrl = objitem.AttachmentUrl,
//                                phoneNumber = phoneNumber,
//                                TenantID = tenantID,
//                                AssetDescriptionAr=item.AssetDescriptionAr,
//                                AssetDescriptionEn=item.AssetDescriptionEn,
//                                AssetNameAr=item.AssetNameAr,
//                                AssetNameEn=item.AssetNameEn


//                            });
//                        }

//                    }



//                return getListPDFModels;

//            }
//            catch(Exception ex)
//            {
//                return getListPDFModels;

//            }

//        }

//        [Route("getListPDFName")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> getListPDFName(int tenantID, string phoneNumber, int? typeId = null, long? levleOneId = 0, long? levelTwoId = 0, long? levelThreeId = 0, long? levelFourId = 0, bool isOffer = false)
//        {

//            if (levleOneId==null)
//                levleOneId=0;
//            if (levelTwoId==null)
//                levelTwoId=0;
//            if (levelThreeId==null)
//                levelThreeId=0;
//            if (levelFourId==null)
//                levelFourId=0;

//            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();

//            try
//            {

//                var lstAsset = _iAssetAppService.GetListOfAsset(tenantID,
//                    levleOneId,
//                    levelTwoId,
//                    typeId,
//                   levelThreeId,
//                   levelFourId
//                   , isOffer);


//                if (lstAsset != null)
//                    foreach (var item in lstAsset)
//                    {
//                        if (item.lstAssetAttachmentDto != null)
//                        {
//                            getListPDFModels.Add(new GetListPDFModel
//                            {
//                                //AttachmentName = objitem.AttachmentName,
//                                // AttachmentType = objitem.AttachmentType,
//                                //AttachmentUrl = objitem.AttachmentUrl,
//                                phoneNumber = phoneNumber,
//                                TenantID = tenantID,
//                                AssetDescriptionAr=item.AssetDescriptionAr,
//                                AssetDescriptionEn=item.AssetDescriptionEn,
//                                AssetNameAr=item.AssetNameAr,
//                                AssetNameEn=item.AssetNameEn


//                            });
//                        }
//                        else
//                        {
//                            getListPDFModels.Add(new GetListPDFModel
//                            {
//                                //AttachmentName = objitem.AttachmentName,
//                                // AttachmentType = objitem.AttachmentType,
//                                //AttachmentUrl = objitem.AttachmentUrl,
//                                phoneNumber = phoneNumber,
//                                TenantID = tenantID,
//                                AssetDescriptionAr=item.AssetDescriptionAr,
//                                AssetDescriptionEn=item.AssetDescriptionEn,
//                                AssetNameAr=item.AssetNameAr,
//                                AssetNameEn=item.AssetNameEn


//                            });
//                        }

//                    }



//                return getListPDFModels;

//            }
//            catch (Exception ex)
//            {
//                return getListPDFModels;

//            }

//        }

//        [Route("getListPDFOfferName")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> getListPDFOfferName(int tenantID, string phoneNumber = "")
//        {
//            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
//            try
//            {

//                var offers = _iAssetAppService.GetOfferAsset(0, 50, tenantID);
//                if (offers.lstAssetDto != null)
//                    foreach (var item in offers.lstAssetDto)
//                    {
//                        if (item.lstAssetAttachmentDto != null)
//                        {
//                            getListPDFModels.Add(new GetListPDFModel
//                            {
//                                AttachmentName = "",
//                                AttachmentType = "",
//                                AttachmentUrl = "",
//                                phoneNumber = phoneNumber,
//                                TenantID = tenantID,
//                                AssetDescriptionAr = item.AssetDescriptionAr,
//                                AssetDescriptionEn = item.AssetDescriptionEn,
//                                AssetNameAr=item.AssetNameAr,
//                                AssetNameEn=item.AssetNameEn


//                            });
//                        }

//                    }
//                return getListPDFModels;

//            }
//            catch (Exception ex)
//            {
//                return getListPDFModels;
//            }


//        }


//        [Route("GetAssetOffers")]
//        [HttpGet]
//        public async Task<List<GetListPDFModel>> GetAssetOffers(int tenantID, string phoneNumber = "")
//        {
//            List<GetListPDFModel> getListPDFModels = new List<GetListPDFModel>();
//            try
//            {
               
//                var offers = _iAssetAppService.GetOfferAsset(0, 50, tenantID);
//                if (offers.lstAssetDto != null)
//                    foreach (var item in offers.lstAssetDto)
//                    {
//                        if (item.lstAssetAttachmentDto != null)
//                        {
//                            foreach (var objitem in item.lstAssetAttachmentDto)
//                            {

//                                getListPDFModels.Add(new GetListPDFModel
//                                {
//                                    AttachmentName = objitem.AttachmentName,
//                                    AttachmentType = objitem.AttachmentType,
//                                    AttachmentUrl = objitem.AttachmentUrl,
//                                    phoneNumber = phoneNumber,
//                                    TenantID = tenantID,
//                                    AssetDescriptionAr = item.AssetDescriptionAr,
//                                    AssetDescriptionEn = item.AssetDescriptionEn,
//                                    AssetNameAr=item.AssetNameAr,
//                                    AssetNameEn=item.AssetNameEn


//                                });

//                                // await sendAssetAttachment(TenantID, phoneNumber, objitem.AttachmentUrl, objitem.AttachmentName, objitem.AttachmentType);

//                            }
//                        }

//                    }
//                return getListPDFModels;

//            }
//            catch(Exception ex)
//            {
//                return getListPDFModels;
//            }
            

//        }
//        [Route("sendAssetAttachmentBot")]
//        [HttpPost]
//        public async Task<string> sendAssetAttachmentBot([FromBody] GetListPDFModel getListPDFModel)
//        {
   
//             await sendAssetAttachment(getListPDFModel.TenantID, getListPDFModel.phoneNumber, getListPDFModel.AttachmentUrl, getListPDFModel.AttachmentName, getListPDFModel.AttachmentType);

//            return "Done";
//        }

//        private async Task<SellingRequestDto> AddSellingRequest(SellingRequestDto sellingRequestDto)
//        {

       
//            var jsonModel = JsonConvert.SerializeObject(sellingRequestDto).ToString();
//            long id=  _iSellingRequestAppService.AddSellingRequest(sellingRequestDto);
//            sellingRequestDto.Id=id ;
//            return sellingRequestDto;

//         }


//        private async Task sendAssetAttachment(int tenantID, string phoneNumber, string mediaUrl, string name, string type)
//        {
//            var Tenant = GetTenantById(tenantID).Result;

//            var customer = await _dbService.GetCustomerWithTenantId(tenantID + "_" + phoneNumber, tenantID);


//            var content = new Content()
//            {
//                type = type,
//                text = "",
//                fileName = name,
//                mediaUrl = mediaUrl,
//                agentName = Tenant.botId,
//                agentId = "1000000"
//            };



//            var masseges = new SendWhatsAppD360Model
//            {

//                mediaUrl = mediaUrl,
//                to = phoneNumber,
//                type = type== "file"? "document":type,
//                fileName= name,
//                document = new SendWhatsAppD360Model.Document
//                {
//                    link = mediaUrl
//                }

//            };
            

//            var result = await WhatsAppDialogConnector.PostMsgToSmooch(Tenant.D360Key, masseges, _telemetry, true);

//            if (result == HttpStatusCode.Created)
//            {

//                var CustomerChat = _dbService.UpdateCustomerChatD360(tenantID, content, tenantID + "_" + phoneNumber, customer.ConversationId);
//                customer.customerChat = CustomerChat;

//               // await _hub2.Clients.All.SendAsync("brodCastAgentMessage", customer);
//                SocketIOManager.SendChat(customer, tenantID);


//            }
//        }

//        #endregion


//        #region Delivery cost Per KiloMeter
//        private GetLocationInfoModel getLocationInfoPerKiloMeter(int tenantID, string query)
//        {


//            try {
//                double latitude = double.Parse(query.Split(",")[0]);
//                double longitude = double.Parse(query.Split(",")[1]);
//                var rez = GetLocation(query);
//                GetLocationInfoModel getLocationInfoModel = new GetLocationInfoModel();


//                getLocationInfoModel.Country = rez.Country.Replace("'", "").Trim();
//                getLocationInfoModel.City = rez.City.Replace("'", "").Trim();
//                getLocationInfoModel.Area = rez.Area.Replace("'", "").Trim();
//                getLocationInfoModel.Distric = rez.Distric.Replace("'", "").Trim();
//                string Route = rez.Route.Replace("'", "").Trim();

//                getLocationInfoModel.DeliveryCostAfter = -1;
//                getLocationInfoModel.DeliveryCostBefor = -1;
//                getLocationInfoModel.LocationId = 0;
//                getLocationInfoModel.Address = Route + " - " + getLocationInfoModel.Distric + " - " + getLocationInfoModel.Area + " - " + getLocationInfoModel.City + " - " + getLocationInfoModel.Country;
//                double distance;
//                AreaDto AreaDto = getNearbyArea(tenantID, latitude, longitude, null,0, out distance);

//                if (AreaDto.Id > 0 && distance != -1)
//                {
//                    DeliveryCostDto deliveryCostDto = _iDeliveryCostAppService.GetDeliveryCostByAreaId(tenantID, AreaDto.Id);

//                    if (deliveryCostDto != null)
//                    {

//                        decimal value = -1;
//                        if (deliveryCostDto.lstDeliveryCostDetailsDto != null)
//                        {
//                            distance = distance / 1000.00; // convert a meter to kilo-meter

//                            foreach (var item in deliveryCostDto.lstDeliveryCostDetailsDto)
//                            {
//                                if ((double)item.From <= distance &&   distance <= (double)item.To)
//                                {
//                                    value = item.Value;
//                                    break;
//                                }
//                            }
//                            if (value == -1)
//                            {
//                                value = deliveryCostDto.AboveValue;
//                            }
//                        }
//                        getLocationInfoModel.DeliveryCostAfter = value;
//                        getLocationInfoModel.DeliveryCostBefor = value;
//                    }
//                }
//                getLocationInfoModel.LocationId =(int)AreaDto.Id;

//                if (AreaDto.IsRestaurantsTypeAll)
//                {
//                    getLocationInfoModel.LocationId=0;

//                }
//                return getLocationInfoModel;

//            }
//            catch(Exception ex)
//            {
//                return new GetLocationInfoModel() { 
                
//                   DeliveryCostAfter=-1,
//                    DeliveryCostBefor=-1,
//                     LocationId=0
                
//                };

//            }

//        }
//        #endregion


//        #region  Jeep  bot

//        [Route("CheckIsFillDisplayName")]
//        [HttpGet]
//        public string checkIsFillDisplayName(int id)
//        {
//            var con = GetContact(id);
//            if (string.IsNullOrEmpty(con.ContactDisplayName))
//                return null;
//            else
//              return con.ContactDisplayName;
//        } 
        
//        [Route("UpdateContcatDisplayName")]
//        [HttpGet]
//        public void updateContcatDisplayName(int id, string contactDisplayName)
//        {
//            ContactDto contactDto = new ContactDto();
//            contactDto.Id = id;  
//            contactDto.ContactDisplayName = contactDisplayName;
//            updateContactSP(contactDto);

//        }

//        private void updateContactSP(ContactDto contactDto)
//        {
//            try
//            {
//                var SP_Name = Constants.Contacts.SP_ContactUpdate;
//                var sqlParameters = new List<SqlParameter>
//                {
//               new SqlParameter("@Id",contactDto.Id)
//               ,new SqlParameter("@ContactDisplayName",contactDto.ContactDisplayName)
//                 };

//                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        #endregion



//        [Route("lstGetChat")]
//        [HttpGet]
//        public async Task<string> fillchat(int length)
//        {

//            SocketIOManager.SendChat("Audai Walla test",27);
//            return "fff";
        
//        }



//        [Route("SingalRGet")]
//        [HttpGet]
//        public async Task<string> SingalRGet(int length)
//        {
//            try
//            {

            

//            for (int i = 0; i < length; i++)
//            {
//                    TestSingalRModel testSingalRModel = new TestSingalRModel();
//                    testSingalRModel.Id = i;
//                    testSingalRModel.Name ="Admin"+ i;
//                   // await _TestSingalRhub.Clients.All.SendAsync("broadcastTestSingalR", testSingalRModel);

//                }
//                return "done";
//            }
//            catch (Exception ex)
//            {

//                return ex.Message;
//            }
//        }
//        }





//}

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
//using Infoseed.MessagingPortal.ContactNotification;
//using Infoseed.MessagingPortal.Evaluations;
//using System.Net;
//using Infoseed.MessagingPortal.DeliveryOrderDetails.Dtos;
//using Microsoft.Extensions.Configuration;
//using System.Globalization;
//using Infoseed.MessagingPortal.OrderDetails;
//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//namespace Infoseed.MessagingPortal.Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BotAPIController : MessagingPortalControllerBase
//    {
//        private readonly IRepository<Items.Item, long> _lookup_ItemRepository;
//        private readonly IRepository<OrderDetail, long> _orderDetailRepository;
//        private readonly IRepository<Order, long> _lookup_orderRepository;
//        private static readonly object CurrentOrder = new object();
//        private IAppNotifier _appNotifier;
//        private IUserNotificationManager _userNotificationManager;
//        private IRepository<Contact> _contactRepos;
//       // private IHubContext<OrderHub> _hub;
//        //private IHubContext<SignalR.TeamInboxHub> _hub2;
//        private IHubContext<EvaluationHub> _evaluationHub;
//        IDBService _dbService;
//        private readonly IRepository<Evaluation, long> _evaluationRepository;
//        private IConfiguration _configuration;
//        public BotAPIController(IRepository<Items.Item, long> lookup_ItemRepository,IRepository<Order, long> lookup_orderRepository,IRepository<OrderDetail, long> orderDetailRepository,IConfiguration configuration,IDBService dbService, 
//            //IHubContext<OrderHub> hub,
//            IRepository<Contact> contactRepos ,
//            IAppNotifier appNotifier, IContactNotification contactNotification, IUserNotificationManager userNotificationManager,
//            IRepository<Evaluation, long> evaluationRepository, IHubContext<EvaluationHub> evaluationHub
//            //, 
//            //IHubContext<SignalR.TeamInboxHub> hub2
//            )
//        {
//            _lookup_ItemRepository = lookup_ItemRepository;
//            _orderDetailRepository = orderDetailRepository;
//            _orderDetailRepository = orderDetailRepository;
//            _configuration = configuration;
//            _userNotificationManager = userNotificationManager;
//            _appNotifier = appNotifier;
//            _contactRepos = contactRepos;
//            _dbService = dbService;
//            //_hub = hub;
//            //_hub2 = hub2;
//            _evaluationHub = evaluationHub;
//            _evaluationRepository = evaluationRepository;
//        }


//        //*
//        [Route("GetOrderDetailString")]
//        [HttpGet]
//        public string GetOrderDetailString(int? TenantID, int? OrderId, string lang)
//        {
//            if(lang==null)
//            {

//                lang = "ar-JO";
//            }
//            var ord = GetOrderS(TenantID, OrderId);

//            var captionQuantityText = GetCaptionFormat("BackEnd_Text_Quantity", lang, TenantID, "", "",0);//العدد :
//            var captionAddtionText = GetCaptionFormat("BackEnd_Text_Addtion", lang, TenantID, "", "",0);//الاضافات
//            var captionTotalText = GetCaptionFormat("BackEnd_Text_Total", lang, TenantID, "", "", 0);//المجموع:       
//            var captionTotalOfAllText = GetCaptionFormat("BackEnd_Text_TotalOfAll", lang, TenantID, "", "", 0);//السعر الكلي للصنف: 

//            var OrderDetailList = GetOrderDetail(TenantID, OrderId);
//            var listString = "-------------------------- \r\n\r\n";
//            decimal? total = 0;

//            foreach (var OrderD in OrderDetailList)
//            {
//                var getOrderDetailExtra = GetOrderDetailExtra(TenantID, OrderD.Id);


//                var item = GetItem(TenantID, OrderD.ItemId);

               

//                if (lang == "ar-JO")
//                {
//                    listString = listString +"*"+ item.ItemName.Trim() + "*" + "\r\n";
//                }
//                else
//                {
//                    listString = listString + "*" + item.ItemNameEnglish.Trim() + "*" + "\r\n";
//                }

                
               


//                if (getOrderDetailExtra.Count > 0)
//                {
//                    listString = listString + "*"+ captionAddtionText.Trim() + "*" + "\r\n";

//                }

//                foreach (var ext in getOrderDetailExtra)
//                {
//                    if (ext.Quantity > 1)
//                    {
//                        listString = listString + ext.Name +  "  ("+ ext.Quantity + ")"+ "\r\n";
//                    }
//                    else
//                    {
//                        listString = listString + ext.Name + "\r\n";

//                    }

                   
//                    //listString = listString + captionQuantityText + ext.Quantity + "\r\n";

//                }

//                listString = listString + "\r\n" + "*"+captionQuantityText + OrderD.Quantity + "*" + "\r\n";

//                listString = listString + captionTotalOfAllText + OrderD.Total + "\r\n\r\n";




//                total = total + OrderD.Total;
//            }
//            listString = listString + "-------------------------- \r\n\r\n";
//            listString = listString + captionTotalText + ord.Total;

//            return listString;
//        }

//        //*
//        [Route("GetAreasWithR")]
//        [HttpGet]
//        public List<Area> GetAreasWithR(string TenantID,int menu)
//        {
//            var list = GetAreasList(TenantID, menu);

//            if (list.Count > 10)
//            {

//                var values = list.Skip(9).Take(9).ToList();

//                return values;
//            }
//            else
//            {

//                return list;
//            }
           
//        }


//        [Route("GetAreasWithPage")]
//        [HttpGet]
//        public List<Area> GetAreasWithPage(string TenantID, int menu , int pageNumber, int pageSize)
//        {
//            var list = GetAreasList(TenantID, menu);

//            if (list.Count > 10)
//            {
//                var values = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();
//                if (pageNumber >=1)
//                {
//                    var values2 = list.Skip(pageSize * pageNumber).Take(pageSize).ToList();
//                    //values2.Add(new Area
//                    //{
//                    //    AreaName = "Back",
//                    //    AreaCoordinate = "<---"


//                    //});
//                    return values2;
//                }
                

//                values.Add(new Area {
//                 AreaName= "Others",
//                  AreaCoordinate="--->"
                 
                
//                });



//                return values;
//            }
//            else
//            {
                

//                    return list;
//            }

//        }

//        private List<Area> GetAreasList(string TenantID, int menu)
//        {
//            //TenantID = "31";
//            var tenantID = int.Parse(TenantID);
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Areas] where TenantID=" + tenantID + " and  RestaurantsType =" + menu;


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
//                        AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString()??"",
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                        UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                        RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                        IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                        IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString()),


//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }

//        [Route("GetDay")]
//        [HttpGet]
//        public List<string> GetDay(string TenantID)
//        {
//            List<string> vs = new List<string>();
//            //var dayName = DateTime.Now.ToString("dddd");

//            for(int i = 0; i <= 6; i++)
//            {
//                var day = DateTime.Now.AddDays(i);
//                var dayName = day.ToString("dddd", new CultureInfo("ar-AE"));
//                var date = day.ToString("dd/MM", new CultureInfo("ar-AE"));

//                var st = dayName + "(" + date + ")";

//                vs.Add(st);
//            }

//            return vs;
//        }


//        [Route("CheckGetDay")]
//        [HttpGet]
//        public bool CheckGetDay(string selectDay)
//        {
//            var list = GetDay("0");
//           var x= list.Where(x => x == selectDay).FirstOrDefault();
//            if (x != null)
//            {

//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }


//        [Route("GetTime")]
//        [HttpGet]
//        public List<string> GetTime(string TenantID,string selectDay)
//        {
//            List<string> vs = new List<string>();
//            List<string> vs2 = new List<string>();
//            var dayName = DateTime.Now.ToString("dddd");

//            var timeNow = DateTime.Now.AddHours(3).ToString("hh:mm:ss", CultureInfo.InvariantCulture);
//            var resulttimeNow = Convert.ToDateTime(timeNow);


//            if (TenantID == "5")
//            {
//                vs.Add("10:00 AM");
//                vs.Add("11:00 AM");
//                vs.Add("12:00 PM");
//                vs.Add("1:00 PM");
//                vs.Add("2:00 PM");
//                vs.Add("3:00 PM");
//                vs.Add("4:00 PM");
//                vs.Add("5:00 PM");
//                vs.Add("6:00 PM");
//                vs.Add("7:00 PM");

//            }
//            else
//            {
//                vs.Add("3:00 PM");
//                vs.Add("4:00 PM");
//                vs.Add("5:00 PM");
//                vs.Add("6:00 PM");
//                vs.Add("7:00 PM");
//                vs.Add("8:00 PM");
//                vs.Add("9:00 PM");
//                vs.Add("10:00 PM");
//                vs.Add("11:00 PM");
//                vs.Add("12:00 PM");

//            }
         


//            if (selectDay == "اليوم")
//                selectDay = dayName;


//            if (selectDay== dayName)
//            {
//                foreach (var item in vs)
//                {
//                    if (int.Parse(item.Split(":")[0]) <= resulttimeNow.Hour)
//                    {

//                        //vs.Remove(item);
//                    }
//                    else
//                    {

//                        vs2.Add(item);
//                    }
//                }


//                return vs2;
//            }
//            else
//            {


//                return vs;
//            }

               
           
            
//        }


//        [Route("CheckGetTime")]
//        [HttpGet]
//        public bool CheckGetTime(string selectTime, string selectDay)
//        {
//            var list = GetTime("0", selectDay);
//            var x = list.Where(x => x == selectTime).FirstOrDefault();
//            if (x != null)
//            {

//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }


//        //*
//        [Route("GetContactId")]
//        [HttpGet]
//        public async Task<GetContactIdModel> GetContactIdAsync(string phoneNumber, string TenantID)
//        {

           




//            //TenantID = "31";
//            //phoneNumber = "962779746365";
//            var tenantID = int.Parse(TenantID);
//            string connString = AppSettingsModel.ConnectionStrings;
//            ContactsSyncService contactsSyncService = new ContactsSyncService(_contactRepos);
//           // var x=  contactsSyncService.Sync(connString);


//            string query = "select * from [dbo].[Contacts] where TenantID=" + tenantID + " and PhoneNumber= '" + phoneNumber + "'";


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            GetContactIdModel contactIdModel = new GetContactIdModel();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                contactIdModel = new GetContactIdModel
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
//                    DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),

//                };
//            }

//            conn.Close();
//            da.Dispose();

//            return contactIdModel;
//        }


//        //*
//        [Route("SplitUserString")]
//        [HttpGet]    
//        public ContactModelBot SplitUserString(string text)
//        {   
//            try
//            {
//                var split = text.Split(",").ToList();
//                ContactModelBot contact = new ContactModelBot
//                { 
//                      PhoneNumber = split[0],
//                      DisplayName = split[1],
//                      //ContactID = split[2],
//                      // UserID= split[3]

//                };

//                return contact;

//            }
//            catch
//            {

//                return null;
//            }


//        }


//        //*
//        [Route("SplitEvaluationString")]
//        [HttpGet]
//        public EvaluationModelBot SplitEvaluationString(string text)
//        {
//            ContactsSyncService contactsSyncService = new ContactsSyncService(_contactRepos);
//            string connString = AppSettingsModel.ConnectionStrings;
//           // var x = contactsSyncService.Sync(connString);
//            try
//            {
//                var split = text.Split(",").ToList();
//                EvaluationModelBot  evaluation = new EvaluationModelBot();

//                evaluation.OrderNumber = split[0];
//                evaluation.TenantId = int.Parse(split[1]);
//                evaluation.EvaluationText = split[2];



//                return evaluation;

//            }
//            catch
//            {

//                return null;
//            }


//        }

//        //*
//        [Route("GetAreasID")]
//        [HttpGet]
//        public Area GetAreasID(string TenantID, string AreaName)
//        {

//            var list = GetAreas(TenantID);

//            var area = list.Where(x =>( x.AreaName + " (" + x.AreaCoordinate + ")").Contains(AreaName)).FirstOrDefault();

//            if (area == null)
//            {

//                return null;
//            }

//            return area;

//        }

//        //*
//        [Route("GetlocationUser")]
//        [HttpGet]
//        public string GetlocationUser(string query)
//        {

//            var rez = GetLocation(query);

//            var bransh = "";
//            foreach (var item in rez.results)
//            {
//                var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
//                if (neighborhood != null)
//                {
//                    bransh = item.address_components.FirstOrDefault().long_name;
//                    break;
//                }

//            }


//            if (bransh != "")
//            {

//                return bransh;
//            }
//            else
//            {
//                foreach (var item in rez.results)
//                {
//                    var neighborhood = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
//                    if (neighborhood != null)
//                    {
//                        bransh = item.address_components.FirstOrDefault().long_name;

//                        break;
//                    }

//                }


//                if (bransh != "")
//                {

//                    return bransh;
//                }
//                else
//                {
//                    foreach (var item in rez.results)
//                    {
//                        var neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
//                        if (neighborhood != null)
//                        {
//                            bransh = item.address_components.FirstOrDefault().long_name;
//                            break;
//                        }

//                    }
//                    return bransh;
//                }
//            }

//        }
//        [Route("GetlocationUserModel")]
//        [HttpGet]
//        public locationAddressModel GetlocationUserModel(string query)
//        {

//           // var dayName = DateTime.UtcNow.ToString("dddd");

//            locationAddressModel locationAddressModel = new locationAddressModel();
//            var rez = GetLocation(query);


//            foreach (var item in rez.results)
//            {

//                //route
//                var route = item.types.Where(x => x == "street_address").FirstOrDefault();
//                if (route != null)
//                {

//                    locationAddressModel.Route = item.formatted_address.Split(",")[0];
//                    //break;
//                }
//                else
//                {
//                     route = item.types.Where(x => x == "route").FirstOrDefault();
//                    if (route != null)
//                    {

//                        locationAddressModel.Route = item.formatted_address.Split(",")[0];

//                    }
//                    else
//                    {
//                        //locationAddressModel.Route = "";
//                    }
//                }

//                //Distric
//                var neighborhood = item.types.Where(x => x == "neighborhood").FirstOrDefault();
//                if (neighborhood != null)
//                {

//                    locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;

//                }
//                else
//                {
//                     neighborhood = item.types.Where(x => x == "administrative_area_level_2").FirstOrDefault();
//                    if (neighborhood != null && locationAddressModel.Distric==null)
//                    {
//                        locationAddressModel.Distric = item.address_components.FirstOrDefault().long_name;


//                    }
//                    else
//                    {
//                        //locationAddressModel.Distric = "";

//                    }


//                }


//                //Area
//                var sublocality_level_1 = item.types.Where(x => x == "sublocality_level_1").FirstOrDefault();
//                if (sublocality_level_1 != null)
//                {

//                    locationAddressModel.Area = item.address_components.FirstOrDefault().long_name;

//                }
//                else
//                {
//                    //locationAddressModel.Area = "";
//                    ///

//                }


//                //city
//                var locality = item.types.Where(x => x == "locality").FirstOrDefault();
//                if (locality != null)
//                {

//                    locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];

//                }
//                else
//                {


//                    locality = item.types.Where(x => x == "administrative_area_level_1").FirstOrDefault();
//                    if (locality != null )
//                    {
//                        if(item.address_components.FirstOrDefault().long_name.Split(",")[0]== "Jerash Governorate")
//                        {
//                            locationAddressModel.City = item.address_components.FirstOrDefault().long_name.Split(",")[0];
//                        }
                       

//                    }
//                    //locationAddressModel.City = "";
//                }


//                //Country
//                var country = item.types.Where(x => x == "country").FirstOrDefault();
//                if (country != null)
//                {

//                    locationAddressModel.Country = item.address_components.FirstOrDefault().long_name;
//                    //break;
//                }


//            }

//            if (locationAddressModel.Route == null)
//                locationAddressModel.Route = "";
//            if (locationAddressModel.Distric == null)
//                locationAddressModel.Distric = "";
//            if (locationAddressModel.Area == null)
//                locationAddressModel.Area = "";
//            if (locationAddressModel.City == null)
//                locationAddressModel.City = "";
//            if (locationAddressModel.Country == null)
//                locationAddressModel.Country = "";


//            return locationAddressModel;      

//        }

//        //*
//        [Route("GetAddresslocationUser")]
//        [HttpGet]
//        public string GetAddresslocationUser(string query)
//        {
//            var rez = GetLocation(query);
//            var street = "";

//            foreach (var item in rez.results)
//            {
//                var str = item.types.Where(x => x == "street_address").FirstOrDefault();
//                if (str != null)
//                {
//                    street = item.formatted_address;
//                    return street;
//                }
//            }

//            if (street == "")
//            {

//                foreach (var item in rez.results)
//                {
//                    var str = item.types.Where(x => x == "route").FirstOrDefault();
//                    if (str != null)
//                    {
//                        street = item.formatted_address;
//                        return street;
//                    }
//                }

//            }

//            return street;
//        }

//        //*
//        [Route("GetBranch")]
//        [HttpGet]
//        public BranchModel GetBranch(int TenantID, string FromDistrichName, string FromAreaName)
//        {
//            try
//            {
//                var LocationList = GetAllLocationInfoModel();
//                var FromDistrich = LocationList.Where(x => x.LocationNameEn == FromDistrichName).FirstOrDefault();
//                var FromArea = LocationList.Where(x => x.LocationNameEn == FromAreaName && x.LevelId == 3).FirstOrDefault();

//                if (FromDistrich == null)
//                {
//                    //////

//                    var costList2 = GetAllLocationDeliveryCost(TenantID);
//                    var Cos3 = costList2.Where(x => x.LocationId == FromArea.Id).FirstOrDefault();
//                    if (Cos3 == null)
//                    {
//                        BranchModel branch = new BranchModel();
//                        branch.Name = FromDistrichName;
//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = -1;
//                        branch.Id = 0;
//                        branch.BranchAreaId = 0;
//                        branch.LocationID = 0;
//                        branch.BranchAreaName = "";
//                        return branch;

//                    }
//                    else
//                    {
//                        var Getare = GetAreas2(Cos3.BranchAreaId);

//                        BranchModel branch = new BranchModel();
//                        branch.Name = FromArea.LocationName;
//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = Cos3.DeliveryCost;
//                        branch.Id = Cos3.Id;

//                        branch.LocationID = FromArea.Id;

//                        branch.BranchAreaId = Cos3.BranchAreaId;

//                        branch.RestaurantMenuType = Getare.RestaurantsType;
//                        branch.BranchAreaName = Getare.AreaName;
//                        return branch;

//                    }

                  

//                }

//                var costList = GetAllLocationDeliveryCost(TenantID);

//                var Cos = costList.Where(x => x.LocationId == FromDistrich.Id ).FirstOrDefault();
//                // var Cos = GetLocationDeliveryCost(rez.Id, TenantID);

//                if (Cos == null)
//                {

//                    var Cos2 = costList.Where(x => x.LocationId == FromArea.Id).FirstOrDefault();

//                    if (Cos2 == null)
//                    {
//                        BranchModel branch = new BranchModel();
//                        branch.Name = FromDistrichName;
//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = -1;
//                        branch.Id = 0;
//                        branch.BranchAreaId = 0;
//                        branch.LocationID = 0;
//                        branch.BranchAreaName = "";
//                        return branch;


//                    }
//                    else
//                    {
//                        var Getare = GetAreas2(Cos2.BranchAreaId);

//                        BranchModel branch = new BranchModel();
//                        branch.Name = FromDistrich.LocationName;
//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = Cos2.DeliveryCost;
//                        branch.Id = Cos2.Id;

//                        branch.LocationID = FromDistrich.Id;

//                        branch.BranchAreaId = Cos2.BranchAreaId;

//                        branch.RestaurantMenuType = Getare.RestaurantsType;
//                        branch.BranchAreaName = Getare.AreaName;
//                        return branch;


//                    }
                   

//                }
//                else
//                {
                    
//                        var Getare = GetAreas2(Cos.BranchAreaId);

//                    BranchModel branch = new BranchModel();
//                        branch.Name = FromDistrich.LocationName;
//                        branch.TenantId = TenantID;
//                        branch.DeliveryCost = Cos.DeliveryCost;
//                        branch.Id = Cos.Id;

//                        branch.LocationID = FromDistrich.Id;

//                        branch.BranchAreaId = Cos.BranchAreaId;

//                        branch.RestaurantMenuType = Getare.RestaurantsType;
//                    branch.BranchAreaName = Getare.AreaName;
//                    return branch;


                    
                   
//                }



//            } catch
//            {
//                BranchModel branch = new BranchModel();
//                branch.Name = FromDistrichName;
//                branch.TenantId = TenantID;
//                branch.DeliveryCost = -1;
//                branch.Id = 0;
//                branch.BranchAreaId = 0;
//                branch.LocationID = 0;
//                branch.BranchAreaName = "";
//                return branch;

//            }



//        }

//        [Route("GetDeliveryBranch")]
//        [HttpGet]
//        public Models.Location.DeliveryLocationCost GetDeliveryBranch(int TenantID, string FromDistrichName, string FromAreaName, string ToDistrichName, string ToAreaName)
//        {
//            try
//            {
//                var LocationList = GetAllLocationInfoModel();

//                var FromDistrich = LocationList.Where(x => x.LocationNameEn == FromDistrichName ).FirstOrDefault();
//                var FromArea = LocationList.Where(x => x.LocationNameEn == FromAreaName && x.LevelId == 3).FirstOrDefault();

//                var ToDistrich = LocationList.Where(x => x.LocationNameEn == ToDistrichName ).FirstOrDefault();
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

       




//                var Cos = costList.Where(x => x.FromLocationId == FromDistrich.Id && x.ToLocationId == ToDistrich.Id).FirstOrDefault();

//                if (Cos == null)
//                {

//                    var Cos2 = costList.Where(x => x.FromLocationId == FromArea.Id && x.ToLocationId == ToArea.Id).FirstOrDefault();

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
//        [Route("IsOrderOffer")]
//        [HttpGet]
//        public async Task<bool> IsOrderOffer(int? TenantID)
//        {
//            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

//            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID);

//            if (tenant != null)
//            {
//                if (tenant.isOrderOffer)
//                {
//                    return true;
//                }
//                else
//                {

//                    return false;
//                }
//            }
//            else
//            {

//                return false;
//            }
            
//        }


//        [Route("GetOrderOfferCost")]
//        [HttpGet]
//        public async Task<OrderOfferCostModel> GetOrderOfferCost(int? TenantID,string AreaO,decimal deliveryCost, decimal Total)
//        {
//            OrderOfferCostModel orderOfferCostModel = new OrderOfferCostModel();


//            var LocationList = GetAllLocationInfoModel();

//            var areaName = LocationList.Where(x => x.LocationNameEn == AreaO).FirstOrDefault();

//            if(areaName==null)
//            {
//                return orderOfferCostModel;
//            }

//            var OrderOfferList = GetOrderOffer(TenantID);


//            foreach(var item in OrderOfferList)
//            {
              
//                if (item.Area.Contains(areaName.LocationName))
//                {
//                    //var totalWithBranchCost = (deliveryCost + Total);

//                    if (Total >= item.FeesStart && Total <= item.FeesEnd)
//                    {

//                        var DateNow = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
//                        var DateStart = item.OrderOfferDateStart.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
//                        var DateEnd = item.OrderOfferDateEnd.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);


//                        var resultDateNow = Convert.ToDateTime(DateNow);
//                        var resultDateStart = Convert.ToDateTime(DateStart);
//                        var resultDateEnd = Convert.ToDateTime(DateEnd);

//                        if (resultDateStart <= resultDateNow && resultDateEnd>= resultDateNow)
//                        {
//                            var timeNow = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
//                            var timeStart = item.OrderOfferStart.AddHours(-3).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
//                            var timeEnd = item.OrderOfferEnd.AddHours(-3).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);

//                            var resulttimeNow = Convert.ToDateTime(timeNow);
//                            var resulttimeStart = Convert.ToDateTime(timeStart);
//                            var resulttimeEnd = Convert.ToDateTime(timeEnd);

//                            var resulttimeStartS = Convert.ToInt64(timeStart.Split(":")[0]);
//                            var resulttimeEndS = Convert.ToInt64(timeEnd.Split(":")[0]);


//                            if ((resulttimeStart <= resulttimeNow && resulttimeNow <= resulttimeEnd))
//                            {
//                                orderOfferCostModel = new OrderOfferCostModel
//                                {
//                                    beforOrderDeliveryCost = deliveryCost,
//                                    afterOrderDeliveryCost = item.NewFees

//                                };

//                                return orderOfferCostModel;

//                            }
//                            else
//                            {
//                                //if (resulttimeStartS >= resulttimeEndS)
//                                //{

//                                //    orderOfferCostModel = new OrderOfferCostModel
//                                //    {
//                                //        beforOrderDeliveryCost = deliveryCost,
//                                //        afterOrderDeliveryCost = item.NewFees

//                                //    };

//                                //    return orderOfferCostModel;

//                                //}


//                            }

//                        }


                        

                       
                       
//                    }
                    
                   
                   
//                }


//            }

//            return orderOfferCostModel;

//        }
//        //*
//        [Route("GetOrder")]
//        [HttpGet]
//        public async Task<Order> GetOrder(int? TenantID, int ContactId)
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
//        //*
//        [Route("UpdateOrder")]
//        [HttpPost]
//        public async Task<string> UpdateOrderAsync([FromBody] OrderBotData orderBotData)
//        {
            
//            var time = DateTime.Now;

//            var timeAdd = time.AddHours(3);
//            string connString = AppSettingsModel.ConnectionStrings;


//            long number =0;

//            lock (CurrentOrder)
//            {
//                 number = UpateOrder(orderBotData.TenantID);
               
//            }


//            var con = GetContact(orderBotData.ContactId);

//            var captionOrderInfoText = GetCaptionFormat("BackEnd_Text_OrderInfo", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//شكرا لك معلومات الطلب \r\n
//            var captionOrderNumberText = GetCaptionFormat("BackEnd_Text_OrderNumber", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//رقم الطلب :
//            var captionTotalOfAllOrderText = GetCaptionFormat("BackEnd_Text_TotalOfAllOrder", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//مجموع القيم :
//            var captionEndOrderText = GetCaptionFormat("BackEnd_Text_EndOrder", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم  \r\n

//            if (orderBotData.TypeChoes == 1)
//            {
//                var area = GetAreas(orderBotData.TenantID.ToString()).Where(x => x.Id == orderBotData.BranchAreaId).FirstOrDefault();
//                long agId = 0;

//                var BranchAreaName = "";

//                //Notification
//                if (orderBotData.BranchAreaId != 0)
//                {


//                    if (area != null)
//                    {
//                        BranchAreaName = area.AreaName;/*+ " ( " + area.AreaCoordinate+" ) ";*/
//                        var message = "The order AssignTo :" + area.AreaName + "(" + area.AreaCoordinate + ")";
//                        UserNotification Notification = await SendNotfAsync(message, Convert.ToInt64(area.UserId), orderBotData.TenantID);
//                        agId = Convert.ToInt64(area.UserId);
//                    }


//                }
//                ///**************//

//                string htmlOrderD = GetHtmlD(orderBotData, timeAdd, number, con , BranchAreaName, orderBotData.BotLocal);

//                ///**************//

//                var captionBranchCostText = GetCaptionFormat("BackEnd_Text_BranchCost", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//قيمة التوصيل :
//                var captionFromLocationText = GetCaptionFormat("BackEnd_Text_FromLocation", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//من الموقع :

//                using (SqlConnection connection = new SqlConnection(connString))
//                using (SqlCommand command = connection.CreateCommand())
//                {
//                    //valdet if the order exist or not 
//                    command.CommandText = "UPDATE Orders SET OrderTime = @OrT, OrderStatus = @OrI ,CreationTime =@CreT ,OrderType = @OrTy, Total= @Tot ,Address=@Add ,DeliveryCost=@Dcos ,IsEvaluation=@IsEv ,BranchAreaId=@BranchAreaId  ,BranchAreaName=@BranchAreaName, OrderNumber =@OrderNumber ,LocationID=@LocationID ,FromLocationDescribation=@FromLocationDescribation, AfterDeliveryCost=@AfterDeliveryCost , SelectDay=@SelectDay, SelectTime=@SelectTime, IsPreOrder=@IsPreOrder, RestaurantName=@RestaurantName , HtmlPrint=@HtmlPrint Where Id = @Id";
//                    command.Parameters.AddWithValue("@Id", orderBotData.Id);
//                    command.Parameters.AddWithValue("@OrT", timeAdd);
//                    command.Parameters.AddWithValue("@CreT", timeAdd);
//                    command.Parameters.AddWithValue("@Tot", (orderBotData.Total + decimal.Parse(orderBotData.BranchCost)));
//                    command.Parameters.AddWithValue("@Add", orderBotData.Address);
//                    command.Parameters.AddWithValue("@Dcos", orderBotData.BranchCost);
//                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Delivery);

//                    if (orderBotData.IsPreOrder)
//                    {
//                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pre_Order);
//                    }
//                    else
//                    {
//                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
//                    }

//                    command.Parameters.AddWithValue("@IsEv", false);
//                    command.Parameters.AddWithValue("@BranchAreaId", orderBotData.BranchAreaId);

//                    command.Parameters.AddWithValue("@BranchAreaName", BranchAreaName);// orderBotData.BranchName);//نفيسة
//                    command.Parameters.AddWithValue("@RestaurantName", orderBotData.RustrantBranchAreaName);//دوار المدينة

//                    command.Parameters.AddWithValue("@LocationID", orderBotData.LocationID);




//                    command.Parameters.AddWithValue("@FromLocationDescribation", "https://maps.google.com/?q=" + orderBotData.LocationFrom.Replace(" ", ""));
//                    command.Parameters.AddWithValue("@OrderNumber", number);

//                    command.Parameters.AddWithValue("@AfterDeliveryCost", decimal.Parse(orderBotData.AfterBranchCost));

//                    command.Parameters.AddWithValue("@SelectDay", orderBotData.SelectDay);
//                    command.Parameters.AddWithValue("@SelectTime", orderBotData.SelectTime);
//                    command.Parameters.AddWithValue("@IsPreOrder", orderBotData.IsPreOrder);

//                    command.Parameters.AddWithValue("@HtmlPrint", htmlOrderD);
//                    connection.Open();
//                    command.ExecuteNonQuery();

//                    connection.Close();
//                }



//                var totalWithBranchCost = (orderBotData.Total + decimal.Parse(orderBotData.BranchCost));

//                var ListString = "------------------ \r\n\r\n";

//                ListString = ListString + captionOrderInfoText;
//                ListString = ListString + captionOrderNumberText + number + "\r\n";
//                ListString = ListString + captionBranchCostText + orderBotData.BranchCost + "\r\n";
//                ListString = ListString + captionFromLocationText + orderBotData.Address + "\r\n";
//                ListString = ListString + captionTotalOfAllOrderText + totalWithBranchCost + "\r\n\r\n";


//                ListString = ListString + "------------------ \r\n\r\n";
//                ListString = ListString + captionEndOrderText;



//                Order order = new Order
//                {
//                    SelectDay = orderBotData.SelectDay,
//                    SelectTime = orderBotData.SelectTime,
//                    IsPreOrder = orderBotData.IsPreOrder,
//                    // SpecialRequestText = orderBotData.SpecialRequest,
//                    AfterDeliveryCost = decimal.Parse(orderBotData.AfterBranchCost),
//                    BranchAreaName = BranchAreaName,
//                    BranchAreaId = orderBotData.BranchAreaId,
//                    Address = orderBotData.Address,
//                    BranchId = orderBotData.BranchID,
//                    ContactId = orderBotData.ContactId,
//                    OrderTime = timeAdd,
//                    CreationTime = timeAdd,
//                    Id = orderBotData.Id,
//                    OrderNumber = number,
//                    TenantId = orderBotData.TenantID,
//                    orderStatus = OrderStatusEunm.Pending,
//                    OrderType = OrderTypeEunm.Delivery,
//                    Total = totalWithBranchCost,
//                    IsDeleted = false,
//                    AgentId = agId,
//                    IsLockByAgent = false,
//                    LocationID = orderBotData.LocationID,
//                    FromLocationDescribation = "https://maps.google.com/?q=" + orderBotData.LocationFrom,
//                    HtmlPrint = htmlOrderD
//                };

//                if (orderBotData.IsPreOrder)
//                {
//                    order.orderStatus = OrderStatusEunm.Pre_Order;
//                }


//                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//                getOrderForViewDto.Order = GetOrderMap;
//                getOrderForViewDto.OrderStatusName = orderStatusName;
//                getOrderForViewDto.OrderTypeName = orderTypeName;

//                getOrderForViewDto.BranchAreaName = BranchAreaName;

//                getOrderForViewDto.IsAssginToAllUser = area.IsAssginToAllUser;
//                getOrderForViewDto.IsAvailableBranch = area.IsAvailableBranch;
//                getOrderForViewDto.TenantId = orderBotData.TenantID;
//                //await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);




//                con.TotalOrder = con.TotalOrder + 1;
//                con.DeliveryOrder = con.DeliveryOrder + 1;
//                con.Description = orderBotData.Address;

//                var contact = _dbService.UpdateCustomerLocation(con).Result;

//                contact.customerChat = null;
//               // await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", contact);

//                //delete bot conversation
//                // DeleteConversation(usertodelete.SunshineConversationId);
//                return ListString;


//            }
//            else
//            {
//                var area = GetAreas(orderBotData.TenantID.ToString()).Where(x => x.Id == orderBotData.AreaID).FirstOrDefault();

//                ///**************//

//                string htmlOrder = GetHtmlPrint(orderBotData, timeAdd, number, con, area, orderBotData.BotLocal);



//                ///**************//



//                var captionAreaNameText = GetCaptionFormat("BackEnd_Text_AreaName", orderBotData.BotLocal, orderBotData.TenantID, "", "", 0);//من الفرع :
//                using (SqlConnection connection = new SqlConnection(connString))
//                using (SqlCommand command = connection.CreateCommand())
//                {


//                    command.CommandText = "UPDATE Orders SET OrderTime = @OrT ,CreationTime =@CreT , OrderStatus = @OrI , OrderType = @OrTy ,AreaId = @ArI ,IsEvaluation=@IsEv , OrderNumber =@OrderNumber , HtmlPrint =@HtmlPrint Where Id = @Id";

//                    command.Parameters.AddWithValue("@Id", orderBotData.Id);
//                    command.Parameters.AddWithValue("@OrT", timeAdd);
//                    command.Parameters.AddWithValue("@CreT", timeAdd);
//                    command.Parameters.AddWithValue("@ArI", orderBotData.AreaID);
//                    command.Parameters.AddWithValue("@OrTy", OrderTypeEunm.Takeaway);
//                    command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Pending);
//                    command.Parameters.AddWithValue("@IsEv", false);
//                    command.Parameters.AddWithValue("@OrderNumber", number);
//                    command.Parameters.AddWithValue("@HtmlPrint", htmlOrder);
//                    connection.Open();
//                    command.ExecuteNonQuery();
//                    connection.Close();
//                }

//                var ListString = "------------------ \r\n\r\n";

//                ListString = ListString + captionOrderInfoText;
//                ListString = ListString + captionOrderNumberText + number + "\r\n";
//                ListString = ListString + captionAreaNameText + orderBotData.AreaName + "\r\n";
//                ListString = ListString + captionTotalOfAllOrderText + orderBotData.Total + "\r\n";

//                ListString = ListString + "------------------ \r\n\r\n";
//                ListString = ListString + captionEndOrderText;




//                long agId = 0;

//                //Notification
//                if (orderBotData.AreaID != 0)
//                {


//                    if (area != null)
//                    {
//                        var message = "The order AssignTo :" + area.AreaName + "(" + area.AreaCoordinate + ")";
//                        UserNotification Notification = await SendNotfAsync(message, Convert.ToInt64(area.UserId), orderBotData.TenantID);
//                        agId = Convert.ToInt64(area.UserId);
//                    }


//                }

//                Order order = new Order
//                {
//                    HtmlPrint = htmlOrder,
//                    SpecialRequestText = orderBotData.SpecialRequest,
//                    AreaId = orderBotData.AreaID,
//                    ContactId = orderBotData.ContactId,
//                    OrderTime = timeAdd,
//                    CreationTime = timeAdd,
//                    Id = orderBotData.Id,
//                    OrderNumber = number,
//                    TenantId = orderBotData.TenantID,
//                    orderStatus = OrderStatusEunm.Pending,
//                    OrderType = OrderTypeEunm.Takeaway,
//                    Total = orderBotData.Total,
//                    IsDeleted = false,
//                    AgentId = agId,
//                    IsLockByAgent = false,
//                };

//                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//                getOrderForViewDto.Order = GetOrderMap;
//                getOrderForViewDto.AreahName = orderBotData.AreaName;
//                getOrderForViewDto.OrderStatusName = orderStatusName;
//                getOrderForViewDto.OrderTypeName = orderTypeName;

//                getOrderForViewDto.IsAssginToAllUser = area.IsAssginToAllUser;
//                getOrderForViewDto.IsAvailableBranch = area.IsAvailableBranch;
//                getOrderForViewDto.TenantId = orderBotData.TenantID;
//                getOrderForViewDto.CustomerCustomerName = con.DisplayName;
//                //await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);

//                //delete bot conversation
//                //  DeleteConversation(usertodelete.SunshineConversationId);



//                con.TotalOrder = con.TotalOrder + 1;
//                con.TakeAwayOrder = con.TakeAwayOrder + 1;

//                var contact = _dbService.UpdateCustomerLocation(con).Result;

//                return ListString;

//            }


//        }

//        private string GetHtmlD(OrderBotData orderBotData, DateTime timeAdd, long number, Contact con , string BranchAreaName, string local)
//        {
//            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.Id).Result;
//            var header = "";
//            if (orderBotData.IsPreOrder)
//            {
//                header = "<div > <strong  style='font-size: large;'> Name : </strong>"  + con.DisplayName  + "</div>"
//             + "<div > <strong  style='font-size: large;'> Mobile : </strong>"  + con.PhoneNumber  + "</div>"
//             + "<div > <strong style='font-size: large;' > Address : </strong>"  + orderBotData.Address  + "</div>"
//             + "<div > <strong  style='font-size: large;' > Time : </strong>"  + timeAdd.ToString("MM/dd hh:mm tt")  + "</div>"
//             + "<div >  <strong  style='font-size: large;' > Type : </strong>"  + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Delivery) +"/  "+ orderBotData.SelectTime + "-" + orderBotData.SelectDay  + "</div>"
//             + "<div > <strong style='font-size: large;'> Branch : </strong>"  + orderBotData.RustrantBranchAreaName + "/" + BranchAreaName  + "</div>"
//             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>"  + number  + "</div>"
//             + "<hr  style='border-top: dotted 1px #000 !important;'>";

//            }
//            else
//            {
//                header = "<div > <strong  style='font-size: large;'> Name : </strong>" + con.DisplayName + "</div>"
//             + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber + "</div>"
//             + "<div > <strong style='font-size: large;' > Address : </strong>" + orderBotData.Address + "</div>"
//             + "<div > <strong  style='font-size: large;' > Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt") + "</div>"
//             + "<div > <strong style='font-size: large;'> Branch : </strong>" + orderBotData.RustrantBranchAreaName + "/" + BranchAreaName + "</div>"
//             + "<div >  <strong  style='font-size: large;'> Order # :  </strong>" + number + "</div>"
//             + "<hr  style='border-top: dotted 1px #000 !important;'>";
//            }


//            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";


//            foreach (var record in orderDetailsList)
//            {

//                if (local == "ar-JO")
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName  + "</strong></td>";
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

//            orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> Delivery fees   : </strong> <strong style='font-size: medium;'>" + orderBotData.BranchCost + " </strong> </div>";

//            if (orderBotData.AfterBranchCost == "0")
//            {
//                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium'  > <strong style='font-size: x-large;'> instead of   : </strong> <strong style='font-size: medium;'>" + orderBotData.AfterBranchCost + " </strong> </div>";
//            }


//            if (orderBotData.SpecialRequest != "NULLNOT" || orderBotData.SpecialRequest != "NULLORDER")
//            {
//                orderDetailsbody = orderDetailsbody + "<div style='color: black;font-size: medium;'  > <strong style='font-size: x-large;'> Note   : </strong> <strong style='font-size: medium;'>" + orderBotData.SpecialRequest + " </strong> </div>";
//            }
//            orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + ((orderBotData.Total + decimal.Parse(orderBotData.BranchCost))) + " </p> </div>";

//            var htmlOrderD = header + orderDetailsbody;
//            return htmlOrderD;
//        }

//        private string GetHtmlPrint(OrderBotData orderBotData, DateTime timeAdd, long number, Contact con, Area area, string local)
//        {
//            var orderDetailsList = GetAllDetail(orderBotData.TenantID, orderBotData.Id).Result;

//            var header = "<div > <strong  style='font-size:large;'> Name : </strong>"  + con.DisplayName + "</div>"
//               + "<div > <strong  style='font-size: large;'> Mobile : </strong>" + con.PhoneNumber +  "</div>"
//               + "<div > <strong  style='font-size: large;'> Time : </strong>" + timeAdd.ToString("MM/dd hh:mm tt")  + "</div>"
//               + "<div >  <strong  style='font-size: large;'> Type : </strong>"  + Enum.GetName(typeof(OrderTypeEunm), OrderTypeEunm.Takeaway)  + "</div>"
//               + "<div > <strong  style='font-size: large;'> Branch : </strong>"  + area.AreaName + "/" + area.AreaCoordinate  + "</div>"
//               + "<div >  <strong  style='font-size: large;'> Order # :  </strong>"  + number  + "</div>"
//               + "<hr  style='border-top: dotted 1px #000 !important;'><hr  style='border-top: dotted 1px #000 !important;'>";

//            var orderDetailsbody = "<div> <table style='border-collapse: collapse;'><tbody>";

        

//            foreach (var record in orderDetailsList)
//            {
//                if (local == "ar-JO")
//                {
//                    orderDetailsbody = orderDetailsbody + "<tr style='border-top: thin solid;'><td style='text-align: center; width: 100px; '> <strong>" + record.ItemName  + "</strong></td>";
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
//                    extraOrderDetails = extraOrderDetails + "<tr ><td style='text-align: center; width: 100px; font-size: smaller; '>" + group.Name +"</td>";
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
//            orderDetailsbody = orderDetailsbody + "<div class='center' style='text-align: center;' ><label style='font-size: x-large;'>Total </label>  <p style='font-size: x-large;'>" + orderBotData.Total + " </p> </div>";

//            var htmlOrder = header + orderDetailsbody;
//            return htmlOrder;
//        }

//        [Route("UpdateDeliveryOrder")]
//        [HttpPost]
//        public async Task<string> UpdateDeliveryOrderAsync([FromBody] OrderBotData orderBotData)
//        {
//            var time = DateTime.Now;

//            var timeAdd = time.AddHours(3);
//            string connString = AppSettingsModel.ConnectionStrings;

//            int modified = 0;
//            long number = 0;

//            lock (CurrentOrder)
//            {
//                number = UpateOrder(orderBotData.TenantID);

//            }


//            var captionOrderInfoText = GetCaptionFormat("BackEnd_Text_OrderInfo", "ar-JO", orderBotData.TenantID, "", "", 0);//شكرا لك معلومات الطلب \r\n
//            var captionOrderNumberText = GetCaptionFormat("BackEnd_Text_OrderNumber", "ar-JO", orderBotData.TenantID, "", "", 0);//رقم الطلب :
//            var captionTotalOfAllOrderText = GetCaptionFormat("BackEnd_Text_TotalOfAllOrder", "ar-JO", orderBotData.TenantID, "", "", 0);//مجموع القيم :
//            var captionEndOrderText = GetCaptionFormat("BackEnd_Text_EndOrder", "ar-JO", orderBotData.TenantID, "", "", 0);//سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم  \r\n
        

//                var captionBranchCostText = GetCaptionFormat("BackEnd_Text_BranchCost", "ar-JO", orderBotData.TenantID, "", "", 0);//قيمة التوصيل :
//                var captionFromLocationText = GetCaptionFormat("BackEnd_Text_FromLocation", "ar-JO", orderBotData.TenantID, "", "", 0);//من الموقع :

//                using (SqlConnection connection = new SqlConnection(connString))
//                using (SqlCommand command = connection.CreateCommand())
//                {
              
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
//                 DeliveryCost= decimal.Parse(orderBotData.BranchCost),
//                  DeliveryCostString= orderBotData.BranchCost,
//                   FromAddress= orderBotData.FromAddress,
//                    FromLocationId= orderBotData.FromLocationID,
//                     FromGoogleURL= "https://maps.google.com/?q="+ orderBotData.LocationFrom,
//                      TenantId= orderBotData.TenantID,
//                       ToAddress= orderBotData.ToAddress,
//                        ToLocationId= orderBotData.ToLocationID,
//                         ToGoogleURL = "https://maps.google.com/?q=" + orderBotData.LocationTo,
//                          OrderId = modified,
                          



//            };


//            Add(deliveryOrderDetailsDto);



//                var ListString = "------------------ \r\n\r\n";

//                ListString = ListString + captionOrderInfoText;
//                ListString = ListString + captionOrderNumberText + number + "\r\n";
//                ListString = ListString + captionBranchCostText + orderBotData.BranchCost + "\r\n";
//                ListString = ListString + "من الموقع :" + orderBotData.FromAddress + "\r\n";
//                ListString = ListString + "الى الموقع :" + orderBotData.ToAddress + "\r\n\r\n";


//            ListString = ListString + "------------------ \r\n\r\n";
//                ListString = ListString + captionEndOrderText;



//                Order order = new Order
//                {
//                    BranchAreaName = "BranchAreaName",
//                    BranchAreaId = orderBotData.BranchAreaId,
//                    Address = orderBotData.Address,
//                    BranchId = orderBotData.BranchID,
//                    ContactId = orderBotData.ContactId,
//                    OrderTime = timeAdd,
//                    CreationTime = timeAdd,
//                    Id = orderBotData.Id,
//                    OrderNumber = number,
//                    TenantId = orderBotData.TenantID,
//                    orderStatus = OrderStatusEunm.Pending,
//                    OrderType = OrderTypeEunm.Delivery,
//                    Total = decimal.Parse(orderBotData.BranchCost),
//                    IsDeleted = false,
//                   // AgentId = agId,
//                    IsLockByAgent = false,
//                    LocationID = orderBotData.LocationID
//                };

//                var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);
//                var orderTypeName = Enum.GetName(typeof(OrderTypeEunm), order.OrderType);

//                GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//                getOrderForViewDto.Order = GetOrderMap;
//                getOrderForViewDto.OrderStatusName = orderStatusName;
//                getOrderForViewDto.OrderTypeName = orderTypeName;

//              getOrderForViewDto.BranchAreaName = "BranchAreaName";

//                getOrderForViewDto.IsAssginToAllUser = true; //area.IsAssginToAllUser;
//                getOrderForViewDto.IsAvailableBranch = true;// area.IsAvailableBranch;
//                getOrderForViewDto.TenantId = orderBotData.TenantID;
//                //await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);

//                //delete bot conversation
//                // DeleteConversation(usertodelete.SunshineConversationId);
//                return ListString;


            
         


//        }

//        //*
//        [Route("DeleteOrderDraft")]
//        [HttpPost]
//        public async void DeleteOrderDraft(int? TenantID,int ContactId)
//        {

//            var orderDrift = GetOrderList(TenantID).Result;
//            var orderDrift2 = orderDrift.Where(x => x.ContactId == ContactId && x.orderStatus == OrderStatusEunm.Draft).ToList();


//            foreach (var order in orderDrift2)
//            {
//                var orderDetailsDrft = GetOrderDetail2(TenantID, order.Id);

//                foreach(var orderDetai in orderDetailsDrft)
//                {
//                    var GetOrderDetailExtraDraft = GetOrderDetailExtra(TenantID, orderDetai.Id);

//                    foreach(var ExtraOrde in GetOrderDetailExtraDraft)
//                    {

//                        DeleteExtraOrderDetail(ExtraOrde.Id);
//                    }

//                    DeleteOrderDetails(orderDetai.Id);
//                }

//                DeleteOrder(order.Id);
//            }
         

//        }

//        //*
//        [Route("CancelUpdateOrder")]
//        [HttpPost]
//        public async Task<CancelOrderModel> CancelUpdateOrderAsync(int? TenantID, string OrderNumber, int ContactId)
//        {

//            int n;
//            bool isNumeric = int.TryParse(OrderNumber, out n);
//            if (!isNumeric)
//            {
//                CancelOrderModel cancelOrderModel = new CancelOrderModel();

//                cancelOrderModel.CancelOrder = false;
//                cancelOrderModel.WrongOrder = true;
//                cancelOrderModel.IsTrueOrder = false;
//                return cancelOrderModel;

//            }

//            var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
//            bool iscancel = false;
//            int cancelTime = 0;

//            string EnglishNumbers = "";
//            var arabicnumbers = OrderNumber.ToString();
//            for (int i = 0; i < arabicnumbers.Length; i++)
//            {
//                EnglishNumbers += char.GetNumericValue(arabicnumbers, i);
//            }
//            int convertednumber = Convert.ToInt32(EnglishNumbers);

//            var ordList = GetOrderListWithContact(TenantID, ContactId).Result;

//            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantID);
//            if (tenant != null)
//            {

//                if (tenant.IsCancelOrder)
//                {
//                    iscancel = tenant.IsCancelOrder;
//                    cancelTime = tenant.CancelTime;

//                    var creationtimeorder = ordList.Where(x => x.OrderNumber== convertednumber).FirstOrDefault().CreationTime;
//                    if (creationtimeorder != null)
//                    {
//                        TimeSpan timeSpan = DateTime.Now.AddHours(3) - creationtimeorder;
//                        int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);


//                        var captionText = GetCaptionFormat("BackEnd_Text_SorryNotSupport", "ar-JO", TenantID, "", "", 0);

//                        if (totalMinutes > tenant.CancelTime)
//                        {
//                            CancelOrderModel cancelOrderModel = new CancelOrderModel();

//                            cancelOrderModel.CancelOrder = true;
//                            cancelOrderModel.WrongOrder = false;
//                            cancelOrderModel.IsTrueOrder = false;
//                            cancelOrderModel.TextCancelOrder = captionText;
//                            return cancelOrderModel;
//                        }

//                    }

                   


//                }


//            }

           
//            try
//            {
//                var ord = ordList.Where(x => x.OrderNumber == convertednumber).FirstOrDefault();

//                if(ord==null)
//                {
//                    CancelOrderModel cancelOrderModel = new CancelOrderModel();

//                    cancelOrderModel.CancelOrder = false;
//                    cancelOrderModel.WrongOrder = true;
//                    cancelOrderModel.IsTrueOrder = false;
//                    return cancelOrderModel;

//                }
//                if (ord.OrderNumber == convertednumber)
//                {
//                    string connString = AppSettingsModel.ConnectionStrings;
//                    using (SqlConnection connection = new SqlConnection(connString))
//                    using (SqlCommand command = connection.CreateCommand())
//                    {
//                        command.CommandText = "UPDATE Orders SET  OrderStatus = @OrI, OrderRemarks=@Rema  Where Id = @Id";
//                        command.Parameters.AddWithValue("@Id", ord.Id);
//                        command.Parameters.AddWithValue("@OrI", OrderStatusEunm.Canceled);
//                        command.Parameters.AddWithValue("@Rema", "CancelByCustomer");

//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }




//                    Order order = new Order
//                    {

//                        ContactId = ContactId,
//                        OrderNumber = convertednumber,
//                        orderStatus = OrderStatusEunm.Canceled,
//                    };

//                    var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), order.orderStatus);

//                    GetOrderForViewDto getOrderForViewDto = new GetOrderForViewDto();
//                    var GetOrderMap = ObjectMapper.Map(order, getOrderForViewDto.Order);

//                    getOrderForViewDto.Order = GetOrderMap;
//                    getOrderForViewDto.OrderStatusName = orderStatusName;
//                    getOrderForViewDto.DeliveryChangeDeliveryServiceProvider = ord.CreationTime.ToString("MM/dd hh:mm tt");
//                   // await _hub.Clients.All.SendAsync("brodCastBotOrder", getOrderForViewDto);

//                    CancelOrderModel cancelOrderModel = new CancelOrderModel();

//                    cancelOrderModel.CancelOrder = false;
//                    cancelOrderModel.WrongOrder = false;
//                    cancelOrderModel.IsTrueOrder = true;
//                    return cancelOrderModel;
//                }
//                else
//                {
//                    CancelOrderModel cancelOrderModel = new CancelOrderModel();

//                    cancelOrderModel.CancelOrder = false;
//                    cancelOrderModel.WrongOrder = true;
//                    cancelOrderModel.IsTrueOrder = false;
//                    return cancelOrderModel;
//                }

//            }
//            catch
//            {

//                CancelOrderModel cancelOrderModel = new CancelOrderModel();
//                cancelOrderModel.CancelOrder = false;
//                cancelOrderModel.WrongOrder = true;
//                return cancelOrderModel;
//            }
           
           

//        }
           
//        //*
//        [Route("CreateEvaluations")]
//        [HttpGet]
//        public async Task<string> CreateEvaluationsAsync(int TenantId,string phoneNumber, string displayName, string EvaluationsText,string orderID, string EvaluationsReat)
//        {
//            var time = DateTime.Now;

//            var timeAdd = time.AddHours(3);
//            Evaluation evaluation = new Evaluation
//            {

//                 ContactName= displayName,
//                  CreationTime= timeAdd,
//                   EvaluationsReat= EvaluationsReat,
//                   EvaluationsText = EvaluationsText.Replace("File upload:", "").Trim(),
//                     OrderNumber=int.Parse(orderID),
//                       PhoneNumber= phoneNumber,
//                        TenantId= TenantId

//            };
//           await  _evaluationRepository.InsertAsync(evaluation);

//            await _evaluationHub.Clients.All.SendAsync("brodCastBotEvaluation", evaluation);

//            return "";
//        }


//        [Route("UpdateSpecialRequest")]
//        [HttpGet]
//        public async Task<string> UpdateSpecialRequest(string SpecialRequestText, string orderID)
//        {

//            SpecialRequestText = SpecialRequestText.Replace("File upload:", "").Trim();
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//            using (SqlCommand command = connection.CreateCommand())
//            {


//                command.CommandText = "UPDATE Orders SET IsSpecialRequest=@IsSpecialRequest , SpecialRequestText =@SpecialRequestText Where Id = @Id";

//                command.Parameters.AddWithValue("@Id", int.Parse(orderID));
//                command.Parameters.AddWithValue("@IsSpecialRequest", true);
//                command.Parameters.AddWithValue("@SpecialRequestText", SpecialRequestText);

//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
      

//            return "";
//        }


//        //*
//        [Route("GetCaptionFormat")]
//        [HttpGet]
//        public string GetCaptionFormat(string key, string lang, int? TenantID,string parm1, string parm2, int parm3)
//        {
//            var LocalID = 1;

//            if (lang == "ar-JO")
//            {
//                LocalID = 1;
//            }
//            else
//            {
//                LocalID = 2;
//            }

//            if(lang==null)
//                lang = "ar-JO";
//            var langg = GetLanguageBot(TenantID, lang);
//            var testR = GetTextResource(TenantID, key);
//            var caption = GetAllCaption(TenantID, langg.Id, testR.Id);

//            if (caption != null && testR.Category == "Welcome_Text")
//            {
//                var text = string.Format(caption.Text, parm1.Trim());

//                return text.Replace("\\r\\n", "\r\n");
//            }
//            if (caption != null && testR.Category == "PlaceOrder_Text")
//            {
//                var text = string.Format(caption.Text, TenantID.ToString(), parm1.Trim(), parm2.Trim(), parm3, LocalID);

//                return text.Replace("\\r\\n", "\r\n");
//            }

//            if (caption != null && testR.Category == "CancelOrder_SeccesMassege")
//            {
//                var text = string.Format(caption.Text, parm1.ToString(), parm2.Trim());

//                return text.Replace("\\r\\n", "\r\n");
//            }

//            if (caption != null && testR.Category == "NextBranch_Text")
//            {
//                var text = string.Format(caption.Text, parm1.ToString());

//                return text.Replace("\\r\\n", "\r\n");
//            }
//            if (caption != null && testR.Category == "Select_Text")
//            {
//                var text = string.Format(caption.Text, parm1.ToString());

//                return text.Replace("\\r\\n", "\r\n");
//            }

//            if (caption != null)
//            {

//                return caption.Text.Replace("\\r\\n", "\r\n");
//            }

//            return "";
//        }

//        //*
//        [Route("GetPhoneNumber")]
//        [HttpGet]
//        public string GetPhoneNumber(int? TenantID)
//        {
//            var tenantEditDto = GetAllTenant(TenantID);
//            return tenantEditDto.PhoneNumber;
//        }

//        //*
//        [Route("UpdateComplaint")]
//        [HttpGet]
//        public async Task<CustomerModel> UpdateComplaintAsync(int contactId)
//        {
//            var con = GetContact(contactId);

//            var result = await _dbService.UpdateComplaint(con.UserId, 0,true);
//            if (result != null)
//            {
//                result.customerChat = null;
//             //   await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", result);              
//            }

//            return result;
//        }

//        [Route("UpdateSupport")]
//        [HttpGet]
//        public async Task<CustomerModel> UpdateSupportAsync(int contactId)
//        {
//            var con = GetContact(contactId);
//            var resultClosed = await _dbService.UpdateIsSupport(con.UserId, false, "Need Support", true);

//            if (resultClosed != null)
//            {
//                resultClosed.IsSupport = true;
//               // await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", resultClosed);
//            }

//            return resultClosed;

//        }


//        [Route("GetBranchDetail")]
//        [HttpGet]
//        public List<Branch> GetBranchDetail(string TenantID)
//        {
//            //TenantID = "31";
//            var tenantID = int.Parse(TenantID);
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Branches] where TenantID=" + tenantID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            List<Branch> branches = new List<Branch>();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                branches.Add(new Branch
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    DeliveryCost = decimal.Parse(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString()),
//                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),




//                });
//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }

//        [Route("SplitString")]
//        [HttpGet]
//        public Branch SplitString(string text)
//        {
//            try
//            {
//                var split = text.Split(";").ToList();
//                Branch branch = new Branch();

//                branch.DeliveryCost = decimal.Parse(split[0]);
//                branch.Name = split[1];
//                branch.Id = long.Parse(split[2]);
//                branch.TenantId = int.Parse(split[3]);


//                return branch;

//            }
//            catch
//            {

//                return null;
//            }


//        }


//        [Route("UpdateOrderNumber")]
//        [HttpGet]
//        public async Task<string> UpdateOrderNumberAsync(int tenantId)
//        {

//            lock (CurrentOrder)
//            {
//                long orderNumber = UpateOrder(tenantId);

//                return orderNumber.ToString();
//            }

//        }

//        [Route("GetCaption")]
//        [HttpGet]
//        public string GetCaption(string key, string lang, int? TenantID)
//        {
//            var langg = GetLanguageBot(TenantID, lang);
//            var testR = GetTextResource(TenantID, key);
//            var caption = GetAllCaption(TenantID, langg.Id, testR.Id);

//            if (caption != null)
//            {
//                return caption.Text.Replace("\\r\\n", "\r\n");
//            }

//            return "";
//        }

//        [Route("GetBranchID")]
//        [HttpGet]
//        public Branch GetBranchID(string TenantID, string BranchName)
//        {

//            var list = GetBranchDetail(TenantID);

//            var branch = list.Where(x => x.Name == BranchName).FirstOrDefault();

//            if (branch == null)
//            {

//                return null;
//            }

//            return branch;

//        }


//        #region private
//        private List<Area> GetAreas(string TenantID)
//        {      
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

//                branches.Add(new Area
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                    AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                    BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                    UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                    RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                    IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                    IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString())

//                });
//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }
//        private Area GetAreas2(int id)
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

//              Area branches = new Area();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                branches=new Area
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    AreaCoordinate = dataSet.Tables[0].Rows[i]["AreaCoordinate"].ToString(),
//                    AreaName = dataSet.Tables[0].Rows[i]["AreaName"].ToString(),
//                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                    BranchID = dataSet.Tables[0].Rows[i]["BranchID"].ToString(),
//                    UserId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["UserId"]),
//                    RestaurantsType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
//                    IsAssginToAllUser = bool.Parse(dataSet.Tables[0].Rows[i]["IsAssginToAllUser"].ToString()),
//                    IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString())

//                };
//            }

//            conn.Close();
//            da.Dispose();

//            return branches;
//        }
//        private async Task<List<Order>> GetOrderList(int? TenantID)
//        {


//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID;


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

//                order.Add(new Order
//                {
//                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                    OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
//                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                    ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
//                    CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
//                    OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),
//                    orderStatus = (OrderStatusEunm)Enum.Parse(typeof(OrderStatusEunm), dataSet.Tables[0].Rows[i]["orderStatus"].ToString(), true),

//                });

               

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }
//        private async Task<List<Order>> GetOrderListWithContact(int? TenantID, int ContactId)
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
                    

//                    order.Add(new Order
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        OrderNumber = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderNumber"]),
//                        Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                        ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString()),
//                        CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
//                        OrderTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderTime"].ToString()),

//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }

//        private List<OrderOffers.OrderOffer> GetOrderOffer(int? TenantID)
//        {

//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[OrderOffer] where TenantID=" + TenantID ;


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

//                    order.Add( new OrderOffers.OrderOffer
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                         Area = dataSet.Tables[0].Rows[i]["Area"].ToString(),
//                         Cities = dataSet.Tables[0].Rows[i]["Cities"].ToString(),
//                        FeesStart = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesStart"].ToString()),
//                         FeesEnd = decimal.Parse(dataSet.Tables[0].Rows[i]["FeesEnd"].ToString()),
//                         NewFees = decimal.Parse(dataSet.Tables[0].Rows[i]["NewFees"].ToString()),
//                           TenantId = int.Parse(dataSet.Tables[0].Rows[i]["TenantId"].ToString()),
//                        isAvailable = bool.Parse(dataSet.Tables[0].Rows[i]["isAvailable"].ToString()),
//                         OrderOfferDateEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateEnd"].ToString()),
//                         OrderOfferDateStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferDateStart"].ToString()),
//                         OrderOfferEnd = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferEnd"].ToString()),
//                         OrderOfferStart = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["OrderOfferStart"].ToString())



//                    });

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

//        }
//        private Order GetOrderS(int? TenantID, int? OrderId)
//        {

//            //var x = GetContactId("962779746365", "28");


//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Orders] where TenantID=" + TenantID + "and id=" + OrderId;


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
//                        ContactId = int.Parse(dataSet.Tables[0].Rows[i]["ContactId"].ToString())

//                    };

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return order;

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
//        private List<OrderDetailDto> GetOrderDetail2(int? TenantID, long? OrderId)
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
//        private List<ExtraOrderDetailsDto> GetOrderDetailExtra(int? TenantID, long? OrderDetailId)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[ExtraOrderDetail] where TenantID=" + TenantID + " and OrderDetailId=" + OrderDetailId;


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
//                    Total = decimal.Parse(dataSet.Tables[0].Rows[i]["Total"].ToString()),
//                    UnitPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["UnitPrice"].ToString()),



//                });
//            }

//            conn.Close();
//            da.Dispose();

//            return orderDetailDtos;
//        }
//        private async Task<List<GetOrderDetailForViewDto>> GetAllDetail(int? TenantId,int orderId)
//        {

//            List<GetOrderDetailForViewDto> getOrderDetailForViewDto = new List<GetOrderDetailForViewDto>();
//            var OrderDetail = GetOrderDetail(TenantId, orderId);
         
//            foreach (var item in OrderDetail)
//            {
//                var items = GetItem(TenantId, item.ItemId);
//                getOrderDetailForViewDto.Add(new GetOrderDetailForViewDto { 
                
//                 OrderDetail= item,
//                   ItemName = items.ItemName ,
//                    ItemNameEnglish = items.ItemNameEnglish
//                });
               


//            }


         



//            foreach (var item in getOrderDetailForViewDto)
//            {

//                var exList = GetOrderDetailExtra(TenantId, item.OrderDetail.Id);

//                List<ExtraOrderDetailsDto> extraOrderDetailsDto = new List<ExtraOrderDetailsDto>();
//                foreach (var itemex in exList)
//                {
//                    extraOrderDetailsDto.Add(new ExtraOrderDetailsDto
//                    {
//                        Id = 0,
//                        Name = itemex.Name,
//                        OrderDetailId = itemex.OrderDetailId,
//                        Quantity = itemex.Quantity,
//                        TenantId = itemex.TenantId,
//                        Total = itemex.Total,
//                        UnitPrice = itemex.UnitPrice,

//                    });

//                }

//                item.OrderDetail.extraOrderDetailsDtos = extraOrderDetailsDto;

//            }




//            return getOrderDetailForViewDto;

//        }
//        private ItemDto GetItem(int? TenantID, long? itemID)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + "and id=" + itemID;


//            SqlConnection conn = new SqlConnection(connString);
//            SqlCommand cmd = new SqlCommand(query, conn);
//            conn.Open();

//            // create the DataSet 
//            DataSet dataSet = new DataSet();

//            // create data adapter
//            SqlDataAdapter da = new SqlDataAdapter(cmd);
//            // this will query your database and return the result to your datatable
//            da.Fill(dataSet);

//            ItemDto itemDtos = new ItemDto();

//            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//            {

//                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

//                if (!IsDeleted)
//                {
//                    itemDtos = new ItemDto
//                    {
//                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                         CategoryNames  = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
//                         CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),
//                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
//                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
//                        IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
//                         ItemDescription  = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
//                         ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),
//                         ItemName  = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
//                         ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
//                        Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
//                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"])

//                    };

//                }

//            }

//            conn.Close();
//            da.Dispose();

//            return itemDtos;

//        }
//        private GoogleMapModel GetLocation(string query)
//        {
//            var client = new HttpClient();
//            string Key =  _configuration["GoogleMapsKey:KeyMap"];
//            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={query}&key="+ Key;
//            var response = client.GetAsync(url).Result;

//            var result = response.Content.ReadAsStringAsync().Result;
//            var resultO = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleMapModel>(result);
//            return resultO;
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

//                    locationInfoModel.Add(new LocationInfoModel
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        LocationName = dataSet.Tables[0].Rows[i]["LocationName"].ToString(),
//                        LocationNameEn = dataSet.Tables[0].Rows[i]["LocationNameEn"].ToString().TrimEnd(),
//                        LevelId=Convert.ToInt32(dataSet.Tables[0].Rows[i]["LevelId"]),
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
//        private List<LocationInfoModel> GetAllLocationDeliveryCost(int TenantId)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[LocationDeliveryCost]  where TenantId="+ TenantId;


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
//        private List<Models.Location.DeliveryLocationCost> GetAllDeliveryLocationCost(int TenantId)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[DeliveryLocationCost] where TenantId = "+ TenantId;


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
//                         FromLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["FromLocationId"]),
//                          ToLocationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ToLocationId"]),
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
//        private Caption GetAllCaption(int? TenantID, int langId, int testRId)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + langId + " and TextResourceId=" + testRId;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                Caption captions = new Caption();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    captions = new Caption
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
//                        LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
//                        TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

//                    };
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
//        private LanguageBot GetLanguageBot(int? TenantID, string lang)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[LanguageBot] where ISO ='" + lang + "'";


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                LanguageBot languageBots = new LanguageBot();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    languageBots = new LanguageBot
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        ISO = dataSet.Tables[0].Rows[i]["ISO"].ToString(),
//                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

//                    };
//                }

//                conn.Close();
//                da.Dispose();

//                return languageBots;

//            }
//            catch
//            {
//                return null;

//            }

//        }
//        private TextResource GetTextResource(int? TenantID, string key)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[TextResource] where Category = '" + key + "'";


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                TextResource textResources = new TextResource();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    textResources = new TextResource
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        Key = dataSet.Tables[0].Rows[i]["Key"].ToString(),
//                        Category = dataSet.Tables[0].Rows[i]["Category"].ToString(),
//                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

//                    };
//                }

//                conn.Close();
//                da.Dispose();

//                return textResources;

//            }
//            catch (Exception ex)
//            {
//                return null;

//            }

//        }
//        private MultiTenancy.Dto.TenantEditDto GetAllTenant(int? TenantID)
//        {

//            try
//            {
//                string connString = AppSettingsModel.ConnectionStrings;
//                string query = "select * from [dbo].[AbpTenants] where Id=" + TenantID;


//                SqlConnection conn = new SqlConnection(connString);
//                SqlCommand cmd = new SqlCommand(query, conn);
//                conn.Open();

//                // create the DataSet 
//                DataSet dataSet = new DataSet();

//                // create data adapter
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                // this will query your database and return the result to your datatable
//                da.Fill(dataSet);

//                MultiTenancy.Dto.TenantEditDto tenant = new MultiTenancy.Dto.TenantEditDto();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    tenant = new MultiTenancy.Dto.TenantEditDto
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),

//                    };
//                }

//                conn.Close();
//                da.Dispose();

//                return tenant;

//            }
//            catch
//            {
//                return null;

//            }

//        }
//        private Contact GetContact(int id)
//        {

//            try
//            {
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

//                Contact contact = new Contact();

//                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
//                {

//                    contact = new Contact
//                    {
//                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
//                        UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(), 
//                         PhoneNumber= dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),
//                        Description = dataSet.Tables[0].Rows[i]["Description"].ToString(),
//                         EmailAddress = dataSet.Tables[0].Rows[i]["EmailAddress"].ToString(),
//                        DisplayName=dataSet.Tables[0].Rows[i]["DisplayName"].ToString(),
//                         TotalOrder= Convert.ToInt32(dataSet.Tables[0].Rows[i]["TotalOrder"]),
//                          TakeAwayOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TakeAwayOrder"]),
//                           DeliveryOrder = Convert.ToInt32(dataSet.Tables[0].Rows[i]["DeliveryOrder"]),
//                            loyalityPoint = Convert.ToInt32(dataSet.Tables[0].Rows[i]["loyalityPoint"])

//                    };
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
     

//        #endregion
//    }
//}

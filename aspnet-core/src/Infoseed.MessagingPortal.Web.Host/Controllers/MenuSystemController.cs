using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.ExtraOrderDetails;
using Infoseed.MessagingPortal.ItemAdditions;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAdditionsCategorys;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.ItemAndAdditionsCategorys;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.ItemSpecificationsDetails;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.MenuCategories.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.OrderDetails;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models.Menu;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using InfoSeedAzureFunction;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuSystemController : MessagingPortalControllerBase
    {
        private IRepository<Order, long> _orderRepository;
        private IRepository<OrderDetail, long> _orderDetailRepository;
        private IRepository<ExtraOrderDetail, long> _extraOrderDetailRepository;
        private IDBService _dbService;
       // private IHubContext<SignalR.TeamInboxHub> _hub;
        private TelemetryClient _telemetry;
        private IConfiguration _configuration;
        private IMenusAppService _iMenusAppService;
        private IItemsAppService _IItemsAppService;
        private IItemAdditionAppService _IItemAdditionAppService;
        private IMenuCategoriesAppService _IMenuCategoriesAppService;
        private IOrdersAppService _IOrdersAppService;
        private readonly IDocumentClient _IDocumentClient;

        public MenuSystemController(
            IConfiguration configuration,
             TelemetryClient telemetry,
          //  IHubContext<SignalR.TeamInboxHub> hub,
            IDBService dbService,
            IRepository<Order, long> orderRepository,
            IRepository<OrderDetail, long> orderDetailRepository,
            IRepository<ExtraOrderDetail, long> extraOrderDetailRepository,
            IMenusAppService IMenusAppService,
            IItemsAppService itemsAppService,
            IItemAdditionAppService iItemAdditionAppService,
            IMenuCategoriesAppService iMenuCategoriesAppService,
            IOrdersAppService iOrdersAppService
            ,IDocumentClient iDocumentClient
            )
        {
            _configuration = configuration;
            _telemetry = telemetry;
          //  _hub = hub;
            _dbService = dbService;
            _extraOrderDetailRepository = extraOrderDetailRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _iMenusAppService = IMenusAppService;
            _IItemsAppService = itemsAppService;
            _IItemAdditionAppService = iItemAdditionAppService;
            _IMenuCategoriesAppService = iMenuCategoriesAppService;
            _IOrdersAppService = iOrdersAppService;
            _IDocumentClient = iDocumentClient;
        }



        [Route("GetInfoTenant")]
        [HttpGet]
        public GetInfoTenantModel GetInfoTenant(int? TenantID, int menu, int LanguageBotId)
        {
            GetInfoTenantModel getInfoTenantModel = new GetInfoTenantModel();
            //   var cat = GetItemCategories2(TenantID, menu, LanguageBotId).Where(x=>x.logoImag!=null).FirstOrDefault();

            var ten = GetAllTenant(TenantID);

            if (ten != null)
            {
                getInfoTenantModel.BgImag = ten.ImageBg;
                getInfoTenantModel.LogoImag = ten.Image;
                getInfoTenantModel.Name = ten.Name;
                getInfoTenantModel.NameEnglish = ten.Name;

                getInfoTenantModel.CurrencyCode = TenantID == 46 ? "IQD" : "JOD";
                return getInfoTenantModel;

            }
            else
            {
                getInfoTenantModel.BgImag = "https://www.wmadaat.com/upload/white/white1.jpg";
                getInfoTenantModel.LogoImag = "https://www.wmadaat.com/upload/white/white1.jpg";
                getInfoTenantModel.Name = "";
                getInfoTenantModel.NameEnglish = "";
                return getInfoTenantModel;
            }







        }

        [Route("GetMenu")]
        [HttpGet]
        public List<GetMenuModel> GetMenu(int TenantID, int MenuType)
        {

            List<GetMenuModel> ListMenu = new List<GetMenuModel>();

            var objMenus = _iMenusAppService.GetMenusWithDetails(TenantID, MenuType);

            if (objMenus != null)
            {
                foreach (var menuItem in objMenus)
                {
                    GetMenuModel model = new GetMenuModel();
                    model.Id = menuItem.Id;
                    model.MenuName = menuItem.MenuName;
                    model.MenuNameEnglish = menuItem.MenuNameEnglish;
                    model.Priority = menuItem.Priority;
                    model.ImageUri = menuItem.ImageUri;
                    model.isInService = checkIsInService(menuItem.SettingJson);

                    model.getCategorysModels = fillGetCategorysModel(menuItem.CategoryEntity);
                    ListMenu.Add(model);
                }
            }



            return ListMenu;
        }


        [Route("GetAllItems")]
        [HttpGet]
        public List<ItemDto> GetAllItems(int tenantID, int menuType, long itemSubCategoryId, int languageBotId = 1, int pageNumber = 0, int pageSize = 10)
        {
            int totalCount = 0;
            List<ItemDto> itemDtos = _IItemsAppService.GetItemsBySubGategory(tenantID, menuType, itemSubCategoryId, languageBotId, pageNumber, pageSize ,out totalCount);
            foreach (var item in itemDtos)
            {
                if (item.CreationTime == DateTime.MinValue)
                    item.CreationTime = DateTime.Now;

                if (item.DateFrom == DateTime.MinValue)
                    item.DateFrom = null;

                if (item.DeletionTime == DateTime.MinValue)
                    item.DeletionTime = null;

                if (item.LastModificationTime == DateTime.MinValue)
                    item.LastModificationTime = null;


                int count = 1;
                foreach (var Specifications in item.ItemSpecifications)
                {
                    Specifications.UniqueId = count;

                    int count2 = 1;
                    foreach (var s in Specifications.SpecificationChoices)
                    {

                        s.UniqueId = count2;
                        count2++;
                    }
                    count++;
                }

            }


            return itemDtos;
        }



        [Route("GetItemById")]
        [HttpGet]
        public ItemDto GetItemById(long id, int tenantId)
        {

            ItemDto itemDtos = _IItemsAppService.GetItemById(id, tenantId);

            if (itemDtos != null)
            {
                if (itemDtos.IsQuantitative)
                {
                    if (itemDtos.IsQuantitative)
                    {
                        var itemAupdate = AddExtra(itemDtos);

                        itemDtos.ItemSpecifications = itemAupdate;
                        itemDtos.ViewPrice = 0;
                    }
                }
            }

            return itemDtos;
        }






        [Route("GetCondiments")]
        [HttpGet]
        public async Task<List<ItemAdditionDto>> GetCondiments(int tenantID, int menuType)
        {
            List<ItemAdditionDto> itemAdditionDto = await _IItemAdditionAppService.GetCondiments(tenantID, menuType);
            return itemAdditionDto;
        }





        [Route("GetCrispy")]
        [HttpGet]
        public async Task<List<ItemAdditionDto>> GetCrispy(int tenantID, int menuType)
        {
            List<ItemAdditionDto> itemAdditionDto = await _IItemAdditionAppService.GetCrispy(tenantID, menuType);
            return itemAdditionDto;
        }


        [Route("GetDeserts")]
        [HttpGet]
        public async Task<List<ItemAdditionDto>> GetDeserts(int tenantID, int menuType)
        {
            List<ItemAdditionDto> itemAdditionDto = await _IItemAdditionAppService.GetDeserts(tenantID, menuType);
            return itemAdditionDto;
        }



        [Route("CreateOrder")]
        [HttpPost]
        public async Task<string> CreateOrder(CreateOrderModel createOrderModel)
        {
            try
            {



                // reads Max+1 from DB on startup
                Order order = new Order
                {
                    TenantId = createOrderModel.TenantId,
                    ContactId = createOrderModel.CustomerId,
                    Total = createOrderModel.Total,
                    IsDeleted = false,
                    OrderTime = DateTime.Now,
                    OrderNumber = new Random().Next(0001, 9999),//OrdersCount + 1, //new Random().Next(1000, 9999),
                    CreationTime = DateTime.Now,
                    AgentId = -1,
                    IsLockByAgent = false,
                    orderStatus = OrderStatusEunm.Draft,
                    IsEvaluation = false


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
                  await   InsertMethod(createOrderModel, null, orderId);
                }
                catch (Exception ex)
                {
                    this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);

                }


              //  this._telemetry.TrackTrace("Create Order with id :" + order.OrderNumber.ToString() + " and the ContactId is : " + createOrderModel.CustomerId.ToString() + " and time is : " + order.CreationTime.ToString(), SeverityLevel.Information);


                // await AddNewOrder(createOrderModel);

                await SendToBotAsync(createOrderModel.CustomerId.Value, createOrderModel.TenantId.Value);
                return order.OrderNumber.ToString();

            }
            catch (Exception ex)
            {
                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;
            }

        }

        //[Route("SendToBot")]
        //[HttpGet]
        private async Task SendToBotAsync(int CustomerId, int tenantId)
        {


            try
            {
                //var contact = GetCustomer(CustomerId);
               // var user = GetCustomerAzuer(contact.UserId);
                //  var tenant = GetTenantByAppIdD360(user.D360Key).Result;
                var user = GetCustomerAzuerById(CustomerId.ToString());
                var tenant = await _dbService.GetTenantInfoById(tenantId);

                //var Tenant = await _dbService.GetTenantInfoById(createOrderModel.TenantId.Value);
                if (tenant != null && !string.IsNullOrEmpty(tenant.AccessToken))
                {
                    var result = await sendToWhatsApp(tenant, user);
                }


                //DirectLineConnector directLineConnector = new DirectLineConnector();
                //var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(user.D360Key, tenant.DirectLineSecret, user.userId, tenant.botId).Result;
                //var Bot = directLineConnector.StartBotConversationD360(user.userId, CustomerId.ToString(), micosoftConversationID.MicrosoftBotId, "اختبار", tenant.DirectLineSecret, tenant.botId, user.phoneNumber, user.TenantId.ToString(), user.displayName, tenant.PhoneNumber, tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null).Result;

                //List<Activity> botListMessages = new List<Activity>();

                //foreach (var msgBot in Bot)
                //{

                //    if (msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
                //    {


                //    }
                //    else
                //    {
                //        botListMessages.Add(msgBot);
                //    }


                //}
                //foreach (var msgBot in botListMessages)
                //{

                //    SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model();


                //    if (msgBot.SuggestedActions == null)
                //    {
                //        sendWhatsAppD360Model = new SendWhatsAppD360Model
                //        {
                //            to = user.phoneNumber,
                //            type = "text",
                //            text = new SendWhatsAppD360Model.Text
                //            {
                //                body = msgBot.Text

                //            }

                //        };
                //    }
                //    else
                //    {

                //        List<SendWhatsAppD360Model.Button> buttons = new List<SendWhatsAppD360Model.Button>();
                //        foreach (var button in msgBot.SuggestedActions.Actions)
                //        {
                //            buttons.Add(new SendWhatsAppD360Model.Button
                //            {
                //                reply = new SendWhatsAppD360Model.Reply { id = button.Title, title = button.Title },
                //                type = "reply"
                //            });

                //        }

                //        if (msgBot.Summary == null)
                //        {

                //            if (msgBot.Text.Length <= 1000)
                //            {
                //                sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                {
                //                    to = user.phoneNumber,
                //                    type = "interactive",
                //                    interactive = new SendWhatsAppD360Model.Interactive
                //                    {
                //                        type = "button",
                //                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //                        body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                //                        // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //                        action = new SendWhatsAppD360Model.Action
                //                        {
                //                            buttons = buttons.ToArray()

                //                        }



                //                    }

                //                };
                //            }
                //            else
                //            {
                //                sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                {
                //                    to = user.phoneNumber,
                //                    type = "text",
                //                    text = new SendWhatsAppD360Model.Text
                //                    {
                //                        body = msgBot.Text

                //                    }

                //                };

                //                var result22 = WhatsAppDialogConnector.PostMsgToSmooch(user.D360Key, sendWhatsAppD360Model, _telemetry).Result;

                //                //update Bot massage in cosmoDB 

                //                if (result22 == HttpStatusCode.Created)
                //                {
                //                    Content message = contentM(msgBot, tenant.botId);
                //                    updatewatermarkD360(user.userId);
                //                    var CustomerChat = _dbService.UpdateCustomerChatD360(user.TenantId, message, user.userId, "");
                //                    user.CreateDate = CustomerChat.CreateDate;
                //                    user.customerChat = CustomerChat;
                //                    user.IsNewContact = false;
                //                    user.IsNew = false;
                //                   // await _hub.Clients.All.SendAsync("brodCastEndUserMessage", user);
                //                    SocketIOManager.SendContact(user, user.TenantId.Value);
                //                }


                //                sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                {
                //                    to = user.phoneNumber,
                //                    type = "interactive",
                //                    interactive = new SendWhatsAppD360Model.Interactive
                //                    {
                //                        type = "button",
                //                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //                        body = new SendWhatsAppD360Model.Body { text = msgBot.Summary },
                //                        // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //                        action = new SendWhatsAppD360Model.Action
                //                        {
                //                            buttons = buttons.ToArray()

                //                        }



                //                    }

                //                };



                //            }

                //        }
                //        else
                //        {

                //            if (msgBot.Summary.Contains("هل تريد") || msgBot.Summary.Contains("Do you want"))
                //            {

                //                if (msgBot.Text.Length <= 1000)
                //                {
                //                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                    {
                //                        to = user.phoneNumber,
                //                        type = "interactive",
                //                        interactive = new SendWhatsAppD360Model.Interactive
                //                        {
                //                            type = "button",
                //                            // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //                            body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                //                            footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //                            action = new SendWhatsAppD360Model.Action
                //                            {
                //                                buttons = buttons.ToArray()

                //                            }



                //                        }

                //                    };
                //                }
                //                else
                //                {
                //                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                    {
                //                        to = user.phoneNumber,
                //                        type = "text",
                //                        text = new SendWhatsAppD360Model.Text
                //                        {
                //                            body = msgBot.Text

                //                        }

                //                    };

                //                    var result22 = WhatsAppDialogConnector.PostMsgToSmooch(user.D360Key, sendWhatsAppD360Model, _telemetry).Result;

                //                    //update Bot massage in cosmoDB 

                //                    if (result22 == HttpStatusCode.Created)
                //                    {

                //                        Content message = contentM(msgBot, tenant.botId);
                //                        updatewatermarkD360(user.userId);
                //                        var CustomerChat = _dbService.UpdateCustomerChatD360(user.TenantId, message, user.userId, "");
                //                        user.CreateDate = CustomerChat.CreateDate;
                //                        user.customerChat = CustomerChat;
                //                        user.IsNewContact = false;
                //                        user.IsNew = false;
                //                     //   await _hub.Clients.All.SendAsync("brodCastEndUserMessage", user);
                //                        SocketIOManager.SendContact(user, user.TenantId.Value);
                //                    }


                //                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                    {
                //                        to = user.phoneNumber,
                //                        type = "interactive",
                //                        interactive = new SendWhatsAppD360Model.Interactive
                //                        {
                //                            type = "button",
                //                            // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //                            body = new SendWhatsAppD360Model.Body { text = msgBot.Summary },
                //                            // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //                            action = new SendWhatsAppD360Model.Action
                //                            {
                //                                buttons = buttons.ToArray()

                //                            }



                //                        }

                //                    };






                //                }



                //            }
                //            else
                //            {

                //                sendWhatsAppD360Model = new SendWhatsAppD360Model
                //                {
                //                    to = user.phoneNumber,
                //                    type = "interactive",
                //                    interactive = new SendWhatsAppD360Model.Interactive
                //                    {
                //                        type = "button",
                //                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //                        body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                //                        footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //                        action = new SendWhatsAppD360Model.Action
                //                        {
                //                            buttons = buttons.ToArray()

                //                        }



                //                    }

                //                };




                //            }



                //            //sendWhatsAppD360Model = new SendWhatsAppD360Model
                //            //{
                //            //    to = user.phoneNumber,
                //            //    type = "interactive",
                //            //    interactive = new SendWhatsAppD360Model.Interactive
                //            //    {
                //            //        type = "button",
                //            //        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                //            //        body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                //            //        footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                //            //        action = new SendWhatsAppD360Model.Action
                //            //        {
                //            //            buttons = buttons.ToArray()

                //            //        }



                //            //    }

                //            //};
                //        }

                //    }

                //    var result = await WhatsAppDialogConnector.PostMsgToSmooch(user.D360Key, sendWhatsAppD360Model, _telemetry);

                //    if (result == HttpStatusCode.Created)
                //    {
                //        Content message = contentM(msgBot, tenant.botId);
                //        this._telemetry.TrackTrace(" send message to bot after created order ,the user is :" + user.displayName + " and the massge is :" + Bot.FirstOrDefault().Text, SeverityLevel.Information);

                //        updatewatermarkD360(user.userId);
                //        var CustomerChat = _dbService.UpdateCustomerChatD360(user.TenantId, message, user.userId, "");
                //        user.CreateDate = CustomerChat.CreateDate;
                //        user.customerChat = CustomerChat;
                //        user.IsNewContact = false;
                //        user.IsNew = false;
                //      //  await _hub.Clients.All.SendAsync("brodCastEndUserMessage", user);
                //        SocketIOManager.SendContact(user, user.TenantId.Value);
                //    }
                //}


            }
            catch (Exception ex)
            {
                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
            }

        }

        [Route("GetAllWithTenantID")]
        [HttpGet]
        public List<MenuCategoryDto> GetAllWithTenantID(int? TenantID, int menu, int LanguageBotId)
        {

            if (LanguageBotId == 0)
                LanguageBotId = 1;
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + "and MenuType=" + menu;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!IsDeleted)
                    {
                        itemCategories.Add(new MenuCategoryDto
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                            NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),

                        });

                    }

                }

                conn.Close();
                da.Dispose();


                this._telemetry.TrackTrace(" Item Categorys for Tenant ID : " + TenantID.ToString() + "and the list is :" + itemCategories.OrderBy(x => x.Priority).ToList(), SeverityLevel.Information);
                return itemCategories.OrderBy(x => x.Priority).ToList();

            }
            catch (Exception ex)
            {

                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;
            }


        }

        [Route("GetMenuSystem")]
        [HttpGet]
        public List<CategorysInItemModel> GetMenuSystem(int TenantId, int MenuType)
        {

            try
            {
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

                var CategorieList = MenuCategories(TenantId, MenuType).OrderBy(x => x.Priority).ToList();
                //var listitem = ItemsList(TenantId, MenuType).Where(x => x.ItemCategoryId == Categorie.Id).OrderBy(x => x.Priority).ToList();
                var listitems = ItemsList(TenantId, MenuType).ToList();
                foreach (var Categorie in CategorieList)
                {

                    var listitemss = listitems.Where(x => x.ItemCategoryId == Categorie.Id).OrderBy(x => x.Priority).ToList();

                    foreach (var item in listitemss)
                    {
                        var ListItemAddition = AdditionList(TenantId, item.Id).ToArray();

                        item.itemAdditionDtos = ListItemAddition;
                    }




                    var menu = MenuList(TenantId, MenuType).OrderBy(x => x.Priority).ToList();



                    if (listitemss != null && listitemss.Count > 0)
                    {
                        categorysInItemModel.Add(new CategorysInItemModel
                        {
                            //  bgImg=
                            menuPriority = menu.Where(x => x.Id == listitemss.FirstOrDefault().MenuId).FirstOrDefault().Priority,
                            menuId = listitemss.FirstOrDefault().MenuId,
                            categoryId = Categorie.Id,
                            categoryName = Categorie.Name,
                            categoryNameEnglish = Categorie.NameEnglish,
                            listItemInCategories = listitemss

                        });
                    }

                }

              //  this._telemetry.TrackTrace(" Item Categorys for Tenant ID : " + TenantId.ToString() + "and the list item  is :" + categorysInItemModel.OrderBy(x => x.menuPriority).ToList(), SeverityLevel.Information);
                return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();
            }
            catch (Exception ex)
            {
                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;

            }




        }


        [Route("GetAllCategoryAndSubCategory")]
        [HttpGet]
        public List<MenuCategoryWithSubDto> GetAllCategoryAndSubCategory(int? TenantID, int menu)
        {
            List<MenuCategoryWithSubDto> categorysInItemModels = new List<MenuCategoryWithSubDto>();
            var Category = GetAllCategor(TenantID, menu);
            var subCategory = GetAllSubCategor();

            foreach (var cat in Category)
            {
                var subCat = subCategory.Where(x => x.ItemCategoryId == cat.Id).ToList();
                cat.menuSubCategoryDtos = subCat;

                if (subCat.Count() > 0)
                    cat.isSubCategory = true;
                else
                    cat.isSubCategory = false;

            }



            return Category;

        }

        [Route("GetMenuSubCategorys")]
        [HttpGet]
        public SubCategorysInItemModel GetMenuSubCategorys(int TenantId, int MenuType, long CategoriesID, long SubCategoryId, int PageSize, int PageNumber, string Search = "", int IsSort = 0)
        {

            int? OrderByPrice = null;
            int? OrderByDiscount = null;
            int IsDescOrder = 0;

            SubCategorysInItemModel subCategorysInItemModel = new SubCategorysInItemModel();

            try
            {

                if (Search != "")
                {
                    var listitem2 = GetItemsPageList(TenantId, MenuType, SubCategoryId, PageSize, PageNumber, Search, OrderByPrice, OrderByDiscount, IsDescOrder, IsSort).ToList();
                    if (listitem2 != null && listitem2.Count() > 0)
                    {

                        var CategorieList = GetItemCategoriesPage(TenantId, MenuType, listitem2.FirstOrDefault().ItemCategoryId).FirstOrDefault();

                        //     var subCategorieList = GetSubItemCategories(TenantId, MenuType).Where(x => x.Id == listitem2.FirstOrDefault().ItemSubCategoryId).FirstOrDefault();

                        subCategorysInItemModel.categoryId = CategorieList.Id;
                        //   subCategorysInItemModel.categoryName = subCategorieList.Name;
                        //   subCategorysInItemModel.categoryNameEnglish = subCategorieList.NameEnglish;
                        subCategorysInItemModel.listItemInCategories = listitem2;//.OrderBy(x => x.Priority).ToList();
                        subCategorysInItemModel.subcategoryId = listitem2.FirstOrDefault().ItemSubCategoryId;
                        subCategorysInItemModel.menuPriority = CategorieList.Priority;
                        subCategorysInItemModel.menuId = CategorieList.Id;
                        //subCategorysInItemModel.itemCount= GetItemCount(SubCategoryId);

                    }




                }
                else
                {
                    var CategorieList = GetItemCategoriesPage(TenantId, MenuType, CategoriesID).FirstOrDefault();

                    var listitem2 = GetItemsPageList(TenantId, MenuType, SubCategoryId, PageSize, PageNumber, Search).ToList();

                    //  var subCategorieList = GetSubItemCategories(TenantId, MenuType).Where(x => x.Id == SubCategoryId).FirstOrDefault();

                    subCategorysInItemModel.categoryId = CategorieList.Id;
                    // subCategorysInItemModel.categoryName = subCategorieList.Name;
                    //  subCategorysInItemModel.categoryNameEnglish = subCategorieList.NameEnglish;
                    subCategorysInItemModel.listItemInCategories = listitem2;//.OrderBy(x => x.Priority).ToList();
                    subCategorysInItemModel.subcategoryId = SubCategoryId;
                    subCategorysInItemModel.menuPriority = CategorieList.Priority;
                    subCategorysInItemModel.menuId = CategorieList.Id;
                    //subCategorysInItemModel.itemCount = GetItemCount(SubCategoryId);


                }



                return subCategorysInItemModel;
            }
            catch
            {

                return subCategorysInItemModel;
            }


        }

        [Route("GetMenuSubCategorys2")]
        [HttpGet]
        public List<CategorysInItemModel> GetMenuSubCategorys2(int TenantId, int MenuType, int CategoriesID, int SubCategoryId, int PageSize, int PageNumber, string Search = "")
        {
            List<CategorysInItemModel> categorysInItemModels = new List<CategorysInItemModel>();
            SubCategorysInItemModel subCategorysInItemModel = new SubCategorysInItemModel();

            try
            {

                if (Search != "")
                {
                    var listitem2 = GetItemsPageList(TenantId, MenuType, SubCategoryId, PageSize, PageNumber, Search).ToList();
                    if (listitem2 != null && listitem2.Count() > 0)
                    {
                        var CategorieList = GetItemCategoriesPage(TenantId, MenuType, listitem2.FirstOrDefault().ItemCategoryId).FirstOrDefault();

                        var subCategorieList = GetSubItemCategories(TenantId, MenuType).Where(x => x.Id == listitem2.FirstOrDefault().ItemSubCategoryId).FirstOrDefault();

                        subCategorysInItemModel.categoryId = CategorieList.Id;
                        subCategorysInItemModel.categoryName = subCategorieList.Name;
                        subCategorysInItemModel.categoryNameEnglish = subCategorieList.NameEnglish;
                        subCategorysInItemModel.listItemInCategories = listitem2;//.OrderBy(x => x.Priority).ToList();
                        subCategorysInItemModel.subcategoryId = listitem2.FirstOrDefault().ItemSubCategoryId;
                        subCategorysInItemModel.menuPriority = CategorieList.Priority;
                        subCategorysInItemModel.menuId = CategorieList.Id;

                    }




                }
                else
                {
                    var CategorieList = GetItemCategoriesPage(TenantId, MenuType, CategoriesID).FirstOrDefault();

                    var listitem2 = GetItemsPageList(TenantId, MenuType, SubCategoryId, PageSize, PageNumber, Search).ToList();

                    var subCategorieList = GetSubItemCategories(TenantId, MenuType).Where(x => x.Id == SubCategoryId).FirstOrDefault();

                    subCategorysInItemModel.categoryId = CategorieList.Id;
                    subCategorysInItemModel.categoryName = subCategorieList.Name;
                    subCategorysInItemModel.categoryNameEnglish = subCategorieList.NameEnglish;
                    subCategorysInItemModel.listItemInCategories = listitem2;//.OrderBy(x => x.Priority).ToList();
                    subCategorysInItemModel.subcategoryId = SubCategoryId;
                    subCategorysInItemModel.menuPriority = CategorieList.Priority;
                    subCategorysInItemModel.menuId = CategorieList.Id;


                }
                CategorysInItemModel categorysInItemModel = new CategorysInItemModel
                {

                    subCategorysInItemModels = new List<SubCategorysInItemModel>()

                };

                categorysInItemModel.subCategorysInItemModels.Add(subCategorysInItemModel);

                categorysInItemModels.Add(categorysInItemModel);


                return categorysInItemModels;
            }
            catch
            {
                CategorysInItemModel categorysInItemModel = new CategorysInItemModel();

                categorysInItemModel.subCategorysInItemModels.Add(subCategorysInItemModel);

                categorysInItemModels.Add(categorysInItemModel);
                return categorysInItemModels;
            }


        }



        #region private

        private async Task<bool> sendToWhatsApp(TenantModel Tenant, CustomerModel user)
        {
            
            //  DirectLineConnector directLineConnector = new DirectLineConnector();
            //  var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
            // var Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments).Result;

            //var contact = GetCustomer(CustomerId);
            
            //var user = GetCustomerAzuerById(contactId.ToString());
            string from = user.phoneNumber;
            string userId = Tenant.id+"_"+user.phoneNumber;
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
                        // await _hub.Clients.All.SendAsync("brodCastAgentMessage", user);
                        SocketIOManager.SendChat(user, user.TenantId.Value);
                    }
                }
               
            }

            return true;

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

        private List<MenuCategoryWithSubDto> GetAllCategor(int? TenantID, int menu)
        {


            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + "and MenuType=" + menu;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<MenuCategoryWithSubDto> itemCategories = new List<MenuCategoryWithSubDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!IsDeleted)
                    {
                        itemCategories.Add(new MenuCategoryWithSubDto
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                            NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),

                        });

                    }

                }

                conn.Close();
                da.Dispose();


                this._telemetry.TrackTrace(" Item Categorys for Tenant ID : " + TenantID.ToString() + "and the list is :" + itemCategories.OrderBy(x => x.Priority).ToList(), SeverityLevel.Information);
                return itemCategories.OrderBy(x => x.Priority).ToList();

            }
            catch (Exception ex)
            {

                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;
            }


        }
        private List<MenuSubCategoryDto> GetAllSubCategor()
        {


            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[ItemSubCategories] ";


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    itemCategories.Add(new MenuSubCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),

                    });



                }

                conn.Close();
                da.Dispose();



                return itemCategories.OrderBy(x => x.Priority).ToList();

            }
            catch (Exception ex)
            {

                this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;
            }


        }
        private List<ItemDto> GetItemsPageList(int? TenantID, long menu, long ItemSubCategoryId, int PageSize = 20, int PageNumber = 0, string Search = "", int? OrderByPrice = null, int? OrderByDiscount = null, int IsDescOrder = 0, int IsSort = 0)
        {

            if (PageSize == 0)
            {
                PageSize = 20;
            }

            if (TenantID == 34)
            {
                string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


                if (IsSort == 1)//اقل  سعر
                {

                    OrderByPrice = 1;
                    IsDescOrder = 0;
                }
                if (IsSort == 2)//اعلى سعر
                {

                    OrderByPrice = 1;
                    IsDescOrder = 1;
                }


                if (IsSort == 3)//اقل  سعر خصم
                {

                    OrderByDiscount = 1;
                    IsDescOrder = 0;
                }
                if (IsSort == 4)//اعلى سعر خصم
                {

                    OrderByDiscount = 1;
                    IsDescOrder = 1;
                }






                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                           new System.Data.SqlClient.SqlParameter("@MenuType", menu),
                           new System.Data.SqlClient.SqlParameter("@PageNumber", PageNumber),
                           new System.Data.SqlClient.SqlParameter("@PageSize", PageSize),
                           new System.Data.SqlClient.SqlParameter("@OrderByPrice", OrderByPrice),
                           new System.Data.SqlClient.SqlParameter("@OrderByDiscount", OrderByDiscount),
                           new System.Data.SqlClient.SqlParameter("@IsDescOrder", IsDescOrder)

                };



                if (Search != "" && Search != null && Search != "null")
                {
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@Search", Search));
                    //if (ItemSubCategoryId == 342 || ItemSubCategoryId == 343 || ItemSubCategoryId == 346 || ItemSubCategoryId == 347 || ItemSubCategoryId == 344 || ItemSubCategoryId == 345)
                    //{



                    //   // sqlParameters.Add(new SqlParameter("@ItemSubCategoryId", ItemSubCategoryId));
                    //    sqlParameters.Add(new SqlParameter("@Search", Search));
                    //    sqlParameters.Add(new SqlParameter("@IsOffer", 1));


                    //    // query = "SELECT * from  [dbo].[Items] where  ( (Status_Code is null or Status_Code=2) and  MenuType = " + menu + "  and DateFrom is not null and DateTo >=DATEADD(day, DATEDIFF(day,0,GETDATE())-1,0)  and ( CONTAINS(ItemName,  N'" + Search.Replace(" ", " AND ") + " ')   Or   CONTAINS(ItemNameEnglish,  N'" + Search.Replace(" ", " AND ") + " ') ) )";

                    //}
                    //else
                    //{
                    //    sqlParameters.Add(new SqlParameter("@Search", Search));
                    //    sqlParameters.Add(new SqlParameter("@IsOffer", 0));
                    //    // sqlParameters.Add(new SqlParameter("@ItemSubCategoryId", ItemSubCategoryId));
                    //    // query = "SELECT * from  [dbo].[Items] where  ( (Status_Code is null or Status_Code=2) and  MenuType = " + menu + " and ( CONTAINS(ItemName,  N'" + Search.Replace(" ", " AND ") + " ')   Or   CONTAINS(ItemNameEnglish,  N'" + Search.Replace(" ", " AND ") + " ') ) )";
                    //}



                }
                else
                {




                    if (ItemSubCategoryId == 342 || ItemSubCategoryId == 343 || ItemSubCategoryId == 346 || ItemSubCategoryId == 347 || ItemSubCategoryId == 344 || ItemSubCategoryId == 345)
                    {
                        // sqlParameters.Add(new SqlParameter("@ItemSubCategoryId", ItemSubCategoryId));
                        sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@IsOffer", true));
                        // command.Parameters.Add(new SqlParameter("@Search", Search));
                        //  query = "select * from [dbo].[Items] where (Status_Code is null or Status_Code=2) and MenuType = " + menu + " and DateFrom is not null and DateTo >=DATEADD(day, DATEDIFF(day,0,GETDATE())-1,0)    ORDER BY Priority  OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";

                    }
                    else
                    {
                        sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@IsOffer", false));
                        sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@ItemSubCategoryId", ItemSubCategoryId));
                        // query = "select * from [dbo].[Items] where (Status_Code is null or Status_Code=2) and MenuType = " + menu + " AND ItemSubCategoryId = " + ItemSubCategoryId + " and ImageUri is not null and  ImageUri != ''   ORDER BY Priority   OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";
                    }

                }

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);

                IList<ItemDto> result =
                              SqlDataHelper.ExecuteReader(
                                  "[ctownjo].[ItemCtownSearch]",
                                  sqlParameters.ToArray(),
                                  MapItems, connString);


                return result.ToList();

            }
            else
            {
                var det = GetItemSpecificationsDetailList((int)TenantID);
                string connString = AppSettingsModel.ConnectionStrings; //"Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                string query = String.Empty;
                if (ItemSubCategoryId > 0)
                    query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + menu + " and ItemSubCategoryId = " + ItemSubCategoryId + " and IsDeleted = " + 0;
                else
                    query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + menu + " and IsDeleted = " + 0;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);




                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();




                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<ItemDto> itemDtos = new List<ItemDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    DataSet dataSetItemSpecification = new DataSet();
                    SqlCommand cmdItemSpecifications = new SqlCommand();
                    cmdItemSpecifications.Connection = conn;
                    cmdItemSpecifications.CommandType = CommandType.StoredProcedure;
                    cmdItemSpecifications.CommandText = "dbo.ItemSpecificationsGet";
                    var _itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);



                    cmdItemSpecifications.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ItemId", _itemId));

                    SqlDataAdapter daItemSpectification = new SqlDataAdapter(cmdItemSpecifications);

                    daItemSpectification.Fill(dataSetItemSpecification);


                    var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!IsDeleted)
                    {
                        List<SpecificationChoice> choices = new List<SpecificationChoice>();

                        var itm = new ItemDto();

                        itm = new ItemDto
                        {

                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            IsQuantitative = bool.Parse(dataSet.Tables[0].Rows[i]["IsQuantitative"].ToString()),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        };

                        itm.CreationTime = DateTime.Now;


                        //Add Specifications
                        for (int x = 0; x < dataSetItemSpecification.Tables[0].Rows.Count; x++)
                        {
                            var itmSpecChoice = new SpecificationChoice
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["Id"]),
                                SpecificationChoiceDescription = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescription"].ToString(),
                                SpecificationId = Convert.ToInt32((dataSetItemSpecification.Tables[0].Rows[x]["SpecificationId"].ToString())),
                                Price = decimal.Parse(dataSetItemSpecification.Tables[0].Rows[x]["Price"].ToString()),


                                SpecificationChoiceDescriptionEnglish = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescriptionEnglish"].ToString(),
                                LanguageBotId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["LanguageBotId"].ToString()),
                                SKU = dataSetItemSpecification.Tables[0].Rows[x]["SKU"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["TenantId"].ToString()),

                            };
                            var fund = det.Where(x => x.SpecificationChoicesId == itmSpecChoice.Id && x.MenuType == itm.MenuType).FirstOrDefault();
                            if (fund != null)
                            {
                                itmSpecChoice.IsInService = fund.IsInService;
                                choices.Add(itmSpecChoice);
                            }
                        }

                        List<ItemSpecification> ItemSpecifications12 = new List<ItemSpecification>();
                        for (int j = 0; j < dataSetItemSpecification.Tables[1].Rows.Count; j++)
                        {
                            var _id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]);
                            var itmSpec = new ItemSpecification
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]),
                                SpecificationDescription = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescription"].ToString(),
                                IsMultipleSelection = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsMultipleSelection"].ToString()),
                                IsRequired = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsRequired"].ToString()),
                                SpecificationChoices = choices.Where(p => p.SpecificationId == _id).ToList(),
                                MaxSelectNumber = int.Parse(dataSetItemSpecification.Tables[1].Rows[j]["MaxSelectNumber"].ToString()),

                                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                                Priority = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Priority"]),
                                SpecificationDescriptionEnglish = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescriptionEnglish"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["TenantId"])



                            };
                            //Add Choices

                            ItemSpecifications12.Add(itmSpec);
                        }
                        itm.ItemSpecifications = ItemSpecifications12.OrderBy(x => x.Priority).ToList();
                        daItemSpectification.Dispose();

                        //audai TOdo
                        if (itm.IsQuantitative)
                        {
                            var itemAupdate = AddExtra(itm);

                            itm.ItemSpecifications = itemAupdate;
                            itm.ViewPrice = 0;
                        }


                        itemDtos.Add(itm);

                    }



                }

                conn.Close();
                // cmdItemSpecifications.Clone();
                // da.Dispose();


                return itemDtos;

            }



        }


        private List<ItemSpecification> AddExtra(ItemDto itm)
        {
            List<SpecificationChoice> Listchoices = new List<SpecificationChoice>();
            SpecificationChoice choices1 = new SpecificationChoice
            {
                TenantId = itm.TenantId,
                Id = 1192,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.25")),
                SpecificationChoiceDescription = "0.25 kg",
                SpecificationChoiceDescriptionEnglish = "0.25 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices2 = new SpecificationChoice
            {
                TenantId = itm.TenantId,
                Id = 1193,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.50")),
                SpecificationChoiceDescription = "0.50 kg",
                SpecificationChoiceDescriptionEnglish = "0.50 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices3 = new SpecificationChoice
            {
                TenantId = itm.TenantId,
                Id = 1194,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("0.75")),
                SpecificationChoiceDescription = "0.75 kg",
                SpecificationChoiceDescriptionEnglish = "0.75 kg",
                SpecificationId = 292,
                SKU = ""

            };
            SpecificationChoice choices4 = new SpecificationChoice
            {
                TenantId = itm.TenantId,
                Id = 1195,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itm.Price * decimal.Parse("1.0")),
                SpecificationChoiceDescription = "1.00 kg",
                SpecificationChoiceDescriptionEnglish = "1.00 kg",
                SpecificationId = 292,
                SKU = ""

            };

            Listchoices.Add(choices1);
            Listchoices.Add(choices2);
            Listchoices.Add(choices3);
            Listchoices.Add(choices4);

            List<ItemSpecification> ItemSpecificationsList = new List<ItemSpecification>();

            ItemSpecification itemSpecification = new ItemSpecification
            {
                Id = 292,
                SpecificationDescription = "الرجاء اختيار الوزن ",
                IsMultipleSelection = false,
                IsRequired = true,
                SpecificationChoices = Listchoices,
                MaxSelectNumber = 0,
                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                Priority = 0,
                SpecificationDescriptionEnglish = "Please select a weight",
                TenantId = itm.TenantId,


            };


            ItemSpecificationsList.Add(itemSpecification);



            return ItemSpecificationsList;
        }


        private Content contentM(Activity msgBot, string botId)
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
        private static void ForeachFun(List<CreateOrderDetailsModel> createOrderDetailsModels, List<CreateExtraOrderDetailsModel> createExtraOrderDetailsModels, List<CreateItemSpecifications> createItemSpecifications)
        {
            foreach (var item in createOrderDetailsModels)
            {

                var isfound = createExtraOrderDetailsModels.Where(x => x.ItemId == item.ItemId).ToList();



                if (isfound.Count != 0)
                {

                    foreach (var exFound in isfound)
                    {

                        item.createExtraOrderDetailsModels.Add(new CreateExtraOrderDetailsModel
                        {

                            ItemId = exFound.ItemId,
                            Name = exFound.Name,
                            NameEnglish = exFound.NameEnglish,
                            Quantity = exFound.Quantity,
                            Total = exFound.Quantity * exFound.UnitPrice,
                            UnitPrice = exFound.UnitPrice


                        });



                    }

                }

                if (createItemSpecifications != null)
                {
                    var sp = createItemSpecifications.Where(x => x.ItemId == item.ItemId).ToList();

                    if (sp.Count != 0)
                    {
                        foreach (var spe in sp)
                        {
                            item.ItemSpecifications.Add(new CreateItemSpecifications
                            {
                                Id = spe.Id,
                                ItemId = spe.ItemId,
                                SpecificationChoices = spe.SpecificationChoices,
                                SpecificationDescription = spe.SpecificationDescription,
                                SpecificationDescriptionEnglish = spe.SpecificationDescriptionEnglish,

                            });

                        }
                    }

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
                    //var orderDetailID = await _orderDetailRepository.InsertAndGetIdAsync(orderDetail);
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

                       // var extID = await _extraOrderDetailRepository.InsertAndGetIdAsync(extraOrderDetail);

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

                           /// var extID = await _extraOrderDetailRepository.InsertAndGetIdAsync(extraOrderDetail);

                        }


                    }



                }


                //foreach (var item in createOrderDetailsModels)
                //{
                //    OrderDetail orderDetail = new OrderDetail
                //    {
                //        IsDeleted = false,
                //        CreationTime = DateTime.Now,
                //        Discount = item.Discount,
                //        ItemId = item.ItemId,
                //        Quantity = item.Quantity,
                //        Total = item.Total,
                //        TenantId = createOrderModel.TenantId,
                //        OrderId = orderId,
                //        UnitPrice = item.UnitPrice

                //    };
                //    var orderDetailID = await _orderDetailRepository.InsertAndGetIdAsync(orderDetail);


                //    foreach (var ext in item.createExtraOrderDetailsModels)
                //    {
                //        ExtraOrderDetail extraOrderDetail = new ExtraOrderDetail
                //        {
                //            Name = ext.Name,
                //              NameEnglish = ext.NameEnglish,
                //            Quantity = ext.Quantity,
                //            TenantId = createOrderModel.TenantId,
                //            UnitPrice = ext.UnitPrice,
                //            OrderDetailId = orderDetailID,
                //            Total = ext.Total

                //        };

                //        var extID = await _extraOrderDetailRepository.InsertAndGetIdAsync(extraOrderDetail);

                //    }

                //    foreach (var ext in item.ItemSpecifications)
                //    {

                //        foreach(var axChoise in ext.SpecificationChoices)
                //        {

                //            ExtraOrderDetail extraOrderDetail = new ExtraOrderDetail
                //            {
                //                Name = axChoise.SpecificationChoiceDescription,
                //                 NameEnglish = axChoise.SpecificationChoiceDescriptionEnglish,
                //                Quantity = 1,
                //                TenantId = createOrderModel.TenantId,
                //                UnitPrice = axChoise.Price,
                //                OrderDetailId = orderDetailID,
                //                Total = axChoise.Price

                //            };

                //            var extID = await _extraOrderDetailRepository.InsertAndGetIdAsync(extraOrderDetail);

                //        }


                //    }


                //}

            }
        }
        private Contact GetCustomer(int? CustomerId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Contacts] where Id=" + CustomerId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            Contact contact = new Contact();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                contact = new Contact
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString()
                };
            }

            conn.Close();
            da.Dispose();

            return contact;
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

        private async Task<TenantModel> GetTenantByAppId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.SmoochAppID == id);
            return tenant;
        }

        private async void updatewatermarkD360(string UserId)
        {
            try
            {
                MicrosoftBotInfo result = new MicrosoftBotInfo();
                var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.userId == UserId).Result;
                var watermark = int.Parse(objConversation.watermark);// + 2;
                objConversation.watermark = watermark.ToString();
                await conversationChat.UpdateItemAsync(objConversation._self, objConversation);
            }
            catch
            {


            }


        }
        private async void updatewatermark(string sunshineConversationId)
        {
            MicrosoftBotInfo result = new MicrosoftBotInfo();
            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.SunshineConversationId == sunshineConversationId).Result;
            var watermark = int.Parse(objConversation.watermark) + 2;
            objConversation.watermark = watermark.ToString();
            await conversationChat.UpdateItemAsync(objConversation._self, objConversation);

        }

        private async Task<TenantModel> GetTenantByAppIdD360(string D360Key)
        {
            if (string.IsNullOrEmpty(D360Key))
                return null;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.D360Key == D360Key);
            return tenant;
        }

        private List<MenuCategoryDto> MenuCategories(int? TenantID, int menu)
        {

            string connString = _configuration["ConnectionStrings:Default"];// AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),

                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<ItemDto> ItemsList(int? TenantID, int mune)
        {
            string connString = _configuration["ConnectionStrings:Default"];// AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + mune;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);




            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();




            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemDto> itemDtos = new List<ItemDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DataSet dataSetItemSpecification = new DataSet();
                SqlCommand cmdItemSpecifications = new SqlCommand();
                cmdItemSpecifications.Connection = conn;
                cmdItemSpecifications.CommandType = CommandType.StoredProcedure;
                cmdItemSpecifications.CommandText = "dbo.ItemSpecificationsGet";
                var _itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);


                if (_itemId == 1472)
                {


                }
                cmdItemSpecifications.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ItemId", _itemId));

                SqlDataAdapter daItemSpectification = new SqlDataAdapter(cmdItemSpecifications);

                daItemSpectification.Fill(dataSetItemSpecification);


                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    List<SpecificationChoice> choices = new List<SpecificationChoice>();

                    var itm = new ItemDto
                    {
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                        CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                        ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                        ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                        ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                        ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),


                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                        //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                        ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                        SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                        //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                        IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),


                        //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    };
                    //Add Specifications
                    for (int x = 0; x < dataSetItemSpecification.Tables[0].Rows.Count; x++)
                    {
                        var itmSpecChoice = new SpecificationChoice
                        {
                            Id = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["Id"]),
                            SpecificationChoiceDescription = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescription"].ToString(),
                            SpecificationId = Convert.ToInt32((dataSetItemSpecification.Tables[0].Rows[x]["SpecificationId"].ToString())),
                            Price = decimal.Parse(dataSetItemSpecification.Tables[0].Rows[x]["Price"].ToString())

                        };
                        choices.Add(itmSpecChoice);
                    }

                    for (int j = 0; j < dataSetItemSpecification.Tables[1].Rows.Count; j++)
                    {
                        var _id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]);
                        var itmSpec = new ItemSpecification
                        {
                            Id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]),
                            SpecificationDescription = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescription"].ToString(),
                            IsMultipleSelection = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsMultipleSelection"].ToString()),
                            IsRequired = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsRequired"].ToString()),
                            SpecificationChoices = choices.Where(p => p.SpecificationId == _id).ToList()


                        };
                        //Add Choices
                        itm.ItemSpecifications.Add(itmSpec);
                    }


                    daItemSpectification.Dispose();
                    itemDtos.Add(itm);

                }



            }

            conn.Close();
            // cmdItemSpecifications.Clone();
            // da.Dispose();

            return itemDtos;

        }
        private List<ItemAdditionDto> AdditionList(int? tenantId, long id)
        {
            string connString = _configuration["ConnectionStrings:Default"];// AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[ItemAdditions] where TenantId=" + tenantId + " and ItemId=" + id;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDtos.Add(new ItemAdditionDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["itemId"]),
                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                });
            }

            conn.Close();
            da.Dispose();

            return itemAdditionDtos;
        }
        private List<MenuDto> MenuList(int? TenantID, int menu)
        {
            string connString = _configuration["ConnectionStrings:Default"];// AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Menus] where TenantID=" + TenantID + " and RestaurantsType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuDto> menuDtos = new List<MenuDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                menuDtos.Add(new MenuDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    //EffectiveTimeFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeFrom"]),
                    //EffectiveTimeTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeTo"]),
                    ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                    MenuDescription = dataSet.Tables[0].Rows[i]["MenuDescription"].ToString(),
                    MenuDescriptionEnglish = dataSet.Tables[0].Rows[i]["MenuDescriptionEnglish"].ToString(),
                    MenuName = dataSet.Tables[0].Rows[i]["MenuName"].ToString(),
                    MenuNameEnglish = dataSet.Tables[0].Rows[i]["MenuNameEnglish"].ToString(),
                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                    RestaurantsType = (RestaurantsTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),

                });
            }

            conn.Close();
            da.Dispose();

            return menuDtos;

        }


        private MultiTenancy.Dto.TenantEditDto GetAllTenant(int? TenantID)
        {

            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                //  string connString = "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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

                MultiTenancy.Dto.TenantEditDto tenant = new MultiTenancy.Dto.TenantEditDto();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    tenant = new MultiTenancy.Dto.TenantEditDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        Image = dataSet.Tables[0].Rows[i]["Image"].ToString(),
                        ImageBg = dataSet.Tables[0].Rows[i]["ImageBg"].ToString()

                    };
                }

                conn.Close();
                da.Dispose();

                return tenant;

            }
            catch
            {
                return null;

            }

        }


        private List<GetCategorysModel> fillGetCategorysModel(List<CategoryEntity> categoryEntity)
        {
            List<GetCategorysModel> getCategorysModels = new List<GetCategorysModel>();
            if (categoryEntity != null)
            {
                foreach (var item in categoryEntity)
                {
                    GetCategorysModel getCategorysModel = new GetCategorysModel();
                    getCategorysModel.Id = item.Id;
                    getCategorysModel.Name = item.Name;
                    getCategorysModel.NameEnglish = item.NameEnglish;
                    getCategorysModel.Priority = item.Priority;
                    getCategorysModel.MenuId = item.MenuId;
                    getCategorysModel.bgImag = item.bgImag;
                    getCategorysModel.logoImag = item.logoImag;

                    getCategorysModel.getSubCategorysModels = fillSubGetCategorysModel(item.ItemSubCategories);
                    getCategorysModels.Add(getCategorysModel);
                }
            }

            return getCategorysModels;

        }
        private List<GetSubCategorysModel> fillSubGetCategorysModel(List<SubCategoryEntity> subCategoryEntity)
        {
            List<GetSubCategorysModel> getSubCategorysModels = new List<GetSubCategorysModel>();
            if (subCategoryEntity != null)
            {
                foreach (var item in subCategoryEntity)
                {

                    GetSubCategorysModel getSubCategorysModel = new GetSubCategorysModel();
                    getSubCategorysModel.Id = item.Id;
                    getSubCategorysModel.Name = item.Name;
                    getSubCategorysModel.NameEnglish = item.NameEnglish;
                    getSubCategorysModel.Priority = item.Priority;
                    getSubCategorysModel.ItemCategoryId = item.ItemCategoryId;
                    getSubCategorysModel.bgImag = item.bgImag;
                    getSubCategorysModel.Price = item.Price;
                    getSubCategorysModel.IsNew = item.IsNew;
                    getSubCategorysModels.Add(getSubCategorysModel);
                }
            }

            return getSubCategorysModels;

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
        #endregion

        #region work around
        [Route("GetMenuItem")]
        [HttpGet]
        public List<CategorysInItemModel> GetMenuItem(int TenantId, int MenuType, int SubCategoryId, int PageSize, int PageNumber, string Search = "", bool isAdvancMenu = false)
        {


            if (TenantId == 34 || TenantId == 47)
            {


                if (SubCategoryId == 0)
                {

                    var sub = GetSubItemCategories(TenantId, MenuType).FirstOrDefault();
                    SubCategoryId = sub.Id;

                }
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

                var CategorieList = GetItemCategories(TenantId, MenuType, 0);
                //var listitem2 = GetItemsPageList(TenantId, MenuType, SubCategoryId, PageSize, PageNumber,Search).ToList();


                var subCategorieList = GetSubItemCategories(TenantId, MenuType);


                foreach (var Categorie in CategorieList)
                {


                    var ListSub = subCategorieList.Where(x => x.ItemCategoryId == Categorie.Id).ToList();

                    List<SubCategorysInItemModel> subCategorysInItemModels = new List<SubCategorysInItemModel>();
                    foreach (var subL in ListSub)
                    {

                        //   var x = GetItemCount(subL.Id);
                        if (subL.Id == SubCategoryId)
                        {


                            subCategorysInItemModels.Add(new SubCategorysInItemModel
                            {
                                categoryId = Categorie.Id,
                                subcategoryId = subL.Id,
                                categoryName = subL.Name,
                                categoryNameEnglish = subL.NameEnglish,
                                menuId = 0,// listitem.FirstOrDefault().MenuId,
                                menuPriority = subL.Priority,
                                listItemInCategories = new List<ItemDto>(),//listitem2,
                                                                           //    itemCount=x

                            });

                        }
                        else
                        {

                            subCategorysInItemModels.Add(new SubCategorysInItemModel
                            {
                                categoryId = Categorie.Id,
                                subcategoryId = subL.Id,
                                categoryName = subL.Name,
                                categoryNameEnglish = subL.NameEnglish,
                                menuId = 0,// listitem.FirstOrDefault().MenuId,
                                menuPriority = subL.Priority,
                                listItemInCategories = new List<ItemDto>(),
                                //   itemCount = x
                            });

                        }


                    }






                    categorysInItemModel.Add(new CategorysInItemModel
                    {
                        menuPriority = Categorie.Priority,//menu.Where(x => x.Id == listitem.FirstOrDefault().MenuId).FirstOrDefault().Priority,
                        menuId = 0,//listitem.FirstOrDefault().MenuId,
                        categoryId = Categorie.Id,
                        categoryName = Categorie.Name,
                        categoryNameEnglish = Categorie.NameEnglish,
                        bgImg = Categorie.bgImag,
                        logImg = Categorie.logoImag,
                        isSubCategory = true,
                        subCategorysInItemModels = subCategorysInItemModels.OrderBy(x => x.menuPriority).ToList(),
                        listItemInCategories = new List<ItemDto>(),// subCategorysInItemModels.FirstOrDefault().listItemInCategories

                    });


                    /////////////

                }

                return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();


            }
            else
            {
                //if (LanguageBotId == 0)
                //    LanguageBotId = 1;
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();
                var result = _IMenuCategoriesAppService.GetCategoryWithItem(TenantId, MenuType);

                var allmenu = GetMenu(TenantId, MenuType, 0).OrderBy(x => x.Priority).ToList();
                foreach (var cat in result)
                {
                    if (cat.listItemInCategories != null)
                    {
                        var MenuId = cat.listItemInCategories.FirstOrDefault().MenuId;

                        if (cat.listItemInCategories.Where(x => x.OldPrice != null).ToList() != null)
                        {
                            foreach (var item in cat.listItemInCategories.Where(x => x.OldPrice != null).ToList())
                            {
                                var o = item.OldPrice;
                                var p = item.Price;

                                item.OldPrice = p;
                                item.Price = o;
                            }
                        }



                        categorysInItemModel.Add(new CategorysInItemModel
                        {
                            menuPriority = allmenu.Where(x => x.Id == MenuId).FirstOrDefault().Priority,
                            menuId = MenuId,
                            categoryId = cat.categoryId,
                            categoryName = cat.categoryName,
                            categoryNameEnglish = cat.categoryNameEnglish,
                            listItemInCategories = cat.listItemInCategories

                        });



                    }
                }
                return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();



                //var CategorieList = GetItemCategories(TenantId, MenuType, 0);
                //var listitem2 = GetItemsList(TenantId, MenuType, 0).ToList();

                //var oldprice = listitem2.Where(x => x.OldPrice != null).ToList();

                //foreach (var item in oldprice)
                //{
                //    var o = item.OldPrice;
                //    var p = item.Price;

                //    item.OldPrice = p;
                //    item.Price = o;
                //}




                //var CatAndAdd = itemAndAdditionsCategory(TenantId, MenuType, 0);




                ////var addCatList = ItemAdditionsCategory(TenantId, MenuType, 0).ToList();
                //var add = GetItemAddition(TenantId);



                //var DitalAdd = GetItemAdditionDetailsList(TenantId);


                //List<ItemDto> itemDtos = new List<ItemDto>();
                //List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();


                //foreach (var Categorie in CategorieList)
                //{


                //    var listitem = listitem2.Where(x => x.ItemCategoryId == Categorie.Id).OrderBy(x => x.Priority).ToList();



                //    foreach (var item in listitem)
                //    {


                //        var CatAndAddList = CatAndAdd.Where(x => x.ItemId == item.Id).ToList();

                //        foreach (var catAndIte in CatAndAddList)
                //        {
                //            var xxxx = add.Where(x => x.ItemAdditionsCategoryId == catAndIte.AdditionsCategorysId).ToList();

                //            foreach (var dsf in xxxx)
                //            {


                //                //dsf.Id = dsf.Id ;
                //                //dsf.itemId = item.Id;
                //                var DetXX = DitalAdd.Where(x => x.ItemAdditionId == dsf.Id && x.MenuType == MenuType).FirstOrDefault();
                //                if (DetXX != null)
                //                {
                //                    var number = new Random().Next(1000, 9999);
                //                    itemAdditionDtos.Add(new ItemAdditionDto
                //                    {
                //                        Id = (dsf.Id + item.Id) * number,
                //                        itemId = item.Id,
                //                        ItemAdditionsCategoryId = dsf.ItemAdditionsCategoryId,
                //                        LanguageBotId = dsf.LanguageBotId,
                //                        MenuType = dsf.MenuType,
                //                        Name = dsf.Name,
                //                        NameEnglish = dsf.NameEnglish,
                //                        price = dsf.price,
                //                        SKU = dsf.SKU,
                //                        IsInService = DetXX.IsInService
                //                    });

                //                }


                //            }
                //            ////////////

                //        }

                //        item.itemAdditionDtos = itemAdditionDtos.Where(x => x.itemId == item.Id).ToArray();

                //        itemDtos.Add(item);

                //        // var ListItemAddition = GetItemAddition2(TenantId, item.Id).ToArray();


                //    }
                //    var menu = GetMenu(TenantId, MenuType, 0).OrderBy(x => x.Priority).ToList();
                //    if (listitem != null && listitem.Count > 0)
                //    {
                //        categorysInItemModel.Add(new CategorysInItemModel
                //        {
                //            menuPriority = menu.Where(x => x.Id == listitem.FirstOrDefault().MenuId).FirstOrDefault().Priority,
                //            menuId = listitem.FirstOrDefault().MenuId,
                //            categoryId = Categorie.Id,
                //            categoryName = Categorie.Name,
                //            categoryNameEnglish = Categorie.NameEnglish,
                //            listItemInCategories = listitem

                //        });
                //    }

                //}


                //return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();
            }

        }


        private List<ItemAdditionDetailsModel> GetItemAdditionDetailsList(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemAdditionDetails] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDetailsModel> itemAdditionDetails = new List<ItemAdditionDetailsModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDetails.Add(new ItemAdditionDetailsModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                    ItemAdditionId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionId"]),
                    //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                });



            }

            conn.Close();
            da.Dispose();

            return itemAdditionDetails;
        }
        private List<ItemSpecificationsDetail> GetItemSpecificationsDetailList(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemSpecificationsDetail] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSpecificationsDetail> itemAdditionDetails = new List<ItemSpecificationsDetail>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDetails.Add(new ItemSpecificationsDetail
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                    SpecificationChoicesId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationChoicesId"]),
                    //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                });



            }

            conn.Close();
            da.Dispose();

            return itemAdditionDetails;
        }


        private int GetItemCount(int subID)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//AppSettingsModel.ConnectionStrings; //

            string query = "";
            if (subID == 343 || subID == 346 || subID == 347)
            {
                query = "SELECT COUNT(Id)  from [dbo].[Items] where ItemSubCategoryId = " + subID + " and DateFrom is not null";

            }
            else
            {
                query = "SELECT COUNT(Id)  from [dbo].[Items] where ItemSubCategoryId = " + subID;

            }





            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();
            var x = 0;
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {



                x = int.Parse(dataSet.Tables[0].Rows[i]["Column1"].ToString());







            }

            conn.Close();
            da.Dispose();

            return x;

        }
        private List<MenuSubCategoryDto> GetSubItemCategories(int TenantID, int MenuType)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";//AppSettingsModel.ConnectionStrings; //
            string query = "select * from [dbo].[ItemSubCategories] where TenantId = " + TenantID + " and MenuType = " + MenuType;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuSubCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                        TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        // IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        // bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        // logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }

        private List<MenuCategoryDto> GetItemCategories(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuType = " + menu + " and IsDeleted = " + 0
                ;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),


                        bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                        Priority = int.Parse(dataSet.Tables[0].Rows[i]["Priority"].ToString()),

                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<MenuCategoryDto> GetItemCategoriesPage(int? TenantID, long menu, long CategoriesId)
        {
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuType = " + menu + " and id = " + CategoriesId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),


                        bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString()
                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<ItemAdditionDto> GetItemAddition(int? tenantId, long id)
        {
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemAdditions] where TenantId=" + tenantId + " and ItemId=" + id;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDtos.Add(new ItemAdditionDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["itemId"]),
                    SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                });
            }

            conn.Close();
            da.Dispose();

            return itemAdditionDtos;
        }
        private List<MenuDto> GetMenu(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[Menus] where TenantID=" + TenantID + " and RestaurantsType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuDto> menuDtos = new List<MenuDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                menuDtos.Add(new MenuDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    //EffectiveTimeFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeFrom"]),
                    //EffectiveTimeTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeTo"]),
                    ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                    MenuDescription = dataSet.Tables[0].Rows[i]["MenuDescription"].ToString(),
                    MenuName = dataSet.Tables[0].Rows[i]["MenuName"].ToString(),
                    MenuDescriptionEnglish = dataSet.Tables[0].Rows[i]["MenuDescriptionEnglish"].ToString(),
                    MenuNameEnglish = dataSet.Tables[0].Rows[i]["MenuNameEnglish"].ToString(),
                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                    RestaurantsType = (RestaurantsTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                    //Tax = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Tax"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),

                });
            }

            conn.Close();
            da.Dispose();

            return menuDtos;

        }
        private List<ItemDto> GetItemsList(int? TenantID, int menu, int LanguageBotId)
        {
            var det = GetItemSpecificationsDetailList((int)TenantID);
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);




            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();




            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemDto> itemDtos = new List<ItemDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DataSet dataSetItemSpecification = new DataSet();
                SqlCommand cmdItemSpecifications = new SqlCommand();
                cmdItemSpecifications.Connection = conn;
                cmdItemSpecifications.CommandType = CommandType.StoredProcedure;
                cmdItemSpecifications.CommandText = "dbo.ItemSpecificationsGet";
                var _itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);



                cmdItemSpecifications.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ItemId", _itemId));

                SqlDataAdapter daItemSpectification = new SqlDataAdapter(cmdItemSpecifications);

                daItemSpectification.Fill(dataSetItemSpecification);


                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                List<SpecificationChoice> choices = new List<SpecificationChoice>();
                if (!IsDeleted)
                {

                    try
                    {
                        var itm = new ItemDto
                        {

                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                            //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                            OldPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? "0"),
                            Discount = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                            CreationTime = DateTime.Now,
                        };


                        //Add Specifications
                        for (int x = 0; x < dataSetItemSpecification.Tables[0].Rows.Count; x++)
                        {
                            var itmSpecChoice = new SpecificationChoice
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["Id"]),
                                SpecificationChoiceDescription = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescription"].ToString(),
                                SpecificationId = Convert.ToInt32((dataSetItemSpecification.Tables[0].Rows[x]["SpecificationId"].ToString())),
                                Price = decimal.Parse(dataSetItemSpecification.Tables[0].Rows[x]["Price"].ToString()),


                                SpecificationChoiceDescriptionEnglish = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescriptionEnglish"].ToString(),
                                LanguageBotId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["LanguageBotId"].ToString()),
                                SKU = dataSetItemSpecification.Tables[0].Rows[x]["SKU"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["TenantId"].ToString()),

                            };
                            var fund = det.Where(x => x.SpecificationChoicesId == itmSpecChoice.Id && x.MenuType == itm.MenuType).FirstOrDefault();
                            if (fund != null)
                            {
                                itmSpecChoice.IsInService = fund.IsInService;
                                choices.Add(itmSpecChoice);
                            }

                        }

                        List<ItemSpecification> ItemSpecifications12 = new List<ItemSpecification>();
                        for (int j = 0; j < dataSetItemSpecification.Tables[1].Rows.Count; j++)
                        {
                            var _id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]);
                            var itmSpec = new ItemSpecification
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]),
                                SpecificationDescription = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescription"].ToString(),
                                IsMultipleSelection = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsMultipleSelection"].ToString()),
                                IsRequired = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsRequired"].ToString()),
                                SpecificationChoices = choices.Where(p => p.SpecificationId == _id).ToList(),
                                MaxSelectNumber = int.Parse(dataSetItemSpecification.Tables[1].Rows[j]["MaxSelectNumber"].ToString()),

                                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                                Priority = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Priority"]),
                                SpecificationDescriptionEnglish = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescriptionEnglish"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["TenantId"])



                            };
                            //Add Choices

                            ItemSpecifications12.Add(itmSpec);
                            //itm.ItemSpecifications.Add(itmSpec);
                        }
                        itm.ItemSpecifications = ItemSpecifications12.OrderBy(x => x.Priority).ToList();
                        // var x=itm.ItemSpecifications.OrderBy(x => x.Priority);
                        daItemSpectification.Dispose();
                        itemDtos.Add(itm);
                    }
                    catch
                    {
                        var itm = new ItemDto
                        {

                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                            //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                            CreationTime = DateTime.Now,
                        };


                        //Add Specifications
                        for (int x = 0; x < dataSetItemSpecification.Tables[0].Rows.Count; x++)
                        {
                            var itmSpecChoice = new SpecificationChoice
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["Id"]),
                                SpecificationChoiceDescription = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescription"].ToString(),
                                SpecificationId = Convert.ToInt32((dataSetItemSpecification.Tables[0].Rows[x]["SpecificationId"].ToString())),
                                Price = decimal.Parse(dataSetItemSpecification.Tables[0].Rows[x]["Price"].ToString()),


                                SpecificationChoiceDescriptionEnglish = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescriptionEnglish"].ToString(),
                                LanguageBotId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["LanguageBotId"].ToString()),
                                SKU = dataSetItemSpecification.Tables[0].Rows[x]["SKU"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["TenantId"].ToString()),

                            };
                            var fund = det.Where(x => x.SpecificationChoicesId == itmSpecChoice.Id && x.MenuType == itm.MenuType).FirstOrDefault();
                            if (fund != null)
                            {
                                itmSpecChoice.IsInService = fund.IsInService;
                                choices.Add(itmSpecChoice);
                            }

                        }

                        List<ItemSpecification> ItemSpecifications12 = new List<ItemSpecification>();
                        for (int j = 0; j < dataSetItemSpecification.Tables[1].Rows.Count; j++)
                        {
                            var _id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]);
                            var itmSpec = new ItemSpecification
                            {
                                Id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]),
                                SpecificationDescription = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescription"].ToString(),
                                IsMultipleSelection = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsMultipleSelection"].ToString()),
                                IsRequired = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsRequired"].ToString()),
                                SpecificationChoices = choices.Where(p => p.SpecificationId == _id).ToList(),
                                MaxSelectNumber = int.Parse(dataSetItemSpecification.Tables[1].Rows[j]["MaxSelectNumber"].ToString()),

                                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                                Priority = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Priority"]),
                                SpecificationDescriptionEnglish = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescriptionEnglish"].ToString(),
                                TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["TenantId"])



                            };
                            //Add Choices

                            ItemSpecifications12.Add(itmSpec);
                            //itm.ItemSpecifications.Add(itmSpec);
                        }
                        itm.ItemSpecifications = ItemSpecifications12.OrderBy(x => x.Priority).ToList();
                        // var x=itm.ItemSpecifications.OrderBy(x => x.Priority);
                        daItemSpectification.Dispose();
                        itemDtos.Add(itm);

                    }




                }



            }

            conn.Close();
            // cmdItemSpecifications.Clone();
            // da.Dispose();

            return itemDtos;

        }


        private List<ItemAdditionsCategory> ItemAdditionsCategory(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemAdditionsCategorys] where TenantId=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionsCategory> itemCategories = new List<ItemAdditionsCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {


                itemCategories.Add(new ItemAdditionsCategory
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
                });





            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<ItemAndAdditionsCategory> itemAndAdditionsCategory(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemAndAdditionsCategorys] where TenantID=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAndAdditionsCategory> itemCategories = new List<ItemAndAdditionsCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {


                itemCategories.Add(new ItemAndAdditionsCategory
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    AdditionsCategorysId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AdditionsCategorysId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    SpecificationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationId"]),
                });





            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }

        private List<ItemAdditionDto> GetItemAddition(int? tenantId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:info-seed-db-server-prod.database.windows.net,1433;Initial Catalog=InfoSeedDB-Prod;Persist Security Info=False;User ID=devadmin;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            string query = "select * from [dbo].[ItemAdditions] where TenantId=" + tenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                try
                {
                    itemAdditionDtos.Add(new ItemAdditionDto
                    {
                        Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                        // itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["itemId"]),
                        // SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                        MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        ItemAdditionsCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionsCategoryId"].ToString()),
                    });
                }
                catch (Exception )
                {


                }

            }

            conn.Close();
            da.Dispose();

            return itemAdditionDtos;
        }

        #endregion

        #region work around 2
        [Route("GetMenuItem2")]
        [HttpGet]
        public List<CategorysInItemModel> GetMenuItem2(int TenantId, int MenuType, int LanguageBotId)
        {


            if (TenantId == 34)
            {
                if (LanguageBotId == 0)
                    LanguageBotId = 1;
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

                var CategorieList = GetItemCategories2(TenantId, MenuType, LanguageBotId);
                var subCategorieList = GetSubItemCategories2();

                var listitem2 = GetItemsList2(TenantId, MenuType, LanguageBotId).ToList();




                var CatAndAdd = itemAndAdditionsCategory2(TenantId, MenuType, LanguageBotId);




                var addCatList = ItemAdditionsCategory2(TenantId, MenuType, LanguageBotId).ToList();
                var add = GetItemAddition2(TenantId);


                var DitalAdd = GetItemAdditionDetailsList2(TenantId);

                List<ItemDto> itemDtos = new List<ItemDto>();
                List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();


                foreach (var Categorie in CategorieList)
                {


                    var listitem = listitem2.Where(x => x.ItemCategoryId == Categorie.Id).OrderBy(x => x.Priority).ToList();



                    foreach (var item in listitem)
                    {


                        var CatAndAddList = CatAndAdd.Where(x => x.ItemId == item.Id).ToList();

                        foreach (var catAndIte in CatAndAddList)
                        {
                            var xxxx = add.Where(x => x.ItemAdditionsCategoryId == catAndIte.AdditionsCategorysId).ToList();

                            var number = new Random().Next(1000, 9999);
                            foreach (var dsf in xxxx)
                            {
                                //dsf.Id = dsf.Id ;
                                //dsf.itemId = item.Id;
                                var DetXX = DitalAdd.Where(x => x.ItemAdditionId == dsf.Id && x.MenuType == MenuType).FirstOrDefault();
                                if (DetXX != null)
                                {

                                    itemAdditionDtos.Add(new ItemAdditionDto
                                    {
                                        Id = (dsf.Id + item.Id) * number,
                                        itemId = item.Id,
                                        ItemAdditionsCategoryId = dsf.ItemAdditionsCategoryId,
                                        LanguageBotId = dsf.LanguageBotId,
                                        MenuType = dsf.MenuType,
                                        Name = dsf.Name,
                                        NameEnglish = dsf.NameEnglish,
                                        price = dsf.price,
                                        SKU = dsf.SKU,
                                        IsInService = DetXX.IsInService

                                    });

                                }


                            }
                            ////////////

                        }

                        item.itemAdditionDtos = itemAdditionDtos.Where(x => x.itemId == item.Id).ToArray();

                        itemDtos.Add(item);

                        // var ListItemAddition = GetItemAddition2(TenantId, item.Id).ToArray();


                    }




                    var menu = GetMenu2(TenantId, MenuType, LanguageBotId).OrderBy(x => x.Priority).ToList();

                    var fundSub = subCategorieList.Where(x => x.ItemCategoryId == Categorie.Id);

                    List<SubCategorysInItemModel> subCategorysInItemModels = new List<SubCategorysInItemModel>();

                    foreach (var sub in fundSub)
                    {


                        var funditemWithSubCat = listitem.Where(x => x.ItemSubCategoryId == sub.Id).ToList();

                        subCategorysInItemModels.Add(new SubCategorysInItemModel
                        {
                            categoryId = Categorie.Id,
                            subcategoryId = sub.Id,
                            categoryName = sub.Name,
                            categoryNameEnglish = sub.NameEnglish,
                            menuId = listitem.FirstOrDefault().MenuId,
                            menuPriority = sub.Priority,
                            listItemInCategories = funditemWithSubCat
                        });

                    }

                    if (listitem != null && listitem.Count > 0)
                    {
                        categorysInItemModel.Add(new CategorysInItemModel
                        {
                            menuPriority = menu.Where(x => x.Id == listitem.FirstOrDefault().MenuId).FirstOrDefault().Priority,
                            menuId = listitem.FirstOrDefault().MenuId,
                            categoryId = Categorie.Id,
                            categoryName = Categorie.Name,
                            categoryNameEnglish = Categorie.NameEnglish,
                            bgImg = Categorie.bgImag,
                            logImg = Categorie.logoImag,
                            isSubCategory = true,
                            subCategorysInItemModels = subCategorysInItemModels,
                            listItemInCategories = subCategorysInItemModels.FirstOrDefault().listItemInCategories

                        });
                    }

                }


                return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();


            }
            else
            {

                if (LanguageBotId == 0)
                    LanguageBotId = 1;
                List<CategorysInItemModel> categorysInItemModel = new List<CategorysInItemModel>();

                var CategorieList = GetItemCategories2(TenantId, MenuType, LanguageBotId);


                var listitem2 = GetItemsList2(TenantId, MenuType, LanguageBotId).ToList();




                var CatAndAdd = itemAndAdditionsCategory2(TenantId, MenuType, LanguageBotId);




                var addCatList = ItemAdditionsCategory2(TenantId, MenuType, LanguageBotId).ToList();
                var add = GetItemAddition2(TenantId);


                var DitalAdd = GetItemAdditionDetailsList2(TenantId);

                List<ItemDto> itemDtos = new List<ItemDto>();
                List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();


                foreach (var Categorie in CategorieList)
                {


                    var listitem = listitem2.Where(x => x.ItemCategoryId == Categorie.Id).OrderBy(x => x.Priority).ToList();



                    foreach (var item in listitem)
                    {


                        var CatAndAddList = CatAndAdd.Where(x => x.ItemId == item.Id).ToList();

                        foreach (var catAndIte in CatAndAddList)
                        {
                            var xxxx = add.Where(x => x.ItemAdditionsCategoryId == catAndIte.AdditionsCategorysId).ToList();

                            var number = new Random().Next(1000, 9999);
                            foreach (var dsf in xxxx)
                            {
                                //dsf.Id = dsf.Id ;
                                //dsf.itemId = item.Id;
                                var DetXX = DitalAdd.Where(x => x.ItemAdditionId == dsf.Id && x.MenuType == MenuType).FirstOrDefault();
                                if (DetXX != null)
                                {

                                    itemAdditionDtos.Add(new ItemAdditionDto
                                    {
                                        Id = (dsf.Id + item.Id) * number,
                                        itemId = item.Id,
                                        ItemAdditionsCategoryId = dsf.ItemAdditionsCategoryId,
                                        LanguageBotId = dsf.LanguageBotId,
                                        MenuType = dsf.MenuType,
                                        Name = dsf.Name,
                                        NameEnglish = dsf.NameEnglish,
                                        price = dsf.price,
                                        SKU = dsf.SKU,
                                        IsInService = DetXX.IsInService

                                    });

                                }


                            }
                            ////////////

                        }

                        item.itemAdditionDtos = itemAdditionDtos.Where(x => x.itemId == item.Id).ToArray();

                        itemDtos.Add(item);

                        // var ListItemAddition = GetItemAddition2(TenantId, item.Id).ToArray();


                    }




                    var menu = GetMenu2(TenantId, MenuType, LanguageBotId).OrderBy(x => x.Priority).ToList();



                    if (listitem != null && listitem.Count > 0)
                    {
                        categorysInItemModel.Add(new CategorysInItemModel
                        {
                            menuPriority = menu.Where(x => x.Id == listitem.FirstOrDefault().MenuId).FirstOrDefault().Priority,
                            menuId = listitem.FirstOrDefault().MenuId,
                            categoryId = Categorie.Id,
                            categoryName = Categorie.Name,
                            isSubCategory = false,
                            categoryNameEnglish = Categorie.NameEnglish,
                            bgImg = Categorie.bgImag,
                            logImg = Categorie.logoImag,
                            listItemInCategories = listitem

                        });
                    }

                }


                return categorysInItemModel.OrderBy(x => x.menuPriority).ToList();
            }

        }

        private List<AdditionsCategorysListModel> GetAdditionsCategorysForitemList(int itemId, int TenantId, int MenuType, int LanguageBotId)
        {

            List<AdditionsCategorysListModel> rType = new List<AdditionsCategorysListModel>();

            var CatAndAdd = itemAndAdditionsCategory2(TenantId, MenuType, LanguageBotId);


            var CatAndAddList = CatAndAdd.Where(x => x.ItemId == itemId).ToList();




            var addCatList = ItemAdditionsCategory2(TenantId, MenuType, LanguageBotId).ToList();

            foreach (var item in CatAndAddList)
            {
                var xx = addCatList.Where(x => x.Id == item.AdditionsCategorysId).ToList();

                foreach (var add in xx)
                {
                    rType.Add(new AdditionsCategorysListModel
                    {

                        AdditionsAndItemId = int.Parse(item.Id.ToString()),
                        Id = int.Parse(add.Id.ToString()),
                        Name = add.Name,
                        NameEnglish = add.NameEnglish

                    });



                }

            }


            return rType;
        }

        private List<ItemAdditionsCategory> ItemAdditionsCategory2(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemAdditionsCategorys] where TenantId=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionsCategory> itemCategories = new List<ItemAdditionsCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {


                itemCategories.Add(new ItemAdditionsCategory
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
                });





            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<ItemAndAdditionsCategory> itemAndAdditionsCategory2(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemAndAdditionsCategorys] where TenantID=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAndAdditionsCategory> itemCategories = new List<ItemAndAdditionsCategory>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {


                itemCategories.Add(new ItemAndAdditionsCategory
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    AdditionsCategorysId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AdditionsCategorysId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    SpecificationId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationId"]),
                });





            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }

        private List<ItemAdditionDetailsModel> GetItemAdditionDetailsList2(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemAdditionDetails] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDetailsModel> itemAdditionDetails = new List<ItemAdditionDetailsModel>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDetails.Add(new ItemAdditionDetailsModel
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                    ItemAdditionId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionId"]),
                    //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                });



            }

            conn.Close();
            da.Dispose();

            return itemAdditionDetails;
        }


        private List<ItemSpecificationsDetail> GetItemSpecificationsDetailList2(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemSpecificationsDetail] where TenantID=" + TenantID;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemSpecificationsDetail> itemAdditionDetails = new List<ItemSpecificationsDetail>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDetails.Add(new ItemSpecificationsDetail
                {
                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                    //CopiedFromId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CopiedFromId"]),
                    SpecificationChoicesId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["SpecificationChoicesId"]),
                    //  ItemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemId"]),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
                    IsInService = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsInService"]),

                });



            }

            conn.Close();
            da.Dispose();

            return itemAdditionDetails;
        }



        private List<MenuCategoryDto> GetItemCategories2(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemCategorys] where TenantID=" + TenantID + " and MenuType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuCategoryDto> itemCategories = new List<MenuCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<MenuSubCategoryDto> GetSubItemCategories2()
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemSubCategories] ";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuSubCategoryDto> itemCategories = new List<MenuSubCategoryDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {

                    itemCategories.Add(new MenuSubCategoryDto
                    {
                        Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                        Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                        Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                        NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                        // IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                        LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        // bgImag = dataSet.Tables[0].Rows[i]["bgImag"].ToString(),
                        // logoImag = dataSet.Tables[0].Rows[i]["logoImag"].ToString(),
                    });


                }


            }

            conn.Close();
            da.Dispose();

            return itemCategories;

        }
        private List<ItemAdditionDto> GetItemAddition2(int? tenantId)
        {
            string connString = AppSettingsModel.ConnectionStrings;//"Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[ItemAdditions] where TenantId=" + tenantId;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemAdditionDto> itemAdditionDtos = new List<ItemAdditionDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                itemAdditionDtos.Add(new ItemAdditionDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
                    NameEnglish = dataSet.Tables[0].Rows[i]["NameEnglish"].ToString(),
                    price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                    // itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["itemId"]),
                    // SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                    MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                    ItemAdditionsCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemAdditionsCategoryId"]),
                });
            }

            conn.Close();
            da.Dispose();

            return itemAdditionDtos;
        }
        private List<MenuDto> GetMenu2(int? TenantID, int menu, int LanguageBotId)
        {
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[Menus] where TenantID=" + TenantID + " and RestaurantsType = " + menu;


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<MenuDto> menuDtos = new List<MenuDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                menuDtos.Add(new MenuDto
                {
                    Id = long.Parse(dataSet.Tables[0].Rows[i]["Id"].ToString()),
                    //EffectiveTimeFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeFrom"]),
                    //EffectiveTimeTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["EffectiveTimeTo"]),
                    ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                    MenuDescription = dataSet.Tables[0].Rows[i]["MenuDescription"].ToString(),
                    MenuName = dataSet.Tables[0].Rows[i]["MenuName"].ToString(),
                    MenuDescriptionEnglish = dataSet.Tables[0].Rows[i]["MenuDescriptionEnglish"].ToString(),
                    MenuNameEnglish = dataSet.Tables[0].Rows[i]["MenuNameEnglish"].ToString(),
                    Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                    RestaurantsType = (RestaurantsTypeEunm)Convert.ToInt32(dataSet.Tables[0].Rows[i]["RestaurantsType"]),
                    //Tax = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Tax"]),
                    LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),

                });
            }

            conn.Close();
            da.Dispose();

            return menuDtos;

        }
        private List<ItemDto> GetItemsList2(int? TenantID, int menu, int LanguageBotId)
        {

            var det = GetItemSpecificationsDetailList2((int)TenantID);
            string connString = AppSettingsModel.ConnectionStrings;// "Server=tcp:teaminboxsvr.database.windows.net,1433;Initial Catalog=TeamInboxDB;Persist Security Info=False;User ID=TeamInboxAdmin;Password=TeamInbox@P@ssw0rd;";
            string query = "select * from [dbo].[Items] where TenantID=" + TenantID + " and MenuType = " + menu;



            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);




            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();




            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<ItemDto> itemDtos = new List<ItemDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DataSet dataSetItemSpecification = new DataSet();
                SqlCommand cmdItemSpecifications = new SqlCommand();
                cmdItemSpecifications.Connection = conn;
                cmdItemSpecifications.CommandType = CommandType.StoredProcedure;
                cmdItemSpecifications.CommandText = "dbo.ItemSpecificationsGet";
                var _itemId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);


                cmdItemSpecifications.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ItemId", _itemId));

                SqlDataAdapter daItemSpectification = new SqlDataAdapter(cmdItemSpecifications);

                daItemSpectification.Fill(dataSetItemSpecification);


                var IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                if (!IsDeleted)
                {
                    List<SpecificationChoice> choices = new List<SpecificationChoice>();
                    ItemDto itm = null;
                    if (TenantID == 34)
                    {
                        itm = new ItemDto
                        {
                            ItemSubCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"]),
                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                            //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                            Qty = int.Parse(dataSet.Tables[0].Rows[i]["Qty"].ToString()),
                            Size = dataSet.Tables[0].Rows[i]["Size"].ToString(),
                            DateFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateFrom"] ?? null),
                            DateTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateTo"] ?? null),
                            OldPrice = decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? null),

                        };
                    }
                    else
                    {
                        itm = new ItemDto
                        {

                            ItemCategoryId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]),
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            CategoryNames = dataSet.Tables[0].Rows[i]["CategoryNames"].ToString(),
                            CategoryNamesEnglish = dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString(),

                            ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            ItemDescriptionEnglish = dataSet.Tables[0].Rows[i]["ItemDescriptionEnglish"].ToString(),

                            ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            ItemNameEnglish = dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString(),
                            IsDeleted = bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            //	CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"]),
                            //DeletionTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DeletionTime"]),
                            ImageUri = dataSet.Tables[0].Rows[i]["ImageUri"].ToString(),
                            SKU = dataSet.Tables[0].Rows[i]["SKU"].ToString(),
                            //	Ingredients = dataSet.Tables[0].Rows[i]["Ingredients"].ToString(),
                            IsInService = bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString()),
                            //ItemDescription = dataSet.Tables[0].Rows[i]["ItemDescription"].ToString(),
                            //ItemName = dataSet.Tables[0].Rows[i]["ItemName"].ToString(),
                            //LastModificationTime = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            Price = decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString()),
                            Priority = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]),
                            MenuId = long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString()),
                            MenuType = Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]),
                            LanguageBotId = int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString()),
                        };

                    }

                    //Add Specifications
                    for (int x = 0; x < dataSetItemSpecification.Tables[0].Rows.Count; x++)
                    {
                        var itmSpecChoice = new SpecificationChoice
                        {
                            Id = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["Id"]),
                            SpecificationChoiceDescription = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescription"].ToString(),
                            SpecificationId = Convert.ToInt32((dataSetItemSpecification.Tables[0].Rows[x]["SpecificationId"].ToString())),
                            Price = decimal.Parse(dataSetItemSpecification.Tables[0].Rows[x]["Price"].ToString()),


                            SpecificationChoiceDescriptionEnglish = dataSetItemSpecification.Tables[0].Rows[x]["SpecificationChoiceDescriptionEnglish"].ToString(),
                            LanguageBotId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["LanguageBotId"].ToString()),
                            SKU = dataSetItemSpecification.Tables[0].Rows[x]["SKU"].ToString(),
                            TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[0].Rows[x]["TenantId"].ToString()),

                        };
                        var fund = det.Where(x => x.SpecificationChoicesId == itmSpecChoice.Id && x.MenuType == itm.MenuType).FirstOrDefault();
                        if (fund != null)
                        {
                            itmSpecChoice.IsInService = fund.IsInService;
                            choices.Add(itmSpecChoice);
                        }

                    }

                    List<ItemSpecification> ItemSpecifications12 = new List<ItemSpecification>();
                    for (int j = 0; j < dataSetItemSpecification.Tables[1].Rows.Count; j++)
                    {
                        var _id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]);
                        var itmSpec = new ItemSpecification
                        {
                            Id = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Id"]),
                            SpecificationDescription = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescription"].ToString(),
                            IsMultipleSelection = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsMultipleSelection"].ToString()),
                            IsRequired = bool.Parse(dataSetItemSpecification.Tables[1].Rows[j]["IsRequired"].ToString()),
                            SpecificationChoices = choices.Where(p => p.SpecificationId == _id).ToList(),
                            MaxSelectNumber = int.Parse(dataSetItemSpecification.Tables[1].Rows[j]["MaxSelectNumber"].ToString()),

                            // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                            Priority = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["Priority"]),
                            SpecificationDescriptionEnglish = dataSetItemSpecification.Tables[1].Rows[j]["SpecificationDescriptionEnglish"].ToString(),
                            TenantId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["TenantId"])



                        };
                        //Add Choices

                        ItemSpecifications12.Add(itmSpec);
                        //itm.ItemSpecifications.Add(itmSpec);
                    }
                    itm.ItemSpecifications = ItemSpecifications12.OrderBy(x => x.Priority).ToList();
                    // var x=itm.ItemSpecifications.OrderBy(x => x.Priority);
                    daItemSpectification.Dispose();
                    itemDtos.Add(itm);

                }



            }

            conn.Close();
            // cmdItemSpecifications.Clone();
            // da.Dispose();

            return itemDtos;

        }

        private ItemDto MapItems(IDataReader dataReader)
        {
            ItemDto model = new ItemDto();
            try
            {

                model.ItemSubCategoryId = int.Parse((SqlDataHelper.GetValue<long>(dataReader, "ItemSubCategoryId")).ToString() ?? "0");// Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemSubCategoryId"] ?? 0);
                model.ItemCategoryId = int.Parse((SqlDataHelper.GetValue<long>(dataReader, "ItemCategoryId")).ToString() ?? "0");//Convert.ToInt32(dataSet.Tables[0].Rows[i]["ItemCategoryId"]);
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");// Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]);
                model.CategoryNames = SqlDataHelper.GetValue<string>(dataReader, "CategoryNames");//dataSet.Tables[0].Rows[i]["CategoryNames"].ToString() ?? "";
                model.CategoryNamesEnglish = SqlDataHelper.GetValue<string>(dataReader, "CategoryNamesEnglish");//dataSet.Tables[0].Rows[i]["CategoryNamesEnglish"].ToString() ?? "";

                model.ItemDescription = SqlDataHelper.GetValue<string>(dataReader, "Size");//dataSet.Tables[0].Rows[i]["Size"].ToString();
                model.ItemDescriptionEnglish = SqlDataHelper.GetValue<string>(dataReader, "Size");//dataSet.Tables[0].Rows[i]["Size"].ToString();

                model.ItemName = SqlDataHelper.GetValue<string>(dataReader, "ItemName");//dataSet.Tables[0].Rows[i]["ItemName"].ToString();
                model.ItemNameEnglish = SqlDataHelper.GetValue<string>(dataReader, "ItemNameEnglish");//dataSet.Tables[0].Rows[i]["ItemNameEnglish"].ToString();
                model.IsDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");//bool.Parse(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());
                model.ImageUri = SqlDataHelper.GetValue<string>(dataReader, "ImageUri");//dataSet.Tables[0].Rows[i]["ImageUri"].ToString();
                model.SKU = SqlDataHelper.GetValue<string>(dataReader, "SKU");//dataSet.Tables[0].Rows[i]["SKU"].ToString();
                model.IsInService = SqlDataHelper.GetValue<bool>(dataReader, "IsInService");// bool.Parse(dataSet.Tables[0].Rows[i]["IsInService"].ToString());
                model.Price = SqlDataHelper.GetValue<decimal>(dataReader, "Price");//decimal.Parse(dataSet.Tables[0].Rows[i]["Price"].ToString());
                model.Priority = SqlDataHelper.GetValue<int>(dataReader, "Priority");// Convert.ToInt32(dataSet.Tables[0].Rows[i]["Priority"]);
                model.MenuId = SqlDataHelper.GetValue<long>(dataReader, "MenuId");// long.Parse(dataSet.Tables[0].Rows[i]["MenuId"].ToString());
                model.MenuType = SqlDataHelper.GetValue<int>(dataReader, "MenuType");//Convert.ToInt32(dataSet.Tables[0].Rows[i]["MenuType"]);
                model.LanguageBotId = SqlDataHelper.GetValue<int>(dataReader, "LanguageBotId");// int.Parse(dataSet.Tables[0].Rows[i]["LanguageBotId"].ToString());
                model.Qty = SqlDataHelper.GetValue<int>(dataReader, "Qty") ?? 0;// Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"] ?? 0);
                model.Size = SqlDataHelper.GetValue<string>(dataReader, "Size");//dataSet.Tables[0].Rows[i]["Size"].ToString() ?? "";
                model.DateFrom = SqlDataHelper.GetValue<DateTime>(dataReader, "DateFrom") ?? (DateTime?)null;//Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateFrom"] ?? null);
                model.DateTo = SqlDataHelper.GetValue<DateTime>(dataReader, "DateTo") ?? (DateTime?)null;// Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DateTo"] ?? null);
                model.OldPrice = SqlDataHelper.GetValue<decimal>(dataReader, "OldPrice") ?? (decimal?)null;// decimal.Parse(dataSet.Tables[0].Rows[i]["OldPrice"].ToString() ?? null);
                model.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");

            }
            catch (Exception )
            {


            }




            model.additionsCategorysListModels = new List<AdditionsCategorysListModel>();

            List<ItemAdditionDto> itemAdditionDtos22 = new List<ItemAdditionDto>();
            model.itemAdditionDtos = itemAdditionDtos22.ToArray();

            if (model.OldPrice != 0 && model.OldPrice != null)
            {
                if (model.OldPrice != model.Price && model.OldPrice > model.Price)
                {

                    var datNow = DateTime.Now.AddHours(AppSettingsModel.AddHour);

                    if (model.DateTo != null)
                    {

                        var x1 = (model.Price / model.OldPrice);
                        var x2 = 100 - (x1 * 100);
                        var xFormat = String.Format("{0:0.##}", x2);
                        model.Discount = xFormat.ToString() + "%";
                        model.DiscountImg = "https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/sale.png";

                    }

                }
                else
                {
                    model.OldPrice = 0;
                }


            }
            model.ViewPrice = -1;
            if (model.ItemDescription.Contains("1 KG") && !model.SKU.StartsWith('T'))
            {
                var itemAupdate = AddExtra(model);

                model.ItemSpecifications = itemAupdate;
                model.ViewPrice = 0;
            }






            return model;
        }

        #endregion
    }
}

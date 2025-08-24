using Infoseed.MassagingPort.OrderingMenu.Helper;
using Infoseed.MassagingPort.OrderingMenu.Pages.Model;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.TenantServicesInfo;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Web;
using static Infoseed.MessagingPortal.Constants;

namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class OrdersModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        private readonly IItemsAppService _IItemsAppService;
        private readonly ITenantServicesInfoAppService _ITenantServicesInfoAppService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IOrdersAppService _IOrdersAppService;
        private ILoyaltyAppService _ILoyaltyAppService;
        private IDBService _dbService;

        private IMenusAppService _iMenusAppService;
        public List<OrderingMenuOrdersHistoryModel> lstOrderingMenuOrdersHistoryModel { get; set; }
        OrderingMenuTenantModel OrderingMenuTenantModel { get; set; }
        public int TenantID
        {
            //get
            //{
            //    return !string.IsNullOrEmpty(HttpContext.Request.Query["TenantId"].ToString()) ? int.Parse(HttpContext.Request.Query["TenantId"].ToString()) : 0;

            //}
            get; set;

        }
        public int ContactId
        {
            //get
            //{
            //    return !string.IsNullOrEmpty(HttpContext.Request.Query["ContactId"].ToString()) ? int.Parse(HttpContext.Request.Query["ContactId"].ToString()) : 0;
            //    ;
            //}
            get; set;

        }

        public int AreaId
        {
            //get
            //{
            //    return !string.IsNullOrEmpty(HttpContext.Request.Query["menu"].ToString()) ? int.Parse(HttpContext.Request.Query["menu"].ToString()) : 0;
            //    ;
            //}
            get; set;

        }
        public int LanguageBot
        {
            //    get
            //    {
            //        return !string.IsNullOrEmpty(HttpContext.Request.Query["LanguageBot"].ToString()) ? int.Parse(HttpContext.Request.Query["LanguageBot"].ToString()) : 0;
            //        ;
            //    }
            get; set;

        }

        public string Lang
        {
            get
            {
                return HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name;

                ;
            }


        }
       
        public string IsLoyal
        {
            get
            {
                return !string.IsNullOrEmpty(HttpContext.Request.Query["IsLoyalty"].ToString()) ? (string)HttpContext.Request.Query["IsLoyalty"].ToString() : "False";
                ;
            }
        }
        public string UrlKey
        {
            get
            {
                return !string.IsNullOrEmpty(HttpContext.Request.QueryString.ToString()) ? HttpContext.Request.QueryString.ToString() : "";
                ;
            }
        }
        public bool IsLoyalty { get; set; }
        public string Key { get; set; }

        public bool NotLoyalty { get; set; }
        public string CurrencyCode { get; private set; }

        public OrdersModel(
           ILogger<IndexModel> logger,
             IItemsAppService itemsAppService
            , ITenantServicesInfoAppService iTenantServicesInfoAppService
              , IViewRenderService viewRenderService
            , IOrdersAppService ordersAppService
           , IDBService dbService
            , ILoyaltyAppService iLoyaltyAppService
            ,IMenusAppService iMenusAppService
            )
        {
            _logger = logger;
            _IItemsAppService = itemsAppService;
            _ITenantServicesInfoAppService = iTenantServicesInfoAppService;
            _viewRenderService = viewRenderService;
            _IOrdersAppService = ordersAppService;
            _dbService = dbService;
            _ILoyaltyAppService = iLoyaltyAppService;
            _iMenusAppService = iMenusAppService;
        }



        public  void OnGet()
        {

            if (this.UrlKey != "")
            {
                var key = this.UrlKey;
                var arr = key.Split('?');
                this.Key = string.Join("?", arr.Skip(1));

                MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel();
                menuContcatKeyModel.KeyMenu = this.Key;
                var link = _iMenusAppService.MenuContactKeyGet(menuContcatKeyModel).Value;
                var uri = new Uri(link);
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (query.AllKeys.Count() > 3)
                {
                    this.TenantID = !string.IsNullOrEmpty(query.Get("TenantID")) ? int.Parse(query.Get("TenantID").ToString()) : 0;
                    this.ContactId = !string.IsNullOrEmpty(query.Get("ContactId")) ? int.Parse(query.Get("ContactId").ToString()) : 0;
                    this.AreaId = !string.IsNullOrEmpty(query.Get("Menu")) ? int.Parse(query.Get("Menu").ToString()) : 0;
                    this.LanguageBot = !string.IsNullOrEmpty(query.Get("LanguageBot")) ? int.Parse(query.Get("LanguageBot").ToString()) : 0;


                }



                // this.isLoyalty = false;
                List<OrderingMenuOrdersHistoryModel> ListOrderingMenuOrdersHistoryModel = new List<OrderingMenuOrdersHistoryModel>();
                var are = this.AreaId;

                OrderEntity orderEntity = _IOrdersAppService.GetAllByContactId(this.ContactId, this.TenantID, 0, 10);


                if (orderEntity.lstOrder != null)
                {
                    foreach (var order in orderEntity.lstOrder)
                    {
                        
                        ListOrderingMenuOrdersHistoryModel.Add(new OrderingMenuOrdersHistoryModel
                        {

                            Id = order.Order.Id,
                            OrderDate = order.Order.CreationTime,//.Date,
                            OrderNumber = order.OrderNumber,
                            OrderStatus = order.Order.OrderStatus,
                            OrderTime = order.Order.OrderTime.ToShortTimeString(),
                            OrderTotalPrice = order.Order.Total,
                            OrderType = order.Order.OrderType,
                            StreetName = order.StreetName,
                            BuildingNumber = order.BuildingNumber,
                            ApartmentNumber = order.ApartmentNumber,
                            FloorNo = order.FloorNo,
                            OrderTotalPoint = order.TotalPoints,
                            DeliveryCost = order.DeliveryCost,
                            IsItemOffer = order.Order.IsItemOffer,
                            ItemOffer = order.Order.ItemOffer.HasValue ? order.Order.ItemOffer.Value : 0,


                        });

                      
                    }

                }


                this.lstOrderingMenuOrdersHistoryModel = ListOrderingMenuOrdersHistoryModel;
            }

        }

        //List<OrderDetailDto> GetOrderDetail(int? TenantID, int? OrderId);
        public async Task<PartialViewResult> OnGetOrderDetailsAsync(int tenantId, long orderid,string orderstatus)

        {
            OrderingOrdersDetailsModel orderingOrdersDetailsModel =new OrderingOrdersDetailsModel();
            List<OrderingOrdersItemModel> lstOrderinItemModel = new List<OrderingOrdersItemModel>();
            List<GetOrderDetailForViewDto> oetOrderDetailForViewDto = await _IOrdersAppService.GetOrderDetailsForMenu(tenantId, orderid);


            if (oetOrderDetailForViewDto != null)
            {
                foreach (var item in oetOrderDetailForViewDto)
                {


                    OrderingOrdersItemModel orderingOrdersItemModel = new OrderingOrdersItemModel();

                    orderingOrdersItemModel.Name = item.ItemName;
                    orderingOrdersItemModel.NameEnglish = item.ItemNameEnglish;
                    orderingOrdersItemModel.TenantId = tenantId;
                    orderingOrdersItemModel.ItemId = item.OrderDetail.ItemId.HasValue ? item.OrderDetail.ItemId.Value : 0;
                    orderingOrdersItemModel.ImageUrl = item.ItemImageUrl;
                    orderingOrdersItemModel.Qty = item.OrderDetail.Quantity.HasValue ? item.OrderDetail.Quantity.Value : 0;
                    orderingOrdersItemModel.Total = item.OrderDetail.Total.HasValue ? item.OrderDetail.Total.Value : 0;
                    orderingOrdersItemModel.TotalPoint = Math.Round(item.OrderDetail.TotalLoyaltyPoints, 0);

                    orderingOrdersItemModel.Name = Translate(orderingOrdersItemModel.Name, orderingOrdersItemModel.NameEnglish);
             
                    orderingOrdersItemModel.ItemNote = item.OrderDetail.ItemNote;

                    if (item.OrderDetail.lstCategoryExtraOrderDetailsDto != null)
                    {
                        List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new List<OrderingCartItemSpecificationModel>();
                        List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModel = new List<OrderingCartItemAdditionalModel>();
                        //foreach (var itemextra in item.OrderDetail.lstCategoryExtraOrderDetailsDto)
                        //{
                        //    OrderingCartItemAdditionalModel objOrderingCartItemAdditionalModel = new OrderingCartItemAdditionalModel();
                        //    foreach (var itemextradetails in itemextra.lstExtraOrderDetailsDto)
                        //    {
                        //        objOrderingCartItemAdditionalModel.Total = itemextradetails.Total.HasValue ? itemextradetails.Total.Value : 0;
                        //        objOrderingCartItemAdditionalModel.Name = itemextradetails.Name;
                        //        objOrderingCartItemAdditionalModel.Id = itemextradetails.SpecificationChoiceId.HasValue ? itemextradetails.SpecificationChoiceId.Value : 0;
                        //        objOrderingCartItemAdditionalModel.TotalLoyaltyPoints = itemextradetails.TotalLoyaltyPoints.HasValue ? itemextradetails.TotalLoyaltyPoints.Value : 0;
                        //        objOrderingCartItemAdditionalModel.Qty = itemextradetails.Quantity.HasValue ? itemextradetails.Quantity.Value : 0;
                        //       // if (itemextradetails.TypeExtraDetails == 2)
                        //      //  {
                        //            objOrderingCartItemAdditionalModel.ItemAdditionsCategoryId = itemextradetails.SpecificationId.HasValue ? itemextradetails.SpecificationId.Value : 0;
                        //            lstOrderingCartItemAdditionalModel.Add(objOrderingCartItemAdditionalModel);
                        //      //  }
                        //    }



                        //}
                        orderingOrdersItemModel.lstOrderingOrderItemAdditionalModel = lstOrderingCartItemAdditionalModel;

                        foreach (var itemextra in item.OrderDetail.lstCategoryExtraOrderDetailsDto)
                        {
                     
                            foreach (var itemextradetails in itemextra.lstExtraOrderDetailsDto)
                            {
                                OrderingCartItemSpecificationModel objOrderingCartItemSpecificationModel1 = new OrderingCartItemSpecificationModel();

                                List<OrderingCartItemSpecificationChoicesModel> choiceModel = new List<OrderingCartItemSpecificationChoicesModel>();
                                OrderingCartItemSpecificationChoicesModel objChoiceModel = new OrderingCartItemSpecificationChoicesModel();
                                objChoiceModel.Total = itemextradetails.Total.HasValue ? itemextradetails.Total.Value : 0;
                                objChoiceModel.Name = itemextradetails.Name;
                                objChoiceModel.NameEnglish = itemextradetails.NameEnglish;
                                objChoiceModel.Id = itemextradetails.SpecificationChoiceId.HasValue ? itemextradetails.SpecificationChoiceId.Value : 0;
                                objChoiceModel.TotalLoyaltyPoints = itemextradetails.TotalLoyaltyPoints.HasValue ? itemextradetails.TotalLoyaltyPoints.Value : 0;
                                objChoiceModel.Qty = itemextradetails.Quantity.HasValue ? itemextradetails.Quantity.Value : 0;
                                // if (itemextradetails.TypeExtraDetails == 1)
                                //  {
                                objChoiceModel.Name = Translate(objChoiceModel.Name, objChoiceModel.NameEnglish);

                                objOrderingCartItemSpecificationModel1.SpecificationId = itemextradetails.SpecificationId.HasValue ? itemextradetails.SpecificationId.Value : 0;
                                    choiceModel.Add(objChoiceModel);
                                    objOrderingCartItemSpecificationModel1.lstOrderingCartItemSpecificationChoicesModel = choiceModel;
                                    lstOrderingCartItemSpecificationModel.Add(objOrderingCartItemSpecificationModel1);

                             //   }

                            }
                            orderingOrdersItemModel.lstOrderingOrderItemSpecificationModel = lstOrderingCartItemSpecificationModel;


                        }

                    }
                    lstOrderinItemModel.Add(orderingOrdersItemModel);
                    orderingOrdersDetailsModel.lstOrderingtemModel=lstOrderinItemModel;
                    orderingOrdersDetailsModel.OrderStatus = orderstatus;
                }

                }




           




            return Partial("Pages/Market/OrderDetails.cshtml", orderingOrdersDetailsModel);
        }


        //for a reorder 

        public async Task<IActionResult> OnPostReOrderAsync(int tenantId, string lstItems, int contactid)
        {
            // OrderingMenuTenantModel TenantInfo = JsonConvert.DeserializeObject<OrderingMenuTenantModel>(tenantInfo);
            List<OrderingCartItemModel> orderingCartItemModels = new List<OrderingCartItemModel>();
            List<OrderingOrdersItemModel> lstOrderinItemModel = JsonConvert.DeserializeObject<List<OrderingOrdersItemModel>>(lstItems);


            orderingCartItemModels = PrepareItems(lstOrderinItemModel, tenantId, contactid);

            return new JsonResult((orderingCartItemModels));

        }



        #region Private Method
        private List<OrderingCartItemModel> PrepareItems(List<OrderingOrdersItemModel> items, int tenantId, int contactid)
        {
            List<OrderingCartItemModel> orderingCartItemModel = new List<OrderingCartItemModel>();

            foreach (var item in items)
            {


                ItemDto itemDtos = _IItemsAppService.GetItemById(item.ItemId, tenantId, true);
                if (itemDtos != null)
                {
                    OrderingCartItemModel objOrderingCartItemModel = new OrderingCartItemModel();
                    objOrderingCartItemModel.IsLoyalClick = false;
                    objOrderingCartItemModel.Total = CalculatePrices(itemDtos, item);


                    objOrderingCartItemModel.Name = itemDtos.ItemName;
                    objOrderingCartItemModel.NameArabic = itemDtos.ItemName;
                    objOrderingCartItemModel.NameEnglish = itemDtos.ItemNameEnglish;
                    objOrderingCartItemModel.TenantId = tenantId;
                    objOrderingCartItemModel.Qty = item.Qty;
                    objOrderingCartItemModel.ContactId = contactid;
                    if (!itemDtos.IsQuantitative)
                    {
                        objOrderingCartItemModel.Price = item.Price;
                    }
                    objOrderingCartItemModel.Id = item.ItemId;
                    objOrderingCartItemModel.ImageUrl = item.ImageUrl;
                    objOrderingCartItemModel.ItemNote = item.ItemNote;


                    objOrderingCartItemModel.lstOrderingCartItemAdditionalModel = item.lstOrderingOrderItemAdditionalModel;
                    objOrderingCartItemModel.lstOrderingCartItemSpecificationModel = item.lstOrderingOrderItemSpecificationModel;

                    orderingCartItemModel.Add(objOrderingCartItemModel);

                }

            }



            return orderingCartItemModel;
        }







        //private List<OrderingCartItemAdditionalModel> prepareAdditions(List<OrderingCartItemAdditionalModel> additions)
        //{

        //    List<OrderingCartItemAdditionalModel> objOrderingCartItemAdditionalModel = new List<OrderingCartItemAdditionalModel>();







        //    return objOrderingCartItemAdditionalModel;
        //}


        private decimal CalculatePrices(ItemDto itemDtos, OrderingOrdersItemModel item)
        {
            if (!itemDtos.SKU.ToUpper().StartsWith("T") && itemDtos.Size.Contains("1 KG"))
            {
                itemDtos.IsQuantitative = true;

            }

            decimal total = 0;
            if (itemDtos != null)
            {

                if (itemDtos.OldPrice != 0 && itemDtos.OldPrice != null)
                {
                    if (item.TenantId != 34)
                    {
                        var o = itemDtos.OldPrice;
                        var p = itemDtos.Price;
                        item.Price = o.Value;
                    }
                }
                else
                {
                    item.Price = itemDtos.Price.Value;
                }




                if (itemDtos.IsQuantitative)
                {
                    total += CalculateQuantitative(itemDtos, item);

                }
                else
                {
                    total = item.Price;

                }



                total += CalculatePricesSpecifcation(itemDtos.ItemSpecifications, item);


                 total += CalculatePricesAdditional(itemDtos.additionsCategorysListModels, item);


                total = total * item.Qty;

                item.Total = total;
            }
            return total;
        }
        private decimal CalculatePricesAdditional(List<AdditionsCategorysListModel> additionsCategorysListModels, OrderingOrdersItemModel item)
        {
            decimal result = 0;
            List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModel = new();

            if (additionsCategorysListModels != null && additionsCategorysListModels.Count > 0)
            {
                if (item.lstOrderingOrderItemAdditionalModel != null && item.lstOrderingOrderItemAdditionalModel.Count > 0)
                {
                    foreach (var additionsCategorys in item.lstOrderingOrderItemAdditionalModel)
                    {
                        var additions = additionsCategorysListModels.Where(x => x.Id == additionsCategorys.ItemAdditionsCategoryId).FirstOrDefault();
                        if (additions != null)
                        {

                            var objadditions = additions.ItemAdditionDto.Where(x => x.ItemAdditionsId == additionsCategorys.Id).FirstOrDefault();
                            if (objadditions != null)
                            {
                                additionsCategorys.UnitPrice = objadditions.price.HasValue ? objadditions.price.Value : 0;

                                additionsCategorys.Price = additionsCategorys.UnitPrice.ToString();
                                additionsCategorys.UnitPrice = additionsCategorys.UnitPrice;
                                additionsCategorys.Total = additionsCategorys.UnitPrice * additionsCategorys.Qty;

                                result += additionsCategorys.Total;
                                lstOrderingCartItemAdditionalModel.Add(additionsCategorys);
                            }




                        }
                    }

                }
            }

            item.lstOrderingOrderItemAdditionalModel = lstOrderingCartItemAdditionalModel;
            return result;
        }




        private decimal CalculatePricesSpecifcation(List<ItemSpecification> itemSpecifications, OrderingOrdersItemModel item)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new();

            List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel = new();
            if (itemSpecifications != null && itemSpecifications.Count > 0)
            {
                if (item.lstOrderingOrderItemSpecificationModel != null && item.lstOrderingOrderItemSpecificationModel.Count > 0)
                {
                    foreach (var itemSpec in item.lstOrderingOrderItemSpecificationModel)
                    {
                        var specfication = itemSpecifications.Where(x => x.Id == itemSpec.SpecificationId).FirstOrDefault();
                        if (specfication != null)
                        {
                            foreach (var objChoices in itemSpec.lstOrderingCartItemSpecificationChoicesModel)
                            {
                                var choices = specfication.SpecificationChoices.Where(x => x.Id == objChoices.Id).FirstOrDefault();
                                if (choices != null)
                                {

                                    objChoices.UnitPrice = choices.Price.HasValue ? choices.Price.Value : 0;
                                    objChoices.Total = objChoices.UnitPrice;
                                    objChoices.Price = objChoices.UnitPrice.ToString();
                                    result = result + objChoices.UnitPrice;
                                    lstOrderingCartItemSpecificationChoicesModel.Add(objChoices);



                                }

                                itemSpec.lstOrderingCartItemSpecificationChoicesModel = lstOrderingCartItemSpecificationChoicesModel;
                                lstOrderingCartItemSpecificationModel.Add(itemSpec);



                            }
                        }

                    }


                }


            }



            return result;
        }
        private List<OrderingMenuItemSpecificationsModel> AddExtra(ItemDto itm)
        {
            List<OrderingMenuSpecificationChoiceModel> Listchoices = new List<OrderingMenuSpecificationChoiceModel>();


            var itmPrice = itm.Price;
            var itmPoint = itm.LoyaltyPoints;

            if (itm.OldPrice != null || itm.OldPrice > 0)

            {
                if (itm.OldPrice < itm.Price)
                {


                    itmPrice = itm.OldPrice;

                }
            }



            OrderingMenuSpecificationChoiceModel choices1 = new OrderingMenuSpecificationChoiceModel()
            {
                TenantId = itm.TenantId,
                Id = 1192,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itmPrice * decimal.Parse("0.25")),
                SpecificationChoiceDescription = "0.25 kg",
                SpecificationChoiceDescriptionEnglish = "0.25 kg",
                SpecificationId = 292,
                SKU = "",


            };

            OrderingMenuSpecificationChoiceModel choices2 = new OrderingMenuSpecificationChoiceModel
            {
                TenantId = itm.TenantId,
                Id = 1193,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itmPrice * decimal.Parse("0.50")),
                SpecificationChoiceDescription = "0.50 kg",
                SpecificationChoiceDescriptionEnglish = "0.50 kg",
                SpecificationId = 292,
                SKU = ""
                  ,



            };
            OrderingMenuSpecificationChoiceModel choices3 = new OrderingMenuSpecificationChoiceModel
            {
                TenantId = itm.TenantId,
                Id = 1194,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itmPrice * decimal.Parse("0.75")),
                SpecificationChoiceDescription = "0.75 kg",
                SpecificationChoiceDescriptionEnglish = "0.75 kg",
                SpecificationId = 292,
                SKU = ""
                  ,

            };
            OrderingMenuSpecificationChoiceModel choices4 = new OrderingMenuSpecificationChoiceModel
            {
                TenantId = itm.TenantId,
                Id = 1195,
                IsInService = true,
                LanguageBotId = 1,
                Price = (itmPrice * decimal.Parse("1.0")),
                SpecificationChoiceDescription = "1.00 kg",
                SpecificationChoiceDescriptionEnglish = "1.00 kg",
                SpecificationId = 292,
                SKU = ""
                  ,
                LoyaltyPoints = itmPoint * decimal.Parse("1"),

            };


            choices1.Price = PriceDigit(choices1.Price);
            choices2.Price = PriceDigit(choices2.Price);
            choices3.Price = PriceDigit(choices3.Price);
            choices4.Price = PriceDigit(choices4.Price);

            Listchoices.Add(choices1);
            Listchoices.Add(choices2);
            Listchoices.Add(choices3);
            Listchoices.Add(choices4);

            List<OrderingMenuItemSpecificationsModel> ItemSpecificationsList = new List<OrderingMenuItemSpecificationsModel>();

            OrderingMenuItemSpecificationsModel itemSpecification = new OrderingMenuItemSpecificationsModel
            {
                Id = 292,
                SpecificationDescription = "الرجاء اختيار الوزن ",
                SpecificationDescriptionArabic = "الرجاء اختيار الوزن ",
                IsMultipleSelection = false,
                IsRequired = true,
                lstOrderingMenuSpecificationChoiceModel = Listchoices,
                MaxSelectNumber = 0,
                // ItemSpecificationId = Convert.ToInt32(dataSetItemSpecification.Tables[1].Rows[j]["ItemSpecificationId"]),
                Priority = 0,
                SpecificationDescriptionEnglish = "Please select a weight",
                TenantId = itm.TenantId,


            };

            ItemSpecificationsList.Add(itemSpecification);



            return ItemSpecificationsList;
        }
        private decimal? PriceDigit(decimal? price)
        {

            if (price.HasValue)
            {


                if (this.CurrencyCode == "IQD" || this.TenantID == 46)
                {
                    price = Math.Round(price.Value, 0);


                }
                else
                {
                    price = Math.Round(price.Value, 2);

                }
                return price.Value;

            }
            return price;
        }





        private decimal CalculateQuantitative(ItemDto ItemDtos, OrderingOrdersItemModel item)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new List<OrderingCartItemSpecificationModel>();

            List<OrderingMenuItemSpecificationsModel> itms = AddExtra(ItemDtos);


            foreach (var objSpecification in item.lstOrderingOrderItemSpecificationModel)
            {
                var specfication = itms.Where(x => x.Id == objSpecification.SpecificationId).FirstOrDefault();
                if (specfication != null)
                {
                    List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel = new List<OrderingCartItemSpecificationChoicesModel>();
                    foreach (var objChoices in objSpecification.lstOrderingCartItemSpecificationChoicesModel)
                    {
                        var choices = specfication.lstOrderingMenuSpecificationChoiceModel.Where(x => x.Id == objChoices.Id).FirstOrDefault();
                        if (choices != null)
                        {

                            //  objChoices.UnitPrice = 0;
                            var UnitPrice = choices.Price.HasValue ? choices.Price.Value : 0;
                            objChoices.Total = UnitPrice;
                            objChoices.Price = UnitPrice.ToString();
                            result = result + UnitPrice;
                            item.Price = UnitPrice;
                            lstOrderingCartItemSpecificationChoicesModel.Add(objChoices);
                        }


                    }
                    objSpecification.lstOrderingCartItemSpecificationChoicesModel = lstOrderingCartItemSpecificationChoicesModel;
                    lstOrderingCartItemSpecificationModel.Add(objSpecification);
                }
            }
            return result;
        }

        private string Translate(string arabicName, string englishName)
        {


            var currentLang = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name;

            if (currentLang == "en" || currentLang == "en-US")
            {

                if (englishName != "")
                {
                    return englishName;
                }
                return arabicName;



            }
            else
            {
                if (arabicName != "")
                {
                    return arabicName;
                }
                return englishName;

            }

        }
        #endregion
    }
}
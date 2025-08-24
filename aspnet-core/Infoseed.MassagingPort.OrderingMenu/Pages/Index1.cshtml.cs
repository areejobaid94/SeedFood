using Framework.Payment;
using Infoseed.MassagingPort.OrderingMenu.Helper;
using Infoseed.MassagingPort.OrderingMenu.Pages.Model;
using Infoseed.MessagingPortal;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Infoseed.MessagingPortal.TenantServicesInfo;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using ItemSpecification = Infoseed.MessagingPortal.Items.ItemSpecification;
using SpecificationChoice = Infoseed.MessagingPortal.Items.SpecificationChoice;



//using Infoseed.Messaging.Portal.BotService;
//using Infoseed.Messaging.Portal.BotService.SQLServices;
//using Infoseed.Messaging.Portal.BotService.Model;

namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class Index1Model : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IMenuCategoriesAppService _IMenuCategoriesAppService;
        private readonly IItemsAppService _IItemsAppService;

        private readonly IHttpContextAccessor _context;


        private readonly ITenantServicesInfoAppService _ITenantServicesInfoAppService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IOrdersAppService _IOrdersAppService;



        //public IBotApis _botApis;
        //private readonly CommonLocalizationService _CommonLocalizationService;
        private IDBService _dbService;
        private readonly IDocumentClient _IDocumentClient;
        //private readonly IContactsAppService _contactService;
        //private IHubContext<SignalR.TeamInboxHub> _hub;
        public List<OrderingMenuCategoryModel> lstOrderingMenuCategoryModel { get; set; }
        public List<OrderingMenuMenuModel> listOrderingMenuMenuModel { get; set; }
        public List<OrderingMenuSubCategoryModel> listOrderingMenuSubCategoryModel { get; set; }
        public List<OrderingMenuItemModel> listOrderingMenuItemModel = new();
        public OrderingMenuTenantModel OrderingMenuTenantModel { get; set; }
        public OrderingMenuLoyaltyModel OrderingMenuLoyaltyModel { get; set; }
        public System.Globalization.CultureInfo CurrentUICulture { get; set; }
        private IMenusAppService _iMenusAppService;
        private ILoyaltyAppService _ILoyaltyAppService;

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
        public string UrlKey
        {
            get
            {
                return !string.IsNullOrEmpty(HttpContext.Request.QueryString.ToString()) ? HttpContext.Request.QueryString.ToString() : "";
                ;
            }
        }
        public string Lang
        {
            get
            {
                return HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name;

                ;
            }


        }
        public string OrderType { get; set; }

        public string CurrencyCode { get; set; }
        public string Key { get; set; }
        public bool IsLoyalty { get; set; }
        public Index1Model(ILogger<IndexModel> logger,
               IMenuCategoriesAppService iMenuCategoriesAppService
            , IItemsAppService itemsAppService
            , ITenantServicesInfoAppService iTenantServicesInfoAppService
              , IViewRenderService viewRenderService
            , IOrdersAppService ordersAppService
           , IDBService dbService
            , IMenusAppService iMenusAppService,
            IHttpContextAccessor context,
        //    IContactsAppService contactsAppService,
            IDocumentClient iDocumentClient
            , ILoyaltyAppService iLoyaltyAppService
            )
        {
            _logger = logger;
            _IMenuCategoriesAppService = iMenuCategoriesAppService;
            _IItemsAppService = itemsAppService;
            _ITenantServicesInfoAppService = iTenantServicesInfoAppService;
            _viewRenderService = viewRenderService;
            _IOrdersAppService = ordersAppService;
            _dbService = dbService;
            _iMenusAppService = iMenusAppService;
            _context = context;
            _IDocumentClient = iDocumentClient;
            _ILoyaltyAppService = iLoyaltyAppService;

        }


        public async void OnGet()
        {

            //var request = HttpContext.Request;
            //var _baseURL = $"{request.Scheme}://{request.Host}";
            //var UrlKey =  HttpContext.Request.QueryString;


            // Response.Cookies.Delete(".AspNetCore.Culture");

            if (this.UrlKey != "")
            {
                var key = this.UrlKey;
                var arr = key.Split('?');
                this.Key = string.Join("?", arr.Skip(1));

                MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel();
                menuContcatKeyModel.KeyMenu = this.Key;
                var link = _iMenusAppService.MenuContactKeyGetNew(menuContcatKeyModel);


                if (link != null)
                {

                    var uri = new Uri(link.Value);
                    var query = HttpUtility.ParseQueryString(uri.Query);



                    if (query.AllKeys != null && query.AllKeys.Count() > 3)
                    {
                        this.TenantID = !string.IsNullOrEmpty(query.Get("TenantID")) ? int.Parse(query.Get("TenantID").ToString()) : 0;
                        this.ContactId = !string.IsNullOrEmpty(query.Get("ContactId")) ? int.Parse(query.Get("ContactId").ToString()) : 0;
                        this.AreaId = !string.IsNullOrEmpty(query.Get("Menu")) ? int.Parse(query.Get("Menu").ToString()) : 0;
                        this.LanguageBot = !string.IsNullOrEmpty(query.Get("LanguageBot")) ? int.Parse(query.Get("LanguageBot").ToString()) : 0;
                        this.OrderType = !string.IsNullOrEmpty(query.Get("OrderType")) ? query.Get("OrderType").ToString() : "";

                    }

                    //if (this.LanguageBot == 0)
                    //{
                    //    OnPostSetLanguage("ar", "");
                    //}

                    // OnGetTenantInfo(TenantID, ContactId, AreaId);

                    //for regular Menu
                    if (this.TenantID != 0)
                    {

                        //fill tenant and contact Info
                        FillTenantInfo();

                        if (false)//&& this.TenantID == 14 || this.TenantID == 46 || this.TenantID == 15 || this.TenantID == 29 || this.TenantID == 24 || this.TenantID == 20 || this.TenantID == 19 || this.TenantID == 35 || this.TenantID == 9

                        {
                            //this.OrderingMenuTenantModel.IsApplyLoyalty = false;
                            //this.IsLoyalty = false;

                            var result = _IMenuCategoriesAppService.GetCategoryWithItem(this.TenantID, this.AreaId);
                            if (result != null)
                            {
                                OrderingMenuMenuModel orderingMenuMenuModel = new();

                                this.lstOrderingMenuCategoryModel = new List<OrderingMenuCategoryModel>();
                                foreach (var item in result)
                                {
                                    if (item.listItemInCategories != null)
                                    {
                                        OrderingMenuCategoryModel objOrderingMenuCategoryModel = new()
                                        {
                                            MenuPriority = item.menuPriority,
                                            MenuId = item.menuId,
                                            CategoryId = item.categoryId,

                                            CategoryName = item.categoryName,


                                            CategoryNameEnglish = item.categoryNameEnglish,
                                            IsSubCategory = item.isSubCategory,

                                            ///  objOrderingMenuCategoryModel =item.listItemInCategories;
                                            ///  


                                            lstOrderingMenuItemModel = PrepareItemList(item.listItemInCategories).OrderBy(x => (x.Priority)).ToList(),



                                            BgImg = item.bgImg
                                        };


                                        objOrderingMenuCategoryModel.CategoryName = Translate(item.categoryName, item.categoryNameEnglish);
                                        this.lstOrderingMenuCategoryModel.Add(objOrderingMenuCategoryModel);
                                    }

                                }

                                //orderingMenuMenuModel.lstOrderingMenuCategoryModel = this.lstOrderingMenuCategoryModel;
                                //  listOrderingMenuMenuModel.Add(orderingMenuMenuModel);
                            }

                            //  this.OrderingMenuCategoryModel = orderingMenuCategoryModel;
                        }

                        else
                        {
                            List<OrderingMenuMenuModel> listOrderingMenuMenuModel = new();

                            //if (this.IsLoyalty) 
                            //{
                            //    OrderingMenuMenuModel orderingloyaltyMenuModel = new();
                            //    orderingloyaltyMenuModel = MenuLoyalty();
                            //    listOrderingMenuMenuModel.Add(orderingloyaltyMenuModel);
                            //}
                            var advResult = _iMenusAppService.GetMenusWithDetails(this.TenantID, this.AreaId);
                            if (advResult != null)
                            {
                                int count = 0;
                                foreach (var menuItem in advResult)
                                {
                                    OrderingMenuMenuModel orderingMenuMenuModel = new OrderingMenuMenuModel();
                                    orderingMenuMenuModel.Id = menuItem.Id;
                                    orderingMenuMenuModel.MenuName = menuItem.MenuName;

                                    orderingMenuMenuModel.MenuNameEnglish = menuItem.MenuNameEnglish;
                                    orderingMenuMenuModel.Priority = menuItem.Priority;
                                    orderingMenuMenuModel.ImageUri = menuItem.ImageUri;

                                    orderingMenuMenuModel.lstOrderingMenuCategoryModel = fillGetCategorysModel(menuItem.CategoryEntity).OrderBy(x => x.MenuPriority).ToList();

                                    orderingMenuMenuModel.MenuName = Translate(menuItem.MenuName, menuItem.MenuNameEnglish);
                                    
                                    if (count == 0)
                                    {
                                        if (this.IsLoyalty)//this.IsLoyalty
                                        {
                                            OrderingMenuMenuModel orderingloyaltyMenuModel = new();
                                            orderingloyaltyMenuModel = MenuLoyalty();
                                            orderingMenuMenuModel.lstOrderingMenuCategoryLoyaltyModel = orderingloyaltyMenuModel.lstOrderingMenuCategoryModel; 
                                        }
                                    }
                                    listOrderingMenuMenuModel.Add(orderingMenuMenuModel);
                                    count++;
                                }
                                //var temp = listOrderingMenuMenuModel[0];
                                //listOrderingMenuMenuModel[0] = listOrderingMenuMenuModel[1];
                                //listOrderingMenuMenuModel[1] = temp;
                                this.listOrderingMenuMenuModel = listOrderingMenuMenuModel;
                            }
                        }
                    }
                }
                else
                {
                }
            }
        }


        public void OnPostSetLanguage(string culture, string returnUrl)
        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US", culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );




        }

        public async Task<IActionResult> OnGetSubCategoriesandItems(int tenantID, bool isLoyaltyApplay, string subCateoryname, string currencyCode, int areaId, int PageNumber, string loyaltyModelJson = "", long subCategoryid = 0, string search = "", int IsSort = 0)
        {

            if (tenantID != 0)
            { 
                OrderingMenuItemPagenationModel orderingMenuItemPagenationModel = new OrderingMenuItemPagenationModel();
                orderingMenuItemPagenationModel.PageSize = 20;//pageSize;
                orderingMenuItemPagenationModel.CurrentPage = PageNumber;
                orderingMenuItemPagenationModel.SubCategoryId = subCategoryid;
                orderingMenuItemPagenationModel.IsApplayLoyal = isLoyaltyApplay;
                if (PageNumber == 0)
                {
                    orderingMenuItemPagenationModel.SubCategoryName = subCateoryname;


                }
                orderingMenuItemPagenationModel.Search = "";
                if (search != null)
                {
                    orderingMenuItemPagenationModel.Search = search;
                }

                List<OrderingMenuItemModel> lstOrderingMenuItemModel = new List<OrderingMenuItemModel>();



                int totalCount = 0;
                List<ItemDto> advResult = new List<ItemDto>();



                if (subCategoryid == 0 && isLoyaltyApplay)
                {
                    advResult = _IItemsAppService.GetLoyaltyItems(tenantID, areaId, 1, PageNumber, orderingMenuItemPagenationModel.PageSize, out totalCount, orderingMenuItemPagenationModel.Search);

                }
                else if (tenantID == 34)
                {
                    advResult = _IItemsAppService.GetItemCTown(tenantID, areaId, subCategoryid, out totalCount, orderingMenuItemPagenationModel.PageSize, PageNumber, orderingMenuItemPagenationModel.Search, IsSort);

                }
                else
                {
                    advResult = _IItemsAppService.GetItemsBySubGategory(tenantID, areaId, subCategoryid, 1, PageNumber, orderingMenuItemPagenationModel.PageSize, out totalCount, orderingMenuItemPagenationModel.Search);
      
                    //if (search != null && isLoyaltyApplay == true)
                    //{
                    //    List<ItemDto> additionalItems = new List<ItemDto>();// Create a new list to store additional items
                    //    additionalItems = _IItemsAppService.GetLoyaltyItems(tenantID, areaId, 1, PageNumber, orderingMenuItemPagenationModel.PageSize, out totalCount, orderingMenuItemPagenationModel.Search);
                    //    advResult.AddRange(additionalItems); // Add the additional items to the existing advResult
                    //}                  
                }



                orderingMenuItemPagenationModel.Count = totalCount;


                foreach (var item in advResult)
                {
                    OrderingMenuItemModel orderingMenuItemModel = new OrderingMenuItemModel();
                    orderingMenuItemModel.Price = item.Price;
                    orderingMenuItemModel.Priority = item.Priority;
                    orderingMenuItemModel.ItemName = item.ItemName;
                    orderingMenuItemModel.ItemNameEnglish = item.ItemNameEnglish;
                    orderingMenuItemModel.ItemNameArabic = item.ItemName;

                    orderingMenuItemModel.Id = item.Id;
                    orderingMenuItemModel.ImageUri = item.ImageUri;
                    orderingMenuItemModel.ItemDescription = item.ItemDescription;
                    orderingMenuItemModel.ItemDescriptionArabic = item.ItemDescription;
                    orderingMenuItemModel.ItemDescriptionEnglish = item.ItemDescriptionEnglish;
                    orderingMenuItemModel.SubCategoryName = item.SubCategoryName;
                    orderingMenuItemModel.SubCategoryNameEnglish = item.SubCategoryNameEnglish;
                    orderingMenuItemModel.Price = item.Price;
                    orderingMenuItemModel.OldPrice = item.OldPrice;
                    orderingMenuItemModel.ItemNameEnglish = item.ItemNameEnglish;
                    orderingMenuItemModel.Discount = item.Discount;
                    orderingMenuItemModel.InServiceIds = item.InServiceIds;
                    orderingMenuItemModel.AreaId = areaId.ToString();

                    if (item.InServiceIds != null && item.InServiceIds != "" && item.InServiceIds != ",")
                    {

                        if (item.InServiceIds.Contains(areaId.ToString()) || areaId == 0)
                        {
                            orderingMenuItemModel.IsInService = true;
                        }
                        else
                        {
                            orderingMenuItemModel.IsInService = false;

                        }
                        
                    }
                    else
                    {
                        //objOrderingMenuItemModels.IsInService = item.IsInService;
                        if (item.InServiceIds != "," && item.InServiceIds != null)
                        {

                            orderingMenuItemModel.IsInService = false;

                        }
                        else 
                        {

                            orderingMenuItemModel.IsInService = item.IsInService;
                        }


                    }

                    //if (item.InServiceIds!=null)
                    //{

                    //    if (item.InServiceIds.Contains(areaId.ToString()) || areaId==0)
                    //    {
                    //        orderingMenuItemModel.IsInService=true;
                    //    }
                    //    else
                    //    {
                    //        orderingMenuItemModel.IsInService=false;

                    //    }

                    //}
                    //else
                    //{
                    //    if (item.InServiceIds=="")
                    //    {

                    //        orderingMenuItemModel.IsInService =false;

                    //    }
                    //    else
                    //    {
                    //        orderingMenuItemModel.IsInService = item.IsInService;
                    //    }


                    //}


                    //orderingMenuItemModel.IsInService = item.IsInService;
                    orderingMenuItemModel.CurrencyCode = currencyCode;

                    orderingMenuItemModel.IsQuantitative = item.IsQuantitative;
                    // orderingMenuItemModel.DateTo = item.DateTo.Value;




                    //Here is the old price and the prize because sending the correct price before the discount
                    //......................
                    if (item.OldPrice != 0 && item.OldPrice != null)
                    {


                        if (tenantID != 34)
                        {
                            var o = item.OldPrice;
                            var p = item.Price;
                            orderingMenuItemModel.OldPrice = p;
                            orderingMenuItemModel.Price = o;

                        }
                        else
                        {
                            if (item.OldPrice != item.Price && item.OldPrice > item.Price)
                            {

                                var datNow = DateTime.Now.AddHours(AppSettingsModel.AddHour);

                                if (item.DateTo != null)
                                {

                                    var x1 = (item.Price / item.OldPrice);
                                    var x2 = 100 - (x1 * 100);
                                    var xFormat = String.Format("{0:0.##}", x2);
                                    orderingMenuItemModel.Discount = xFormat.ToString();

                                }

                            }
                            else
                            {
                                item.OldPrice = 0;
                            }

                        }
                    }



                    orderingMenuItemModel.OldPrice = PriceDigit(orderingMenuItemModel.OldPrice);
                    orderingMenuItemModel.Price = PriceDigit(orderingMenuItemModel.Price);


                    if (isLoyaltyApplay)
                    {
                        LoyaltyModel loyaltyModel = PrepareLoyaltyModel(loyaltyModelJson);

                        orderingMenuItemModel.IsLoyal = item.IsLoyal;
                        orderingMenuItemModel.LoyaltyDefinitionId = item.LoyaltyDefinitionId;

                        orderingMenuItemModel.LoyaltyPoints = PointDigit(item.LoyaltyPoints);

                        orderingMenuItemModel.IsOverrideLoyaltyPoints = item.IsOverrideLoyaltyPoints;
                        orderingMenuItemModel.OriginalLoyaltyPoints = item.OriginalLoyaltyPoints;

                        var model = _ILoyaltyAppService.GetItemLoyaltyLog(orderingMenuItemModel.Id, tenantID);
                        orderingMenuItemModel.LoyaltyDefinitionId = model.LoyaltyDefinitionId;
                        orderingMenuItemModel.OriginalLoyaltyPoints = model.OriginalLoyaltyPoints;

                        if (!item.IsLoyal)
                        {
                            orderingMenuItemModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(orderingMenuItemModel.Price, orderingMenuItemModel.LoyaltyPoints, orderingMenuItemModel.OriginalLoyaltyPoints, 0, loyaltyModel);

                        }
                        else
                        {

                            if (subCategoryid == 0)
                            {
                                if (item.OldPrice != 0 && item.OldPrice != null)
                                {


                                    if (tenantID != 34)
                                    {
                                        var o = item.Price;
                                        var p = item.OldPrice;
                                        orderingMenuItemModel.OldPrice = p;
                                        orderingMenuItemModel.Price = o;

                                    }
                                    else
                                    {
                                        if (item.OldPrice != item.Price && item.OldPrice > item.Price)
                                        {

                                            var datNow = DateTime.Now.AddHours(AppSettingsModel.AddHour);

                                            if (item.DateTo != null)
                                            {

                                                var x1 = (item.Price / item.OldPrice);
                                                var x2 = 100 - (x1 * 100);
                                                var xFormat = String.Format("{0:0.##}", x2);
                                                orderingMenuItemModel.Discount = xFormat.ToString();

                                            }

                                        }
                                        else
                                        {
                                            item.OldPrice = 0;
                                        }

                                    }
                                }



                                orderingMenuItemModel.OldPrice = PriceDigit(orderingMenuItemModel.OldPrice);
                                orderingMenuItemModel.Price = PriceDigit(orderingMenuItemModel.Price);
                                orderingMenuItemModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(orderingMenuItemModel.Price, orderingMenuItemModel.LoyaltyPoints, model.OriginalLoyaltyPoints, model.LoyaltyDefinitionId, loyaltyModel);

                            }
                            else
                            {
                                orderingMenuItemModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToCustomerPoints(orderingMenuItemModel.Price, orderingMenuItemModel.LoyaltyPoints, model.OriginalLoyaltyPoints, model.LoyaltyDefinitionId, loyaltyModel);

                            }


                        }
                    }

                    if (item.ItemDescription == "")
                    {
                        orderingMenuItemModel.ItemDescription = item.Size;
                        orderingMenuItemModel.ItemDescriptionEnglish = item.Size;
                        orderingMenuItemModel.ItemDescriptionArabic = item.Size;



                    }

                    orderingMenuItemModel.HasOption = true;

                    //old price before transfer
                    //.....



                    if (isLoyaltyApplay)
                    {

                        orderingMenuItemModel.LoyaltycreditPoints = CalculatPointcredit(tenantID, orderingMenuItemModel.Price, loyaltyModelJson);
                        orderingMenuItemModel.LoyaltycreditPoints = PointDigit(orderingMenuItemModel.LoyaltycreditPoints);
                    }

                    orderingMenuItemModel.ItemName = Translate(orderingMenuItemModel.ItemNameArabic, orderingMenuItemModel.ItemNameEnglish);
                    orderingMenuItemModel.ItemDescription = Translate(orderingMenuItemModel.ItemDescriptionArabic, orderingMenuItemModel.ItemDescriptionEnglish);

                    
                        orderingMenuItemModel.TenantId = tenantID;
                    

                        lstOrderingMenuItemModel.Add(orderingMenuItemModel);
                    //  this.listOrderingMenuItemModel.Add(orderingMenuItemModel);

                }
                // get pagination info for the current page
                // Pager = new Pager(lstOrderingMenuItemModel.Count(), page, PageSize, MaxPages);
                // Pager = new Pager(lstOrderingMenuItemModel.Count(), page, 3, 5);

                orderingMenuItemPagenationModel.lstOrderingMenuItemModel = lstOrderingMenuItemModel; //.OrderBy(x => (x.Priority)).ToList();



                if (orderingMenuItemPagenationModel.lstOrderingMenuItemModel != null && orderingMenuItemPagenationModel.lstOrderingMenuItemModel.Count > 0)
                {

                    if (subCategoryid == 0 && isLoyaltyApplay)
                    {
                        return Partial("Pages/Shared/_LoyaltyItems2.cshtml", orderingMenuItemPagenationModel);

                    }
                    else
                    {

                        return Partial("Pages/Shared/_SubCategoryItems2.cshtml", orderingMenuItemPagenationModel);

                    }

                }
                else
                {

                    return new JsonResult(new { msg = "No items found", StatusCode = 0 });

                }
            }
            else
            {
                return new JsonResult(new { msg = "No items found", StatusCode = 0 });
            }
        }
        //public void OnGetCategoriesandSub(long id)
        //{

        //    this.lstOrderingMenuCategoryModel = this.lstOrderingMenuCategoryModel.Where(x => x.CategoryId == id).ToList();
        //}




        public PartialViewResult OnGetProuductDetails(int tenantId, bool isLoyaltyApplay, string loyaltyModelJson, long id, long subcatId)

        {
            
            OrderingMenuProductDetailsModel orderingMenuProductDetailsModel = new OrderingMenuProductDetailsModel();
            ItemDto itemDtos = _IItemsAppService.GetItemById(id, tenantId, true);
            LoyaltyModel loyaltyModel = new LoyaltyModel();
            if (itemDtos != null)
            {

                
                orderingMenuProductDetailsModel.TenantId = tenantId;
                orderingMenuProductDetailsModel.Id = itemDtos.Id;
                orderingMenuProductDetailsModel.Name = itemDtos.ItemName;
                orderingMenuProductDetailsModel.NameEnglish = itemDtos.ItemNameEnglish;
                orderingMenuProductDetailsModel.NameArabic = itemDtos.ItemName;
                orderingMenuProductDetailsModel.Price = itemDtos.Price;
                orderingMenuProductDetailsModel.OldPrice = itemDtos.OldPrice;
                orderingMenuProductDetailsModel.ItemDescription = itemDtos.ItemDescription;
                orderingMenuProductDetailsModel.ItemDescriptionArabic = itemDtos.ItemDescription;
                orderingMenuProductDetailsModel.ItemDescriptionEnglish = itemDtos.ItemDescriptionEnglish;

                orderingMenuProductDetailsModel.ImageUri = itemDtos.ImageUri;
                orderingMenuProductDetailsModel.lstItemImages = itemDtos.lstItemImages;
                orderingMenuProductDetailsModel.Discount = itemDtos.Discount;
                //orderingMenuProductDetailsModel.TotalOrderNotComblet = itemDtos.TotalOrderNotComblet;
                if (itemDtos.InServiceIds!=null)
                {

                    if (itemDtos.InServiceIds.Contains(this.AreaId.ToString()) || this.AreaId==0)
                    {
                        orderingMenuProductDetailsModel.IsInService=true;
                    }
                    else
                    {
                        orderingMenuProductDetailsModel.IsInService=false;

                    }

                }
                else
                {

                    if (itemDtos.InServiceIds=="")
                    {

                        orderingMenuProductDetailsModel.IsInService =false;

                    }
                    else
                    {

                        orderingMenuProductDetailsModel.IsInService = itemDtos.IsInService;
                    }

                }

                // orderingMenuProductDetailsModel.IsInService = itemDtos.IsInService;




                orderingMenuProductDetailsModel.ContactId = this.ContactId;
                orderingMenuProductDetailsModel.ViewPrice = itemDtos.Price.HasValue ? orderingMenuProductDetailsModel.ViewPrice : 0;
                orderingMenuProductDetailsModel.ViewPoint = itemDtos.LoyaltyPoints;
                orderingMenuProductDetailsModel.IsQuantitative = itemDtos.IsQuantitative;
                orderingMenuProductDetailsModel.IsLoyalClick = false;
                orderingMenuProductDetailsModel.IsApplayLoyalty = isLoyaltyApplay;
                //Here is the old price and the prize because sending the correct price before the discount
                //......................
                if (itemDtos.OldPrice != null && tenantId != 34)
                {
                    var o = itemDtos.OldPrice;
                    var p = itemDtos.Price;

                    orderingMenuProductDetailsModel.OldPrice = p;
                    orderingMenuProductDetailsModel.Price = o;
                }

                orderingMenuProductDetailsModel.OldPrice = PriceDigit(orderingMenuProductDetailsModel.OldPrice);
                orderingMenuProductDetailsModel.Price = PriceDigit(orderingMenuProductDetailsModel.Price);
                orderingMenuProductDetailsModel.ViewPrice = PriceDigit(orderingMenuProductDetailsModel.Price);


                //for applay loyalty on tennat
                if (isLoyaltyApplay)
                {


                    loyaltyModel = PrepareLoyaltyModel(loyaltyModelJson);

                    orderingMenuProductDetailsModel.IsLoyal = itemDtos.IsLoyal;

                    orderingMenuProductDetailsModel.LoyaltyPoints = PointDigit(itemDtos.LoyaltyPoints);


                    orderingMenuProductDetailsModel.IsOverrideLoyaltyPoints = itemDtos.IsOverrideLoyaltyPoints;
                    orderingMenuProductDetailsModel.OriginalLoyaltyPoints = itemDtos.OriginalLoyaltyPoints;
                    orderingMenuProductDetailsModel.LoyaltyDefinitionId = itemDtos.LoyaltyDefinitionId;

                    if (!orderingMenuProductDetailsModel.IsLoyal)
                    {

                        orderingMenuProductDetailsModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(orderingMenuProductDetailsModel.Price, orderingMenuProductDetailsModel.LoyaltyPoints, orderingMenuProductDetailsModel.OriginalLoyaltyPoints, 0, loyaltyModel);
                        itemDtos.LoyaltyPoints = orderingMenuProductDetailsModel.LoyaltyPoints;
                    }
                    else
                    {
                        if (subcatId == 0)
                        {
                            if (itemDtos.OldPrice != null && tenantId != 34)
                            {
                                var o = itemDtos.Price;
                                var p = itemDtos.OldPrice;

                                orderingMenuProductDetailsModel.OldPrice = p;
                                orderingMenuProductDetailsModel.Price = o;
                            }

                            orderingMenuProductDetailsModel.OldPrice = PriceDigit(orderingMenuProductDetailsModel.OldPrice);
                            orderingMenuProductDetailsModel.Price = PriceDigit(orderingMenuProductDetailsModel.Price);
                            orderingMenuProductDetailsModel.ViewPrice = PriceDigit(orderingMenuProductDetailsModel.Price);

                            orderingMenuProductDetailsModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(orderingMenuProductDetailsModel.Price, orderingMenuProductDetailsModel.LoyaltyPoints, orderingMenuProductDetailsModel.OriginalLoyaltyPoints, orderingMenuProductDetailsModel.LoyaltyDefinitionId, loyaltyModel);
                            itemDtos.LoyaltyPoints = orderingMenuProductDetailsModel.LoyaltyPoints;
                        }
                        else
                        {
                            orderingMenuProductDetailsModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToCustomerPoints(orderingMenuProductDetailsModel.Price, orderingMenuProductDetailsModel.LoyaltyPoints, orderingMenuProductDetailsModel.OriginalLoyaltyPoints, orderingMenuProductDetailsModel.LoyaltyDefinitionId, loyaltyModel);
                            itemDtos.LoyaltyPoints = orderingMenuProductDetailsModel.LoyaltyPoints;
                        }
                    }
                    if (subcatId == 0)
                    {
                        orderingMenuProductDetailsModel.IsLoyalClick = true;
                        if (itemDtos.IsQuantitative)
                        {
                            orderingMenuProductDetailsModel.ViewPoint = 0;
                        }
                        else
                        {
                            orderingMenuProductDetailsModel.ViewPoint = orderingMenuProductDetailsModel.LoyaltyPoints;
                        }
                    }
                }

                //if ( !itemDtos.SKU.ToUpper().StartsWith("T") && itemDtos.Size.Contains("1 KG"))
                //{
                //    orderingMenuProductDetailsModel.IsQuantitative = true;

                //}






                if (itemDtos.ItemDescription == null || itemDtos.ItemDescription == "")
                {
                    orderingMenuProductDetailsModel.ItemDescription = itemDtos.Size;
                    orderingMenuProductDetailsModel.ItemDescriptionArabic = itemDtos.Size;
                    orderingMenuProductDetailsModel.ItemDescriptionEnglish = itemDtos.Size;



                }







                if (itemDtos.ItemSpecifications != null)
                {

                    orderingMenuProductDetailsModel.lstOrderingMenuItemSpecificationsModel = PrepareSpesifcationList(isLoyaltyApplay, itemDtos.ItemSpecifications, loyaltyModel , subcatId);

                }
                if (itemDtos.additionsCategorysListModels != null)
                {

                    


                    orderingMenuProductDetailsModel.lstOrderingMenuItemAdditionsCategorysModel = PrepareAdditionsList(isLoyaltyApplay, itemDtos.additionsCategorysListModels, loyaltyModel, subcatId);

                }

                //old price before transfer
                //.....

                decimal priceBeforeQuantitative = (decimal)orderingMenuProductDetailsModel.ViewPrice;
                if (orderingMenuProductDetailsModel.IsQuantitative)
                {


                    var itemAupdate = AddExtra(itemDtos, isLoyaltyApplay, loyaltyModel, orderingMenuProductDetailsModel.IsLoyalClick);
                    foreach (var item in itemAupdate)
                    {

                        orderingMenuProductDetailsModel.lstOrderingMenuItemSpecificationsModel.Add(item);

                    }
                    orderingMenuProductDetailsModel.ViewPrice = 0;
                    orderingMenuProductDetailsModel.ViewPoint = 0;

                }






                orderingMenuProductDetailsModel.Name = Translate(orderingMenuProductDetailsModel.NameArabic, orderingMenuProductDetailsModel.NameEnglish);
                orderingMenuProductDetailsModel.ItemDescription = Translate(orderingMenuProductDetailsModel.ItemDescriptionArabic, orderingMenuProductDetailsModel.ItemDescriptionEnglish);
                
                if (isLoyaltyApplay && !orderingMenuProductDetailsModel.IsLoyalClick)
                {
                    orderingMenuProductDetailsModel.LoyaltycreditPoints = CalculatPointcredit(tenantId, priceBeforeQuantitative, loyaltyModelJson);
                }


            }


            return Partial("Pages/Market/ProductDetails1.cshtml", orderingMenuProductDetailsModel);
        }

        public async Task<IActionResult> OnPostCalculatPointcreditAsync(int tenantId, decimal price, string loyaltyModelJson)
        {
            //    LoyaltyModel loyaltyModel = new LoyaltyModel();
            //    loyaltyModel = PrepareLoyaltyModel(loyaltyModelJson);
            decimal Pointcredit = CalculatPointcredit(tenantId, price, loyaltyModelJson);




            var result = await _viewRenderService.RenderToStringAsync("_PointCredit", Pointcredit);
            return new JsonResult(result); ;

        }

        public PartialViewResult OnGetTenantInfo(string lst)

        {
            OrderingMenuTenantModel orderingMenuTenantModel = JsonConvert.DeserializeObject<OrderingMenuTenantModel>(lst);


            return Partial("Pages/Shared/_HeaderMarket2.cshtml", orderingMenuTenantModel);


        }

        public async Task<IActionResult> OnPostCartItemAsync(bool isApplayLoyalty, string ItemsJson, string loyaltyModelJson)
        {
            List<OrderingCartItemModel> lstOrderingCartItemModel = new List<OrderingCartItemModel>();

            
            if (!string.IsNullOrEmpty(ItemsJson))
            {
                //var settings = new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //    MissingMemberHandling = MissingMemberHandling.Ignore
                //};
                //var jsonModel = JsonConvert.DeserializeObject<Customer>(jsonString, settings);


                ItemsJson=ItemsJson.Replace("٫", ".");


                lstOrderingCartItemModel = JsonConvert.DeserializeObject<List<OrderingCartItemModel>>(ItemsJson);

                foreach (var cartItem in lstOrderingCartItemModel)
                {
                    if (isApplayLoyalty)
                    {
                        //cartItem.CridetPoint = CalculatPointcredit(cartItem.TenantId, cartItem.Total, loyaltyModelJson);
                        if (cartItem.IsLoyalClick)
                        {
                            cartItem.TotalLoyaltyPoints = CalculatPointt(cartItem.TenantId, cartItem.Total, cartItem.TotalLoyaltyPoints, loyaltyModelJson);
                        }
                        else
                        {
                            cartItem.CridetPoint = CalculatPointcredit(cartItem.TenantId, cartItem.Total, loyaltyModelJson);
                        }
                    }
                }


            }


            var result = await _viewRenderService.RenderToStringAsync("_CartItem2", lstOrderingCartItemModel);
            return new JsonResult(result); ;


            //   return Content(sss.ToString());
            //  ;
            // return Content(sss.ToString());
        }
        



        public async Task<IActionResult> OnPostSubListAsync(string ItemsJson)
        {




            listOrderingMenuSubCategoryModel = new List<OrderingMenuSubCategoryModel>();
            listOrderingMenuSubCategoryModel = JsonConvert.DeserializeObject<List<OrderingMenuSubCategoryModel>>(ItemsJson);
             
            var result = await _viewRenderService.RenderToStringAsync("_SubCategorys2", listOrderingMenuSubCategoryModel);
            return new JsonResult(result); ;

        }

        public async Task<IActionResult> OnPostAllSubListAsync(string ItemsJson)
        {
            listOrderingMenuSubCategoryModel = new List<OrderingMenuSubCategoryModel>();
            listOrderingMenuSubCategoryModel = JsonConvert.DeserializeObject<List<OrderingMenuSubCategoryModel>>(ItemsJson);

            var result = await _viewRenderService.RenderToStringAsync("_SubAllCategorys2 ", listOrderingMenuSubCategoryModel);
            return new JsonResult(result); ;
        }
        public async Task<IActionResult> OnPostSubmitOrderAsync(string ItemsJson, bool isLoyaltyApplay, string orderNote)
        {
            List<OrderingCartItemModel> lstOrderingCartItemModel = new List<OrderingCartItemModel>();
            if (!string.IsNullOrEmpty(ItemsJson))
            {
                lstOrderingCartItemModel = JsonConvert.DeserializeObject<List<OrderingCartItemModel>>(ItemsJson);
            }
            OrderModel orderModel = new OrderModel();
            orderModel.ContactId = lstOrderingCartItemModel.FirstOrDefault().ContactId;  // 39743;// Hala
            orderModel.TenantId = lstOrderingCartItemModel.FirstOrDefault().TenantId;
            orderModel.OrderNotes = "";
            orderModel.HasNote = false;

            if (orderNote != null)
            {
                orderModel.OrderNotes = orderNote;
                orderModel.HasNote = true;
            }
            foreach (var item in lstOrderingCartItemModel)
            {
                if (item.IsLoyal && item.IsLoyalClick)
                {
                    item.Total = 0;
                }
            }
            orderModel.lstOrderDetailsModel = PrepareOrder(lstOrderingCartItemModel, orderModel.TenantId, isLoyaltyApplay, out decimal total, out decimal totalPoints, out decimal TotalCreditPoints);
            orderModel.Total = total;
            orderModel.TotalPoints = totalPoints;

            orderModel.TotalCreditPoints = TotalCreditPoints;

            //_Ior
            string x = JsonConvert.SerializeObject(orderModel);
            // long orderId = 0;



            long orderId = await _IOrdersAppService.PostNewOrder(JsonConvert.SerializeObject(orderModel));
            if (orderId > 0)
            {
                await SendToBotAsync(orderModel.ContactId, orderModel.TenantId, orderId, orderModel.Total, orderModel.TotalPoints, orderModel.TotalCreditPoints);
            }

            //CreateMenuReminderMessageModel createMenuReminderMessageModel = new CreateMenuReminderMessageModel();
            //if (createMenuReminderMessageModel!=null) 
            //{ }

                OnPostUpdateReminderMessageAsync();

            return new JsonResult("Done");
        }







        #region Private Method
        //  for tenantInfo


        private void FillTenantInfo()
        {


            OrderingMenuTenantModel orderingMenuTenantModel = new OrderingMenuTenantModel();

            try
            {
                if (this.TenantID != 0)
                {


                    TenantInfoForOrdaringSystemDto tenantInfo = _ITenantServicesInfoAppService.GetTenantsById(TenantID, ContactId);
                    if (tenantInfo != null)
                    {
                        orderingMenuTenantModel.UrlKey = this.Key;
                        orderingMenuTenantModel.LogoImag = tenantInfo.LogoImag;
                        orderingMenuTenantModel.PhoneNumber = tenantInfo.PhoneNumber;
                        orderingMenuTenantModel.Name = tenantInfo.Name;
                        orderingMenuTenantModel.BgImag = tenantInfo.BgImag;
                        orderingMenuTenantModel.ContactId = ContactId;
                        orderingMenuTenantModel.TenantID = TenantID;
                        orderingMenuTenantModel.Lang = this.Lang;
                        orderingMenuTenantModel.LanguageBot = this.LanguageBot;
                        orderingMenuTenantModel.AreaId = AreaId;
                        orderingMenuTenantModel.CurrencyCode = tenantInfo.CurrencyCode;
                        orderingMenuTenantModel.IsApplyLoyalty = tenantInfo.IsApplyLoyalty;
                        orderingMenuTenantModel.DisplayName = tenantInfo.DisplayName;
                        this.CurrencyCode = tenantInfo.CurrencyCode;

                        var DateNow = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        var DateStart = Convert.ToDateTime(tenantInfo.StartDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        var DateEnd = Convert.ToDateTime(tenantInfo.EndDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));


                        if (!string.IsNullOrEmpty(tenantInfo.OrderType) && tenantInfo.OrderType.Contains(this.OrderType.ToString()) && tenantInfo.IsApplyLoyalty && DateStart <= DateNow && DateEnd >= DateNow)
                        {
                            orderingMenuTenantModel.IsApplyLoyalty = true;

                        }
                        else
                        {

                            orderingMenuTenantModel.IsApplyLoyalty = false;
                        }


                        this.IsLoyalty = orderingMenuTenantModel.IsApplyLoyalty;
                        if (orderingMenuTenantModel.IsApplyLoyalty)
                        {
                            orderingMenuTenantModel.orderingMenuContactLoyaltyModel = FillContactLoyaltyInfo(tenantInfo.DisplayName, tenantInfo.ContactDisplayName, tenantInfo.OriginalLoyaltyPoints);
                            orderingMenuTenantModel.orderingMenuLoyaltyModel = FillLoyaltyModel(TenantID);


                            //  this.OrderingMenuLoyaltyModel = orderingMenuTenantModel.orderingMenuLoyaltyModel;
                        }


                    }

                    orderingMenuTenantModel.Name = Translate(orderingMenuTenantModel.Name, orderingMenuTenantModel.NameEnglish);


                    this.CurrencyCode = orderingMenuTenantModel.CurrencyCode;
                }
                this.OrderingMenuTenantModel = orderingMenuTenantModel;
            }
            catch
            {


            }
            

        }


        //for the loyalty

        private OrderingMenuLoyaltyModel FillLoyaltyModel(int tenantId)
        {

            LoyaltyModel loyaltymodel = _ILoyaltyAppService.GetLoyaltyForMenu(this.TenantID);
            OrderingMenuLoyaltyModel orderingMenuLoyaltyModel = new OrderingMenuLoyaltyModel();
            orderingMenuLoyaltyModel.Id = loyaltymodel.Id;
            orderingMenuLoyaltyModel.CreatedBy = loyaltymodel.CreatedBy;
            orderingMenuLoyaltyModel.ItemsCurrencyValue = loyaltymodel.ItemsCurrencyValue;
            orderingMenuLoyaltyModel.CustomerCurrencyValue = loyaltymodel.CustomerCurrencyValue;
            orderingMenuLoyaltyModel.IsLatest = loyaltymodel.IsLatest;
            orderingMenuLoyaltyModel.CustomerPoints = loyaltymodel.CustomerPoints;
            orderingMenuLoyaltyModel.CreatedDate = loyaltymodel.CreatedDate;
            orderingMenuLoyaltyModel.IsOverrideUpdatedPrice = loyaltymodel.IsOverrideUpdatedPrice;
            orderingMenuLoyaltyModel.IsLoyalityPoint = loyaltymodel.IsLoyalityPoint;
            orderingMenuLoyaltyModel.TenantId = tenantId;
            orderingMenuLoyaltyModel.ItemsPoints = loyaltymodel.ItemsPoints;



            return orderingMenuLoyaltyModel;

        }
        private OrderingMenuContactLoyaltyModel FillContactLoyaltyInfo(string DisplayName, string ContactDisplayName, decimal OriginalLoyaltyPoints)
        {

            // var ConResult = _ITenantServicesInfoAppService.GetContactbyId(contactId);


            OrderingMenuContactLoyaltyModel orderingMenuContactLoyaltyModel = new OrderingMenuContactLoyaltyModel();
            orderingMenuContactLoyaltyModel.OriginalLoyaltyPoints = OriginalLoyaltyPoints;
            orderingMenuContactLoyaltyModel.DisplayName = DisplayName;

            orderingMenuContactLoyaltyModel.ContactDisplayName = ContactDisplayName;

            return orderingMenuContactLoyaltyModel;
        }


        private List<OrderingMenuSubCategoryModel> SubCategoryLoyalty()
        {
            List<OrderingMenuSubCategoryModel> lstSubCategorysModel = new List<OrderingMenuSubCategoryModel>();


            OrderingMenuSubCategoryModel getSubCategorysModel = new OrderingMenuSubCategoryModel();
            getSubCategorysModel.ItemSubCategoryId = 0;
            getSubCategorysModel.SubCategoryName = "مكافآتي";
            getSubCategorysModel.SubCategoryNameEnglish = "Rewards";
            getSubCategorysModel.MenuPriority = 1;
            getSubCategorysModel.MenuId = 0;
            getSubCategorysModel.ItemCategoryId = 0;
            getSubCategorysModel.SubCategoryName = Translate(getSubCategorysModel.SubCategoryName, getSubCategorysModel.SubCategoryNameEnglish);

            lstSubCategorysModel.Add(getSubCategorysModel);

            return lstSubCategorysModel;

        }
        private List<OrderingMenuCategoryModel> CategoryLoyalty()
        {
            List<OrderingMenuCategoryModel> lstOrderingMenuCategoryModel = new List<OrderingMenuCategoryModel>();

            OrderingMenuCategoryModel objOrderingMenuCategoryModel = new OrderingMenuCategoryModel();
            objOrderingMenuCategoryModel.MenuPriority = 1;
            objOrderingMenuCategoryModel.MenuId = 0;
            objOrderingMenuCategoryModel.CategoryId = 0;
            objOrderingMenuCategoryModel.CategoryName = "مكافآتي";
            objOrderingMenuCategoryModel.CategoryNameEnglish = "Rewards";
            objOrderingMenuCategoryModel.IsSubCategory = true;
            objOrderingMenuCategoryModel.CategoryName = Translate(objOrderingMenuCategoryModel.CategoryName, objOrderingMenuCategoryModel.CategoryNameEnglish);
            //   objOrderingMenuCategoryModel.lstOrderingMenuItemModel = ItemsLoyalty().OrderBy(x => x.ItemSubCategoryId).ToList();
            objOrderingMenuCategoryModel.lstOrderingMenuSubCategoryModel = SubCategoryLoyalty().OrderBy(x => x.MenuPriority).ToList();
            lstOrderingMenuCategoryModel.Add(objOrderingMenuCategoryModel);

            return lstOrderingMenuCategoryModel;
        }


        private OrderingMenuMenuModel MenuLoyalty()
        {

            OrderingMenuMenuModel orderingMenuMenuModel = new OrderingMenuMenuModel();
            orderingMenuMenuModel.Id = 0;
            orderingMenuMenuModel.MenuName = "Loyalty";
            orderingMenuMenuModel.MenuNameEnglish = "Loyalty";
            orderingMenuMenuModel.Priority = 1;
            orderingMenuMenuModel.MenuName = Translate(orderingMenuMenuModel.MenuName, orderingMenuMenuModel.MenuNameEnglish);
            orderingMenuMenuModel.lstOrderingMenuCategoryModel = CategoryLoyalty().OrderBy(x => x.MenuPriority).ToList(); ;


            return orderingMenuMenuModel;
        }





        //for translate Names

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



        //isQuantiitve
        private List<OrderingMenuItemSpecificationsModel> AddExtra(ItemDto itm, bool isLoyaltyApplay, LoyaltyModel loyaltyModel, bool IsLoyalClick = false)
        {
            List<OrderingMenuSpecificationChoiceModel> Listchoices = new List<OrderingMenuSpecificationChoiceModel>();


            var itmPrice = itm.Price;
            var itmPoint = itm.LoyaltyPoints;

            if (itm.OldPrice != null || itm.OldPrice > 0)

            {
                if (itm.OldPrice < itm.Price)
                {
                    if (!IsLoyalClick)
                    {
                        itmPrice = itm.OldPrice;
                    }
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
                LoyaltyPoints = itmPoint * decimal.Parse("0.25")


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
                LoyaltyPoints = itmPoint * decimal.Parse("0.50")


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
                LoyaltyPoints = itmPoint * decimal.Parse("0.75")

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
            if (isLoyaltyApplay && itm.IsLoyal)
            {

                // choices1.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices1.Price, choices1.LoyaltyPoints, choices1.OriginalLoyaltyPoints, 0, loyaltyModel);
                //choices2.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices2.Price, choices2.LoyaltyPoints, choices2.OriginalLoyaltyPoints, 0, loyaltyModel);
                //choices3.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices3.Price, choices3.LoyaltyPoints, choices3.OriginalLoyaltyPoints, 0, loyaltyModel);
                // choices4.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices4.Price, choices4.LoyaltyPoints, choices4.OriginalLoyaltyPoints, 0, loyaltyModel);

                choices1.LoyaltyPoints = PointDigit(choices1.LoyaltyPoints);
                choices2.LoyaltyPoints = PointDigit(choices2.LoyaltyPoints);
                choices3.LoyaltyPoints = PointDigit(choices3.LoyaltyPoints);
                choices4.LoyaltyPoints = PointDigit(choices4.LoyaltyPoints);

            }

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

            itemSpecification.SpecificationDescription = Translate(itemSpecification.SpecificationDescriptionArabic, itemSpecification.SpecificationDescriptionEnglish);
            ItemSpecificationsList.Add(itemSpecification);



            return ItemSpecificationsList;
        }


        private List<OrderingMenuItemModel> PrepareItemList(List<ItemDto> itemDtos)
        {


            List<OrderingMenuItemModel> lstOrderingMenuItemModels = new List<OrderingMenuItemModel>();
            foreach (var item in itemDtos)
            {
                OrderingMenuItemModel objOrderingMenuItemModels = new OrderingMenuItemModel();
                objOrderingMenuItemModels.ItemName = item.ItemName;
                objOrderingMenuItemModels.Id = item.Id;
                objOrderingMenuItemModels.ImageUri = item.ImageUri;
                objOrderingMenuItemModels.ItemDescription = item.ItemDescription;
                objOrderingMenuItemModels.ItemDescriptionEnglish = item.ItemDescriptionEnglish;
                objOrderingMenuItemModels.ItemDescriptionArabic = item.ItemDescription;
                objOrderingMenuItemModels.Price = item.Price;
                objOrderingMenuItemModels.OldPrice = item.OldPrice;
                objOrderingMenuItemModels.ItemNameEnglish = item.ItemNameEnglish;
                objOrderingMenuItemModels.ItemNameArabic = item.ItemName;
                objOrderingMenuItemModels.Discount = item.Discount;
                objOrderingMenuItemModels.IsInService = item.IsInService;
                objOrderingMenuItemModels.CurrencyCode = this.CurrencyCode;
                objOrderingMenuItemModels.IsQuantitative = item.IsQuantitative;
                if (item.HasAdditions || item.HasSpecifications || item.IsQuantitative)
                {
                    objOrderingMenuItemModels.HasOption = true;
                }

                if (item.OldPrice != null)
                {
                    var o = item.OldPrice;
                    var p = item.Price;
                    objOrderingMenuItemModels.OldPrice = p;
                    objOrderingMenuItemModels.Price = o;
                }

                objOrderingMenuItemModels.OldPrice = PriceDigit(objOrderingMenuItemModels.OldPrice);
                objOrderingMenuItemModels.Price = PriceDigit(objOrderingMenuItemModels.Price);

                objOrderingMenuItemModels.ItemName = Translate(objOrderingMenuItemModels.ItemNameArabic, objOrderingMenuItemModels.ItemNameEnglish);
                objOrderingMenuItemModels.ItemDescription = Translate(objOrderingMenuItemModels.ItemDescriptionArabic, objOrderingMenuItemModels.ItemDescriptionEnglish);


                lstOrderingMenuItemModels.Add(objOrderingMenuItemModels);
            }
            return lstOrderingMenuItemModels;
        }

        private decimal PointDigit(decimal Point)
        {

            Point = Math.Round(Point, 0);
            return Point;
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

        private List<OrderingMenuItemSpecificationsModel> PrepareSpesifcationList(bool isLoyaltyApplay, List<ItemSpecification> _OrderingMenuItemSpecificationsModel, LoyaltyModel loyaltyModel, long subcatId =0)
        {
            List<OrderingMenuItemSpecificationsModel> lstOrderingMenuItemSpecificationsModel = new List<OrderingMenuItemSpecificationsModel>();
            int UniqueId = 1;

            foreach (var item in _OrderingMenuItemSpecificationsModel)
            {
                OrderingMenuItemSpecificationsModel objOrderingMenuItemSpecificationsModel = new OrderingMenuItemSpecificationsModel();
                objOrderingMenuItemSpecificationsModel.SpecificationDescription = item.SpecificationDescription;
                objOrderingMenuItemSpecificationsModel.SpecificationDescriptionEnglish = item.SpecificationDescriptionEnglish;
                objOrderingMenuItemSpecificationsModel.SpecificationDescriptionArabic = item.SpecificationDescription;

                objOrderingMenuItemSpecificationsModel.Id = item.Id;
                objOrderingMenuItemSpecificationsModel.IsMultipleSelection = item.IsMultipleSelection;
                objOrderingMenuItemSpecificationsModel.UniqueId = UniqueId;
                objOrderingMenuItemSpecificationsModel.IsRequired = item.IsRequired;
                objOrderingMenuItemSpecificationsModel.MaxSelectNumber = item.MaxSelectNumber;
                if (item.SpecificationChoices != null && item.SpecificationChoices.Count > 0)
                {
                    objOrderingMenuItemSpecificationsModel.lstOrderingMenuSpecificationChoiceModel = PrepareSpesifcationChoiceList(isLoyaltyApplay, item.SpecificationChoices, loyaltyModel, subcatId);

                    lstOrderingMenuItemSpecificationsModel.Add(objOrderingMenuItemSpecificationsModel);
                }

                UniqueId++;
                objOrderingMenuItemSpecificationsModel.SpecificationDescription = Translate(objOrderingMenuItemSpecificationsModel.SpecificationDescription, objOrderingMenuItemSpecificationsModel.SpecificationDescriptionEnglish);

            }
            return lstOrderingMenuItemSpecificationsModel;
        }
        private List<OrderingMenuItemAdditionsCategorysModel> PrepareAdditionsList(bool isLoyaltyApplay, List<AdditionsCategorysListModel> _OrderingMenuItemAdditionsCategorysModel, LoyaltyModel loyaltyModel , long subcatId =0)
        {
            List<OrderingMenuItemAdditionsCategorysModel> lstOrderingMenuItemAdditionsCategorysModel = new List<OrderingMenuItemAdditionsCategorysModel>();
            if (_OrderingMenuItemAdditionsCategorysModel != null)
            {
                foreach (var item in _OrderingMenuItemAdditionsCategorysModel)
                {
                    OrderingMenuItemAdditionsCategorysModel objOrderingMenuItemAdditionsCategorysModel = new OrderingMenuItemAdditionsCategorysModel();
                    objOrderingMenuItemAdditionsCategorysModel.Name = item.Name;
                    objOrderingMenuItemAdditionsCategorysModel.NameEnglish = item.NameEnglish;
                    objOrderingMenuItemAdditionsCategorysModel.NameArabic = item.Name;

                    objOrderingMenuItemAdditionsCategorysModel.AdditionsAndItemId = item.AdditionsAndItemId;
                    if (item.ItemAdditionDto != null && item.ItemAdditionDto.Count > 0)
                    {
                        objOrderingMenuItemAdditionsCategorysModel.OrderingMenuItemAdditionModel = PrepareAdditionList(isLoyaltyApplay, item.ItemAdditionDto, loyaltyModel , subcatId);

                        lstOrderingMenuItemAdditionsCategorysModel.Add(objOrderingMenuItemAdditionsCategorysModel);
                    }
                    objOrderingMenuItemAdditionsCategorysModel.Name = Translate(objOrderingMenuItemAdditionsCategorysModel.Name, objOrderingMenuItemAdditionsCategorysModel.NameEnglish);

                }
            }
            return lstOrderingMenuItemAdditionsCategorysModel;

        }

        private List<OrderingMenuItemAdditionModel> PrepareAdditionList(bool isLoyaltyApplay, List<ItemAdditionDto> _OrderingMenuItemAdditionModel, LoyaltyModel loyaltyModel , long subcatId = 0)
        {

            List<OrderingMenuItemAdditionModel> lstOrderingMenuItemAdditionModel = new List<OrderingMenuItemAdditionModel>();

            if (_OrderingMenuItemAdditionModel != null)
            {
                foreach (var item in _OrderingMenuItemAdditionModel)
                {
                    OrderingMenuItemAdditionModel objOrderingMenuItemAdditionModel = new OrderingMenuItemAdditionModel();

                    objOrderingMenuItemAdditionModel.ItemAdditionsId = item.ItemAdditionsId;
                    objOrderingMenuItemAdditionModel.Name = item.Name;
                    objOrderingMenuItemAdditionModel.NameEnglish = item.NameEnglish;
                    objOrderingMenuItemAdditionModel.NameArabic = item.Name;
                    objOrderingMenuItemAdditionModel.price = PriceDigit(item.price);
                    objOrderingMenuItemAdditionModel.ItemAdditionsCategoryId = item.ItemAdditionsCategoryId;

                    if (isLoyaltyApplay)
                    {
                        objOrderingMenuItemAdditionModel.LoyaltyPoints = item.LoyaltyPoints;
                        objOrderingMenuItemAdditionModel.OriginalLoyaltyPoints = item.OriginalLoyaltyPoints;
                        objOrderingMenuItemAdditionModel.LoyaltyDefinitionId = item.LoyaltyDefinitionId;
                        if (subcatId == 0)
                        {
                            objOrderingMenuItemAdditionModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(objOrderingMenuItemAdditionModel.price, objOrderingMenuItemAdditionModel.LoyaltyPoints, objOrderingMenuItemAdditionModel.OriginalLoyaltyPoints, objOrderingMenuItemAdditionModel.LoyaltyDefinitionId, loyaltyModel);

                        }
                        else
                        {
                            objOrderingMenuItemAdditionModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToCustomerPoints(objOrderingMenuItemAdditionModel.price, objOrderingMenuItemAdditionModel.LoyaltyPoints, objOrderingMenuItemAdditionModel.OriginalLoyaltyPoints, objOrderingMenuItemAdditionModel.LoyaltyDefinitionId, loyaltyModel);

                        }

                        // objOrderingMenuItemAdditionModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(objOrderingMenuItemAdditionModel.price, objOrderingMenuItemAdditionModel.LoyaltyPoints, objOrderingMenuItemAdditionModel.OriginalLoyaltyPoints, 0, loyaltyModel);
                        //  objOrderingMenuItemAdditionModel.LoyaltyPoints = PointDigit(item.LoyaltyPoints);


                    }
                    objOrderingMenuItemAdditionModel.Name = Translate(objOrderingMenuItemAdditionModel.NameArabic, objOrderingMenuItemAdditionModel.NameEnglish);

                    lstOrderingMenuItemAdditionModel.Add(objOrderingMenuItemAdditionModel);
                }
            }
            return lstOrderingMenuItemAdditionModel;

        }




        private List<OrderingMenuSpecificationChoiceModel> PrepareSpesifcationChoiceList(bool isLoyaltyApplay, List<SpecificationChoice> _OrderingMenuSpecificationChoiceModel, LoyaltyModel loyaltyModel , long subcatId =0)
        {
            List<OrderingMenuSpecificationChoiceModel> lstOrderingMenuSpecificationChoiceModel = new List<OrderingMenuSpecificationChoiceModel>();
            int UniqueId = 1;
            foreach (var item in _OrderingMenuSpecificationChoiceModel)
            {
                OrderingMenuSpecificationChoiceModel objOrderingMenuSpecificationChoiceModel = new OrderingMenuSpecificationChoiceModel();
                objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescription = item.SpecificationChoiceDescription;
                objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescriptionEnglish = item.SpecificationChoiceDescriptionEnglish;
                objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescriptionArabic = item.SpecificationChoiceDescription;

                objOrderingMenuSpecificationChoiceModel.Id = item.Id;
                objOrderingMenuSpecificationChoiceModel.UniqueId = item.UniqueId;
                objOrderingMenuSpecificationChoiceModel.Price = PriceDigit(item.Price);
                objOrderingMenuSpecificationChoiceModel.IsInService = item.IsInService;
                if (isLoyaltyApplay)
                {
                    objOrderingMenuSpecificationChoiceModel.LoyaltyPoints = item.LoyaltyPoints;
                    objOrderingMenuSpecificationChoiceModel.LoyaltyDefinitionId = item.LoyaltyDefinitionId;
                    objOrderingMenuSpecificationChoiceModel.OriginalLoyaltyPoints = item.OriginalLoyaltyPoints;
                    if (subcatId == 0)
                    {
                        objOrderingMenuSpecificationChoiceModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(objOrderingMenuSpecificationChoiceModel.Price, objOrderingMenuSpecificationChoiceModel.LoyaltyPoints, objOrderingMenuSpecificationChoiceModel.OriginalLoyaltyPoints, objOrderingMenuSpecificationChoiceModel.LoyaltyDefinitionId, loyaltyModel);

                    }
                    else
                    {
                        objOrderingMenuSpecificationChoiceModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToCustomerPoints(objOrderingMenuSpecificationChoiceModel.Price, objOrderingMenuSpecificationChoiceModel.LoyaltyPoints, objOrderingMenuSpecificationChoiceModel.OriginalLoyaltyPoints, objOrderingMenuSpecificationChoiceModel.LoyaltyDefinitionId, loyaltyModel);

                    }
                    

                    // objOrderingMenuSpecificationChoiceModel.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(objOrderingMenuSpecificationChoiceModel.Price, objOrderingMenuSpecificationChoiceModel.LoyaltyPoints, objOrderingMenuSpecificationChoiceModel.OriginalLoyaltyPoints, 0, loyaltyModel);
                    // objOrderingMenuSpecificationChoiceModel.LoyaltyPoints = PointDigit(item.LoyaltyPoints);

                }
                objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescription = Translate(objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescriptionArabic, objOrderingMenuSpecificationChoiceModel.SpecificationChoiceDescriptionEnglish);


                lstOrderingMenuSpecificationChoiceModel.Add(objOrderingMenuSpecificationChoiceModel);
                UniqueId++;



            }
            return lstOrderingMenuSpecificationChoiceModel;
        }


        private List<OrderDetailsModel> PrepareOrder(List<OrderingCartItemModel> orderingCartItemModels, int TenantID, bool isLoyaltyApplay, out decimal totalPrice, out decimal totalPoints, out decimal totalCreditPoints)
        {
            List<OrderDetailsModel> lstOrderDetailsModel = new List<OrderDetailsModel>();
            LoyaltyModel loyaltyModel = new LoyaltyModel();

            totalPrice = 0;
            totalPoints = 0;
            totalCreditPoints = 0;
            if (isLoyaltyApplay)
            {
                loyaltyModel = _ILoyaltyAppService.GetLoyaltyForMenu(TenantID);


            }



            foreach (var item in orderingCartItemModels)
            {



                if (item.IsLoyalClick && isLoyaltyApplay)
                {

                    Calculatepoints(item, loyaltyModel);

                }
                else
                {

                    CalculatePrices(item, isLoyaltyApplay, loyaltyModel);
                }

                OrderDetailsModel orderDetailsModel = new()
                {
                    CartItemId = item.CartItemId,
                    Id = item.Id,
                    TenantId = item.TenantId,
                    ContactId = item.ContactId,
                    Quantity = item.Qty,
                    Name = item.Name,
                    NameEnglish = item.NameEnglish,
                    NameArabic = item.Name
                   ,
                    ItemNote = "",
                    HasItemNote = false,
                };
                if (!string.IsNullOrEmpty(item.ItemNote))
                {
                    orderDetailsModel.ItemNote = item.ItemNote;
                    orderDetailsModel.HasItemNote = true;

                }

                if (item.IsLoyalClick)
                {
                    orderDetailsModel.TotalLoyaltyPoints = item.TotalLoyaltyPoints;
                    orderDetailsModel.UnitPoints = item.LoyaltyPoints;

                }
                else
                {
                    orderDetailsModel.UnitPrice = item.Price;
                    orderDetailsModel.Total = item.Total;

                }
                totalPrice += item.Total;
                if (isLoyaltyApplay)
                {
                    totalPoints += item.TotalLoyaltyPoints;
                    totalCreditPoints += item.TotalCreditPoints;
                }
                if (item.lstOrderingCartItemSpecificationModel != null)
                {
                    orderDetailsModel.lstOrderDetailsExtraModel = PrepareOrderDetialsExtra(item);

                }
                else
                {
                    orderDetailsModel.lstOrderDetailsExtraModel = new();
                }
                if (item.lstOrderingCartItemAdditionalModel != null)
                {
                    PrepareItemAdditional(item.lstOrderingCartItemAdditionalModel,
                        orderDetailsModel.lstOrderDetailsExtraModel, orderDetailsModel.TenantId, item.IsLoyalClick);

                }



                lstOrderDetailsModel.Add(orderDetailsModel);
            }

            return lstOrderDetailsModel;
        }

        private List<OrderDetailsExtraModel> PrepareOrderDetialsExtra(OrderingCartItemModel itemModel)
        {

            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new();
            lstOrderingCartItemSpecificationModel = itemModel.lstOrderingCartItemSpecificationModel;
            List<OrderDetailsExtraModel> lstOrderDetailsExtraModel = new();
            foreach (var item in lstOrderingCartItemSpecificationModel)
            {
                foreach (var obj in item.lstOrderingCartItemSpecificationChoicesModel)
                {
                    OrderDetailsExtraModel orderDetailsExtraModel = new()
                    {
                        Name = obj.Name,
                        NameEnglish = obj.NameEnglish,
                        NameArabic = obj.NameArabic,
                        SpecificationName = item.SpecificationName,
                        SpecificationNameArabic = item.SpecificationNameArabic,
                        SpecificationNameEnglish = item.SpecificationNameEnglish,
                        SpecificationUniqueId = 1,
                        Quantity = 1,
                        TenantId = item.TenantId,

                        UnitPrice = obj.UnitPrice,
                        Total = obj.Total,

                        TotalCreditPoints = obj.TotalCreditPoints,
                        UnitPoints = 0,
                        TotalLoyaltyPoints = 0,
                        SpecificationChoiceId = (int)obj.Id,
                        SpecificationId = (int)item.SpecificationId,
                        TypeExtraDetails = 1,
                         IsMultipleSelection=item.IsMultipleSelection,
                        MaxSelectNumber=item.MaxSelectNumber,

                    };

                    if (itemModel.IsLoyalClick)
                    {
                        orderDetailsExtraModel.UnitPoints = obj.LoyaltyPoints;
                        orderDetailsExtraModel.TotalLoyaltyPoints = obj.TotalLoyaltyPoints * itemModel.Qty;
                    }



                    lstOrderDetailsExtraModel.Add(orderDetailsExtraModel);
                }
            }

            return lstOrderDetailsExtraModel;
        }
        private void PrepareItemAdditional(List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModels, List<OrderDetailsExtraModel> lstOrderDetailsExtraModel, int tenantId, bool IsLoyalClick)
        {
            foreach (var item in lstOrderingCartItemAdditionalModels)
            {

                OrderDetailsExtraModel orderDetailsExtraModel = new()
                {
                    Name = item.Name,
                    NameEnglish = item.NameEnglish,
                    NameArabic = item.Name,
                    SpecificationUniqueId = 1,
                    Quantity = item.Qty,
                    TenantId = tenantId,
                    UnitPrice = item.UnitPrice,
                    Total = item.Total,
                    UnitPoints = 0,
                    TotalLoyaltyPoints = 0,
                    TotalCreditPoints = item.TotalCreditPoints,
                    SpecificationChoiceId = (int)item.Id,
                    SpecificationId = (int)item.ItemAdditionsCategoryId,
                    TypeExtraDetails = 2

                };

                if (IsLoyalClick)
                {
                    orderDetailsExtraModel.UnitPoints = PointDigit(item.LoyaltyPoints);
                    orderDetailsExtraModel.TotalLoyaltyPoints = item.TotalLoyaltyPoints;
                }



                lstOrderDetailsExtraModel.Add(orderDetailsExtraModel);

            }

        }

        private async Task SendToBotAsync(int CustomerId, int tenantId, long orderId, decimal orderTotal,decimal totalPoints, decimal totalCreditPoints)
        {


            try
            {
                //var contact = GetCustomer(CustomerId);
                // var user = GetCustomerAzuer(contact.UserId);
                //  var tenant = GetTenantByAppIdD360(user.D360Key).Result;
                var user = await _dbService.GetCustomerByContactId(CustomerId.ToString());
                var tenant = await _dbService.GetTenantInfoById(tenantId);

                //var Tenant = await _dbService.GetTenantInfoById(createOrderModel.TenantId.Value);
                if (tenant != null && !string.IsNullOrEmpty(tenant.AccessToken))
                {

                    var result = await sendToWhatsApp(tenant, user, orderId, orderTotal,  totalPoints, totalCreditPoints);
                }




            }
            catch (Exception ex)
            {
                //this._telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
            }

        }

        private async Task SendToRestaurantsBot(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi+ "api/RestaurantsChatBot/RestaurantsMessageHandler");
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


        private async Task SendToFlowsBot(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi+ "api/FlowsChatBot/FlowsBotMessageHandler");
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
        private async Task<bool> sendToWhatsApp(TenantModel Tenant, CustomerModel user, long orderId, decimal orderTotal, decimal totalPoints, decimal totalCreditPoints)
        {

            //  DirectLineConnector directLineConnector = new DirectLineConnector();
            //  var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
            // var Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments).Result;

            //var contact = GetCustomer(CustomerId);

            //var user = GetCustomerAzuerById(contactId.ToString());
            string from = user.phoneNumber;
            List<Microsoft.Bot.Connector.DirectLine.Activity> Bot = new List<Microsoft.Bot.Connector.DirectLine.Activity>();



            if (Tenant.botId=="FlowsBot")
            {
                decimal total = 0;
                decimal discount = 0;
                decimal deliveryCostAfter = 0;

                user.customerChat.text="اختبار";
                //.CustomerStepModel.ChatStepId=4;
                //user.CustomerStepModel.IsLinkMneuStep=true;
                user.CustomerStepModel.OrderTotal = total;
                user.CustomerStepModel.Discount = discount;
                user.CustomerStepModel.OrderId = orderId;
                user.CustomerStepModel.TotalPoints = totalPoints;
                user.CustomerStepModel.TotalCreditPoints = totalCreditPoints;
                //user.CustomerStepModel.DeliveryCostAfter = deliveryCostAfter;


                SendToFlowsBot(user);
                return true;


            }
            else if (Tenant.botId=="RestaurantBot")
            {


               // BotServiceController botService = new BotServiceController(_botApis);
                decimal total = 0; 
                decimal discount = 0; 
                decimal deliveryCostAfter=0;

                user.customerChat.text="اختبار";
                user.CustomerStepModel.ChatStepId=4;
                //user.CustomerStepModel.IsLinkMneuStep=true;
                user.CustomerStepModel.OrderTotal = total;
                user.CustomerStepModel.Discount = discount;
                user.CustomerStepModel.OrderId = orderId;
                user.CustomerStepModel.TotalPoints = totalPoints;
                user.CustomerStepModel.TotalCreditPoints = totalCreditPoints;
                //user.CustomerStepModel.DeliveryCostAfter = deliveryCostAfter;


                SendToRestaurantsBot(user);
                return true;
                //var aaaa = JsonConvert.SerializeObject(user);
                //Bot = botService.BotMessageHandler(aaaa);

                //string ids = string.Empty;
                //var objSteps = new BotStepsService().GetTextResourceIdsForOrder(Tenant.BotTemplateId.Value, out ids);

                //long areaId = user.CustomerStepModel.LocationId;
                //if (user.CustomerStepModel.OrderTypeId == BotOrderTypeEnum.Pickup___0b_111_111_000_111.ToString())
                //    areaId = user.CustomerStepModel.SelectedAreaId;

                //var botOrderModel = _IOrdersAppService.GetOrderDetailsForBot(orderId, user.CustomerStepModel.LangId, ids, Tenant.isOrderOffer, areaId);
                //decimal deliveryCost = user.CustomerStepModel.DeliveryCostBefor.HasValue ? user.CustomerStepModel.DeliveryCostBefor.Value : 0;
                //decimal total = 0; decimal discount = 0; decimal deliveryCostAfter;
                //Bot = new BotSqlService().GetMessageFromBotConversation(botOrderModel, user.CustomerStepModel.LangId, objSteps, deliveryCost, user.CustomerStepModel.OrderTypeId, out total, out discount, out deliveryCostAfter);
                //user.CustomerStepModel.OrderTotal = total;
                //user.CustomerStepModel.Discount = discount;
                //user.CustomerStepModel.OrderId = orderId;
                //user.CustomerStepModel.TotalPoints = totalPoints;
                //user.CustomerStepModel.TotalCreditPoints = totalCreditPoints;
                //user.CustomerStepModel.DeliveryCostAfter = deliveryCostAfter;

            }
            else
            {
                DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
                var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, user.userId, Tenant.botId).Result;
                Bot = directLineConnector.StartBotConversationD360(user.userId, user.ContactID.ToString(), micosoftConversationID.MicrosoftBotId, "testinfoseed", Tenant.DirectLineSecret, Tenant.botId, user.phoneNumber, user.TenantId.ToString(), user.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null, Tenant.BotTemplateId).Result;
            }



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


            //   bool isBotSendAttachments = false;
            //  bool Islist = true;

            foreach (var msgBot in botListMessages)
            {
                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = new List<PostWhatsAppMessageModel>();
                if (Tenant.TenantId == 75 || string.IsNullOrEmpty(Tenant.botId))
                {
                    lstpostWhatsAppMessageModel = await new WhatsAppAppService().BotInfoChatWithCustomer(msgBot, from, Tenant.botId);


                }
                else
                {
                    lstpostWhatsAppMessageModel = await new WhatsAppAppService().BotChatWithCustomer(msgBot, from, Tenant.botId);

                }
                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                {


      
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {
                            WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, Tenant.botId, user.userId, Tenant.TenantId.Value, user.ConversationId);
                            var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);
                            user.customerChat = CustomerChat;

                            if (Tenant.TenantId == 75 || string.IsNullOrEmpty(Tenant.botId))
                            {
                                _dbService.UpdateCustomerStatus(user);

                            }
                            //await _hub.Clients.All.SendAsync("brodCastAgentMessage", user);
                            SocketIOManager.SendContact(user, user.TenantId.Value);
                        }
                    
                   
                }
            }

            return true;

        }
        #region Loyalty 

        private LoyaltyModel PrepareLoyaltyModel(string model)
        {
            LoyaltyModel loyaltyModel = new LoyaltyModel();

            if (!string.IsNullOrEmpty(model))
            {
                OrderingMenuTenantModel orderingMenuTenantModel = JsonConvert.DeserializeObject<OrderingMenuTenantModel>(model);
                if (orderingMenuTenantModel.orderingMenuLoyaltyModel != null)
                {
                    loyaltyModel.ItemsCurrencyValue = orderingMenuTenantModel.orderingMenuLoyaltyModel.ItemsCurrencyValue;
                    loyaltyModel.ItemsPoints = orderingMenuTenantModel.orderingMenuLoyaltyModel.ItemsPoints;
                    loyaltyModel.Id = orderingMenuTenantModel.orderingMenuLoyaltyModel.Id;
                    loyaltyModel.CreatedBy = orderingMenuTenantModel.orderingMenuLoyaltyModel.CreatedBy;
                    loyaltyModel.ItemsCurrencyValue = orderingMenuTenantModel.orderingMenuLoyaltyModel.ItemsCurrencyValue;
                    loyaltyModel.CustomerCurrencyValue = orderingMenuTenantModel.orderingMenuLoyaltyModel.CustomerCurrencyValue;
                    loyaltyModel.IsLatest = orderingMenuTenantModel.orderingMenuLoyaltyModel.IsLatest;
                    loyaltyModel.CustomerPoints = orderingMenuTenantModel.orderingMenuLoyaltyModel.CustomerPoints;
                    loyaltyModel.CreatedDate = orderingMenuTenantModel.orderingMenuLoyaltyModel.CreatedDate;
                    loyaltyModel.IsOverrideUpdatedPrice = orderingMenuTenantModel.orderingMenuLoyaltyModel.IsOverrideUpdatedPrice;
                    loyaltyModel.IsLoyalityPoint = orderingMenuTenantModel.orderingMenuLoyaltyModel.IsLoyalityPoint;
                    loyaltyModel.TenantId = orderingMenuTenantModel.orderingMenuLoyaltyModel.TenantId;
                }

            }
            return loyaltyModel;

        }


        private void Calculatepoints(OrderingCartItemModel item, LoyaltyModel loyaltyModel)
        {
            ItemDto itemDtos = _IItemsAppService.GetItemById(item.Id, item.TenantId, true);




            if (!itemDtos.SKU.ToUpper().StartsWith("T") && itemDtos.Size.Contains("1 KG"))
            {
                itemDtos.IsQuantitative = true;

            }


            item.LoyaltyPoints = PointDigit(itemDtos.LoyaltyPoints);



            if (!itemDtos.IsLoyal)
            {

                item.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(itemDtos.Price, itemDtos.LoyaltyPoints, itemDtos.OriginalLoyaltyPoints, 0, loyaltyModel);

            }
            else
            {


                item.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(itemDtos.Price, itemDtos.LoyaltyPoints, itemDtos.OriginalLoyaltyPoints, itemDtos.LoyaltyDefinitionId, loyaltyModel);
            }
            itemDtos.LoyaltyPoints = item.LoyaltyPoints;

            decimal totalPoints = 0;
            if (itemDtos.IsQuantitative)
            {

                totalPoints += CalculatePointsQuantitative(itemDtos, item, loyaltyModel);

            }
            else
            {

                totalPoints = item.LoyaltyPoints;
            }



            totalPoints += CalculatePointsSpecifcation(itemDtos.ItemSpecifications, item, loyaltyModel);
            totalPoints += CalculatePointsAdditional(itemDtos.additionsCategorysListModels, item, loyaltyModel);


            totalPoints = totalPoints * item.Qty;
            item.TotalLoyaltyPoints = totalPoints;

        }
        #endregion

        #region pricec 
        private void CalculatePrices(OrderingCartItemModel item, bool isLoyaltyApplay, LoyaltyModel loyaltyModel)
        {


            decimal crditpoint = loyaltyModel.CustomerPoints;
            ItemDto itemDtos = _IItemsAppService.GetItemById(item.Id, item.TenantId, true);



            if (!itemDtos.SKU.ToUpper().StartsWith("T") && itemDtos.Size.Contains("1 KG"))
            {
                itemDtos.IsQuantitative = true;

            }

            if (itemDtos.OldPrice != null)
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

            decimal total = 0;
            decimal TotalCreditPoints = 0;


            if (itemDtos.IsQuantitative)
            {
                total += CalculatePricesQuantitative(itemDtos, isLoyaltyApplay, item, loyaltyModel);

            }
            else
            {
                total = item.Price;

            }

            total += CalculatePricesSpecifcation(itemDtos.ItemSpecifications, item);


            total += CalculatePricesAdditional(itemDtos.additionsCategorysListModels, item);


            total = total * item.Qty;

            if (itemDtos.IsLoyal)
            {

                if (loyaltyModel.IsLoyalityPoint)
                    TotalCreditPoints += (loyaltyModel.CustomerPoints * total) / (loyaltyModel.CustomerCurrencyValue);
            }
            item.Total = total;
            if (isLoyaltyApplay)
            {
                item.TotalCreditPoints = TotalCreditPoints;
            }
        }
        private decimal CalculatePricesQuantitative(ItemDto ItemDtos, bool isLoyaltyApplay, OrderingCartItemModel item, LoyaltyModel loyaltyModel)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new List<OrderingCartItemSpecificationModel>();

            List<OrderingMenuItemSpecificationsModel> itms = AddExtra(ItemDtos, isLoyaltyApplay, loyaltyModel);


            foreach (var objSpecification in item.lstOrderingCartItemSpecificationModel)
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
                            objChoices.NameEnglish = choices.SpecificationChoiceDescriptionEnglish;
                            objChoices.NameArabic = choices.SpecificationChoiceDescriptionArabic;
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

        private decimal CalculatePricesSpecifcation(List<ItemSpecification> itemSpecifications, OrderingCartItemModel item)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new();

            if (itemSpecifications != null && itemSpecifications.Count > 0)
            {
                if (item.lstOrderingCartItemSpecificationModel != null && item.lstOrderingCartItemSpecificationModel.Count > 0)
                {
                    foreach (var objSpecification in item.lstOrderingCartItemSpecificationModel)
                    {
                        var specfication = itemSpecifications.Where(x => x.Id == objSpecification.SpecificationId).FirstOrDefault();
                        if (specfication != null)
                        {
                            List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel = new();
                            foreach (var objChoices in objSpecification.lstOrderingCartItemSpecificationChoicesModel)
                            {
                                var choices = specfication.SpecificationChoices.Where(x => x.Id == objChoices.Id).FirstOrDefault();
                                if (choices != null)
                                {
                                    objChoices.UnitPrice = choices.Price.HasValue ? choices.Price.Value : 0;
                                    objChoices.Total = objChoices.UnitPrice;

                                    objChoices.Price = objChoices.UnitPrice.ToString();
                                    objChoices.NameArabic = choices.SpecificationChoiceDescription;
                                    objChoices.NameEnglish = choices.SpecificationChoiceDescriptionEnglish;
                                    result = result + objChoices.UnitPrice;
                                    lstOrderingCartItemSpecificationChoicesModel.Add(objChoices);
                                }


                            }
                            objSpecification.lstOrderingCartItemSpecificationChoicesModel = lstOrderingCartItemSpecificationChoicesModel;
                            lstOrderingCartItemSpecificationModel.Add(objSpecification);
                        }
                    }

                }
            }

            //tem.lstOrderingCartItemSpecificationModel = lstOrderingCartItemSpecificationModel;
            return result;
        }

        private decimal CalculatePricesAdditional(List<AdditionsCategorysListModel> additionsCategorysListModels, OrderingCartItemModel item)
        {
            decimal result = 0;
            List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModel = new();

            if (additionsCategorysListModels != null && additionsCategorysListModels.Count > 0)
            {
                if (item.lstOrderingCartItemAdditionalModel != null && item.lstOrderingCartItemAdditionalModel.Count > 0)
                {
                    foreach (var additionsCategorys in item.lstOrderingCartItemAdditionalModel)
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
                                additionsCategorys.NameArabic = objadditions.Name;
                                additionsCategorys.NameEnglish = objadditions.NameEnglish;


                                result += additionsCategorys.Total;
                                lstOrderingCartItemAdditionalModel.Add(additionsCategorys);
                            }




                        }
                    }

                }
            }

            item.lstOrderingCartItemAdditionalModel = lstOrderingCartItemAdditionalModel;
            return result;
        }

        private decimal CalculatePointsQuantitative(ItemDto ItemDtos, OrderingCartItemModel item, LoyaltyModel loyaltyModel)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new();

            List<OrderingMenuItemSpecificationsModel> itms = AddExtra(ItemDtos, true, loyaltyModel, true);


            foreach (var objSpecification in item.lstOrderingCartItemSpecificationModel)
            {
                var specfication = itms.Where(x => x.Id == objSpecification.SpecificationId).FirstOrDefault();
                if (specfication != null)
                {
                    List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel = new();
                    foreach (var objChoices in objSpecification.lstOrderingCartItemSpecificationChoicesModel)
                    {
                        var choices = specfication.lstOrderingMenuSpecificationChoiceModel.Where(x => x.Id == objChoices.Id).FirstOrDefault();

                        if (choices != null)
                        {
                            //choices.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices.Price, choices.LoyaltyPoints, choices.OriginalLoyaltyPoints, choices.LoyaltyDefinitionId, loyaltyModel);

                            //  objChoices.LoyaltyPoints = 0;

                            var UnitPoint = choices.LoyaltyPoints;
                            objChoices.TotalLoyaltyPoints = UnitPoint;
                            objChoices.LoyaltyPoints = UnitPoint;

                            result = result + UnitPoint;
                            item.LoyaltyPoints = UnitPoint;

                            lstOrderingCartItemSpecificationChoicesModel.Add(objChoices);
                        }


                    }
                    objSpecification.lstOrderingCartItemSpecificationChoicesModel = lstOrderingCartItemSpecificationChoicesModel;
                    lstOrderingCartItemSpecificationModel.Add(objSpecification);
                }
            }







            return result;
        }

        private decimal CalculatePointsSpecifcation(List<ItemSpecification> itemSpecifications, OrderingCartItemModel item, LoyaltyModel loyaltyModel)
        {
            decimal result = 0;
            List<OrderingCartItemSpecificationModel> lstOrderingCartItemSpecificationModel = new List<OrderingCartItemSpecificationModel>();

            if (itemSpecifications != null && itemSpecifications.Count > 0)
            {
                if (item.lstOrderingCartItemSpecificationModel != null && item.lstOrderingCartItemSpecificationModel.Count > 0)
                {
                    foreach (var objSpecification in item.lstOrderingCartItemSpecificationModel)
                    {

                        var specfication = itemSpecifications.Where(x => x.Id == objSpecification.SpecificationId).FirstOrDefault();
                        if (specfication != null)
                        {
                            List<OrderingCartItemSpecificationChoicesModel> lstOrderingCartItemSpecificationChoicesModel = new List<OrderingCartItemSpecificationChoicesModel>();
                            foreach (var objChoices in objSpecification.lstOrderingCartItemSpecificationChoicesModel)
                            {
                                var choices = specfication.SpecificationChoices.Where(x => x.Id == objChoices.Id).FirstOrDefault();



                                if (choices != null)
                                {
                                    choices.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(choices.Price, choices.LoyaltyPoints, choices.OriginalLoyaltyPoints, choices.LoyaltyDefinitionId, loyaltyModel);
                                    objChoices.LoyaltyPoints = choices.LoyaltyPoints;
                                    objChoices.NameEnglish = objChoices.NameEnglish;
                                    objChoices.NameArabic = objChoices.NameArabic;
                                    objChoices.TotalLoyaltyPoints = objChoices.LoyaltyPoints;
                                    result = result + objChoices.LoyaltyPoints;
                                    lstOrderingCartItemSpecificationChoicesModel.Add(objChoices);
                                }


                            }
                            objSpecification.lstOrderingCartItemSpecificationChoicesModel = lstOrderingCartItemSpecificationChoicesModel;
                            lstOrderingCartItemSpecificationModel.Add(objSpecification);
                        }
                    }

                }
            }
            else
            {
                //tem.lstOrderingCartItemSpecificationModel = null;

            }
            //tem.lstOrderingCartItemSpecificationModel = lstOrderingCartItemSpecificationModel;
            return result;
        }

        private decimal CalculatePointsAdditional(List<AdditionsCategorysListModel> additionsCategorysListModels, OrderingCartItemModel item, LoyaltyModel loyaltyModel)
        {
            decimal result = 0;
            List<OrderingCartItemAdditionalModel> lstOrderingCartItemAdditionalModel = new List<OrderingCartItemAdditionalModel>();

            if (additionsCategorysListModels != null && additionsCategorysListModels.Count > 0)
            {
                if (item.lstOrderingCartItemAdditionalModel != null && item.lstOrderingCartItemAdditionalModel.Count > 0)
                {
                    foreach (var additionsCategorys in item.lstOrderingCartItemAdditionalModel)
                    {
                        var additions = additionsCategorysListModels.Where(x => x.Id == additionsCategorys.ItemAdditionsCategoryId).FirstOrDefault();
                        if (additions != null)
                        {

                            var objadditions = additions.ItemAdditionDto.Where(x => x.ItemAdditionsId == additionsCategorys.Id).FirstOrDefault();

                            if (objadditions != null)
                            {
                                objadditions.LoyaltyPoints = _ILoyaltyAppService.ConvertPriceToPoints(objadditions.price, objadditions.LoyaltyPoints, objadditions.OriginalLoyaltyPoints, objadditions.LoyaltyDefinitionId, loyaltyModel);
                                additionsCategorys.NameEnglish = objadditions.NameEnglish;
                                additionsCategorys.NameArabic = objadditions.Name;

                                additionsCategorys.TotalLoyaltyPoints = additionsCategorys.LoyaltyPoints * additionsCategorys.Qty;

                                result += additionsCategorys.TotalLoyaltyPoints;
                                lstOrderingCartItemAdditionalModel.Add(additionsCategorys);
                            }




                        }
                    }

                }
            }

            item.lstOrderingCartItemAdditionalModel = lstOrderingCartItemAdditionalModel;
            return result;
        }
        #endregion
        private List<OrderingMenuCategoryModel> fillGetCategorysModel(List<CategoryEntity> categoryEntity)
        {
            List<OrderingMenuCategoryModel> lstOrderingMenuCategoryModel = new List<OrderingMenuCategoryModel>();
            if (categoryEntity != null)
            {

                foreach (var item in categoryEntity)
                {

                    OrderingMenuCategoryModel objOrderingMenuCategoryModel = new OrderingMenuCategoryModel();
                    objOrderingMenuCategoryModel.MenuPriority = item.Priority;
                    objOrderingMenuCategoryModel.MenuId = item.MenuId;
                    objOrderingMenuCategoryModel.CategoryId = item.Id;
                    objOrderingMenuCategoryModel.CategoryName = item.Name;
                    objOrderingMenuCategoryModel.CategoryNameEnglish = item.NameEnglish;



                    objOrderingMenuCategoryModel.lstOrderingMenuSubCategoryModel = fillSubGetCategorysModel(item.ItemSubCategories).OrderBy(x => x.MenuPriority).ToList();


                    objOrderingMenuCategoryModel.AlllstOrderingMenuSubCategoryModel = fillSubGetCategorysModel(item.ItemSubCategories).OrderBy(x => x.MenuPriority).ToList();

                    objOrderingMenuCategoryModel.CategoryName = Translate(objOrderingMenuCategoryModel.CategoryName, objOrderingMenuCategoryModel.CategoryNameEnglish);
                    if (item.ItemSubCategories != null)
                    {
                        lstOrderingMenuCategoryModel.Add(objOrderingMenuCategoryModel);
                    }
                    // ListOrderingMenuCategoryModel.Add(objOrderingMenuCategoryModel);
                }


            }

            return lstOrderingMenuCategoryModel;



        }
        public void OnPostCreateReminderMessageAsync(int contactId)
        {
            //CreateMenuReminderMessageModel createMenuReminderMessageModel = new CreateMenuReminderMessageModel();
            
            if (HttpContext.Session.GetInt32("RemindarID") == null || HttpContext.Session.GetInt32("RemindarID")==0)
            {
                long RemindarID = _iMenusAppService.CreateMenuReminderMessage(new MenuReminderMessages
                {
                    ContactId = contactId,
                    //CreationDate = DateTime.UtcNow,
                    IsActive = true
                });

                HttpContext.Session.SetInt32("RemindarID", (int)RemindarID);
                HttpContext.Session.SetString("IsRemindar", "True");
            }
        }
        public void OnPostUpdateReminderMessageAsync()
        {
            if (HttpContext.Session.GetInt32("RemindarID") != null || HttpContext.Session.GetInt32("RemindarID") != 0)
            {
                if (HttpContext.Session.GetString("IsRemindar") != null || HttpContext.Session.GetString("IsRemindar") != "False")
                {
                    long RemindarID = (long)HttpContext.Session.GetInt32("RemindarID");
                    if (RemindarID != 0 || RemindarID != null)
                    {
                        _iMenusAppService.UpdateMenuReminderMessage(RemindarID);
                        HttpContext.Session.Remove("RemindarID");
                        RemindarID = 0;
                        HttpContext.Session.SetString("IsRemindar", "False");
                        HttpContext.Session.Remove("IsRemindar");
                    }
                }
            }
        }
        private decimal CalculatPointcredit(int tenantId, decimal? ViewPrice, string loyaltyModelJson)
        {

            LoyaltyModel loyaltyModel = new LoyaltyModel();
            loyaltyModel = PrepareLoyaltyModel(loyaltyModelJson);

            decimal Pointcredit = _ILoyaltyAppService.ConvertCustomerPriceToPoint(ViewPrice, tenantId, loyaltyModel);


            return Pointcredit;


        }
        private decimal CalculatPointt(int tenantId, decimal? ViewPrice, decimal loyaltyPoints, string loyaltyModelJson)
        {


            LoyaltyModel loyaltyModel = new LoyaltyModel();
            loyaltyModel = PrepareLoyaltyModel(loyaltyModelJson);

            decimal Pointcredit = _ILoyaltyAppService.ConvertItemsPriceToPoint(ViewPrice, loyaltyPoints, tenantId, loyaltyModel);
            return Pointcredit;
        }
        private List<OrderingMenuSubCategoryModel> fillSubGetCategorysModel(List<SubCategoryEntity> subCategoryEntity)
        {
            List<OrderingMenuSubCategoryModel> getSubCategorysModels = new List<OrderingMenuSubCategoryModel>();


            if (subCategoryEntity != null)
            {
                foreach (var item in subCategoryEntity)
                {


                    OrderingMenuSubCategoryModel getSubCategorysModel = new OrderingMenuSubCategoryModel();

                    getSubCategorysModel.ItemSubCategoryId = item.Id;
                    getSubCategorysModel.SubCategoryName = item.Name;
                    getSubCategorysModel.SubCategoryNameEnglish = item.NameEnglish;
                    getSubCategorysModel.MenuPriority = item.Priority;
                    getSubCategorysModel.ItemCategoryId = item.ItemCategoryId;




                    getSubCategorysModel.SubCategoryName = Translate(getSubCategorysModel.SubCategoryName, getSubCategorysModel.SubCategoryNameEnglish);
                    getSubCategorysModels.Add(getSubCategorysModel);

                    //   ListOrderingMenuSubCategoryModel.Add(getSubCategorysModel);
                }

            }
            //this.listOrderingMenuSubCategoryModel = ListOrderingMenuSubCategoryModel;

            return getSubCategorysModels;



        }




        #endregion

    }
}



// Full postback
//public IActionResult OnPostSetLanguage(string culture, string returnUrl)
//{
//    var request = _context.HttpContext.Request;

//    Response.Cookies.Append(
//        CookieRequestCultureProvider.DefaultCookieName,
//        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US", culture)),
//        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
//    );
//    string  newurl=string.Empty;
//    try
//    {
//         newurl = returnUrl.Split("&handler")[0];
//    }
//    catch (Exception)
//    {

//         newurl = returnUrl;      
//            }
//    //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
//    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
//    //System.Threading.Thread.CurrentThread.CurrentCulture = ci;
//    return LocalRedirect( newurl);
//}


//Javscript Call
//public void OnPostSetLanguage(string culture)
//{

//    Response.Cookies.Append(
//         CookieRequestCultureProvider.DefaultCookieName,
//         CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US", culture)),
//         new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
//     );

//}

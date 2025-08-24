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


namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class WhatsApp : PageModel
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
            get
            {
                return !string.IsNullOrEmpty(HttpContext.Request.Query["TenantId"].ToString()) ? int.Parse(HttpContext.Request.Query["TenantId"].ToString()) : 0;
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
        public WhatsApp(ILogger<IndexModel> logger,
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
            if (this.TenantID!=0)
            {


            }
          
        }



    }
}




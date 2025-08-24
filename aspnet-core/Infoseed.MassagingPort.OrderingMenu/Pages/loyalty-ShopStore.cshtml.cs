using Infoseed.MassagingPort.OrderingMenu.Pages.Model;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Orders.Dtos;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

using Infoseed.MassagingPort.OrderingMenu.Helper;
using Infoseed.MessagingPortal.ItemAdditionsCategorys.Dtos;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.OrderDetails.Dtos;
using Infoseed.MessagingPortal.TenantServicesInfo;
using Infoseed.MessagingPortal.Web.Sunshine;
using Newtonsoft.Json;
using static Infoseed.MessagingPortal.Constants;

namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class loyalty_ShopStoreModel : PageModel
    {
        private readonly IItemsAppService _IItemsAppService;
        private readonly IOrdersAppService _IOrdersAppService;
        private IMenusAppService _iMenusAppService;
        //public List<OrderingMenuOrdersHistoryModel> lstOrderingMenuOrdersHistoryModel { get; set; }
        public List<LoyaltyRemainingdaysModel> lstLoyaltyRemainingdaysModel { get; set; }
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

        public loyalty_ShopStoreModel(
             IItemsAppService itemsAppService,
             IOrdersAppService ordersAppService,
             IMenusAppService iMenusAppService
            )
        {
            _IItemsAppService = itemsAppService;
            _IOrdersAppService = ordersAppService;
            _iMenusAppService = iMenusAppService;
        }
        public void OnGet()
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
                var are = this.AreaId;

                OrderEntity orderEntity = _IOrdersAppService.GetAllLoyaltyRemainingdays(this.ContactId, this.TenantID, 0, 10);
                //List<OrderingMenuOrdersHistoryModel> ListOrderingMenuOrdersHistoryModel = new List<OrderingMenuOrdersHistoryModel>();
                //List<LoyaltyRemainingdaysDto > vs =new List<LoyaltyRemainingdaysDto>;
                List<LoyaltyRemainingdaysModel> ListLoyaltyRemainingdaysModel = new List<LoyaltyRemainingdaysModel>();

                if (orderEntity.lstOrder != null)
                {
                    foreach (var order in orderEntity.lstOrder)
                    {
                        ListLoyaltyRemainingdaysModel.Add(new LoyaltyRemainingdaysModel
                        {
                            Points = order.LoyaltyRemainingdays.Points,
                            CreatedDate = order.LoyaltyRemainingdays.CreatedDate,//.Date,
                            LoyaltyExpiration = order.LoyaltyRemainingdays.LoyaltyExpiration,
                            OrderNumber = order.LoyaltyRemainingdays.OrderNumber,
                            Total = order.LoyaltyRemainingdays.Total,
                            Remainingdays = order.LoyaltyRemainingdays.Remainingdays,
                            OrderType = order.LoyaltyRemainingdays.OrderType,
                        });
                    }
                }
                ListLoyaltyRemainingdaysModel.Reverse();
                this.lstLoyaltyRemainingdaysModel = ListLoyaltyRemainingdaysModel;
            }
        }

    }
}

using Infoseed.MassagingPort.OrderingMenu.Pages.Model;
using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Orders;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nancy.Json;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;

namespace Infoseed.MassagingPort.OrderingMenu.Pages
{
    public class CheckOuts : PageModel
    {
        private readonly IItemsAppService _IItemsAppService;
        private readonly IOrdersAppService _IOrdersAppService;
        private IMenusAppService _iMenusAppService;
        //public List<OrderingMenuOrdersHistoryModel> lstOrderingMenuOrdersHistoryModel { get; set; }
        public CheckOutsModel  checkOutsModel { get; set; }
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
        //public an OrderPayId { get; set; }

        public bool IsLoyalty { get; set; }
        public string Key { get; set; }
        public bool NotLoyalty { get; set; }
        public string CurrencyCode { get; private set; }

        public CheckOuts(
             IItemsAppService itemsAppService,
             IOrdersAppService ordersAppService,
             IMenusAppService iMenusAppService
            )
        {
            _IItemsAppService = itemsAppService;
            _IOrdersAppService = ordersAppService;
            _iMenusAppService = iMenusAppService;
        }
        public async Task OnGetAsync()
        {
            if (this.UrlKey != "")
            {


                var key = this.UrlKey;
                var arr = key.Split('&');
                var Id = arr[0].ToString().Replace("?id=", "");

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://eu-test.oppwa.com/v1/checkouts/"+Id+"/payment?entityId=8a8294174b7ecb28014b9699220015ca");
                request.Headers.Add("Authorization", "Bearer OGE4Mjk0MTc0YjdlY2IyODAxNGI5Njk5MjIwMDE1Y2N8c3k2S0pzVDg=");
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("entityId", "8a8294174b7ecb28014b9699220015ca"));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var x = await response.Content.ReadAsStringAsync();
                checkOutsModel = JsonConvert.DeserializeObject<CheckOutsModel>(x);



            }


        }

      
     }
}

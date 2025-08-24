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
    public class OrderPay : PageModel
    {
        private readonly IItemsAppService _IItemsAppService;
        private readonly IOrdersAppService _IOrdersAppService;
        private IMenusAppService _iMenusAppService;
        //public List<OrderingMenuOrdersHistoryModel> lstOrderingMenuOrdersHistoryModel { get; set; }
        public OrderPayModel OrderPayModel { get; set; }
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

        public OrderPay(
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


                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://eu-test.oppwa.com/v1/checkouts");
                    request.Headers.Add("Authorization", "Bearer OGE4Mjk0MTc0YjdlY2IyODAxNGI5Njk5MjIwMDE1Y2N8c3k2S0pzVDg=");
                    var collection = new List<KeyValuePair<string, string>>();
                    collection.Add(new("entityId", "8a8294174b7ecb28014b9699220015ca"));
                    collection.Add(new("amount", "92.00"));
                    collection.Add(new("currency", "EUR"));
                    collection.Add(new("paymentType", "DB"));
                    var content = new FormUrlEncodedContent(collection);
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var x = await response.Content.ReadAsStringAsync();
                    OrderPayModel = JsonConvert.DeserializeObject<OrderPayModel>(x);
               
              

            }


        }

        public async Task checkouts()
        {
            Dictionary<string, dynamic> responseData;
            string data = "entityId=8a8294174b7ecb28014b9699220015ca";
            string url = "https://eu-test.oppwa.com/v1/checkouts/"+this.OrderPayModel.id+"/payment?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer OGE4Mjk0MTc0YjdlY2IyODAxNGI5Njk5MjIwMDE1Y2N8c3k2S0pzVDg=";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }

        }
     }
}

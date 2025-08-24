using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework.Data;
using Infoseed.MessagingPortal;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NUglify.Html;
using SocketIOClient;
using System.Xml;
using WhatsAppOffLine.Model;

namespace WhatsAppOffLine.Pages
{
    public class StartDailog : PageModel
    {
        private readonly ILogger<StartDailog> _logger;
        public List<CustomerChat> massgesModels;
        private readonly IJSRuntime _jsRuntime;
        IDBService _dbService;
        private readonly IDocumentClient _IDocumentClient;
        [BindProperty]
        public int TenantID { get; set; }
        [BindProperty]
        public string PhoneNumber { get; set; }
        [BindProperty]
        public string UserInput { get; set; }
        public StartDailog(ILogger<StartDailog> logger, IJSRuntime jsRuntime, IDBService dbService, IDocumentClient IDocumentClient)
        {
            _logger = logger;
            _jsRuntime= jsRuntime;
            _dbService= dbService;
            _IDocumentClient=IDocumentClient;
            this.massgesModels = new List<CustomerChat>();
            //CustomerChat massgesModel = new CustomerChat() { text="hi", sender=MessageSenderType.Customer };
            //CustomerChat massgesModel2 = new CustomerChat() { text="hlow how are you", sender=MessageSenderType.TeamInbox };
            //this.massgesModels.Add(massgesModel);
            //this.massgesModels.Add(massgesModel2);

            

        }


        public async Task<IActionResult> OnGet(int tenantID,string phonenumber)
        {
            
           
            this.TenantID = tenantID;
            this.PhoneNumber=phonenumber;
            List<CustomerChat> lstCustomerChat = _dbService.GetCustomersChat(tenantID+"_"+phonenumber, 0, 20).Result;
            lstCustomerChat.Reverse();
            this.massgesModels=lstCustomerChat.ToList();
            ViewData["ScrollToBottom"] = "scrollToBottom();";
            ViewData["InvokeFunction"] = "yourFunction();";

            //SocketIO customerClient;
            //var uri = new Uri("https://infoseedsocketioserverstg.azurewebsites.net");

            //customerClient = new SocketIO(uri, new SocketIOOptions
            //{
            //    Query = new Dictionary<string, string>
            //    {
            //        {"token", "313cb6e6-caad-4457-a0e4-669415d93250" }
            //    },
            //});
            //customerClient.OnDisconnected += Client_OnDisconnected;

            //customerClient.On("chat-get", Socket_ChatRequest);


            //customerClient.OnConnected += Socket_OnConnected;
            //customerClient.OnPing += Socket_OnPing;
            //customerClient.OnPong += Socket_OnPong;
            //customerClient.OnDisconnected += Socket_OnDisconnected;
            //customerClient.OnReconnectFailed += Client_OnReconnectFailed;
            //customerClient.OnReconnecting += Client_OnReconnecting;
            //customerClient.OnError += Socket_OnError;
            //await customerClient.ConnectAsync();
            return Page();
        }

        public async void GetChat(int tenantID, string phonenumber)
        {
            this.TenantID = tenantID;
            this.PhoneNumber=phonenumber;
            List<CustomerChat> lstCustomerChat = _dbService.GetCustomersChat(tenantID+"_"+phonenumber, 0, 20).Result;
            lstCustomerChat.Reverse();
            this.massgesModels=lstCustomerChat.ToList();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var Customer = GetCustomer(this.TenantID+"_"+this.PhoneNumber);//Get  Customer
            Customer.customerChat.text=this.UserInput;

            var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, this.TenantID+"_"+this.PhoneNumber, this.UserInput, "text", this.TenantID, 0, "", string.Empty, string.Empty, MessageSenderType.Customer, "");
            Customer.customerChat = CustomerSendChat;
            SendToRestaurantsBot(Customer);

             await Task.Delay(2000);
            // Process the registration logic here RedirectToPage("/YourPageName", new { handler = "YourHandlerName" })

            return RedirectToPage("StartDailog" , "GetChat" ,new { tenantID = this.TenantID, phonenumber = this.PhoneNumber });
        }


        public async Task<IActionResult> OnGetSendButtonAsync(string massage,int TenantID ,string phonenumber)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var Customer = GetCustomer(TenantID+"_"+phonenumber);//Get  Customer
            Customer.customerChat.text=massage;

            var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, TenantID+"_"+phonenumber, massage, "text", TenantID, 0, "", string.Empty, string.Empty, MessageSenderType.Customer, "");
            Customer.customerChat = CustomerSendChat;
            SendToRestaurantsBot(Customer);
            // await Task.Delay(2000);
            // Process the registration logic here RedirectToPage("/YourPageName", new { handler = "YourHandlerName" })

            return RedirectToPage("StartDailog", "GetChat", new { tenantID = TenantID, phonenumber = phonenumber });
        }
        private async Task SendToRestaurantsBot(CustomerModel jsonData)
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
        private CustomerModel GetCustomer(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }
        private  void Socket_ChatRequest(SocketIOResponse response)
        {
            var result = response.GetValue().ToString();
            var customerModel = JsonConvert.DeserializeObject<CustomerModel>(result);
            this.massgesModels.Add(customerModel.customerChat);
            ViewData["InvokeFunction"] = "yourFunction();";
            //testcahtAsync();
            RedirectToPage("StartDailog", "GetChat", new { tenantID = this.TenantID, phonenumber = this.PhoneNumber });

        }



        private static void Socket_OnDisconnected(object sender, string e)
        {
           // Console.WriteLine("disconnect: " + e);
        }

        private static async void Socket_OnConnected(object sender, EventArgs e)
        {
           // Console.WriteLine("Socket_OnConnected");
            var socket = sender as SocketIO;
            //Console.WriteLine("Socket.Id:" + socket.Id);

            await socket.EmitAsync("connected", 27);
        }

        private static void Socket_OnPing(object sender, EventArgs e)
        {
           // Console.WriteLine("Ping");
        }

        private static void Socket_OnPong(object sender, TimeSpan e)
        {
            //Console.WriteLine("Pong: " + e.TotalMilliseconds);
        }

        private static void Client_OnDisconnected(object sender, string e)
        {
        }

        private static void Client_OnReconnecting(object sender, int e)
        {
        }

        private static void Client_OnReconnectFailed(object sender, Exception e)
        {
        }

        private static void Socket_OnError(object sender, string e)
        {
        }

        public class OrderDriverRequestModel
        {
            public decimal? DeliveryFees { get; set; }
        }
    }
}
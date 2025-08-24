using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using IdentityServer4.Models;
using Infoseed.MessagingPortal.MenuCategories;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NUglify.Html;
using SocketIOClient;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using WhatsAppOffLine.Model;

namespace WhatsAppOffLine.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<CustomerChat> massgesModels;
        private readonly IJSRuntime _jsRuntime;
        IDBService _dbService;
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

        [BindProperty]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }


        public IndexModel(ILogger<IndexModel> logger, IJSRuntime jsRuntime, IDBService dbService)
        {
            _logger = logger;
            _jsRuntime= jsRuntime;
            _dbService= dbService;
            this.massgesModels = new List<CustomerChat>();
            //CustomerChat massgesModel = new CustomerChat() { text="hi", sender=MessageSenderType.Customer };
            //CustomerChat massgesModel2 = new CustomerChat() { text="hlow how are you", sender=MessageSenderType.TeamInbox };
            //this.massgesModels.Add(massgesModel);
            //this.massgesModels.Add(massgesModel2);

            

        }


        public async void OnGet()
        {
            
          
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Process the registration logic here

            return RedirectToPage("/StartDailog", new { tenantID= 27, phonenumber = PhoneNumber });
        }
    }
}
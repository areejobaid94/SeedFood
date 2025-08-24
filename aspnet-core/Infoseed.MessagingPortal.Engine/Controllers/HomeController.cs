//using Abp.Auditing;
//using Infoseed.MessagingPortal.Web.Controllers;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Hosting;

//namespace Infoseed.MessagingPortal.Engine.Controllers
//{
//    public class HomeController : MessagingPortalControllerBase
//    {
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public HomeController(IWebHostEnvironment webHostEnvironment)
//        {
//            _webHostEnvironment = webHostEnvironment;
//        }

//        [DisableAuditing]
//        public IActionResult Index()
//        {
//            if (_webHostEnvironment.IsDevelopment())
//            {
//                return RedirectToAction("Index", "Ui");
//            }

//            return Redirect("/index.html");
//        }
//    }
//}

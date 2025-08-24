using Microsoft.AspNetCore.Mvc;
using Infoseed.MessagingPortal.Web.Controllers;

namespace Infoseed.MessagingPortal.Web.Public.Controllers
{
    public class AboutController : MessagingPortalControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
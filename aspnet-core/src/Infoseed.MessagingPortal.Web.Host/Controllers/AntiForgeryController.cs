using Microsoft.AspNetCore.Antiforgery;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    public class AntiForgeryController : MessagingPortalControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}

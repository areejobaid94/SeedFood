using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Infoseed.MassagingPort.OrderingMenu.Pages.Model;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;


namespace Infoseed.MassagingPort.OrderingMenu.ViewComponents
{
    public class CultureSwitcherViewComponent : ViewComponent
    {



        private readonly IOptions<RequestLocalizationOptions> localizationOptions;
        public CultureSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions) =>
            this.localizationOptions = localizationOptions;

        public IViewComponentResult Invoke()
        {

            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var cultureItems = new List<SelectListItem>();
          
            cultureItems.AddRange(localizationOptions.Value.SupportedUICultures
                .Select(c => new SelectListItem { Value = c.Name, Text = c.DisplayName })
                .ToList());
            var returnUrl = string.IsNullOrEmpty(HttpContext.Request.Path) ? "~/" : $"~{HttpContext.Request.Path.Value}";

            var culture = cultureFeature.RequestCulture.Culture;
           //Response.Cookies.Append(
          HttpContext.Response.Cookies.Append(
                 CookieRequestCultureProvider.DefaultCookieName,
                 CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                 new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
             );



            var model = new CultureSwitcherModel
            {
                SupportedCultures = localizationOptions.Value.SupportedUICultures.ToList(),
                CurrentUICulture = Thread.CurrentThread.CurrentCulture
            };

            return View();

        }

       
    }
}

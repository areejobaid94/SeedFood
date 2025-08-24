using System.Globalization;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class CultureSwitcherModel
    {
        public CultureInfo CurrentUICulture { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }
    }

}

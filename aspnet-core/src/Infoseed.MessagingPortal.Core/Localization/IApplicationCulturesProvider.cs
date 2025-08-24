using System.Globalization;

namespace Infoseed.MessagingPortal.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}
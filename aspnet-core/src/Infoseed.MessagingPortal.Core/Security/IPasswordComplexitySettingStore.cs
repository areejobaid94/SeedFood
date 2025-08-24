using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}

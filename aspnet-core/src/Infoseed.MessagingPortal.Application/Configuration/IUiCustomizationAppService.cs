using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.Configuration.Dto;

namespace Infoseed.MessagingPortal.Configuration
{
    public interface IUiCustomizationSettingsAppService : IApplicationService
    {
        Task<List<ThemeSettingsDto>> GetUiManagementSettings();

        Task UpdateUiManagementSettings(ThemeSettingsDto settings);

        Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings);

        Task UseSystemDefaultSettings();
    }
}

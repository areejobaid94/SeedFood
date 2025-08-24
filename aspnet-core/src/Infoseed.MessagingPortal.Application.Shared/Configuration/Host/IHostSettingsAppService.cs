using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.Configuration.Host.Dto;

namespace Infoseed.MessagingPortal.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}

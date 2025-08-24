using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.Install.Dto;

namespace Infoseed.MessagingPortal.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}
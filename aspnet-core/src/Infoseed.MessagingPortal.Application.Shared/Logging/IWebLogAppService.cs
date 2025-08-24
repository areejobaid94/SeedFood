using Abp.Application.Services;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Logging.Dto;

namespace Infoseed.MessagingPortal.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}

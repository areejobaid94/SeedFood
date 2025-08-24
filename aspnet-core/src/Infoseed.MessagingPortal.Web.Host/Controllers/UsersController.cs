using Abp.AspNetCore.Mvc.Authorization;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Storage;
using Abp.BackgroundJobs;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}
using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.Sessions.Dto;

namespace Infoseed.MessagingPortal.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}

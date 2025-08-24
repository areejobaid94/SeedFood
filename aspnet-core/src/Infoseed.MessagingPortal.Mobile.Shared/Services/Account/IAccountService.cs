using System.Threading.Tasks;
using Infoseed.MessagingPortal.ApiClient.Models;

namespace Infoseed.MessagingPortal.Services.Account
{
    public interface IAccountService
    {
        AbpAuthenticateModel AbpAuthenticateModel { get; set; }
        
        AbpAuthenticateResultModel AuthenticateResultModel { get; set; }
        
        Task LoginUserAsync();

        Task LogoutAsync();
    }
}

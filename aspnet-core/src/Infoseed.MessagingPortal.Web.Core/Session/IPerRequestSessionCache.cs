using System.Threading.Tasks;
using Infoseed.MessagingPortal.Sessions.Dto;

namespace Infoseed.MessagingPortal.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}

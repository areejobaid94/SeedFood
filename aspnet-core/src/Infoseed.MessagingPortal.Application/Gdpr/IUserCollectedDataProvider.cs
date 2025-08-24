using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}

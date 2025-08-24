using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.Editions.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<EditionsSelectOutput> GetEditionsForSelect();

        Task<EditionSelectDto> GetEdition(int editionId);
    }
}
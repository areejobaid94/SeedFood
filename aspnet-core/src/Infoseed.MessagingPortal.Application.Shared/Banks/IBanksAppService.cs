using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Banks.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Banks
{
    public interface IBanksAppService : IApplicationService
    {
        Task<PagedResultDto<GetBankForViewDto>> GetAll(GetAllBanksInput input);

        Task<GetBankForViewDto> GetBankForView(int id);

        Task<GetBankForEditOutput> GetBankForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditBankDto input);

        Task Delete(EntityDto input);

    }
}
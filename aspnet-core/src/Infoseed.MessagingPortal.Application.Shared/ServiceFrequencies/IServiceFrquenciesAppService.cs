using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ServiceFrequencies.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ServiceFrequencies
{
    public interface IServiceFrquenciesAppService : IApplicationService
    {
        Task<PagedResultDto<GetServiceFrquencyForViewDto>> GetAll(GetAllServiceFrquenciesInput input);

        Task<GetServiceFrquencyForViewDto> GetServiceFrquencyForView(int id);

        Task<GetServiceFrquencyForEditOutput> GetServiceFrquencyForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditServiceFrquencyDto input);

        Task Delete(EntityDto input);

    }
}
using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ServiceStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ServiceStatuses
{
    public interface IServiceStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetServiceStatusForViewDto>> GetAll(GetAllServiceStatusesInput input);

        Task<GetServiceStatusForViewDto> GetServiceStatusForView(int id);

        Task<GetServiceStatusForEditOutput> GetServiceStatusForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditServiceStatusDto input);

        Task Delete(EntityDto input);

    }
}
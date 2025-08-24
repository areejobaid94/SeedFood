using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ServiceTypes.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ServiceTypes
{
    public interface IServiceTypesAppService : IApplicationService
    {
        Task<PagedResultDto<GetServiceTypeForViewDto>> GetAll(GetAllServiceTypesInput input);

        Task<GetServiceTypeForViewDto> GetServiceTypeForView(int id);

        Task<GetServiceTypeForEditOutput> GetServiceTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditServiceTypeDto input);

        Task Delete(EntityDto input);

    }
}
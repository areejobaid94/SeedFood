using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ContactStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ContactStatuses
{
    public interface IContactStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetContactStatuseForViewDto>> GetAll(GetAllContactStatusesInput input);

        Task<GetContactStatuseForEditOutput> GetContactStatuseForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditContactStatuseDto input);

        Task Delete(EntityDto input);

    }
}
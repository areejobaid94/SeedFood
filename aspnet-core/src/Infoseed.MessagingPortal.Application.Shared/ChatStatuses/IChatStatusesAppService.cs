using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ChatStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ChatStatuses
{
    public interface IChatStatusesAppService : IApplicationService
    {
        Task<PagedResultDto<GetChatStatuseForViewDto>> GetAll(GetAllChatStatusesInput input);

        Task<GetChatStatuseForEditOutput> GetChatStatuseForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditChatStatuseDto input);

        Task Delete(EntityDto input);

    }
}
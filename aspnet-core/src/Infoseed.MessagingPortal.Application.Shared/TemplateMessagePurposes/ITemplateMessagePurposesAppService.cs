using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes
{
    public interface ITemplateMessagePurposesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTemplateMessagePurposeForViewDto>> GetAll(GetAllTemplateMessagePurposesInput input);

        Task<GetTemplateMessagePurposeForViewDto> GetTemplateMessagePurposeForView(int id);

        Task<GetTemplateMessagePurposeForEditOutput> GetTemplateMessagePurposeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTemplateMessagePurposeDto input);

        Task Delete(EntityDto input);

    }
}
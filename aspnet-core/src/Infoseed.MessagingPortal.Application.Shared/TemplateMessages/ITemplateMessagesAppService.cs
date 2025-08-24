using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;
using Infoseed.MessagingPortal.TemplateMessages;
namespace Infoseed.MessagingPortal.TemplateMessages
{
    public interface ITemplateMessagesAppService : IApplicationService
    {
        Task<PagedResultDto<GetTemplateMessageForViewDto>> GetAll(GetAllTemplateMessagesInput input);
        Task<PagedResultDto<GetTemplateMessageForViewDto>> GetAllNoFilter();

        GetTemplateMessageForViewDto GetTemplateMessageFromTenantId(int? tenantId);

        Task<GetTemplateMessageForViewDto> GetTemplateMessageForView(int id);

        Task<GetTemplateMessageForEditOutput> GetTemplateMessageForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTemplateMessageDto input);

        Task CreateTemplate();

        Task Delete(EntityDto input);

        Task<FileDto> GetTemplateMessagesToExcel(GetAllTemplateMessagesForExcelInput input);

        Task<List<TemplateMessageTemplateMessagePurposeLookupTableDto>> GetAllTemplateMessagePurposeForTableDropdown();

    }
}
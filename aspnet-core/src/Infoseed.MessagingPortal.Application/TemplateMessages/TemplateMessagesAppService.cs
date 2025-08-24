using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Framework.Data;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.TemplateMessagePurposes;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.TemplateMessages.Exporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.TemplateMessages
{
    [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages)]
    public class TemplateMessagesAppService : MessagingPortalAppServiceBase, ITemplateMessagesAppService
    {
        private readonly IRepository<TemplateMessage> _templateMessageRepository;
        private readonly ITemplateMessagesExcelExporter _templateMessagesExcelExporter;
        private readonly IRepository<TemplateMessagePurpose, int> _lookup_templateMessagePurposeRepository;

        public TemplateMessagesAppService(IRepository<TemplateMessage> templateMessageRepository, ITemplateMessagesExcelExporter templateMessagesExcelExporter, IRepository<TemplateMessagePurpose, int> lookup_templateMessagePurposeRepository)
        {
            _templateMessageRepository = templateMessageRepository;
            _templateMessagesExcelExporter = templateMessagesExcelExporter;
            _lookup_templateMessagePurposeRepository = lookup_templateMessagePurposeRepository;

        }

        public async Task<PagedResultDto<GetTemplateMessageForViewDto>> GetAll(GetAllTemplateMessagesInput input)
        {

            var filteredTemplateMessages = _templateMessageRepository.GetAll()
                        .Include(e => e.TemplateMessagePurposeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TemplateMessageName.Contains(input.Filter) || e.MessageText.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TemplateMessageNameFilter), e => e.TemplateMessageName == input.TemplateMessageNameFilter)
                        .WhereIf(input.MinMessageCreationDateFilter != null, e => e.MessageCreationDate >= input.MinMessageCreationDateFilter)
                        .WhereIf(input.MaxMessageCreationDateFilter != null, e => e.MessageCreationDate <= input.MaxMessageCreationDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TemplateMessagePurposePurposeFilter), e => e.TemplateMessagePurposeFk != null && e.TemplateMessagePurposeFk.Purpose == input.TemplateMessagePurposePurposeFilter);

            var pagedAndFilteredTemplateMessages = filteredTemplateMessages
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var templateMessages = from o in pagedAndFilteredTemplateMessages
                                   join o1 in _lookup_templateMessagePurposeRepository.GetAll() on o.TemplateMessagePurposeId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   select new GetTemplateMessageForViewDto()
                                   {
                                       TemplateMessage = new TemplateMessageDto
                                       {
                                           TemplateMessageName = o.TemplateMessageName,
                                           MessageCreationDate = o.MessageCreationDate,
                                           Id = o.Id,
                                           AttachmentId = o.AttachmentId
                                       },
                                       TemplateMessagePurposePurpose = s1 == null || s1.Purpose == null ? "" : s1.Purpose.ToString()
                                   };

            var totalCount = await filteredTemplateMessages.CountAsync();

            return new PagedResultDto<GetTemplateMessageForViewDto>(
                totalCount,
                await templateMessages.ToListAsync()
            );
        }


        private static GetTemplateMessageForViewDto ConvertItemsToDto(IDataReader dataReader)
        {
            return new GetTemplateMessageForViewDto
            {
                TemplateMessage = new TemplateMessageDto
                {
                    Id = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                    TemplateMessageName = SqlDataHelper.GetValue<string>(dataReader, "TemplateMessageName"),
                    TemplateMessagePurposeId = SqlDataHelper.GetValue<int>(dataReader, "TemplateMessagePurposeId"),
                    MessageCreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "MessageCreationDate"),
                    AttachmentId = SqlDataHelper.GetValue<int?>(dataReader, "AttachmentId")
                    // MessageText = SqlDataHelper.GetValue<string>(dataReader, "MessageText") // Uncomment if needed
                },
                TemplateMessagePurposePurpose = SqlDataHelper.GetValue<string>(dataReader, "TemplateMessagePurposePurpose")
            };
        }


        public async Task<PagedResultDto<GetTemplateMessageForViewDto>> GetAllNoFilter()
        {
            try
            {
                var SP_Name = Constants.TemplateMessage.SP_GetAllTemplateMessagesNoFilter;
                var sqlParameters = new List<SqlParameter>
        {
            new SqlParameter("@TenantId", (int?)AbpSession.TenantId)
        };

                var items = SqlDataHelper.ExecuteReader(
                    SP_Name,
                    sqlParameters.ToArray(),
                    ConvertItemsToDto,
                    AppSettingsModel.ConnectionStrings
                ).ToList();

                return new PagedResultDto<GetTemplateMessageForViewDto>
                {
                    Items = items,
                    TotalCount = items.Count
                };
            }
            catch
            {
                return new PagedResultDto<GetTemplateMessageForViewDto>
                {
                    Items = new List<GetTemplateMessageForViewDto>(),
                    TotalCount = 0
                };
            }
        }


        public GetTemplateMessageForViewDto GetTemplateMessageFromTenantId(int? tenantId)
        {
            var TemplateMessages = _templateMessageRepository.GetAll().ToList();

            //var templateMessagePurpose = _lookup_templateMessagePurposeRepository.GetAll().ToList();

            var Template = TemplateMessages.Where(x => x.TenantId == tenantId && x.TemplateMessagePurposeFk.Purpose== "WorkTime").FirstOrDefault();
            //var MessagePurpose = templateMessagePurpose.Where(x => x.Id == Template.TemplateMessagePurposeId).FirstOrDefault();

            

            if (Template!=null)
            {

                GetTemplateMessageForViewDto getTemplateMessageForViewDto = new GetTemplateMessageForViewDto
                {

                    TemplateMessage = new TemplateMessageDto
                    {
                        AttachmentId = Template.AttachmentId,
                        Id = Template.Id,
                        MessageCreationDate = Template.MessageCreationDate,
                        TemplateMessageName = Template.TemplateMessageName,
                        TemplateMessagePurposeId = Template.TemplateMessagePurposeId

                    },
                    TemplateMessagePurposePurpose = Template.TemplateMessagePurposeFk.Purpose
                };
                return getTemplateMessageForViewDto;
            }

            
            return null;
        }



        public async Task<GetTemplateMessageForViewDto> GetTemplateMessageForView(int id)
        {
            var templateMessage = await _templateMessageRepository.GetAsync(id);

            var output = new GetTemplateMessageForViewDto { TemplateMessage = ObjectMapper.Map<TemplateMessageDto>(templateMessage) };

            //if (output.TemplateMessage.TemplateMessagePurposeId != null)
            //{
                var _lookupTemplateMessagePurpose = await _lookup_templateMessagePurposeRepository.FirstOrDefaultAsync((int)output.TemplateMessage.TemplateMessagePurposeId);
                output.TemplateMessagePurposePurpose = _lookupTemplateMessagePurpose?.Purpose?.ToString();
            //}

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages_Edit)]
        public async Task<GetTemplateMessageForEditOutput> GetTemplateMessageForEdit(EntityDto input)
        {
            var templateMessage = await _templateMessageRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTemplateMessageForEditOutput { TemplateMessage = ObjectMapper.Map<CreateOrEditTemplateMessageDto>(templateMessage) };

            //if (output.TemplateMessage.TemplateMessagePurposeId != null)
            //{
                var _lookupTemplateMessagePurpose = await _lookup_templateMessagePurposeRepository.FirstOrDefaultAsync((int)output.TemplateMessage.TemplateMessagePurposeId);
                output.TemplateMessagePurposePurpose = _lookupTemplateMessagePurpose?.Purpose?.ToString();
            //}

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTemplateMessageDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages_Create)]
        protected virtual async Task Create(CreateOrEditTemplateMessageDto input)
        {
            var templateMessage = ObjectMapper.Map<TemplateMessage>(input);

            if (AbpSession.TenantId != null)
            {
                templateMessage.TenantId = (int?)AbpSession.TenantId;
            }

            await _templateMessageRepository.InsertAsync(templateMessage);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages_Edit)]
        protected virtual async Task Update(CreateOrEditTemplateMessageDto input)
        {
            var templateMessage = await _templateMessageRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, templateMessage);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _templateMessageRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetTemplateMessagesToExcel(GetAllTemplateMessagesForExcelInput input)
        {

            var filteredTemplateMessages = _templateMessageRepository.GetAll()
                        .Include(e => e.TemplateMessagePurposeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TemplateMessageName.Contains(input.Filter) || e.MessageText.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TemplateMessageNameFilter), e => e.TemplateMessageName == input.TemplateMessageNameFilter)
                        .WhereIf(input.MinMessageCreationDateFilter != null, e => e.MessageCreationDate >= input.MinMessageCreationDateFilter)
                        .WhereIf(input.MaxMessageCreationDateFilter != null, e => e.MessageCreationDate <= input.MaxMessageCreationDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TemplateMessagePurposePurposeFilter), e => e.TemplateMessagePurposeFk != null && e.TemplateMessagePurposeFk.Purpose == input.TemplateMessagePurposePurposeFilter);

            var query = (from o in filteredTemplateMessages
                         join o1 in _lookup_templateMessagePurposeRepository.GetAll() on o.TemplateMessagePurposeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new GetTemplateMessageForViewDto()
                         {
                             TemplateMessage = new TemplateMessageDto
                             {
                                 TemplateMessageName = o.TemplateMessageName,
                                 MessageCreationDate = o.MessageCreationDate,
                                 Id = o.Id
                             },
                             TemplateMessagePurposePurpose = s1 == null || s1.Purpose == null ? "" : s1.Purpose.ToString()
                         });

            var templateMessageListDtos = await query.ToListAsync();

            return _templateMessagesExcelExporter.ExportToFile(templateMessageListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_TemplateMessages)]
        public async Task<List<TemplateMessageTemplateMessagePurposeLookupTableDto>> GetAllTemplateMessagePurposeForTableDropdown()
        {
            return await _lookup_templateMessagePurposeRepository.GetAll()
                .Select(templateMessagePurpose => new TemplateMessageTemplateMessagePurposeLookupTableDto
                {
                    Id = templateMessagePurpose.Id,
                    DisplayName = templateMessagePurpose == null || templateMessagePurpose.Purpose == null ? "" : templateMessagePurpose.Purpose.ToString()
                }).ToListAsync();
        }

        public async Task CreateTemplate()
        {
            CreateOrEditTemplateMessageDto createOrEditTemplateMessageDto = new CreateOrEditTemplateMessageDto
            {
                TemplateMessagePurposeId = 1,
                MessageCreationDate = DateTime.Now,
                MessageText = "sorry our working hour from 8Am to 5Pm",
                TemplateMessageName = "working hour"

            };
            await Create(createOrEditTemplateMessageDto);
        }
    }
}
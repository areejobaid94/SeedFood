using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ChatStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.ChatStatuses
{
    [AbpAuthorize(AppPermissions.Pages_Administration_ChatStatuses)]
    public class ChatStatusesAppService : MessagingPortalAppServiceBase, IChatStatusesAppService
    {
        private readonly IRepository<ChatStatuse> _chatStatuseRepository;

        public ChatStatusesAppService(IRepository<ChatStatuse> chatStatuseRepository)
        {
            _chatStatuseRepository = chatStatuseRepository;

        }

        public async Task<PagedResultDto<GetChatStatuseForViewDto>> GetAll(GetAllChatStatusesInput input)
        {

            var filteredChatStatuses = _chatStatuseRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ChatStatusName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChatStatusNameFilter), e => e.ChatStatusName == input.ChatStatusNameFilter)
                        .WhereIf(input.IsEnabledFilter.HasValue && input.IsEnabledFilter > -1, e => (input.IsEnabledFilter == 1 && e.IsEnabled) || (input.IsEnabledFilter == 0 && !e.IsEnabled));

            var pagedAndFilteredChatStatuses = filteredChatStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var chatStatuses = from o in pagedAndFilteredChatStatuses
                               select new GetChatStatuseForViewDto()
                               {
                                   ChatStatuse = new ChatStatuseDto
                                   {
                                       ChatStatusName = o.ChatStatusName,
                                       IsEnabled = o.IsEnabled,
                                       Id = o.Id
                                   }
                               };

            var totalCount = await filteredChatStatuses.CountAsync();

            return new PagedResultDto<GetChatStatuseForViewDto>(
                totalCount,
                await chatStatuses.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ChatStatuses_Edit)]
        public async Task<GetChatStatuseForEditOutput> GetChatStatuseForEdit(EntityDto input)
        {
            var chatStatuse = await _chatStatuseRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetChatStatuseForEditOutput { ChatStatuse = ObjectMapper.Map<CreateOrEditChatStatuseDto>(chatStatuse) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditChatStatuseDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_ChatStatuses_Create)]
        protected virtual async Task Create(CreateOrEditChatStatuseDto input)
        {
            var chatStatuse = ObjectMapper.Map<ChatStatuse>(input);

            await _chatStatuseRepository.InsertAsync(chatStatuse);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ChatStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditChatStatuseDto input)
        {
            var chatStatuse = await _chatStatuseRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, chatStatuse);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ChatStatuses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _chatStatuseRepository.DeleteAsync(input.Id);
        }
    }
}
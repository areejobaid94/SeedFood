using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ContactStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.ContactStatuses
{
    [AbpAuthorize(AppPermissions.Pages_Administration_ContactStatuses)]
    public class ContactStatusesAppService : MessagingPortalAppServiceBase, IContactStatusesAppService
    {
        private readonly IRepository<ContactStatuse> _contactStatuseRepository;

        public ContactStatusesAppService(IRepository<ContactStatuse> contactStatuseRepository)
        {
            _contactStatuseRepository = contactStatuseRepository;

        }

        public async Task<PagedResultDto<GetContactStatuseForViewDto>> GetAll(GetAllContactStatusesInput input)
        {

            var filteredContactStatuses = _contactStatuseRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ContactStatusName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactStatusNameFilter), e => e.ContactStatusName == input.ContactStatusNameFilter)
                        .WhereIf(input.IsEnabledFilter.HasValue && input.IsEnabledFilter > -1, e => (input.IsEnabledFilter == 1 && e.IsEnabled) || (input.IsEnabledFilter == 0 && !e.IsEnabled));

            var pagedAndFilteredContactStatuses = filteredContactStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var contactStatuses = from o in pagedAndFilteredContactStatuses
                                  select new GetContactStatuseForViewDto()
                                  {
                                      ContactStatuse = new ContactStatuseDto
                                      {
                                          ContactStatusName = o.ContactStatusName,
                                          IsEnabled = o.IsEnabled,
                                          Id = o.Id
                                      }
                                  };

            var totalCount = await filteredContactStatuses.CountAsync();

            return new PagedResultDto<GetContactStatuseForViewDto>(
                totalCount,
                await contactStatuses.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ContactStatuses_Edit)]
        public async Task<GetContactStatuseForEditOutput> GetContactStatuseForEdit(EntityDto input)
        {
            var contactStatuse = await _contactStatuseRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetContactStatuseForEditOutput { ContactStatuse = ObjectMapper.Map<CreateOrEditContactStatuseDto>(contactStatuse) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditContactStatuseDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_ContactStatuses_Create)]
        protected virtual async Task Create(CreateOrEditContactStatuseDto input)
        {
            var contactStatuse = ObjectMapper.Map<ContactStatuse>(input);

            await _contactStatuseRepository.InsertAsync(contactStatuse);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ContactStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditContactStatuseDto input)
        {
            var contactStatuse = await _contactStatuseRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, contactStatuse);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ContactStatuses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _contactStatuseRepository.DeleteAsync(input.Id);
        }
    }
}
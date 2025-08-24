using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.TemplateMessagePurposes.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.TemplateMessagePurposes
{
    [AbpAuthorize(AppPermissions.Pages_TemplateMessagePurposes)]
    public class TemplateMessagePurposesAppService : MessagingPortalAppServiceBase, ITemplateMessagePurposesAppService
    {
        private readonly IRepository<TemplateMessagePurpose> _templateMessagePurposeRepository;

        public TemplateMessagePurposesAppService(IRepository<TemplateMessagePurpose> templateMessagePurposeRepository)
        {
            _templateMessagePurposeRepository = templateMessagePurposeRepository;

        }

        public async Task<PagedResultDto<GetTemplateMessagePurposeForViewDto>> GetAll(GetAllTemplateMessagePurposesInput input)
        {

            var filteredTemplateMessagePurposes = _templateMessagePurposeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Purpose.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PurposeFilter), e => e.Purpose == input.PurposeFilter);

            var pagedAndFilteredTemplateMessagePurposes = filteredTemplateMessagePurposes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var templateMessagePurposes = from o in pagedAndFilteredTemplateMessagePurposes
                                          select new GetTemplateMessagePurposeForViewDto()
                                          {
                                              TemplateMessagePurpose = new TemplateMessagePurposeDto
                                              {
                                                  Purpose = o.Purpose,
                                                  Id = o.Id
                                              }
                                          };

            var totalCount = await filteredTemplateMessagePurposes.CountAsync();

            return new PagedResultDto<GetTemplateMessagePurposeForViewDto>(
                totalCount,
                await templateMessagePurposes.ToListAsync()
            );
        }

        public async Task<GetTemplateMessagePurposeForViewDto> GetTemplateMessagePurposeForView(int id)
        {
            var templateMessagePurpose = await _templateMessagePurposeRepository.GetAsync(id);

            var output = new GetTemplateMessagePurposeForViewDto { TemplateMessagePurpose = ObjectMapper.Map<TemplateMessagePurposeDto>(templateMessagePurpose) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TemplateMessagePurposes_Edit)]
        public async Task<GetTemplateMessagePurposeForEditOutput> GetTemplateMessagePurposeForEdit(EntityDto input)
        {
            var templateMessagePurpose = await _templateMessagePurposeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTemplateMessagePurposeForEditOutput { TemplateMessagePurpose = ObjectMapper.Map<CreateOrEditTemplateMessagePurposeDto>(templateMessagePurpose) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTemplateMessagePurposeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TemplateMessagePurposes_Create)]
        protected virtual async Task Create(CreateOrEditTemplateMessagePurposeDto input)
        {
            var templateMessagePurpose = ObjectMapper.Map<TemplateMessagePurpose>(input);

            await _templateMessagePurposeRepository.InsertAsync(templateMessagePurpose);
        }

        [AbpAuthorize(AppPermissions.Pages_TemplateMessagePurposes_Edit)]
        protected virtual async Task Update(CreateOrEditTemplateMessagePurposeDto input)
        {
            var templateMessagePurpose = await _templateMessagePurposeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, templateMessagePurpose);
        }

        [AbpAuthorize(AppPermissions.Pages_TemplateMessagePurposes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _templateMessagePurposeRepository.DeleteAsync(input.Id);
        }
    }
}
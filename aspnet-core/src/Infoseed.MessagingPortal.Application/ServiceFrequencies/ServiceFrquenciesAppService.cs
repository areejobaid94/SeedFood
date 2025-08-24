using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ServiceFrequencies.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.ServiceFrequencies
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceFrquencies)]
    public class ServiceFrquenciesAppService : MessagingPortalAppServiceBase, IServiceFrquenciesAppService
    {
        private readonly IRepository<ServiceFrquency> _serviceFrquencyRepository;

        public ServiceFrquenciesAppService(IRepository<ServiceFrquency> serviceFrquencyRepository)
        {
            _serviceFrquencyRepository = serviceFrquencyRepository;

        }

        public async Task<PagedResultDto<GetServiceFrquencyForViewDto>> GetAll(GetAllServiceFrquenciesInput input)
        {

            var filteredServiceFrquencies = _serviceFrquencyRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServiceFrequencyName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceFrequencyNameFilter), e => e.ServiceFrequencyName == input.ServiceFrequencyNameFilter)
                        .WhereIf(input.IsEnabledFilter.HasValue && input.IsEnabledFilter > -1, e => (input.IsEnabledFilter == 1 && e.IsEnabled) || (input.IsEnabledFilter == 0 && !e.IsEnabled))
                        .WhereIf(input.MinCreationDateFilter != null, e => e.CreationDate >= input.MinCreationDateFilter)
                        .WhereIf(input.MaxCreationDateFilter != null, e => e.CreationDate <= input.MaxCreationDateFilter);

            var pagedAndFilteredServiceFrquencies = filteredServiceFrquencies
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var serviceFrquencies = from o in pagedAndFilteredServiceFrquencies
                                    select new GetServiceFrquencyForViewDto()
                                    {
                                        ServiceFrquency = new ServiceFrquencyDto
                                        {
                                            ServiceFrequencyName = o.ServiceFrequencyName,
                                            IsEnabled = o.IsEnabled,
                                            CreationDate = o.CreationDate,
                                            Id = o.Id
                                        }
                                    };

            var totalCount = await filteredServiceFrquencies.CountAsync();

            return new PagedResultDto<GetServiceFrquencyForViewDto>(
                totalCount,
                await serviceFrquencies.ToListAsync()
            );
        }

        public async Task<GetServiceFrquencyForViewDto> GetServiceFrquencyForView(int id)
        {
            var serviceFrquency = await _serviceFrquencyRepository.GetAsync(id);

            var output = new GetServiceFrquencyForViewDto { ServiceFrquency = ObjectMapper.Map<ServiceFrquencyDto>(serviceFrquency) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ServiceFrquencies_Edit)]
        public async Task<GetServiceFrquencyForEditOutput> GetServiceFrquencyForEdit(EntityDto input)
        {
            var serviceFrquency = await _serviceFrquencyRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetServiceFrquencyForEditOutput { ServiceFrquency = ObjectMapper.Map<CreateOrEditServiceFrquencyDto>(serviceFrquency) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditServiceFrquencyDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceFrquencies_Create)]
        protected virtual async Task Create(CreateOrEditServiceFrquencyDto input)
        {
            var serviceFrquency = ObjectMapper.Map<ServiceFrquency>(input);

            await _serviceFrquencyRepository.InsertAsync(serviceFrquency);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceFrquencies_Edit)]
        protected virtual async Task Update(CreateOrEditServiceFrquencyDto input)
        {
            var serviceFrquency = await _serviceFrquencyRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, serviceFrquency);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceFrquencies_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _serviceFrquencyRepository.DeleteAsync(input.Id);
        }
    }
}
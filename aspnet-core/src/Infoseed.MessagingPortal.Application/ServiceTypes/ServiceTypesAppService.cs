using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ServiceTypes.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.ServiceTypes
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceTypes)]
    public class ServiceTypesAppService : MessagingPortalAppServiceBase, IServiceTypesAppService
    {
        private readonly IRepository<ServiceType> _serviceTypeRepository;

        public ServiceTypesAppService(IRepository<ServiceType> serviceTypeRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;

        }

        public async Task<PagedResultDto<GetServiceTypeForViewDto>> GetAll(GetAllServiceTypesInput input)
        {

            var filteredServiceTypes = _serviceTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServicetypeName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServicetypeNameFilter), e => e.ServicetypeName == input.ServicetypeNameFilter)
                        .WhereIf(input.IsEnabledFilter.HasValue && input.IsEnabledFilter > -1, e => (input.IsEnabledFilter == 1 && e.IsEnabled) || (input.IsEnabledFilter == 0 && !e.IsEnabled))
                        .WhereIf(input.MinCreationDateFilter != null, e => e.CreationDate >= input.MinCreationDateFilter)
                        .WhereIf(input.MaxCreationDateFilter != null, e => e.CreationDate <= input.MaxCreationDateFilter);

            var pagedAndFilteredServiceTypes = filteredServiceTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var serviceTypes = from o in pagedAndFilteredServiceTypes
                               select new GetServiceTypeForViewDto()
                               {
                                   ServiceType = new ServiceTypeDto
                                   {
                                       ServicetypeName = o.ServicetypeName,
                                       IsEnabled = o.IsEnabled,
                                       CreationDate = o.CreationDate,
                                       Id = o.Id
                                   }
                               };

            var totalCount = await filteredServiceTypes.CountAsync();

            return new PagedResultDto<GetServiceTypeForViewDto>(
                totalCount,
                await serviceTypes.ToListAsync()
            );
        }

        public async Task<GetServiceTypeForViewDto> GetServiceTypeForView(int id)
        {
            var serviceType = await _serviceTypeRepository.GetAsync(id);

            var output = new GetServiceTypeForViewDto { ServiceType = ObjectMapper.Map<ServiceTypeDto>(serviceType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_ServiceTypes_Edit)]
        public async Task<GetServiceTypeForEditOutput> GetServiceTypeForEdit(EntityDto input)
        {
            var serviceType = await _serviceTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetServiceTypeForEditOutput { ServiceType = ObjectMapper.Map<CreateOrEditServiceTypeDto>(serviceType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditServiceTypeDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceTypes_Create)]
        protected virtual async Task Create(CreateOrEditServiceTypeDto input)
        {
            var serviceType = ObjectMapper.Map<ServiceType>(input);

            await _serviceTypeRepository.InsertAsync(serviceType);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceTypes_Edit)]
        protected virtual async Task Update(CreateOrEditServiceTypeDto input)
        {
            var serviceType = await _serviceTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, serviceType);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _serviceTypeRepository.DeleteAsync(input.Id);
        }
    }
}
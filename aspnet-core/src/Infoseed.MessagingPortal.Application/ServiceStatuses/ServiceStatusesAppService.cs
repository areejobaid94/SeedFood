using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.ServiceStatuses.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.ServiceStatuses
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceStatuses)]
    public class ServiceStatusesAppService : MessagingPortalAppServiceBase, IServiceStatusesAppService
    {
        private readonly IRepository<ServiceStatus> _serviceStatusRepository;

        public ServiceStatusesAppService(IRepository<ServiceStatus> serviceStatusRepository)
        {
            _serviceStatusRepository = serviceStatusRepository;

        }

        public async Task<PagedResultDto<GetServiceStatusForViewDto>> GetAll(GetAllServiceStatusesInput input)
        {

            var filteredServiceStatuses = _serviceStatusRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServiceStatusName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceStatusNameFilter), e => e.ServiceStatusName == input.ServiceStatusNameFilter)
                        .WhereIf(input.IsEnabledFilter.HasValue && input.IsEnabledFilter > -1, e => (input.IsEnabledFilter == 1 && e.IsEnabled) || (input.IsEnabledFilter == 0 && !e.IsEnabled))
                        .WhereIf(input.MinCreationDateFilter != null, e => e.CreationDate >= input.MinCreationDateFilter)
                        .WhereIf(input.MaxCreationDateFilter != null, e => e.CreationDate <= input.MaxCreationDateFilter);

            var pagedAndFilteredServiceStatuses = filteredServiceStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var serviceStatuses = from o in pagedAndFilteredServiceStatuses
                                  select new GetServiceStatusForViewDto()
                                  {
                                      ServiceStatus = new ServiceStatusDto
                                      {
                                          ServiceStatusName = o.ServiceStatusName,
                                          IsEnabled = o.IsEnabled,
                                          CreationDate = o.CreationDate,
                                          Id = o.Id
                                      }
                                  };

            var totalCount = await filteredServiceStatuses.CountAsync();

            return new PagedResultDto<GetServiceStatusForViewDto>(
                totalCount,
                await serviceStatuses.ToListAsync()
            );
        }

        public async Task<GetServiceStatusForViewDto> GetServiceStatusForView(int id)
        {
            var serviceStatus = await _serviceStatusRepository.GetAsync(id);

            var output = new GetServiceStatusForViewDto { ServiceStatus = ObjectMapper.Map<ServiceStatusDto>(serviceStatus) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceStatuses_Edit)]
        public async Task<GetServiceStatusForEditOutput> GetServiceStatusForEdit(EntityDto input)
        {
            var serviceStatus = await _serviceStatusRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetServiceStatusForEditOutput { ServiceStatus = ObjectMapper.Map<CreateOrEditServiceStatusDto>(serviceStatus) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditServiceStatusDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceStatuses_Create)]
        protected virtual async Task Create(CreateOrEditServiceStatusDto input)
        {
            var serviceStatus = ObjectMapper.Map<ServiceStatus>(input);

            await _serviceStatusRepository.InsertAsync(serviceStatus);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceStatuses_Edit)]
        protected virtual async Task Update(CreateOrEditServiceStatusDto input)
        {
            var serviceStatus = await _serviceStatusRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, serviceStatus);
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_ServiceStatuses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _serviceStatusRepository.DeleteAsync(input.Id);
        }
    }
}
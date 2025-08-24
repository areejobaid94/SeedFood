using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.InfoSeedServices.Dtos;
using Infoseed.MessagingPortal.InfoSeedServices.Exporting;
using Infoseed.MessagingPortal.ServiceFrequencies;
using Infoseed.MessagingPortal.ServiceStatuses;
using Infoseed.MessagingPortal.ServiceTypes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.InfoSeedServices
{
    [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices)]
    public class InfoSeedServicesAppService : MessagingPortalAppServiceBase, IInfoSeedServicesAppService
    {
        private readonly IRepository<InfoSeedService> _infoSeedServiceRepository;
        private readonly IInfoSeedServicesExcelExporter _infoSeedServicesExcelExporter;
        private readonly IRepository<ServiceType, int> _lookup_serviceTypeRepository;
        private readonly IRepository<ServiceStatus, int> _lookup_serviceStatusRepository;
        private readonly IRepository<ServiceFrquency, int> _lookup_serviceFrquencyRepository;

        public InfoSeedServicesAppService(IRepository<InfoSeedService> infoSeedServiceRepository, IInfoSeedServicesExcelExporter infoSeedServicesExcelExporter, IRepository<ServiceType, int> lookup_serviceTypeRepository, IRepository<ServiceStatus, int> lookup_serviceStatusRepository, IRepository<ServiceFrquency, int> lookup_serviceFrquencyRepository)
        {
            _infoSeedServiceRepository = infoSeedServiceRepository;
            _infoSeedServicesExcelExporter = infoSeedServicesExcelExporter;
            _lookup_serviceTypeRepository = lookup_serviceTypeRepository;
            _lookup_serviceStatusRepository = lookup_serviceStatusRepository;
            _lookup_serviceFrquencyRepository = lookup_serviceFrquencyRepository;

        }

        public async Task<PagedResultDto<GetInfoSeedServiceForViewDto>> GetAll(GetAllInfoSeedServicesInput input)
        {

            var filteredInfoSeedServices = _infoSeedServiceRepository.GetAll()
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.ServiceStatusFk)
                        .Include(e => e.ServiceFrquencyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServiceID.Contains(input.Filter) || e.ServiceName.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceIDFilter), e => e.ServiceID == input.ServiceIDFilter)
                        .WhereIf(input.MinServiceFeesFilter != null, e => e.ServiceFees >= input.MinServiceFeesFilter)
                        .WhereIf(input.MaxServiceFeesFilter != null, e => e.ServiceFees <= input.MaxServiceFeesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceNameFilter), e => e.ServiceName == input.ServiceNameFilter)
                        .WhereIf(input.MinServiceCreationDateFilter != null, e => e.ServiceCreationDate >= input.MinServiceCreationDateFilter)
                        .WhereIf(input.MaxServiceCreationDateFilter != null, e => e.ServiceCreationDate <= input.MaxServiceCreationDateFilter)
                        .WhereIf(input.MinServiceStoppingDateFilter != null, e => e.ServiceStoppingDate >= input.MinServiceStoppingDateFilter)
                        .WhereIf(input.MaxServiceStoppingDateFilter != null, e => e.ServiceStoppingDate <= input.MaxServiceStoppingDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceTypeServicetypeNameFilter), e => e.ServiceTypeFk != null && e.ServiceTypeFk.ServicetypeName == input.ServiceTypeServicetypeNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceStatusServiceStatusNameFilter), e => e.ServiceStatusFk != null && e.ServiceStatusFk.ServiceStatusName == input.ServiceStatusServiceStatusNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceFrquencyServiceFrequencyNameFilter), e => e.ServiceFrquencyFk != null && e.ServiceFrquencyFk.ServiceFrequencyName == input.ServiceFrquencyServiceFrequencyNameFilter);

            var pagedAndFilteredInfoSeedServices = filteredInfoSeedServices
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var infoSeedServices = from o in pagedAndFilteredInfoSeedServices
                                   join o1 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_serviceStatusRepository.GetAll() on o.ServiceStatusId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   join o3 in _lookup_serviceFrquencyRepository.GetAll() on o.ServiceFrquencyId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   select new GetInfoSeedServiceForViewDto()
                                   {
                                       InfoSeedService = new InfoSeedServiceDto
                                       {
                                           ServiceID = o.ServiceID,
                                           ServiceName = o.ServiceName,
                                           ServiceCreationDate = o.ServiceCreationDate,
                                           ServiceStoppingDate = o.ServiceStoppingDate,
                                             FeesForFirstOrder=o.FeesForFirstOrder,
                                              FirstNumberOfOrders=o.FirstNumberOfOrders,
                                           Id = o.Id
                                       },
                                       ServiceTypeServicetypeName = s1 == null || s1.ServicetypeName == null ? "" : s1.ServicetypeName.ToString(),
                                       ServiceStatusServiceStatusName = s2 == null || s2.ServiceStatusName == null ? "" : s2.ServiceStatusName.ToString(),
                                       ServiceFrquencyServiceFrequencyName = s3 == null || s3.ServiceFrequencyName == null ? "" : s3.ServiceFrequencyName.ToString()
                                   };

            var totalCount = await filteredInfoSeedServices.CountAsync();

            return new PagedResultDto<GetInfoSeedServiceForViewDto>(
                totalCount,
                await infoSeedServices.ToListAsync()
            );
        }

        public async Task<GetInfoSeedServiceForViewDto> GetInfoSeedServiceForView(int id)
        {
            var infoSeedService = await _infoSeedServiceRepository.GetAsync(id);

            var output = new GetInfoSeedServiceForViewDto { InfoSeedService = ObjectMapper.Map<InfoSeedServiceDto>(infoSeedService) };

            //if (output.InfoSeedService.ServiceTypeId != null)
            //{
                var _lookupServiceType = await _lookup_serviceTypeRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceTypeId);
                output.ServiceTypeServicetypeName = _lookupServiceType?.ServicetypeName?.ToString();
            //}

            //if (output.InfoSeedService.ServiceStatusId != null)
            //{
                var _lookupServiceStatus = await _lookup_serviceStatusRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceStatusId);
                output.ServiceStatusServiceStatusName = _lookupServiceStatus?.ServiceStatusName?.ToString();
            //}

            //if (output.InfoSeedService.ServiceFrquencyId != null)
            //{
                var _lookupServiceFrquency = await _lookup_serviceFrquencyRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceFrquencyId);
                output.ServiceFrquencyServiceFrequencyName = _lookupServiceFrquency?.ServiceFrequencyName?.ToString();
            //}

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices_Edit)]
        public async Task<GetInfoSeedServiceForEditOutput> GetInfoSeedServiceForEdit(EntityDto input)
        {
            var infoSeedService = await _infoSeedServiceRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetInfoSeedServiceForEditOutput { InfoSeedService = ObjectMapper.Map<CreateOrEditInfoSeedServiceDto>(infoSeedService) };

            //if (output.InfoSeedService.ServiceTypeId != null)
            //{
                var _lookupServiceType = await _lookup_serviceTypeRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceTypeId);
                output.ServiceTypeServicetypeName = _lookupServiceType?.ServicetypeName?.ToString();
            //}

            //if (output.InfoSeedService.ServiceStatusId != null)
            //{
                var _lookupServiceStatus = await _lookup_serviceStatusRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceStatusId);
                output.ServiceStatusServiceStatusName = _lookupServiceStatus?.ServiceStatusName?.ToString();
            //}

            //if (output.InfoSeedService.ServiceFrquencyId != null)
            //{
                var _lookupServiceFrquency = await _lookup_serviceFrquencyRepository.FirstOrDefaultAsync((int)output.InfoSeedService.ServiceFrquencyId);
                output.ServiceFrquencyServiceFrequencyName = _lookupServiceFrquency?.ServiceFrequencyName?.ToString();
            //}

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditInfoSeedServiceDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices_Create)]
        protected virtual async Task Create(CreateOrEditInfoSeedServiceDto input)
        {
            var infoSeedService = ObjectMapper.Map<InfoSeedService>(input);

            await _infoSeedServiceRepository.InsertAsync(infoSeedService);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices_Edit)]
        protected virtual async Task Update(CreateOrEditInfoSeedServiceDto input)
        {
            var infoSeedService = await _infoSeedServiceRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, infoSeedService);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _infoSeedServiceRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetInfoSeedServicesToExcel(GetAllInfoSeedServicesForExcelInput input)
        {

            var filteredInfoSeedServices = _infoSeedServiceRepository.GetAll()
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.ServiceStatusFk)
                        .Include(e => e.ServiceFrquencyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ServiceID.Contains(input.Filter) || e.ServiceName.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceIDFilter), e => e.ServiceID == input.ServiceIDFilter)
                        .WhereIf(input.MinServiceFeesFilter != null, e => e.ServiceFees >= input.MinServiceFeesFilter)
                        .WhereIf(input.MaxServiceFeesFilter != null, e => e.ServiceFees <= input.MaxServiceFeesFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceNameFilter), e => e.ServiceName == input.ServiceNameFilter)
                        .WhereIf(input.MinServiceCreationDateFilter != null, e => e.ServiceCreationDate >= input.MinServiceCreationDateFilter)
                        .WhereIf(input.MaxServiceCreationDateFilter != null, e => e.ServiceCreationDate <= input.MaxServiceCreationDateFilter)
                        .WhereIf(input.MinServiceStoppingDateFilter != null, e => e.ServiceStoppingDate >= input.MinServiceStoppingDateFilter)
                        .WhereIf(input.MaxServiceStoppingDateFilter != null, e => e.ServiceStoppingDate <= input.MaxServiceStoppingDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceTypeServicetypeNameFilter), e => e.ServiceTypeFk != null && e.ServiceTypeFk.ServicetypeName == input.ServiceTypeServicetypeNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceStatusServiceStatusNameFilter), e => e.ServiceStatusFk != null && e.ServiceStatusFk.ServiceStatusName == input.ServiceStatusServiceStatusNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceFrquencyServiceFrequencyNameFilter), e => e.ServiceFrquencyFk != null && e.ServiceFrquencyFk.ServiceFrequencyName == input.ServiceFrquencyServiceFrequencyNameFilter);

            var query = (from o in filteredInfoSeedServices
                         join o1 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_serviceStatusRepository.GetAll() on o.ServiceStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_serviceFrquencyRepository.GetAll() on o.ServiceFrquencyId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetInfoSeedServiceForViewDto()
                         {
                             InfoSeedService = new InfoSeedServiceDto
                             {
                                 ServiceID = o.ServiceID,
                                 ServiceName = o.ServiceName,
                                 ServiceCreationDate = o.ServiceCreationDate,
                                 ServiceStoppingDate = o.ServiceStoppingDate,
                                 FeesForFirstOrder = o.FeesForFirstOrder,
                                 FirstNumberOfOrders = o.FirstNumberOfOrders,
                                 Id = o.Id
                             },
                             ServiceTypeServicetypeName = s1 == null || s1.ServicetypeName == null ? "" : s1.ServicetypeName.ToString(),
                             ServiceStatusServiceStatusName = s2 == null || s2.ServiceStatusName == null ? "" : s2.ServiceStatusName.ToString(),
                             ServiceFrquencyServiceFrequencyName = s3 == null || s3.ServiceFrequencyName == null ? "" : s3.ServiceFrequencyName.ToString()
                         });

            var infoSeedServiceListDtos = await query.ToListAsync();

            return _infoSeedServicesExcelExporter.ExportToFile(infoSeedServiceListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices)]
        public async Task<PagedResultDto<InfoSeedServiceServiceTypeLookupTableDto>> GetAllServiceTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_serviceTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServicetypeName != null && e.ServicetypeName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var serviceTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<InfoSeedServiceServiceTypeLookupTableDto>();
            foreach (var serviceType in serviceTypeList)
            {
                lookupTableDtoList.Add(new InfoSeedServiceServiceTypeLookupTableDto
                {
                    Id = serviceType.Id,
                    DisplayName = serviceType.ServicetypeName?.ToString()
                });
            }

            return new PagedResultDto<InfoSeedServiceServiceTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices)]
        public async Task<PagedResultDto<InfoSeedServiceServiceStatusLookupTableDto>> GetAllServiceStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_serviceStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServiceStatusName != null && e.ServiceStatusName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var serviceStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<InfoSeedServiceServiceStatusLookupTableDto>();
            foreach (var serviceStatus in serviceStatusList)
            {
                lookupTableDtoList.Add(new InfoSeedServiceServiceStatusLookupTableDto
                {
                    Id = serviceStatus.Id,
                    DisplayName = serviceStatus.ServiceStatusName?.ToString()
                });
            }

            return new PagedResultDto<InfoSeedServiceServiceStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_InfoSeedServices)]
        public async Task<PagedResultDto<InfoSeedServiceServiceFrquencyLookupTableDto>> GetAllServiceFrquencyForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_serviceFrquencyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServiceFrequencyName != null && e.ServiceFrequencyName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var serviceFrquencyList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<InfoSeedServiceServiceFrquencyLookupTableDto>();
            foreach (var serviceFrquency in serviceFrquencyList)
            {
                lookupTableDtoList.Add(new InfoSeedServiceServiceFrquencyLookupTableDto
                {
                    Id = serviceFrquency.Id,
                    DisplayName = serviceFrquency.ServiceFrequencyName?.ToString()
                });
            }

            return new PagedResultDto<InfoSeedServiceServiceFrquencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}
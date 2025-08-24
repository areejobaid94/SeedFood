using Infoseed.MessagingPortal.InfoSeedServices;
using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.Currencies;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.AccountBillings.Exporting;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.BackgroundJobs;

namespace Infoseed.MessagingPortal.AccountBillings
{
   // [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
    public class HostAccountBillingsAppService : MessagingPortalAppServiceBase, IAccountBillingsAppService
    {
        private readonly IRepository<AccountBilling> _accountBillingRepository;
        private readonly IAccountBillingsExcelExporter _accountBillingsExcelExporter;
        private readonly IRepository<InfoSeedService, int> _lookup_infoSeedServiceRepository;
        private readonly IRepository<ServiceType, int> _lookup_serviceTypeRepository;
        private readonly IRepository<Currency, int> _lookup_currencyRepository;

        public HostAccountBillingsAppService(IRepository<AccountBilling> accountBillingRepository, IAccountBillingsExcelExporter accountBillingsExcelExporter, IRepository<InfoSeedService, int> lookup_infoSeedServiceRepository, IRepository<ServiceType, int> lookup_serviceTypeRepository, IRepository<Currency, int> lookup_currencyRepository)
        {
            _accountBillingRepository = accountBillingRepository;
            _accountBillingsExcelExporter = accountBillingsExcelExporter;
            _lookup_infoSeedServiceRepository = lookup_infoSeedServiceRepository;
            _lookup_serviceTypeRepository = lookup_serviceTypeRepository;
            _lookup_currencyRepository = lookup_currencyRepository;

        }

        public async Task<PagedResultDto<GetAccountBillingForViewDto>> GetAll(GetAllAccountBillingsInput input)
        {
           // var test = _accountBillingRepository.GetAll().ToList();
            var tenantId = AbpSession.TenantId;
            var filteredAccountBillings = _accountBillingRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.CurrencyFk)
                        .Where(e=>e.TenantId == input.TenantId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillIDFilter), e => e.BillID == input.BillIDFilter)
                        .WhereIf(input.MinBillDateFromFilter != null, e => e.BillDateFrom >= input.MinBillDateFromFilter)
                        .WhereIf(input.MaxBillDateFromFilter != null, e => e.BillDateFrom <= input.MaxBillDateFromFilter)
                        .WhereIf(input.MinBillDateToFilter != null, e => e.BillDateTo >= input.MinBillDateToFilter)
                        .WhereIf(input.MaxBillDateToFilter != null, e => e.BillDateTo <= input.MaxBillDateToFilter)
                        .WhereIf(input.MinOpenAmountFilter != null, e => e.OpenAmount >= input.MinOpenAmountFilter)
                        .WhereIf(input.MaxOpenAmountFilter != null, e => e.OpenAmount <= input.MaxOpenAmountFilter)
                        .WhereIf(input.MinBillAmountFilter != null, e => e.BillAmount >= input.MinBillAmountFilter)
                        .WhereIf(input.MaxBillAmountFilter != null, e => e.BillAmount <= input.MaxBillAmountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InfoSeedServiceServiceNameFilter), e => e.InfoSeedServiceFk != null && e.InfoSeedServiceFk.ServiceName == input.InfoSeedServiceServiceNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceTypeServicetypeNameFilter), e => e.ServiceTypeFk != null && e.ServiceTypeFk.ServicetypeName == input.ServiceTypeServicetypeNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.CurrencyName == input.CurrencyCurrencyNameFilter);

            var pagedAndFilteredAccountBillings = filteredAccountBillings
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var accountBillings = from o in pagedAndFilteredAccountBillings
                                  join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  join o2 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o2.Id into j2
                                  from s2 in j2.DefaultIfEmpty()

                                  join o3 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o3.Id into j3
                                  from s3 in j3.DefaultIfEmpty()

                                  select new GetAccountBillingForViewDto()
                                  {
                                      AccountBilling = new AccountBillingDto
                                      {
                                          BillID = o.BillID,
                                          BillDateFrom = o.BillDateFrom,
                                          BillDateTo = o.BillDateTo,
                                          OpenAmount = o.OpenAmount,
                                          BillAmount = o.BillAmount,
                                           Qty=o.Qty,
                                          Id = o.Id,
                                          TenantId = o.TenantId 
                                      },
                                      InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString(),
                                      ServiceTypeServicetypeName = s2 == null || s2.ServicetypeName == null ? "" : s2.ServicetypeName.ToString(),
                                      CurrencyCurrencyName = s3 == null || s3.CurrencyName == null ? "" : s3.CurrencyName.ToString()
                                  };

            var totalCount = await filteredAccountBillings.CountAsync();

            var result = new PagedResultDto<GetAccountBillingForViewDto>(
                totalCount,
                await accountBillings.ToListAsync()
            );

            return result;
            
        }

        public async Task<GetAccountBillingForViewDto> GetAccountBillingForView(int id)
        {
            var accountBilling = await _accountBillingRepository.GetAsync(id);

            var output = new GetAccountBillingForViewDto { AccountBilling = ObjectMapper.Map<AccountBillingDto>(accountBilling) };

            if (output.AccountBilling.InfoSeedServiceId != null)
            {
                var _lookupInfoSeedService = await _lookup_infoSeedServiceRepository.FirstOrDefaultAsync((int)output.AccountBilling.InfoSeedServiceId);
                output.InfoSeedServiceServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
            }

            if (output.AccountBilling.ServiceTypeId != null)
            {
                var _lookupServiceType = await _lookup_serviceTypeRepository.FirstOrDefaultAsync((int)output.AccountBilling.ServiceTypeId);
                output.ServiceTypeServicetypeName = _lookupServiceType?.ServicetypeName?.ToString();
            }

            if (output.AccountBilling.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.AccountBilling.CurrencyId);
                output.CurrencyCurrencyName = _lookupCurrency?.CurrencyName?.ToString();
            }

            return output;
        }

     //   [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Edit)]
        public async Task<GetAccountBillingForEditOutput> GetAccountBillingForEdit(EntityDto input)
        {
            var accountBilling = await _accountBillingRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetAccountBillingForEditOutput { AccountBilling = ObjectMapper.Map<CreateOrEditAccountBillingDto>(accountBilling) };

            if (output.AccountBilling.InfoSeedServiceId != null)
            {
                var _lookupInfoSeedService = await _lookup_infoSeedServiceRepository.FirstOrDefaultAsync((int)output.AccountBilling.InfoSeedServiceId);
                output.InfoSeedServiceServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
            }

            if (output.AccountBilling.ServiceTypeId != null)
            {
                var _lookupServiceType = await _lookup_serviceTypeRepository.FirstOrDefaultAsync((int)output.AccountBilling.ServiceTypeId);
                output.ServiceTypeServicetypeName = _lookupServiceType?.ServicetypeName?.ToString();
            }

            if (output.AccountBilling.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.AccountBilling.CurrencyId);
                output.CurrencyCurrencyName = _lookupCurrency?.CurrencyName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditAccountBillingDto input)
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

       // [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Create)]
        protected virtual async Task Create(CreateOrEditAccountBillingDto input)
        {
            var accountBilling = ObjectMapper.Map<AccountBilling>(input);

            if (AbpSession.TenantId != null)
            {
                accountBilling.TenantId = (int?)AbpSession.TenantId;
            }

            await _accountBillingRepository.InsertAsync(accountBilling);
        }

       // [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Edit)]
        protected virtual async Task Update(CreateOrEditAccountBillingDto input)
        {
            var accountBilling = await _accountBillingRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, accountBilling);
        }

     //   [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _accountBillingRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetAccountBillingsToExcel(GetAllAccountBillingsForExcelInput input)
        {

            var filteredAccountBillings = _accountBillingRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.CurrencyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillIDFilter), e => e.BillID == input.BillIDFilter)
                        .WhereIf(input.MinBillDateFromFilter != null, e => e.BillDateFrom >= input.MinBillDateFromFilter)
                        .WhereIf(input.MaxBillDateFromFilter != null, e => e.BillDateFrom <= input.MaxBillDateFromFilter)
                        .WhereIf(input.MinBillDateToFilter != null, e => e.BillDateTo >= input.MinBillDateToFilter)
                        .WhereIf(input.MaxBillDateToFilter != null, e => e.BillDateTo <= input.MaxBillDateToFilter)
                        .WhereIf(input.MinOpenAmountFilter != null, e => e.OpenAmount >= input.MinOpenAmountFilter)
                        .WhereIf(input.MaxOpenAmountFilter != null, e => e.OpenAmount <= input.MaxOpenAmountFilter)
                        .WhereIf(input.MinBillAmountFilter != null, e => e.BillAmount >= input.MinBillAmountFilter)
                        .WhereIf(input.MaxBillAmountFilter != null, e => e.BillAmount <= input.MaxBillAmountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InfoSeedServiceServiceNameFilter), e => e.InfoSeedServiceFk != null && e.InfoSeedServiceFk.ServiceName == input.InfoSeedServiceServiceNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceTypeServicetypeNameFilter), e => e.ServiceTypeFk != null && e.ServiceTypeFk.ServicetypeName == input.ServiceTypeServicetypeNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.CurrencyName == input.CurrencyCurrencyNameFilter);

            var query = (from o in filteredAccountBillings
                         join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetAccountBillingForViewDto()
                         {
                             AccountBilling = new AccountBillingDto
                             {
                                 BillID = o.BillID,
                                 BillDateFrom = o.BillDateFrom,
                                 BillDateTo = o.BillDateTo,
                                 OpenAmount = o.OpenAmount,
                                 BillAmount = o.BillAmount,
                                  Qty=o.Qty,
                                 Id = o.Id
                             },
                             InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString(),
                             ServiceTypeServicetypeName = s2 == null || s2.ServicetypeName == null ? "" : s2.ServicetypeName.ToString(),
                             CurrencyCurrencyName = s3 == null || s3.CurrencyName == null ? "" : s3.CurrencyName.ToString()
                         });

            var accountBillingListDtos = await query.ToListAsync();

            return _accountBillingsExcelExporter.ExportToFile(accountBillingListDtos);
        }

     //   [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingInfoSeedServiceLookupTableDto>> GetAllInfoSeedServiceForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_infoSeedServiceRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServiceName != null && e.ServiceName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var infoSeedServiceList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AccountBillingInfoSeedServiceLookupTableDto>();
            foreach (var infoSeedService in infoSeedServiceList)
            {
                lookupTableDtoList.Add(new AccountBillingInfoSeedServiceLookupTableDto
                {
                    Id = infoSeedService.Id,
                    DisplayName = infoSeedService.ServiceName?.ToString()
                });
            }

            return new PagedResultDto<AccountBillingInfoSeedServiceLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

     //   [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingServiceTypeLookupTableDto>> GetAllServiceTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_serviceTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ServicetypeName != null && e.ServicetypeName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var serviceTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AccountBillingServiceTypeLookupTableDto>();
            foreach (var serviceType in serviceTypeList)
            {
                lookupTableDtoList.Add(new AccountBillingServiceTypeLookupTableDto
                {
                    Id = serviceType.Id,
                    DisplayName = serviceType.ServicetypeName?.ToString()
                });
            }

            return new PagedResultDto<AccountBillingServiceTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

       // [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_currencyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.CurrencyName != null && e.CurrencyName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AccountBillingCurrencyLookupTableDto>();
            foreach (var currency in currencyList)
            {
                lookupTableDtoList.Add(new AccountBillingCurrencyLookupTableDto
                {
                    Id = currency.Id,
                    DisplayName = currency.CurrencyName?.ToString()
                });
            }

            return new PagedResultDto<AccountBillingCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<GetAccountBillingForViewDto>> GetAllBillingAccountPerTenant(GetAllAccountBillingsInput input)
        {
            var test = _accountBillingRepository.GetAllList();
            var tenantId = AbpSession.TenantId;
            var filteredAccountBillings = _accountBillingRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.CurrencyFk);
                      
                      
         

            var accountBillings = from o in filteredAccountBillings
                                  join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                                  from s1 in j1.DefaultIfEmpty()

                                  join o2 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o2.Id into j2
                                  from s2 in j2.DefaultIfEmpty()

                                  join o3 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o3.Id into j3
                                  from s3 in j3.DefaultIfEmpty()

                                  select new GetAccountBillingForViewDto()
                                  {
                                      AccountBilling = new AccountBillingDto
                                      {
                                          BillID = o.BillID,
                                          BillDateFrom = o.BillDateFrom,
                                          BillDateTo = o.BillDateTo,
                                          OpenAmount = o.OpenAmount,
                                          BillAmount = o.BillAmount,
                                           Qty=o.Qty,
                                          Id = o.Id,
                                          TenantId = o.TenantId
                                      },
                                      InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString(),
                                      ServiceTypeServicetypeName = s2 == null || s2.ServicetypeName == null ? "" : s2.ServicetypeName.ToString(),
                                      CurrencyCurrencyName = s3 == null || s3.CurrencyName == null ? "" : s3.CurrencyName.ToString()
                                  };

            var totalCount = await filteredAccountBillings.CountAsync();

            var result = new PagedResultDto<GetAccountBillingForViewDto>(
                totalCount,
                await accountBillings.ToListAsync()
            );

            return result;

        }

        Task<List<GetAccountBillingForViewDto>> IAccountBillingsAppService.GetAccountBillingForView(int id)
        {
            throw new NotImplementedException();
        }
    }
}
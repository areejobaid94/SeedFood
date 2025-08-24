using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.AccountBillings.Exporting;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Billings;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.InfoSeedServices;
using Infoseed.MessagingPortal.ServiceTypes;
using Infoseed.MessagingPortal.TenantServices;
using Infoseed.MessagingPortal.TenantServices.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.AccountBillings
{
    //[AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
    public class AccountBillingsAppService : MessagingPortalAppServiceBase, IAccountBillingsAppService
    {
        private readonly IRepository<TenantService> _tenantServiceRepository;
        private readonly IRepository<AccountBilling> _accountBillingRepository;
        private readonly IAccountBillingsExcelExporter _accountBillingsExcelExporter;
        private readonly IRepository<InfoSeedService, int> _lookup_infoSeedServiceRepository;
        private readonly IRepository<ServiceType, int> _lookup_serviceTypeRepository;
        private readonly IRepository<Currency, int> _lookup_currencyRepository;
        private readonly IRepository<Billing, int> _lookup_billingRepository;

        public AccountBillingsAppService(IRepository<AccountBilling> accountBillingRepository, IAccountBillingsExcelExporter accountBillingsExcelExporter, IRepository<InfoSeedService, int> lookup_infoSeedServiceRepository, IRepository<ServiceType, int> lookup_serviceTypeRepository, IRepository<Currency, int> lookup_currencyRepository, IRepository<Billing, int> lookup_billingRepository, IRepository<TenantService> tenantServiceRepository)
        {
            _accountBillingRepository = accountBillingRepository;
            _accountBillingsExcelExporter = accountBillingsExcelExporter;
            _lookup_infoSeedServiceRepository = lookup_infoSeedServiceRepository;
            _lookup_serviceTypeRepository = lookup_serviceTypeRepository;
            _lookup_currencyRepository = lookup_currencyRepository;
            _lookup_billingRepository = lookup_billingRepository;
            _tenantServiceRepository = tenantServiceRepository;

        }

        public async Task<PagedResultDto<GetAccountBillingForViewDto>> GetAll(GetAllAccountBillingsInput input)
        {
            var test = _accountBillingRepository.GetAll().ToList();
            var tenantId = AbpSession.TenantId;
            if (!input.TenantId.HasValue)
            {
                tenantId = AbpSession.TenantId;
            }
            else
            {
                tenantId = input.TenantId.Value;
            }
            var filteredAccountBillings = _accountBillingRepository.GetAll()
                        .Include(e => e.InfoSeedServiceFk)
                        .Include(e => e.ServiceTypeFk)
                        .Include(e => e.CurrencyFk)
                        .Where(e => e.TenantId == tenantId)
                        //.Include(e => e.BillingFk)
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
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.BillingBillingIDFilter), e => e.BillingFk != null && e.BillingFk.BillingID == input.BillingBillingIDFilter);

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

                                  //join o4 in _lookup_billingRepository.GetAll() on o.BillingId equals o4.Id into j4
                                  //from s4 in j4.DefaultIfEmpty()

                                  select new GetAccountBillingForViewDto()
                                  {
                                      AccountBilling = new AccountBillingDto
                                      {
                                          BillID = o.BillID,
                                          BillDateFrom = o.BillDateFrom,
                                          BillDateTo = o.BillDateTo,
                                          OpenAmount = o.OpenAmount,
                                          BillAmount = o.BillAmount,
                                          Id = o.Id,
                                          TenantId = o.TenantId,
                                           Qty = o.Qty,
                                            BillingId = o.BillingId,
                                             

                                          //BillingId = o.BillingId
                                      },
                                      InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString(),
                                      ServiceTypeServicetypeName = s2 == null || s2.ServicetypeName == null ? "" : s2.ServicetypeName.ToString(),
                                      CurrencyCurrencyName = s3 == null || s3.CurrencyName == null ? "" : s3.CurrencyName.ToString(),
                                      //BillingBillingID = s4 == null || s4.BillingID == null ? "" : s4.BillingID.ToString()
                                  };

            var totalCount = await filteredAccountBillings.CountAsync();

            var result = new PagedResultDto<GetAccountBillingForViewDto>(
                 totalCount,
                 await accountBillings.ToListAsync()
             );

            return result;
        }



        public async Task<List<GetAccountBillingForViewDto>> GetAccountBillingForView(int id)
        {

            var accountBillinginfo = GetAccountBilling();
            var tenantServersList = _tenantServiceRepository.GetAll();

            var listinfo = accountBillinginfo.Where(x => x.BillingId == id).ToList();
            if (listinfo != null)
            {

               // var order = GetOrderList(listinfo.TenantId).Result;

               // var orderCount = order.Where(x => x.CreationTime.IsBetween(listinfo.BillDateFrom, listinfo.BillDateTo) && x.orderStatus!= OrderStatusEunm.Draft).ToList().Count();
                List<GetAccountBillingForViewDto> output = new List<GetAccountBillingForViewDto>();
                foreach (var item in listinfo)
                {


                    GetAccountBillingForViewDto o = new GetAccountBillingForViewDto();
                    TenantServiceDto tenantServiceDto = new TenantServiceDto();

                    var tenantServers = tenantServersList.Where(x=>x.InfoSeedServiceId== item.InfoSeedServiceId).FirstOrDefault();
                    if (tenantServers != null)
                    {
                        tenantServiceDto = new TenantServiceDto
                        {
                            FeesForFirstOrder = tenantServers.FeesForFirstOrder,
                            FirstNumberOfOrders = tenantServers.FirstNumberOfOrders,
                            InfoSeedServiceId = tenantServers.InfoSeedServiceId,
                            ServiceFees = tenantServers.ServiceFees,
                            Id = tenantServers.Id,

                        };

                    }
                    

                    o.AccountBilling =new AccountBillingDto
                    {
                        Id = item.Id,
                        BillAmount = item.BillAmount,
                        BillDateFrom = item.BillDateFrom,
                        BillDateTo = item.BillDateTo,
                        BillingId = item.BillingId,
                        BillID = item.BillID,
                        CurrencyId = item.CurrencyId,
                        InfoSeedServiceId = item.InfoSeedServiceId,
                        OpenAmount = item.OpenAmount,
                        Qty = item.Qty,
                        ServiceTypeId = item.ServiceTypeId,
                        TenantId = item.TenantId,

                    };


                    o.TenantServiceDto = tenantServiceDto;
                  //  o.TotalOrder = orderCount;


                    var _lookupInfoSeedService = await _lookup_infoSeedServiceRepository.FirstOrDefaultAsync((int)item.InfoSeedServiceId);
                    o.InfoSeedServiceServiceName = _lookupInfoSeedService?.ServiceName?.ToString();
                    o.IsFeesPerTransaction = _lookupInfoSeedService.IsFeesPerTransaction;

                    var _lookupServiceType = await _lookup_serviceTypeRepository.FirstOrDefaultAsync((int)item.ServiceTypeId);
                    o.ServiceTypeServicetypeName = _lookupServiceType?.ServicetypeName?.ToString();

                    var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)item.CurrencyId);
                    o.CurrencyCurrencyName = _lookupCurrency?.CurrencyName?.ToString();

                    var _lookupBilling = await _lookup_billingRepository.FirstOrDefaultAsync((int)item.BillingId);
                    o.BillingBillingID = _lookupBilling?.BillingID?.ToString();

                   

                    output.Add(o);

                }
                return output;
            }

            return null;
        }

      //  [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Edit)]
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

            if (output.AccountBilling.BillingId != null)
            {
                var _lookupBilling = await _lookup_billingRepository.FirstOrDefaultAsync((int)output.AccountBilling.BillingId);
                output.BillingBillingID = _lookupBilling?.BillingID?.ToString();
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

      //  [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Edit)]
        protected virtual async Task Update(CreateOrEditAccountBillingDto input)
        {
            var accountBilling = await _accountBillingRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, accountBilling);
        }

       // [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings_Delete)]
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
                        .Include(e => e.BillingFk)
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
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.CurrencyName == input.CurrencyCurrencyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingBillingIDFilter), e => e.BillingFk != null && e.BillingFk.BillingID == input.BillingBillingIDFilter);

            var query = (from o in filteredAccountBillings
                         join o1 in _lookup_infoSeedServiceRepository.GetAll() on o.InfoSeedServiceId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_serviceTypeRepository.GetAll() on o.ServiceTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_billingRepository.GetAll() on o.BillingId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetAccountBillingForViewDto()
                         {
                             AccountBilling = new AccountBillingDto
                             {
                                 BillID = o.BillID,
                                 BillDateFrom = o.BillDateFrom,
                                 BillDateTo = o.BillDateTo,
                                 OpenAmount = o.OpenAmount,
                                 BillAmount = o.BillAmount,
                                 Id = o.Id
                             },
                             InfoSeedServiceServiceName = s1 == null || s1.ServiceName == null ? "" : s1.ServiceName.ToString(),
                             ServiceTypeServicetypeName = s2 == null || s2.ServicetypeName == null ? "" : s2.ServicetypeName.ToString(),
                             CurrencyCurrencyName = s3 == null || s3.CurrencyName == null ? "" : s3.CurrencyName.ToString(),
                             BillingBillingID = s4 == null || s4.BillingID == null ? "" : s4.BillingID.ToString()
                         });

            var accountBillingListDtos = await query.ToListAsync();

            return _accountBillingsExcelExporter.ExportToFile(accountBillingListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingInfoSeedServiceLookupTableDto>> GetAllInfoSeedServiceForLookupTable(Dtos.GetAllForLookupTableInput input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingServiceTypeLookupTableDto>> GetAllServiceTypeForLookupTable(Dtos.GetAllForLookupTableInput input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(Dtos.GetAllForLookupTableInput input)
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

      //  [AbpAuthorize(AppPermissions.Pages_Administration_AccountBillings)]
        public async Task<PagedResultDto<AccountBillingBillingLookupTableDto>> GetAllBillingForLookupTable(Dtos.GetAllForLookupTableInput input)
        {
            var query = _lookup_billingRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.BillingID != null && e.BillingID.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var billingList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AccountBillingBillingLookupTableDto>();
            foreach (var billing in billingList)
            {
                lookupTableDtoList.Add(new AccountBillingBillingLookupTableDto
                {
                    Id = billing.Id,
                    DisplayName = billing.BillingID?.ToString()
                });
            }

            return new PagedResultDto<AccountBillingBillingLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        public List<AccountBilling> GetAccountBilling()
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[AccountBillings] where TenantID=" + AbpSession.TenantId;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<AccountBilling> accountBillings = new List<AccountBilling>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    var isdelete = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!isdelete)
                    {

                        accountBillings.Add(new AccountBilling
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            BillAmount = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["BillAmount"].ToString()),
                            BillDateFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillDateFrom"].ToString()),
                            BillDateTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillDateTo"].ToString()),
                            BillingId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BillingId"].ToString()),
                            //BillID = dataSet.Tables[0].Rows[i]["BillID"].ToString(),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            CurrencyId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CurrencyId"].ToString()),
                            InfoSeedServiceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InfoSeedServiceId"].ToString()),
                            IsDeleted = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            OpenAmount = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["OpenAmount"].ToString()),
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"].ToString()),
                            ServiceTypeId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ServiceTypeId"].ToString()),
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString())



                        });

                    }

                }

                conn.Close();
                da.Dispose();

                return accountBillings;
            }
            catch (Exception )
            {
                return null;
            }

          
        }

        public List<AccountBilling> GetAccountBillingForId(int TenantId)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[AccountBillings] where TenantID=" + TenantId;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<AccountBilling> accountBillings = new List<AccountBilling>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    var isdelete = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!isdelete)
                    {

                        accountBillings.Add(new AccountBilling
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                            BillAmount = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["BillAmount"].ToString()),
                            BillDateFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillDateFrom"].ToString()),
                            BillDateTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillDateTo"].ToString()),
                            BillingId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["BillingId"].ToString()),
                            //BillID = dataSet.Tables[0].Rows[i]["BillID"].ToString(),
                            CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            CurrencyId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CurrencyId"].ToString()),
                            InfoSeedServiceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["InfoSeedServiceId"].ToString()),
                            IsDeleted = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString()),
                            OpenAmount = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["OpenAmount"].ToString()),
                            Qty = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"].ToString()),
                            ServiceTypeId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ServiceTypeId"].ToString()),
                            TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString())



                        });

                    }

                }

                conn.Close();
                da.Dispose();

                return accountBillings;
            }
            catch (Exception )
            {
                return null;
            }


        }
    }
}
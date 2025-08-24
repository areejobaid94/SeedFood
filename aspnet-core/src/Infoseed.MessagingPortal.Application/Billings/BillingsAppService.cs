using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.AccountBillings;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.Billings.Exporting;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.TenantServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Billings
{
    [AbpAuthorize(AppPermissions.Pages_Billings)]
    public class BillingsAppService : MessagingPortalAppServiceBase, IBillingsAppService
    {
        private ITenantServicesAppService  _tenantServicesAppService;
        //private readonly IAccountBillingsAppService _accountBillingsAppService;
        private readonly IRepository<Billing> _billingRepository;
        private readonly IBillingsExcelExporter _billingsExcelExporter;
        private readonly IRepository<Currency, int> _lookup_currencyRepository;

        public BillingsAppService(IRepository<Billing> billingRepository, IBillingsExcelExporter billingsExcelExporter, IRepository<Currency, int> lookup_currencyRepository, ITenantServicesAppService tenantServicesAppService/*, IAccountBillingsAppService accountBillingsAppService*/)
        {
            _tenantServicesAppService = tenantServicesAppService;
            //_accountBillingsAppService = accountBillingsAppService;
            _billingRepository = billingRepository;
            _billingsExcelExporter = billingsExcelExporter;
            _lookup_currencyRepository = lookup_currencyRepository;

        
           

        }

        public async Task<PagedResultDto<GetBillingForViewDto>> GetAll(GetAllBillingsInput input)
        {
            try
            {
                // testAsync();

                var filteredBillings = _billingRepository.GetAll()
                            .Include(e => e.CurrencyFk)
                            //.Include(e => e.TenantServiceFk)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillingID.Contains(input.Filter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.BillingIDFilter), e => e.BillingID == input.BillingIDFilter)
                            .WhereIf(input.MinBillingDateFilter != null, e => e.BillingDate >= input.MinBillingDateFilter)
                            .WhereIf(input.MaxBillingDateFilter != null, e => e.BillingDate <= input.MaxBillingDateFilter)
                            .WhereIf(input.MinTotalAmountFilter != null, e => e.TotalAmount >= input.MinTotalAmountFilter)
                            .WhereIf(input.MaxTotalAmountFilter != null, e => e.TotalAmount <= input.MaxTotalAmountFilter)
                            .WhereIf(input.MinBillPeriodToFilter != null, e => e.BillPeriodTo >= input.MinBillPeriodToFilter)
                            .WhereIf(input.MaxBillPeriodToFilter != null, e => e.BillPeriodTo <= input.MaxBillPeriodToFilter)
                            .WhereIf(input.MinBillPeriodFromFilter != null, e => e.BillPeriodFrom >= input.MinBillPeriodFromFilter)
                            .WhereIf(input.MaxBillPeriodFromFilter != null, e => e.BillPeriodFrom <= input.MaxBillPeriodFromFilter)
                            .WhereIf(input.MinDueDateFilter != null, e => e.DueDate >= input.MinDueDateFilter)
                            .WhereIf(input.MaxDueDateFilter != null, e => e.DueDate <= input.MaxDueDateFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.CurrencyName == input.CurrencyCurrencyNameFilter);

                var pagedAndFilteredBillings = filteredBillings
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var billings = from o in pagedAndFilteredBillings
                               join o1 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               select new GetBillingForViewDto()
                               {
                                   Billing = new BillingDto
                                   {
                                       BillingID = o.BillingID,
                                       BillingDate = o.BillingDate,
                                       TotalAmount = o.TotalAmount,
                                       BillPeriodTo = o.BillPeriodTo,
                                       BillPeriodFrom = o.BillPeriodFrom,
                                       DueDate = o.DueDate,
                                       BillingResponse = o.BillingResponse,
                                       //TenantServiceId=o.TenantServiceId,
                                       IsPayed = o.IsPayed,
                                       Id = o.Id
                                   },
                                   CurrencyCurrencyName = s1 == null || s1.CurrencyName == null ? "" : s1.CurrencyName.ToString()
                               };




                var totalCount = await filteredBillings.CountAsync();


                return new PagedResultDto<GetBillingForViewDto>(
                    totalCount,
                    await billings.ToListAsync()
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public async Task<PagedResultDto<GetBillingForViewDto>> HostGetAll(int TenantId)
        {
            try
            {
                var billing = GetBillingHostForId(TenantId);


                var view = new List<GetBillingForViewDto>();

                foreach (var item in billing)
                {
                    view.Add(new GetBillingForViewDto
                    {

                        Billing = item,
                        CurrencyCurrencyName = "JD"
                    });

                }

                var totalCount = 10000;


                return new PagedResultDto<GetBillingForViewDto>(
                    totalCount,
                     view
                );

            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }
        private List<BillingDto> GetBillingHostForId(int TenantId)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;
                string query = "select * from [dbo].[Billings] where TenantId=" + TenantId;


                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create the DataSet 
                DataSet dataSet = new DataSet();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataSet);

                List<BillingDto> accountBillings = new List<BillingDto>();

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    var isdelete = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsDeleted"].ToString());

                    if (!isdelete)
                    {

                        accountBillings.Add(new BillingDto
                        {
                            Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                              TotalAmount = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["TotalAmount"].ToString()),
                             BillingDate = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillingDate"].ToString()),
                             BillingID = dataSet.Tables[0].Rows[i]["BillingID"].ToString(),
                             BillingResponse = dataSet.Tables[0].Rows[i]["BillingResponse"].ToString(),
                            //BillID = dataSet.Tables[0].Rows[i]["BillID"].ToString(),
                             BillPeriodFrom = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString()),
                            CurrencyId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["CurrencyId"].ToString()),
                             BillPeriodTo = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["BillPeriodTo"].ToString()),
                             DueDate = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["DueDate"].ToString()),
                             IsPayed = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["IsPayed"].ToString()),
                            // TenantServiceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Qty"].ToString()),
                           // ServiceTypeId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ServiceTypeId"].ToString()),
                         //   TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"].ToString())



                        });

                    }

                }

                conn.Close();
                da.Dispose();

                return accountBillings;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        async Task testAsync()
        {

            var filteredBillings = _billingRepository.GetAll().Where(x => x.BillingID == "12321").FirstOrDefault();
            if (filteredBillings != null)
                DeleteBilling(filteredBillings.Id);

            var list = _tenantServicesAppService.GetTenatServices((int)AbpSession.TenantId);

            decimal total = 0;
            DateTime? dateTime = DateTime.Now;
            foreach (var item in list)
            {
                if (item.IsSelected)
                {
                    dateTime = item.TenantServiceCreationTime;
                    total = total + decimal.ToInt32(item.Fees);
                    //dtos.Add(item);
                }

            }

            CreateOrEditBillingDto createOrEditBillingDto = new CreateOrEditBillingDto
            {
                TotalAmount = total,
                BillingDate = (DateTime)dateTime,
                BillingID = "12321",
                BillPeriodFrom = (DateTime)dateTime,
                BillPeriodTo = (DateTime)dateTime,
                DueDate = (DateTime)dateTime,
                
                CurrencyId = 1


            };
           await Create(createOrEditBillingDto);

        }

        private void DeleteBilling(int id)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = "DELETE FROM Billings Where Id = @Id";

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void CreateBillingAndServes(CreateOrEditBillingDto Billing)
        {

            CreateBilling(Billing);
        }

        public void CreateBilling(CreateOrEditBillingDto Billing)
        {
            try
            {
                decimal total = 0;
                foreach (var item in Billing.TenantService)
                {
                    if (item.IsSelected)
                    {

                        AccountBilling BillingModel = new AccountBilling();


                        total = total + (item.Fees * item.FeesForFirstOrder);


                    }



                }

                /////////
                int modified = 0;
                var billing = ObjectMapper.Map<Billing>(Billing);
                if (AbpSession.TenantId != null)
                {
                    billing.TenantId = (int?)AbpSession.TenantId;
                }
                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "INSERT INTO Billings (TotalAmount,BillingDate,BillingID,BillPeriodFrom,BillPeriodTo,DueDate,CurrencyId,IsDeleted,TenantId,CreationTime) VALUES (@TotalAmount,@BillingDate,@BillingID, @BillPeriodFrom, @BillPeriodTo, @DueDate, @CurrencyId,@IsDeleted,@TenantId,@CreationTime);SELECT SCOPE_IDENTITY(); "; ;

                    command.Parameters.AddWithValue("@TotalAmount", total);
                    command.Parameters.AddWithValue("@BillingDate", billing.BillingDate);
                    command.Parameters.AddWithValue("@BillingID", "INV-" + billing.TenantId + "-" + billing.BillingDate.Month + "-" + billing.BillingDate.Year);
                    command.Parameters.AddWithValue("@BillPeriodFrom", billing.BillPeriodFrom);
                    command.Parameters.AddWithValue("@BillPeriodTo", billing.BillPeriodTo);
                    command.Parameters.AddWithValue("@DueDate", billing.DueDate);
                    command.Parameters.AddWithValue("@CurrencyId", billing.CurrencyId);
                    command.Parameters.AddWithValue("@IsDeleted", false);
                    command.Parameters.AddWithValue("@TenantId", billing.TenantId);
                    command.Parameters.AddWithValue("@CreationTime", billing.BillingDate);

                    connection.Open();
                    modified = Convert.ToInt32(command.ExecuteScalar());
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }


                foreach (var item in Billing.TenantService)
                {
                    if (item.IsSelected)
                    {

                        AccountBilling BillingModel = new AccountBilling();


                        BillingModel.TenantId = billing.TenantId;

                        BillingModel.BillDateFrom = billing.BillPeriodFrom;
                        BillingModel.BillDateTo = billing.BillPeriodTo;

                        BillingModel.OpenAmount = item.Fees * item.FeesForFirstOrder;
                        BillingModel.BillAmount = item.Fees * item.FeesForFirstOrder;
                        BillingModel.InfoSeedServiceId = item.ServiceId;
                        BillingModel.ServiceTypeId = 1;
                        BillingModel.CurrencyId = billing.CurrencyId;
                        BillingModel.BillingId = modified;
                        BillingModel.Qty = int.Parse(item.Fees.ToString());

                        CreateAcountBilling(BillingModel);
                    }



                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            


        }



        public void CreateAcountBilling(AccountBilling Billing)
        {
            try
            {
                int modified = 0;

                string connString = AppSettingsModel.ConnectionStrings;
                using (SqlConnection connection = new SqlConnection(connString))
                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "INSERT INTO AccountBillings " +
                        "(CreationTime,IsDeleted,TenantId,BillDateFrom,BillDateTo,OpenAmount,BillAmount,InfoSeedServiceId,ServiceTypeId,CurrencyId,BillingId,Qty) " +
                        "VALUES (@CreationTime,@IsDeleted,@TenantId, @BillDateFrom, @BillDateTo, @OpenAmount, @BillAmount,@InfoSeedServiceId,@ServiceTypeId,@CurrencyId,@BillingId,@Qty);SELECT SCOPE_IDENTITY(); "; ;

                    command.Parameters.AddWithValue("@CreationTime", Billing.CreationTime);
                    command.Parameters.AddWithValue("@IsDeleted", false);
                    command.Parameters.AddWithValue("@TenantId", Billing.TenantId);

                    command.Parameters.AddWithValue("@BillDateFrom", Billing.BillDateFrom);
                    command.Parameters.AddWithValue("@BillDateTo", Billing.BillDateTo);
                    command.Parameters.AddWithValue("@OpenAmount", Billing.OpenAmount);
                    command.Parameters.AddWithValue("@BillAmount", Billing.BillAmount);
                    command.Parameters.AddWithValue("@InfoSeedServiceId", Billing.InfoSeedServiceId);
                    command.Parameters.AddWithValue("@ServiceTypeId", Billing.ServiceTypeId);
                    command.Parameters.AddWithValue("@CurrencyId", Billing.CurrencyId);

                    command.Parameters.AddWithValue("@BillingId", Billing.BillingId);
                    command.Parameters.AddWithValue("@Qty", Billing.Qty);

                    connection.Open();
                    modified = Convert.ToInt32(command.ExecuteScalar());
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            



        }

        public async Task<GetBillingForViewDto> GetBillingForView(int id)
        {
            try
            {
                var billing = await _billingRepository.GetAsync(id);

                var output = new GetBillingForViewDto { Billing = ObjectMapper.Map<BillingDto>(billing) };

                //if (output.Billing.CurrencyId != null)
                //{
                    var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Billing.CurrencyId);
                    output.CurrencyCurrencyName = _lookupCurrency?.CurrencyName?.ToString();
                //}

                //var idA= _accountBillingsAppService.GetAccountBillingForView(output.Billing.Id);

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        [AbpAuthorize(AppPermissions.Pages_Billings_Edit)]
        public async Task<GetBillingForEditOutput> GetBillingForEdit(EntityDto input)
        {
            try
            {
                var billing = await _billingRepository.FirstOrDefaultAsync(input.Id);

                var output = new GetBillingForEditOutput { Billing = ObjectMapper.Map<CreateOrEditBillingDto>(billing) };
                if (output.Billing == null)
                {

                    return null;
                }
                //if (output.Billing.CurrencyId != null)
                //{
                    var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Billing.CurrencyId);
                    output.CurrencyCurrencyName = _lookupCurrency?.CurrencyName?.ToString();
                //}

                return output;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task CreateOrEdit(CreateOrEditBillingDto input)
        {
            try
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
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        //[AbpAuthorize(AppPermissions.Pages_Billings_Create)]
        protected virtual async Task Create(CreateOrEditBillingDto input)
        {
            try
            {
                var billing = ObjectMapper.Map<Billing>(input);

                if (AbpSession.TenantId != null)
                {
                    billing.TenantId = (int?)AbpSession.TenantId;
                }

                await _billingRepository.InsertAsync(billing);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        //[AbpAuthorize(AppPermissions.Pages_Billings_Edit)]
        protected virtual async Task Update(CreateOrEditBillingDto input)
        {
            try
            {
                var billing = await _billingRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, billing);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Billings_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _billingRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetBillingsToExcel(GetAllBillingsForExcelInput input)
        {
            try
            {
                var filteredBillings = _billingRepository.GetAll()
                        .Include(e => e.CurrencyFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillingID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingIDFilter), e => e.BillingID == input.BillingIDFilter)
                        .WhereIf(input.MinBillingDateFilter != null, e => e.BillingDate >= input.MinBillingDateFilter)
                        .WhereIf(input.MaxBillingDateFilter != null, e => e.BillingDate <= input.MaxBillingDateFilter)
                        .WhereIf(input.MinTotalAmountFilter != null, e => e.TotalAmount >= input.MinTotalAmountFilter)
                        .WhereIf(input.MaxTotalAmountFilter != null, e => e.TotalAmount <= input.MaxTotalAmountFilter)
                        .WhereIf(input.MinBillPeriodToFilter != null, e => e.BillPeriodTo >= input.MinBillPeriodToFilter)
                        .WhereIf(input.MaxBillPeriodToFilter != null, e => e.BillPeriodTo <= input.MaxBillPeriodToFilter)
                        .WhereIf(input.MinBillPeriodFromFilter != null, e => e.BillPeriodFrom >= input.MinBillPeriodFromFilter)
                        .WhereIf(input.MaxBillPeriodFromFilter != null, e => e.BillPeriodFrom <= input.MaxBillPeriodFromFilter)
                        .WhereIf(input.MinDueDateFilter != null, e => e.DueDate >= input.MinDueDateFilter)
                        .WhereIf(input.MaxDueDateFilter != null, e => e.DueDate <= input.MaxDueDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.CurrencyName == input.CurrencyCurrencyNameFilter);

                var query = (from o in filteredBillings
                             join o1 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             select new GetBillingForViewDto()
                             {
                                 Billing = new BillingDto
                                 {
                                     BillingID = o.BillingID,
                                     BillingDate = o.BillingDate,
                                     TotalAmount = o.TotalAmount,
                                     BillPeriodTo = o.BillPeriodTo,
                                     BillPeriodFrom = o.BillPeriodFrom,
                                     DueDate = o.DueDate,
                                     Id = o.Id
                                 },
                                 CurrencyCurrencyName = s1 == null || s1.CurrencyName == null ? "" : s1.CurrencyName.ToString()
                             });

                var billingListDtos = await query.ToListAsync();

                return _billingsExcelExporter.ExportToFile(billingListDtos);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        [AbpAuthorize(AppPermissions.Pages_Billings)]
        public async Task<PagedResultDto<BillingCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
        {
            try
            {
                var query = _lookup_currencyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.CurrencyName != null && e.CurrencyName.Contains(input.Filter)
               );

                var totalCount = await query.CountAsync();

                var currencyList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<BillingCurrencyLookupTableDto>();
                foreach (var currency in currencyList)
                {
                    lookupTableDtoList.Add(new BillingCurrencyLookupTableDto
                    {
                        Id = currency.Id,
                        DisplayName = currency.CurrencyName?.ToString()
                    });
                }

                return new PagedResultDto<BillingCurrencyLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }


    }
}
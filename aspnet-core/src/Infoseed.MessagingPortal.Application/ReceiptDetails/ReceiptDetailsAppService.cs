using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.AccountBillings;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.ReceiptDetails.Dtos;
using Infoseed.MessagingPortal.ReceiptDetails.Exporting;
using Infoseed.MessagingPortal.Receipts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ReceiptDetails
{
    [AbpAuthorize(AppPermissions.Pages_ReceiptDetails)]
    public class ReceiptDetailsAppService : MessagingPortalAppServiceBase, IReceiptDetailsAppService
    {
        private readonly IRepository<ReceiptDetail> _receiptDetailRepository;
        private readonly IReceiptDetailsExcelExporter _receiptDetailsExcelExporter;
        private readonly IRepository<Receipt, int> _lookup_receiptRepository;
        private readonly IRepository<AccountBilling, int> _lookup_accountBillingRepository;

        public ReceiptDetailsAppService(IRepository<ReceiptDetail> receiptDetailRepository, IReceiptDetailsExcelExporter receiptDetailsExcelExporter, IRepository<Receipt, int> lookup_receiptRepository, IRepository<AccountBilling, int> lookup_accountBillingRepository)
        {
            _receiptDetailRepository = receiptDetailRepository;
            _receiptDetailsExcelExporter = receiptDetailsExcelExporter;
            _lookup_receiptRepository = lookup_receiptRepository;
            _lookup_accountBillingRepository = lookup_accountBillingRepository;

        }

        public async Task<PagedResultDto<GetReceiptDetailForViewDto>> GetAll(GetAllReceiptDetailsInput input)
        {

            var filteredReceiptDetails = _receiptDetailRepository.GetAll()
                        .Include(e => e.ReceiptFk)
                        .Include(e => e.AccountBillingFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillingNumber.Contains(input.Filter) || e.ServiceName.Contains(input.Filter) || e.CurrencyName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingNumberFilter), e => e.BillingNumber == input.BillingNumberFilter)
                        .WhereIf(input.MinBillDateFromFilter != null, e => e.BillDateFrom >= input.MinBillDateFromFilter)
                        .WhereIf(input.MaxBillDateFromFilter != null, e => e.BillDateFrom <= input.MaxBillDateFromFilter)
                        .WhereIf(input.MinBillDateToFilter != null, e => e.BillDateTo >= input.MinBillDateToFilter)
                        .WhereIf(input.MaxBillDateToFilter != null, e => e.BillDateTo <= input.MaxBillDateToFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceNameFilter), e => e.ServiceName == input.ServiceNameFilter)
                        .WhereIf(input.MinBillAmountFilter != null, e => e.BillAmount >= input.MinBillAmountFilter)
                        .WhereIf(input.MaxBillAmountFilter != null, e => e.BillAmount <= input.MaxBillAmountFilter)
                        .WhereIf(input.MinOpenAmountFilter != null, e => e.OpenAmount >= input.MinOpenAmountFilter)
                        .WhereIf(input.MaxOpenAmountFilter != null, e => e.OpenAmount <= input.MaxOpenAmountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyName == input.CurrencyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReceiptReceiptNumberFilter), e => e.ReceiptFk != null && e.ReceiptFk.ReceiptNumber == input.ReceiptReceiptNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AccountBillingBillIDFilter), e => e.AccountBillingFk != null && e.AccountBillingFk.BillID == input.AccountBillingBillIDFilter)
                        .WhereIf(input.ReceiptIdFilter.HasValue, e => false || e.ReceiptId == input.ReceiptIdFilter.Value);

            var pagedAndFilteredReceiptDetails = filteredReceiptDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var receiptDetails = from o in pagedAndFilteredReceiptDetails
                                 join o1 in _lookup_receiptRepository.GetAll() on o.ReceiptId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_accountBillingRepository.GetAll() on o.AccountBillingId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 select new GetReceiptDetailForViewDto()
                                 {
                                     ReceiptDetail = new ReceiptDetailDto
                                     {
                                         BillingNumber = o.BillingNumber,
                                         BillDateFrom = o.BillDateFrom,
                                         BillDateTo = o.BillDateTo,
                                         ServiceName = o.ServiceName,
                                         BillAmount = o.BillAmount,
                                         OpenAmount = o.OpenAmount,
                                         CurrencyName = o.CurrencyName,
                                         Id = o.Id
                                     },
                                     ReceiptReceiptNumber = s1 == null || s1.ReceiptNumber == null ? "" : s1.ReceiptNumber.ToString(),
                                     AccountBillingBillID = s2 == null || s2.BillID == null ? "" : s2.BillID.ToString()
                                 };

            var totalCount = await filteredReceiptDetails.CountAsync();

            return new PagedResultDto<GetReceiptDetailForViewDto>(
                totalCount,
                await receiptDetails.ToListAsync()
            );
        }

        public async Task<GetReceiptDetailForViewDto> GetReceiptDetailForView(int id)
        {
            var receiptDetail = await _receiptDetailRepository.GetAsync(id);

            var output = new GetReceiptDetailForViewDto { ReceiptDetail = ObjectMapper.Map<ReceiptDetailDto>(receiptDetail) };

            //if (output.ReceiptDetail.ReceiptId != null)
            //{
                var _lookupReceipt = await _lookup_receiptRepository.FirstOrDefaultAsync((int)output.ReceiptDetail.ReceiptId);
                output.ReceiptReceiptNumber = _lookupReceipt?.ReceiptNumber?.ToString();
            //}

            if (output.ReceiptDetail.AccountBillingId != null)
            {
                var _lookupAccountBilling = await _lookup_accountBillingRepository.FirstOrDefaultAsync((int)output.ReceiptDetail.AccountBillingId);
                output.AccountBillingBillID = _lookupAccountBilling?.BillID?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails_Edit)]
        public async Task<GetReceiptDetailForEditOutput> GetReceiptDetailForEdit(EntityDto input)
        {
            var receiptDetail = await _receiptDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetReceiptDetailForEditOutput { ReceiptDetail = ObjectMapper.Map<CreateOrEditReceiptDetailDto>(receiptDetail) };

            //if (output.ReceiptDetail.ReceiptId != null)
            //{
                var _lookupReceipt = await _lookup_receiptRepository.FirstOrDefaultAsync((int)output.ReceiptDetail.ReceiptId);
                output.ReceiptReceiptNumber = _lookupReceipt?.ReceiptNumber?.ToString();
            //}

            if (output.ReceiptDetail.AccountBillingId != null)
            {
                var _lookupAccountBilling = await _lookup_accountBillingRepository.FirstOrDefaultAsync((int)output.ReceiptDetail.AccountBillingId);
                output.AccountBillingBillID = _lookupAccountBilling?.BillID?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditReceiptDetailDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails_Create)]
        protected virtual async Task Create(CreateOrEditReceiptDetailDto input)
        {
            var receiptDetail = ObjectMapper.Map<ReceiptDetail>(input);

            await _receiptDetailRepository.InsertAsync(receiptDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails_Edit)]
        protected virtual async Task Update(CreateOrEditReceiptDetailDto input)
        {
            var receiptDetail = await _receiptDetailRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, receiptDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _receiptDetailRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetReceiptDetailsToExcel(GetAllReceiptDetailsForExcelInput input)
        {

            var filteredReceiptDetails = _receiptDetailRepository.GetAll()
                        .Include(e => e.ReceiptFk)
                        .Include(e => e.AccountBillingFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.BillingNumber.Contains(input.Filter) || e.ServiceName.Contains(input.Filter) || e.CurrencyName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingNumberFilter), e => e.BillingNumber == input.BillingNumberFilter)
                        .WhereIf(input.MinBillDateFromFilter != null, e => e.BillDateFrom >= input.MinBillDateFromFilter)
                        .WhereIf(input.MaxBillDateFromFilter != null, e => e.BillDateFrom <= input.MaxBillDateFromFilter)
                        .WhereIf(input.MinBillDateToFilter != null, e => e.BillDateTo >= input.MinBillDateToFilter)
                        .WhereIf(input.MaxBillDateToFilter != null, e => e.BillDateTo <= input.MaxBillDateToFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ServiceNameFilter), e => e.ServiceName == input.ServiceNameFilter)
                        .WhereIf(input.MinBillAmountFilter != null, e => e.BillAmount >= input.MinBillAmountFilter)
                        .WhereIf(input.MaxBillAmountFilter != null, e => e.BillAmount <= input.MaxBillAmountFilter)
                        .WhereIf(input.MinOpenAmountFilter != null, e => e.OpenAmount >= input.MinOpenAmountFilter)
                        .WhereIf(input.MaxOpenAmountFilter != null, e => e.OpenAmount <= input.MaxOpenAmountFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyName == input.CurrencyNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReceiptReceiptNumberFilter), e => e.ReceiptFk != null && e.ReceiptFk.ReceiptNumber == input.ReceiptReceiptNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AccountBillingBillIDFilter), e => e.AccountBillingFk != null && e.AccountBillingFk.BillID == input.AccountBillingBillIDFilter);

            var query = (from o in filteredReceiptDetails
                         join o1 in _lookup_receiptRepository.GetAll() on o.ReceiptId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_accountBillingRepository.GetAll() on o.AccountBillingId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetReceiptDetailForViewDto()
                         {
                             ReceiptDetail = new ReceiptDetailDto
                             {
                                 BillingNumber = o.BillingNumber,
                                 BillDateFrom = o.BillDateFrom,
                                 BillDateTo = o.BillDateTo,
                                 ServiceName = o.ServiceName,
                                 BillAmount = o.BillAmount,
                                 OpenAmount = o.OpenAmount,
                                 CurrencyName = o.CurrencyName,
                                 Id = o.Id
                             },
                             ReceiptReceiptNumber = s1 == null || s1.ReceiptNumber == null ? "" : s1.ReceiptNumber.ToString(),
                             AccountBillingBillID = s2 == null || s2.BillID == null ? "" : s2.BillID.ToString()
                         });

            var receiptDetailListDtos = await query.ToListAsync();

            return _receiptDetailsExcelExporter.ExportToFile(receiptDetailListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails)]
        public async Task<PagedResultDto<ReceiptDetailReceiptLookupTableDto>> GetAllReceiptForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_receiptRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ReceiptNumber != null && e.ReceiptNumber.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var receiptList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ReceiptDetailReceiptLookupTableDto>();
            foreach (var receipt in receiptList)
            {
                lookupTableDtoList.Add(new ReceiptDetailReceiptLookupTableDto
                {
                    Id = receipt.Id,
                    DisplayName = receipt.ReceiptNumber?.ToString()
                });
            }

            return new PagedResultDto<ReceiptDetailReceiptLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_ReceiptDetails)]
        public async Task<PagedResultDto<ReceiptDetailAccountBillingLookupTableDto>> GetAllAccountBillingForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_accountBillingRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.BillID != null && e.BillID.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var accountBillingList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ReceiptDetailAccountBillingLookupTableDto>();
            foreach (var accountBilling in accountBillingList)
            {
                lookupTableDtoList.Add(new ReceiptDetailAccountBillingLookupTableDto
                {
                    Id = accountBilling.Id,
                    DisplayName = accountBilling.BillID?.ToString()
                });
            }

            return new PagedResultDto<ReceiptDetailAccountBillingLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}
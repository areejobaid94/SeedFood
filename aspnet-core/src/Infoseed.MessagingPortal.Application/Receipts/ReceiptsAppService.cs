using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Banks;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.PaymentMethods;
using Infoseed.MessagingPortal.Receipts.Dtos;
using Infoseed.MessagingPortal.Receipts.Exporting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Receipts
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Receipts)]
    public class ReceiptsAppService : MessagingPortalAppServiceBase, IReceiptsAppService
    {
        private readonly IRepository<Receipt> _receiptRepository;
        private readonly IReceiptsExcelExporter _receiptsExcelExporter;
        private readonly IRepository<Bank, int> _lookup_bankRepository;
        private readonly IRepository<PaymentMethod, int> _lookup_paymentMethodRepository;

        public ReceiptsAppService(IRepository<Receipt> receiptRepository, IReceiptsExcelExporter receiptsExcelExporter, IRepository<Bank, int> lookup_bankRepository, IRepository<PaymentMethod, int> lookup_paymentMethodRepository)
        {
            _receiptRepository = receiptRepository;
            _receiptsExcelExporter = receiptsExcelExporter;
            _lookup_bankRepository = lookup_bankRepository;
            _lookup_paymentMethodRepository = lookup_paymentMethodRepository;

        }

        public async Task<PagedResultDto<GetReceiptForViewDto>> GetAll(GetAllReceiptsInput input)
        {

            var filteredReceipts = _receiptRepository.GetAll()
                        .Include(e => e.BankFk)
                        .Include(e => e.PaymentMethodFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ReceiptNumber.Contains(input.Filter) || e.PaymentReferenceNumber.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReceiptNumberFilter), e => e.ReceiptNumber == input.ReceiptNumberFilter)
                        .WhereIf(input.MinReceiptDateFilter != null, e => e.ReceiptDate >= input.MinReceiptDateFilter)
                        .WhereIf(input.MaxReceiptDateFilter != null, e => e.ReceiptDate <= input.MaxReceiptDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentReferenceNumberFilter), e => e.PaymentReferenceNumber == input.PaymentReferenceNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BankBankNameFilter), e => e.BankFk != null && e.BankFk.BankName == input.BankBankNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentMethodPaymnetMethodFilter), e => e.PaymentMethodFk != null && e.PaymentMethodFk.PaymnetMethod == input.PaymentMethodPaymnetMethodFilter);

            var pagedAndFilteredReceipts = filteredReceipts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var receipts = from o in pagedAndFilteredReceipts
                           join o1 in _lookup_bankRepository.GetAll() on o.BankId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           join o2 in _lookup_paymentMethodRepository.GetAll() on o.PaymentMethodId equals o2.Id into j2
                           from s2 in j2.DefaultIfEmpty()

                           select new GetReceiptForViewDto()
                           {
                               Receipt = new ReceiptDto
                               {
                                   ReceiptNumber = o.ReceiptNumber,
                                   ReceiptDate = o.ReceiptDate,
                                   PaymentReferenceNumber = o.PaymentReferenceNumber,
                                   Id = o.Id
                               },
                               BankBankName = s1 == null || s1.BankName == null ? "" : s1.BankName.ToString(),
                               PaymentMethodPaymnetMethod = s2 == null || s2.PaymnetMethod == null ? "" : s2.PaymnetMethod.ToString()
                           };

            var totalCount = await filteredReceipts.CountAsync();

            return new PagedResultDto<GetReceiptForViewDto>(
                totalCount,
                await receipts.ToListAsync()
            );
        }

        public async Task<GetReceiptForViewDto> GetReceiptForView(int id)
        {
            var receipt = await _receiptRepository.GetAsync(id);

            var output = new GetReceiptForViewDto { Receipt = ObjectMapper.Map<ReceiptDto>(receipt) };

            if (output.Receipt.BankId != null)
            {
                var _lookupBank = await _lookup_bankRepository.FirstOrDefaultAsync((int)output.Receipt.BankId);
                output.BankBankName = _lookupBank?.BankName?.ToString();
            }

            //if (output.Receipt.PaymentMethodId != null)
            //{
                var _lookupPaymentMethod = await _lookup_paymentMethodRepository.FirstOrDefaultAsync((int)output.Receipt.PaymentMethodId);
                output.PaymentMethodPaymnetMethod = _lookupPaymentMethod?.PaymnetMethod?.ToString();
            //}

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts_Edit)]
        public async Task<GetReceiptForEditOutput> GetReceiptForEdit(EntityDto input)
        {
            var receipt = await _receiptRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetReceiptForEditOutput { Receipt = ObjectMapper.Map<CreateOrEditReceiptDto>(receipt) };

            if (output.Receipt.BankId != null)
            {
                var _lookupBank = await _lookup_bankRepository.FirstOrDefaultAsync((int)output.Receipt.BankId);
                output.BankBankName = _lookupBank?.BankName?.ToString();
            }

            //if (output.Receipt.PaymentMethodId != null)
            //{
                var _lookupPaymentMethod = await _lookup_paymentMethodRepository.FirstOrDefaultAsync((int)output.Receipt.PaymentMethodId);
                output.PaymentMethodPaymnetMethod = _lookupPaymentMethod?.PaymnetMethod?.ToString();
            //}

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditReceiptDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts_Create)]
        protected virtual async Task Create(CreateOrEditReceiptDto input)
        {
            var receipt = ObjectMapper.Map<Receipt>(input);

            if (AbpSession.TenantId != null)
            {
                receipt.TenantId = (int?)AbpSession.TenantId;
            }

            await _receiptRepository.InsertAsync(receipt);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts_Edit)]
        protected virtual async Task Update(CreateOrEditReceiptDto input)
        {
            var receipt = await _receiptRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, receipt);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _receiptRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetReceiptsToExcel(GetAllReceiptsForExcelInput input)
        {

            var filteredReceipts = _receiptRepository.GetAll()
                        .Include(e => e.BankFk)
                        .Include(e => e.PaymentMethodFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ReceiptNumber.Contains(input.Filter) || e.PaymentReferenceNumber.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReceiptNumberFilter), e => e.ReceiptNumber == input.ReceiptNumberFilter)
                        .WhereIf(input.MinReceiptDateFilter != null, e => e.ReceiptDate >= input.MinReceiptDateFilter)
                        .WhereIf(input.MaxReceiptDateFilter != null, e => e.ReceiptDate <= input.MaxReceiptDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentReferenceNumberFilter), e => e.PaymentReferenceNumber == input.PaymentReferenceNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BankBankNameFilter), e => e.BankFk != null && e.BankFk.BankName == input.BankBankNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PaymentMethodPaymnetMethodFilter), e => e.PaymentMethodFk != null && e.PaymentMethodFk.PaymnetMethod == input.PaymentMethodPaymnetMethodFilter);

            var query = (from o in filteredReceipts
                         join o1 in _lookup_bankRepository.GetAll() on o.BankId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_paymentMethodRepository.GetAll() on o.PaymentMethodId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetReceiptForViewDto()
                         {
                             Receipt = new ReceiptDto
                             {
                                 ReceiptNumber = o.ReceiptNumber,
                                 ReceiptDate = o.ReceiptDate,
                                 PaymentReferenceNumber = o.PaymentReferenceNumber,
                                 Id = o.Id
                             },
                             BankBankName = s1 == null || s1.BankName == null ? "" : s1.BankName.ToString(),
                             PaymentMethodPaymnetMethod = s2 == null || s2.PaymnetMethod == null ? "" : s2.PaymnetMethod.ToString()
                         });

            var receiptListDtos = await query.ToListAsync();

            return _receiptsExcelExporter.ExportToFile(receiptListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts)]
        public async Task<PagedResultDto<ReceiptBankLookupTableDto>> GetAllBankForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_bankRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.BankName != null && e.BankName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var bankList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ReceiptBankLookupTableDto>();
            foreach (var bank in bankList)
            {
                lookupTableDtoList.Add(new ReceiptBankLookupTableDto
                {
                    Id = bank.Id,
                    DisplayName = bank.BankName?.ToString()
                });
            }

            return new PagedResultDto<ReceiptBankLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Receipts)]
        public async Task<PagedResultDto<ReceiptPaymentMethodLookupTableDto>> GetAllPaymentMethodForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_paymentMethodRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.PaymnetMethod != null && e.PaymnetMethod.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var paymentMethodList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ReceiptPaymentMethodLookupTableDto>();
            foreach (var paymentMethod in paymentMethodList)
            {
                lookupTableDtoList.Add(new ReceiptPaymentMethodLookupTableDto
                {
                    Id = paymentMethod.Id,
                    DisplayName = paymentMethod.PaymnetMethod?.ToString()
                });
            }

            return new PagedResultDto<ReceiptPaymentMethodLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}
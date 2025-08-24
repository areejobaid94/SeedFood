//using Infoseed.MessagingPortal.CloseDealStatuses;

//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Abp.Domain.Repositories;
//using Infoseed.MessagingPortal.Close_Deals.Exporting;
//using Infoseed.MessagingPortal.Close_Deals.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Authorization;
//using Abp.Extensions;
//using Abp.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Abp.UI;
//using Infoseed.MessagingPortal.Storage;
//using Infoseed.MessagingPortal.DealStatuses;
//using Infoseed.MessagingPortal.DealTypes;
//using Infoseed.MessagingPortal.Territories;
//using Infoseed.MessagingPortal.Forcasts;

//namespace Infoseed.MessagingPortal.Close_Deals
//{
//    //[AbpAuthorize(AppPermissions.Pages_Close_Deals)]
//    public class Close_DealsAppService : MessagingPortalAppServiceBase, IClose_DealsAppService
//    {
//        private readonly IRepository<Close_Deal> _close_DealRepository;
//        private readonly IClose_DealsExcelExporter _close_DealsExcelExporter;
//        private readonly IRepository<CloseDealStatus, int> _lookup_closeDealStatusRepository;

//        private readonly IRepository<DealStatus, int> _lookup_dealStatusRepository;
//        private readonly IRepository<DealType, int> _lookup_dealTypeRepository;
//        private readonly IRepository<Territory> _lookup_territoryRepository;

//        public Close_DealsAppService(IRepository<Close_Deal> close_DealRepository, IClose_DealsExcelExporter close_DealsExcelExporter, IRepository<CloseDealStatus, int> lookup_closeDealStatusRepository, IRepository<DealStatus, int> lookup_dealStatusRepository, IRepository<DealType, int> lookup_dealTypeRepository, IRepository<Territory> lookup_territoryRepository)
//        {
//            _close_DealRepository = close_DealRepository;
//            _close_DealsExcelExporter = close_DealsExcelExporter;
//            _lookup_closeDealStatusRepository = lookup_closeDealStatusRepository;

//            _lookup_dealStatusRepository = lookup_dealStatusRepository;
//            _lookup_dealTypeRepository = lookup_dealTypeRepository;
//            _lookup_territoryRepository = lookup_territoryRepository;
//        }

//        public async Task<PagedResultDto<GetClose_DealForViewDto>> GetAll(GetAllClose_DealsInput input)
//        {

//            var filteredClose_Deals = _close_DealRepository.GetAll()
//                        .Include(e => e.CloseDealStatusFk)
//                        .Include(e => e.DealStatusFk)
//                        .Include(e => e.DealTypeFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.DealName.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.DealAge.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealNameFilter), e => e.DealName == input.DealNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
//                        .WhereIf(input.MinCloseDateFilter != null, e => e.CloseDate >= input.MinCloseDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealAgeFilter), e => e.DealAge == input.DealAgeFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusStatusFilter), e => e.DealStatusFk != null && e.DealStatusFk.Status == input.DealStatusStatusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealTypeDeal_TypeFilter), e => e.DealTypeFk != null && e.DealTypeFk.Deal_Type == input.DealTypeDeal_TypeFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerCommercialNameArabicFilter), e => e.CustomerCommercialNameArabic == input.CustomerCommercialNameArabicFilter)
//                        .WhereIf(input.MinContrevelDateFilter != null, e => e.ContrevelDate >= input.MinContrevelDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.FirstPayFilter), e => e.FirstPay == input.FirstPayFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.SecondPayFilter), e => e.SecondPay == input.SecondPayFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.AMClosedFilter), e => e.AMClosed == input.AMClosedFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CloseDealStatusStatusFilter), e => e.CloseDealStatusFk != null && e.CloseDealStatusFk.Status == input.CloseDealStatusStatusFilter);

//            var pagedAndFilteredClose_Deals = filteredClose_Deals
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var close_Deals = from o in pagedAndFilteredClose_Deals
//                              join o1 in _lookup_closeDealStatusRepository.GetAll() on o.CloseDealStatusId equals o1.Id into j1
//                              from s1 in j1.DefaultIfEmpty()

//                              join o2 in _lookup_dealStatusRepository.GetAll() on o.DealStatusId equals o2.Id into j2
//                              from s2 in j2.DefaultIfEmpty()

//                              join o3 in _lookup_dealTypeRepository.GetAll() on o.DealTypeId equals o3.Id into j3
//                              from s3 in j3.DefaultIfEmpty()

//                              select new
//                              {

//                                  o.UserName,
//                                  o.CustomerName,
//                                  o.CustomerCommercialNameArabic,
//                                  o.ARR,
//                                  o.OrderFees,
//                                  o.ContrevelDate,
//                                  o.FirstPay,
//                                  o.SecondPay,
//                                  o.AMClosed,
//                                  Id = o.Id,                               
//                                  o.DealName,
//                                  o.CloseDate,
//                                  o.DealAge,
//                                  CloseDealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString(),
//                                  DealStatusStatus = s2 == null || s2.Status == null ? "" : s2.Status.ToString(),
//                                  DealTypeDeal_Type = s3 == null || s3.Deal_Type == null ? "" : s3.Deal_Type.ToString()
//                              };

//            var totalCount = await filteredClose_Deals.CountAsync();

//            var dbList = await close_Deals.ToListAsync();
//            var results = new List<GetClose_DealForViewDto>();

//            foreach (var o in dbList)
//            {
//                var res = new GetClose_DealForViewDto()
//                {
//                    Close_Deal = new Close_DealDto
//                    {
//                        UserName = o.UserName,
//                        CustomerName = o.CustomerName,
//                        ARR = o.ARR,
//                        OrderFees = o.OrderFees,
//                        CustomerCommercialNameArabic = o.CustomerCommercialNameArabic,
//                        ContrevelDate = o.ContrevelDate,
//                        FirstPay = o.FirstPay,
//                        SecondPay = o.SecondPay,
//                        AMClosed = o.AMClosed,
//                        Id = o.Id,
//                        DealName = o.DealName,
//                        CloseDate = o.CloseDate,
//                        DealAge = o.DealAge,
//                    },
//                    CloseDealStatusStatus = o.CloseDealStatusStatus,
//                    DealStatusStatus = o.DealStatusStatus,
//                    DealTypeDeal_Type = o.DealTypeDeal_Type
//                };

//                results.Add(res);
//            }

//            return new PagedResultDto<GetClose_DealForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetClose_DealForViewDto> GetClose_DealForView(int id)
//        {
//            var close_Deal = await _close_DealRepository.GetAsync(id);

//            var output = new GetClose_DealForViewDto { Close_Deal = ObjectMapper.Map<Close_DealDto>(close_Deal) };

//            if (output.Close_Deal.CloseDealStatusId != null)
//            {
//                var _lookupCloseDealStatus = await _lookup_closeDealStatusRepository.FirstOrDefaultAsync((int)output.Close_Deal.CloseDealStatusId);
//                output.CloseDealStatusStatus = _lookupCloseDealStatus?.Status?.ToString();
//            }

//            return output;
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Close_Deals_Edit)]
//        public async Task<GetClose_DealForEditOutput> GetClose_DealForEdit(EntityDto input)
//        {
//            var close_Deal = await _close_DealRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetClose_DealForEditOutput { Close_Deal = ObjectMapper.Map<CreateOrEditClose_DealDto>(close_Deal) };

//            if (output.Close_Deal.CloseDealStatusId != null)
//            {
//                var _lookupCloseDealStatus = await _lookup_closeDealStatusRepository.FirstOrDefaultAsync((int)output.Close_Deal.CloseDealStatusId);
//                output.CloseDealStatusStatus = _lookupCloseDealStatus?.Status?.ToString();
//            }

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditClose_DealDto input)
//        {
//            if (input.Id == null)
//            {
//                await Create(input);
//            }
//            else
//            {
//                await Update(input);
//            }
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Close_Deals_Create)]
//        protected virtual async Task Create(CreateOrEditClose_DealDto input)
//        {
//            var close_Deal = ObjectMapper.Map<Close_Deal>(input);

//            await _close_DealRepository.InsertAsync(close_Deal);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Close_Deals_Edit)]
//        protected virtual async Task Update(CreateOrEditClose_DealDto input)
//        {
//            var close_Deal = await _close_DealRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, close_Deal);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Close_Deals_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _close_DealRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetClose_DealsToExcel(GetAllClose_DealsForExcelInput input)
//        {

//            var filteredClose_Deals = _close_DealRepository.GetAll()
//                        .Include(e => e.CloseDealStatusFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.CustomerCommercialNameArabic.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.FirstPay.Contains(input.Filter) || e.SecondPay.Contains(input.Filter) || e.AMClosed.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerCommercialNameArabicFilter), e => e.CustomerCommercialNameArabic == input.CustomerCommercialNameArabicFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
//                        .WhereIf(input.MinContrevelDateFilter != null, e => e.ContrevelDate >= input.MinContrevelDateFilter)
//                        .WhereIf(input.MaxContrevelDateFilter != null, e => e.ContrevelDate <= input.MaxContrevelDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.FirstPayFilter), e => e.FirstPay == input.FirstPayFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.SecondPayFilter), e => e.SecondPay == input.SecondPayFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.AMClosedFilter), e => e.AMClosed == input.AMClosedFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CloseDealStatusStatusFilter), e => e.CloseDealStatusFk != null && e.CloseDealStatusFk.Status == input.CloseDealStatusStatusFilter);

//            var query = (from o in filteredClose_Deals
//                         join o1 in _lookup_closeDealStatusRepository.GetAll() on o.CloseDealStatusId equals o1.Id into j1
//                         from s1 in j1.DefaultIfEmpty()

//                         select new GetClose_DealForViewDto()
//                         {
//                             Close_Deal = new Close_DealDto
//                             {
//                                 UserName = o.UserName,
//                                 CustomerName = o.CustomerName,
//                                 CustomerCommercialNameArabic = o.CustomerCommercialNameArabic,
//                                 ARR = o.ARR,
//                                 OrderFees = o.OrderFees,
//                                 ContrevelDate = o.ContrevelDate,
//                                 FirstPay = o.FirstPay,
//                                 SecondPay = o.SecondPay,
//                                 AMClosed = o.AMClosed,
//                                 Id = o.Id
//                             },
//                             CloseDealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString()
//                         });

//            var close_DealListDtos = await query.ToListAsync();

//            return _close_DealsExcelExporter.ExportToFile(close_DealListDtos);
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Close_Deals)]
//        public async Task<PagedResultDto<Close_DealCloseDealStatusLookupTableDto>> GetAllCloseDealStatusForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_closeDealStatusRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.Status != null && e.Status.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var closeDealStatusList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<Close_DealCloseDealStatusLookupTableDto>();
//            foreach (var closeDealStatus in closeDealStatusList)
//            {
//                lookupTableDtoList.Add(new Close_DealCloseDealStatusLookupTableDto
//                {
//                    Id = closeDealStatus.Id,
//                    DisplayName = closeDealStatus.Status?.ToString()
//                });
//            }

//            return new PagedResultDto<Close_DealCloseDealStatusLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }

//    }
//}
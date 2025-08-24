//using Abp.Application.Services.Dto;
//using Abp.Domain.Repositories;
//using Abp.Linq.Extensions;
//using Infoseed.MessagingPortal.Close_Deals;
//using Infoseed.MessagingPortal.Deals.Dtos;
//using Infoseed.MessagingPortal.Deals.Exporting;
//using Infoseed.MessagingPortal.DealStatuses;
//using Infoseed.MessagingPortal.DealTypes;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Forcasts;
//using Infoseed.MessagingPortal.Territories;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;

//namespace Infoseed.MessagingPortal.Deals
//{
//    //[AbpAuthorize(AppPermissions.Pages_Deals)]
//    public class DealsAppService : MessagingPortalAppServiceBase, IDealsAppService
//    {
//        private readonly IRepository<Deal> _dealRepository;
//        private readonly IDealsExcelExporter _dealsExcelExporter;
//        private readonly IRepository<DealStatus, int> _lookup_dealStatusRepository;
//        private readonly IRepository<DealType, int> _lookup_dealTypeRepository;
//        private readonly IRepository<Territory> _lookup_territoryRepository;
//        private readonly IRepository<Forcats> _forcatsRepository;
//        private readonly IRepository<SalesUserCreate.SalesUserCreate> _salesUserCreateRepository;

//        public DealsAppService(IRepository<Deal> dealRepository, IDealsExcelExporter dealsExcelExporter, IRepository<DealStatus, int> lookup_dealStatusRepository, IRepository<DealType, int> lookup_dealTypeRepository,IRepository<Territory> lookup_territoryRepository, IRepository<Forcats> forcatsRepository, IRepository<SalesUserCreate.SalesUserCreate> salesUserCreateRepository)
//        {
//            _dealRepository = dealRepository;
//            _dealsExcelExporter = dealsExcelExporter;
//            _lookup_dealStatusRepository = lookup_dealStatusRepository;
//            _lookup_dealTypeRepository = lookup_dealTypeRepository;
//            _lookup_territoryRepository = lookup_territoryRepository;
//            _forcatsRepository = forcatsRepository;
//            _salesUserCreateRepository = salesUserCreateRepository;
//        }

//        public async Task<PagedResultDto<GetDealForViewDto>> GetAll(GetAllDealsInput input)
//        {
   

//            var filteredDeals = _dealRepository.GetAll()
//                        .Include(e => e.DealStatusFk)
//                        .Include(e => e.DealTypeFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.DealName.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.DealAge.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealNameFilter), e => e.DealName == input.DealNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
//                        .WhereIf(input.MinCloseDateFilter != null, e => e.CloseDate >= input.MinCloseDateFilter)
//                        .WhereIf(input.MaxCloseDateFilter != null, e => e.CloseDate <= input.MaxCloseDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealAgeFilter), e => e.DealAge == input.DealAgeFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusStatusFilter), e => e.DealStatusFk != null && e.DealStatusFk.Status == input.DealStatusStatusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealTypeDeal_TypeFilter), e => e.DealTypeFk != null && e.DealTypeFk.Deal_Type == input.DealTypeDeal_TypeFilter);

//            var pagedAndFilteredDeals = filteredDeals
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var deals = from o in pagedAndFilteredDeals
//                        join o1 in _lookup_dealStatusRepository.GetAll() on o.DealStatusId equals o1.Id into j1
//                        from s1 in j1.DefaultIfEmpty()

//                        join o2 in _lookup_dealTypeRepository.GetAll() on o.DealTypeId equals o2.Id into j2
//                        from s2 in j2.DefaultIfEmpty()

//                        select new
//                        {

//                            o.UserName,
//                            o.CustomerName,
//                            o.DealName,
//                            o.ARR,
//                            o.OrderFees,
//                            o.CloseDate,
//                            o.DealAge,
//                            Id = o.Id,
//                            DealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString(),
//                            DealTypeDeal_Type = s2 == null || s2.Deal_Type == null ? "" : s2.Deal_Type.ToString()
//                        };

//            var totalCount = await filteredDeals.CountAsync();

//            var dbList = await deals.ToListAsync();
//            var results = new List<GetDealForViewDto>();

//            var list = _salesUserCreateRepository.GetAll().ToList();

//            var found = list.Where(x => x.UserName == input.UserNameFilter).FirstOrDefault();
//            if (found != null)
//            {

//                foreach (var o in dbList)
//                {
//                    var res = new GetDealForViewDto()
//                    {
//                        Deal = new DealDto
//                        {

//                            UserName = o.UserName,
//                            CustomerName = o.CustomerName,
//                            DealName = o.DealName,
//                            ARR = o.ARR,
//                            OrderFees = o.OrderFees,
//                            CloseDate = o.CloseDate,
//                            DealAge = o.DealAge,
//                            Id = o.Id,
//                        },
//                        DealStatusStatus = o.DealStatusStatus,
//                        DealTypeDeal_Type = o.DealTypeDeal_Type,
//                         IsSubmitActiveButton= found.IsActiveSubmitButton
//                    };

//                    results.Add(res);
//                }

//            }
//            else
//            {
//                foreach (var o in dbList)
//                {
//                    var res = new GetDealForViewDto()
//                    {
//                        Deal = new DealDto
//                        {

//                            UserName = o.UserName,
//                            CustomerName = o.CustomerName,
//                            DealName = o.DealName,
//                            ARR = o.ARR,
//                            OrderFees = o.OrderFees,
//                            CloseDate = o.CloseDate,
//                            DealAge = o.DealAge,
//                            Id = o.Id,
//                        },
//                        DealStatusStatus = o.DealStatusStatus,
//                        DealTypeDeal_Type = o.DealTypeDeal_Type,
//                        IsSubmitActiveButton = false
//                    };

//                    results.Add(res);
//                }
//            }

            

//            return new PagedResultDto<GetDealForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetDealForViewDto> GetDealForView(int id)
//        {
//            var deal = await _dealRepository.GetAsync(id);

//            var output = new GetDealForViewDto { Deal = ObjectMapper.Map<DealDto>(deal) };

//            //if (output.Deal.DealStatusId != null)
//            //{
//                var _lookupDealStatus = await _lookup_dealStatusRepository.FirstOrDefaultAsync((int)output.Deal.DealStatusId);
//                output.DealStatusStatus = _lookupDealStatus?.Status?.ToString();
//            //}

//            //if (output.Deal.DealTypeId != null)
//            //{
//                var _lookupDealType = await _lookup_dealTypeRepository.FirstOrDefaultAsync((int)output.Deal.DealTypeId);
//                output.DealTypeDeal_Type = _lookupDealType?.Deal_Type?.ToString();
//            //}

//            return output;
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Deals_Edit)]
//        public async Task<GetDealForEditOutput> GetDealForEdit(EntityDto input)
//        {
//            var deal = await _dealRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetDealForEditOutput { Deal = ObjectMapper.Map<CreateOrEditDealDto>(deal) };

//            //if (output.Deal.DealStatusId != null)
//            //{
//                var _lookupDealStatus = await _lookup_dealStatusRepository.FirstOrDefaultAsync((int)output.Deal.DealStatusId);
//                output.DealStatusStatus = _lookupDealStatus?.Status?.ToString();
//            //}

//            //if (output.Deal.DealTypeId != null)
//            //{
//                var _lookupDealType = await _lookup_dealTypeRepository.FirstOrDefaultAsync((int)output.Deal.DealTypeId);
//                output.DealTypeDeal_Type = _lookupDealType?.Deal_Type?.ToString();
//            //}

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditDealDto input)
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

//        //[AbpAuthorize(AppPermissions.Pages_Deals_Create)]
//        protected virtual async Task Create(CreateOrEditDealDto input)
//        {
//            var deal = ObjectMapper.Map<Deal>(input);

//            if (AbpSession.TenantId != null)
//            {
//                deal.TenantId = (int?)AbpSession.TenantId;
//            }

//            await _dealRepository.InsertAsync(deal);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Deals_Edit)]
//        protected virtual async Task Update(CreateOrEditDealDto input)
//        {
//            var deal = await _dealRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, deal);

//        }

//        //[AbpAuthorize(AppPermissions.Pages_Deals_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _dealRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetDealsToExcel(GetAllDealsForExcelInput input)
//        {

//            var filteredDeals = _dealRepository.GetAll()
//                        .Include(e => e.DealStatusFk)
//                        .Include(e => e.DealTypeFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.DealName.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.DealAge.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealNameFilter), e => e.DealName == input.DealNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
//                        .WhereIf(input.MinCloseDateFilter != null, e => e.CloseDate >= input.MinCloseDateFilter)
//                        .WhereIf(input.MaxCloseDateFilter != null, e => e.CloseDate <= input.MaxCloseDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealAgeFilter), e => e.DealAge == input.DealAgeFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusStatusFilter), e => e.DealStatusFk != null && e.DealStatusFk.Status == input.DealStatusStatusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealTypeDeal_TypeFilter), e => e.DealTypeFk != null && e.DealTypeFk.Deal_Type == input.DealTypeDeal_TypeFilter);

//            var query = (from o in filteredDeals
//                         join o1 in _lookup_dealStatusRepository.GetAll() on o.DealStatusId equals o1.Id into j1
//                         from s1 in j1.DefaultIfEmpty()

//                         join o2 in _lookup_dealTypeRepository.GetAll() on o.DealTypeId equals o2.Id into j2
//                         from s2 in j2.DefaultIfEmpty()

//                         select new GetDealForViewDto()
//                         {
//                             Deal = new DealDto
//                             {
//                                 UserName = o.UserName,
//                                 CustomerName = o.CustomerName,
//                                 DealName = o.DealName,
//                                 ARR = o.ARR,
//                                 OrderFees = o.OrderFees,
//                                 CloseDate = o.CloseDate,
//                                 DealAge = o.DealAge,
//                                 Id = o.Id
//                             },
//                             DealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString(),
//                             DealTypeDeal_Type = s2 == null || s2.Deal_Type == null ? "" : s2.Deal_Type.ToString()
//                         });

//            var dealListDtos = await query.ToListAsync();

//            return _dealsExcelExporter.ExportToFile(dealListDtos);
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Deals)]
//        public async Task<PagedResultDto<DealDealStatusLookupTableDto>> GetAllDealStatusForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_dealStatusRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.Status != null && e.Status.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var dealStatusList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<DealDealStatusLookupTableDto>();
//            foreach (var dealStatus in dealStatusList)
//            {
//                lookupTableDtoList.Add(new DealDealStatusLookupTableDto
//                {
//                    Id = dealStatus.Id,
//                    DisplayName = dealStatus.Status?.ToString()
//                });
//            }

//            return new PagedResultDto<DealDealStatusLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }

//        //[AbpAuthorize(AppPermissions.Pages_Deals)]
//        public async Task<PagedResultDto<DealDealTypeLookupTableDto>> GetAllDealTypeForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_dealTypeRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.Deal_Type != null && e.Deal_Type.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var dealTypeList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<DealDealTypeLookupTableDto>();
//            foreach (var dealType in dealTypeList)
//            {
//                lookupTableDtoList.Add(new DealDealTypeLookupTableDto
//                {
//                    Id = dealType.Id,
//                    DisplayName = dealType.Deal_Type?.ToString()
//                });
//            }

//            return new PagedResultDto<DealDealTypeLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }


//        //[AbpAuthorize(AppPermissions.Pages_Deals)]
//        public async Task<PagedResultDto<TerritoryLookupTableDto>> GetAllTerritoryForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_territoryRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.UserName != null && e.UserName.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var dealTypeList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<TerritoryLookupTableDto>();
//            foreach (var dealType in dealTypeList)
//            {
//                lookupTableDtoList.Add(new TerritoryLookupTableDto
//                {
//                    Id = dealType.Id,
//                    DisplayName = dealType.EnglishName?.ToString()
//                });
//            }

//            return new PagedResultDto<TerritoryLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }


//        public async Task<PagedResultDto<GetDealForViewDto>> SubmitDeal(string username)
//        {
//            GetAllDealsInput input = new GetAllDealsInput
//            {

//                ARRFilter = "",
//                CustomerNameFilter = "",
//                DealAgeFilter = "",
//                DealNameFilter = "",
//                DealStatusStatusFilter = "",
//                DealTypeDeal_TypeFilter = "",
//                Filter = username,
//                //Sorting = "",
//                SkipCount = 0,
//                MaxResultCount =int.MaxValue,
//                MaxCloseDateFilter =null,

//                MinCloseDateFilter = null,
//                OrderFeesFilter = "",

//                UserNameFilter = "",



//            };
//            var filteredDeals = _dealRepository.GetAll()
//                        .Include(e => e.DealStatusFk)
//                        .Include(e => e.DealTypeFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.DealName.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.DealAge.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealNameFilter), e => e.DealName == input.DealNameFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
//                        .WhereIf(input.MinCloseDateFilter != null, e => e.CloseDate >= input.MinCloseDateFilter)
//                        .WhereIf(input.MaxCloseDateFilter != null, e => e.CloseDate <= input.MaxCloseDateFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealAgeFilter), e => e.DealAge == input.DealAgeFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusStatusFilter), e => e.DealStatusFk != null && e.DealStatusFk.Status == input.DealStatusStatusFilter)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealTypeDeal_TypeFilter), e => e.DealTypeFk != null && e.DealTypeFk.Deal_Type == input.DealTypeDeal_TypeFilter);

//            var pagedAndFilteredDeals = filteredDeals
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var deals = from o in pagedAndFilteredDeals
//                        join o1 in _lookup_dealStatusRepository.GetAll() on o.DealStatusId equals o1.Id into j1
//                        from s1 in j1.DefaultIfEmpty()

//                        join o2 in _lookup_dealTypeRepository.GetAll() on o.DealTypeId equals o2.Id into j2
//                        from s2 in j2.DefaultIfEmpty()

//                        select new
//                        {
//                            o.DealStatusId,
//                            o.DealTypeId,
//                            o.UserName,
//                            o.CustomerName,
//                            o.DealName,
//                            o.ARR,
//                            o.OrderFees,
//                            o.CloseDate,
//                            o.DealAge,
//                            Id = o.Id,
//                            DealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString(),
//                            DealTypeDeal_Type = s2 == null || s2.Deal_Type == null ? "" : s2.Deal_Type.ToString()
//                        };

//            var totalCount = await filteredDeals.CountAsync();

//            var dbList = await deals.ToListAsync();
//            var results = new List<GetDealForViewDto>();

//            foreach (var o in dbList)
//            {
//                var res = new GetDealForViewDto()
//                {
//                    Deal = new DealDto
//                    {

//                        UserName = o.UserName,
//                        CustomerName = o.CustomerName,
//                        DealName = o.DealName,
//                        ARR = o.ARR,
//                        OrderFees = o.OrderFees,
//                        CloseDate = o.CloseDate,
//                         DealStatusId= o.DealStatusId,
//                          DealTypeId= o.DealTypeId,
//                        DealAge = o.DealAge,
//                        Id = o.Id,
//                    },
//                    DealStatusStatus = o.DealStatusStatus,
//                    DealTypeDeal_Type = o.DealTypeDeal_Type
//                };

//                results.Add(res);
//            }

//            var list = _salesUserCreateRepository.GetAll().ToList();

//            var found = list.Where(x => x.UserName == username).FirstOrDefault();

//            int count = 0;


//            if (found != null)
//            {
//                count = found.TotalCreate;
//            }

//            foreach (var item in results)
//            {
//                if(item.DealTypeDeal_Type== "Commit")
//                {
//                    Forcats forcats = new Forcats
//                    { Id=0,
//                        ARR = item.Deal.ARR,
//                        CloseDate = item.Deal.CloseDate,
//                        CustomerName = item.Deal.CustomerName,
//                        DealAge = item.Deal.DealAge,
//                        DealName = item.Deal.DealName,
//                        DealStatusId = item.Deal.DealStatusId,
//                        DealTypeId = item.Deal.DealTypeId,
//                        OrderFees = item.Deal.OrderFees,
//                        SubmitDate = DateTime.UtcNow,
//                        TenantId = AbpSession.TenantId,
//                        UserName = item.Deal.UserName

//                    };

//                    AddForcats(forcats);
     

//                    if (found != null)
//                    {
//                        SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                        {
//                            Id = found.Id,
//                             IsActiveSubmitButton=false,
//                            IsActiveButton = true,
//                            TenantId = AbpSession.TenantId,
//                            TotalCreate = found.TotalCreate ,
//                            UserId = 0,
//                            UserName = username
//                        };

//                        UpdateSalesUserCreate(salesUserCreate);
//                    }

//                }else if (item.DealTypeDeal_Type == "Close")
//                {
//                    Close_Deal  close_Deal = new Close_Deal
//                    {
//                        Id = 0,
//                        ARR = item.Deal.ARR,
//                        CloseDate = item.Deal.CloseDate,
//                        CustomerName = item.Deal.CustomerName,
//                        DealAge = item.Deal.DealAge,
//                        DealName = item.Deal.DealName,
//                        DealStatusId = item.Deal.DealStatusId,
//                        DealTypeId = item.Deal.DealTypeId,
//                        OrderFees = item.Deal.OrderFees,
//                         ContrevelDate = DateTime.UtcNow,
//                        TenantId = AbpSession.TenantId,
//                        UserName = item.Deal.UserName

//                    };

//                    AddClose_Deals(close_Deal);
//                    await _dealRepository.DeleteAsync(item.Deal.Id);

                    
//                    if (found != null)
//                    {
//                        SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                        {
//                            Id = found.Id,
//                            IsActiveSubmitButton = false,
//                            IsActiveButton = true,
//                            TenantId = AbpSession.TenantId,
//                            TotalCreate = ++count,
//                            UserId = 0,
//                            UserName = username
//                        };

//                        UpdateSalesUserCreate(salesUserCreate);
//                    }

                   
//                }
//                else
//                {
//                    SalesUserCreate.SalesUserCreate salesUserCreate = new SalesUserCreate.SalesUserCreate
//                    {
//                        Id = found.Id,
//                        IsActiveSubmitButton = false,
//                        IsActiveButton = true,
//                        TenantId = AbpSession.TenantId,
//                        TotalCreate = found.TotalCreate,
//                        UserId = 0,
//                        UserName = username
//                    };

//                    UpdateSalesUserCreate(salesUserCreate);

//                }


//            }
//           ;
//            return new PagedResultDto<GetDealForViewDto>(
//                         totalCount,
//                         results
//                     );

//        }

//        private void AddForcats(Forcats  forcats)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "INSERT INTO Forcasts (UserName , SubmitDate, ARR, CloseDate, CustomerName, DealAge, DealName, DealStatusId, DealTypeId, OrderFees, TenantId) VALUES (@UserName , @SubmitDate, @ARR, @CloseDate, @CustomerName, @DealAge, @DealName, @DealStatusId, @DealTypeId, @OrderFees, @TenantId) ";

//                        command.Parameters.AddWithValue("@UserName", forcats.UserName);
//                        command.Parameters.AddWithValue("@SubmitDate", forcats.SubmitDate);
//                        command.Parameters.AddWithValue("@ARR", forcats.ARR);
//                        command.Parameters.AddWithValue("@CloseDate", forcats.CloseDate);
//                        command.Parameters.AddWithValue("@CustomerName", forcats.CustomerName);

//                        command.Parameters.AddWithValue("@DealAge", forcats.DealAge);
//                        command.Parameters.AddWithValue("@DealName", forcats.DealName);
//                        command.Parameters.AddWithValue("@DealStatusId", forcats.DealStatusId);
//                        command.Parameters.AddWithValue("@DealTypeId", forcats.DealTypeId);
//                        command.Parameters.AddWithValue("@OrderFees", forcats.OrderFees);
//                        command.Parameters.AddWithValue("@TenantId", forcats.TenantId);


//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception )
//                {


//                }

//        }

//        private void AddClose_Deals(Close_Deal  close_Deal)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "INSERT INTO Close_Deals (UserName , ContrevelDate, ARR, CloseDate, CustomerName, DealAge, DealName, DealStatusId, DealTypeId, OrderFees, TenantId) VALUES (@UserName , @ContrevelDate, @ARR, @CloseDate, @CustomerName, @DealAge, @DealName, @DealStatusId, @DealTypeId, @OrderFees, @TenantId) ";

//                        command.Parameters.AddWithValue("@UserName", close_Deal.UserName);
//                        command.Parameters.AddWithValue("@ContrevelDate", close_Deal.ContrevelDate);
//                        command.Parameters.AddWithValue("@ARR", close_Deal.ARR);
//                        command.Parameters.AddWithValue("@CloseDate", close_Deal.CloseDate);
//                        command.Parameters.AddWithValue("@CustomerName", close_Deal.CustomerName);

//                        command.Parameters.AddWithValue("@DealAge", close_Deal.DealAge);
//                        command.Parameters.AddWithValue("@DealName", close_Deal.DealName);
//                        command.Parameters.AddWithValue("@DealStatusId", close_Deal.DealStatusId);
//                        command.Parameters.AddWithValue("@DealTypeId", close_Deal.DealTypeId);
//                        command.Parameters.AddWithValue("@OrderFees", close_Deal.OrderFees);
//                        command.Parameters.AddWithValue("@TenantId", close_Deal.TenantId);


//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception)
//                {


//                }

//        }
//        private void UpdateSalesUserCreate(SalesUserCreate.SalesUserCreate salesUserCreate)
//        {
//            string connString = AppSettingsModel.ConnectionStrings;
//            using (SqlConnection connection = new SqlConnection(connString))
//                try
//                {

//                    using (SqlCommand command = connection.CreateCommand())
//                    {

//                        command.CommandText = "UPDATE SalesUserCreate SET TenantId = @TenantId ,UserId =@UserId , UserName = @UserName , TotalCreate = @TotalCreate ,IsActiveButton = @IsActiveButton, IsActiveSubmitButton=@IsActiveSubmitButton  Where Id = @Id";

//                        command.Parameters.AddWithValue("@Id", salesUserCreate.Id);
//                        command.Parameters.AddWithValue("@TenantId", salesUserCreate.TenantId);
//                        command.Parameters.AddWithValue("@UserId", salesUserCreate.UserId);
//                        command.Parameters.AddWithValue("@UserName", salesUserCreate.UserName);
//                        command.Parameters.AddWithValue("@TotalCreate", salesUserCreate.TotalCreate);
//                        command.Parameters.AddWithValue("@IsActiveButton", salesUserCreate.IsActiveButton);
//                        command.Parameters.AddWithValue("@IsActiveSubmitButton", salesUserCreate.IsActiveSubmitButton);
                        
//                        connection.Open();
//                        command.ExecuteNonQuery();
//                        connection.Close();
//                    }
//                }
//                catch (Exception )
//                {


//                }

//        }

//    }
//}
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Forcasts.Exporting;
using Infoseed.MessagingPortal.Forcasts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Infoseed.MessagingPortal.Storage;
using Infoseed.MessagingPortal.DealStatuses;
using Infoseed.MessagingPortal.DealTypes;
using Infoseed.MessagingPortal.Territories;

namespace Infoseed.MessagingPortal.Forcasts
{
    //[AbpAuthorize(AppPermissions.Pages_Forcatses)]
    public class ForcatsesAppService : MessagingPortalAppServiceBase, IForcatsesAppService
    {
        private readonly IRepository<Forcats> _forcatsRepository;
        private readonly IForcatsesExcelExporter _forcatsesExcelExporter;

        private readonly IRepository<DealStatus, int> _lookup_dealStatusRepository;
        private readonly IRepository<DealType, int> _lookup_dealTypeRepository;
        private readonly IRepository<Territory> _lookup_territoryRepository;

        public ForcatsesAppService(IRepository<Forcats> forcatsRepository, IForcatsesExcelExporter forcatsesExcelExporter ,IRepository<DealStatus, int> lookup_dealStatusRepository, IRepository<DealType, int> lookup_dealTypeRepository, IRepository<Territory> lookup_territoryRepository)
        {
            _forcatsRepository = forcatsRepository;
            _forcatsesExcelExporter = forcatsesExcelExporter;

            _lookup_dealStatusRepository = lookup_dealStatusRepository;
            _lookup_dealTypeRepository = lookup_dealTypeRepository;
            _lookup_territoryRepository = lookup_territoryRepository;
        }

        public async Task<PagedResultDto<GetForcatsForViewDto>> GetAll(GetAllForcatsesInput input)
        {

            var filteredForcatses = _forcatsRepository.GetAll()
                             .Include(e => e.DealStatusFk)
                        .Include(e => e.DealTypeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.CustomerName.Contains(input.Filter) || e.DealName.Contains(input.Filter) || e.ARR.Contains(input.Filter) || e.OrderFees.Contains(input.Filter) || e.DealAge.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerName == input.CustomerNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealNameFilter), e => e.DealName == input.DealNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ARRFilter), e => e.ARR == input.ARRFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.OrderFeesFilter), e => e.OrderFees == input.OrderFeesFilter)
                        .WhereIf(input.MinCloseDateFilter != null, e => e.CloseDate >= input.MinCloseDateFilter)
                        .WhereIf(input.MaxCloseDateFilter != null, e => e.CloseDate <= input.MaxCloseDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealAgeFilter), e => e.DealAge == input.DealAgeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusStatusFilter), e => e.DealStatusFk != null && e.DealStatusFk.Status == input.DealStatusStatusFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealTypeDeal_TypeFilter), e => e.DealTypeFk != null && e.DealTypeFk.Deal_Type == input.DealTypeDeal_TypeFilter);

            var pagedAndFilteredForcatses = filteredForcatses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var forcatses = from o in pagedAndFilteredForcatses
                            join o1 in _lookup_dealStatusRepository.GetAll() on o.DealStatusId equals o1.Id into j1
                            from s1 in j1.DefaultIfEmpty()

                            join o2 in _lookup_dealTypeRepository.GetAll() on o.DealTypeId equals o2.Id into j2
                            from s2 in j2.DefaultIfEmpty()

                            select new
                            {


                                o.UserName,
                                o.CustomerName,
                                o.DealName,
                                o.ARR,
                                o.OrderFees,
                                o.CloseDate,
                                o.DealAge,
                                DealStatusStatus = s1 == null || s1.Status == null ? "" : s1.Status.ToString(),
                                DealTypeDeal_Type = s2 == null || s2.Deal_Type == null ? "" : s2.Deal_Type.ToString(),
                                o.TotalCommit,
                                o.TotalClosed,
                                o.SubmitDate,
                                Id = o.Id
                            };

            var totalCount = await filteredForcatses.CountAsync();

            var dbList = await forcatses.ToListAsync();
            var results = new List<GetForcatsForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetForcatsForViewDto()
                {
                    Forcats = new ForcatsDto
                    {

                        UserName = o.UserName,
                        CustomerName = o.CustomerName,
                        DealName = o.DealName,
                        ARR = o.ARR,
                        OrderFees = o.OrderFees,
                        CloseDate = o.CloseDate,
                        DealAge = o.DealAge,
                        TotalCommit = o.TotalCommit,
                        TotalClosed = o.TotalClosed,
                        SubmitDate = o.SubmitDate,
                        Id = o.Id,
                    },
                    DealStatusStatus = o.DealStatusStatus,
                    DealTypeDeal_Type = o.DealTypeDeal_Type
                };

                results.Add(res);
            }

            return new PagedResultDto<GetForcatsForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetForcatsForViewDto> GetForcatsForView(int id)
        {
            var forcats = await _forcatsRepository.GetAsync(id);

            var output = new GetForcatsForViewDto { Forcats = ObjectMapper.Map<ForcatsDto>(forcats) };

            return output;
        }

        //[AbpAuthorize(AppPermissions.Pages_Forcatses_Edit)]
        public async Task<GetForcatsForEditOutput> GetForcatsForEdit(EntityDto input)
        {
            var forcats = await _forcatsRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetForcatsForEditOutput { Forcats = ObjectMapper.Map<CreateOrEditForcatsDto>(forcats) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditForcatsDto input)
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

        //[AbpAuthorize(AppPermissions.Pages_Forcatses_Create)]
        protected virtual async Task Create(CreateOrEditForcatsDto input)
        {
            var forcats = ObjectMapper.Map<Forcats>(input);

            await _forcatsRepository.InsertAsync(forcats);

        }

        //[AbpAuthorize(AppPermissions.Pages_Forcatses_Edit)]
        protected virtual async Task Update(CreateOrEditForcatsDto input)
        {
            var forcats = await _forcatsRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, forcats);

        }

        //[AbpAuthorize(AppPermissions.Pages_Forcatses_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _forcatsRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetForcatsesToExcel(GetAllForcatsesForExcelInput input)
        {

            var filteredForcatses = _forcatsRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UserName.Contains(input.Filter) || e.TotalCommit.Contains(input.Filter) || e.TotalClosed.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserName == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TotalCommitFilter), e => e.TotalCommit == input.TotalCommitFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TotalClosedFilter), e => e.TotalClosed == input.TotalClosedFilter)
                        .WhereIf(input.MinSubmitDateFilter != null, e => e.SubmitDate >= input.MinSubmitDateFilter)
                        .WhereIf(input.MaxSubmitDateFilter != null, e => e.SubmitDate <= input.MaxSubmitDateFilter);

            var query = (from o in filteredForcatses
                         select new GetForcatsForViewDto()
                         {
                             Forcats = new ForcatsDto
                             {
                                 UserName = o.UserName,
                                 TotalCommit = o.TotalCommit,
                                 TotalClosed = o.TotalClosed,
                                 SubmitDate = o.SubmitDate,
                                 Id = o.Id
                             }
                         });

            var forcatsListDtos = await query.ToListAsync();

            return _forcatsesExcelExporter.ExportToFile(forcatsListDtos);
        }

    }
}
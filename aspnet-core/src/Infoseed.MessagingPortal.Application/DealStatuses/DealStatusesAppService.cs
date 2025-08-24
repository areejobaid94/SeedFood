//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Abp.Domain.Repositories;
//using Infoseed.MessagingPortal.DealStatuses.Exporting;
//using Infoseed.MessagingPortal.DealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Authorization;
//using Abp.Extensions;
//using Abp.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Abp.UI;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.DealStatuses
//{
//    [AbpAuthorize(AppPermissions.Pages_DealStatuses)]
//    public class DealStatusesAppService : MessagingPortalAppServiceBase, IDealStatusesAppService
//    {
//        private readonly IRepository<DealStatus> _dealStatusRepository;
//        private readonly IDealStatusesExcelExporter _dealStatusesExcelExporter;

//        public DealStatusesAppService(IRepository<DealStatus> dealStatusRepository, IDealStatusesExcelExporter dealStatusesExcelExporter)
//        {
//            _dealStatusRepository = dealStatusRepository;
//            _dealStatusesExcelExporter = dealStatusesExcelExporter;

//        }

//        public async Task<PagedResultDto<GetDealStatusForViewDto>> GetAll(GetAllDealStatusesInput input)
//        {

//            var filteredDealStatuses = _dealStatusRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Status.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusFilter), e => e.Status == input.DealStatusFilter);

//            var pagedAndFilteredDealStatuses = filteredDealStatuses
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var dealStatuses = from o in pagedAndFilteredDealStatuses
//                               select new
//                               {

//                                   o.Status,
//                                   Id = o.Id
//                               };

//            var totalCount = await filteredDealStatuses.CountAsync();

//            var dbList = await dealStatuses.ToListAsync();
//            var results = new List<GetDealStatusForViewDto>();

//            foreach (var o in dbList)
//            {
//                var res = new GetDealStatusForViewDto()
//                {
//                    DealStatus = new DealStatusDto
//                    {

//                        DealStatus = o.Status,
//                        Id = o.Id,
//                    }
//                };

//                results.Add(res);
//            }

//            return new PagedResultDto<GetDealStatusForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetDealStatusForViewDto> GetDealStatusForView(int id)
//        {
//            var dealStatus = await _dealStatusRepository.GetAsync(id);

//            var output = new GetDealStatusForViewDto { DealStatus = ObjectMapper.Map<DealStatusDto>(dealStatus) };

//            return output;
//        }

//        [AbpAuthorize(AppPermissions.Pages_DealStatuses_Edit)]
//        public async Task<GetDealStatusForEditOutput> GetDealStatusForEdit(EntityDto input)
//        {
//            var dealStatus = await _dealStatusRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetDealStatusForEditOutput { DealStatus = ObjectMapper.Map<CreateOrEditDealStatusDto>(dealStatus) };

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditDealStatusDto input)
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

//        [AbpAuthorize(AppPermissions.Pages_DealStatuses_Create)]
//        protected virtual async Task Create(CreateOrEditDealStatusDto input)
//        {
//            var dealStatus = ObjectMapper.Map<DealStatus>(input);

//            await _dealStatusRepository.InsertAsync(dealStatus);

//        }

//        [AbpAuthorize(AppPermissions.Pages_DealStatuses_Edit)]
//        protected virtual async Task Update(CreateOrEditDealStatusDto input)
//        {
//            var dealStatus = await _dealStatusRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, dealStatus);

//        }

//        [AbpAuthorize(AppPermissions.Pages_DealStatuses_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _dealStatusRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetDealStatusesToExcel(GetAllDealStatusesForExcelInput input)
//        {

//            var filteredDealStatuses = _dealStatusRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Status.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.DealStatusFilter), e => e.Status == input.DealStatusFilter);

//            var query = (from o in filteredDealStatuses
//                         select new GetDealStatusForViewDto()
//                         {
//                             DealStatus = new DealStatusDto
//                             {
//                                 DealStatus = o.Status,
//                                 Id = o.Id
//                             }
//                         });

//            var dealStatusListDtos = await query.ToListAsync();

//            return _dealStatusesExcelExporter.ExportToFile(dealStatusListDtos);
//        }

//    }
//}
//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Abp.Domain.Repositories;
//using Infoseed.MessagingPortal.CloseDealStatuses.Exporting;
//using Infoseed.MessagingPortal.CloseDealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Authorization;
//using Abp.Extensions;
//using Abp.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Abp.UI;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.CloseDealStatuses
//{
//    [AbpAuthorize(AppPermissions.Pages_CloseDealStatuses)]
//    public class CloseDealStatusesAppService : MessagingPortalAppServiceBase, ICloseDealStatusesAppService
//    {
//        private readonly IRepository<CloseDealStatus> _closeDealStatusRepository;
//        private readonly ICloseDealStatusesExcelExporter _closeDealStatusesExcelExporter;

//        public CloseDealStatusesAppService(IRepository<CloseDealStatus> closeDealStatusRepository, ICloseDealStatusesExcelExporter closeDealStatusesExcelExporter)
//        {
//            _closeDealStatusRepository = closeDealStatusRepository;
//            _closeDealStatusesExcelExporter = closeDealStatusesExcelExporter;

//        }

//        public async Task<PagedResultDto<GetCloseDealStatusForViewDto>> GetAll(GetAllCloseDealStatusesInput input)
//        {

//            var filteredCloseDealStatuses = _closeDealStatusRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Status.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter), e => e.Status == input.StatusFilter);

//            var pagedAndFilteredCloseDealStatuses = filteredCloseDealStatuses
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var closeDealStatuses = from o in pagedAndFilteredCloseDealStatuses
//                                    select new
//                                    {

//                                        o.Status,
//                                        Id = o.Id
//                                    };

//            var totalCount = await filteredCloseDealStatuses.CountAsync();

//            var dbList = await closeDealStatuses.ToListAsync();
//            var results = new List<GetCloseDealStatusForViewDto>();

//            foreach (var o in dbList)
//            {
//                var res = new GetCloseDealStatusForViewDto()
//                {
//                    CloseDealStatus = new CloseDealStatusDto
//                    {

//                        Status = o.Status,
//                        Id = o.Id,
//                    }
//                };

//                results.Add(res);
//            }

//            return new PagedResultDto<GetCloseDealStatusForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetCloseDealStatusForViewDto> GetCloseDealStatusForView(int id)
//        {
//            var closeDealStatus = await _closeDealStatusRepository.GetAsync(id);

//            var output = new GetCloseDealStatusForViewDto { CloseDealStatus = ObjectMapper.Map<CloseDealStatusDto>(closeDealStatus) };

//            return output;
//        }

//        [AbpAuthorize(AppPermissions.Pages_CloseDealStatuses_Edit)]
//        public async Task<GetCloseDealStatusForEditOutput> GetCloseDealStatusForEdit(EntityDto input)
//        {
//            var closeDealStatus = await _closeDealStatusRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetCloseDealStatusForEditOutput { CloseDealStatus = ObjectMapper.Map<CreateOrEditCloseDealStatusDto>(closeDealStatus) };

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditCloseDealStatusDto input)
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

//        [AbpAuthorize(AppPermissions.Pages_CloseDealStatuses_Create)]
//        protected virtual async Task Create(CreateOrEditCloseDealStatusDto input)
//        {
//            var closeDealStatus = ObjectMapper.Map<CloseDealStatus>(input);

//            await _closeDealStatusRepository.InsertAsync(closeDealStatus);

//        }

//        [AbpAuthorize(AppPermissions.Pages_CloseDealStatuses_Edit)]
//        protected virtual async Task Update(CreateOrEditCloseDealStatusDto input)
//        {
//            var closeDealStatus = await _closeDealStatusRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, closeDealStatus);

//        }

//        [AbpAuthorize(AppPermissions.Pages_CloseDealStatuses_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _closeDealStatusRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetCloseDealStatusesToExcel(GetAllCloseDealStatusesForExcelInput input)
//        {

//            var filteredCloseDealStatuses = _closeDealStatusRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Status.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter), e => e.Status == input.StatusFilter);

//            var query = (from o in filteredCloseDealStatuses
//                         select new GetCloseDealStatusForViewDto()
//                         {
//                             CloseDealStatus = new CloseDealStatusDto
//                             {
//                                 Status = o.Status,
//                                 Id = o.Id
//                             }
//                         });

//            var closeDealStatusListDtos = await query.ToListAsync();

//            return _closeDealStatusesExcelExporter.ExportToFile(closeDealStatusListDtos);
//        }

//    }
//}
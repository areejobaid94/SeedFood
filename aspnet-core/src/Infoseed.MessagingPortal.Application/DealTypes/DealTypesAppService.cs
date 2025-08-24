//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Abp.Domain.Repositories;
//using Infoseed.MessagingPortal.DealTypes.Exporting;
//using Infoseed.MessagingPortal.DealTypes.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Authorization;
//using Abp.Extensions;
//using Abp.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Abp.UI;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.DealTypes
//{
//   // [AbpAuthorize(AppPermissions.Pages_DealTypes)]
//    public class DealTypesAppService : MessagingPortalAppServiceBase, IDealTypesAppService
//    {
//        private readonly IRepository<DealType> _dealTypeRepository;
//        private readonly IDealTypesExcelExporter _dealTypesExcelExporter;

//        public DealTypesAppService(IRepository<DealType> dealTypeRepository, IDealTypesExcelExporter dealTypesExcelExporter)
//        {
//            _dealTypeRepository = dealTypeRepository;
//            _dealTypesExcelExporter = dealTypesExcelExporter;

//        }

//        public async Task<PagedResultDto<GetDealTypeForViewDto>> GetAll(GetAllDealTypesInput input)
//        {

//            var filteredDealTypes = _dealTypeRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Deal_Type.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Deal_TypeFilter), e => e.Deal_Type == input.Deal_TypeFilter);

//            var pagedAndFilteredDealTypes = filteredDealTypes
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var dealTypes = from o in pagedAndFilteredDealTypes
//                            select new
//                            {

//                                o.Deal_Type,
//                                Id = o.Id
//                            };

//            var totalCount = await filteredDealTypes.CountAsync();

//            var dbList = await dealTypes.ToListAsync();
//            var results = new List<GetDealTypeForViewDto>();

//            foreach (var o in dbList)
//            {
//                var res = new GetDealTypeForViewDto()
//                {
//                    DealType = new DealTypeDto
//                    {

//                        Deal_Type = o.Deal_Type,
//                        Id = o.Id,
//                    }
//                };

//                results.Add(res);
//            }

//            return new PagedResultDto<GetDealTypeForViewDto>(
//                totalCount,
//                results
//            );

//        }

//        public async Task<GetDealTypeForViewDto> GetDealTypeForView(int id)
//        {
//            var dealType = await _dealTypeRepository.GetAsync(id);

//            var output = new GetDealTypeForViewDto { DealType = ObjectMapper.Map<DealTypeDto>(dealType) };

//            return output;
//        }

//       // [AbpAuthorize(AppPermissions.Pages_DealTypes_Edit)]
//        public async Task<GetDealTypeForEditOutput> GetDealTypeForEdit(EntityDto input)
//        {
//            var dealType = await _dealTypeRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetDealTypeForEditOutput { DealType = ObjectMapper.Map<CreateOrEditDealTypeDto>(dealType) };

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditDealTypeDto input)
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

//       // [AbpAuthorize(AppPermissions.Pages_DealTypes_Create)]
//        protected virtual async Task Create(CreateOrEditDealTypeDto input)
//        {
//            var dealType = ObjectMapper.Map<DealType>(input);

//            if (AbpSession.TenantId != null)
//            {
//                dealType.TenantId = (int?)AbpSession.TenantId;
//            }

//            await _dealTypeRepository.InsertAsync(dealType);

//        }

//       // [AbpAuthorize(AppPermissions.Pages_DealTypes_Edit)]
//        protected virtual async Task Update(CreateOrEditDealTypeDto input)
//        {
//            var dealType = await _dealTypeRepository.FirstOrDefaultAsync((int)input.Id);
//            ObjectMapper.Map(input, dealType);

//        }

//       // [AbpAuthorize(AppPermissions.Pages_DealTypes_Delete)]
//        public async Task Delete(EntityDto input)
//        {
//            await _dealTypeRepository.DeleteAsync(input.Id);
//        }

//        public async Task<FileDto> GetDealTypesToExcel(GetAllDealTypesForExcelInput input)
//        {

//            var filteredDealTypes = _dealTypeRepository.GetAll()
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Deal_Type.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Deal_TypeFilter), e => e.Deal_Type == input.Deal_TypeFilter);

//            var query = (from o in filteredDealTypes
//                         select new GetDealTypeForViewDto()
//                         {
//                             DealType = new DealTypeDto
//                             {
//                                 Deal_Type = o.Deal_Type,
//                                 Id = o.Id
//                             }
//                         });

//            var dealTypeListDtos = await query.ToListAsync();

//            return _dealTypesExcelExporter.ExportToFile(dealTypeListDtos);
//        }

//    }
//}
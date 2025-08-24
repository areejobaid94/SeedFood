

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Genders.Exporting;
using Infoseed.MessagingPortal.Genders.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Genders
{
	[AbpAuthorize(AppPermissions.Pages_Genders)]
    public class GendersAppService : MessagingPortalAppServiceBase, IGendersAppService
    {
		 private readonly IRepository<Gender, long> _genderRepository;
		 private readonly IGendersExcelExporter _gendersExcelExporter;
		 

		  public GendersAppService(IRepository<Gender, long> genderRepository, IGendersExcelExporter gendersExcelExporter ) 
		  {
			_genderRepository = genderRepository;
			_gendersExcelExporter = gendersExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetGenderForViewDto>> GetAll(GetAllGendersInput input)
         {
			
			var filteredGenders = _genderRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var pagedAndFilteredGenders = filteredGenders
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var genders = from o in pagedAndFilteredGenders
                         select new GetGenderForViewDto() {
							Gender = new GenderDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						};

            var totalCount = await filteredGenders.CountAsync();

            return new PagedResultDto<GetGenderForViewDto>(
                totalCount,
                await genders.ToListAsync()
            );
         }
		 
		 public async Task<GetGenderForViewDto> GetGenderForView(long id)
         {
            var gender = await _genderRepository.GetAsync(id);

            var output = new GetGenderForViewDto { Gender = ObjectMapper.Map<GenderDto>(gender) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Genders_Edit)]
		 public async Task<GetGenderForEditOutput> GetGenderForEdit(EntityDto<long> input)
         {
            var gender = await _genderRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetGenderForEditOutput {Gender = ObjectMapper.Map<CreateOrEditGenderDto>(gender)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditGenderDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Genders_Create)]
		 protected virtual async Task Create(CreateOrEditGenderDto input)
         {
            var gender = ObjectMapper.Map<Gender>(input);

			
			if (AbpSession.TenantId != null)
			{
				gender.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _genderRepository.InsertAsync(gender);
         }

		 [AbpAuthorize(AppPermissions.Pages_Genders_Edit)]
		 protected virtual async Task Update(CreateOrEditGenderDto input)
         {
            var gender = await _genderRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, gender);
         }

		 [AbpAuthorize(AppPermissions.Pages_Genders_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _genderRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetGendersToExcel(GetAllGendersForExcelInput input)
         {
			
			var filteredGenders = _genderRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var query = (from o in filteredGenders
                         select new GetGenderForViewDto() { 
							Gender = new GenderDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						 });


            var genderListDtos = await query.ToListAsync();

            return _gendersExcelExporter.ExportToFile(genderListDtos);
         }


    }
}
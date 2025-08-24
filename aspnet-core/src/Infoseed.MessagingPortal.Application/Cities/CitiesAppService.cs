

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Infoseed.MessagingPortal.Cities.Exporting;
using Infoseed.MessagingPortal.Cities.Dtos;
using Infoseed.MessagingPortal.Dto;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Infoseed.MessagingPortal.Cities
{
	[AbpAuthorize(AppPermissions.Pages_Cities)]
    public class CitiesAppService : MessagingPortalAppServiceBase, ICitiesAppService
    {
		 private readonly IRepository<City, long> _cityRepository;
		 private readonly ICitiesExcelExporter _citiesExcelExporter;
		 

		  public CitiesAppService(IRepository<City, long> cityRepository, ICitiesExcelExporter citiesExcelExporter ) 
		  {
			_cityRepository = cityRepository;
			_citiesExcelExporter = citiesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetCityForViewDto>> GetAll(GetAllCitiesInput input)
         {
			
			var filteredCities = _cityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var pagedAndFilteredCities = filteredCities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var cities = from o in pagedAndFilteredCities
                         select new GetCityForViewDto() {
							City = new CityDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCities.CountAsync();

            return new PagedResultDto<GetCityForViewDto>(
                totalCount,
                await cities.ToListAsync()
            );
         }
		 
		 public async Task<GetCityForViewDto> GetCityForView(long id)
         {
            var city = await _cityRepository.GetAsync(id);

            var output = new GetCityForViewDto { City = ObjectMapper.Map<CityDto>(city) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Cities_Edit)]
		 public async Task<GetCityForEditOutput> GetCityForEdit(EntityDto<long> input)
         {
            var city = await _cityRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCityForEditOutput {City = ObjectMapper.Map<CreateOrEditCityDto>(city)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCityDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Cities_Create)]
		 protected virtual async Task Create(CreateOrEditCityDto input)
         {
            var city = ObjectMapper.Map<City>(input);

			
			if (AbpSession.TenantId != null)
			{
				city.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _cityRepository.InsertAsync(city);
         }

		 [AbpAuthorize(AppPermissions.Pages_Cities_Edit)]
		 protected virtual async Task Update(CreateOrEditCityDto input)
         {
            var city = await _cityRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, city);
         }

		 [AbpAuthorize(AppPermissions.Pages_Cities_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _cityRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetCitiesToExcel(GetAllCitiesForExcelInput input)
         {
			
			var filteredCities = _cityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter);

			var query = (from o in filteredCities
                         select new GetCityForViewDto() { 
							City = new CityDto
							{
                                Name = o.Name,
                                Id = o.Id
							}
						 });


            var cityListDtos = await query.ToListAsync();

            return _citiesExcelExporter.ExportToFile(cityListDtos);
         }


    }
}
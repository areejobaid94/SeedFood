using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Cities.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.Cities
{
    public interface ICitiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCityForViewDto>> GetAll(GetAllCitiesInput input);

        Task<GetCityForViewDto> GetCityForView(long id);

		Task<GetCityForEditOutput> GetCityForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditCityDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetCitiesToExcel(GetAllCitiesForExcelInput input);

		
    }
}
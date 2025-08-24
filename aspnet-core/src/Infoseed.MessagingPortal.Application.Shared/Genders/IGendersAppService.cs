using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Genders.Dtos;
using Infoseed.MessagingPortal.Dto;


namespace Infoseed.MessagingPortal.Genders
{
    public interface IGendersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetGenderForViewDto>> GetAll(GetAllGendersInput input);

        Task<GetGenderForViewDto> GetGenderForView(long id);

		Task<GetGenderForEditOutput> GetGenderForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditGenderDto input);

		Task Delete(EntityDto<long> input);

		Task<FileDto> GetGendersToExcel(GetAllGendersForExcelInput input);

		
    }
}
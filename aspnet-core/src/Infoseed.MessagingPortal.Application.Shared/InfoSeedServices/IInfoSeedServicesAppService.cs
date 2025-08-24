using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.InfoSeedServices.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.InfoSeedServices
{
    public interface IInfoSeedServicesAppService : IApplicationService
    {
        Task<PagedResultDto<GetInfoSeedServiceForViewDto>> GetAll(GetAllInfoSeedServicesInput input);

        Task<GetInfoSeedServiceForViewDto> GetInfoSeedServiceForView(int id);

        Task<GetInfoSeedServiceForEditOutput> GetInfoSeedServiceForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditInfoSeedServiceDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetInfoSeedServicesToExcel(GetAllInfoSeedServicesForExcelInput input);

        Task<PagedResultDto<InfoSeedServiceServiceTypeLookupTableDto>> GetAllServiceTypeForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<InfoSeedServiceServiceStatusLookupTableDto>> GetAllServiceStatusForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<InfoSeedServiceServiceFrquencyLookupTableDto>> GetAllServiceFrquencyForLookupTable(GetAllForLookupTableInput input);

    }
}
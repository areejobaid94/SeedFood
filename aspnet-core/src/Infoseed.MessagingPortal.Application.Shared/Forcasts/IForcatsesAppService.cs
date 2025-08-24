using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Forcasts.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Forcasts
{
    public interface IForcatsesAppService : IApplicationService
    {
        Task<PagedResultDto<GetForcatsForViewDto>> GetAll(GetAllForcatsesInput input);

        Task<GetForcatsForViewDto> GetForcatsForView(int id);

        Task<GetForcatsForEditOutput> GetForcatsForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditForcatsDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetForcatsesToExcel(GetAllForcatsesForExcelInput input);

    }
}
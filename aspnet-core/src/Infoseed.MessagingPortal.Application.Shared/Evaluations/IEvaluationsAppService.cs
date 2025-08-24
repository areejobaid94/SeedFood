using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Evaluations
{
    public interface IEvaluationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetEvaluationForViewDto>> GetAll(GetAllEvaluationsInput input);
        //Task<FileDto> GetEvaluationsToExcel();
        Task<GetEvaluationForViewDto> GetEvaluationForView(long id);

        Task<GetEvaluationForEditOutput> GetEvaluationForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditEvaluationDto input);

        Task Delete(EntityDto<long> input);
        Task DeleteAll();
    }
}

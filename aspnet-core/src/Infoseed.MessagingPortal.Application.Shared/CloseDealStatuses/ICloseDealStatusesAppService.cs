//using System;
//using System.Threading.Tasks;
//using Abp.Application.Services;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.CloseDealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;

//namespace Infoseed.MessagingPortal.CloseDealStatuses
//{
//    public interface ICloseDealStatusesAppService : IApplicationService
//    {
//        Task<PagedResultDto<GetCloseDealStatusForViewDto>> GetAll(GetAllCloseDealStatusesInput input);

//        Task<GetCloseDealStatusForViewDto> GetCloseDealStatusForView(int id);

//        Task<GetCloseDealStatusForEditOutput> GetCloseDealStatusForEdit(EntityDto input);

//        Task CreateOrEdit(CreateOrEditCloseDealStatusDto input);

//        Task Delete(EntityDto input);

//        Task<FileDto> GetCloseDealStatusesToExcel(GetAllCloseDealStatusesForExcelInput input);

//    }
//}
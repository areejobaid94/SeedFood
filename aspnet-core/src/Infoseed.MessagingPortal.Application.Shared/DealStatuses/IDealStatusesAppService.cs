//using System;
//using System.Threading.Tasks;
//using Abp.Application.Services;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.DealStatuses.Dtos;
//using Infoseed.MessagingPortal.Dto;

//namespace Infoseed.MessagingPortal.DealStatuses
//{
//    public interface IDealStatusesAppService : IApplicationService
//    {
//        Task<PagedResultDto<GetDealStatusForViewDto>> GetAll(GetAllDealStatusesInput input);

//        Task<GetDealStatusForViewDto> GetDealStatusForView(int id);

//        Task<GetDealStatusForEditOutput> GetDealStatusForEdit(EntityDto input);

//        Task CreateOrEdit(CreateOrEditDealStatusDto input);

//        Task Delete(EntityDto input);

//        Task<FileDto> GetDealStatusesToExcel(GetAllDealStatusesForExcelInput input);

//    }
//}
//using System;
//using System.Threading.Tasks;
//using Abp.Application.Services;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.DealTypes.Dtos;
//using Infoseed.MessagingPortal.Dto;

//namespace Infoseed.MessagingPortal.DealTypes
//{
//    public interface IDealTypesAppService : IApplicationService
//    {
//        Task<PagedResultDto<GetDealTypeForViewDto>> GetAll(GetAllDealTypesInput input);

//        Task<GetDealTypeForViewDto> GetDealTypeForView(int id);

//        Task<GetDealTypeForEditOutput> GetDealTypeForEdit(EntityDto input);

//        Task CreateOrEdit(CreateOrEditDealTypeDto input);

//        Task Delete(EntityDto input);

//        Task<FileDto> GetDealTypesToExcel(GetAllDealTypesForExcelInput input);

//    }
//}
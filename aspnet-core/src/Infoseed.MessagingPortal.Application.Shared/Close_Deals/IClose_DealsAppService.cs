//using System;
//using System.Threading.Tasks;
//using Abp.Application.Services;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Close_Deals.Dtos;
//using Infoseed.MessagingPortal.Dto;

//namespace Infoseed.MessagingPortal.Close_Deals
//{
//    public interface IClose_DealsAppService : IApplicationService
//    {
//        Task<PagedResultDto<GetClose_DealForViewDto>> GetAll(GetAllClose_DealsInput input);

//        Task<GetClose_DealForViewDto> GetClose_DealForView(int id);

//        Task<GetClose_DealForEditOutput> GetClose_DealForEdit(EntityDto input);

//        Task CreateOrEdit(CreateOrEditClose_DealDto input);

//        Task Delete(EntityDto input);

//        Task<FileDto> GetClose_DealsToExcel(GetAllClose_DealsForExcelInput input);

//        Task<PagedResultDto<Close_DealCloseDealStatusLookupTableDto>> GetAllCloseDealStatusForLookupTable(GetAllForLookupTableInput input);

//    }
//}
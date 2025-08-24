//using System;
//using System.Threading.Tasks;
//using Abp.Application.Services;
//using Abp.Application.Services.Dto;
//using Infoseed.MessagingPortal.Deals.Dtos;
//using Infoseed.MessagingPortal.Dto;

//namespace Infoseed.MessagingPortal.Deals
//{
//    public interface IDealsAppService : IApplicationService
//    {
//        Task<PagedResultDto<GetDealForViewDto>> GetAll(GetAllDealsInput input);
//        Task<PagedResultDto<GetDealForViewDto>> SubmitDeal(string username);

//        Task<GetDealForViewDto> GetDealForView(int id);

//        Task<GetDealForEditOutput> GetDealForEdit(EntityDto input);

//        Task CreateOrEdit(CreateOrEditDealDto input);

//        Task Delete(EntityDto input);

//        Task<FileDto> GetDealsToExcel(GetAllDealsForExcelInput input);

//        Task<PagedResultDto<DealDealStatusLookupTableDto>> GetAllDealStatusForLookupTable(GetAllForLookupTableInput input);

//        Task<PagedResultDto<DealDealTypeLookupTableDto>> GetAllDealTypeForLookupTable(GetAllForLookupTableInput input);
//        Task<PagedResultDto<TerritoryLookupTableDto>> GetAllTerritoryForLookupTable(GetAllForLookupTableInput input);
//    }
//}
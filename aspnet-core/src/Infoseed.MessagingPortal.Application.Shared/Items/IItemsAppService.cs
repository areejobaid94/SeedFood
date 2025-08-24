using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Items
{
    public interface IItemsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetItemForViewDto>> GetAll(GetAllItemsInput input);
        Task<GetItemForViewDto> GetItemForView(long id);
		Task<GetItemForEditOutput> GetItemForEdit(EntityDto<long> input);
		Task<long> CreateOrEdit(CreateOrEditItemDto input);
        Task<long> CreateOrEditItems(CreateOrEditItemDto input);
        bool DeleteItem(long itemId);
        Task<FileDto> GetItemsToExcel(GetAllItemsForExcelInput input);
        //List<ItemDto> GetAllWithTenantID(int? TenantID, int menuID);
        List<ItemDto> GetItemsByMenuID(long menuID, long ItemCategoryId=0, long ItemSubCategoryId=0, int pageNumber=0, int pageSize=20);
        List<ItemDto> GetItemsBySubGategory(int tenantID, int menuType, long itemSubCategoryId, int languageBotId, int pageNumber , int pageSize , out int totalCount , string Search = "");
        ItemDto GetItemById(long id, int? tenantID = null, bool isFromMenu = false);
        List<ItemDto> GetLoyaltyItems(int tenantID, int menuType, int languageBotId, int pageNumber, int pageSize, out int totalCount, string Search = "");
        ItemDto GetItemInfoForBot(long id, int tenantID);
        List<ItemDto> GetItemCTown(int TenantID, int menu, long ItemSubCategoryId, out int totalCount,int PageSize = 20, int PageNumber = 0, string Search = "", int IsSort = 0, int? OrderByPrice = null, int? OrderByDiscount = null, int IsDescOrder = 0, bool isvisible = false);
        FileDto ExportItemsToExcel(long categoryId, long subCategoryId);
    }
}
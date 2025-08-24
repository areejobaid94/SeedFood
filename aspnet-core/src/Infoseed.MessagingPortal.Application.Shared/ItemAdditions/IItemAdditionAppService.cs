using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ItemAdditions.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ItemAdditions
{
    public interface IItemAdditionAppService : IApplicationService
    {
        Task<PagedResultDto<GetItemAdditionForViewDto>> GetAll(GetAllItemAdditionInput input);
        Task<GetItemAdditionForViewDto> GetItemCategoryForView(long id);
        Task<GetItemAdditionForEditOutput> GetItemCategoryForEdit(EntityDto<long> input);
        Task<long> CreateOrEdit(CreateOrEditItemAdditionDto input);
        //Task Delete(EntityDto<long> input);
        //List<ItemAdditionDto> GetAllWithTenantID(int? TenantID);
        Task<List<ItemAdditionDto>> GetCondiments(int tenantID, int menuType);
        Task<List<ItemAdditionDto>> GetCrispy(int tenantID, int menuType);
        Task<List<ItemAdditionDto>> GetDeserts(int tenantID, int menuType);
        long DeleteItemAdditionCategory(long input);
        bool DeleteItemAddition(long id);
        bool DeleteSpecification(int id);

    }
}
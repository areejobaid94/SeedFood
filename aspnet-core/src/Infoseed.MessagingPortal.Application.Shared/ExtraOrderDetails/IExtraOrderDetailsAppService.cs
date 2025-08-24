using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.ExtraOrderDetails.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.ExtraOrderDetails
{
    public interface IExtraOrderDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetExtraOrderDetailsForViewDto>> GetAll(GetAllExtraOrderDetailsInput input);

        Task<GetExtraOrderDetailsForViewDto> GetExtraOrderDetailsForView(long id);

        Task<GetExtraOrderDetailsForEditOutput> GetExtraOrderDetailsForEdit(EntityDto<long> input);

        Task<long> CreateOrEdit(CreateOrEditExtraOrderDetailsDto input);

        Task Delete(EntityDto<long> input);

        //Task<FileDto> GetItemCategoryToExcel(GetAllMenuCategoriesForExcelInput input);

        List<ExtraOrderDetailsDto> GetAllWithTenantID(int? TenantID);
    }
}
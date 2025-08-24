using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.SalesUserCreate.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.SalesUserCreate
{
    public interface ISalesUserCreateAppService : IApplicationService
    {
        Task<PagedResultDto<GetSalesUserCreateForViewDto>> GetAll(GetAllSalesUserCreateInput input);

        Task<GetSalesUserCreateForViewDto> GetSalesUserCreateForView(int id);

        Task<GetSalesUserCreateForEditOutput> GetSalesUserCreateForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditSalesUserCreateDto input);

        Task Delete(EntityDto input);

    }
}

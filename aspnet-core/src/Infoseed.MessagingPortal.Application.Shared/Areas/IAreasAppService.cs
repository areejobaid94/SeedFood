using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;

namespace Infoseed.MessagingPortal.Areas
{
    public interface IAreasAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAreaForViewDto>> GetAll(GetAllAreasInput input);

        Task<GetAreaForViewDto> GetAreaForView(long id);

        Task<GetAreaForEditOutput> GetAreaForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditAreaDto input);

        Task Delete(EntityDto<long> input);

        Task<FileDto> GetAreasToExcel(GetAllAreasForExcelInput input);
        LocationInfoModelDto GetLocationDeliveryCost(int tenantID, string JsonLocation);
        List<AreaDto> GetAllAreas(int tenantID, bool? isAvailableBranch = null);
        AreasEntity GetAreas(int pageNumber = 0, int pageSize = 50);
        AreaDto GetAreaById(int id, int tenantID);
        WorkModel GetMenuSetting(long BranchId);
        void SaveSetting(long BranchId, WorkModel workModel);
        List<AreaDto> GetAvailableAreas(int tenantId);
        string DeleteArea(long areaId);
    }
}
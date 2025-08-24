using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Location.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Location
{
    public interface ILocationAppService : IApplicationService
    {
        List<LocationModel> GetRootLocations();
        List<LocationModel> GetCountryLocation();
        List<LocationModel> GetLocationsByParentId(int parentId);
        PagedResultDto<LocationModel> GetAllLocations(int SkipCount, int MaxResultCount, string Sorting, int? tenantId = null, int? cityId = null, int? areaId = null);
        int CreateOrUpdateLocation(LocationModel model);
        void CreateDefaultLocation(int tenantId);
    }
}

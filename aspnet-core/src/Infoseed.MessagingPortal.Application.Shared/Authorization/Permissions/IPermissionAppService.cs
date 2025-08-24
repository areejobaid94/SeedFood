using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();

        List<string> GetUserPermissions(long id, int? tenantId);
    }
}

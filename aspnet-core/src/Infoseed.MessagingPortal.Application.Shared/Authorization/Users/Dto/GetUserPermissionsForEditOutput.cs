using System.Collections.Generic;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
using System.Collections.Generic;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;

namespace Infoseed.MessagingPortal.Authorization.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Framework.Data;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;

namespace Infoseed.MessagingPortal.Authorization.Permissions
{
    public class PermissionAppService : MessagingPortalAppServiceBase, IPermissionAppService
    {
        public ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions()
        {
            var permissions = PermissionManager.GetAllPermissions();
            var rootPermissions = permissions.Where(p => p.Parent == null);

            var result = new List<FlatPermissionWithLevelDto>();

            foreach (var rootPermission in rootPermissions)
            {
                var level = 0;
                AddPermission(rootPermission, permissions, result, level);
            }

            return new ListResultDto<FlatPermissionWithLevelDto>
            {
                Items = result
            };
        }

        public List<string> GetUserPermissions(long id, int? tenantId)
        {
            try
            {
                var SP_Name = Constants.PermissionsUser.SP_PermissionsUser;

                var sqlParameters = new List<SqlParameter>
                {
                   new SqlParameter("@tenantId",tenantId),
                   new SqlParameter("@id",id)
                 };

                List<string> PermissionsDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.ConvertToGetPermissions, AppSettingsModel.ConnectionStrings).ToList();


                return PermissionsDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddPermission(Permission permission, IReadOnlyList<Permission> allPermissions, List<FlatPermissionWithLevelDto> result, int level)
        {
            var flatPermission = ObjectMapper.Map<FlatPermissionWithLevelDto>(permission);
            flatPermission.Level = level;
            result.Add(flatPermission);

            if (permission.Children == null)
            {
                return;
            }

            var children = allPermissions.Where(p => p.Parent != null && p.Parent.Name == permission.Name).ToList();

            foreach (var childPermission in children)
            {
                AddPermission(childPermission, allPermissions, result, level + 1);
            }
        }
    }
}
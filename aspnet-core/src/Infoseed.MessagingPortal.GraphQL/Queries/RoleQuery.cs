using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Core.Base;
using Infoseed.MessagingPortal.Core.Extensions;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Types;

namespace Infoseed.MessagingPortal.Queries
{
    public class RoleQuery : MessagingPortalQueryBase<ListGraphType<RoleType>, List<RoleDto>>
    {
        private readonly RoleManager _roleManager;

        public static class Args
        {
            public const string Id = "id";
            public const string TenantId = "tenantId";
            public const string Name = "name";
        }

        public RoleQuery(RoleManager roleManager)
            : base("roles", new Dictionary<string, Type>
                {
                    {Args.Id, typeof(IdGraphType)},
                    {Args.TenantId, typeof(IntGraphType)},
                    {Args.Name, typeof(StringGraphType)}
                }
            )
        {
            _roleManager = roleManager;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Roles)]
        protected override async Task<List<RoleDto>> Resolve(ResolveFieldContext<object> context)
        {
            var query = _roleManager.Roles.AsNoTracking();

            context
                .ContainsArgument<int>(Args.Id, id => query = query.Where(r => r.Id == id))
                .ContainsArgument<string>(Args.TenantId, name => query = query.Where(r => r.Name == name))
                .ContainsArgument<int?>(Args.Name, tenantId => query = query.Where(r => r.TenantId == tenantId.Value));

            return await ProjectToListAsync<RoleDto>(query);
        }
    }
}
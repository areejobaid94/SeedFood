using Infoseed.MessagingPortal.TemplateMessages;
using System;
using System.Linq;
using Abp.Organizations;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.MultiTenancy;

namespace Infoseed.MessagingPortal.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(TemplateMessage),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(TemplateMessage),
            typeof(OrganizationUnit), typeof(Role)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}
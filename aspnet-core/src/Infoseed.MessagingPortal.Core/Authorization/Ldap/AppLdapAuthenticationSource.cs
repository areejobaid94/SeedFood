using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.MultiTenancy;

namespace Infoseed.MessagingPortal.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}
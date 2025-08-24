using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Infoseed.MessagingPortal.Authentication.TwoFactor.Google;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.MultiTenancy;

namespace Infoseed.MessagingPortal.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddAbpIdentity<Tenant, User, Role>(options =>
                {
                    options.Tokens.ProviderMap[GoogleAuthenticatorProvider.Name] = new TokenProviderDescriptor(typeof(GoogleAuthenticatorProvider));
                })
                .AddAbpTenantManager<TenantManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}

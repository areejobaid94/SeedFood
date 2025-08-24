using Abp.Application.Services;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.MultiTenancy;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task UpdateLoyalty(LoyaltyModel input);

        CaptionDto GetCaptionById(int captionId);

        bool UpdateCaptionById(CaptionDto captions);

        Task ClearLogo();

        Task ClearCustomCss();
        string GetGoogleAuthUrl(int tenantId);
        Task<(string accessToken, string refreshToken)> GetGoogleAccessTokenAsync(string code, int tenantId);
        Task<string> RevokeGoogleAccess(int tenantId);
        GoogleSheetConfigDto GoogleSheetConfigGet(int? tenantId);


    }
}

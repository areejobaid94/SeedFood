using Abp.Auditing;
using Infoseed.MessagingPortal.Configuration.Dto;

namespace Infoseed.MessagingPortal.Configuration.Tenants.Dto
{
    public class TenantEmailSettingsEditDto : EmailSettingsEditDto
    {
        public bool UseHostDefaultEmailSettings { get; set; }
    }
}
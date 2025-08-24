using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Infoseed.MessagingPortal.Configuration.Dto;
using Infoseed.MessagingPortal.Configuration.Host.Dto;

namespace Infoseed.MessagingPortal.Install.Dto
{
    public class InstallDto
    {
        [Required]
        [DisableAuditing]
        public string ConnectionString { get; set; }

        [Required]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        [Required]
        public string WebSiteUrl { get; set; }

        public string ServerUrl { get; set; }

        [Required]
        public string DefaultLanguage { get; set; }

        public EmailSettingsEditDto SmtpSettings { get; set; }

        public HostBillingSettingsEditDto BillInfo { get; set; }
    }
}
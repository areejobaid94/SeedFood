using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Runtime.Security;

namespace Infoseed.MessagingPortal.Net.Emailing
{
    public class MessagingPortalSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        public MessagingPortalSmtpEmailSenderConfiguration(ISettingManager settingManager) : base(settingManager)
        {

        }

        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
    }
}
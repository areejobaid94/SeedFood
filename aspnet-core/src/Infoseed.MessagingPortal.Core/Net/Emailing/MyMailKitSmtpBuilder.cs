using Abp.MailKit;
using Abp.Net.Mail.Smtp;


namespace Infoseed.MessagingPortal.Net.Emailing
{
    public class MyMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        public MyMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration, IAbpMailKitConfiguration abpMailKitConfiguration)
            : base(smtpEmailSenderConfiguration, abpMailKitConfiguration)
        {
        }

        protected override void ConfigureClient(MailKit.Net.Smtp.SmtpClient client)
        {
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            base.ConfigureClient(client);
        }
    }
}

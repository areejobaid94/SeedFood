using System.Threading.Tasks;
using Infoseed.MessagingPortal.Security.Recaptcha;

namespace Infoseed.MessagingPortal.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}

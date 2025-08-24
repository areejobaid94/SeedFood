using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}
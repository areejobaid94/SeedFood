using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}

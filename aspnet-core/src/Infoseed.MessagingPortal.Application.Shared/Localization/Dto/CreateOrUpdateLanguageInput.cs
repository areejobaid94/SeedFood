using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}
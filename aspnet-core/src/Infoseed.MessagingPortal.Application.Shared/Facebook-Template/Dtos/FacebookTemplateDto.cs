using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class FacebookTemplateDto
    {
        public TemplateNameLanguageFacebook Language { get; set; }
        public TemplateContent content { get; set; }
        public ButtonsTemplate buttons { get; set; }
        public CatalogFormat catalogFormat { get; set; }
        public TemplateContentAuthentication templateContentAuthentication { get; set; }
        public AppSetup appSetup { get; set; }
        public ButtonAuthontcation buttonAuthontcation { get; set; }
        public MessageValidityPeriod messageValidityPeriod { get; set; }
        public CodeDeliverySetupDTO codeDeliverySetupDTO { get; set; }

    }
}

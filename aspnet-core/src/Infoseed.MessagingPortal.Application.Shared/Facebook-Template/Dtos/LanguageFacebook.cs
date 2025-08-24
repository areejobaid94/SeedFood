using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class TemplateNameLanguageFacebook
    {
        public string TemplateName { get; set; }
        public  Language Language { get; }

    }
   public enum Language{
    English,
    Arabic
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models.BotModel
{
    public class TranslatorModel
    {

           public Detectedlanguage detectedLanguage { get; set; }
           public Translation[] translations { get; set; }
        

        public class Detectedlanguage
        {
            public string language { get; set; }
            public float score { get; set; }
        }

        public class Translation
        {
            public string text { get; set; }
            public string to { get; set; }
        }








    }
}

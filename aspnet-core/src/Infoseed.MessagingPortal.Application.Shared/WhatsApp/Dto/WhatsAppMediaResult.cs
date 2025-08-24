using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppMediaResult
    {
        public WhatsAppHeaderUrl header { get; set; }
        public WhatsAppSession session { get; set; }

        public class WhatsAppHeaderUrl
        {
            public string filename { get; set; }
            public string h { get; set; }
            public string InfoSeedUrl { get; set; }
        }

        public class WhatsAppSession
        {
            public string id { get; set; }
        }

        public class WhatsAppMediaID
        {
            public string id { get; set; }
          
        }
    }
}

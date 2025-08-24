using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.WhatsAppApi.Dto
{
    public class WhatsAppMediaResult
    {
        public WhatsAppHeaderUrl header { get; set; }
        public WhatsAppSession session { get; set; }

        public class WhatsAppHeaderUrl
        {
            public string h { get; set; }
            public string InfoSeedUrl { get; set; }
        }

        public class WhatsAppSession
        {
            public string id { get; set; }
        }

    }
}

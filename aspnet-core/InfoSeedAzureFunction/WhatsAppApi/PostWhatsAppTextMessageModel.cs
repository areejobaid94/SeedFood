using System;
using System.Collections.Generic;
using System.Text;
using static InfoSeedAzureFunction.WhatsAppApi.PostWhatsAppMessageModel;

namespace InfoSeedAzureFunction.WhatsAppApi
{
    public class PostWhatsAppTextMessageModel
    {


 

        public string messaging_product { get; set; }
        public string recipient_type { get; set; }
        public string type { get; set; }
        public string to { get; set; }
        public Text text { get; set; }




    }
}

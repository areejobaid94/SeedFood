using System;
using System.Collections.Generic;
using System.Text;
using static Infoseed.MessagingPortal.WhatsApp.Dto.PostWhatsAppMessageModel;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
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

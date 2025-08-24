
using static NewFunctionApp.PostWhatsAppMessageModel;

namespace NewFunctionApp
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

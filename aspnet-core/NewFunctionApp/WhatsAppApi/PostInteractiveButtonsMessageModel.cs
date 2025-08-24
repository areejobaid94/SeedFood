using System;
using System.Collections.Generic;
using System.Text;


namespace NewFunctionApp
{



    public class PostWhatsAppInteractiveButtonModel
    {
        public string messaging_product { get; set; }
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public InteractiveButtonsModel interactive { get; set; }
    }


    public class InteractiveButtonsModel
    {
        public string type { get; set; }
        public InteractiveBodyModel body { get; set; }
        public InteractiveFooterModel footer { get; set; }
        public InteractiveButtonActionModel action { get; set; }
    }


    public class InteractiveBodyModel
    {
        public string text { get; set; }
    }
     public class InteractiveFooterModel
    {
        public string text { get; set; }
    }



    public class InteractiveButtonActionModel
    {
        public List<InteractiveButtonModel> buttons { get; set; }
    }

    public class InteractiveButtonModel
    {
        public string type { get; set; }
        public InteractiveButtonsReplyModel reply { get; set; }
    }

    public class InteractiveButtonsReplyModel
    {
        public string id { get; set; }
        public string title { get; set; }
    }
}

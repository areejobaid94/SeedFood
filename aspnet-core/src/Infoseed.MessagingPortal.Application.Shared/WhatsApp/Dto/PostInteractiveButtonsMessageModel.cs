using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using static Infoseed.MessagingPortal.WhatsApp.Dto.PostWhatsAppMessageModel;
using static System.Net.Mime.MediaTypeNames;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
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
        public InteractiveHeaderModel header { get; set; }
        public InteractiveBodyModel body { get; set; }
        public InteractiveFooterModel footer { get; set; }
        public InteractiveButtonActionModel action { get; set; }
    }


    public class InteractiveHeaderModel
    {
        public string type { get; set; }
        public Interactiveimage image { get; set; }
        public WhatsAppVideoModel video { get; set; }

        public WhatsAppDocumentModel document { get; set; }

        public WhatsAppVoiceModel voice { get; set; }

    }
    public class InteractiveBodyModel
    {
        public string text { get; set; }
    }
     public class InteractiveFooterModel
    {
        public string text { get; set; }
    }

    public class WhatsAppVideoModel
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
    }

    public class WhatsAppDocumentModel
    {
        public string caption { get; set; }
        public string filename { get; set; }
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string link { get; set; }
    }
    public class WhatsAppVoiceModel
    {
        public string id { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string caption { get; set; }
        public string link { get; set; }
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
        public string image { get; set; }
    }

    public class Interactiveimage
    {
        public string link { get; set; }
    }


}

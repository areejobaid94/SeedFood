using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSendNew.WhatsAppApi.Dto
{

    public class PostMessageTemplateModel
    {
        public string messaging_product { get; set; }
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public WhatsAppTemplateModel template { get; set; }
    }

    public class WhatsAppTemplateModel
    {
        public string name { get; set; }
        public WhatsAppLanguageModel language { get; set; }
        public List<Component> components { get; set; }

    }
    public class WhatsAppLanguageModel
    {
        public string code { get; set; }
    }
    public class Component
    {
        public string type { get; set; }
        public string sub_type { get; set; }
        public int index { get; set; } = 0;
        public List<Parameter> parameters { get; set; }= new List<Parameter>();
    }

    public class Parameter
    {
        public string type { get; set; }
        public string text { get; set; }
        public ImageTemplate image { get; set; }
        public VideoTemplate video { get; set; }
        public DocumentTemplate document { get; set; }


    }
    public class ImageTemplate
    {
        public string link { get; set; }
    }
    public class VideoTemplate
    {
        public string link { get; set; }
    }
    public class DocumentTemplate
    {
        public string link { get; set; }
        public string filename { get; set; }
    }


    public class ParametersModel
    {
        public string type { get; set; }
        public string text { get; set; }
    }

}


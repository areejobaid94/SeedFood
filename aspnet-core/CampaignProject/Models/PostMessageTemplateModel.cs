using System.Collections.Generic;

namespace CampaignProject.Models
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
    public class CardComponent
    {
        public List<Component> components { get; set; }
        public  int card_index { get; set; }
}
    public class WhatsAppLanguageModel
    {
        public string code { get; set; }
    }
    public class Component
    {
        public string type { get; set; }
        public string sub_type { get; set; }
        public int? index { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<CardComponent> cards { get; set; }
        public string format { get; set; } // Add this for header format

    }

    public class Parameter
    {
        public string type { get; set; }
        public string coupon_code { get; set; }

        public string text { get; set; }
        public ImageTemplate image { get; set; }
        public VideoTemplate video { get; set; }
        public DocumentTemplate document { get; set; }
        public MediaIdTemplate mediaId { get; set; }
        public string payload { get; set; }


    }
    public class ImageTemplate
    {
        public string link { get; set; }
        public string id { get; set; }

    }
    public class VideoTemplate
    {
        public string link { get; set; }
        public string id { get; set; }

    }
    public class MediaIdTemplate
    {
        public string id { get; set; }
    }
    public class DocumentTemplate
    {
        public string link { get; set; }
        public string filename { get; set; }
    }

}


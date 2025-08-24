using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
    public class PostWhatsAppMessageModel
    {


        public string mediaUrl { get; set; }
        public string typeContent { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }

        public string from { get; set; }
        public string id { get; set; }
        public string messaging_product { get; set; }
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public Text text { get; set; }
        public Image image { get; set; }
        public Audio audio { get; set; }
        public Document document { get; set; }
        public Video video { get; set; }
        public Location location { get; set; }
        public PostInteractiveListMessageModel postWhatsAppInteractiveListModel { get; set; }
        public PostWhatsAppInteractiveButtonModel postWhatsAppInteractiveButtonModel { get; set; }






        public class Text
        {
            public bool preview_url { get; set; }
            public string body { get; set; }
        }
        public class Video
        {
            public string id { get; set; }
            public string caption { get; set; }
            public string link { get; set; }

        }

        public class Audio
        {
            public string id { get; set; }
            public string link { get; set; }

        }
        public class Image
        {
            public string id { get; set; }

            public string link { get; set; }
        }

        public class Document
        {
            public string id { get; set; }
            public string caption { get; set; }
            public string filename { get; set; }
            public string link { get; set; }
        }

        public class Location
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string name { get; set; }
            public string address { get; set; }
        }


        public class PostWhatsAppMediaModel
        {
            public string messaging_product { get; set; }
            public string recipient_type { get; set; }
            public string to { get; set; }
            public string type { get; set; }
            public Image image { get; set; }
            public Video video { get; set; }
            public Document document { get; set; }
            public Audio audio { get; set; }
            public Location location { get; set; }
        }





    }








}






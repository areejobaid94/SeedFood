using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewFunctionApp
{
    public class SendWhatsAppD360Model
    {
        public class Image
        {


            public byte[] Content { get; set; }

            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }

            public string caption { get; set; }
            public string link { get; set; }
        }



        public class Audio
        {
            public string id { get; set; }
            public string link { get; set; }
        }


        public string mediaUrl { get; set; }
        public string typeContent { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string to { get; set; }
        public string from { get; set; }
        public string id { get; set; }
        public Text text { get; set; }
        public Image image { get; set; }
        public Video video { get; set; }
        public Location location { get; set; }
        public Document document { get; set; }
        public Voice voice { get; set; }
        public Audio audio { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
        public byte[] Content { get; set; }


        public class Text
        {
            public string body { get; set; }
        }

        public class Video
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
            public string caption { get; set; }
            public string link { get; set; }
            public byte[] Content { get; set; }
        }
        public class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }
        public class Document
        {
            public string caption { get; set; }
            public string filename { get; set; }
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
            public string link { get; set; }
            public byte[] Content { get; set; }
        }
        public class Voice
        {
            public string id { get; set; }
            public string mime_type { get; set; }
            public string sha256 { get; set; }
            public string caption { get; set; }
            public string link { get; set; }
            public byte[] Content { get; set; }
        }





        public Interactive interactive { get; set; }


        public class Interactive
        {
            public string type { get; set; }
            public Header header { get; set; }
            public Body body { get; set; }
            public Footer footer { get; set; }
            public Action action { get; set; }
        }

        public class Header
        {
            public string type { get; set; }
            public string text { get; set; }
        }

        public class Body
        {
            public string text { get; set; }
        }

        public class Footer
        {
            public string text { get; set; }
        }

        public class Action
        {
            public Button[] buttons { get; set; }
            public string button { get; set; }
            public Section[] sections { get; set; }
        }

        public class Section
        {
            public string title { get; set; }
            public Row[] rows { get; set; }
        }

        public class Row
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }





          
        

        public class Button
        {
            public string type { get; set; }
            public Reply reply { get; set; }
        }

        public class Reply
        {
            public string id { get; set; }
            public string title { get; set; }
        }


    }
}

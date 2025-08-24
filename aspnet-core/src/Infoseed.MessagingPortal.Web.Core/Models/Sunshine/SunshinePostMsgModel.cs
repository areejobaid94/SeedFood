using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    //public class Author
    //{
    //    public string type { get; set; }
    //}
    public class Action
    {
        public string text { get; set; }
        public string type { get; set; }
        public string payload { get; set; }
        public string uri { get; set; }
    }

    public class Coordinates
    {
        public double lat { get; set; }
        public double @long { get; set; }
    }

    public class Location
    {
        public string address { get; set; }
        public string name { get; set; }
    }

    public class Item
    {
        public string title { get; set; }
        public string description { get; set; }
        public string mediaUrl { get; set; }
        public string size { get; set; }
        public List<Action> actions { get; set; }
    }
    public class Content
    {
        //types are: text-image-file-carousel-location , go to the doc for other types.
        //https://docs.smooch.io/rest/#operation/postMessage
        public string type { get; set; }
        public List<Item> items { get; set; }
        public string text { get; set; }
        public string mediaUrl { get; set; }
        public string altText { get; set; } 
        public string agentName { get; set; }
        public string agentId { get; set; }
        public string fileName { get; set; }
        public Location location { get; set; }
        public Coordinates coordinates { get; set; }

    }


    //https://docs.smooch.io/rest/#operation/postMessage
    public class SunshinePostMsgModel
    {
        public Author author;
        public Content content;
    }

}

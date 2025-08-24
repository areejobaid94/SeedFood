using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppBundleModel
    {
        public Conversation_Analytics1 conversation_analytics { get; set; }
        public string id { get; set; }
    }

    public class Conversation_Analytics1
    {
        public Datum1[] data { get; set; }
    }

    public class Datum1
    {
        public Data_Points1[] data_points { get; set; }
    }

    public class Data_Points1
    {
        public int start { get; set; }
        public int end { get; set; }
        public int conversation { get; set; }
        public string phone_number { get; set; }
        public string country { get; set; }
        public string conversation_type { get; set; }
        public string conversation_direction { get; set; }
        public string conversation_category { get; set; }
        public float cost { get; set; }
    }
}

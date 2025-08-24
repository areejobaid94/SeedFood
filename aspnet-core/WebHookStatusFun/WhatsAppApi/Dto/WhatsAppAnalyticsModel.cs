using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebHookStatusFun
{
        public class WhatsAppAnalyticsModel
        {
            [JsonProperty("conversation_analytics")]
            public Conversation_Analytics conversation_analytics { get; set; }
            [JsonProperty("id")]
            public string id { get; set; }
        }

        public class Conversation_Analytics
        {
            [JsonProperty("data")]
            public List<Datum> data { get; set; }
        }

        public class Datum
        {
            [JsonProperty("data_points")]
            public List<Data_Points> data_points { get; set; }
        }

        public class Data_Points
        {
            [JsonProperty("start")]
            public int start { get; set; }

            [JsonProperty("end")]
            public int end { get; set; }

            [JsonProperty("conversation")]
            public int conversation { get; set; }

            [JsonProperty("phone_number")]
            public string phone_number { get; set; }

            [JsonProperty("country")]
            public string country { get; set; }

            [JsonProperty("conversation_type")]
            public string conversation_type { get; set; }

            [JsonProperty("conversation_direction")]
            public string conversation_direction { get; set; }

            [JsonProperty("cost")]
            public int cost { get; set; }
        }

}


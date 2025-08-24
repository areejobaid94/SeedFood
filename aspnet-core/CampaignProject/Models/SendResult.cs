namespace CampaignProject.Models
{
    public class SendResult
    {
       
            public Result result { get; set; }
            public object targetUrl { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        



        public class Error
        {
            public object message { get; set; }
            public string type { get; set; }
            public int code { get; set; }
            public Error_Data error_data { get; set; }
            public string fbtrace_id { get; set; }
        }

        public class Error_Data
        {
            public string messaging_product { get; set; }
            public string details { get; set; }
        }


        public class Result
        {
            public string messaging_product { get; set; }
            public Contact[] contacts { get; set; }
            public Message[] messages { get; set; }
            public Error error { get; set; }
        }

        public class Contact
        {
            public string input { get; set; }
            public string wa_id { get; set; }
        }

        public class Message
        {
            public string id { get; set; }
        }

    }
}

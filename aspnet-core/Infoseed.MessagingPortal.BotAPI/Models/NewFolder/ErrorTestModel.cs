namespace Infoseed.MessagingPortal.BotAPI.Models.NewFolder
{
    public class ErrorTestModel
    {
      
        public Error error { get; set; }
        

        public class Error
        {
            public string message { get; set; }
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

    }
}

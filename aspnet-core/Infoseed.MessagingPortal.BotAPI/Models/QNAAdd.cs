namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class QNAAdd
    {

     
      public Class1[] Property1 { get; set; }
       
        public class Class1
        {
            public string op { get; set; }
            public Value value { get; set; }
        }

        public class Value
        {
            public int id { get; set; }
            public string answer { get; set; }
            public string source { get; set; }
            public string[] questions { get; set; }
            public Metadata metadata { get; set; }
            public Dialog dialog { get; set; }
        }

        public class Metadata
        {
        }

        public class Dialog
        {
            public bool isContextOnly { get; set; }
            public object[] prompts { get; set; }
        }

    }
}

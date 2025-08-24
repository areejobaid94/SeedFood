namespace Infoseed.MessagingPortal.BotAPI.Models.BotModel
{
    public class QNAModel
    {

        public Answer[] answers { get; set; }
     

        public class Answer
        {
            public string[] questions { get; set; }
            public string answer { get; set; }
            public float confidenceScore { get; set; }
            public int id { get; set; }
            public string source { get; set; }
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

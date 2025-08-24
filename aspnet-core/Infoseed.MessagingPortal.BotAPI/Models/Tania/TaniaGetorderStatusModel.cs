namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaGetorderStatusModel
    {

       
            public int code { get; set; }
            public string order_number { get; set; }
            public string order_status_en { get; set; }
            public string order_status_ar { get; set; }
            public string message_en { get; set; }
            public string message_ar { get; set; }

           public string invoice_link { get; set; }
    }
}

namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaValidatePromocodeModel
    {


        public int code { get; set; }
        public string message_en { get; set; }
        public string message_ar { get; set; }
        public bool is_valid { get; set; }


        public string total_before_discount { get; set; }
        public string discount { get; set; }
        public string total_after_discount { get; set; }
        public string promotion_message_en { get; set; }
        public string promotion_message_ar { get; set; }

    }
}

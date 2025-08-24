namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaPromoOffersModel
    {      
            public int code { get; set; }
            public string message_en { get; set; }
            public string message_ar { get; set; }
            public Offer[] offers { get; set; }
  

        public class Offer
        {
            public string title_en { get; set; }
            public string title_ar { get; set; }
            public string image_en { get; set; }
            public string image_ar { get; set; }
            public string start_date { get; set; }
            public string end_date { get; set; }
        }

    }
}

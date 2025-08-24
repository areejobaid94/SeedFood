namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class JoPetrolPriceModel
    {

        public class Rootobject
        {
            public Fuelprice[] Fuelprice { get; set; }
        }

        public class Fuelprice
        {
            public string fuelname { get; set; }
            public string price { get; set; }
        }

    }
}

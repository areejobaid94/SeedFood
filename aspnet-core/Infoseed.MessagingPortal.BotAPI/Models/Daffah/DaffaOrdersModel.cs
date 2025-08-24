namespace Infoseed.MessagingPortal.BotAPI.Models.Daffah
{
    public class DaffaOrdersModel
    {

        public Order[] orders { get; set; }
        

        public class Order
        {
            public string increment_id { get; set; }
            public string status { get; set; }
            public float total { get; set; }
            public string order_date { get; set; }
            public int order_id { get; set; }


            public string textorder { get; set; }
        }

    }
}

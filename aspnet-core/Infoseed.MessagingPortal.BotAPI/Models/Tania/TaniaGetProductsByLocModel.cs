namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaGetProductsByLocModel
    {

        
            public int code { get; set; }
            public bool service_coverage { get; set; }
            public string message_en { get; set; }
            public string message_ar { get; set; }
            public string area { get; set; }
            public string city { get; set; }
            public int address_id { get; set; }
            public string min_order_quantity { get; set; }
            public string min_order_amount { get; set; }
            public Product[] products { get; set; }
        

        public class Product
        {
            public int product_id { get; set; }
            public string product_name_en { get; set; }
            public string product_name_ar { get; set; }
            public string category_name_en { get; set; }
            public string category_name_ar { get; set; }
            public double price { get; set; }
            public string pic_url { get; set; }
        }

    }
}

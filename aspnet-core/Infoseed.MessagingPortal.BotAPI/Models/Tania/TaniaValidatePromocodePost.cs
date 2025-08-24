using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaValidatePromocodePost
    {
 
        public List<Cart> cart { get; set; }
        public int use_wallet { get; set; }
        public int use_coupon { get; set; }
        public string delivery_day { get; set; }
        public int address_id { get; set; }
        public string promocode { get; set; }
        

        public class Cart
        {
            public string product_id { get; set; }
            public string quantity { get; set; }
        }

    }
}

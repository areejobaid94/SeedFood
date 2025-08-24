using System;

namespace Infoseed.MessagingPortal.BotAPI.Models.Tania
{
    public class TaniaCustomerCheckModel
    {
       
            public int code { get; set; }
            public string message_en { get; set; }
            public string message_ar { get; set; }
            public string customer { get; set; }
            public string last_order_status_en { get; set; }
            public string last_order_status_ar { get; set; }
            public DateTime? last_order_date { get; set; }
            public string last_order_number { get; set; }
            public int user_coupons { get; set; }
            public string user_wallet { get; set; }
            public Last_Order_Prducts[] last_order_prducts { get; set; }
            public Address[] address { get; set; }
       

        public class Last_Order_Prducts
        {
            public int product { get; set; }
            public string title_en { get; set; }
            public string title_ar { get; set; }
            public int quantity { get; set; }
        }

        public class Address
        {
            public int address_id { get; set; }
            public string address_title { get; set; }
        }

    }
}

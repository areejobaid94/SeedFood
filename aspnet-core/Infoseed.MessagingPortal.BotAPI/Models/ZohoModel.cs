using System;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class ZohoModel
    {

       
            public string tran_ref { get; set; }
            public string cart_id { get; set; }
            public string cart_description { get; set; }
            public string cart_currency { get; set; }
            public string cart_amount { get; set; }
            public string tran_currency { get; set; }
            public string tran_total { get; set; }
            public Customer_Details customer_details { get; set; }
            public Shipping_Details shipping_details { get; set; }
            public Payment_Result payment_result { get; set; }
            public Payment_Info payment_info { get; set; }
        

        public class Customer_Details
        {
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string street1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string ip { get; set; }
        }

        public class Shipping_Details
        {
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string street1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string ip { get; set; }
        }

        public class Payment_Result
        {
            public string response_status { get; set; }
            public string response_code { get; set; }
            public string response_message { get; set; }
            public string cvv_result { get; set; }
            public string avs_result { get; set; }
            public DateTime transaction_time { get; set; }
        }

        public class Payment_Info
        {
            public string card_type { get; set; }
            public string card_scheme { get; set; }
            public string payment_description { get; set; }
            public int expiryMonth { get; set; }
            public int expiryYear { get; set; }
        }

    }
}

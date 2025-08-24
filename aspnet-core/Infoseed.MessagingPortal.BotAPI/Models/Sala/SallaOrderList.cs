namespace Infoseed.MessagingPortal.BotAPI.Models.Sala
{
    public class SallaOrderList
    {

            public int status { get; set; }
            public bool success { get; set; }
            public Datumu1[] data { get; set; }
            public Paginationn1 pagination { get; set; }


        public class Paginationn1
        {
            public int count { get; set; }
            public int total { get; set; }
            public int perPage { get; set; }
            public int currentPage { get; set; }
            public int totalPages { get; set; }
        }

        public class Datumu1
        {
            public int id { get; set; }
            public int reference_id { get; set; }
            public Urlss1 urls { get; set; }
            public Totall1 total { get; set; }
            public Exchange_Ratee1 exchange_rate { get; set; }
            public Datee1 date { get; set; }
            public string source { get; set; }
            public Statuss1 status { get; set; }
            public bool draft { get; set; }
            public bool read { get; set; }
            public bool can_cancel { get; set; }
            public bool can_reorder { get; set; }
            public string payment_method { get; set; }
            public bool is_pending_payment { get; set; }
            public int pending_payment_ends_at { get; set; }
            public Featuress1 features { get; set; }
            public Payment_Actionss1 payment_actions { get; set; }
            public Itemm1[] items { get; set; }
            public Customerr1 customer { get; set; }
        }

        public class Urlss1
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Totall1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Exchange_Ratee1
        {
            public string base_currency { get; set; }
            public string exchange_currency { get; set; }
            public string rate { get; set; }
        }

        public class Datee1
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Statuss1
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public Customizedd1 customized { get; set; }
        }

        public class Customizedd1
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Featuress1
        {
            public bool shippable { get; set; }
            public bool digitalable { get; set; }
            public bool pickable { get; set; }
            public bool has_suspicious_alert { get; set; }
        }

        public class Payment_Actionss1
        {
            public Refund_Actionn1 refund_action { get; set; }
            public Remaining_Actionn1 remaining_action { get; set; }
        }

        public class Refund_Actionn1
        {
            public bool has_refund_amount { get; set; }
            public string payment_method_label { get; set; }
            public bool can_print_refund_invoice { get; set; }
            public Paid_Amountt1 paid_amount { get; set; }
            public Refund_Amountt1 refund_amount { get; set; }
            public bool can_send_sms { get; set; }
            public string can_send_sms_msg { get; set; }
        }

        public class Paid_Amountt1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Refund_Amountt1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Remaining_Actionn1
        {
            public bool has_remaining_amount { get; set; }
            public string payment_method_label { get; set; }
            public Paid_Amount111 paid_amount { get; set; }
            public string checkout_url { get; set; }
            public Remaining_Amountt1 remaining_amount { get; set; }
        }

        public class Paid_Amount111
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Remaining_Amountt1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Customerr1
        {
            public int id { get; set; }
            public string full_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int mobile { get; set; }
            public string mobile_code { get; set; }
            public string email { get; set; }
            public Urls111 urls { get; set; }
            public string avatar { get; set; }
            public string gender { get; set; }
            public Birthdayy1 birthday { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string currency { get; set; }
            public string location { get; set; }
            public string lang { get; set; }
            public Created_Att1 created_at { get; set; }
            public Updated_Att1 updated_at { get; set; }
        }

        public class Urls111
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Birthdayy1
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Created_Att1
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Updated_Att1
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Itemm1
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public string thumbnail { get; set; }
        }

    }
}

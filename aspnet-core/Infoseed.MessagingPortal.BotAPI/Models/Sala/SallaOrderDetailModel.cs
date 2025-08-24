namespace Infoseed.MessagingPortal.BotAPI.Models.Sala
{
    public class SallaOrderDetailModel
    {

            public int status { get; set; }
            public bool success { get; set; }
            public Dataa data { get; set; }
        

        public class Dataa
        {
            public int id { get; set; }
            public int reference_id { get; set; }
            public string source { get; set; }
            public bool draft { get; set; }
            public bool read { get; set; }
            public Status status { get; set; }
            public string payment_method { get; set; }
            public string receipt_image { get; set; }
            public string currency { get; set; }
            public Amountss amounts { get; set; }
            public Exchange_Ratee exchange_rate { get; set; }
            public bool show_weight { get; set; }
            public int pending_payment_ends_at { get; set; }
            public string total_weight { get; set; }
            public object receiver { get; set; }
            public object notes { get; set; }
            public Customerr customer { get; set; }
            public object[] tags { get; set; }
        }





        public class Status
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public Customizedd customized { get; set; }
        }

        public class Customizedd
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Amountss
        {
            public Sub_Totall sub_total { get; set; }
            public Sub_Totall shipping_cost { get; set; }
            public Sub_Totall cash_on_delivery { get; set; }
            public Taxx tax { get; set; }
            public Totall total { get; set; }
        }

        public class Sub_Totall
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Taxx
        {
            public string percent { get; set; }
            public Totall amount { get; set; }
        }



        public class Totall
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Exchange_Ratee
        {
            public string base_currency { get; set; }
            public string exchange_currency { get; set; }
            public string rate { get; set; }
        }


        public class Customerr
        {
            public int id { get; set; }
            public string full_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int mobile { get; set; }
            public string mobile_code { get; set; }
            public string email { get; set; }
            public Urls1l urls { get; set; }
            public string avatar { get; set; }
            public string gender { get; set; }
            public Birthdayy birthday { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string currency { get; set; }
            public string location { get; set; }
            public string lang { get; set; }
            public Created_Att created_at { get; set; }
            public Created_Att updated_at { get; set; }
        }

        public class Urls1l
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Birthdayy
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Created_Att
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

      



    }
}

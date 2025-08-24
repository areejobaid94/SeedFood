using Newtonsoft.Json;

namespace Infoseed.MessagingPortal.BotAPI.Models.Sala
{
    public class SallaModel
    {

        [JsonProperty("event")]
        public string _event { get; set; }
        [JsonProperty("merchant")]
        public int merchant { get; set; }
        [JsonProperty("created_at")]
        public string created_at { get; set; }
        [JsonProperty("data")]
        public Data data { get; set; }


        public class Data
        {




            public int id { get; set; }
            public Promotion promotion { get; set; }
            public string sku { get; set; }
            public string thumbnail { get; set; }
            public object mpn { get; set; }
            public object gtin { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string short_link_code { get; set; }
            public Urls urls { get; set; }
            public Price price { get; set; }
            public Taxed_Price taxed_price { get; set; }
            public Pre_Tax_Price pre_tax_price { get; set; }
            public Tax tax { get; set; }
            public string description { get; set; }
            public int quantity { get; set; }
            public string status { get; set; }
            public bool is_available { get; set; }
            public int views { get; set; }
            public Sale_Price sale_price { get; set; }
            public object sale_end { get; set; }
            public bool require_shipping { get; set; }
            public string cost_price { get; set; }
            public int weight { get; set; }
            public string weight_type { get; set; }
            public bool with_tax { get; set; }
            public object tax_reason { get; set; }
            public string url { get; set; }
            public string main_image { get; set; }
            public Image[] images { get; set; }
            public int sold_quantity { get; set; }
            public Rating rating { get; set; }
            public Regular_Price regular_price { get; set; }
            public int max_items_per_user { get; set; }
            public object maximum_quantity_per_order { get; set; }
            public bool show_in_app { get; set; }
            public object notify_quantity { get; set; }
            public bool hide_quantity { get; set; }
            public bool unlimited_quantity { get; set; }
            public bool managed_by_branches { get; set; }
            public Services_Blocks services_blocks { get; set; }
            public object calories { get; set; }
            public bool customized_sku_quantity { get; set; }
            public object[] channels { get; set; }
            public Metadata metadata { get; set; }
            public Branches_Quantities[] branches_quantities { get; set; }
            public bool allow_attachments { get; set; }
            public bool is_pinned { get; set; }
            public string pinned_date { get; set; }
            public bool active_advance { get; set; }
            public int sort { get; set; }
            public bool enable_upload_image { get; set; }
            public bool enable_note { get; set; }
            public string updated_at { get; set; }
            public Notifier notifier { get; set; }
            public Option[] options { get; set; }
            public Sku[] skus { get; set; }
            public Category[] categories { get; set; }
            public object[] tags { get; set; }
            public object[] files { get; set; }


            public string full_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public int mobile { get; set; }
            public string mobile_code { get; set; }
            public string email { get; set; }






            public string avatar { get; set; }
            public string gender { get; set; }
            public Birthday birthday { get; set; }
            public object city { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public object currency { get; set; }
            public object location { get; set; }
            public object lang { get; set; }
            public Created_At created_at { get; set; }
            public object[] groups { get; set; }
            public Source source { get; set; }








        }
        public class Birthday
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Created_At
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class Updated_At
        {


            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }


        }

        public class Source
        {


            public string device { get; set; }
            public string useragent { get; set; }
            public string ip { get; set; }


        }
        public class Promotion
        {
            public object title { get; set; }
            public object sub_title { get; set; }
        }

        public class Urls
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Taxed_Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Pre_Tax_Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Tax
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Sale_Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Rating
        {
            public int total { get; set; }
            public int count { get; set; }
            public int rate { get; set; }
        }

        public class Regular_Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Services_Blocks
        {
            public object[] installments { get; set; }
        }

        public class Metadata
        {
            public object title { get; set; }
            public object description { get; set; }
            public object url { get; set; }
        }

        public class Notifier
        {
            public object notify_quantity { get; set; }
            public int minimum_notify_quantity { get; set; }
            public int subscribers_percentage { get; set; }
        }

        public class Image
        {
            public int id { get; set; }
            public string url { get; set; }
            public bool main { get; set; }
            public string three_d_image_url { get; set; }
            public string alt { get; set; }
            public string video_url { get; set; }
            public string type { get; set; }
            public int sort { get; set; }
        }

        public class Branches_Quantities
        {
            public int id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
        }

        public class Option
        {
            public int id { get; set; }
            public int product_id { get; set; }
            public string name { get; set; }
            public object description { get; set; }
            public string type { get; set; }
            public bool required { get; set; }
            public int associated_with_order_time { get; set; }
            public bool availability_range { get; set; }
            public bool not_same_day_order { get; set; }
            public object choose_date_time { get; set; }
            public object from_date_time { get; set; }
            public object to_date_time { get; set; }
            public object sort { get; set; }
            public bool advance { get; set; }
            public string purpose { get; set; }
            public string display_type { get; set; }
            public string visibility { get; set; }
            public Translations translations { get; set; }
            public Value[] values { get; set; }
        }

        public class Translations
        {
            public Ar ar { get; set; }
        }

        public class Ar
        {
            public string option_name { get; set; }
            public object description { get; set; }
        }

        public class Value
        {
            public int id { get; set; }
            public string name { get; set; }
            public Price1 price { get; set; }
            public string formatted_price { get; set; }
            public string formatted_price_without_tax { get; set; }
            public object display_value { get; set; }
            public bool advance { get; set; }
            public int option_id { get; set; }
            public object image_url { get; set; }
            public object hashed_display_value { get; set; }
            public Translations1 translations { get; set; }
            public bool is_default { get; set; }
            public bool is_out_of_stock { get; set; }
        }

        public class Price1
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Translations1
        {
            public Ar1 ar { get; set; }
        }

        public class Ar1
        {
            public string option_details_name { get; set; }
        }

        public class Sku
        {
            public int id { get; set; }
            public int product_id { get; set; }
            public Price2 price { get; set; }
            public Regular_Price1 regular_price { get; set; }
            public Cost_Price cost_price { get; set; }
            public Sale_Price1 sale_price { get; set; }
            public bool has_special_price { get; set; }
            public int stock_quantity { get; set; }
            public bool unlimited_quantity { get; set; }
            public object notify_low { get; set; }
            public object barcode { get; set; }
            public object sku { get; set; }
            public object mpn { get; set; }
            public object gtin { get; set; }
            public string updated_at { get; set; }
            public int[] related_options { get; set; }
            public int[] related_option_values { get; set; }
            public object weight { get; set; }
            public string weight_type { get; set; }
            public string weight_label { get; set; }
            public bool is_user_subscribed_to_sku { get; set; }
            public bool is_default { get; set; }
            public Branches_Quantities1[] branches_quantities { get; set; }
        }

        public class Price2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Regular_Price1
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Cost_Price
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Sale_Price1
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Branches_Quantities1
        {
            public int id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
        }

        public class Category
        {
            public int id { get; set; }
            public string name { get; set; }
            public object image { get; set; }
            public Urls1 urls { get; set; }
            public int parent_id { get; set; }
            public int sort_order { get; set; }
            public string status { get; set; }
            public Show_In show_in { get; set; }
            public bool has_hidden_products { get; set; }
            public string update_at { get; set; }
            public Metadata1 metadata { get; set; }
        }

        public class Urls1
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Show_In
        {
            public bool app { get; set; }
            public bool salla_points { get; set; }
        }

        public class Metadata1
        {
            public object title { get; set; }
            public object description { get; set; }
            public object url { get; set; }
        }


    }
}

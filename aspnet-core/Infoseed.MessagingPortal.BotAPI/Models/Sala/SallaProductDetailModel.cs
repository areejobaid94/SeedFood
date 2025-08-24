namespace Infoseed.MessagingPortal.BotAPI.Models.Sala
{
    public class SallaProductDetailModel
    {

            public int status { get; set; }
            public bool success { get; set; }
            public Dataa2 data { get; set; }
        

        public class Dataa2
        {
            public int id { get; set; }
            public string sku { get; set; }
            public string thumbnail { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string short_link_code { get; set; }
            public Urlss2 urls { get; set; }
            public Pricee2 price { get; set; }
            public Taxed_Pricee2 taxed_price { get; set; }
            public Pre_Tax_Pricee2 pre_tax_price { get; set; }
            public Taxx2 tax { get; set; }
            public string description { get; set; }
            public string status { get; set; }
            public bool is_available { get; set; }
            public int views { get; set; }
            public Sale_Pricee2 sale_price { get; set; }
 
            public bool require_shipping { get; set; }
            public string cost_price { get; set; }
            public float weight { get; set; }
            public string weight_type { get; set; }
            public bool with_tax { get; set; }

            public string url { get; set; }
            public string main_image { get; set; }
            public Imagee2[] images { get; set; }
            public int sold_quantity { get; set; }
            public Ratingg2 rating { get; set; }
            public Regular_Pricee2 regular_price { get; set; }
            public int max_items_per_user { get; set; }
            public object maximum_quantity_per_order { get; set; }
            public bool show_in_app { get; set; }
            public bool hide_quantity { get; set; }
            public bool unlimited_quantity { get; set; }
            public bool managed_by_branches { get; set; }
            public Services_Blockss2 services_blocks { get; set; }
            public bool customized_sku_quantity { get; set; }
            public string[] channels { get; set; }
            public Metadataa2 metadata { get; set; }
            public Branches_Quantitiess2[] branches_quantities { get; set; }
            public object vendor_category_id { get; set; }
            public bool allow_attachments { get; set; }
            public bool is_pinned { get; set; }
            public string pinned_date { get; set; }
            public bool active_advance { get; set; }
            public int sort { get; set; }
            public bool enable_upload_image { get; set; }
            public bool enable_note { get; set; }
            public string updated_at { get; set; }
            public Notifierr2 notifier { get; set; }
            public Optionn2[] options { get; set; }
            public Skuu2[] skus { get; set; }
            public Categoryy2[] categories { get; set; }
        }



        public class Urlss2
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Pricee2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Taxed_Pricee2
        {
            public float amount { get; set; }
            public string currency { get; set; }
        }

        public class Pre_Tax_Pricee2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Taxx2
        {
            public float amount { get; set; }
            public string currency { get; set; }
        }

        public class Sale_Pricee2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Ratingg2
        {
            public int total { get; set; }
            public int count { get; set; }
            public int rate { get; set; }
        }

        public class Regular_Pricee2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Services_Blockss2
        {
            public object[] installments { get; set; }
        }

        public class Metadataa2
        {
            public object title { get; set; }
            public object description { get; set; }
            public object url { get; set; }
        }

        public class Notifierr2
        {
            public object notify_quantity { get; set; }
            public int minimum_notify_quantity { get; set; }
            public int subscribers_percentage { get; set; }
        }

        public class Imagee2
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

        public class Branches_Quantitiess2
        {
            public int id { get; set; }
            public string name { get; set; }
            public object quantity { get; set; }
        }

        public class Optionn2
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
            public Translationss2 translations { get; set; }
            public Valuee2[] values { get; set; }
        }

        public class Translationss2
        {
            public Arr2 ar { get; set; }
        }

        public class Arr2
        {
            public string option_name { get; set; }
            public object description { get; set; }
        }

        public class Valuee2
        {
            public int id { get; set; }
            public string name { get; set; }
            public Price112 price { get; set; }
            public string formatted_price { get; set; }
            public string formatted_price_without_tax { get; set; }
            public object display_value { get; set; }
            public bool advance { get; set; }
            public int option_id { get; set; }
            public object image_url { get; set; }
            public object hashed_display_value { get; set; }
            public Translations112 translations { get; set; }
            public bool is_default { get; set; }
            public bool is_out_of_stock { get; set; }
        }

        public class Price112
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Translations112
        {
            public Ar112 ar { get; set; }
        }

        public class Ar112
        {
            public string option_details_name { get; set; }
        }

        public class Skuu2
        {
            public int id { get; set; }
            public int product_id { get; set; }
            public Price222 price { get; set; }
            public Regular_Price112 regular_price { get; set; }
            public Cost_Pricee2 cost_price { get; set; }
            public Sale_Price112 sale_price { get; set; }
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
            public Branches_Quantities112[] branches_quantities { get; set; }
        }

        public class Price222
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Regular_Price112
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Cost_Pricee2
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Sale_Price112
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Branches_Quantities112
        {
            public int id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
        }

        public class Categoryy2
        {
            public int id { get; set; }
            public string name { get; set; }
            public object image { get; set; }
            public Urls112 urls { get; set; }
            public int parent_id { get; set; }
            public int sort_order { get; set; }
            public string status { get; set; }
            public Show_Inn2 show_in { get; set; }
            public bool has_hidden_products { get; set; }
            public string update_at { get; set; }
            public Metadata112 metadata { get; set; }
        }

        public class Urls112
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Show_Inn2
        {
            public bool app { get; set; }
            public bool salla_points { get; set; }
        }

        public class Metadata112
        {
            public object title { get; set; }
            public object description { get; set; }
            public object url { get; set; }
        }

    }
}

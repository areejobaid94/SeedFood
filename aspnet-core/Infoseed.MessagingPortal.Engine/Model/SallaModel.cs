using Infoseed.MessagingPortal.OrderStatuses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Engine.Model
{
    public class SallaModel
    {
        [JsonProperty("event")]
        public string _event { get; set; }
        public int merchant { get; set; }
        public string created_at { get; set; }
        public Data data { get; set; }
        public class Subtotal
        {

            public double amount { get; set; }
            public string currency { get; set; }
        }
        public class Shipment
        {
            [JsonProperty("courier_name")]
            public string CourierName { get; set; }

            [JsonProperty("tracking_link")]
            public string TrackingLink { get; set; }

            [JsonProperty("tracking_number")]
            public string TrackingNumber { get; set; }
        }
        public class DateTimeInfo
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }

        public class MonetaryValue
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class TaxInfo
        {
            public string percent { get; set; }
            public MonetaryValue amount { get; set; }
        }

        public class Coupon
        {
            public int id { get; set; }
            public string code { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public double amount { get; set; }
            public double minimum_amount { get; set; }
            public string expiry_date { get; set; }
            public object start_date { get; set; }
            public string created_at { get; set; }
            public bool free_shipping { get; set; }
        }

        public class Product
        {
            public int id { get; set; }
            public string type { get; set; }
            public Promotion promotion { get; set; }
            public double quantity { get; set; }
            public string status { get; set; }
            public bool is_available { get; set; }
            public string sku { get; set; }
            public string name { get; set; }
            public MonetaryValue price { get; set; }
            public MonetaryValue sale_price { get; set; }
            public string currency { get; set; }
            public string url { get; set; }
            public object thumbnail { get; set; }
            public bool has_special_price { get; set; }
            public MonetaryValue regular_price { get; set; }
            public object calories { get; set; }
            public object mpn { get; set; }
            public object gtin { get; set; }
            public object description { get; set; }
            public object favorite { get; set; }
            public ProductFeatures features { get; set; }
        }

        public class Promotion
        {
            public object title { get; set; }
            public object sub_title { get; set; }
        }

        public class ProductFeatures
        {
            public object availability_notify { get; set; }
            public bool show_rating { get; set; }
        }

        public class Item
        {
            public int id { get; set; }
            public int product_id { get; set; }
            public string name { get; set; }
            public string sku { get; set; }
            public double quantity { get; set; }
            public double weight { get; set; }
            public ItemAmounts amounts { get; set; }
            public object product_sku_id { get; set; }
            public string currency { get; set; }
            public string weight_label { get; set; }
            public string weight_type { get; set; }
            public object product_type { get; set; }
            public object product_thumbnail { get; set; }
            public object mpn { get; set; }
            public object gtin { get; set; }
            public string notes { get; set; }
            public Product product { get; set; }
            public object[] options { get; set; }
            public object[] images { get; set; }
            public object[] reservations { get; set; }
            public object[] product_reservations { get; set; }
        }

        public class ItemAmounts
        {
            [JsonProperty("price_without_tax")]
            public MonetaryValue price_without_tax { get; set; }

            [JsonProperty("total_discount")]
            public MonetaryValue total_discount { get; set; }

            public TaxInfo tax { get; set; }
            public MonetaryValue total { get; set; }
        }

        public class Urls
        {
            public string customer { get; set; }
            public string admin { get; set; }
            public object rating_link { get; set; }
            public string digital_content { get; set; }
            public string product_card { get; set; }
        }

        public class Source_Details
        {
            public string type { get; set; }
            public object value { get; set; }
            public string device { get; set; }
            public string useragent { get; set; }
            public string utm_source { get; set; }
            public string utm_campaign { get; set; }
            public string utm_medium { get; set; }
            public string utm_term { get; set; }
            public string utm_content { get; set; }
            public object ip { get; set; }
        }

        public class Status2
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public Customized customized { get; set; }
        }

        public class Customized
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string slug { get; set; }
            public int sort { get; set; }
            public object message { get; set; }
            public string icon { get; set; }
            public bool is_active { get; set; }
            public Original original { get; set; }
            public object parent { get; set; }
            public Translations translations { get; set; }
        }
        public class Translations
        {
            public AR ar;

        }
        public class AR
        {
            public string name { get; set; }
            public string message { get; set; }
        }
        public class Original
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Amounts
        {
            [JsonProperty("sub_total")]
            public MonetaryValue sub_total { get; set; }

            [JsonProperty("shipping_cost")]
            public MonetaryValue shipping_cost { get; set; }

            [JsonProperty("cash_on_delivery")]
            public MonetaryValue cash_on_delivery { get; set; }

            public TaxInfo tax { get; set; }
            public object[] discounts { get; set; }
            public MonetaryValue total { get; set; }
        }

        public class Campaign
        {
            public string medium { get; set; }
            public string source { get; set; }
            [JsonProperty("campaign")]
            public string campaign { get; set; }
        }

        public class Features
        {
            public bool shippable { get; set; }
            public bool digitalable { get; set; }
            public bool pickable { get; set; }
            public bool multiple_shipments_supported { get; set; }
            public bool order_type_price_quote { get; set; }
            public bool has_suspicious_alert { get; set; }
        }

        public class Payment_Actions
        {
            public Refund_Action refund_action { get; set; }
            public Remaining_Action remaining_action { get; set; }
        }

        public class Refund_Action
        {
            public bool has_refund_amount { get; set; }
            public string payment_method_label { get; set; }
            public bool can_print_refund_invoice { get; set; }
            public MonetaryValue paid_amount { get; set; }
            public MonetaryValue refund_amount { get; set; }
            public object[] loyalty_point_programs { get; set; }
            public bool can_send_sms { get; set; }
            public string can_send_sms_msg { get; set; }
        }

        public class Remaining_Action
        {
            public bool has_remaining_amount { get; set; }
            public string payment_method_label { get; set; }
            public MonetaryValue paid_amount { get; set; }
            public string checkout_url { get; set; }
            public MonetaryValue remaining_amount { get; set; }
        }

        public class Customer
        {
            public int id { get; set; }
            public string name { get; set; }
            public string mobile { get; set; }
            public string email { get; set; }
            public string avatar { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string full_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string mobile_code { get; set; }
            public Urls urls { get; set; }
            public string gender { get; set; }
            public DateTimeInfo birthday { get; set; }
            public string country_code { get; set; }
            public string currency { get; set; }
            public string location { get; set; }
            public string lang { get; set; }
            public DateTimeInfo created_at { get; set; }
            public DateTimeInfo updated_at { get; set; }
            public object[] groups { get; set; }
        }

        public class Bank
        {
            public int id { get; set; }
            public string bank_name { get; set; }
            public int bank_id { get; set; }
            public string account_name { get; set; }
            public string account_number { get; set; }
            public string iban_number { get; set; }
            public object iban_certificate { get; set; }
            public object sbc_certificate { get; set; }
            public string certificate_type { get; set; }
            public string status { get; set; }
            public int lean_supported { get; set; }
        }

        public class Store
        {
            public int id { get; set; }
            public int store_id { get; set; }
            public int user_id { get; set; }
            public string user_email { get; set; }
            public string username { get; set; }
            public Name name { get; set; }
            public string avatar { get; set; }
        }

        public class Name
        {
            public string ar { get; set; }
            public object en { get; set; }
        }

        public class Created_By
        {
            public object id { get; set; }
            public string name { get; set; }
            public string avatar { get; set; }
        }

        public class Category
        {
            public int id { get; set; }
            public string name { get; set; }
            public Urls urls { get; set; }
        }

        public class BranchQuantity
        {
            public int id { get; set; }
            public string name { get; set; }
            public int quantity { get; set; }
        }

        public class Notifier
        {
            public int? minimum_notify_quantity { get; set; }
            public int? subscribers_percentage { get; set; }
        }

        public class Order
        {
            public int id { get; set; }
            public object checkout_id { get; set; }
            public int reference_id { get; set; }
            public Urls urls { get; set; }
            public DateTimeInfo date { get; set; }
            public DateTimeInfo updated_at { get; set; }
            public string source { get; set; }
            public bool draft { get; set; }
            public bool read { get; set; }
            public Source_Details source_details { get; set; }
            public Status2 status { get; set; }
            public bool is_price_quote { get; set; }
            public string payment_method { get; set; }
            public string receipt_image { get; set; }
            public string currency { get; set; }
            public Amounts amounts { get; set; }
            public Exchange_Rate exchange_rate { get; set; }
            public bool can_cancel { get; set; }
            public Campaign campaign { get; set; }
            public bool show_weight { get; set; }
            public bool can_reorder { get; set; }
            public bool is_pending_payment { get; set; }
            public int pending_payment_ends_at { get; set; }
            public string total_weight { get; set; }
            public Features features { get; set; }
            public object shipping { get; set; }
            public object shipments { get; set; }
            public object[] shipment_branch { get; set; }
            public Payment_Actions payment_actions { get; set; }
            public Customer customer { get; set; }
            public Item[] items { get; set; }
            public MonetaryValue total { get; set; }
            public List<Shipment> Shipments { get; set; }
            public string Status { get; set; }


        }

        public class Exchange_Rate
        {
            public string base_currency { get; set; }
            public string exchange_currency { get; set; }
            public string rate { get; set; }
        }

        public class ProductData
        {
            public int id { get; set; }
            public string name { get; set; }
            public string sku { get; set; }
            public int quantity { get; set; }
            public bool is_available { get; set; }
            public string status { get; set; }
            public MonetaryValue price { get; set; }
            public MonetaryValue sale_price { get; set; }
            public MonetaryValue taxed_price { get; set; }
            public MonetaryValue pre_tax_price { get; set; }
            public MonetaryValue regular_price { get; set; }
            public TaxInfo tax { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public Urls urls { get; set; }
            public List<Category> categories { get; set; }
            public string updated_at { get; set; }
            public List<BranchQuantity> branches_quantities { get; set; }
            public Notifier notifier { get; set; }
            public List<object> skus { get; set; }
            public List<object> options { get; set; }
            public List<object> tags { get; set; }
        }

        public class Data
        {
            public string access_token { get; set; }
            public int expires { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            [JsonProperty("status")]
            public JToken StatusRaw { get; set; }

            [JsonIgnore]
            public string status
            {
                get
                {
                    if (StatusRaw == null)
                        return null;

                    // إذا كان Status كـ string
                    if (StatusRaw.Type == JTokenType.String)
                        return StatusRaw.ToString();

                    // إذا كان Status كـ object
                    if (StatusRaw.Type == JTokenType.Object)
                        return StatusRaw["name"]?.ToString();

                    return null;
                }
            }
            public string token_type { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public MonetaryValue total { get; set; }
            public MonetaryValue subtotal { get; set; }
            public MonetaryValue total_discount { get; set; }
            public string checkout_url { get; set; }
            public int age_in_minutes { get; set; }
            public DateTimeInfo created_at { get; set; }
            public DateTimeInfo updated_at { get; set; }
            public Customer customer { get; set; }
            public Coupon coupon { get; set; }
            public Item[] items { get; set; }
            //public string status { get; set; }
            public Customized customized { get; set; }
            public string note { get; set; }
            public Created_By created_by { get; set; }
            public string type { get; set; }
            public Order order { get; set; }
            public object checkout_id { get; set; }
            public int reference_id { get; set; }
            public Urls urls { get; set; }
            public DateTimeInfo date { get; set; }
            public string source { get; set; }
            public bool draft { get; set; }
            public bool read { get; set; }
            public Source_Details source_details { get; set; }
            public bool is_price_quote { get; set; }
            public string payment_method { get; set; }
            public string receipt_image { get; set; }
            public string currency { get; set; }
            public Amounts amounts { get; set; }
            public Exchange_Rate exchange_rate { get; set; }
            public bool can_cancel { get; set; }
            public Campaign campaign { get; set; }
            public bool show_weight { get; set; }
            public bool can_reorder { get; set; }
            public bool is_pending_payment { get; set; }
            public int pending_payment_ends_at { get; set; }
            public string total_weight { get; set; }
            public Features features { get; set; }
            public object shipping { get; set; }
            public object shipments { get; set; }
            public object[] shipment_branch { get; set; }
            public Payment_Actions payment_actions { get; set; }
            public Bank bank { get; set; }
            public object[] tags { get; set; }
            public Store store { get; set; }
            public ProductData product { get; set; }

            public Status2 Status { get; set; }
            //[JsonProperty("status")]
            //[JsonConverter(typeof(StatusConverter))]
            //public string Status { get; set; }

        }




        public class Total_Discount
        {
            public double amount { get; set; }
            public string currency { get; set; }
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



        public class Date
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }


        public class Sub_Total
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Shipping_Cost
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Cash_On_Delivery
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Tax
        {
            public string percent { get; set; }
            public Amount amount { get; set; }
        }

        public class Amount
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Total
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }






        public class Paid_Amount
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Refund_Amount
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }



        public class Paid_Amount1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Remaining_Amount
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }


        public class Urls1
        {
            public string customer { get; set; }
            public string admin { get; set; }
        }

        public class Birthday
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }




        public class Updated_At1
        {
            public string date { get; set; }
            public int timezone_type { get; set; }
            public string timezone { get; set; }
        }






        public class Amounts1
        {
            public Price_Without_Tax price_without_tax { get; set; }
            public Total_Discount total_discount { get; set; }
            public Tax1 tax { get; set; }
            public Total1 total { get; set; }
        }

        public class Price_Without_Tax
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Tax1
        {
            public string percent { get; set; }
            public Amount1 amount { get; set; }
        }

        public class Amount1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Total1
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }




        public class Price
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Sale_Price
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Regular_Price
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

        public class Features1
        {
            public object availability_notify { get; set; }
            public bool show_rating { get; set; }
        }








        public class Customized1
        {
            public int id { get; set; }
            public string name { get; set; }
        }













        public class Total2
        {
            public double amount { get; set; }
            public string currency { get; set; }
        }

    }

    public class StatusConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.String)
                return token.ToString();

            if (token.Type == JTokenType.Object)
                return token["name"]?.ToString();

            return null;
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }

}
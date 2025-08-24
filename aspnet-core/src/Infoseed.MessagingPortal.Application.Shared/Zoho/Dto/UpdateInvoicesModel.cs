using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{

    public class UpdateInvoicesModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public Invoice1 invoice { get; set; }



  

        public class Invoice1
        {
            public string invoice_id { get; set; }
            public string invoice_number { get; set; }
            public string date { get; set; }
            public string due_date { get; set; }
            public string offline_created_date_with_time { get; set; }
            public string customer_id { get; set; }
            public string customer_name { get; set; }
            public object[] customer_custom_fields { get; set; }
            public Customer_Custom_Field_Hash1 customer_custom_field_hash { get; set; }
            public string email { get; set; }
            public string currency_id { get; set; }
            public string invoice_source { get; set; }
            public string currency_code { get; set; }
            public string currency_symbol { get; set; }
            public string currency_name_formatted { get; set; }
            public string status { get; set; }
            public Custom_Fields1[] custom_fields { get; set; }
            public Custom_Field_Hash1 custom_field_hash { get; set; }
            public string recurring_invoice_id { get; set; }
            public int payment_terms { get; set; }
            public string payment_terms_label { get; set; }
            public bool payment_reminder_enabled { get; set; }
            public float payment_made { get; set; }
            public string zcrm_potential_id { get; set; }
            public string zcrm_potential_name { get; set; }
            public string reference_number { get; set; }
            public bool is_inventory_valuation_pending { get; set; }
            public Lock_Details1 lock_details { get; set; }
            public Line_Items1[] line_items { get; set; }
            public float exchange_rate { get; set; }
            public bool is_autobill_enabled { get; set; }
            public bool inprocess_transaction_present { get; set; }
            public bool allow_partial_payments { get; set; }
            public int price_precision { get; set; }
            public float sub_total { get; set; }
            public float tax_total { get; set; }
            public float discount_total { get; set; }
            public float discount_percent { get; set; }
            public float discount { get; set; }
            public float discount_applied_on_amount { get; set; }
            public string discount_type { get; set; }
            public string tax_override_preference { get; set; }
            public string tds_override_preference { get; set; }
            public bool is_discount_before_tax { get; set; }
            public float adjustment { get; set; }
            public string adjustment_description { get; set; }
            public string shipping_charge_tax_id { get; set; }
            public string shipping_charge_tax_name { get; set; }
            public string shipping_charge_tax_type { get; set; }
            public string shipping_charge_tax_percentage { get; set; }
            public string shipping_charge_tax { get; set; }
            public string bcy_shipping_charge_tax { get; set; }
            public float shipping_charge_exclusive_of_tax { get; set; }
            public float shipping_charge_inclusive_of_tax { get; set; }
            public string shipping_charge_tax_formatted { get; set; }
            public string shipping_charge_exclusive_of_tax_formatted { get; set; }
            public string shipping_charge_inclusive_of_tax_formatted { get; set; }
            public float shipping_charge { get; set; }
            public float bcy_shipping_charge { get; set; }
            public float bcy_adjustment { get; set; }
            public float bcy_sub_total { get; set; }
            public float bcy_discount_total { get; set; }
            public float bcy_tax_total { get; set; }
            public float bcy_total { get; set; }
            public float total { get; set; }
            public float balance { get; set; }
            public float write_off_amount { get; set; }
            public float roundoff_value { get; set; }
            public string transaction_rounding_type { get; set; }
            public bool is_inclusive_tax { get; set; }
            public float sub_total_inclusive_of_tax { get; set; }
            public string contact_category { get; set; }
            public string tax_rounding { get; set; }
            public object[] taxes { get; set; }
            public string tds_calculation_type { get; set; }
            public bool can_send_invoice_sms { get; set; }
            public string payment_expected_date { get; set; }
            public float payment_discount { get; set; }
            public bool stop_reminder_until_payment_expected_date { get; set; }
            public string last_payment_date { get; set; }
            public bool ach_supported { get; set; }
            public bool ach_payment_initiated { get; set; }
            public Payment_Options1 payment_options { get; set; }
            public bool reader_offline_payment_initiated { get; set; }
            public object[] contact_persons { get; set; }
            public string attachment_name { get; set; }
            public object[] documents { get; set; }
            public string computation_type { get; set; }
            public Late_Fee1 late_fee { get; set; }
            public object[] debit_notes { get; set; }
            public object[] deliverychallans { get; set; }
            public string merchant_id { get; set; }
            public string merchant_name { get; set; }
            public string ecomm_operator_id { get; set; }
            public string ecomm_operator_name { get; set; }
            public string salesorder_id { get; set; }
            public string salesorder_number { get; set; }
            public object[] salesorders { get; set; }
            public object[] shipping_bills { get; set; }
            public Contact_Persons_Details1[] contact_persons_details { get; set; }
            public Contact1 contact { get; set; }
            public string salesperson_id { get; set; }
            public string salesperson_name { get; set; }
            public bool is_emailed { get; set; }
            public int reminders_sent { get; set; }
            public string last_reminder_sent_date { get; set; }
            public string next_reminder_date_formatted { get; set; }
            public bool is_viewed_by_client { get; set; }
            public string client_viewed_time { get; set; }
            public string submitter_id { get; set; }
            public string approver_id { get; set; }
            public string submitted_date { get; set; }
            public string submitted_by { get; set; }
            public string submitted_by_name { get; set; }
            public string submitted_by_email { get; set; }
            public string submitted_by_photo_url { get; set; }
            public string template_id { get; set; }
            public string template_name { get; set; }
            public string template_type { get; set; }
            public string notes { get; set; }
            public string terms { get; set; }
            public Billing_Address1 billing_address { get; set; }
            public Shipping_Address1 shipping_address { get; set; }
            public string invoice_url { get; set; }
            public string subject_content { get; set; }
            public bool can_send_in_mail { get; set; }
            public DateTime created_time { get; set; }
            public DateTime last_modified_time { get; set; }
            public string created_date { get; set; }
            public string created_by_id { get; set; }
            public string created_by_name { get; set; }
            public string last_modified_by_id { get; set; }
            public string page_width { get; set; }
            public string page_height { get; set; }
            public string orientation { get; set; }
            public string is_backorder { get; set; }
            public string sales_channel { get; set; }
            public string type { get; set; }
            public string color_code { get; set; }
            public string current_sub_status_id { get; set; }
            public string current_sub_status { get; set; }
            public object[] sub_statuses { get; set; }
            public string reason_for_debit_note { get; set; }
            public string estimate_id { get; set; }
            public bool is_client_review_settings_enabled { get; set; }
            public float unused_retainer_payments { get; set; }
            public float credits_applied { get; set; }
            public float tax_amount_withheld { get; set; }
            public string schedule_time { get; set; }
            public int no_of_copies { get; set; }
            public bool show_no_of_copies { get; set; }
            public Customer_Default_Billing_Address1 customer_default_billing_address { get; set; }
            public Reference_Invoice1 reference_invoice { get; set; }
            public bool includes_package_tracking_info { get; set; }
            public object[] approvers_list { get; set; }
        }

        public class Customer_Custom_Field_Hash1
        {
        }

        public class Custom_Field_Hash1
        {
            public string cf_test { get; set; }
            public string cf_test_unformatted { get; set; }
        }

        public class Lock_Details1
        {
        }

        public class Payment_Options1
        {
            public Payment_Gateways1[] payment_gateways { get; set; }
        }

        public class Payment_Gateways1
        {
            public bool configured { get; set; }
            public bool can_show_billing_address { get; set; }
            public bool is_bank_account_applicable { get; set; }
            public bool can_pay_using_new_card { get; set; }
            public string gateway_name { get; set; }
        }

        public class Late_Fee1
        {
            public string name { get; set; }
            public string type { get; set; }
            public float rate { get; set; }
            public float amount { get; set; }
            public string frequency_type { get; set; }
        }

        public class Contact1
        {
            public float customer_balance { get; set; }
            public float credit_limit { get; set; }
            public float unused_customer_credits { get; set; }
            public bool is_credit_limit_migration_completed { get; set; }
        }

        public class Billing_Address1
        {
            public string street { get; set; }
            public string address { get; set; }
            public string street2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public string country { get; set; }
            public string fax { get; set; }
            public string phone { get; set; }
            public string attention { get; set; }
        }

        public class Shipping_Address1
        {
            public string street { get; set; }
            public string address { get; set; }
            public string street2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public string country { get; set; }
            public string fax { get; set; }
            public string phone { get; set; }
            public string attention { get; set; }
        }

        public class Customer_Default_Billing_Address1
        {
            public string zip { get; set; }
            public string country { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string phone { get; set; }
            public string street2 { get; set; }
            public string state { get; set; }
            public string fax { get; set; }
            public string state_code { get; set; }
        }

        public class Reference_Invoice1
        {
            public string reference_invoice_id { get; set; }
        }

        public class Custom_Fields1
        {
            public string field_id { get; set; }
            public string customfield_id { get; set; }
            public bool show_in_store { get; set; }
            public bool show_in_portal { get; set; }
            public bool is_active { get; set; }
            public int index { get; set; }
            public string label { get; set; }
            public bool show_on_pdf { get; set; }
            public bool edit_on_portal { get; set; }
            public bool edit_on_store { get; set; }
            public string api_name { get; set; }
            public bool show_in_all_pdf { get; set; }
            public string value_formatted { get; set; }
            public string search_entity { get; set; }
            public string data_type { get; set; }
            public string placeholder { get; set; }
            public string value { get; set; }
            public bool is_dependent_field { get; set; }
        }

        public class Line_Items1
        {
            public string line_item_id { get; set; }
            public string item_id { get; set; }
            public string sku { get; set; }
            public int item_order { get; set; }
            public string name { get; set; }
            public string internal_name { get; set; }
            public string description { get; set; }
            public string unit { get; set; }
            public float quantity { get; set; }
            public float discount_amount { get; set; }
            public float discount { get; set; }
            public object[] discounts { get; set; }
            public float bcy_rate { get; set; }
            public float rate { get; set; }
            public string header_id { get; set; }
            public string header_name { get; set; }
            public string pricebook_id { get; set; }
            public string tax_id { get; set; }
            public string tax_name { get; set; }
            public string tax_type { get; set; }
            public int tax_percentage { get; set; }
            public float item_total { get; set; }
            public object[] item_custom_fields { get; set; }
            public object[] documents { get; set; }
            public object[] line_item_taxes { get; set; }
            public string project_id { get; set; }
            public object[] time_entry_ids { get; set; }
            public string expense_id { get; set; }
            public string item_type { get; set; }
            public string expense_receipt_name { get; set; }
            public string purchase_rate { get; set; }
            public string salesorder_item_id { get; set; }
            public int cost_amount { get; set; }
        }

        public class Contact_Persons_Details1
        {
            public string contact_person_id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string mobile { get; set; }
            public bool is_primary_contact { get; set; }
            public string photo_url { get; set; }
        }


    }

}

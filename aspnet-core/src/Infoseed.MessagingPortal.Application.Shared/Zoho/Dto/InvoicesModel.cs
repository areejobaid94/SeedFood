using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{
    public class InvoicesModel
    {
        public int TenantId { get; set; }
        public bool IsCaution { get; set; }
        public bool IsPaidInvoice { get; set; }
        public int code { get; set; }
            public string message { get; set; }
            public Invoice[] invoices { get; set; }
            public Page_Context page_context { get; set; }
       

        public class Page_Context
        {
            public int total { get; set; }
            public int total_pages { get; set; }
      
            public int page { get; set; }
            public int per_page { get; set; }
            public bool has_more_page { get; set; }
            public string report_name { get; set; }
            public string applied_filter { get; set; }
            public object[] custom_fields { get; set; }
            public string sort_column { get; set; }
            public string sort_order { get; set; }
        }

        public class Invoice
        {
            public string invoice_id { get; set; }
            public bool ach_payment_initiated { get; set; }
            public string zcrm_potential_id { get; set; }
            public string zcrm_potential_name { get; set; }
            public string customer_name { get; set; }
            public string customer_id { get; set; }
            public string company_name { get; set; }
            public string status { get; set; }
            public string color_code { get; set; }
            public string current_sub_status_id { get; set; }
            public string current_sub_status { get; set; }
            public string invoice_number { get; set; }
            public string reference_number { get; set; }
            public string date { get; set; }
            public string due_date { get; set; }
            public string due_days { get; set; }
            public string currency_id { get; set; }
            public string schedule_time { get; set; }
            public string email { get; set; }
            public string currency_code { get; set; }
            public string currency_symbol { get; set; }
            public string template_type { get; set; }
            public int no_of_copies { get; set; }
            public bool show_no_of_copies { get; set; }
            public bool is_viewed_by_client { get; set; }
            public bool has_attachment { get; set; }
            public string client_viewed_time { get; set; }
            public string invoice_url { get; set; }
            public string project_name { get; set; }
            public Billing_Address billing_address { get; set; }
            public Shipping_Address shipping_address { get; set; }
            public string country { get; set; }
            public string phone { get; set; }
            public string created_by { get; set; }
            public DateTime updated_time { get; set; }
            public string transaction_type { get; set; }
            public float total { get; set; }
            public float balance { get; set; }
            public DateTime created_time { get; set; }
            public DateTime last_modified_time { get; set; }
            public bool is_emailed { get; set; }
            public bool is_viewed_in_mail { get; set; }
            public string mail_first_viewed_time { get; set; }
            public string mail_last_viewed_time { get; set; }
            public int reminders_sent { get; set; }
            public string last_reminder_sent_date { get; set; }
            public string payment_expected_date { get; set; }
            public string last_payment_date { get; set; }
            public object[] custom_fields { get; set; }
            public Custom_Field_Hash custom_field_hash { get; set; }
            public string template_id { get; set; }
            public string documents { get; set; }
            public string salesperson_id { get; set; }
            public string salesperson_name { get; set; }
            public float shipping_charge { get; set; }
            public float adjustment { get; set; }
            public float write_off_amount { get; set; }
            public float exchange_rate { get; set; }
        }

        public class Billing_Address
        {
            public string address { get; set; }
            public string street2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zipcode { get; set; }
            public string country { get; set; }
            public string phone { get; set; }
            public string fax { get; set; }
            public string attention { get; set; }
        }

        public class Shipping_Address
        {
            public string address { get; set; }
            public string street2 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zipcode { get; set; }
            public string country { get; set; }
            public string phone { get; set; }
            public string fax { get; set; }
            public string attention { get; set; }
        }

        public class Custom_Field_Hash
        {
        }


    }
}

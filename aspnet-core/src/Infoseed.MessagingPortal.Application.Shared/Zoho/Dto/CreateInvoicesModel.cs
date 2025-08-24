using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{

        public class CreateInvoicesModel
        {
            public string invoice_number { get; set; }
            public string date { get; set; }
            public string status { get; set; }
            public string payment_terms_label { get; set; }
            public string due_date { get; set; }
            public string payment_expected_date { get; set; }
            public string last_payment_date { get; set; }
            public string reference_number { get; set; }
            public long customer_id { get; set; }
            public string customer_name { get; set; }
            public long currency_id { get; set; }
            public string currency_code { get; set; }
            public int discount { get; set; }
            public string discount_type { get; set; }
            public bool is_inclusive_tax { get; set; }
            public bool is_viewed_by_client { get; set; }
            public bool has_attachment { get; set; }
            public string client_viewed_time { get; set; }
            public Line_Items[] line_items { get; set; }
            public int total { get; set; }
            public bool payment_reminder_enabled { get; set; }
            public float payment_made { get; set; }
            public float credits_applied { get; set; }
            public int tax_amount_withheld { get; set; }
            public int balance { get; set; }
            public int write_off_amount { get; set; }
            public bool allow_partial_payments { get; set; }
            public int price_precision { get; set; }
            public Payment_Options payment_options { get; set; }
            public bool is_emailed { get; set; }
            public int reminders_sent { get; set; }
            public string last_reminder_sent_date { get; set; }
            public string notes { get; set; }
            public string terms { get; set; }
            public string attachment_name { get; set; }
            public bool can_send_in_mail { get; set; }
            public string salesperson_id { get; set; }
            public string salesperson_name { get; set; }
            public string callback { get; set; }
            public string _return { get; set; }
        }

        public class Payment_Options
        {
            public Payment_Gateways[] payment_gateways { get; set; }
        }

        public class Payment_Gateways
        {
            public bool configured { get; set; }
            public string additional_field1 { get; set; }
            public string gateway_name { get; set; }
        }

        public class Line_Items
        {
            public string name { get; set; }
            public string description { get; set; }
            public int item_order { get; set; }
            public decimal rate { get; set; }
            public int quantity { get; set; }
            public string unit { get; set; }
            public int discount_amount { get; set; }
            public int discount { get; set; }
            public decimal item_total { get; set; }
        }
}

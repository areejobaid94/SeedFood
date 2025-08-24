using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{
    public class ZohoContactsModel
    {   
            public int code { get; set; }
            public string message { get; set; }
            public ContactZoho[] contacts { get; set; }
          

        public class ContactZoho
        {
            public string contact_id { get; set; }
            public string contact_name { get; set; }
            public string customer_name { get; set; }
            public string vendor_name { get; set; }
            public string company_name { get; set; }
            public string website { get; set; }
            public string language_code { get; set; }
            public string language_code_formatted { get; set; }
            public string contact_type { get; set; }
            public string contact_type_formatted { get; set; }
            public string status { get; set; }
            public string customer_sub_type { get; set; }
            public string source { get; set; }
            public bool is_linked_with_zohocrm { get; set; }
            public int payment_terms { get; set; }
            public string payment_terms_label { get; set; }
            public string currency_id { get; set; }
            public string twitter { get; set; }
            public string facebook { get; set; }
            public string currency_code { get; set; }
            public float outstanding_receivable_amount { get; set; }
            public float outstanding_receivable_amount_bcy { get; set; }
            public float unused_credits_receivable_amount { get; set; }
            public float unused_credits_receivable_amount_bcy { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string mobile { get; set; }
            public string portal_status { get; set; }
            public DateTime created_time { get; set; }
            public string created_time_formatted { get; set; }
            public DateTime last_modified_time { get; set; }
            public string last_modified_time_formatted { get; set; }
      
          
            public bool ach_supported { get; set; }
            public bool has_attachment { get; set; }
        }

      

    }
}

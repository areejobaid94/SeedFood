using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Zoho.Dto
{
    public class StatementsModel
    {


            public int code { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
          
       

        public class Data
        {
            public string contact_name { get; set; }
            public string html_string { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
            public string from_date_formatted { get; set; }
            public string contact_id { get; set; }
            public string to_date_formatted { get; set; }
            public string page_width { get; set; }
            public string page_height { get; set; }
        }      

        public class Group_By
        {
            public string field { get; set; }
            public string group { get; set; }
        }

    }
}

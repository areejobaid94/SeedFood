using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Careem_Express
{
    public class CareemModel
    {
        public string event_id { get; set; }
        public DateTime occurred_at { get; set; }
        public string event_type { get; set; }
        public Details details { get; set; }
        public string client_id { get; set; }

        public class Details
        {
            public string delivery_id { get; set; }
            public int reference_id { get; set; }
            public string carrier_reference { get; set; }
            public Captain_Location captain_location { get; set; }
            public Captain_Eta captain_eta { get; set; }
            public Captain captain { get; set; }
            public string external_order_reference { get; set; }
        }

        public class Captain_Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class Captain_Eta
        {
            public int distance { get; set; }
            public int duration { get; set; }
        }

        public class Captain
        {
            public string name { get; set; }
            public string image_url { get; set; }
            public string phone_number { get; set; }
        }


    }
}

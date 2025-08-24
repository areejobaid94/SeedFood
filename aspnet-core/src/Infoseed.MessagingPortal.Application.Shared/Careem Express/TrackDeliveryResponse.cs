using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Careem_Express
{
    public class TrackDeliveryResponse
    {

        public Eta eta { get; set; }
        public string status { get; set; }
        public CaptainInfo captain { get; set; }
        public LocationInfo location { get; set; }
        public string tracking_url { get; set; }

        public class Eta
        {
            public int distance { get; set; }
            public int duration { get; set; }
        }

        public class CaptainInfo
        {
            public string name { get; set; }
            public string image_url { get; set; }
            public string phone_number { get; set; }
        }

        public class LocationInfo
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

    }
}

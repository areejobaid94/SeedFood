using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Sunshine
{
    public class LocationMapModel
    {

       
        public Datum[] data { get; set; }
        
        public class Datum
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
            public string type { get; set; }
            public float distance { get; set; }
            public string name { get; set; }
            public string number { get; set; }
            public object postal_code { get; set; }
            public string street { get; set; }
            public float confidence { get; set; }
            public string region { get; set; }
            public string region_code { get; set; }
            public object county { get; set; }
            public string locality { get; set; }
            public object administrative_area { get; set; }
            public object neighbourhood { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string continent { get; set; }
            public string label { get; set; }
        }

    }
}

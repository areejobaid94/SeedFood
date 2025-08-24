namespace Infoseed.MessagingPortal.Engine.Model
{
    public class SallaInfoModel
    {


            public int status { get; set; }
            public bool success { get; set; }
            public Data55 data { get; set; }
        

        public class Data55
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public string role { get; set; }
            public string created_at { get; set; }
            public Merchant55 merchant { get; set; }
            public Context55 context { get; set; }
        }

        public class Merchant55
        {
            public int id { get; set; }
            public string username { get; set; }
            public string name { get; set; }
            public string avatar { get; set; }
            public object store_location { get; set; }
            public string plan { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public string domain { get; set; }
            public object tax_number { get; set; }
            public object commercial_number { get; set; }
            public bool from_competitor { get; set; }
            public string created_at { get; set; }
        }

        public class Context55
        {
            public int app { get; set; }
            public string scope { get; set; }
            public int exp { get; set; }
        }

    }
}

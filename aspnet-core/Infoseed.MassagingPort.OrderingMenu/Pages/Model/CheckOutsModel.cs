namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class CheckOutsModel
    {

        
            public string id { get; set; }
            public string paymentType { get; set; }
            public string paymentBrand { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string descriptor { get; set; }
            public Result result { get; set; }
            public Resultdetails resultDetails { get; set; }
            public Card card { get; set; }
            public Customer customer { get; set; }
            public Threedsecure threeDSecure { get; set; }
            public Customparameters customParameters { get; set; }
            public Risk risk { get; set; }
            public string buildNumber { get; set; }
            public string timestamp { get; set; }
            public string ndc { get; set; }
        

        public class Result
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Resultdetails
        {
            public string clearingInstituteName { get; set; }
        }

        public class Card
        {
            public string bin { get; set; }
            public string last4Digits { get; set; }
            public string holder { get; set; }
            public string expiryMonth { get; set; }
            public string expiryYear { get; set; }
        }

        public class Customer
        {
            public string ip { get; set; }
            public string ipCountry { get; set; }
            public Browserfingerprint browserFingerprint { get; set; }
        }

        public class Browserfingerprint
        {
            public string value { get; set; }
        }

        public class Threedsecure
        {
            public string eci { get; set; }
            public string xid { get; set; }
        }

        public class Customparameters
        {
            public string SHOPPER_EndToEndIdentity { get; set; }
            public string CTPE_DESCRIPTOR_TEMPLATE { get; set; }
            public string FEEDZAI_DATA_FEED { get; set; }
        }

        public class Risk
        {
            public string score { get; set; }
        }

    }
}

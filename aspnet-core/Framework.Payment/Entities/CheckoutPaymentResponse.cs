using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment
{
    public class Result
    {
        public string code { get; set; }
        public string description { get; set; }
    }
    public class CheckoutPaymentResponse
    {
        public Result result { get; set; }
        public string buildNumber { get; set; }
        public string timestamp { get; set; }
        public string ndc { get; set; }
        public string id { get; set; }
    }
}

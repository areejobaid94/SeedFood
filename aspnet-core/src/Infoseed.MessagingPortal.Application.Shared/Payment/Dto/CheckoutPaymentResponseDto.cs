using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Payment
{
    public class CheckoutResultDto
    {
        public string code { get; set; }
        public string description { get; set; }
    }
    public class CheckoutPaymentResponseDto
    {
        public CheckoutResultDto result { get; set; }
        public string buildNumber { get; set; }
        public string timestamp { get; set; }
        public string ndc { get; set; }
        public string id { get; set; }
        public string CheckoutUrl { get; set; }
    }
}

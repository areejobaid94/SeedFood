using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment
{
   public class CheckoutPaymentRequest
    {
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Currency { get; set; }
        public string PaymentBrand { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}

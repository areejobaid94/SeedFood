using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Payment
{
   public class CheckoutPaymentRequestDto
    {
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public string Currency { get; set; }
        public string PaymentBrand { get; set; }
        public string PaymentType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Payment
{
  public interface IHyperPayPaymentService
    {
        CheckoutPaymentResponseDto Checkout(CheckoutPaymentRequestDto request);
    }
}

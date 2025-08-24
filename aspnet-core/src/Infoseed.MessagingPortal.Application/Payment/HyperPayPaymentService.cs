using Framework.Payment;
using Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Payment
{
    public class HyperPayPaymentService : MessagingPortalAppServiceBase, IHyperPayPaymentService
    {
        private readonly IConfiguration _appConfiguration;
        public HyperPayPaymentService(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }
        public CheckoutPaymentResponseDto Checkout(CheckoutPaymentRequestDto request)
        {
            CheckoutPaymentResponseDto result = null;
            PaymentProvider paymentProvider = new PaymentProvider(_appConfiguration);
            var response = paymentProvider.PrepareCheckout(new CheckoutPaymentRequest()
            {
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentBrand = request.PaymentBrand,
                PaymentType = (PaymentType)Enum.Parse(typeof(PaymentType), request.PaymentType),
                TaxAmount = request.TaxAmount
            });
            result = new CheckoutPaymentResponseDto()
            {
                id = response.id,
                buildNumber = response.buildNumber,
                ndc = response.ndc,
                result = new CheckoutResultDto() { code = response.result.code, description = response.result.description },
                timestamp = response.timestamp,
                CheckoutUrl = $"{this._appConfiguration["Payment:HyperPay:BaseUrl"]}/paymentWidgets.js?checkoutId={response.id}"
            };
            return result;
        }
    }
}

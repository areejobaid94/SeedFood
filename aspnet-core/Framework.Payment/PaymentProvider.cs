using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

using System;

namespace Framework.Payment
{
    public class PaymentProvider
    {
        private IConfiguration _configuration;

        public PaymentProvider(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
      
        public CheckoutPaymentResponse PrepareCheckout(CheckoutPaymentRequest checkoutPaymentRequest)
        {
           // var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string baseUrl = this._configuration["Payment:HyperPay:BaseUrl"];
            string accessToken = this._configuration["Payment:HyperPay:ClientSecret"];
            string entityId = this._configuration["Payment:HyperPay:ClientId"];
            var client = new RestClient($"{baseUrl}/checkouts");
            client.Timeout = -1;
            var _request = new RestRequest(Method.POST);
            _request.AddHeader("Authorization", $"Bearer {accessToken}");
            _request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            _request.AddParameter("entityId", entityId);
            _request.AddParameter("amount", checkoutPaymentRequest.Amount);
            _request.AddParameter("currency", checkoutPaymentRequest.Currency);
            _request.AddParameter("paymentType", checkoutPaymentRequest.PaymentType);
            //############# Log Request ###################.
            ActionLog log = PaymentHelper.BuildLogRequest(_request, "application/json", $"{baseUrl}/checkouts");
            PaymentRepository.InsertActionLog(log, _configuration);
            //############# Execute ###################.
            IRestResponse _response = client.Execute(_request);
            //############# Log Response ###################.
            PaymentHelper.BuildLogResponse(log, _response);
            PaymentRepository.UpdateActionLog(log, _configuration);
            var response = JsonConvert.DeserializeObject<CheckoutPaymentResponse>(_response.Content);
            return response;
        }

        public GetPaymentStatusResponse GetPaymentStatus(GetPaymentStatusRequest getPaymentStatusRequest)
        {
            GetPaymentStatusResponse response = new GetPaymentStatusResponse();
            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string baseUrl = this._configuration["Payment:HyperPay:BaseUrl"];
            string accessToken = this._configuration["Payment:HyperPay:ClientSecret"];
            string entityId = this._configuration["Payment:HyperPay:ClientId"];

            var client = new RestClient($"{baseUrl}/checkouts/{getPaymentStatusRequest.Id}/payment?entityId={entityId}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("entityId", entityId);
            //############# Log Request ###################.
            ActionLog log = PaymentHelper.BuildLogRequest(request, "application/json", $"{baseUrl}/checkouts/{getPaymentStatusRequest.Id}/payment?entityId={entityId}");
            PaymentRepository.InsertActionLog(log, _configuration);
            //############# Execute ###################.
            IRestResponse _response = client.Execute(request);
            //############# Log Response ###################.
            PaymentHelper.BuildLogResponse(log, _response);
            PaymentRepository.UpdateActionLog(log, _configuration);
            response = JsonConvert.DeserializeObject<GetPaymentStatusResponse>(_response.Content);
            return response;
        }

        public void Capture()
        {

        }

        public void Refund()
        {

        }

        public void Reverse()
        {

        }

        public void Rebill()
        {

        }

        public void Credit()
        {

        }
    }
}

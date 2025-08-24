using Castle.Core.Logging;
using Infoseed.MessagingPortal.Web.Host;
using Infoseed.MessagingPortal.Web.Host.Gateway;
using Infoseed.MessagingPortal.Web.Host.Models;
using Infoseed.MessagingPortal.Web.Host.Utils;
using Infoseed.MessagingPortal.Web.Models.payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostedCheckout : MessagingPortalControllerBase
    {
        protected readonly GatewayApiConfig GatewayApiConfig;
        protected readonly GatewayApiClient GatewayApiClient;
       // protected readonly NVPApiClient NVPApiClient;
       private IConfiguration _configuration;
        public HostedCheckout(IOptions<GatewayApiConfig> gatewayApiConfig, GatewayApiClient gatewayApiClient, IConfiguration configuration)
        {
            GatewayApiConfig = gatewayApiConfig.Value;
            GatewayApiClient = gatewayApiClient;
            //NVPApiClient = nvpApiClient;
            _configuration = configuration;


        }

        [HttpGet("TesthostedCheckout")]
        public CheckoutModel TesthostedCheckout()
        {

            return null;
                
         }
            [HttpGet("hostedCheckout2")]
        public CheckoutModel hostedCheckout2()
        {
            

                GatewayApiConfig.Debug = bool.Parse(_configuration["GatewayApiConfig:Debug"]);
                GatewayApiConfig.UseSsl = bool.Parse(_configuration["GatewayApiConfig:UseSsl"]);
                GatewayApiConfig.IgnoreSslErrors = bool.Parse(_configuration["GatewayApiConfig:IgnoreSslErrors"]);
                GatewayApiConfig.Version = _configuration["GatewayApiConfig:Version"];
                GatewayApiConfig.UseProxy = bool.Parse(_configuration["GatewayApiConfig:UseProxy"]);
                GatewayApiConfig.ProxyHost = _configuration["GatewayApiConfig:ProxyHost"];
                GatewayApiConfig.ProxyUser = _configuration["GatewayApiConfig:ProxyUser"];
                GatewayApiConfig.ProxyPassword = _configuration["GatewayApiConfig:ProxyPassword"];
                GatewayApiConfig.ProxyDomain = _configuration["GatewayApiConfig:ProxyDomain"];

                GatewayApiConfig.WebhooksNotificationSecret = _configuration["GatewayApiConfig:WebhooksNotificationSecret"];

                GatewayApiConfig.MerchantId = _configuration["GatewayApiConfig:GATEWAY_MERCHANT_ID"];
                GatewayApiConfig.Password = _configuration["GatewayApiConfig:GATEWAY_API_PASSWORD"];
                GatewayApiConfig.Currency = _configuration["GatewayApiConfig:GATEWAY_CURRENCY"];
                GatewayApiConfig.GatewayUrl = _configuration["GatewayApiConfig:GATEWAY_BASE_URL"];



                GatewayApiRequest gatewayApiRequest = new GatewayApiRequest(GatewayApiConfig);
                gatewayApiRequest.ApiOperation = "CREATE_CHECKOUT_SESSION";
                gatewayApiRequest.OrderId = IdUtils.generateSampleId();
                gatewayApiRequest.OrderCurrency = GatewayApiConfig.Currency;

                gatewayApiRequest.buildSessionRequestUrl();
                gatewayApiRequest.buildPayload();

                gatewayApiRequest.ApiMethod = GatewayApiClient.POST;


                String response = GatewayApiClient.SendTransaction(gatewayApiRequest);



                CheckoutSessionModel checkoutSessionModel = CheckoutSessionModel.toCheckoutSessionModel(response);

                ViewBag.CheckoutJsUrl = $@"{GatewayApiConfig.GatewayUrl}/checkout/version/{GatewayApiConfig.Version}/checkout.js";
                ViewBag.MerchantId = GatewayApiConfig.MerchantId;
                ViewBag.OrderId = gatewayApiRequest.OrderId;
                ViewBag.CheckoutSession = checkoutSessionModel;
                ViewBag.Currency = GatewayApiConfig.Currency;

                var jsonModel = JsonConvert.SerializeObject(checkoutSessionModel).ToString();


                CheckoutModel checkoutModel = new CheckoutModel
                {
                    CheckoutJsUrl = $@"{GatewayApiConfig.GatewayUrl}/checkout/version/{GatewayApiConfig.Version}/checkout.js",

                    MerchantId = GatewayApiConfig.MerchantId,
                    SessionId = checkoutSessionModel.Id,
                    SessionVersion = checkoutSessionModel.Version,
                    SuccessIndicator = checkoutSessionModel,
                    OrderId = gatewayApiRequest.OrderId,
                    Currency = GatewayApiConfig.Currency,




                };

                return checkoutModel;


           

        }

        /// <summary>
        /// This method receives the callback from the Hosted Checkout redirect. It looks up the order using the RETRIEVE_ORDER operation and
        /// displays either the receipt or an error page.
        /// </summary>
        /// <param name="orderId">needed to retrieve order</param>
        /// <param name="result">Result of Hosted Checkout operation (success or error) - sent from hostedCheckout.html complete() callback</param>
        /// <returns>IActionResult for hosted checkout receipt page or error page</returns>
        [HttpGet("hostedCheckout/{orderId}/{result}/{billingId}")]
        public TransactionResponseModel hostedCheckout([FromRoute(Name = "orderId")] string orderId, [FromRoute(Name = "result")] string result, [FromRoute(Name = "sessionId")] string sessionId, [FromRoute(Name = "billingId")] string billingId)
        {
            // Logger.LogInformation($"PaymentApiController HostedCheckoutReceipt action orderId {orderId} result {result} sessionId {sessionId}");
            if (billingId == null)
            {
                return null;

            }
            if (result == "SUCCESS")
            {
                GatewayApiConfig.Debug = bool.Parse(_configuration["GatewayApiConfig:Debug"]);
                GatewayApiConfig.UseSsl = bool.Parse(_configuration["GatewayApiConfig:UseSsl"]);
                GatewayApiConfig.IgnoreSslErrors = bool.Parse(_configuration["GatewayApiConfig:IgnoreSslErrors"]);
                GatewayApiConfig.Version = _configuration["GatewayApiConfig:Version"];
                GatewayApiConfig.UseProxy = bool.Parse(_configuration["GatewayApiConfig:UseProxy"]);
                GatewayApiConfig.ProxyHost = _configuration["GatewayApiConfig:ProxyHost"];
                GatewayApiConfig.ProxyUser = _configuration["GatewayApiConfig:ProxyUser"];
                GatewayApiConfig.ProxyPassword = _configuration["GatewayApiConfig:ProxyPassword"];
                GatewayApiConfig.ProxyDomain = _configuration["GatewayApiConfig:ProxyDomain"];

                GatewayApiConfig.WebhooksNotificationSecret = _configuration["GatewayApiConfig:WebhooksNotificationSecret"];

                GatewayApiConfig.MerchantId = _configuration["GatewayApiConfig:GATEWAY_MERCHANT_ID"];
                GatewayApiConfig.Password = _configuration["GatewayApiConfig:GATEWAY_API_PASSWORD"];
                GatewayApiConfig.Currency = _configuration["GatewayApiConfig:GATEWAY_CURRENCY"];
                GatewayApiConfig.GatewayUrl = _configuration["GatewayApiConfig:GATEWAY_BASE_URL"];


                GatewayApiRequest gatewayApiRequest = new GatewayApiRequest(GatewayApiConfig)
                {
                    ApiOperation = "RETRIEVE_ORDER",
                    OrderId = orderId,
                    ApiMethod = GatewayApiClient.GET
                };

                gatewayApiRequest.buildOrderUrl();


                string response = GatewayApiClient.SendTransaction(gatewayApiRequest);

                // Logger.LogInformation($"Hosted checkout retrieve order response {response}");

                //parse response
                TransactionResponseModel transactionResponseModel = null;
                try
                {
                    transactionResponseModel = TransactionResponseModel.toTransactionResponseModel(response);
                }
                catch (Exception e)
                {
                    // Logger.LogError($"Hosted Checkout Receipt error : {JsonConvert.SerializeObject(e)}");
                    updateOrder(int.Parse(billingId), $"Hosted Checkout Receipt error : {JsonConvert.SerializeObject(e)}");
                    return null;
                    //return View("Error", new ErrorViewModel
                    //{
                    //    RequestId = getRequestId(),
                    //    Cause = e.InnerException != null ? e.InnerException.StackTrace : e.StackTrace,
                    //    Message = e.Message
                    //});
                }
                var jsonModel = JsonConvert.SerializeObject(transactionResponseModel).ToString();
                updateOrder(int.Parse(billingId), jsonModel);
                return transactionResponseModel;
                //  return View(ViewList.GetValueOrDefault("Receipt"), transactionResponseModel);
            }
            else
            {
                // Logger.LogError($"The payment was unsuccessful {result}");
                updateOrder(int.Parse(billingId), $"The payment was unsuccessful {result}");
                return null;
                //return View("Error", new ErrorViewModel
                //{
                //    RequestId = getRequestId(),
                //    Cause = "Payment was unsuccessful",
                //    Message = "There was a problem completing your transaction."
                //});
            }
        }



        private string getRequestId()
        {
            return Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }

        private void updateOrder(int Id, string BillingResponse)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Billings SET  IsPayed = @IsPayed , BillingResponse=@BillingResponse  Where Id = @Id";
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@IsPayed", true);
                command.Parameters.AddWithValue("@BillingResponse", BillingResponse);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}

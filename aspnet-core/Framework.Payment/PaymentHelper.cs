using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace Framework.Payment
{
   public class PaymentHelper
    {
        public static ActionLog BuildLogRequest(RestRequest request, string contentType, string uri)
        {
            ActionLog log = new ActionLog()
            {
                RequestContent = JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }),
                RequestContentType = contentType,
                RequestorIPAddress = HttpContext.Current != null? HttpContext.Current.Connection.RemoteIpAddress.ToString():"",
                RequestMethod = request.Method.ToString(),
                RequestUri = uri,
                RequestTimestamp = DateTime.Now,
                MachineName = HttpContext.Current != null ? HttpContext.Current.Request.Host.Host:"",
                UserName = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name:""
            };
            return log;
        }

        public static void BuildLogResponse(ActionLog logRequest, IRestResponse response)
        {
            logRequest.ResponseStatusCode = (int)response.StatusCode;
            logRequest.ResponseContentType = response.ContentType;
            logRequest.ResponseContent = response.Content;
            logRequest.ResponseHeaders = JsonConvert.SerializeObject(response.Headers);
            logRequest.ResponseTimestamp = DateTime.Now;
        }
    }
}

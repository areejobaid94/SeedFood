using Infoseed.MessagingPortal.MultiTenancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal
{
    public class SunshineConnectorModel
    {
        public static async Task<HttpStatusCode> PostMsgToSmooch(string appID, string conversationID, SunshinePostMsgBotModel.Content msg, string SmoochAPIKeyID, string SmoochSecretKey)
        {
            if (msg == null)
            {
                return HttpStatusCode.BadRequest;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client = await getSunshineHttpClient(SmoochAPIKeyID, SmoochSecretKey);
            var model = GetContentMessageModel(msg);
            var jsonModel = JsonConvert.SerializeObject(model,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(URL, content);
            // string resStr = result.Content.ReadAsStringAsync().Result;
            return result.StatusCode;
        }

        public static SunshinePostMsgBotModel GetContentMessageModel(SunshinePostMsgBotModel.Content content)
        {
            var  req = new SunshinePostMsgBotModel()
                {
                    author = new SunshinePostMsgBotModel.Author { type = "business" },
                    content = content
                };
         
            return req;
        }


        public static async Task<HttpClient> getSunshineHttpClient(string SmoochAPIKeyID, string SmoochSecretKey)
        {
            var client = new HttpClient();         
            string appID = SmoochAPIKeyID; //ConfigurationManager.AppSettings["smoochAppID"];
            string secretKey = SmoochSecretKey; // ConfigurationManager.AppSettings["smoochSecretKey"]; 
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(appID + ":" + secretKey);
            var encodedKey = System.Convert.ToBase64String(plainTextBytes);
            client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Basic", encodedKey);
            return client;
        }
        public static string getSunishinePostMsgEndpoint(string appID, string conversationID)
        {
            string URL = $"https://api.smooch.io/v2/apps/{appID}/conversations/{conversationID}/messages";
            return URL;
        }



    }
}

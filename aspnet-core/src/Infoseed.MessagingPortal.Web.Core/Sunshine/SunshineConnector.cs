using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public static class SunshineConnector
    {

        public static async Task<HttpStatusCode> PostMsgToSmooch(string appID, string conversationID, Content msg)
        {
            if (msg == null)
            {
                return HttpStatusCode.BadRequest;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client = await getSunshineHttpClient(appID);
            var model = GetContentMessageModel(msg);
            var jsonModel = JsonConvert.SerializeObject(model,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(URL, content);
           // string resStr = result.Content.ReadAsStringAsync().Result;
            return result.StatusCode;
        }    
        public static async Task postTextMsgToSmooch(string appID, string conversationID, string msg)
        {
            if (msg == null)
            {
                return;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client = await getSunshineHttpClient(appID);
            var model = GetTextMessageModel(msg);
            var jsonModel = JsonConvert.SerializeObject(model,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");
            // var result = await client.PostAsync(URL, content);
        }
        public static async Task postImageMsgToSmooch(string appID, string conversationID, string msg, string mediaUrl)
        {
            if (msg == null)
            {
                return;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client =  await getSunshineHttpClient(appID);
            var model = GetImageMessageModel(msg, mediaUrl);
            var jsonModel = JsonConvert.SerializeObject(model,
                          Newtonsoft.Json.Formatting.None,
                          new JsonSerializerSettings
                          {
                              NullValueHandling = NullValueHandling.Ignore
                          });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(URL, content);
        }
        public static async Task<HttpStatusCode> postVideoMsgToSmooch(string appID, string conversationID, string msg, string mediaUrl)
        {
            if (msg == null)
            {
                return HttpStatusCode.BadRequest; ;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client = await getSunshineHttpClient(appID);
            var model = GetVideoMessageModel(msg, mediaUrl);
            var jsonModel = JsonConvert.SerializeObject(model,
                          Newtonsoft.Json.Formatting.None,
                          new JsonSerializerSettings
                          {
                              NullValueHandling = NullValueHandling.Ignore
                          });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");          
            var result = await client.PostAsync(URL, content);
         

            return result.StatusCode;
        }

        public static async Task postListMsgToSmooch(string appID, string conversationID, List<HeroCard> heroCards)
        {
            if (heroCards == null)
            {
                return;
            }

            string URL = getSunishinePostMsgEndpoint(appID, conversationID);
            var client = await getSunshineHttpClient(appID);
            var model = GetListMessageModel(heroCards);
            var jsonModel = JsonConvert.SerializeObject(model,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var content = new StringContent(jsonModel.ToString(), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(URL, content);
        }
        #region Helpers
        public static SunshinePostMsgModel GetImageMessageModel(string msg, string mediaUrl)
        {
            var req = new SunshinePostMsgModel()
            {
                author = new Author() { type = "business" },
                content = new Content() { type = "image", text = msg, mediaUrl = mediaUrl }
            };
            return req;
        }
       

        public static SunshinePostMsgModel GetVideoMessageModel(string msg, string mediaUrl)
        {
            var req = new SunshinePostMsgModel()
            {
                author = new Author() { type = "business" },
                content = new Content() { type = "file", text = msg, mediaUrl = mediaUrl }
            };



            return req;
        }
        public static SunshinePostMsgModel GetContentMessageModel(Content content)
        {
            var req = new SunshinePostMsgModel();

            //if (content.mediaUrl.EndsWith(".oga"))
            //    content.type = "audio";

            if (content.type.ToLower()== "video"|| content.type.ToLower() == "audio")
            {
                req = new SunshinePostMsgModel()
                {
                    author = new Author() { type = "business" },
                    content = new Content() { 
                        type = "file",
                        text = content.text,
                        agentId= content.agentId,
                        mediaUrl= content.mediaUrl,
                        fileName=content.fileName,
                        agentName= content.agentName,
                        altText= content.altText,
                        coordinates= content.coordinates,
                        items= content.items,
                        location= content.location,
                    }
                };
            }
            else
            {
                req = new SunshinePostMsgModel()
                {
                    author = new Author() { type = "business" },
                    content = content
                };
            }
            return req;
        }
        public static SunshinePostMsgModel GetTextMessageModel(string msg)
        {
            var req = new SunshinePostMsgModel()
            {
                author = new Author() { type = "business" },
                content = new Content() { type = "text", text = msg }
            };



            return req;
        }
        public static SunshinePostMsgModel GetListMessageModel(List<HeroCard> heroCards)
        {
            var items = new List<Item>();
            foreach (var card in heroCards)
            {
                var item = new Item()
                {
                    title = card.Title,
                    description = card.Text,
                    size = "compact"


                };
                if (card != null && card.Images.Count > 0)
                {
                    item.mediaUrl = card.Images[0].Url;
                }
                var actions = new List<Models.Sunshine.Action>();
                foreach (var button in card.Buttons)
                {
                    actions.Add(new Models.Sunshine.Action()
                    {
                        text = button.Title,
                        type = "postback",
                        payload = button.Value.ToString()
                    });
                }
                item.actions = actions;

                items.Add(item);
            }

            var req = new SunshinePostMsgModel()
            {
                author = new Author() { type = "business" },
                content = new Content()
                {
                    type = "list",
                    items = items
                }
            };



            return req;
        }
        public static async Task<HttpClient> getSunshineHttpClient(string appid )
        {
            var client = new HttpClient();
            CosmosDBService cosmosDBService = new CosmosDBService();
            TenantModel tenantModel = await cosmosDBService.GetTenantByAppId(appid);

            //Make the basic dynamic = Encode AppiD:SecretKey 
            string appID = tenantModel.SmoochAPIKeyID; //ConfigurationManager.AppSettings["smoochAppID"];
            string secretKey = tenantModel.SmoochSecretKey; // ConfigurationManager.AppSettings["smoochSecretKey"]; 
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(appID+":"+secretKey);
            var encodedKey= System.Convert.ToBase64String(plainTextBytes);
            client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Basic", encodedKey);
            return client;
        }
        public static string getSunishinePostMsgEndpoint(string appID, string conversationID)
        {
            string URL = $"https://api.smooch.io/v2/apps/{appID}/conversations/{conversationID}/messages";
            return URL;
        }
        #endregion
        private static string CreateJson(object msg)
        {


            var req = new SunshinePostMsgModel()
            {
                author = new Author() { type = "business" },
                content = new Content() { type = "text", text = msg.ToString() }
            };

            var jsonData = JsonConvert.SerializeObject(req);

            return jsonData;
        }


    }
}

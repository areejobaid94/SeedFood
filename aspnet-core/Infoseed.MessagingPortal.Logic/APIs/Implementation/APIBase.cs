
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Infoseed.MessagingPortal.Logic
{
    public class APIBase : IAPIBase
    {

        public APIBase(string baseUrl, string Key)
        {
            BaseUrl = baseUrl;
            basicAuthToken = "Bearer "+Key;
        }

        public string BaseUrl { get; set; }    
        protected string basicAuthToken { get; set; }


        public string HttpGet(string url, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }
                client.DefaultRequestHeaders.Add($"Authorization", basicAuthToken);

                var result = client.GetAsync(url).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return resStr;
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }
        public JObject[] HttpGetArray(string url, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }
                client.DefaultRequestHeaders.Add($"Authorization", basicAuthToken);

                var result = client.GetAsync(url).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<JObject[]>(resStr);
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }   
        public virtual string HttpPost(string url, object body, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }


                var Jso = JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                HttpContent content = new StringContent(Jso, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add($"Authorization", basicAuthToken);
                var result = client.PostAsync(url, content).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return resStr;
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }
        public virtual string HttpPut(string url, object body, Dictionary<string, string> queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (queryString != null)
                {
                    StringBuilder b = QueryStringBuilder(url, queryString);
                    url = b.ToString();
                }
                var Jso = JsonConvert.SerializeObject(body);
                HttpContent content = new StringContent(Jso, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add($"Authorization", basicAuthToken);
                var result = client.PutAsync(url, content).Result;
                string resStr = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return resStr;
                }
                else
                {
                    throw new Exception(resStr);
                }
            }
        }

        private static StringBuilder QueryStringBuilder(string url, Dictionary<string, string> queryString)
        {
            StringBuilder b = new StringBuilder(url);
            var first = queryString.First();
            b = b.Append($"?{first.Key}={first.Value}");
            queryString.Remove(first.Key);
            foreach (var item in queryString)
            {
                b = b.Append($"{item.Key}={item.Value}");
            }

            return b;
        }
        private static string Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }

    }
}

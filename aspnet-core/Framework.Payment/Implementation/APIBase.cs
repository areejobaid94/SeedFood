using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Framework.Payment.Interfaces;
using RestSharp;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Framework.Payment.Model;
using System.Net;
using Framework.Data;
using System.Data;
using System.Data.SqlClient;

namespace Framework.Payment.Implementation
{
    public class APIBase : IAPIBase
    {
        public string BaseUrl { get; set; }
        public static string basicAuthToken { get; set; }
        public static string access_token { get; set; } 
        public static string refresh_token { get; set; } 
        public IConfiguration _configuration { get; set; }

        public APIBase( IConfiguration configuration)
        {
             _configuration =configuration;         
          //  basicAuthToken = "Bearer "+Key;
        }

        public void RefreshToken()
        {
            var model = getZohoAccessToken();

            var client = new RestClient("https://accounts.zoho.com/oauth/v2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "_zcsr_tmp=6a67ec77-3364-4b51-b0e8-efa20cdea769; b266a5bf57=a711b6da0e6cbadb5e254290f114a026; iamcsr=6a67ec77-3364-4b51-b0e8-efa20cdea769");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("client_id", model.client_id);
            request.AddParameter("client_secret", model.client_secret);
            request.AddParameter("refresh_token", model.refresh_token);
            request.AddParameter("grant_type", "refresh_token");
            IRestResponse response = client.Execute(request);

            var rez = JsonConvert.DeserializeObject<AccessTokenModel>(response.Content);

            model.access_token=rez.access_token;
            access_token =rez.access_token;
            basicAuthToken = "Bearer "+access_token;

            UpdateZohoAccessToken(model);

        }



        public string HttpGet(string url, Dictionary<string, string> queryString = null)
        {
   
            if (basicAuthToken==null)
            {
                UpdatedData();
            }
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
                    if (result.StatusCode==HttpStatusCode.Unauthorized)
                    {
                        RefreshToken();
                        return HttpGet(url);
                    }
                    else
                    {
                        throw new Exception(resStr);
                    }
                    
                }
            }
        }

        private void UpdatedData()
        {
            var model = getZohoAccessToken();

            access_token=model.access_token;
            refresh_token=model.refresh_token;
            basicAuthToken = "Bearer "+access_token;


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



        private void UpdateZohoAccessToken(AccessTokenModel modek)
        {
            try
            {

                var SP_Name = "ZohoAccessTokenUpdate";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@access_token",modek.access_token),
                     new System.Data.SqlClient.SqlParameter("@refresh_token",modek.refresh_token),
                     new System.Data.SqlClient.SqlParameter("@expires_in",3600),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), this._configuration["ConnectionStrings:Default"]);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AccessTokenModel getZohoAccessToken()
        {
            try
            {
                AccessTokenModel liveChatEntity = new AccessTokenModel();
                var SP_Name = "GetZohoAccessToken";

                var sqlParameters = new List<SqlParameter>
                {
                };

                liveChatEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapAccessToken, this._configuration["ConnectionStrings:Default"]).FirstOrDefault();


                return liveChatEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static AccessTokenModel MapAccessToken(IDataReader dataReader)
        {
            try
            {
                AccessTokenModel model = new AccessTokenModel();
                model.access_token = SqlDataHelper.GetValue<string>(dataReader, "access_token");
                model.refresh_token = SqlDataHelper.GetValue<string>(dataReader, "refresh_token");
                model.client_secret = SqlDataHelper.GetValue<string>(dataReader, "client_secret");
                model.client_id = SqlDataHelper.GetValue<string>(dataReader, "client_id");
                model.redirect_uri = SqlDataHelper.GetValue<string>(dataReader, "redirect_uri");
                model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

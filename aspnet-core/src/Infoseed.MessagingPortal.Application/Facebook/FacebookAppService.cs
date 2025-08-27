using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Framework.Data;
using Infoseed.MessagingPortal.ChatStatuses;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Contacts.Exporting;
using Infoseed.MessagingPortal.ContactStatuses;
using Infoseed.MessagingPortal.FacebookDTO;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Notifications;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebJobEntities;
using static Infoseed.MessagingPortal.FacebookDTO.DTO.PostFacebookMessageModel;

namespace Infoseed.MessagingPortal.Facebook
{
    public class FacebookAppService : MessagingPortalAppServiceBase, IFacebookAppService
    {

        private readonly IDocumentClient _IDocumentClient;
        private readonly ICacheManager _cacheManager;

        public FacebookAppService(IDocumentClient iDocumentClient, ICacheManager cacheManager)

        {
            _IDocumentClient = iDocumentClient;
            _cacheManager = cacheManager;

        }
        private string GetFacebookPageAccessToken(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[TenantFacebookPageAccessToken] where TenantId=" + TenantID+" and Channel=N'facebook'";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            string token =  "";

            int Id = 0;

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                token =dataSet.Tables[0].Rows[i]["PageAccessToken"].ToString();
            



            }

            conn.Close();
            da.Dispose();

            return token;
        }
        public async Task<bool> SubscribePage(string pageId, string pageAccessToken, bool isSubscribe)
        {
            var client = new HttpClient();
            var url = $"https://graph.facebook.com/v19.0/{pageId}/subscribed_apps?access_token={pageAccessToken}";

            HttpResponseMessage response;

            if (isSubscribe)
            {
                var subscribedFields = new
                {
                    subscribed_fields = new[]
                    {
                "messages",
                "message_deliveries",
                "message_reads",
                "messaging_postbacks"
                    }
                };

                var jsonBody = JsonSerializer.Serialize(subscribedFields);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, content);


            }
            else
            {
                // Unsubscribe (DELETE request)
                response = await client.DeleteAsync(url);


                pageId="";
                pageAccessToken="";

            }

            var result = await response.Content.ReadAsStringAsync();

            //if (!response.IsSuccessStatusCode)
            //{
            //    return false;
            //}


            await updateTenant(pageId, pageAccessToken);
            return true;
        }
        private string GetFacebookPageId(int TenantID)
        {
            //TenantID = "31";
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where Id=" + TenantID+"";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            string token = "";

            int Id = 0;

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {

                token =dataSet.Tables[0].Rows[i]["FacebookPageId"].ToString();




            }

            conn.Close();
            da.Dispose();

            return token;
        }
        public async Task<FacebookPagesModel?> GetFacebookPages(string userToken)
        {


            if (string.IsNullOrEmpty(userToken))
            {
                var token = GetFacebookPageAccessToken(AbpSession.TenantId.Value);
 
                userToken=token;
            }else
            {


            }


            try
            {

                // userToken="EAAMlb5ZBkNS0BO1oe0AF92KETb4ZB2TM771J7534YeI0cvbAbmrp1FwLS2BcD35ZCZAHuSzKG7Iqctu0sK8djaZBNWPZBOLgmmV2UDeVob7K0BjBiSEt1YcLV2NKA0emRRm8TZA3FYH6XNbvu3tEcAKpmi7rGzjoGIkTVGHdWZBLY7PvETS323vFsLjoY8o5Dq1zXwXelUEQ02o7";
                var url = $"https://graph.facebook.com/v19.0/me/accounts?access_token={userToken}";

                using var client1 = new HttpClient();
                var response = await client1.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Failed to get pages: " + content);
                    return null;
                }

                var pages = JsonSerializer.Deserialize<FacebookPagesModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pages?.data == null || pages.data.Length == 0)
                    return pages;



              //  var pagID = GetFacebookPageId(AbpSession.TenantId.Value);

              //var p=  pages.data.Where(x => x.id==pagID).FirstOrDefault();
              //  await SubscribePage(p.id, p.access_token, true);

              //  pages.data=pages.data.Where(x => x.id==pagID).ToArray();

                // Loop through pages to check subscription status
                foreach (var page in pages.data)
                {
                    var checkUrl = $"https://graph.facebook.com/v19.0/{page.id}/subscribed_apps?access_token={page.access_token}";

                    var subResponse = await client1.GetAsync(checkUrl);
                    var subJson = await subResponse.Content.ReadAsStringAsync();

                    if (subResponse.IsSuccessStatusCode)
                    {
                        using var subDoc = JsonDocument.Parse(subJson);
                        var subData = subDoc.RootElement.GetProperty("data");
                        page.isSubscribe = subData.GetArrayLength() > 0;
                    }
                    else
                    {
                        page.isSubscribe = true; // default to false on failure
                    }

                    await SubscribePage(page.id, page.access_token, true);
                }
                //pages.data.FirstOrDefault().isSubscribe=true;






                //_cacheManager.GetCache("CacheTenant").Remove(pages.data[0].id.ToString());


                //try
                //{
                //    var client = new HttpClient();
                //    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.EngineApi+"api/WhatsAppAPI/DeleteCache?phoneNumberId="+pages.data[0].id.ToString()+"&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges");
                //    request.Headers.Add("accept", "text/plain");
                //    var response1 = await client.SendAsync(request);
                //    response1.EnsureSuccessStatusCode();
                //    Console.WriteLine(await response1.Content.ReadAsStringAsync());
                //}
                //catch
                //{


                //}


                //try
                //{
                //    var client = new HttpClient();
                //    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/RestaurantsChatBot/DeleteCache?TenantId="+AbpSession.TenantId.Value);
                //    request.Headers.Add("accept", "text/plain");
                //    var response1 = await client.SendAsync(request);
                //    response1.EnsureSuccessStatusCode();


                //}
                //catch
                //{

                //}




                return pages;
            }
            catch(Exception ex)
            {

                return null;

            }
          
        }
        public async Task<string> GetPageAccessToken(string code)
        {
            var accessToken = "";
            if (string.IsNullOrEmpty(code))
            {

                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);


                accessToken=tenant.FacebookAccessToken;
            }
            else
            {
                try
                {
                    var clientId = "885586280068397";
                    var clientSecret = "6bb8de39780aefe14f7a49a4acbf0c98";
                    var redirectUri = "https://waapi.info-seed.com/app/admin/facebook-connect";
                   // var redirectUri = "https://teaminbox-stg.azurewebsites.net/app/admin/facebook-connect";

                    var requestUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                                     $"?client_id={clientId}" +
                                     $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                                     $"&client_secret={clientSecret}" +
                                     $"&code={code}";

                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(requestUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new Exception("Facebook token exchange failed.");
                    }


                    var json = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(json);
                    accessToken = doc.RootElement.GetProperty("access_token").GetString();

                    await updateOrAddTenantFacebookPageAccessToken(accessToken, "facebook");
                }
                catch
                {
                    var token = GetFacebookPageAccessToken(AbpSession.TenantId.Value);

                    code=token;

                    //var clientId = "885586280068397";
                    //var clientSecret = "6bb8de39780aefe14f7a49a4acbf0c98";
                    //var redirectUri = "https://teaminbox-stg.azurewebsites.net/app/admin/facebook-connect";

                    //var requestUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                    //                 $"?client_id={clientId}" +
                    //                 $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                    //                 $"&client_secret={clientSecret}" +
                    //                 $"&code={code}";

                    //using var httpClient = new HttpClient();
                    //var response = await httpClient.GetAsync(requestUrl);

                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    var errorContent = await response.Content.ReadAsStringAsync();
                    //    throw new Exception("Facebook token exchange failed.");
                    //}


                    //var json = await response.Content.ReadAsStringAsync();

                    //using var doc = JsonDocument.Parse(json);
                    //accessToken = doc.RootElement.GetProperty("access_token").GetString();

                    //await updateOrAddTenantFacebookPageAccessToken(accessToken, "facebook");
                }


            }



            return accessToken!;
        }




        public async Task<string> CheckInstagram()
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);

            if(string.IsNullOrEmpty(tenant.InstagramId))
            {


                return "";
            }

            var username = await GetInstagramUserIdAsync(tenant.InstagramAccessToken);

            return username;
        }
        public async Task<string> GetInstagramToken(string code, string f3_request_id, string ig_app_id)
        {

            try
            {
                var clientId = "907410431131999";
                var clientSecret = "de72129b2d82829d7bc6894a4af31d50";
                var redirectUri = "https://waapi.info-seed.com/app/admin/instagram-connect";
                //var redirectUri = "https://teaminbox-stg.azurewebsites.net/app/admin/instagram-connect";


                var client = new HttpClient();
                var requestUrl = "https://api.instagram.com/oauth/access_token";

                var formContent = new MultipartFormDataContent
             {
                 { new StringContent(clientId), "client_id" },
                 { new StringContent(clientSecret), "client_secret" },
                 { new StringContent("authorization_code"), "grant_type" },
                 { new StringContent(redirectUri), "redirect_uri" },
                 { new StringContent(code), "code" }
             };

                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = formContent
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var accessToken = doc.RootElement.GetProperty("access_token").GetString();






                var token = await GetLongLivedInstagramTokenAsync(accessToken);
                var ID = await GetInstagramUserIdAsync(token);
                if (string.IsNullOrEmpty(ID))
                {
                    return "";

                }
                var id = ID.Split(",")[0];

                await InstagramupdateTenant(id, token);
                await updateOrAddTenantFacebookPageAccessToken(token, "instagram");


                return ID!;
            }
            catch
            {

                return code;

            }
           
        }
        public async Task DeleteInstagramAsync()
        {

            await  InstagramupdateTenant("","");

        }


      



        private async Task updateOrAddTenantFacebookPageAccessToken(string PageAccessToken, string Channel)
        {

            try
            {

                //var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                //var tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);


                //tenant.FacebookAccessToken=PageAccessToken;

                //await itemsCollection.UpdateItemAsync(tenant._self, tenant);


                var SP_Name = "updateOrAddTenantFacebookPageAccessToken";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                    new System.Data.SqlClient.SqlParameter("@PageAccessToken",PageAccessToken),
                    new System.Data.SqlClient.SqlParameter("@Channel",Channel),

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


            }
            catch
            {

            }

        }
  


        private async Task InstagramupdateTenant(string InstagramId, string InstagramAccessToken)
        {
            try
            {
                var SP_Name = "TenantUpdateInstagram";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                    new System.Data.SqlClient.SqlParameter("@InstagramId",InstagramId),
                    new System.Data.SqlClient.SqlParameter("@InstagramAccessToken",InstagramAccessToken),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


            }
            catch
            {

            }


            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);
            tenant.InstagramId = InstagramId;
            tenant.InstagramAccessToken = InstagramAccessToken;
            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
            // _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());


            //_cacheManager.GetCache("CacheTenant").Remove(tenant.InstagramId.ToString());
            await RemoveCache("", InstagramId, tenant);



        }
        private async Task<string> GetLongLivedInstagramTokenAsync(string shortLivedToken)
        {
            var clientSecret = "de72129b2d82829d7bc6894a4af31d50"; // Your App Secret

            using var client = new HttpClient();

            var requestUrl = $"https://graph.instagram.com/access_token" +
                             $"?grant_type=ig_exchange_token" +
                             $"&client_secret={clientSecret}" +
                             $"&access_token={shortLivedToken}";

            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var longLivedAccessToken = doc.RootElement.GetProperty("access_token").GetString();

            return longLivedAccessToken;
        }
        private async Task<string> GetInstagramUserIdAsync(string accessToken)
        {
            try
            {
                using var client = new HttpClient();

                var requestUrl = $"https://graph.instagram.com/v22.0/me?fields=user_id,username,profile_picture_url&access_token={accessToken}";

                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);
                var userId = document.RootElement.GetProperty("user_id").GetString();
                var username = document.RootElement.GetProperty("username").GetString();
                var profile_picture_url = document.RootElement.GetProperty("profile_picture_url").GetString();

                return userId+","+username+","+profile_picture_url;
            }
            catch
            {

                return "";
            }
          
        }





        private async Task updateTenant(string FacebookPageId, string FacebookAccessToken)
        {
            try
            {
                var SP_Name = "TenantUpdateFacebook";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                    new System.Data.SqlClient.SqlParameter("@FacebookPageId",FacebookPageId),
                    new System.Data.SqlClient.SqlParameter("@FacebookAccessToken",FacebookAccessToken),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


            }
            catch
            {

            }


            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);
            tenant.FacebookPageId = FacebookPageId;
            tenant.FacebookAccessToken = FacebookAccessToken;
            await itemsCollection.UpdateItemAsync(tenant._self, tenant);

            await RemoveCache(FacebookPageId,"", tenant);

        }

        private async Task RemoveCache(string FacebookPageId, string InstagramId, TenantModel tenant)
        {

            if (!string.IsNullOrEmpty(FacebookPageId))
            {
                _cacheManager.GetCache("CacheTenant").Remove(tenant.FacebookPageId.ToString());
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.EngineApi+"api/WhatsAppAPI/DeleteCache?phoneNumberId="+FacebookPageId+"&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges");
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                catch
                {


                }


                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/RestaurantsChatBot/DeleteCache?TenantId="+tenant.TenantId);
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                }
                catch
                {

                }


                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/FlowsChatBot/FlowsBotDeleteCache?TenantId="+tenant.TenantId);
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                }
                catch
                {

                }

            }
            if (!string.IsNullOrEmpty(InstagramId))
            {
                _cacheManager.GetCache("CacheTenant").Remove(tenant.InstagramId.ToString());

                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.EngineApi+"api/WhatsAppAPI/DeleteCache?phoneNumberId="+InstagramId+"&apiKey=oixCrEB8X12y9TseyjOubhFRHiU0Q2wQBfX8Ges");
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                catch
                {


                }


                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/RestaurantsChatBot/DeleteCache?TenantId="+tenant.TenantId);
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                }
                catch
                {

                }


                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/FlowsChatBot/FlowsBotDeleteCache?TenantId="+tenant.TenantId);
                    request.Headers.Add("accept", "text/plain");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                }
                catch
                {

                }
            }
      

        }
    }
}

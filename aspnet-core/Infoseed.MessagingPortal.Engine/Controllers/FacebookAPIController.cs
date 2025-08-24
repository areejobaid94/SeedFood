using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Web.Models;
using DocumentFormat.OpenXml.Drawing;
using Framework.Data;
using Framework.Integration.Implementation;
using Framework.Integration.Interfaces;
using Framework.Integration.Model;
using Hangfire.Storage;
using Infoseed.MessagingPortal.Engine.Model;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Dashboard;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Spatial;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebJobEntities;
using static Infoseed.MessagingPortal.Constants;

namespace Infoseed.MessagingPortal.Engine.Controllers
{
    public class FacebookAPIController : MessagingPortalControllerBase
    {
        private ILiveChatAppService _iliveChat;
        //private string fbToken = "EAAKTbPZAaKtYBAGxYaZAa3JckCvl7QDd1wbK8w9MNrwjsvMkUDKBFDoHKVKgE9JaYNkUc4C1IvdxQgn73nLPQ81zW6bhbfflnfZC2xpG7ofzGqP2T7YXCSu7LbWccPVJVdafiCFw5UnyikowudxmYW9VLzEcvbobqlW4ZBqe47IUHid3IbZBC6SmC9GyiJBGkrV1cAmuZAEQZDZD";
        //private string postUrl = "https://graph.facebook.com/v17.0/103674912368849/messages";
        public string URLG = "https://d51c-109-237-205-93.ngrok-free.app";
        //public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindbqa.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";

        //  public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindb.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";
        IDBService _dbService;
        // private IHubContext<SignalR.TeamInboxHub> _hub;
        public IContactsAPI _contactsAPI;
        public ITicketsAPI _ticketsAPI;
        private readonly ICacheManager _cacheManager;

        private readonly IDocumentClient _IDocumentClient;
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        public FacebookAPIController(
            IDBService dbService
               , ICacheManager cacheManager
             , IDocumentClient iDocumentClient
             , ILiveChatAppService iliveChat
             , TenantDashboardAppService tenantDashboardAppService
            )
        {
            _dbService = dbService;
            _contactsAPI = new ContactsAPI(SettingsModel.MgUrl, SettingsModel.MgKey);
            _cacheManager = cacheManager;
            _IDocumentClient = iDocumentClient;
            _iliveChat = iliveChat;
            _tenantDashboardAppService = tenantDashboardAppService;
        }
        private async Task MoveToStg(FacebookModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedengineapi-stg.azurewebsites.net/API/WhatsAppAPI/webhook");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }
        }
        private async Task MoveToQa(FacebookModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://infoseedengineapi-qa.azurewebsites.net/API/WhatsAppAPI/webhook");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }
        }
        private void SetConversationMeasurmentsInQueue(ConservationMeasurementMessage message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("conservation-measurements");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }

        //[HttpPost("send-message")]
        //public async Task<IActionResult> SendMessage()
        //{
        //    const string AccessToken = "EAAMlb5ZBkNS0BO5Aqu9RsqR8iCNYfCY9tsG2zseNrkbR23zZBr11LTqA8JTG331depSm7mtKIBCoCGUiqmQgUgUNLRzOsuVDbTqv0ih2zS6Iq9WjZC8eAhpWo7be5gAaU9nMUDZBhSsJ3xxV7Q3hlwI3osZBjISK15yJ0zfWzBnqbKriLixUlwSGtS7tVK2UJX2okTL8ddHMYA1glGRQ85ZCyMo1toLOLq74oZD";
        //    const string UserId = "9446932605376530";
        //    var url = $"https://graph.facebook.com/v22.0/me/messages?access_token={AccessToken}";
                 
        //var payload = new
        //    {
        //        recipient = new { id = UserId },
        //        messaging_type = "RESPONSE",
        //        message = new { text = "Hello, world!" }
        //    };
        //    string jsonPayload = NemonsoftJsonJsonSerializer.Serialize(payload);

        //    using (var httpClient = new HttpClient())
        //    {
        //        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        //        var response = await httpClient.PostAsync(url, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseData = await response.Content.ReadAsStringAsync();
        //            return Ok(new { status = "success", message = "Message sent successfully!", data = responseData });
        //        }
        //        else
        //        {
        //            var errorData = await response.Content.ReadAsStringAsync();
        //            return StatusCode((int)response.StatusCode, new { status = "error", message = "Failed to send message", details = errorData });
        //        }
        //    }
        //}

        [HttpGet("Facebookwebhook1")]
        [DontWrapResult]
        public string Webhook(
         [FromQuery(Name = "hub.mode")] string mode,
         [FromQuery(Name = "hub.challenge")] string challenge,
         [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            string currentDattime = "InfoSeed-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            if (verify_token.Equals(currentDattime) && mode.Equals("subscribe"))
            {
                return challenge;
            }
            else
            {
                return null;
            }
        }


        [HttpPost("Facebookwebhook1")]
        public async Task<IActionResult> Webhook([FromBody] dynamic jsonData2)
        {
            try
            {
                var aaaa = JsonConvert.SerializeObject(jsonData2);
                FacebookModel jsonData = JsonConvert.DeserializeObject<FacebookModel>(aaaa);

                if (jsonData.Entry[0].Messaging.FirstOrDefault().Message==null)
                {
                    return Ok();

                }
                // Process the data from the webhook
                var phoneNumberId = "";
                string userId = string.Empty;
                if (jsonData.Entry != null && jsonData.Entry.Count > 0)
                {
                    var messaging = jsonData.Entry[0].Messaging;
                    if (messaging != null && messaging.Count > 0)
                    {
                        userId = messaging[0].Sender.Id;
                        phoneNumberId = messaging[0].Recipient.Id;
                        //var messageText = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text;
                    }
                }

                // Fetch tenant information
                TenantModel Tenant = new TenantModel();
                var tenantId = jsonData.Entry[0].Messaging.FirstOrDefault().Recipient.Id;
                var objTenant = _cacheManager.GetCache("CacheTenant").Get(tenantId.ToString(), cache => cache);
                if (objTenant.Equals(tenantId.ToString()))
                {
                    Tenant = await _dbService.GetTenantByFacebookId("", tenantId.ToString());
                    if (Tenant != null)
                    {
                        _cacheManager.GetCache("CacheTenant").Set(tenantId.ToString(), Tenant);
                    }
                }
                else
                {
                    Tenant = (TenantModel)objTenant;
                }

                if (jsonData.Entry[0].Messaging[0].Sender.Id == null)
                {
                    return Ok();
                }





                   
                // Process message if it exists
                if (jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text != null)
                {
                    ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
                    {
                        TenantId = Tenant.TenantId.Value,
                        MessageStatusId = 4,
                        MessageDateTime = DateTime.Now,
                        MessageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid
                    };
                    SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
                    //var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;

                }

                // Extract customer information
                var name2 = jsonData.Entry[0].Messaging[0].Sender.Name ?? jsonData.Entry[0].Messaging[0].Sender.Id;
                var messageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid;
                var from = jsonData.Entry[0].Messaging[0].Sender.Id;
                userId = (Tenant.TenantId + "_" + from).ToString();
              //  string msgBody = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text;
                string medaiUrl = string.Empty;
                string interactiveId = string.Empty;
                var type = "text";

              // Initialize WhatsApp service
              WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();





                var msgBody = whatsAppAppService.MassageTypeFaceBook(jsonData.Entry[0].Messaging, Tenant.FacebookAccessToken, tAttachments, ref medaiUrl, attachmentMessageModel, out interactiveId);


                type= interactiveId;

                // Retrieve customer information from Cosmos DB
                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                var Customer = await customerResult;
                var avatarUrl = "avatar3";
                var Gender = "";
                FacebookUserInfoModel facebookUserInfoModel = new FacebookUserInfoModel(); 
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/"+from+"?fields=first_name,last_name,profile_pic,gender&access_token="+Tenant.FacebookAccessToken);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    facebookUserInfoModel = JsonConvert.DeserializeObject<FacebookUserInfoModel>(jsonString);

                    name2=facebookUserInfoModel.first_name+" "+facebookUserInfoModel.last_name;
                    avatarUrl=facebookUserInfoModel.profile_pic;
                    Gender=facebookUserInfoModel.gender;
                }
                catch
                {


                }

                facebookUserInfoModel.profile_pic="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/facebook.png";
                facebookUserInfoModel.first_name=name2;
                facebookUserInfoModel.last_name=name2;
                facebookUserInfoModel.gender="male";

                // name2=facebookUserInfoModel.first_name+" "+facebookUserInfoModel.last_name;
                avatarUrl ="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/facebook.png";//facebookUserInfoModel.profile_pic;
                Gender="male";// facebookUserInfoModel.gender;



                if (Customer == null)
                {
                    Customer = _dbService.CreateNewCustomerFacebook(from, name2, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId, avatarUrl, Gender);
                    Customer.customerChat.text = msgBody;
                }
                if (string.IsNullOrEmpty(Customer.displayName))
                {
                    Customer.displayName=from;
                }
                if (Customer.creation_timestamp==0)
                {
                    Customer.creation_timestamp =  int.Parse(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                   Customer.expiration_timestamp =   int.Parse(DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString());
                    Customer.IsConversationExpired=false;
                }

                var nowd = int.Parse(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                if (nowd>Customer.expiration_timestamp)
                {

                    Customer.creation_timestamp =  int.Parse(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                    Customer.expiration_timestamp =   int.Parse(DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString());
                    Customer.IsConversationExpired=false;

                }


                Customer.displayName=name2;
                Customer.channel="facebook";
                Customer.avatarUrl=avatarUrl;
                Customer.gender=Gender;
                Customer.facebookUserInfoModel=facebookUserInfoModel;

                if (Customer.expiration_timestamp != 0)
                {
                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    // Check if the expiration time has passed
                    if (currentTimestamp > Customer.expiration_timestamp)
                    {
                        Customer.IsLockedByAgent = false;
                        Customer.IsOpen = false;
                        Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };
                        Customer.IsHumanhandover = false;
                        //Console.WriteLine("The time has expired.");
                    }
                    else
                    {
                        //Console.WriteLine("The time has not expired yet.");
                    }

                }

                // Process if tenant bundle is not active
                if (Customer.CustomerStepModel == null)
                {
                    Customer.CustomerStepModel = new CustomerStepModel() { ChatStepId = -1, ChatStepPervoiusId = 0, IsLiveChat = false, UserParmeter = new Dictionary<string, string>() };
                }
                Customer.CustomerStepModel.UserParmeter.Remove("ContactID");
                Customer.CustomerStepModel.UserParmeter.Add("ContactID", Customer.ContactID.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("TenantId");
                Customer.CustomerStepModel.UserParmeter.Add("TenantId", Customer.TenantId.ToString());
                Customer.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                Customer.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer.phoneNumber.ToString());

                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Name");
                        Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Name");
                    Customer.CustomerStepModel.UserParmeter.Add("Name", Customer.displayName);
                }

                try
                {
                    if (!Customer.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                    {
                        Customer.CustomerStepModel.UserParmeter.Remove("Location");
                        Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                    }
                }
                catch
                {
                    Customer.CustomerStepModel.UserParmeter.Remove("Location");
                    Customer.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                }
                if (string.IsNullOrEmpty(Customer.displayName))
                {
                    Customer.displayName = name2;
                }
                if (Customer.IsBlock)
                {
                    return Ok();
                }
                if (Tenant == null)
                {
                    return Ok();
                }
                if (!Tenant.IsBundleActive)
                {
                    return Ok();
                }
                if (!Tenant.IsBotActive)
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.to = from;
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
                    postWhatsAppMessageModel.text.body = Tenant.MassageIfBotNotActive;
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.FacebookAccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        Content message = new Content();
                        message.text = Tenant.MassageIfBotNotActive;
                        message.agentName = Tenant.botId;
                        message.agentId = "1000000";
                        message.type = "text";
                        var massageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid;


                        var CustomerSendChat2 = _dbService.UpdateCustomerChat(Customer, userId, msgBody, "text", Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId, null, null);
                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, message.text, message.type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.TeamInbox, massageId, null, null);

                        SocketIOManager.SendChat(Customer, Tenant.TenantId.Value);

                    }
                   
                    return Ok();
                }
                if (!Tenant.IsPaidInvoice)
                {
                    return Ok();
                }


                if (attachmentMessageModel.IsHasAttachment)
                {

                    CustomerChat customerChat = new CustomerChat()
                    {
                        messageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid,
                        TenantId = Tenant.TenantId.Value,
                        userId = userId,
                        text = medaiUrl,
                        type = type,//hjhj
                        CreateDate = DateTime.Now.AddSeconds(-3),
                        status = (int)Messagestatus.New,
                        sender = MessageSenderType.Customer,
                        ItemType= InfoSeedContainerItemTypes.ConversationItem,
                        mediaUrl = medaiUrl,
                        UnreadMessagesCount = 0,
                        agentName = "admin",
                        agentId = "",
                    };

                    Customer.customerChat=customerChat;


                    var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid);

                    attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);

                    //SetAttachmentMessageInQueue(attachmentMessageModel);
                }
                else
                {
                    // var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, "text", Tenant.TenantId.Value, 0, "", string.Empty, string.Empty, MessageSenderType.Customer, jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid, null,  jsonData.Entry[0].Changes[0].Value.Messages[0].referral);
                    var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, "", string.Empty, string.Empty, MessageSenderType.Customer, jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid, null, null);
                    Customer.customerChat = CustomerSendChat;

                    if (Customer.IsConversationExpired)
                    {
                        Customer.IsConversationExpired = false;
                    }
                }
                if (Tenant.IsWorkActive && !Customer.IsOpen)
                {
                    string SFormat = string.Empty;
                    /// out of working hours  
                    if (!checkIsInService(Tenant.workModel, out SFormat))
                    {
                        PostWhatsAppMessageModel postWhatsAppMessageModel1 = new PostWhatsAppMessageModel();
                        postWhatsAppMessageModel1.type = "text";
                        postWhatsAppMessageModel1.to = from;
                        postWhatsAppMessageModel1.text = new PostWhatsAppMessageModel.Text();
                        postWhatsAppMessageModel1.text.body = SFormat;
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel1, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {
                            Content message = new Content();
                            message.text = SFormat;
                            message.agentName = Tenant.botId;
                            message.agentId = "1000000";
                            message.type = "text";
                            var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                            Customer.CreateDate = CustomerChat.CreateDate;
                            Customer.customerChat = CustomerChat;
                            // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        }
                        return Ok();
                    }
                }
                if (Customer.customerChat != null)
                {
                    if (Customer.IsOpen && Customer.customerChat.sender == MessageSenderType.Customer)
                    {
                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, true, Customer.ConversationsCount, Customer.creation_timestamp, Customer.expiration_timestamp);
                    }
                    else
                    {
                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, false, 0, Customer.creation_timestamp, Customer.expiration_timestamp);
                    }
                }
                if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
                {
                    await PrepareBotChatWithCustomer(from, Tenant, Customer, msgBody, tAttachments, string.Empty);

                }
                else
                {
                    SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                    return Ok();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, "An error occurred while processing the request.");
            }

        }


        private void SetAttachmentMessageInQueue(AttachmentMessageModel message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("attachment-messages");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {
                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }
        }

        
        private async Task<bool> PrepareBotChatWithCustomer(string from, TenantModel Tenant, CustomerModel Customer, string msg, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string interactiveId)
        {
            Customer.TennantPhoneNumberId = Tenant.D360Key;
            Customer.interactiveId = interactiveId;
            Customer.attachments = tAttachments;
            List<Activity> Bot = new List<Activity>();
            if (Tenant.botId == "RestaurantBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToRestaurantsBot(Customer);
                return true;
            }
            else if (Tenant.botId == "BookingBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToBookingChatBott(Customer);
                return true;
            }
            else if (Tenant.botId == "FlowsBot")
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                SendToFlowsChatBot(Customer);
                return true;
            }
            else
            {
                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
                var aaaa = JsonConvert.SerializeObject(Customer.customerChat);

                if ((string.IsNullOrEmpty(Tenant.botId) || string.IsNullOrEmpty(Tenant.DirectLineSecret)) && (!Tenant.BotTemplateId.HasValue))
                {
                    return false;
                }
                DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
                var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
                Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments, Tenant.BotTemplateId, interactiveId).Result;
                List<Activity> botListMessages = new List<Activity>();

                foreach (var msgBot in Bot)
                {

                    if (msgBot.Text.Contains("The process cannot access the file") || msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
                    {

                    }
                    else
                    {
                        botListMessages.Add(msgBot);
                    }


                }

                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();


                foreach (var msgBot in botListMessages)
                {


                    List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, from, Tenant.botId);

                    foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                    {
                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                        if (result)
                        {

                            //var message = PrepareMessageContent(msgBot, Tenant.botId);
                            //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);

                            WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, Tenant.botId, Customer.userId, Customer.TenantId.Value, Customer.ConversationId);
                            var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);
                            var aaaa2 = JsonConvert.SerializeObject(CustomerChat);
                            Customer.customerChat = CustomerChat;
                            //await _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                            SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        }
                    }

                }
            }

            return true;
        }
        private async Task SendToRestaurantsBot(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/RestaurantsChatBot/RestaurantsMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }




        }
        private async Task SendToBookingChatBott(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/BookingChatBot/BookingMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }




        }
        private async Task SendToFlowsChatBot(CustomerModel jsonData)
        {
            try
            {

                //if (jsonData.phoneNumber=="962779746365")
                //{

                //    AppSettingsModel.BotApi ="https://e787-176-29-133-35.ngrok-free.app/";
                //}
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi + "api/FlowsChatBot/FlowsBotMessageHandler");
                request.Headers.Add("accept", "*/*");
                var content = new StringContent(constra
                    , null, "application/json-patch+json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch
            {

            }
        }
        private DateTime getValidValue(dynamic value)
        {
            DateTime result = DateTime.MinValue;
            try
            {
                result = DateTime.Parse(value.ToString());
                return result;
            }
            catch (Exception)
            {
                return result;
                throw;
            }

        }


        private bool checkIsInService(Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel workModel, out string outWHMessage)
        {

            bool result = true;
            outWHMessage = string.Empty;
            DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
            DayOfWeek wk = currentDateTime.DayOfWeek;
            TimeSpan timeOfDay = currentDateTime.TimeOfDay;
            var options = new JsonSerializerOptions { WriteIndented = true };

            switch (wk)
            {
                case DayOfWeek.Saturday:
                    if (workModel.IsWorkActiveSat)
                    {
                        var StartDateSat = getValidValue(workModel.StartDateSat).AddHours(AppSettingsModel.AddHour);
                        var EndDateSat = getValidValue(workModel.EndDateSat).AddHours(AppSettingsModel.AddHour);

                        var StartDateSatSP = getValidValue(workModel.StartDateSatSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSatSP = getValidValue(workModel.EndDateSatSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }

                    break;
                case DayOfWeek.Sunday:
                    if (workModel.IsWorkActiveSun)
                    {
                        var StartDate = getValidValue(workModel.StartDateSun).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateSun).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateSunSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateSunSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextSun, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Monday:

                    if (workModel.IsWorkActiveMon)
                    {
                        var StartDate = getValidValue(workModel.StartDateMon).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateMon).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateMonSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateMonSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }

                    break;
                case DayOfWeek.Tuesday:
                    if (workModel.IsWorkActiveTues)
                    {
                        var StartDate = getValidValue(workModel.StartDateTues).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateTues).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateTuesSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateTuesSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextTues, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Wednesday:
                    if (workModel.IsWorkActiveWed)
                    {
                        var StartDate = getValidValue(workModel.StartDateWed).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateWed).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateWedSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateWedSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextWed, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Thursday:
                    if (workModel.IsWorkActiveThurs)
                    {
                        var StartDate = getValidValue(workModel.StartDateThurs).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateThurs).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateThursSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateThursSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextThurs, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                case DayOfWeek.Friday:
                    if (workModel.IsWorkActiveFri)
                    {
                        var StartDate = getValidValue(workModel.StartDateFri).AddHours(AppSettingsModel.AddHour);
                        var EndDate = getValidValue(workModel.EndDateFri).AddHours(AppSettingsModel.AddHour);

                        var StartDateSP = getValidValue(workModel.StartDateFriSP).AddHours(AppSettingsModel.AddHour);
                        var EndDateSP = getValidValue(workModel.EndDateFriSP).AddHours(AppSettingsModel.AddHour);

                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

                        {
                            result = true;
                        }
                        else
                        {
                            outWHMessage = string.Format(workModel.WorkTextFri, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
                            result = false;
                        }
                    }
                    break;
                default:

                    break;

            }



            return result;

        }

        //[HttpPost("webhookFacebook")]
        //public async void Webhook(dynamic jsonData2)
        //{
        //    try
        //    {
        //        FacebookModel jsonData = new FacebookModel
        //        {
        //            Object = "page",
        //            Entry = new List<Entry>
        //        {
        //            new Entry
        //            {
        //                Id = "123456789",
        //                Time = 1527459824,
        //                Changes = new List<Change>(), 
        //                leadgenChange = new List<LeadgenChange>(), 
        //                Messaging = new List<Messaging>
        //                {
        //                    new Messaging
        //                    {
        //                        Field = "messages",
        //                        timestamp = "1527459824",
        //                        Sender = new Sender
        //                        {
        //                            Id = "12334",
        //                            Name = "John Doe"
        //                        },
        //                        Recipient = new Recipient
        //                        {
        //                            Id = "23245"
        //                        },

        //                        Message = new MessageModel
        //                        {
        //                            Mid = "test_message_id",
        //                            Text = "test_message"
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        };
        //        //var aaaa = JsonConvert.SerializeObject(jsonData2);
        //        //FacebookModel jsonData = JsonConvert.DeserializeObject<FacebookModel>(aaaa);

        //       // return;
        //        //var jsonData = jsonData2.ToObject<FacebookModel>(); 
        //        var phoneNumberId = "";

        //        string userId = string.Empty;
        //        if (jsonData.Entry != null && jsonData.Entry.Count > 0)
        //        {
        //            var messaging = jsonData.Entry[0].Messaging;
        //            if (messaging != null && messaging.Count > 0)
        //            {
        //                userId = messaging[0].Sender.Id;
        //                phoneNumberId = messaging[0].Sender.Id;
        //                var x = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text;
        //            }
        //        }
        //        TenantModel Tenant = new TenantModel();
        //        var tenantId = jsonData.Entry[0].Messaging.FirstOrDefault().Recipient.Id;
        //        var objTenant = _cacheManager.GetCache("CacheTenant").Get(tenantId.ToString(), cache => cache);
        //        if (objTenant.Equals(tenantId.ToString()))
        //        {
        //            Tenant = await _dbService.GetTenantByKey("", tenantId.ToString());
        //            if (Tenant != null)
        //            {
        //                _cacheManager.GetCache("CacheTenant").Set(tenantId.ToString(), Tenant);
        //            }
        //            //_cacheManager.GetCache("CacheTenant").Set(tenantId.ToString(), Tenant);
        //        }
        //        else
        //        {
        //            Tenant = (TenantModel)objTenant;
        //        }

        //        if (jsonData.Entry[0].Messaging[0].Sender.Id == null)
        //        {
        //            return;
        //        }
        //        if (jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text != null)
        //        {
        //            //if (jsonData.Entry[0].Changes[0].Value.Messages[0].context != null)
        //            //{

        //                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
        //                {
        //                    TenantId = Tenant.TenantId.Value,
        //                    MessageStatusId = 4,
        //                    MessageDateTime = DateTime.Now,
        //                    MessageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid

        //                };
        //                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
        //                //var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;
        //            //}
        //        }

        //        if (jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text != null)
        //        {
        //            if (jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text != null)
        //            {

        //                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
        //                {
        //                    TenantId = Tenant.TenantId.Value,
        //                    MessageStatusId = 4,
        //                    MessageDateTime = DateTime.Now,
        //                    MessageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid

        //                };
        //                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

        //              //  var replayid = jsonData.Entry[0].Changes[0].Value.Messages[0].context.id;
        //            }



        //        }

        //        var name2 = "";
        //        if (jsonData.Entry[0].Messaging[0].Sender.Name != null)
        //        {
        //           name2 = jsonData.Entry[0].Messaging[0].Sender.Name;

        //        }
        //        var massageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid;
        //        var from = jsonData.Entry[0].Messaging[0].Sender.Id; // extract the phone number from the webhook payload
        //        string medaiUrl = string.Empty;
        //        userId = (Tenant.TenantId + "_" + from).ToString();
        //         string interactiveId = string.Empty;


        //        WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
        //        AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
        //        List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();

        //        string msgBody = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Text;

        //        var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //        var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == "27_962785495992" && a.TenantId ==27);
        //        var Customer = await customerResult;

        //        if (Customer == null)
        //        {
        //            Customer = _dbService.CreateNewCustomer(from, name2, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
        //            Customer.customerChat.text = msgBody;
        //        }

        //        //in here
        //        if (!Tenant.IsBundleActive)
        //        {
        //            string UserId = jsonData.Entry[0].Messaging.FirstOrDefault().Sender.Id;
        //            var name = "";
        //            if (jsonData.Entry[0].Messaging.FirstOrDefault().Sender.Name != null)
        //            {
        //                name = jsonData.Entry[0].Messaging.FirstOrDefault().Sender.Name;
        //            }
        //            userId = (Tenant.TenantId + "_" + UserId).ToString();
        //            //var status = jsonData.Entry[0].Changes[0].Value.statuses[0].status;


        //            //var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //            //var campaignCosmoResult = campaignCosmoDBModel.GetItemAsync(a => a.itemType == 5 && a.messagesId == jsonData.Entry[0].Changes[0].Value.statuses[0].id  && a.tenantId == Tenant.TenantId);
        //            //var campaignCosmo = campaignCosmoResult.Result;

        //            var itemsCollection3 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
        //            var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
        //            var Customer3 = await customerResult3;



        //            //if (status == "sent")
        //            //{
        //            if (Customer3 == null)
        //            {

        //                Customer3 = _dbService.CreateNewCustomer(UserId, name, "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
        //                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
        //                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
        //                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("UserId");
        //                Customer3.CustomerStepModel.UserParmeter.Add("UserId", Customer3.userId.ToString());
        //                //Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
        //                //Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("Name");
        //                Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("Location");
        //                Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");

        //                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Messaging.FirstOrDefault().timestamp);
        //                //Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;

        //                var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

        //            }
        //            else
        //            {

        //                Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Messaging.FirstOrDefault().timestamp);
        //                //Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;

        //                if (Customer3.IsBlock)
        //                {
        //                    return;
        //                }
        //                Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
        //                Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
        //                Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
        //                Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
        //                Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());

        //                try
        //                {
        //                    if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Name"))
        //                    {
        //                        Customer3.CustomerStepModel.UserParmeter.Remove("Name");
        //                        Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
        //                    }
        //                }
        //                catch
        //                {
        //                    Customer3.CustomerStepModel.UserParmeter.Remove("Name");
        //                    Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
        //                }

        //            }
        //            return;
        //        }

        //        if (jsonData.Entry[0].Messaging.FirstOrDefault().Message != null)
        //        {
        //            if (jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid != null)
        //            {

        //                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
        //                {
        //                    TenantId = Tenant.TenantId.Value,
        //                    MessageStatusId = 4,
        //                    MessageDateTime = DateTime.Now,
        //                    MessageId = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid
        //                };
        //                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

        //                var replayid = jsonData.Entry[0].Messaging.FirstOrDefault().Message.Mid;
        //            }
        //        }
        //        // var name2 = jsonData.Entry[0].Messaging.FirstOrDefault().Sender.Name;




        //        //var massageId = jsonData.Entry[0].Messaging.FirstOrDefault().message.Mid;
        //        //var from = jsonData.Entry[0].Changes[0].Value.Messages[0].From; // extract the phone number from the webhook payload
        //        //string medaiUrl = string.Empty;
        //        //userId = (Tenant.TenantId + "_" + from).ToString();
        //        //var type = jsonData.Entry[0].Changes[0].Value.Messages[0].Type;
        //        //string interactiveId = string.Empty;
        //    }
        //    catch
        //    {
        //    }
        //    return;
        //}



    }
}

//using Abp.Runtime.Caching;
//using Abp.Web.Models;
//using Framework.Data;
//using Framework.Integration.Implementation;
//using Framework.Integration.Interfaces;
//using Framework.Integration.Model;
//using Infoseed.MessagingPortal.LiveChat;
//using Infoseed.MessagingPortal.SocketIOClient;
//using Infoseed.MessagingPortal.Web.Controllers;
//using Infoseed.MessagingPortal.Web.Models;
//using Infoseed.MessagingPortal.Web.Models.Sunshine;
//using Infoseed.MessagingPortal.Web.Sunshine;
//using Infoseed.MessagingPortal.WhatsApp;
//using Infoseed.MessagingPortal.WhatsApp.Dto;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Documents;
//using Microsoft.Bot.Connector.DirectLine;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Queue;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading.Tasks;
//using WebJobEntities;
//using static Framework.Integration.Model.CreateContactMg;

//namespace Infoseed.MessagingPortal.Engine.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class WhatsApp360DialogAPIController : MessagingPortalControllerBase
//    {
//        private ILiveChatAppService _iliveChat;
//        public string URLG = "https://6f5a-176-29-82-244.ngrok-free.app";
//        public string DialogURL = "https://waba-sandbox.360dialog.io/v1/configs/webhook";

//        IDBService _dbService;
//        public IContactsAPI _contactsAPI;
//        public ITicketsAPI _ticketsAPI;
//        private readonly ICacheManager _cacheManager;

//        private readonly IDocumentClient _IDocumentClient;


//        public WhatsApp360DialogAPIController(
//            IDBService dbService
//              , ICacheManager cacheManager
//            , IDocumentClient iDocumentClient
//            , ILiveChatAppService iliveChat
//           )
//        {
//            _dbService = dbService;
//            _contactsAPI = new ContactsAPI(SettingsModel.MgUrl, SettingsModel.MgKey);
//            _cacheManager = cacheManager;
//            _IDocumentClient = iDocumentClient;
//            _iliveChat=iliveChat;

//        }

//        [HttpGet("SetWebhook")]
//        [DontWrapResult]
//        public async Task<string> SetWebhook(string url,string Key)
//        {
//            var client = new HttpClient();
//            var request = new HttpRequestMessage(HttpMethod.Post, DialogURL);
//            request.Headers.Add("D360-API-KEY", Key);
//            var content = new StringContent("{\r\n  \"url\": \""+url+"/api/WhatsApp360DialogAPI/Webhook360Dailog?key="+Key+"\"\r\n}", null, "application/json");
//            request.Content = content;
//            var response = await client.SendAsync(request);
//            response.EnsureSuccessStatusCode();
//            return "ok";
//        }
//        [HttpPost("Webhook360Dailog")]
//        public async void Webhook360Dailog([FromBody] WhatsAppValueModel jsonData, string key)
//        {

//            //var json = (dynamic)null;
//            //var aaaa = JsonConvert.SerializeObject(jsonData);
//           // var  = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);


//            try
//            {
//                if (jsonData.Messages == null && jsonData.statuses == null)
//                {
//                    return;
//                }




//                string userId = string.Empty;
//                var phoneNumberId = key;// jsonData.Metadata.phone_number_id;

//                TenantModel Tenant = new TenantModel();

//                var objTenant = _cacheManager.GetCache("CacheTenant").Get(phoneNumberId.ToString(), cache => cache);
//                if (objTenant.Equals(phoneNumberId.ToString()))
//                {
//                    Tenant = _dbService.GetTenantByKey("", phoneNumberId.ToString()).Result;
//                    _cacheManager.GetCache("CacheTenant").Set(phoneNumberId.ToString(), Tenant);
//                }
//                else
//                {
//                    Tenant = (TenantModel)objTenant;
//                }




//                if (jsonData.statuses != null)
//                {
//                    var from2 = jsonData.statuses[0].recipient_id; // extract the phone number from the webhook payload

//                    //if (from2=="962779746365")
//                    //{
//                    //    await testtAsync(jsonData);
//                    //    return;
//                    //}
//                    if (Tenant.IsBundleActive)
//                    {
//                        int creation_timestamp = 0;
//                        int expiration_timestamp = 0;

//                        try
//                        {
//                            creation_timestamp=int.Parse(jsonData.statuses.FirstOrDefault().timestamp);
//                        }
//                        catch
//                        {

//                        }
//                        try
//                        {
//                            if (jsonData.statuses[0].conversation != null)
//                            {
//                                expiration_timestamp=jsonData.statuses.FirstOrDefault().conversation.expiration_timestamp;

//                            }

//                        }
//                        catch
//                        {

//                        }






//                        if (jsonData.statuses[0].status.Equals("sent"))
//                        {
//                            if (jsonData.statuses[0].conversation != null)
//                            {

//                                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                                {
//                                    TenantId = Tenant.TenantId.Value,
//                                    CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.statuses[0].pricing.category),
//                                    MessageDateTime = DateTime.Now,
//                                    ConversationId = jsonData.statuses.FirstOrDefault().conversation.id,
//                                    PhoneNumber = jsonData.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.statuses.FirstOrDefault().id,
//                                    creation_timestamp=creation_timestamp,
//                                    expiration_timestamp=expiration_timestamp


//                                };
//                                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                                userId = (Tenant.TenantId + "_" + conservationMeasurementMessage.PhoneNumber).ToString();
//                                UpdateContactConversationId(jsonData.statuses.FirstOrDefault().conversation.id, userId, creation_timestamp, expiration_timestamp);
//                            }
//                        }
//                        else if (jsonData.statuses[0].status.Equals("read"))
//                        {

//                            if (jsonData.statuses[0].pricing != null)
//                            {
//                                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                                {
//                                    TenantId = Tenant.TenantId.Value,
//                                    CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.statuses[0].pricing.category),
//                                    MessageDateTime = DateTime.Now,
//                                    ConversationId = jsonData.statuses.FirstOrDefault().conversation.id,
//                                    PhoneNumber = jsonData.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.statuses.FirstOrDefault().id,
//                                    MessageStatusId = (int)MessageStatusWhatsApp.Read,
//                                    creation_timestamp=creation_timestamp,
//                                    expiration_timestamp=expiration_timestamp
//                                };
//                                if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                                {
//                                    SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                                }
//                            }
//                            else
//                            {
//                                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                                {
//                                    TenantId = Tenant.TenantId.Value,
//                                    CommunicationInitiated = CommunicationInitiated.business_initiated,
//                                    MessageDateTime = DateTime.Now,
//                                    ConversationId = null,
//                                    PhoneNumber = jsonData.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.statuses.FirstOrDefault().id,
//                                    MessageStatusId = (int)MessageStatusWhatsApp.Read,
//                                    creation_timestamp=creation_timestamp,
//                                    expiration_timestamp=0
//                                };
//                                if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                                {
//                                    SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                                }
//                            }

//                            try
//                            {
//                                var from3 = jsonData.statuses[0].recipient_id;
//                                userId = (Tenant.TenantId + "_" + from3).ToString();

//                                var x = _dbService.UpdateCustomerChatStatusNew(jsonData.statuses.FirstOrDefault().id, Tenant.TenantId.Value);
//                                var itemsCollection3 = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
//                                var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
//                                var Customer3 = customerResult3.Result;
//                                Customer3.customerChat=x;

//                                SocketIOManager.SendChat(Customer3, Tenant.TenantId.Value);
//                            }
//                            catch
//                            {


//                            }

//                        }

//                        else if (jsonData.statuses[0].status.Equals("delivered"))
//                        {
//                            ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                            {
//                                TenantId = Tenant.TenantId.Value,
//                                CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.statuses[0].pricing.category),
//                                MessageDateTime = DateTime.Now,
//                                ConversationId = jsonData.statuses.FirstOrDefault().conversation.id,
//                                PhoneNumber = jsonData.statuses.FirstOrDefault().recipient_id,
//                                MessageId = jsonData.statuses.FirstOrDefault().id,
//                                MessageStatusId = (int)MessageStatusWhatsApp.Delivered
//                            };
//                            if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                            {
//                                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                            }
//                        }

//                        else if (jsonData.statuses[0].status.Equals("failed"))
//                        {
//                            ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                            {
//                                TenantId = Tenant.TenantId.Value,
//                                CommunicationInitiated = CommunicationInitiated.business_initiated,
//                                MessageDateTime = DateTime.Now,
//                                ConversationId = null,
//                                PhoneNumber = jsonData.statuses.FirstOrDefault().recipient_id,
//                                MessageId = jsonData.statuses.FirstOrDefault().id,
//                                MessageStatusId = (int)MessageStatusWhatsApp.Failed,
//                                creation_timestamp=0,
//                                expiration_timestamp=0
//                            };
//                            if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                            {
//                                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                            }
//                        }
//                    }
//                    return;
//                }

//                if (jsonData.Contacts != null)
//                {
//                    if (jsonData.Contacts != null)
//                    {

//                        ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                        {
//                            TenantId = Tenant.TenantId.Value,
//                            MessageStatusId=4,
//                            MessageDateTime = DateTime.Now,
//                            MessageId = jsonData.Contacts[0].Wa_Id

//                        };
//                        SetConversationMeasurmentsInQueue(conservationMeasurementMessage);

//                        var replayid = jsonData.Contacts[0].Wa_Id;
//                    }



//                }
//                var massageId = jsonData.Messages[0].Id;

//                string medaiUrl = string.Empty;
//                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
//                var from = jsonData.Messages[0].From; // extract the phone number from the webhook payload
//                //if (from=="962779746365")
//                //{
//                //    await testtAsync(jsonData);
//                //    return;
//                //}

//                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();
//                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();
//                string interactiveId = string.Empty;
//                var aaaa = JsonConvert.SerializeObject(jsonData.Messages);
//                string msgBody = whatsAppAppService.MassageTypeText360Dailog(aaaa, Tenant.AccessToken, tAttachments, ref medaiUrl, attachmentMessageModel, out interactiveId);
//                var type = jsonData.Messages[0].Type;
//                userId = (Tenant.TenantId + "_" + from).ToString();





//                var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
//                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
//                var Customer = customerResult.Result;

//                if (Customer == null)
//                {
//                    var name = jsonData.Contacts[0].Profile.Name;
//                    Customer = _dbService.CreateNewCustomer(from, name, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);


//                    //create new contact in MG
//                    if (Tenant.TenantId.Value == 59)
//                    {
//                        CreateContactFromInfoSeed model = new CreateContactFromInfoSeed();

//                        model.firstname = name;
//                        model.lastname = name;
//                        model.phone = from;

//                        CreateContactMg createContactMg = CreateContactMgFun(model);
//                        SetMgMotorIntegrationInQueue(createContactMg);
//                        // _contactsAPI.Create(createContactMg);
//                    }
//                }







//                // var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty);
//                // Customer.customerChat = CustomerSendChat;
//                // string messageId =null;
//                if (attachmentMessageModel.IsHasAttachment)
//                {

//                    var objCustomer = _dbService.PrePareCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
//                    //   var rr =  objCustomer.customerChat.messageId;
//                    attachmentMessageModel.CustomerModel = JsonConvert.SerializeObject(objCustomer);
//                    SetAttachmentMessageInQueue(attachmentMessageModel);

//                }
//                else
//                {

//                    var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty, MessageSenderType.Customer, massageId);
//                    Customer.customerChat = CustomerSendChat;

//                }


//                if (!Tenant.IsBotActive)
//                {
//                    // if the bot is not Active in Tenant
//                    //  var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, Key);
//                    // var x = BotNotActive(model, userId, Tenant.TenantId, Key, jsonModel).Result;

//                    //  _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                    //SocketIOManager.SendContact(Customer, Customer.TenantId.Value);
//                    return;

//                }

//                if (!Tenant.IsPaidInvoice)
//                {
//                    // if the bot is not Active in Tenant
//                    //  var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, Key);
//                    // var x = BotNotActive(model, userId, Tenant.TenantId, Key, jsonModel).Result;

//                    //  _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                    //SocketIOManager.SendContact(Customer, Customer.TenantId.Value);
//                    return;

//                }

//                //if ((string.IsNullOrEmpty(Tenant.botId) || string.IsNullOrEmpty(Tenant.DirectLineSecret)) && (!Tenant.BotTemplateId.HasValue))
//                //{
//                //    return;
//                //}

//                if (Tenant == null)
//                {
//                    return;
//                }
//                if (!Tenant.IsBundleActive)
//                {
//                    return;
//                }
//                if (Customer.IsBlock)
//                {
//                    return;
//                }
//                if (Tenant.IsWorkActive && !Customer.IsOpen)
//                {
//                    string SFormat = string.Empty;
//                    /// out of working hours  
//                    if (!checkIsInService(Tenant.workModel, out SFormat))
//                    {
//                        PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
//                        postWhatsAppMessageModel.type = "text";
//                        postWhatsAppMessageModel.to = from;
//                        postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text();
//                        postWhatsAppMessageModel.text.body = SFormat;
//                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken, Tenant.IsD360Dialog);
//                        if (result)
//                        {
//                            Content message = new Content();
//                            message.text = SFormat;
//                            message.agentName = Tenant.botId;
//                            message.agentId = "1000000";
//                            message.type = "text";
//                            var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
//                            Customer.CreateDate = CustomerChat.CreateDate;
//                            Customer.customerChat = CustomerChat;
//                            //_hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
//                            // SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
//                        }

//                        return;
//                    }
//                }



//                if (!attachmentMessageModel.IsHasAttachment)
//                {
//                    //await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                    SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);

//                }

//                if (Customer.customerChat!=null)
//                {
//                    if (Customer.IsOpen && Customer.customerChat.sender==MessageSenderType.Customer)
//                    {

//                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, true, Customer.ConversationsCount, Customer.creation_timestamp, Customer.expiration_timestamp);
//                    }
//                    else
//                    {
//                        _iliveChat.UpdateIsOpenLiveChat(Tenant.TenantId.Value, Customer.phoneNumber, false, 0, Customer.creation_timestamp, Customer.expiration_timestamp);

//                    }
//                }



//                if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
//                {
//                    await PrepareBotChatWithCustomer(from, Tenant, Customer, msgBody, tAttachments, interactiveId);

//                }

//                else
//                {
//                    return;
//                }




//                //  dynamic data = JsonConvert.DeserializeObject(json);
//                //postToFB(from, msgBody, from);
//            }
//            catch (Exception ex)
//            {
//                return;
//            }
//        }




//        #region Private Method 
//        private async Task testtAsync(WhatsAppModel jsonData)
//        {
//            try
//            {
//                var constra = JsonConvert.SerializeObject(jsonData);
//                var client = new HttpClient();
//                var request = new HttpRequestMessage(HttpMethod.Post, URLG+"/api/WhatsAppAPI/WebhookTest");
//                request.Headers.Add("accept", "*/*");
//                var content = new StringContent(constra
//                    , null, "application/json-patch+json");
//                request.Content = content;
//                var response = await client.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//                //Console.WriteLine(await response.Content.ReadAsStringAsync());
//            }
//            catch
//            {

//            }


//        }
//        private async Task SendToRestaurantsBot(CustomerModel jsonData)
//        {
//            try
//            {
//                var constra = JsonConvert.SerializeObject(jsonData);
//                var client = new HttpClient();
//                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi +"api/RestaurantsChatBot/RestaurantsMessageHandler");
//                request.Headers.Add("accept", "*/*");
//                var content = new StringContent(constra
//                    , null, "application/json-patch+json");
//                request.Content = content;
//                var response = await client.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//                //Console.WriteLine(await response.Content.ReadAsStringAsync());
//            }
//            catch
//            {

//            }




//        }
//        private async Task SendToBookingChatBott(CustomerModel jsonData)
//        {
//            try
//            {
//                var constra = JsonConvert.SerializeObject(jsonData);
//                var client = new HttpClient();
//                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi +"api/BookingChatBot/BookingMessageHandler");
//                request.Headers.Add("accept", "*/*");
//                var content = new StringContent(constra
//                    , null, "application/json-patch+json");
//                request.Content = content;
//                var response = await client.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//                //Console.WriteLine(await response.Content.ReadAsStringAsync());
//            }
//            catch
//            {

//            }




//        }

//        private async Task<bool> PrepareBotChatWithCustomer(string from, TenantModel Tenant, CustomerModel Customer, string msg, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string interactiveId)
//        {
//            Customer.TennantPhoneNumberId=Tenant.D360Key;
//            Customer.interactiveId=interactiveId;
//            List<Activity> Bot = new List<Activity>();
//            if (Tenant.botId=="RestaurantBot")
//            {
//                SendToRestaurantsBot(Customer);
//                return true;
//            }
//            else if (Tenant.botId=="BookingBot")
//            {
//                SendToBookingChatBott(Customer);
//                return true;
//            }
//            else
//            {


//                if ((string.IsNullOrEmpty(Tenant.botId) || string.IsNullOrEmpty(Tenant.DirectLineSecret)) && (!Tenant.BotTemplateId.HasValue))
//                {
//                    return false;
//                }
//                DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
//                var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
//                Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments, Tenant.BotTemplateId, interactiveId).Result;

//            }

//            List<Activity> botListMessages = new List<Activity>();

//            foreach (var msgBot in Bot)
//            {

//                if (msgBot.Text.Contains("The process cannot access the file") || msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
//                {


//                }
//                else
//                {
//                    botListMessages.Add(msgBot);
//                }


//            }

//            WhatsAppAppService whatsAppAppService = new WhatsAppAppService();


//            foreach (var msgBot in botListMessages)
//            {


//                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, from, Tenant.botId);

//                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
//                {
//                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
//                    if (result)
//                    {

//                        //var message = PrepareMessageContent(msgBot, Tenant.botId);
//                        //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);

//                        WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, Tenant.botId, Customer.userId, Customer.TenantId.Value, Customer.ConversationId);
//                        var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);

//                        Customer.customerChat = CustomerChat;
//                        //await _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
//                        SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
//                    }
//                }

//            }



//            return true;
//        }

//        private static CreateContactMg CreateContactMgFun(CreateContactFromInfoSeed model)
//        {
//            model.company="infoseed";
//            model.email=model.company+"-"+model.phone+"@infoseed.com";

//            CreateContactMg createContactMg = new CreateContactMg();
//            Property1 property1 = new Property1 { property="email", value=model.email };
//            Property1 firstname = new Property1 { property="firstname", value=model.firstname };
//            Property1 lastname = new Property1 { property="lastname", value=model.lastname };
//            //  Property1 website = new Property1 { property="website", value="http://hubspot.com" };
//            Property1 company = new Property1 { property="company", value=model.company };
//            Property1 phone = new Property1 { property="phone", value=model.phone };
//            // Property1 address = new Property1 { property="address", value="25 First Street" };
//            //  Property1 city = new Property1 { property="city", value="Cambridge" };
//            //  Property1 state = new Property1 { property="state", value="MA" };
//            // Property1 zip = new Property1 { property="zip", value="02139" };
//            List<Property1> properties = new List<Property1>();

//            properties.Add(property1);
//            properties.Add(firstname);
//            properties.Add(lastname);
//            //  properties.Add(website);
//            properties.Add(company);
//            properties.Add(phone);
//            // properties.Add(address);
//            //  properties.Add(city);
//            //  properties.Add(state);
//            //  properties.Add(zip);

//            createContactMg.properties=properties.ToArray();
//            return createContactMg;
//        }
//        private Content PrepareMessageContent(Activity msgBot, string botId)
//        {
//            string tMessageToSend = string.Empty;
//            List<CardAction> tOutActions = new List<CardAction>();
//            int tOrder = 1;
//            var optionlst = new Dictionary<int, string>();
//            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
//            {
//                tOutActions.AddRange(msgBot.SuggestedActions.Actions);
//            }

//            foreach (var hc in tOutActions)
//            {
//                tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
//                optionlst.Add(tOrder, hc.Title);
//                tOrder++;
//            }

//            Content message = new Content
//            {
//                text = msgBot.Text + "\r\n" + tMessageToSend,
//                type = "text",
//                agentName = botId,
//                agentId = "1000000"

//            };

//            return message;

//        }



//        private bool checkIsInService(Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel workModel, out string outWHMessage)
//        {

//            bool result = true;
//            outWHMessage = string.Empty;
//            DateTime currentDateTime = DateTime.UtcNow.AddHours(AppSettingsModel.AddHour);
//            DayOfWeek wk = currentDateTime.DayOfWeek;
//            TimeSpan timeOfDay = currentDateTime.TimeOfDay;
//            var options = new JsonSerializerOptions { WriteIndented = true };

//            switch (wk)
//            {
//                case DayOfWeek.Saturday:
//                    if (workModel.IsWorkActiveSat)
//                    {
//                        var StartDateSat = getValidValue(workModel.StartDateSat).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSat = getValidValue(workModel.EndDateSat).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSatSP = getValidValue(workModel.StartDateSatSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSatSP = getValidValue(workModel.EndDateSatSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) || (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }

//                    break;
//                case DayOfWeek.Sunday:
//                    if (workModel.IsWorkActiveSun)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateSun).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateSun).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateSunSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateSunSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextSun, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Monday:

//                    if (workModel.IsWorkActiveMon)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateMon).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateMon).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateMonSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateMonSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextMon, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }

//                    break;
//                case DayOfWeek.Tuesday:
//                    if (workModel.IsWorkActiveTues)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateTues).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateTues).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateTuesSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateTuesSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextTues, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Wednesday:
//                    if (workModel.IsWorkActiveWed)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateWed).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateWed).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateWedSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateWedSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextWed, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Thursday:
//                    if (workModel.IsWorkActiveThurs)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateThurs).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateThurs).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateThursSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateThursSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextThurs, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                case DayOfWeek.Friday:
//                    if (workModel.IsWorkActiveFri)
//                    {
//                        var StartDate = getValidValue(workModel.StartDateFri).AddHours(AppSettingsModel.AddHour);
//                        var EndDate = getValidValue(workModel.EndDateFri).AddHours(AppSettingsModel.AddHour);

//                        var StartDateSP = getValidValue(workModel.StartDateFriSP).AddHours(AppSettingsModel.AddHour);
//                        var EndDateSP = getValidValue(workModel.EndDateFriSP).AddHours(AppSettingsModel.AddHour);

//                        if ((timeOfDay >= StartDate.TimeOfDay && timeOfDay <= EndDate.TimeOfDay) || (timeOfDay >= StartDateSP.TimeOfDay && timeOfDay <= EndDateSP.TimeOfDay))

//                        {
//                            result = true;
//                        }
//                        else
//                        {
//                            outWHMessage = string.Format(workModel.WorkTextFri, StartDate.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDate.ToString("hh:mm tt", CultureInfo.InvariantCulture)
//                                           , StartDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSP.ToString("hh:mm tt", CultureInfo.InvariantCulture));
//                            result = false;
//                        }
//                    }
//                    break;
//                default:

//                    break;

//            }



//            return result;

//        }

//        private DateTime getValidValue(dynamic value)
//        {
//            DateTime result = DateTime.MinValue;
//            try
//            {
//                result = DateTime.Parse(value.ToString());
//                return result;
//            }
//            catch (Exception)
//            {
//                return result;
//                throw;
//            }

//        }

//        private void SetConversationMeasurmentsInQueue(ConservationMeasurementMessage message)
//        {
//            try
//            {
//                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
//                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
//                CloudQueue queue;
//                queue = queueClient.GetQueueReference("conservation-measurements");
//                queue.CreateIfNotExistsAsync();

//                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
//                           message
//                        ));
//                queue.AddMessageAsync(queueMessage);

//            }
//            catch (Exception e)
//            {

//                var Error = JsonConvert.SerializeObject(message);
//                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
//                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
//                //
//            }


//        }

//        private void SetAttachmentMessageInQueue(AttachmentMessageModel message)
//        {
//            try
//            {
//                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
//                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
//                CloudQueue queue;
//                queue = queueClient.GetQueueReference("attachment-messages");
//                queue.CreateIfNotExistsAsync();

//                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
//                           message
//                        ));
//                queue.AddMessageAsync(queueMessage);

//            }
//            catch (Exception e)
//            {

//                var Error = JsonConvert.SerializeObject(message);
//                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
//                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
//                //
//            }


//        }
//        private void SetMgMotorIntegrationInQueue(CreateContactMg ContactMg)
//        {
//            try
//            {
//                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(SettingsModel.AzureWebJobsStorage);
//                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
//                CloudQueue queue;
//                queue = queueClient.GetQueueReference("mgcontacts-sync");
//                queue.CreateIfNotExistsAsync();

//                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
//                           ContactMg
//                        ));
//                queue.AddMessageAsync(queueMessage);

//            }
//            catch (Exception e)
//            {

//                var Error = JsonConvert.SerializeObject(ContactMg);
//                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
//                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
//                //
//            }


//        }

//        private void UpdateContactConversationId(string ConversationId, string userId, int creation_timestamp, int expiration_timestamp)
//        {




//            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
//            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);//&& a.TenantId== TenantId

//            if (customerResult.IsCompletedSuccessfully)
//            {
//                var customer = customerResult.Result;
//                if (customer == null)
//                {

//                }
//                else
//                {


//                    if (customer.ConversationId != ConversationId)
//                    {
//                        customer.ConversationId = ConversationId;
//                        customer.LastConversationStartDateTime = DateTime.Now;//.AddHours(AppSettingsModel.AddHour);

//                        customer.expiration_timestamp=expiration_timestamp;
//                        customer.creation_timestamp=creation_timestamp;

//                        if (customer.creation_timestamp==0 || customer.expiration_timestamp==0)
//                        {
//                            var model = getConversationSessions(customer.TenantId.Value, customer.phoneNumber, ConversationId);

//                            if (model != null && model.expiration_timestamp!=0 &&model.creation_timestamp!=0)
//                            {
//                                customer.expiration_timestamp=model.expiration_timestamp;
//                                customer.creation_timestamp=model.creation_timestamp;
//                            }

//                        }




//                    }



//                    // customer.LastConversationStartDateTime = ConversationId;
//                    var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
//                }
//            }
//        }
//        private static ConversationSessionsModel getConversationSessions(int TenantId, string PhoneNumber, string ConversationID)
//        {
//            try
//            {
//                ConversationSessionsModel ConversationSessionsEntity = new ConversationSessionsModel();
//                var SP_Name = "GetConversationSessions";

//                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
//                {
//                     new System.Data.SqlClient.SqlParameter("@ConversationID",ConversationID)
//                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
//                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
//                };

//                ConversationSessionsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSessions, AppSettingsModel.ConnectionStrings).FirstOrDefault();


//                return ConversationSessionsEntity;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        public static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
//        {
//            try
//            {
//                ConversationSessionsModel model = new ConversationSessionsModel();
//                model.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "creation_timestamp");
//                model.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "expiration_timestamp");
//                ///model.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");

//                return model;
//            }
//            catch (Exception)
//            {
//                ConversationSessionsModel model = new ConversationSessionsModel();
//                model.creation_timestamp = 0;
//                model.expiration_timestamp = 0;

//                return model;
//            }
//        }
//        #endregion


//    }
//}

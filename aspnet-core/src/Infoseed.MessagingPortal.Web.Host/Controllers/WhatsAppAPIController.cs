//using Abp.Web.Models;
//using Infoseed.MessagingPortal.Web.Models;
//using Infoseed.MessagingPortal.Web.Sunshine;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Bot.Connector.DirectLine;
//using Infoseed.MessagingPortal.Web.Models.Sunshine;
//using Framework.Data;
//using System.Linq;
//using System.Net;
//using System.Globalization;
//using System.Text.Json;
//using Infoseed.MessagingPortal.SocketIOClient;
//using Microsoft.AspNetCore.SignalR;
//using Infoseed.MessagingPortal.Web.Models.WhatsAppDialog;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Queue;
//using WebJobEntities;
//using Infoseed.MessagingPortal.WhatsApp.Dto;
//using Infoseed.MessagingPortal.WhatsApp;
//using Infoseed.MessagingPortal.MgSystem;
//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace Infoseed.MessagingPortal.Web.Controllers
//{



//    [Route("api/[controller]")]
//    [ApiController]
//    public class WhatsAppAPIController : MessagingPortalControllerBase
//    {

//        //private string fbToken = "EAAKTbPZAaKtYBAGxYaZAa3JckCvl7QDd1wbK8w9MNrwjsvMkUDKBFDoHKVKgE9JaYNkUc4C1IvdxQgn73nLPQ81zW6bhbfflnfZC2xpG7ofzGqP2T7YXCSu7LbWccPVJVdafiCFw5UnyikowudxmYW9VLzEcvbobqlW4ZBqe47IUHid3IbZBC6SmC9GyiJBGkrV1cAmuZAEQZDZD";
//        //private string postUrl = "https://graph.facebook.com/v14.0/103674912368849/messages";
//        IDBService _dbService;
//        // private IHubContext<SignalR.TeamInboxHub> _hub;
//        public WhatsAppAPIController(
//            IDBService dbService
//           //,
//           // IHubContext<SignalR.TeamInboxHub> hub
//           )
//        {
//            //   _hub = hub;
//            _dbService = dbService;
//        }



//        [HttpGet("webhook")]
//        [DontWrapResult]
//        public string Webhook(
//     [FromQuery(Name = "hub.mode")] string mode,
//     [FromQuery(Name = "hub.challenge")] string challenge,
//     [FromQuery(Name = "hub.verify_token")] string verify_token)
//        {
//            return null;

//            string currentDattime = "InfoSeed-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
//            if (verify_token.Equals(currentDattime) && mode.Equals("subscribe"))
//            {
//                return challenge;
//            }
//            else
//            {
//                return null;
//            }
//        }


//        [HttpPost("webhook")]
//        public async void Webhook(WhatsAppModel jsonData)
//        {


//            return;
//            var json = (dynamic)null;

//            //    var aaaa = JsonConvert.SerializeObject(jsonData);
//            //var  = JsonConvert.DeserializeObject<WhatsAppModel>(aaaa);
//            try
//            {
//                if (jsonData.Entry[0].Changes[0].Value.Messages == null && jsonData.Entry[0].Changes[0].Value.statuses == null)
//                {
//                    return;
//                }

//                string userId = string.Empty;
//                var phoneNumberId = jsonData.Entry[0].Changes[0].Value.Metadata.phone_number_id;
//                var Tenant = _dbService.GetTenantByKey("", phoneNumberId.ToString()).Result;


//                if (jsonData.Entry[0].Changes[0].Value.statuses != null)
//                {
//                    if (Tenant.IsBundleActive)
//                    {
//                        if (jsonData.Entry[0].Changes[0].Value.statuses[0].status.Equals("sent"))
//                        {
//                            if (jsonData.Entry[0].Changes[0].Value.statuses[0].conversation != null)
//                            {

//                                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                                {
//                                    TenantId = Tenant.TenantId.Value,
//                                    CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category),
//                                    MessageDateTime = DateTime.Now,
//                                    ConversationId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.id,
//                                    PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id

//                                };
//                                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                                userId = (Tenant.TenantId + "_" + conservationMeasurementMessage.PhoneNumber).ToString();
//                                UpdateContactConversationId(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.id, userId);
//                            }
//                        }
//                        else if (jsonData.Entry[0].Changes[0].Value.statuses[0].status.Equals("read"))
//                        {

//                            if (jsonData.Entry[0].Changes[0].Value.statuses[0].pricing != null)
//                            {
//                                ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                                {
//                                    TenantId = Tenant.TenantId.Value,
//                                    CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category),
//                                    MessageDateTime = DateTime.Now,
//                                    ConversationId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.id,
//                                    PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id,
//                                    MessageStatusId = (int)MessageStatusWhatsApp.Read
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
//                                    PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id,
//                                    MessageId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id,
//                                    MessageStatusId = (int)MessageStatusWhatsApp.Read
//                                };
//                                if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                                {
//                                    SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                                }
//                            }
//                        }

//                        else if (jsonData.Entry[0].Changes[0].Value.statuses[0].status.Equals("delivered"))
//                        {
//                            ConservationMeasurementMessage conservationMeasurementMessage = new ConservationMeasurementMessage()
//                            {
//                                TenantId = Tenant.TenantId.Value,
//                                CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category),
//                                MessageDateTime = DateTime.Now,
//                                ConversationId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.id,
//                                PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id,
//                                MessageId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id,
//                                MessageStatusId = (int)MessageStatusWhatsApp.Delivered
//                            };
//                            if (CommunicationInitiated.business_initiated == conservationMeasurementMessage.CommunicationInitiated)
//                            {
//                                SetConversationMeasurmentsInQueue(conservationMeasurementMessage);
//                            }
//                        }
//                    }
//                    return;
//                }


//                string medaiUrl = string.Empty;
//                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
//                var from = jsonData.Entry[0].Changes[0].Value.Messages[0].From; // extract the phone number from the webhook payload

//                List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();
//                AttachmentMessageModel attachmentMessageModel = new AttachmentMessageModel();

//                string msgBody = whatsAppAppService.MassageTypeText(jsonData.Entry[0].Changes[0].Value.Messages, Tenant.AccessToken, tAttachments, ref medaiUrl, attachmentMessageModel);
//                var type = jsonData.Entry[0].Changes[0].Value.Messages[0].Type;
//                userId = (Tenant.TenantId + "_" + from).ToString();





//                var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
//                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
//                var Customer = customerResult.Result;

//                if (Customer == null)
//                {
//                    var name = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;
//                    Customer = _dbService.CreateNewCustomer(from, name, type, Tenant.botId, Tenant.TenantId.Value, phoneNumberId);


//                    //create new contact in MG
//                    if (Tenant.TenantId.Value == 59)
//                    {
//                        CreateContactFromInfoSeed model = new CreateContactFromInfoSeed();

//                        model.firstname = name;
//                        model.lastname = name;
//                        model.phone = from;


//                        MGApiController mGApiController = new MGApiController();

//                        mGApiController.CreateContactsMg(model);
//                    }
//                }





//                var CustomerSendChat = _dbService.UpdateCustomerChat(Customer, userId, msgBody, type, Tenant.TenantId.Value, 0, medaiUrl, string.Empty, string.Empty);
//                Customer.customerChat = CustomerSendChat;





//                if (!Tenant.IsBotActive)
//                {
//                    // if the bot is not Active in Tenant
//                    //  var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, Key);
//                    // var x = BotNotActive(model, userId, Tenant.TenantId, Key, jsonModel).Result;

//                    //  _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                    //SocketIOManager.SendContact(Customer, Customer.TenantId.Value);
//                    return;

//                }



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
//                if (Tenant.IsWorkActive)
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
//                        var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, phoneNumberId, Tenant.AccessToken);
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




//                // await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);



//                if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
//                {
//                    await PrepareBotChatWithCustomer(from, Tenant, Customer, msgBody, tAttachments);

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

//        private async Task<bool> PrepareBotChatWithCustomer(string from, TenantModel Tenant, CustomerModel Customer, string msg, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments)
//        {
//            DirectLineConnector directLineConnector = new DirectLineConnector();
//            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
//            var Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments).Result;
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
//                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken);
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


//        #region Private Method 
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
//                        var StartDateSat = getValidValue(workModel.StartDateSat);
//                        var EndDateSat = getValidValue(workModel.EndDateSat);

//                        var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
//                        var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

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
//                        var StartDate = getValidValue(workModel.StartDateSun);
//                        var EndDate = getValidValue(workModel.EndDateSun);

//                        var StartDateSP = getValidValue(workModel.StartDateSunSP);
//                        var EndDateSP = getValidValue(workModel.EndDateSunSP);

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
//                        var StartDate = getValidValue(workModel.StartDateMon);
//                        var EndDate = getValidValue(workModel.EndDateMon);

//                        var StartDateSP = getValidValue(workModel.StartDateMonSP);
//                        var EndDateSP = getValidValue(workModel.EndDateMonSP);

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
//                        var StartDate = getValidValue(workModel.StartDateTues);
//                        var EndDate = getValidValue(workModel.EndDateTues);

//                        var StartDateSP = getValidValue(workModel.StartDateTuesSP);
//                        var EndDateSP = getValidValue(workModel.EndDateTuesSP);

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
//                        var StartDate = getValidValue(workModel.StartDateWed);
//                        var EndDate = getValidValue(workModel.EndDateWed);

//                        var StartDateSP = getValidValue(workModel.StartDateWedSP);
//                        var EndDateSP = getValidValue(workModel.EndDateWedSP);

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
//                        var StartDate = getValidValue(workModel.StartDateThurs);
//                        var EndDate = getValidValue(workModel.EndDateThurs);

//                        var StartDateSP = getValidValue(workModel.StartDateThursSP);
//                        var EndDateSP = getValidValue(workModel.EndDateThursSP);

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
//                        var StartDate = getValidValue(workModel.StartDateFri);
//                        var EndDate = getValidValue(workModel.EndDateFri);

//                        var StartDateSP = getValidValue(workModel.StartDateFriSP);
//                        var EndDateSP = getValidValue(workModel.StartDateFriSP);

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

//        private void UpdateContactConversationId(string ConversationId, string userId)
//        {




//            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
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
//                    }



//                    // customer.LastConversationStartDateTime = ConversationId;
//                    var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
//                }
//            }
//        }

//        #endregion


//    }
//}

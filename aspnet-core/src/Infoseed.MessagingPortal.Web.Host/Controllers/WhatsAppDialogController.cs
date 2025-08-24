

using Abp.Runtime.Caching;
using Abp.Web.Models;
using Framework.Data;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Models.WhatsAppDialog;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.Web.WhatsAppDialog;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebJobEntities;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppDialogController : MessagingPortalControllerBase
    {
        IDBService _dbService;
      //  private IHubContext<SignalR.TeamInboxHub> _hub;
        private TelemetryClient telemetry;

        private IUserAppService _iUserAppService;
        private readonly ICacheManager _cacheManager;

        public static Dictionary<string, bool> MessagesSent { get; set; }
        private readonly IDocumentClient _IDocumentClient;

        public WhatsAppDialogController(
             IDBService dbService,
           // IHubContext<SignalR.TeamInboxHub> hub,
             TelemetryClient telemetry
             ,IUserAppService iUserAppService
            ,ICacheManager cacheManager
                        , IDocumentClient iDocumentClient

            )
        {
            _dbService = dbService;
            //_hub = hub;
            this.telemetry = telemetry;
            _iUserAppService = iUserAppService;
            _cacheManager = cacheManager;
            _IDocumentClient = iDocumentClient;

        }

        public static string jsonMsg = "";



        [HttpGet("SetBundleActive")]//WebHookD360Model  //_360DialogModel
        public async Task<ActionResult> SetBundleActive(int TenantId ,string Key)
        {

            //   var Tenant = _dbService.GetTenantByKey2(Key).Result;
            if (Key.Equals("InfoSeed!@#$%^&*("))
            {
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == TenantId);

                if (tenant.workModel == null)
                {
                    tenant.workModel = new Infoseed.MessagingPortal.Web.Models.Sunshine.WorkModel();
                }
                tenant.IsBundleActive = true;
                _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());

                var result = await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                return Ok(true);
            }
            else
            {
                return Ok(Key);
            }
          

        }

        [HttpGet("getIsBundleActive")]//WebHookD360Model  //_360DialogModel
        public async Task<ActionResult> getIsBundleActive(int Key)
        {

            //   var Tenant = _dbService.GetTenantByKey2(Key).Result;

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == Key);
       


            return Ok(tenant.IsBundleActive);

        }

        [HttpPost("MessageHandler")]//WebHookD360Model  //_360DialogModel
        public ActionResult MessageHandler([FromBody] WebHookD360Model model, string Key)
        {
            return Ok();
            //try
            //{
            //    var jsonModel = JsonConvert.SerializeObject(model).ToString();

            //    // jsonMsg = jsonModel;
            //    if (model.statuses != null)
            //    {

            //        if (model.statuses.FirstOrDefault().conversation!=null)
            //        {
            //            this.telemetry.TrackTrace($"This is a statuses Message request: {jsonModel}", SeverityLevel.Critical);
            //            var Tenant = _dbService.GetTenantByKey2(Key).Result;

            //            if (Tenant!=null)
            //            {
            //                if (Tenant.IsBundleActive)
            //                {
            //                    SetConversationMeasurmentsInQueue(new ConservationMeasurementMessage()
            //                    {
            //                        TenantId = Tenant.TenantId.Value,
            //                        CommunicationInitiated = (CommunicationInitiated)Enum.Parse(typeof(CommunicationInitiated), model.statuses.FirstOrDefault().conversation.origin.type),
            //                        MessageDateTime = DateTime.Now.AddHours(AppSettingsModel.AddHour),
            //                        ConversationId = model.statuses.FirstOrDefault().conversation.id,
            //                        PhoneNumber = model.statuses.FirstOrDefault().recipient_id

            //                    });

            //                    UpdateContactConversationId(model.statuses.FirstOrDefault().conversation.id, model.statuses.FirstOrDefault().recipient_id, Tenant.TenantId.Value);
            //                }
            //                return Ok();
            //            }
            //            else
            //            {

            //                return Ok();
            //            }


            //        }
            //        return Ok();
            //    }


            //    //Duplicate request Bug
            //    if (jsonMsg.Equals(jsonModel))
            //    {
            //        this.telemetry.TrackTrace($"This is a douplicate request: {jsonModel}", SeverityLevel.Critical);
            //        return Ok();
            //    }
            //    jsonMsg = jsonModel;





            //    var userId = Key + model.messages[0].from;


            //    this.telemetry.TrackTrace($"Enter Message Hander: {jsonModel}", SeverityLevel.Information);


            //    if (MessagesSent == null)
            //        MessagesSent = new Dictionary<string, bool>();

            //    MessagesSent.TryAdd(userId, false);


            //    if (!MessagesSent[userId])
            //    {

            //        MessagesSent[userId] = true;

            //        //This scenario happened when the TeamInbox Send the message so smooch sends the trigger of that msg,
            //        //So discard it since it, not from the user.
            //        if (userId == null)
            //        {
            //            this.telemetry.TrackTrace($"This scenario happened when the TeamInbox Send the message so smooch sends the trigger of that msg So discard it since it, not from the user: {jsonModel}", SeverityLevel.Critical);
            //            MessagesSent[userId] = false;
            //            return Ok();
            //        }


            //        if (model.messages.Length > 0)
            //        {
            //            var msg = "";
            //            var tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();
            //            var Tenant = _dbService.GetTenantByKey(model.messages[0].from, Key).Result;
            //            if (Tenant==null)
            //            {
            //                MessagesSent[userId] = false;
            //                return Ok();
            //            }
            //            if (!Tenant.IsBundleActive)
            //            {
            //                MessagesSent[userId] = false;
            //                return Ok();
            //            }


            //            // return massages type
            //            msg = MassageType(Key, model, tAttachments, msg).Result;

            //            //Autoreply message
            //            if (Tenant.IsWorkActive)
            //            {
            //                // Work time 
            //                var x = IsAutoreplyAsync(Tenant, model, jsonModel, Key).Result;
            //                if (x != null)
            //                {
            //                    MessagesSent[userId] = false;
            //                    return Ok();

            //                }

            //            }


            //            // if bot is not Active in Tenant
            //            if (!Tenant.IsBotActive)
            //            {
            //                // if the bot is not Active in Tenant
            //                var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, Key);
            //               // var x = BotNotActive(model, userId, Tenant.TenantId, Key, jsonModel).Result;
            //                MessagesSent[userId] = false;
            //                return Ok();

            //            }
            //            else
            //            {
            //                var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, Key);



            //                if (Customer == null)
            //                {
            //                    MessagesSent[userId] = false;
            //                    return Ok();
            //                }
            //                //if the Customer bloked by admin 
            //                if (Customer.IsBlock)
            //                {
            //                    IsBlockCustomer(userId, jsonModel);
            //                    MessagesSent[userId] = false;
            //                    return Ok();
            //                }


            //               // var x = _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
            //                SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
            //                if (model.messages.FirstOrDefault().typeD360 == "location")
            //                {
            //                    model.messages.FirstOrDefault().typeD360 = "text";

            //                    model.messages.FirstOrDefault().type = "text";
            //                }


            //                try
            //                {
            //                    //if the Customer is not Expired( 24 Hour) and not locked by agent and chat with bot 
            //                    if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
            //                    {
            //                        //if Customer chating with bot
            //                        MessagesSent[userId] = true;
            //                        var xx = BotChatWithCustomer(Tenant, Customer, model, tAttachments, userId, msg, Tenant.TenantId, Key).Result;
            //                        MessagesSent[userId] = false;
            //                        return Ok();

            //                    }
            //                }
            //                catch
            //                {
            //                    MessagesSent[userId] = false;
            //                    return Ok();
            //                }


            //            }


            //        }

            //        MessagesSent[userId] = false;
            //        return Ok();

            //    }
            //    else
            //    {
            //        this.telemetry.TrackTrace($"This is a douplicate request: {jsonModel}", SeverityLevel.Critical);
            //        MessagesSent[userId] = false;
            //        return Ok();

            //    }


            //}
            //catch (Exception ex)
            //{



            //    this.telemetry.TrackException(ex);
            //    throw;

            //}

        }




        [Route("UpdateUserToken")]
        [HttpPost]
        public void UpdateUserToken(UserTokenModel userTokenModel)
        {
            _iUserAppService.UpdateUserToken(userTokenModel);


        }

        private static async Task<AttachmentModel> RetrievingMediaAsync(string appKey, string imgeID)
        {

            AttachmentModel attachmentModel = new AttachmentModel();

            var url = "https://waba.360dialog.io/v1/media/" + imgeID;
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            var client = new HttpClient(clientHandler);

            // Request headers
            client.DefaultRequestHeaders.Add("D360-Api-Key", appKey);

            var response = client.GetAsync(url).Result;
            attachmentModel.contentByte = response.Content.ReadAsByteArrayAsync().Result;
            attachmentModel.contentType = response.Content.Headers.ContentType.MediaType;

            return attachmentModel;
        }




        #region private
        private async Task<ActionResult> AgentChatWithCustomer(WebHookD360Model model, TenantModel Tenant, string D360Key)
        {
            var Customer = _dbService.CheckCustomerD360(model, Tenant.TenantId, D360Key);
           // var x = _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
            SocketIOManager.SendContact(Customer, Tenant.TenantId.Value);
            MessagesSent[Customer.userId] = false;
            return Ok();
        }

        private async Task<ActionResult> BotChatWithCustomer(TenantModel Tenant, CustomerModel Customer, WebHookD360Model model, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string userId, string msg, int? TenantId, string D360Key)
        {
            DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(D360Key, Tenant.DirectLineSecret, Customer.userId, Tenant.botId).Result;
            var Bot = directLineConnector.StartBotConversationD360(Customer.userId, Customer.ContactID, micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, Customer.phoneNumber, Customer.TenantId.ToString(), Customer.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, tAttachments).Result;

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == Customer.userId && a.TenantId == TenantId);

            Customer = customerResult.Result;

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


            bool isBotSendAttachments = false;
           // bool Islist = true;

            foreach (var msgBot in botListMessages)
            {

                SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model();

    

                if (msgBot.SuggestedActions == null)
                {


                    if (msgBot.InputHint=="image"|| msgBot.InputHint=="file"|| msgBot.InputHint=="video")
                    {

                        sendWhatsAppD360Model = new SendWhatsAppD360Model
                        {

                            mediaUrl = msgBot.Text.Replace("\r\n",""),
                            to = model.messages.FirstOrDefault().from,
                            type = msgBot.InputHint,
                            fileName= msgBot.Speak,
                            document = new SendWhatsAppD360Model.Document
                            {
                                link = msgBot.Text.Replace("\r\n", "")
                            }

                        };


                        isBotSendAttachments=true;
                    }
                    else
                    {
                        sendWhatsAppD360Model = new SendWhatsAppD360Model
                        {
                            to = model.messages.FirstOrDefault().from,
                            type = "text",
                            text = new SendWhatsAppD360Model.Text
                            {
                                body = msgBot.Text

                            }

                        };
                    }



                }
                else
                {
                    List<SendWhatsAppD360Model.Button> buttons = new List<SendWhatsAppD360Model.Button>();
                    foreach (var button in msgBot.SuggestedActions.Actions)
                    {
                        buttons.Add(new SendWhatsAppD360Model.Button
                        {
                            reply = new SendWhatsAppD360Model.Reply { id = button.Title, title = button.Title },
                            type = "reply"
                        });

                    }
                    if (msgBot.Attachments == null)
                    {





                        if (msgBot.Summary == null)
                        {

                            if (msgBot.Text.Length <= 1000)
                            {
                                sendWhatsAppD360Model = new SendWhatsAppD360Model
                                {
                                    to = model.messages.FirstOrDefault().from,
                                    type = "interactive",
                                    interactive = new SendWhatsAppD360Model.Interactive
                                    {
                                        type = "button",
                                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                        body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                                        // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                        action = new SendWhatsAppD360Model.Action
                                        {
                                            buttons = buttons.ToArray()

                                        }



                                    }

                                };
                            }
                            else
                            {
                                sendWhatsAppD360Model = new SendWhatsAppD360Model
                                {
                                    to = model.messages.FirstOrDefault().from,
                                    type = "text",
                                    text = new SendWhatsAppD360Model.Text
                                    {
                                        body = msgBot.Text

                                    }

                                };

                                var result22 = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry).Result;

                                //update Bot massage in cosmoDB 

                                if (result22 == HttpStatusCode.Created)
                                {

                                    Content message = contentM(msgBot, Tenant.botId);
                                    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                                    Customer.CreateDate = CustomerChat.CreateDate;
                                    Customer.customerChat = CustomerChat;
                                  //  var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                                    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                                }
                                else
                                {
                                    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
                                    MessagesSent[Customer.userId] = false;
                                    return Ok();

                                }

                                sendWhatsAppD360Model = new SendWhatsAppD360Model
                                {
                                    to = model.messages.FirstOrDefault().from,
                                    type = "interactive",
                                    interactive = new SendWhatsAppD360Model.Interactive
                                    {
                                        type = "button",
                                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                        body = new SendWhatsAppD360Model.Body { text = msgBot.Summary },
                                        // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                        action = new SendWhatsAppD360Model.Action
                                        {
                                            buttons = buttons.ToArray()

                                        }



                                    }

                                };



                            }




                        }
                        else
                        {

                            if (msgBot.Summary.Contains("هل تريد") || msgBot.Summary.Contains("Do you want"))
                            {

                                if (msgBot.Text.Length <= 1000)
                                {
                                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                                    {
                                        to = model.messages.FirstOrDefault().from,
                                        type = "interactive",
                                        interactive = new SendWhatsAppD360Model.Interactive
                                        {
                                            type = "button",
                                            // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                            body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                                            footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                            action = new SendWhatsAppD360Model.Action
                                            {
                                                buttons = buttons.ToArray()

                                            }



                                        }

                                    };
                                }
                                else
                                {
                                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                                    {
                                        to = model.messages.FirstOrDefault().from,
                                        type = "text",
                                        text = new SendWhatsAppD360Model.Text
                                        {
                                            body = msgBot.Text

                                        }

                                    };

                                    var result22 = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry).Result;

                                    //update Bot massage in cosmoDB 

                                    if (result22 == HttpStatusCode.Created)
                                    {

                                        Content message = contentM(msgBot, Tenant.botId);
                                        var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                                        Customer.CreateDate = CustomerChat.CreateDate;
                                        Customer.customerChat = CustomerChat;
                                      //  var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                                        SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                                    }
                                    else
                                    {
                                        this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
                                        MessagesSent[Customer.userId] = false;
                                        return Ok();

                                    }


                                    sendWhatsAppD360Model = new SendWhatsAppD360Model
                                    {
                                        to = model.messages.FirstOrDefault().from,
                                        type = "interactive",
                                        interactive = new SendWhatsAppD360Model.Interactive
                                        {
                                            type = "button",
                                            // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                            body = new SendWhatsAppD360Model.Body { text = msgBot.Summary },
                                            // footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                            action = new SendWhatsAppD360Model.Action
                                            {
                                                buttons = buttons.ToArray()

                                            }



                                        }

                                    };






                                }



                            }
                            else
                            {

                                sendWhatsAppD360Model = new SendWhatsAppD360Model
                                {
                                    to = model.messages.FirstOrDefault().from,
                                    type = "interactive",
                                    interactive = new SendWhatsAppD360Model.Interactive
                                    {
                                        type = "button",
                                        // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                        body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                                        footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                        action = new SendWhatsAppD360Model.Action
                                        {
                                            buttons = buttons.ToArray()

                                        }



                                    }

                                };




                            }









                        }

                    }
                    else
                    {


                        if (msgBot.InputHint == "منطقتك" || msgBot.SuggestedActions.Actions.Count() > 10)//msgBot.SuggestedActions.Actions.Count()>10
                        {
                            Content message2 = contentM(msgBot, Tenant.botId);

                            sendWhatsAppD360Model = new SendWhatsAppD360Model
                            {
                                to = model.messages.FirstOrDefault().from,
                                type = "text",
                                text = new SendWhatsAppD360Model.Text
                                {
                                    body = message2.text
                                }


                            };

                        }
                        else
                        {
                            List<SendWhatsAppD360Model.Section> sections = new List<SendWhatsAppD360Model.Section>();
                            List<SendWhatsAppD360Model.Row> rows = new List<SendWhatsAppD360Model.Row>();
                            foreach (var button in msgBot.SuggestedActions.Actions)
                            {

                                rows.Add(new SendWhatsAppD360Model.Row
                                {
                                    id = button.Title.Replace(" ", ""),
                                    title = button.Title,
                                    description = ""

                                });



                            }

                            sections.Add(new SendWhatsAppD360Model.Section
                            {
                                title = msgBot.InputHint,
                                rows = rows.ToArray()
                            });

                            sendWhatsAppD360Model = new SendWhatsAppD360Model
                            {
                                to = model.messages.FirstOrDefault().from,
                                type = "interactive",
                                interactive = new SendWhatsAppD360Model.Interactive
                                {
                                    type = "list",
                                    // header=new SendWhatsAppD360Model.Header {type="text", text="" },
                                    body = new SendWhatsAppD360Model.Body { text = msgBot.Text },
                                    footer = new SendWhatsAppD360Model.Footer { text = msgBot.Summary },
                                    action = new SendWhatsAppD360Model.Action
                                    {

                                        button = msgBot.InputHint,
                                        sections = sections.ToArray()

                                    }



                                }

                            };

                        }

                    }


                }


                var result = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry, isBotSendAttachments).Result;

                //update Bot massage in cosmoDB 

                if (result == HttpStatusCode.Created)
                {

                    Content message = contentM(msgBot, Tenant.botId);
                    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                    Customer.CreateDate = CustomerChat.CreateDate;
                    Customer.customerChat = CustomerChat;
                   // var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                }
                else
                {
                    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
                  //  MessagesSent[Customer.userId] = false;
                   // return Ok();

                }



            }


            MessagesSent[userId] = false;
            return Ok();
        }

        private Content contentM(Activity msgBot, string botId)
        {
            string tMessageToSend = string.Empty;
            List<CardAction> tOutActions = new List<CardAction>();
            int tOrder = 1;
            var optionlst = new Dictionary<int, string>();
            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
            {
                tOutActions.AddRange(msgBot.SuggestedActions.Actions);
            }

            foreach (var hc in tOutActions)
            {
                tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
                optionlst.Add(tOrder, hc.Title);
                tOrder++;
            }

            Content message = new Content
            {
                text = msgBot.Text + "\r\n" + tMessageToSend,
                type = "text",
                agentName = botId,
                agentId = "1000000"

            };

            return message;

        }

        private ActionResult IsBlockCustomer(string userId, string jsonModel)
        {
            this.telemetry.TrackTrace($"Customer is blocked : {jsonModel}", SeverityLevel.Warning);
            MessagesSent[userId] = false;
            return Ok();
        }

        private async Task<ActionResult> BotNotActive(WebHookD360Model model, string userId, int? Tenant, string D360Key, string jsonModel)
        {
            var Customer = _dbService.CheckCustomerD360(model, Tenant, D360Key);

            if (Customer.IsBlock)
            {
                this.telemetry.TrackTrace($"Customer is blocked : {jsonModel}", SeverityLevel.Warning);
                MessagesSent[userId] = false;
                return Ok();
            }

          //  var x = _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
            SocketIOManager.SendContact(Customer, Customer.TenantId.Value);
           
            MessagesSent[userId] = false;
            return Ok();
        }

        private async Task<ActionResult> AutoreplyAsync(TenantModel Tenant, WebHookD360Model model, string jsonModel, string D360Key)
        {


            var dayName = DateTime.Now.ToString("dddd");

            string timeNow = "";
            string timeStart = "";
            string timeEnd = "";
            string Text = "";

            if (dayName == "Saturday")
            {
                if (!Tenant.workModel.IsWorkActiveSat)
                {
                    return null;
                }
                timeNow = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateSat.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateSat.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextSat;

            }
            else if (dayName == "Sunday")
            {
                if (!Tenant.workModel.IsWorkActiveSun)
                {
                    return null;
                }

                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateSun.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateSun.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextSun;

            }
            else if (dayName == "Monday")
            {
                if (!Tenant.workModel.IsWorkActiveMon)
                {
                    return null;
                }

                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateMon.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateMon.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextMon;

            }
            else if (dayName == "Tuesday")
            {
                if (!Tenant.workModel.IsWorkActiveTues)
                {
                    return null;
                }

                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateTues.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateTues.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextTues;

            }
            else if (dayName == "Wednesday")
            {
                if (!Tenant.workModel.IsWorkActiveWed)
                {
                    return null;
                }

                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateWed.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateWed.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextWed;

            }
            else if (dayName == "Thursday")
            {
                if (!Tenant.workModel.IsWorkActiveThurs)
                {
                    return null;
                }

                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateThurs.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateThurs.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextThurs;

            }
            else if (dayName == "Friday")
            {
                if (!Tenant.workModel.IsWorkActiveFri)
                {
                    return null;
                }
                timeNow = DateTime.Now.ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeStart = Tenant.workModel.StartDateFri.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                timeEnd = Tenant.workModel.EndDateFri.AddHours(AppSettingsModel.DivHour).ToString("HH:mm:ss ", CultureInfo.InvariantCulture);
                Text = Tenant.workModel.WorkTextFri;

            }



            var resulttimeNow = Convert.ToDateTime(timeNow);
            var resulttimeStart = Convert.ToDateTime(timeStart);
            var resulttimeEnd = Convert.ToDateTime(timeEnd);

            var resulttimeNowS = Convert.ToInt64(timeNow.Split(":")[0]);
            var resulttimeStartS = Convert.ToInt64(timeStart.Split(":")[0]);
            var resulttimeEndS = Convert.ToInt64(timeEnd.Split(":")[0]);


            var resulttimeStart2 = Convert.ToDateTime(timeStart);
            var resulttimeEnd2 = Convert.ToDateTime(timeEnd);



            var SFormat = "";

            try
            {
                SFormat = string.Format(Text, resulttimeStart2.ToString("hh:mm tt", CultureInfo.InvariantCulture), resulttimeEnd2.ToString("hh:mm tt", CultureInfo.InvariantCulture));
            }
            catch
            {
                SFormat = Text + " : " + resulttimeStart2.ToString("hh:mm tt", CultureInfo.InvariantCulture) + "  =>  " + resulttimeEnd2.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            }

            if (!(resulttimeStart <= resulttimeNow && resulttimeNow <= resulttimeEnd))
            {
                var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, D360Key);

                //if the Customer bloked by admin 
                if (Customer.IsBlock)
                {
                    IsBlockCustomer(model.messages.FirstOrDefault().from, jsonModel);
                    MessagesSent[model.messages.FirstOrDefault().from] = false;
                    return Ok();
                }

                Content message = new Content();
                message.text = SFormat;
                message.agentName = Tenant.botId;
                message.agentId = "1000000";
                message.type = "text";

                SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model
                {
                    to = model.messages.FirstOrDefault().from,
                    type = "text",
                    text = new SendWhatsAppD360Model.Text
                    {
                        body = SFormat

                    }

                };

                //Todo Make the connector as a Service and return status in this method
                var result = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry).Result;


                if (result == HttpStatusCode.Created)
                {
                    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                    Customer.CreateDate = CustomerChat.CreateDate;
                    Customer.customerChat = CustomerChat;
                  //  var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                    MessagesSent[Customer.userId] = false;
                    return Ok();
                }
                else
                {
                    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog  : {jsonModel}", SeverityLevel.Critical);
                }

                MessagesSent[model.messages.FirstOrDefault().from] = false;
                return Ok();

            }
            else
            {
                if (resulttimeStartS >= resulttimeEndS)
                {

                    var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, D360Key);

                    //if the Customer bloked by admin 
                    if (Customer.IsBlock)
                    {
                        IsBlockCustomer(model.messages.FirstOrDefault().from, jsonModel);
                        MessagesSent[model.messages.FirstOrDefault().from] = false;
                        return Ok();
                    }

                    Content message = new Content();
                    message.text = SFormat;
                    message.agentName = Tenant.botId;
                    message.agentId = "1000000";
                    message.type = "text";

                    SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model
                    {
                        to = model.messages.FirstOrDefault().from,
                        type = "text",
                        text = new SendWhatsAppD360Model.Text
                        {
                            body = SFormat

                        }

                    };

                    //Todo Make the connector as a Service and return status in this method
                    var result = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry).Result;


                    if (result == HttpStatusCode.Created)
                    {
                        var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                        Customer.CreateDate = CustomerChat.CreateDate;
                        Customer.customerChat = CustomerChat;
                        //var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                        SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                        MessagesSent[Customer.userId] = false;
                        return Ok();
                    }
                    else
                    {
                        this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog  : {jsonModel}", SeverityLevel.Critical);
                    }

                    MessagesSent[model.messages.FirstOrDefault().from] = false;
                    return Ok();

                }

                return null;
            }
            //MessagesSent[model.messages.FirstOrDefault().from] = false;
            //return Ok();
        }
        private async Task<ActionResult> IsAutoreplyAsync(TenantModel Tenant, WebHookD360Model model, string jsonModel, string D360Key)
        {


         
      

           
             // SFormat = string.Format(Text, resulttimeStart2.ToString("hh:mm tt", CultureInfo.InvariantCulture), resulttimeEnd2.ToString("hh:mm tt", CultureInfo.InvariantCulture));

            string SFormat = string.Empty;
            /// out of working hours  
            if (Tenant.IsWorkActive && checkIsInService(Tenant.workModel, out SFormat))

            {
                return null;
            }

            else {
                var Customer = _dbService.CheckIsNewCustomerWithBotD360(model, Tenant.botId, Tenant.TenantId, D360Key);
                //if the Customer bloked by admin 
                if (Customer.IsBlock)
                {
                    IsBlockCustomer(model.messages.FirstOrDefault().from, jsonModel);
                    MessagesSent[model.messages.FirstOrDefault().from] = false;
                    return Ok();
                }
                Content message = new Content();
                message.text = SFormat;
                message.agentName = Tenant.botId;
                message.agentId = "1000000";
                message.type = "text";
                SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model
                {
                    to = model.messages.FirstOrDefault().from,
                    type = "text",
                    text = new SendWhatsAppD360Model.Text
                    {
                        body = SFormat

                    }

                };
                //Todo Make the connector as a Service and return status in this method
                var result = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, sendWhatsAppD360Model, telemetry).Result;
                if (result == HttpStatusCode.Created)
                {
                    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                    Customer.CreateDate = CustomerChat.CreateDate;
                    Customer.customerChat = CustomerChat;
                   // var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                    MessagesSent[Customer.userId] = false;
                    return Ok();
                }
                else
                {
                    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog  : {jsonModel}", SeverityLevel.Critical);
                }

                MessagesSent[model.messages.FirstOrDefault().from] = false;
                return Ok();


            }
           

         
          
        }

        private static async Task<string> MassageType(string key, WebHookD360Model model, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string msg)
        {
            var type = "";
            var url = "";
            if (model.messages[0].type == SmoochContentTypeEnum.Video)
            {
                // Retrieving Media byet
                var RetrievingMedia = RetrievingMediaAsync(key, model.messages[0].video.id).Result;

                byte[] videoBytes = RetrievingMedia.contentByte;

                var extention = "." + model.messages[0].video.mime_type.Split("/")[1];
                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                AttachmentContent attachmentContent = new AttachmentContent()
                {
                    Content = videoBytes,
                    Extension = extention,
                    MimeType = model.messages[0].video.mime_type
                };
                url = azureBlobProvider.Save(attachmentContent).Result;
                model.messages.FirstOrDefault().video.link = url;

                type = "video";
                model.messages.FirstOrDefault().type = model.messages[0].video.mime_type;
                model.messages.FirstOrDefault().mediaUrl = url;


                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(model.messages[0].video.mime_type, url, videoBytes));


            }


            else if (model.messages[0].type == SmoochContentTypeEnum.Image)
            {
                // Retrieving Media byet
                var RetrievingMedia = RetrievingMediaAsync(key, model.messages[0].image.id).Result;

                byte[] imageBytes = RetrievingMedia.contentByte;

                var extention = "." + model.messages[0].image.mime_type.Split("/")[1];
                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                AttachmentContent attachmentContent = new AttachmentContent()
                {
                    Content = imageBytes,
                    Extension = extention,
                    MimeType = model.messages[0].image.mime_type
                };
                url = azureBlobProvider.Save(attachmentContent).Result;
                model.messages.FirstOrDefault().image.link = url;

               type = "image";
                model.messages.FirstOrDefault().type = model.messages[0].image.mime_type;
                model.messages.FirstOrDefault().mediaUrl = url;


                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(model.messages[0].image.mime_type, url, imageBytes,"", url));


            }
            else if (model.messages[0].type == SmoochContentTypeEnum.Document)
            {
                // Retrieving Media byet
                var RetrievingMedia = RetrievingMediaAsync(key, model.messages[0].document.id).Result;
                byte[] imageBytes = RetrievingMedia.contentByte;
                var extention = "." + model.messages[0].document.mime_type.Split("/")[1];
                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                AttachmentContent attachmentContent = new AttachmentContent()
                {
                    Content = imageBytes,
                    Extension = extention,
                    MimeType = model.messages[0].document.mime_type
                };
                type = "file";
                url = azureBlobProvider.Save(attachmentContent).Result;
                //model.messages.FirstOrDefault().document.link = url;
                //model.messages.FirstOrDefault().document.mime_type = "file";

                model.messages.FirstOrDefault().type = "file";
                model.messages.FirstOrDefault().mediaUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(model.messages[0].document.mime_type, url, imageBytes, "", url));

            }

            else if (model.messages[0].type == SmoochContentTypeEnum.Voice)
            {
                // Retrieving Media byet
                var RetrievingMedia = RetrievingMediaAsync(key, model.messages[0].voice.id).Result;
                byte[] imageBytes = RetrievingMedia.contentByte;
                var extention = "." + model.messages[0].voice.mime_type.Split("/")[1];
                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                AttachmentContent attachmentContent = new AttachmentContent()
                {
                    Content = imageBytes,
                    Extension = extention,
                    MimeType = model.messages[0].voice.mime_type
                };
                type = "audio";
                url = azureBlobProvider.Save(attachmentContent).Result;
                //model.messages.FirstOrDefault().voice.link = url;
                //model.messages.FirstOrDefault().voice.mime_type = "file";
                model.messages.FirstOrDefault().type = "audio";
                model.messages.FirstOrDefault().mediaUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(model.messages[0].voice.mime_type, url, imageBytes, "", url));

            }

            else if (model.messages[0].type == SmoochContentTypeEnum.Location)
            {
                var query = model.messages[0].location.latitude.ToString() + "," + model.messages[0].location.longitude.ToString();
                model.messages.FirstOrDefault().text = new WebHookD360Model.Text { body = query };
                model.messages.FirstOrDefault().type = "location";
                type = "location";
                model.messages.FirstOrDefault().textD360 = query;
                msg = query;
            }
            else if (model.messages[0].type == SmoochContentTypeEnum.Text)
            {
                msg = model.messages[0].text.body;


                model.messages.FirstOrDefault().textD360 = msg;
                model.messages.FirstOrDefault().type = "text";
                type = "text";
            }
            else if (model.messages[0].type == SmoochContentTypeEnum.Interactive)
            {
                if (model.messages[0].interactive.type == "list_reply")
                {

                    msg = model.messages[0].interactive.list_reply.title;

                    model.messages.FirstOrDefault().textD360 = msg;
                    model.messages.FirstOrDefault().type = "text";
                    type = "text";

                }
                else
                {

                    msg = model.messages[0].interactive.button_reply.title;


                    model.messages.FirstOrDefault().textD360 = msg;
                    model.messages.FirstOrDefault().type = "text";
                    type = "text";
                }

            }

            model.messages.FirstOrDefault().textD360 = msg;
            model.messages.FirstOrDefault().mediaUrl = url;
            model.messages.FirstOrDefault().typeD360 = type;

            return msg;
        }


        private async Task<string> DeleteConversation(string userId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.userId == userId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.userId= '" + userId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
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
            catch(Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                 this.telemetry.TrackTrace(Error, SeverityLevel.Error);

            }
           

        }


        private void UpdateContactConversationId(string ConversationId, string phoneNumber, int? TenantId)
        {




            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == TenantId + "_" + phoneNumber);//&& a.TenantId== TenantId

            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer == null)
                {

                }
                else
                {


                    if(customer.ConversationId != ConversationId)
                    {
                        customer.ConversationId = ConversationId;
                        customer.LastConversationStartDateTime = DateTime.Now.AddHours(AppSettingsModel.AddHour);
                    }


                    
                   // customer.LastConversationStartDateTime = ConversationId;
                    var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                }
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
                         var StartDateSat = getValidValue(workModel.StartDateSat); 
                         var EndDateSat= getValidValue(workModel.EndDateSat);

                         var StartDateSatSP = getValidValue(workModel.StartDateSatSP);
                         var EndDateSatSP = getValidValue(workModel.EndDateSatSP);

                         if ((timeOfDay >= StartDateSat.TimeOfDay && timeOfDay <= EndDateSat.TimeOfDay) ||  (timeOfDay >= StartDateSatSP.TimeOfDay && timeOfDay <= EndDateSatSP.TimeOfDay))

                            {
                                result = true;
                            }
                            else
                            {
                            outWHMessage=  string.Format(workModel.WorkTextSat, StartDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture),EndDateSat.ToString("hh:mm tt", CultureInfo.InvariantCulture)
                                           ,StartDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture), EndDateSatSP.ToString("hh:mm tt", CultureInfo.InvariantCulture) );
                            result = false;
                           }
                        }

                        break;
                    case DayOfWeek.Sunday:
                        if (workModel.IsWorkActiveSun)
                        {
                        var StartDate = getValidValue(workModel.StartDateSun);
                        var EndDate = getValidValue(workModel.EndDateSun);

                        var StartDateSP = getValidValue(workModel.StartDateSunSP);
                        var EndDateSP = getValidValue(workModel.EndDateSunSP);

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
                        var StartDate = getValidValue(workModel.StartDateMon);
                        var EndDate = getValidValue(workModel.EndDateMon);

                        var StartDateSP = getValidValue(workModel.StartDateMonSP);
                        var EndDateSP = getValidValue(workModel.EndDateMonSP);

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
                        var StartDate = getValidValue(workModel.StartDateTues);
                        var EndDate = getValidValue(workModel.EndDateTues);

                        var StartDateSP = getValidValue(workModel.StartDateTuesSP);
                        var EndDateSP = getValidValue(workModel.EndDateTuesSP);

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
                        var StartDate = getValidValue(workModel.StartDateWed);
                        var EndDate = getValidValue(workModel.EndDateWed);

                        var StartDateSP = getValidValue(workModel.StartDateWedSP);
                        var EndDateSP = getValidValue(workModel.EndDateWedSP);

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
                        var StartDate = getValidValue(workModel.StartDateThurs);
                        var EndDate = getValidValue(workModel.EndDateThurs);

                        var StartDateSP = getValidValue(workModel.StartDateThursSP);
                        var EndDateSP = getValidValue(workModel.EndDateThursSP);

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
                        var StartDate = getValidValue(workModel.StartDateFri);
                        var EndDate = getValidValue(workModel.EndDateFri);

                        var StartDateSP = getValidValue(workModel.StartDateFriSP);
                        var EndDateSP = getValidValue(workModel.StartDateFriSP);

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
        #endregion


        [HttpGet("getallCache")]
        [DontWrapResult]
        public string getCache(string phoneNumberId)
        {
            TenantModel Tenant = new TenantModel();



            var objTenant = _cacheManager.GetCache("CacheTenant").Get(phoneNumberId.ToString(), cache => cache);
            if (objTenant.Equals(phoneNumberId.ToString()))
            {
                Tenant = _dbService.GetTenantByKey("", phoneNumberId.ToString()).Result;
                _cacheManager.GetCache("CacheTenant").Set(phoneNumberId.ToString(), Tenant);
            }
            else
            {
                Tenant = (TenantModel)objTenant;
            }
            return "";
        }
    }
}

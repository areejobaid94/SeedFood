//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using Infoseed.MessagingPortal.Web.Models;
//using Infoseed.MessagingPortal.Web.Models.Sunshine;
//using Infoseed.MessagingPortal.Web.Sunshine;
//using LiteDB;
//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.DataContracts;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Newtonsoft.Json;

//namespace Infoseed.MessagingPortal.Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SunshineController : MessagingPortalControllerBase
//    {
//        IDBService _dbService;
//        private IHubContext<SignalR.TeamInboxHub> _hub;
//        //  private TelemetryClient telemetry;
//        public static Dictionary<string, bool> MessagesSent { get; set; }

//        public SunshineController(
//             IDBService dbService,
//            IHubContext<SignalR.TeamInboxHub> hub
//            //  TelemetryClient telemetry
//            )
//        {
//            _dbService = dbService;
//            _hub = hub;
//            //  this.telemetry = telemetry;
//        }



//        [HttpPost("MessageHandler")]
//        public async Task<ActionResult> MessageHandler(SunshineMsgReceivedV1Model model)
//        {
//            try
//            {
//                var jsonModel = JsonConvert.SerializeObject(model).ToString();
//                // this.telemetry.TrackTrace($"Enter Message Hander: {jsonModel}", SeverityLevel.Information);                       

//                if (MessagesSent == null)
//                    MessagesSent = new Dictionary<string, bool>();

//                MessagesSent.TryAdd(model.conversation._id, false);

//                //Duplicate reques qick fix SunshineMsgReceivedV1Model
//                if (!MessagesSent[model.conversation._id])
//                {
//                    MessagesSent[model.conversation._id] = true;

//                    //This scenario happened when the TeamInbox Send the message so smooch sends the trigger of that msg,
//                    //So discard it since it, not from the user.
//                    if (model.conversation._id == null)
//                    {
//                        //  this.telemetry.TrackTrace($"This scenario happened when the TeamInbox Send the message so smooch sends the trigger of that msg So discard it since it, not from the user: {jsonModel}", SeverityLevel.Critical);
//                        MessagesSent[model.conversation._id] = false;
//                        return Ok();
//                    }

//                    if (model.messages.Length > 0)
//                    {
//                        var msg = model.messages.FirstOrDefault().text;
//                        var location = "";
//                        var conversationID = model.conversation._id;
//                        var content = model.messages.FirstOrDefault();
//                        var sunshineMsgInfo = new SunshineReqInfoModel() { appID = model.app._id, conversationID = conversationID };
//                        var tAttachments = new List<Microsoft.Bot.Connector.DirectLine.Attachment>();
//                        var userId = model.conversation._id;
//                        var Tenant = _dbService.GetTenant(model.app._id).Result;


//                        // return massages type
//                        msg = await MassageType(model, msg, content, tAttachments);

//                        //Autoreply message
//                        if (Tenant.IsWorkActive)
//                        {
//                            // Work time 
//                            await AutoreplyAsync(Tenant, model, userId, conversationID, jsonModel);
//                            MessagesSent[userId] = false;
//                            return Ok();
//                        }

//                        // if bot is not Active in Tenant
//                        if (!Tenant.IsBotActive)
//                        {
//                            // if the bot is not Active in Tenant
//                            await BotNotActive(model, userId, jsonModel);
//                            MessagesSent[userId] = false;
//                            return Ok();

//                        }
//                        else
//                        {
//                            var Customer = _dbService.CheckIsNewCustomerWithBot(model, Tenant.botId);

//                            //if the Customer bloked by admin 
//                            if (Customer.IsBlock)
//                            {

//                                IsBlockCustomer(userId, jsonModel);
//                                MessagesSent[userId] = false;
//                                return Ok();
//                            }

//                            //if the Customer is not Expired( 24 Hour) and not locked by agent and chat with bot 
//                            if (!Customer.IsConversationExpired && !Customer.IsLockedByAgent && Customer.IsBotChat && !Customer.IsSupport)
//                            {
//                                //if Customer chating with bot
//                                await BotChatWithCustomer(conversationID, Tenant, sunshineMsgInfo, Customer, model, location, tAttachments, userId, msg);
//                                MessagesSent[model.conversation._id] = false;
//                                return Ok();

//                            }
//                            else
//                            {
//                                //if Customer chating with Agent
//                                await AgentChatWithCustomer(model, Tenant);
//                                MessagesSent[model.conversation._id] = false;
//                                return Ok();

//                            }

//                        }

//                    }

//                    MessagesSent[model.conversation._id] = false;
//                    return Ok();



//                }
//                else
//                {
//                    // this.telemetry.TrackTrace($"This is a douplicate request: {jsonModel}", SeverityLevel.Critical);
//                    MessagesSent[model.conversation._id] = false;
//                    return Ok();

//                }

//            }
//            catch (Exception ex)
//            {
//                // this.telemetry.TrackException(ex);
//                throw;
//            }
//        }




//        #region private
//        private async Task<ActionResult> AgentChatWithCustomer(SunshineMsgReceivedV1Model model, TenantModel Tenant)
//        {
//            var Customer = _dbService.CheckCustomer(model);
//            Customer.TenantId = Tenant.TenantId;

//            await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//            return Ok();
//        }

//        private async Task<ActionResult> BotChatWithCustomer(string conversationID, TenantModel Tenant, SunshineReqInfoModel sunshineMsgInfo, CustomerModel Customer, SunshineMsgReceivedV1Model model, string location, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, string userId, string msg)
//        {
//            //update Customer massage in cosmoDB 
//            var Customer2 = _dbService.CheckCustomerBot(model, Customer.IsLockedByAgent, Customer.IsBotChat);
//            await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer2);


//            DirectLineConnector directLineConnector = new DirectLineConnector();
//            var micosoftConversationID = directLineConnector.CheckIsNewConversation(conversationID, Tenant.DirectLineSecret, sunshineMsgInfo.appID, Customer.userId, Tenant.botId).Result;
//            var Bot = directLineConnector.StartBotConversation(micosoftConversationID.MicrosoftBotId, msg, Tenant.DirectLineSecret, Tenant.botId, model.appUser.clients.FirstOrDefault().externalId, Customer.TenantId.ToString(), Customer.displayName, micosoftConversationID.watermark, tAttachments).Result;

//            foreach (var msgBot in Bot)
//            {
//                Content message = new Content
//                {
//                    text = msgBot.Text,
//                    type = "text",
//                    agentName = Tenant.botId,
//                    agentId = "1000000"

//                };
//                var result = await SunshineConnector.PostMsgToSmooch(Customer.SunshineAppID, Customer.SunshineConversationId, message);


//                //update Bot massage in cosmoDB 
//                var CustomerChat = _dbService.UpdateCustomerChat(Customer.TenantId, message, Customer.userId, Customer.SunshineConversationId);
//                Customer.CreateDate = CustomerChat.CreateDate;
//                Customer.customerChat = CustomerChat;
//                Customer.IsComplaint = Customer2.IsComplaint;
//                await _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);

//            }


//            MessagesSent[userId] = false;
//            return Ok();
//        }

//        private ActionResult IsBlockCustomer(string userId, string jsonModel)
//        {
//            /// this.telemetry.TrackTrace($"Customer is blocked : {jsonModel}", SeverityLevel.Warning);
//            MessagesSent[userId] = false;
//            return Ok();
//        }

//        private async Task<ActionResult> BotNotActive(SunshineMsgReceivedV1Model model, string userId, string jsonModel)
//        {
//            var Customer = _dbService.CheckCustomer(model);

//            if (Customer.IsBlock)
//            {
//                //this.telemetry.TrackTrace($"Customer is blocked : {jsonModel}", SeverityLevel.Warning);
//                MessagesSent[userId] = false;
//                return Ok();
//            }

//            await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//            MessagesSent[userId] = false;
//            return Ok();
//        }

//        private async Task<ActionResult> AutoreplyAsync(TenantModel Tenant, SunshineMsgReceivedV1Model model, string userId, string conversationID, string jsonModel)
//        {
//            string timeNow = DateTime.Now.AddHours(6).ToString("hh:mm:ss tt");
//            string timeStart = Tenant.StartDate.AddHours(3).ToString("hh:mm:ss tt");
//            string timeEnd = Tenant.EndDate.AddHours(3).ToString("hh:mm:ss tt");

//            var resulttimeNow = Convert.ToDateTime(timeNow);
//            var resulttimeStart = Convert.ToDateTime(timeStart);
//            var resulttimeEnd = Convert.ToDateTime(timeEnd);


//            var resulttimeStart2 = Convert.ToDateTime(timeStart);
//            var resulttimeEnd2 = Convert.ToDateTime(timeEnd);



//            var SFormat = "";

//            try
//            {
//                SFormat = string.Format(Tenant.WorkText, resulttimeStart2.AddHours(3).ToString("hh:mm tt"), resulttimeEnd2.AddHours(3).ToString("hh:mm tt"));
//            }
//            catch
//            {
//                SFormat = Tenant.WorkText + " : " + resulttimeStart2.AddHours(3).ToString("hh:mm tt") + "  =>  " + resulttimeEnd2.AddHours(3).ToString("hh:mm tt");
//            }

//            if (!(resulttimeStart <= resulttimeNow && resulttimeNow <= resulttimeEnd))
//            {
//                var Customer = _dbService.CheckIsNewCustomerWithBot(model, Tenant.botId);

//                //if the Customer bloked by admin 
//                if (Customer.IsBlock)
//                {
//                    IsBlockCustomer(userId, jsonModel);
//                    MessagesSent[userId] = false;
//                    return Ok();
//                }

//                Content message = new Content();
//                message.text = SFormat;
//                message.agentName = Tenant.botId;
//                message.agentId = "1000000";
//                message.type = "text";


//                //Todo Make the connector as a Service and return status in this method
//                var result = await SunshineConnector.PostMsgToSmooch(model.app._id, conversationID, message);

//                if (result == HttpStatusCode.Created)
//                {
//                    var CustomerChat = _dbService.UpdateCustomerChat(Tenant.TenantId, message, userId, conversationID);
//                    Customer.CreateDate = CustomerChat.CreateDate;
//                    Customer.customerChat = CustomerChat;
//                    await _hub.Clients.All.SendAsync("brodCastEndUserMessage", Customer);
//                    MessagesSent[userId] = false;
//                    return Ok();
//                }

//                MessagesSent[userId] = false;
//                return Ok();

//            }
//            MessagesSent[userId] = false;
//            return Ok();
//        }

//        private static async Task<string> MassageType(SunshineMsgReceivedV1Model model, string msg, SunshineMsgReceivedV1Model.Message content, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments)
//        {
//            if (content.type == SmoochContentTypeEnum.Image)
//            {
//                var webClient = new WebClient();
//                byte[] imageBytes = webClient.DownloadData(content.mediaUrl);
//                var extention = Path.GetExtension(content.mediaUrl);
//                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
//                AttachmentContent attachmentContent = new AttachmentContent()
//                {
//                    Content = imageBytes,
//                    Extension = extention,
//                    MimeType = content.mediaType
//                };
//                var url = await azureBlobProvider.Save(attachmentContent);
//                model.messages.FirstOrDefault().mediaUrl = url;


//                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(content.mediaType, url, imageBytes));


//            }
//            else if (content.type == SmoochContentTypeEnum.File)
//            {
//                var types = AppsettingsModel.AttacmentTypesAllowed;
//                var webClient = new WebClient();
//                byte[] imageBytes = webClient.DownloadData(content.mediaUrl);
//                var extention = Path.GetExtension(content.mediaUrl);

//                var type = types[extention];
//                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
//                AttachmentContent attachmentContent = new AttachmentContent()
//                {
//                    Content = imageBytes,
//                    Extension = extention,
//                    MimeType = content.mediaType

//                };
//                var url = await azureBlobProvider.Save(attachmentContent);
//                model.messages.FirstOrDefault().mediaUrl = url;
//                model.messages.FirstOrDefault().type = type;
//            }
//            else if (content.type == SmoochContentTypeEnum.Text && model.messages.FirstOrDefault().text.Contains("http"))
//            {
//                var types = AppsettingsModel.AttacmentTypesAllowed;
//                var url = model.messages.FirstOrDefault().text.Substring(model.messages.FirstOrDefault().text.IndexOf("http"));
//                var extention = Path.GetExtension(url);
//                var type = types[extention];
//                model.messages.FirstOrDefault().mediaUrl = url;
//                model.messages.FirstOrDefault().text = "";
//                model.messages.FirstOrDefault().type = type;
//            }
//            else if (content.type == SmoochContentTypeEnum.Location && model.messages.FirstOrDefault().text.Contains("https"))
//            {

//                var url = model.messages.FirstOrDefault().text.Substring(model.messages.FirstOrDefault().text.IndexOf("https"));
//                var query = url.Substring(url.IndexOf("=") + 1);
//                msg = query;
//                model.messages.FirstOrDefault().text = query;
//                model.messages.FirstOrDefault().type = "text";
//            }

//            return msg;
//        }

//        #endregion

//    }

//}


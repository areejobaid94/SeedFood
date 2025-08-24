using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.Sunshine.Infrastructure;
using Infoseed.MessagingPortal.Sunshine.Interfaces;
using Infoseed.MessagingPortal.Sunshine.Models;
using Infoseed.MessagingPortal.Sunshine.SignalR;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
//using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;

namespace Infoseed.MessagingPortal.Sunshine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SunshineController : ControllerBase
    {
        IDBService _liteDBService;
        private IHubContext<TeamInboxHub> _hub;

        public SunshineController(
            IDBService liteDBService,
            IHubContext<TeamInboxHub> hub)
        {
            _liteDBService = liteDBService;
            _hub = hub;
        }

        public static string jsonMsg = "";
        [HttpPost("MessageHandler")]
        public async Task<ActionResult> MessageHandler(SunshineMsgReceivedModel model)
        {
            try
            {
               
                //Duplicate reques qick fix
                var jsonModel = JsonConvert.SerializeObject(model).ToString();


                //Duplicate request Bug
                if (jsonMsg.Equals(jsonModel))
                {
                    return Ok();
                }

                //This scenario happened when the TeamInbox Send the message so smooch sends the trigger of that msg,
                //So discard it since it, not from the user.
                if (model.events[0].payload.message.author.userId == null)
                {
                    return Ok();
                }

                jsonMsg = jsonModel;
                if (model.events.Count > 0)
                {
                    var conversationID = model.events[0].payload.conversation.id;
                    //Check IF the conversation already been made with the bot else create a conversation  DirectLineConnector.StartBotConversation()
                    var content = model.events[0].payload.message.content;
                    if(content.type== SmoochContentTypeEnum.Image)
                    {
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(content.mediaUrl);
                        var extention=Path.GetExtension(content.mediaUrl);
                        AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                        AttachmentContent attachmentContent = new AttachmentContent()
                        {
                            Content = imageBytes,
                            Extension = extention,
                            MimeType = content.mediaType
                        };
                        var url=await azureBlobProvider.Save(attachmentContent);
                        model.events[0].payload.message.content.mediaUrl = url;
                        await _liteDBService.CheckIsNewCustomer(model);
                    }
                    else if(content.type == SmoochContentTypeEnum.File)
                    {
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(content.mediaUrl);
                        var extention = Path.GetExtension(content.mediaUrl);
                        //string mimeType = "";
                        //new FileExtensionContentTypeProvider().TryGetContentType(content.mediaUrl, out mimeType);
                        AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                        AttachmentContent attachmentContent = new AttachmentContent()
                        {
                            Content = imageBytes,
                            Extension = extention,
                            MimeType= content.mediaType

                        };
                        var url = await azureBlobProvider.Save(attachmentContent);
                        model.events[0].payload.message.content.mediaUrl = url;
                        await _liteDBService.CheckIsNewCustomer(model);
                    }
                    else if(content.type == SmoochContentTypeEnum.Text)
                    {
                        //Send the msg to the Microsoft Bot 
                        var msg = model.events[0].payload.message.content.text;

                        //Sunshine request information that will be used in process 
                        var sunshineMsgInfo = new SunshineReqInfoModel() { appID = model.app.id, conversationID = conversationID };

                        //DirectLineConnector directLineConnector = new DirectLineConnector();
                        await _liteDBService.CheckIsNewCustomer(model);
                        //  var micosoftConversationID=await directLineConnector.CheckIsNewConversation(conversationID);
                        //await directLineConnector.StartBotConversation(micosoftConversationID, msg, sunshineMsgInfo);

                        //await StartBotConversation2(null, msg, sunshineMsgInfo);
                        //await postMsgToSmooch(model.app.id, conversationID, msg);
                    }
                    else if(content.type == SmoochContentTypeEnum.Location)
                    {
                       
                        var msg = model.events[0].payload.message.content.text;

                        //The Coordinates
                        var coordinate = model.events[0].payload.message.content.coordinates;
                        //If it google map the link will be attached in the text 
                        //as: Location shared:\nhttps://maps.google.com/maps?q=31.962173,35.834653 
                        var locationLink = msg;

                        //Sunshine request information that will be used in process 
                        var sunshineMsgInfo = new SunshineReqInfoModel() { appID = model.app.id, conversationID = conversationID };

                        
                        await _liteDBService.CheckIsNewCustomer(model);
                    }


                    var userId = model.events[0].payload.message.author.userId;
                    CustomerChat CustomerChat = await _liteDBService.GetCustomersLastMessage(userId);
                    await _hub.Clients.All.SendAsync("brodCastEndUserMessage", CustomerChat);
                }





                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
        }








    }

}


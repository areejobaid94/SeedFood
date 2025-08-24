using Infoseed.MessagingPortal.Sunshine.Infrastructure;
using Infoseed.MessagingPortal.Sunshine.Interfaces;
using Infoseed.MessagingPortal.Sunshine.Models;
using Infoseed.MessagingPortal.Sunshine.Models.Attachment;
using Infoseed.MessagingPortal.Sunshine.Models.Sunshine;
using Infoseed.MessagingPortal.Sunshine.SignalR;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamInboxController : ControllerBase
    {
        IDBService _dbService;
        private IHubContext<TeamInboxHub> _hub;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public TeamInboxController(IDBService dbService, IHostingEnvironment hostingEnvironment, IHubContext<TeamInboxHub> hub , IHttpClientFactory httpClientFactory)
        {
            _dbService = dbService;
            _hostingEnvironment = hostingEnvironment;
            _hub = hub;
            _httpClientFactory = httpClientFactory;

        }

          [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string fileUrl, string fileName)
        {
            if (string.IsNullOrEmpty(fileUrl) || string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File URL and File Name are required.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(fileUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound("File not found or unable to download.");
                }

                var fileBytes = await response.Content.ReadAsByteArrayAsync();

                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = fileName,
                    Inline = false 
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch
            {
                return StatusCode(500, "An error occurred while downloading the file.");
            }
        }

        [HttpPost("GetCustomers")]
        public async Task<IActionResult> GetCustomers(SearchCustomerModel model)
        {
            if (!string.IsNullOrEmpty(model.SearchTerm))
                model.SearchTerm = model.SearchTerm.ToLower();
            var lstCustomerModel = await _dbService.GetCustomersAsync(model.PageNumber, model.PageSize, model.SearchTerm);
            if (lstCustomerModel != null)
            {
                foreach (var item in lstCustomerModel)
                {
                    TimeSpan timeSpan = DateTime.Now - item.LastMessageData;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours <= 24)
                        item.IsOpen = true;
                    else
                        item.IsOpen = false;
                }
            }
            return Ok(lstCustomerModel);
        }

        [HttpGet("GetCustomersChat")]
        public async Task<IActionResult> GetCustomersChat(string userId, int pageNumber = 1, int pageSize = 20)
        {
            List<CustomerChat> lstCustomerChat = await _dbService.GetCustomersChat(userId, pageNumber, pageSize);

            return Ok(lstCustomerChat);
        }


        [HttpPost("UpdateCustomersChat")]
        public IActionResult UpdateCustomersChat(string userId, List<string> messageIds)
        {
            var task = _dbService.UpdateCustomersChat(userId, messageIds);

            return Ok();
        }

        [HttpPost("PostMessage")]
        public async Task<IActionResult> PostMessage(string userId, string text, string agentName, string agentId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(text))
            {
                return BadRequest();
            }
            Content message = new Content();
            message.text = text;
            message.agentName = agentName;
            message.agentId = agentId;
            message.type = "text";
            HttpStatusCode result = HttpStatusCode.OK;
            var customer = await _dbService.GetCustomer(userId);
            if (customer != null && customer.SunshineConversationId != null)
            {
                //Todo Make the connector as a Service and return status in this method
                result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.SunshineConversationId, message);

                if (result == HttpStatusCode.Created)
                {

                    _dbService.UpdateCustomerChat(message, userId, customer.SunshineConversationId);
                    CustomerChat CustomerChat = await _dbService.GetCustomersLastMessage(userId);
                    await _hub.Clients.All.SendAsync("brodCastAgentMessage", CustomerChat);
                }

            }
            else
            {
                result = HttpStatusCode.BadRequest;
            }

            return Ok(result);




        }

        [HttpPost("PostAttachment")]
        public async Task<IActionResult> PostAttachment([FromForm] TeamInboxAttachmentModel model)
        {
            if (model == null || model.UserID == null)
            {
                return BadRequest();
            }

            HttpStatusCode result = HttpStatusCode.OK;
            var customer = await _dbService.GetCustomer(model.UserID);
            if (customer != null && customer.SunshineConversationId != null)
            {
                if (model.FormFile != null && model.FormFile.Length > 0)
                {
                    var formFile = model.FormFile;
                    long ContentLength = formFile.Length;
                    byte[] fileData = null;
                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);
                        fileData = ms.ToArray();


                    }

                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                    AttachmentContent attachmentContent = new AttachmentContent()
                    {
                        Content = fileData,
                        Extension = Path.GetExtension(formFile.FileName),
                        MimeType = formFile.ContentType

                    };

                    var url = await azureBlobProvider.Save(attachmentContent);

                    var content = new Content()
                    {
                        type = model.Type,
                        text = model.Text,
                        mediaUrl = url,
                        altText = model.altText,
                        agentName = model.agentName,
                        agentId = model.agentId
                    };


                    result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.SunshineConversationId, content);

                    if (result == HttpStatusCode.Created)
                    {
                        _dbService.UpdateCustomerChat(content, model.UserID, customer.SunshineConversationId);

                    }
                }
                else
                {
                    result = HttpStatusCode.BadRequest;
                }
            }
            else
            {
                result = HttpStatusCode.BadRequest;
            }

            return Ok(result);




        }
        
        [HttpPost("PostLocation")]
        public async Task<IActionResult> PostLocation( LocaitionTeamInboxModel locationModel)
        {
            if ( locationModel!=null && string.IsNullOrEmpty(locationModel.userId))
            {
                return BadRequest();
            }
            Content message = new Content();
            message.coordinates = locationModel.coordinates;
            message.location = locationModel.location;
            message.type = SmoochContentTypeEnum.Location;
            HttpStatusCode result = HttpStatusCode.OK;
            var customer = await _dbService.GetCustomer(locationModel.userId);
            if (customer != null && customer.SunshineConversationId != null)
            {
                //Todo Make the connector as a Service and return status in this method
                result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.SunshineConversationId, message);

                if (result == HttpStatusCode.Created)
                {

                    _dbService.UpdateCustomerChat(message, locationModel.userId, customer.SunshineConversationId);

                }

            }
            else
            {
                result = HttpStatusCode.BadRequest;
            }

            return Ok(result);




        }

        [HttpGet("test")]
        public async Task<IActionResult> test()
        {

            Content content = new Content()
            {
                type = "file",
                text = "Hello",
                mediaUrl = "https://www.learningcontainer.com/download/sample-doc-file-for-testing/?wpdmdl=1576&refresh=5fc40877805cf1606682743",
                altText = "A wonderful image"
            };
            var result = await SunshineConnector.PostMsgToSmooch("5fbacfef3c6a0d000cc56283", "e9af89c5bc7bf13b50899fed", content);
            //await SunshineConnector.postImageMsgToSmooch("5fbacfef3c6a0d000cc56283", "e9af89c5bc7bf13b50899fed","Hello", "https://www.vectorico.com/wp-content/uploads/2018/02/Whatsapp-Icon-300x300.png");
            return Ok(Guid.NewGuid());
        }

        [HttpPost("LockedByAgent")]
        public async Task<IActionResult> LockedByAgent(string userId, string agentName)
        {
            var result = await _dbService.LockedAndUnlockedByAgent(userId, agentName, true);

            return Ok(result);
        }

        [HttpPost("UnlockedByAgent")]
        public async Task<IActionResult> UnlockedByAgent(string userId, string agentName)
        {
            var result = await _dbService.LockedAndUnlockedByAgent(userId, agentName, false);

            return Ok(result);
        }

        [HttpPost("AssignTo")]
        public async Task<IActionResult> AssignTo(string userId, string agentName)
        {
            var result = await _dbService.LockedAndUnlockedByAgent(userId, agentName, true);

            return Ok(result);
        }


        [HttpGet("GetChatStatus")]
        public async Task<IActionResult> GetChatStatus()
        {
            var result = _dbService.GetChatStatus();
            return Ok(result);
        }

        [HttpPost("UpdateCustomerInfo")]
        public async Task<IActionResult> UpdateCustomerInfo(UpdateCustomerModel model)
        {
            var result = await _dbService.UpdateCustomerInfo(model);

            return Ok(result);
        }





    }
}

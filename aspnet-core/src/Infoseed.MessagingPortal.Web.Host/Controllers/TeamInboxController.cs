using Abp;
using Abp.Authorization;
using Abp.Notifications;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using Framework.Data;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.ContactNotification;
using Infoseed.MessagingPortal.LiveChat;
using Infoseed.MessagingPortal.Notifications;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Wallet;
using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Models.TeamInboxModels;
using Infoseed.MessagingPortal.Web.Models.WhatsAppDialog;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.Web.WhatsAppDialog;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.ApplicationInsights;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;
using DocumentFormat.OpenXml.Wordprocessing;
using Abp.Collections.Extensions;
using System.Net.Http.Headers;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;
using System.Globalization;
using System.Net.Http;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using static Infoseed.MessagingPortal.FacebookDTO.DTO.PostFacebookMessageModel;
using Microsoft.Extensions.Logging;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using static Infoseed.MessagingPortal.Constants;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TeamInboxController : MessagingPortalControllerBase
    {
        private TelemetryClient _telemetry;
        private IContactNotification _contactNotification;
        private IUserNotificationManager _userNotificationManager;
        public Content message = new Content();
        private IHubContext<LiveChatHub> _LiveChatHubhub;
        private IAppNotifier _appNotifier;
        IDBService _dbService;
        //   private IHubContext<SignalR.TeamInboxHub> _hub;
        private ILiveChatAppService _iliveChat;
        private readonly IDocumentClient _IDocumentClient;
        private WhatsAppAppService _whatsAppAppService = new WhatsAppAppService();
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        private readonly IWalletAppService _walletAppService;
        private readonly IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        private readonly UserManager _userManager;

        private readonly IHttpClientFactory _httpClientFactory;
        // private string url = "https://startcampingstgnew.azurewebsites.net/api/startCampaign";
        //private string url = "https://startcampign.azurewebsites.net/api/startCampaign";
        public TeamInboxController(IHubContext<LiveChatHub> LiveChatHubhub, TelemetryClient telemetry, IDBService dbService,
            //IHubContext<SignalR.TeamInboxHub> hub,
            IAppNotifier appNotifier, IUserNotificationManager userNotificationManager, IContactNotification contactNotification, ILiveChatAppService iliveChat
            , IDocumentClient iDocumentClient
            , TenantDashboardAppService tenantDashboardAppService
            , IWalletAppService walletAppService
            , IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService
            , IHttpClientFactory httpClientFactory
            , UserManager userManager

            )
        {
            _telemetry = telemetry;
            _contactNotification = contactNotification;
            _userNotificationManager = userNotificationManager;
            _appNotifier = appNotifier;
            _dbService = dbService;
            //_hub = hub;
            _LiveChatHubhub = LiveChatHubhub;
            _iliveChat = iliveChat;
            _IDocumentClient = iDocumentClient;
            _tenantDashboardAppService = tenantDashboardAppService;
            _walletAppService = walletAppService;
            _whatsAppMessageTemplateAppService = whatsAppMessageTemplateAppService;
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
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


        [HttpPost("GetLastCustomers")]
        public async Task<IActionResult> GetLastCustomers(SearchCustomerModel model)
        {
            if (!string.IsNullOrEmpty(model.SearchTerm))
                model.SearchTerm = model.SearchTerm.ToLower();
            var lstCustomerModel = await _dbService.GetCustomersAsync(AbpSession.TenantId, model.PageNumber, model.PageSize, model.SearchTerm);
            if (lstCustomerModel != null)
            {
                foreach (var item in lstCustomerModel)
                {







                    TimeSpan timeSpan = DateTime.Now - item.LastMessageData.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                    if (totalHours <= 24)
                    {
                        item.IsConversationExpired = false;
                    }
                    else
                    {
                        item.IsConversationExpired = true;
                    }

                    CustomerChat CustomerChat = await _dbService.GetLastMessage(item.userId);

                    if (CustomerChat != null)
                    {
                        var text = "";
                        if (CustomerChat.text == null)
                        {
                            text = CustomerChat.fileName;
                        }
                        else
                        {
                            text = CustomerChat.text;
                        }
                        item.LastMessageText = text;
                    }



                }
            }




            return Ok(lstCustomerModel.FirstOrDefault());
        }

        [HttpPost("GetCustomers")]
        public async Task<List<CustomerModel>> GetCustomers(SearchCustomerModel model)
        {
            if (!string.IsNullOrEmpty(model.SearchTerm))
                model.SearchTerm = model.SearchTerm.ToLower();
            var lstCustomerModel = await _dbService.GetCustomersAsync(AbpSession.TenantId, model.PageNumber, model.PageSize, model.SearchTerm);

            return lstCustomerModel;
        }
        [HttpGet("CustomersGetAll")]
        public async Task<List<CustomerModel>> CustomersGetAllAsync(string searchTerm, int searchId = 0, int chatFilterID = 0, int pageNumber = 0, int pageSize = 10,int agentId=0,string userId="")
        {
            if (!string.IsNullOrEmpty(searchTerm))
                searchTerm = searchTerm.ToLower();
            var lstCustomerModel = await _dbService.CustomersGetAllAsync(AbpSession.TenantId, pageNumber, pageSize, searchTerm, searchId, chatFilterID, agentId, userId);


            return lstCustomerModel;
        }
        [HttpPost("IsValidContact")]
        public Dictionary<string, dynamic> IsValidContactAsync(string phoneNumber, string countryCode)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();
                if (AbpSession.TenantId.HasValue && !string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(countryCode))
                {
                    int tenantId = AbpSession.TenantId.Value;
                    string userId = tenantId + "_" + countryCode + phoneNumber;

                    var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
                    var Customer = customerResult.Result;
                    if (Customer == null)
                    {
                        bool valid = _dbService.IsValidContact(phoneNumber);
                        if (valid)
                        {
                            response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "The number not exists" } };
                        }
                        else
                        {
                            response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", "Format for number is not valid" } };
                        }
                    }
                    else
                    {
                        response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The number already exists" } };
                    }
                    return response;
                }
                // Handle the case when AbpSession.TenantId doesn't have a value
                return new Dictionary<string, dynamic> { { "state", 1 }, { "message", "Tenant ID missing." } };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }

        internal async Task<Dictionary<string, dynamic>> NewContactAsync(string phoneNumber, string contactName, bool isExternal)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();

                if (AbpSession.TenantId.HasValue)
                {
                    int tenantId = AbpSession.TenantId.Value;
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);

                    string userId = tenant.TenantId + "_" + phoneNumber;
                    var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
                    var Customer = customerResult.Result;

                    if (Customer == null || isExternal)
                    {
                        //type = text
                        ContactsTeamInboxs contactsTeamInboxs = new ContactsTeamInboxs();
                        var customerModel = _dbService.CreateNewCustomer(phoneNumber, contactName, "text", tenant.botId, tenantId, tenant.D360Key);

                        if (customerModel != null)
                        {
                            contactsTeamInboxs.contactId = int.Parse(customerModel.ContactID);
                            contactsTeamInboxs.displayName = customerModel.displayName;
                            contactsTeamInboxs.phoneNumber = customerModel.phoneNumber;
                            contactsTeamInboxs.userId = customerModel.userId;

                            if (customerModel.displayName.Length != 0)
                            {
                                contactsTeamInboxs.combinedValue = customerModel.phoneNumber + " (" + customerModel.displayName + ")";
                            }
                            else
                            {
                                contactsTeamInboxs.combinedValue = customerModel.phoneNumber;
                            }

                            response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactsTeamInboxs } };
                        }
                        else
                        {
                            // Handle the case when customerModel is null
                            response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "Contact Create failed." } };
                        }
                    }
                    else
                    {
                        response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The number already exists" } };
                    }

                    return response;
                }

                // Handle the case when AbpSession.TenantId doesn't have a value
                return new Dictionary<string, dynamic> { { "state", 4 }, { "message", "Tenant ID missing." } };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }

        [HttpGet("GetCustomersChat")]
        public async Task<IActionResult> GetCustomersChat(string userId, int pageNumber = 1, int pageSize = 20)
        {
            List<CustomerChat> lstCustomerChat = await _dbService.GetCustomersChat(userId, pageNumber, pageSize);

            lstCustomerChat.Reverse();

            List<CustomerChat> values = lstCustomerChat.ToList();


            return Ok(values);
        }



        [HttpGet("GetNoteChat")]
        public async Task<IActionResult> GetNoteChat(string userId, int pageNumber = 1, int pageSize = 20)
        {
            List<CustomerChat> lstCustomerChat = await _dbService.GetNoteChat(userId, pageNumber, pageSize);

            lstCustomerChat.Reverse();

            List<CustomerChat> values = lstCustomerChat.ToList();


            return Ok(values);
        }

        [HttpPost("PostMessageD360")]
        public async Task<IActionResult> PostMessageD360(PostMessageD360Model postMessage)
        {
            //postMessage.Type="note";



            if (string.IsNullOrEmpty(postMessage.To) || string.IsNullOrEmpty(postMessage.Text))
            {
                return BadRequest();
            }
            if (!IsBundleActive())
            {
                return BadRequest();
            }



            HttpStatusCode result = HttpStatusCode.OK;
            var userId = AbpSession.TenantId + "_" + postMessage.To;
            var customer = await _dbService.GetCustomer(userId);
            var tenant = await _dbService.GetTenantInfoById(AbpSession.TenantId.Value);

         
            _iliveChat.UpdateConversationsCount(userId);

            //if (postMessage.selectedLiveChatID!=0)
            //{
            //    _iliveChat.UpdateConversationsCount(userId);
            //}
            customer.ConversationsCount=0;

            if (!string.IsNullOrEmpty(tenant.AccessToken))
            {



                var postBody = "";
                if (customer.channel.ToLower()=="facebook")
                {

                    var textMessage = new
                    {
                        recipient = new { id = postMessage.To },
                        message = new { text = postMessage.Text },
                        messaging_type = "RESPONSE",
                    };

                    postBody = JsonConvert.SerializeObject(textMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    InstagramResult modelR = new InstagramResult();


                    if (postMessage.Type!="note")
                    {

                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.FacebookPageId, tenant.FacebookAccessToken, customer.channel);
                         modelR = JsonConvert.DeserializeObject<InstagramResult>(x);

                    }
                    else
                    {
                        FacebookResult Message3 = new FacebookResult() { message_id="" };


                        List<FacebookResult> messages = new List<FacebookResult>();

                        messages.Add(Message3);
                        modelR.message_id="";

                        _iliveChat.UpdateNote(tenant.TenantId.Value, customer.phoneNumber, true);
                    }



                    var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, postMessage.Text, postMessage.Type, tenant.TenantId.Value, 0, null, postMessage.AgentName, postMessage.AgentId, MessageSenderType.TeamInbox, modelR.message_id);
                    customer.customerChat = CustomerSendChat;
                }
                else if(customer.channel == "instagram")
                {
                    var textMessage = new
                    {
                        recipient = new { id = postMessage.To },
                        message = new { text = postMessage.Text },
                        messaging_type = "RESPONSE",
                    };

                    postBody = JsonConvert.SerializeObject(textMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


                    FacebookResult modelR = new FacebookResult();


                    if (postMessage.Type!="note")
                    {
                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.InstagramId, tenant.InstagramAccessToken, customer.channel);
                        modelR = JsonConvert.DeserializeObject<FacebookResult>(x);

                    }
                    else
                    {
                        FacebookResult Message3 = new FacebookResult() { message_id="" };


                        List<FacebookResult> messages = new List<FacebookResult>();

                        messages.Add(Message3);
                        modelR.message_id="";

                        _iliveChat.UpdateNote(tenant.TenantId.Value, customer.phoneNumber, true);
                    }


                    var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, postMessage.Text, postMessage.Type, tenant.TenantId.Value, 0, null, postMessage.AgentName, postMessage.AgentId, MessageSenderType.TeamInbox, modelR.message_id);
                    customer.customerChat = CustomerSendChat;
                }
                else
                {
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();

                    postWhatsAppMessageModel.to = postMessage.To;
                    postWhatsAppMessageModel.type = postMessage.Type;
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text
                    {
                        body = postMessage.Text
                    };


    
                    WhatsAppResult modelR = new WhatsAppResult();


                

                    if (postMessage.Type!="note")
                    {
                        postBody = _whatsAppAppService.PrepareTextMessage(postWhatsAppMessageModel);
                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.D360Key, tenant.AccessToken, customer.channel);
                        modelR = JsonConvert.DeserializeObject<WhatsAppResult>(x);
                       
                    }
                    else
                    {
                        WhatsAppResult.Message Message3 = new WhatsAppResult.Message() { id="" };


                        List<WhatsAppResult.Message> messages = new List<WhatsAppResult.Message>();

                        messages.Add(Message3);
                        modelR.messages=messages.ToArray();

                        _iliveChat.UpdateNote(tenant.TenantId.Value, customer.phoneNumber, true);
                    }
                  

                    var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, postMessage.Text, postMessage.Type, tenant.TenantId.Value, 0, null, postMessage.AgentName, postMessage.AgentId, MessageSenderType.TeamInbox, modelR.messages.FirstOrDefault().id);
                    customer.customerChat = CustomerSendChat;

                }

      

                SocketIOManager.SendChat(customer, AbpSession.TenantId.Value);

            }
            else if (customer != null)
            {

                if (!string.IsNullOrEmpty(customer.D360Key))
                {

                    SendWhatsAppD360Model masseges = new SendWhatsAppD360Model
                    {
                        to = postMessage.To,
                        type = "text",
                        text = new SendWhatsAppD360Model.Text
                        {
                            body = postMessage.Text
                        }
                    };

                    var content = new Content()
                    {
                        type = "text",
                        text = postMessage.Text,

                        agentName = postMessage.AgentName,
                        agentId = postMessage.AgentId
                    };

                    result = await WhatsAppDialogConnector.PostMsgToSmooch(customer.D360Key, masseges, _telemetry);
                    if (result == HttpStatusCode.Created)
                    {
                        var CustomerChat = _dbService.UpdateCustomerChatD360(AbpSession.TenantId, content, AbpSession.TenantId + "_" + postMessage.To, customer.ConversationId);
                        customer.customerChat = CustomerChat;
                        SocketIOManager.SendChat(customer, customer.TenantId.Value);

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


            }
            else
            {
                result = HttpStatusCode.BadRequest;

            }




            return Ok(result);
        }

        [RequestSizeLimit(104857600)]
        [HttpPost("PostD360Attachment")]
        public async Task<IActionResult> PostD360Attachment([FromForm] D360PostAttachmentModel model)
        {
            if (model == null || model.To == null)
            {
                return BadRequest();
            }
            if (!IsBundleActive())
            {
                return BadRequest();
            }
            HttpStatusCode result = HttpStatusCode.OK;
            var userId = AbpSession.TenantId + "_" + model.To;


            if (model.selectedLiveChatID!=0)
            {
                _iliveChat.UpdateConversationsCount(userId);
            }


            if (model.FormFile != null)
            {
                var types = AppsettingsModel.AttacmentTypesAllowed;
                var customer = await _dbService.GetCustomer(userId);
                var tenant = await _dbService.GetTenantInfoById(AbpSession.TenantId.Value);
                List<ListAttachmentModel> listAttachmentModels = new List<ListAttachmentModel>();

                customer.ConversationsCount=0;
                foreach (var item in model.FormFile)
                {
                    if (item.Length > 0)
                    {
                        var formFile = item;
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
                            MimeType = formFile.ContentType,

                        };

                        string filepath = System.IO.Path.GetDirectoryName(formFile.FileName);
                        var url = await azureBlobProvider.Save(attachmentContent);
                        var extention = Path.GetExtension(url);
                        var type = types[extention];

                        listAttachmentModels.Add(new ListAttachmentModel
                        {
                            typeContent = item.ContentType,
                            FilePath = url,
                            content = fileData,
                            fileName = formFile.FileName,
                            mediaUrl = url,
                            type = type
                        });


                    }


                }

                foreach (var item in listAttachmentModels)
                {
                    string postBody = string.Empty;
                    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();
                    SendWhatsAppD360Model masseges = new SendWhatsAppD360Model();
                    WhatsAppAppService whatsAppApiService = new WhatsAppAppService();

                    postWhatsAppMessageModel.to = model.To;
                    if (item.type.ToLower() == "image")
                    {

                        postWhatsAppMessageModel.image = new PostWhatsAppMessageModel.Image();
                        postWhatsAppMessageModel.image.link = item.mediaUrl;

                        if (customer.channel.ToLower()=="facebook"||customer.channel.ToLower()=="instagram")
                        {
                            var imageMessage = new
                            {
                                recipient = new { id = customer.userId.Split("_")[1] },
                                message = new
                                {
                                    attachment = new
                                    {
                                        type = "image",
                                        payload = new
                                        {
                                            url = item.mediaUrl,
                                            is_reusable = true
                                        }
                                    }
                                }
                            };
                            postBody = JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


                        }
                        else
                        {
                            postBody = whatsAppApiService.PrepareImageMessage(postWhatsAppMessageModel, true);
                          


                        }


                    }
                    else if (item.type.ToLower() == "video")
                    {
                        postWhatsAppMessageModel.video = new PostWhatsAppMessageModel.Video();
                        // postWhatsAppMessageModel.image.id = null;
                        postWhatsAppMessageModel.video.link = item.mediaUrl;
                        postWhatsAppMessageModel.video.caption = item.fileName;

                        if (customer.channel.ToLower()=="facebook"||customer.channel.ToLower()=="instagram")
                        {
                            var imageMessage = new
                            {
                                recipient = new { id = customer.userId.Split("_")[1] },
                                message = new
                                {
                                    attachment = new
                                    {
                                        type = "video",
                                        payload = new
                                        {
                                            url = item.mediaUrl,
                                            is_reusable = true
                                        }
                                    }
                                }
                            };
                            postBody = JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });





                        }
                        else
                        {
                            postBody = whatsAppApiService.PrepareVideoMessage(postWhatsAppMessageModel, true);
                          


                        }

                      

                    }
                    else if (item.type.ToLower() == "audio")
                    {
                        postWhatsAppMessageModel.audio = new PostWhatsAppMessageModel.Audio();
                        postWhatsAppMessageModel.audio.link = item.mediaUrl;


                        if (customer.channel.ToLower()=="facebook"||customer.channel.ToLower()=="instagram")
                        {
                            var imageMessage = new
                            {
                                recipient = new { id = customer.userId.Split("_")[1] },
                                message = new
                                {
                                    attachment = new
                                    {
                                        type = "audio",
                                        payload = new
                                        {
                                            url = item.mediaUrl,
                                            is_reusable = true
                                        }
                                    }
                                }
                            };
                            postBody = JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });





                        }
                        else
                        {
                            postBody = whatsAppApiService.PrepareVoiceMessage(postWhatsAppMessageModel, true);



                        }
                       

                    }
                    else
                    {
                        postWhatsAppMessageModel.document = new PostWhatsAppMessageModel.Document();
                        postWhatsAppMessageModel.document.filename = item.fileName;
                        postWhatsAppMessageModel.document.link = item.mediaUrl;



                        if (customer.channel.ToLower()=="facebook"||customer.channel.ToLower()=="instagram")
                        {


                            var imageMessage = new
                            {
                                recipient = new { id = customer.userId.Split("_")[1] },
                                message = new
                                {
                                    attachment = new
                                    {
                                        type = "document",
                                        payload = new
                                        {
                                            url = item.mediaUrl,
                                            is_reusable = true
                                        }
                                    }
                                }
                            };
                            postBody = JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


                        }
                        else
                        {

                            postBody = whatsAppApiService.PrepareDocumentMessage(postWhatsAppMessageModel, true);
                          

                        }



                       

                    }




                    var content = new Content()
                    {
                        type = item.type,
                        text = model.Text,
                        fileName = item.fileName,
                        mediaUrl = item.mediaUrl,
                        //altText = model.altText,
                        agentName = model.agentName,
                        agentId = model.agentId
                    };


                    if (!string.IsNullOrEmpty(tenant.AccessToken)&&customer.channel.ToLower()=="whatsapp")
                    {

                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.D360Key, tenant.AccessToken, customer.channel.ToLower());
                        var modelR = JsonConvert.DeserializeObject<WhatsAppResult>(x);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, model.Text, item.type, tenant.TenantId.Value, 0, null, model.agentName, model.agentId, MessageSenderType.TeamInbox, modelR.messages.FirstOrDefault().id, content);
                        customer.customerChat = CustomerSendChat;

                        SocketIOManager.SendChat(customer, AbpSession.TenantId.Value);

                    }


                    if (!string.IsNullOrEmpty(tenant.FacebookAccessToken)&&customer.channel.ToLower()=="facebook")
                    {
                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.FacebookPageId, tenant.FacebookAccessToken, customer.channel.ToLower());
                        var modelR = JsonConvert.DeserializeObject<FacebookResult>(x);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, model.Text, item.type, tenant.TenantId.Value, 0, null, model.agentName, model.agentId, MessageSenderType.TeamInbox, modelR.message_id, content);
                        customer.customerChat = CustomerSendChat;

                        SocketIOManager.SendChat(customer, AbpSession.TenantId.Value);

                    }


                    if (!string.IsNullOrEmpty(tenant.FacebookAccessToken)&&customer.channel.ToLower()=="instagram")
                    {
                        var x = await _whatsAppAppService.SendToWhatsAppNew(postBody, tenant.InstagramId, tenant.InstagramAccessToken, customer.channel.ToLower());
                        var modelR = JsonConvert.DeserializeObject<FacebookResult>(x);

                        var CustomerSendChat = _dbService.UpdateCustomerChat(customer, userId, model.Text, item.type, tenant.TenantId.Value, 0, null, model.agentName, model.agentId, MessageSenderType.TeamInbox, modelR.message_id, content);
                        customer.customerChat = CustomerSendChat;

                        SocketIOManager.SendChat(customer, AbpSession.TenantId.Value);

                    }


                }


                return Ok(result);

            }


            return BadRequest();
        }

        [HttpPost("GetImageURL")]
        public async Task<string> GetImageURL([FromForm] GetImageURLModel model)
        {
            var url = "";
            if (model.FormFile != null)
            {
                if (model.FormFile.Length > 0)
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
                        MimeType = formFile.ContentType,
                        fileName=formFile.FileName.Replace(Path.GetExtension(formFile.FileName), "")

                    };

                    url = await azureBlobProvider.Save(attachmentContent);

                }



            }

            return url;
        }

        [HttpPost("GetCustomersLastMessage")]
        public async Task<IActionResult> GetCustomersLastMessage(string contactId)
        {
            CustomerChat CustomerChat = await _dbService.GetCustomersLastMessage(contactId);
            return Ok(CustomerChat);
        }

        [HttpPost("PostLocation")]
        public async Task<IActionResult> PostLocation(LocaitionTeamInboxModel locationModel)
        {
            if (locationModel != null && string.IsNullOrEmpty(locationModel.userId))
            {
                return BadRequest();
            }
            Content message = new Content();
            message.coordinates = locationModel.coordinates;
            message.location = locationModel.location;
            message.type = SmoochContentTypeEnum.Location;
            HttpStatusCode result = HttpStatusCode.OK;
            var customer = await _dbService.GetCustomer(locationModel.userId);
            if (customer != null && customer.ConversationId != null)
            {
                //Todo Make the connector as a Service and return status in this method
                result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.ConversationId, message);

                if (result == HttpStatusCode.Created)
                {
                    _dbService.UpdateCustomerChat(AbpSession.TenantId, message, locationModel.userId, customer.ConversationId);
                }
            }
            else
            {
                result = HttpStatusCode.BadRequest;
            }

            return Ok(result);
        }


        // updateNote
        [HttpPost("UpdateNote")]
        public async Task<IActionResult> UpdateNote(string contactId, int agentId, string agentName ,bool IsNote=false)
        {

            var result = await _dbService.UpdateNoteByAgent(contactId, agentId, agentName, IsNote);


            return Ok();
        }

        // Open Chat
        [HttpPost("LockedByAgent")]
        public async Task<IActionResult> LockedByAgent(string contactId, int agentId, string agentName, int selectedLiveChatID = 0)
        {



            //if(selectedLiveChatID > 0 && AbpSession.TenantId==59)
            //{
            //    return Ok();


            //}
            var message = "Opened By :" + agentName;

            var result = await _dbService.OpenByAgent(contactId, agentId, agentName, true, message);




            if (result != null)
            {
                CustomerModel dsfsdfsdf = new CustomerModel();

                SocketIOManager.SendChat(result, result.TenantId.Value);
                var aaaa = JsonConvert.SerializeObject(result);
                if (selectedLiveChatID > 0)
                {

                    try
                    {

                        result.IsliveChat = false;
                        result.LiveChatStatus = 2;//open live chat
                        result.LiveChatStatusName = "Open";//open live chat
                        result.OpenTime = DateTime.Now.AddHours(AppSettingsModel.AddHour);
                        result.OpenTimeTicket = DateTime.Now.AddHours(AppSettingsModel.AddHour);
                        _iliveChat.UpdateIsOpenLiveChat(result.TenantId.Value, result.phoneNumber, true, 0, result.creation_timestamp, result.expiration_timestamp);
                        var x = _iliveChat.UpdateLiveChat(result.TenantId, result.phoneNumber, result.userId, result.displayName, result.LiveChatStatus, result.IsliveChat, agentId, agentName, selectedLiveChatID, true);
                        x.TenantId = result.TenantId.Value;
                        SocketIOManager.SendLiveChat(x, result.TenantId.Value);

                    }
                    catch
                    {

                    }

                }


                return Ok(result);

            }


            return Ok();
        }

        [HttpPost("UpdateCustomerStatus")]
        //Closed chat
        public async Task<IActionResult> UpdateCustomerStatus(string userId, string lockedByAgentName, bool IsOpen, int selectedLiveChatID = 0)
        {
            var message = "Closed By :" + lockedByAgentName;
            var result = await _dbService.UpdateCustomerStatus(userId, lockedByAgentName, IsOpen, message);
            if (result != null)
            {
                SocketIOManager.SendChat(result, result.TenantId.Value);


                // Audai.Jamaani  :   Hassn Snaid shloud  check it  due there is  an issue with the code 
                // Hasan snaid  :   it is ok no issue in this code (^____^) 

                //result.IsliveChat = false;
                //result.LiveChatStatus = 3;//close live chat
                //result.CloseTime = DateTime.Now.AddHours(AppSettingsModel.AddHour);
                //result.LiveChatStatusName = "Done";


                try
                {
                    _iliveChat.UpdateIsOpenLiveChat(result.TenantId.Value, result.phoneNumber, IsOpen, 0, result.creation_timestamp, result.expiration_timestamp);

                    //SocketIOManager.SendLiveChat(result, result.TenantId.Value);
                }
                catch
                {

                }

                try
                {
                    if (result.TenantId.Value==208)
                    {

                        var ten = await _dbService.GetTenantInfoById(result.TenantId.Value);

                        DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
                        var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(ten.D360Key, ten.DirectLineSecret, result.userId, ten.botId).Result;


                        var Bot = directLineConnector.StartBotConversationD360(result.userId, result.ContactID.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", ten.DirectLineSecret, ten.botId, result.phoneNumber, result.TenantId.Value.ToString(), result.displayName, ten.PhoneNumber, ten.EvaluationText.ToString(), micosoftConversationID.watermark, null, ten.BotTemplateId, "").Result;
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


                            List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, result.phoneNumber, ten.botId);

                            foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                            {
                                var result222 = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, ten.D360Key, ten.AccessToken, ten.IsD360Dialog);
                                if (result222)
                                {

                                    //var message = PrepareMessageContent(msgBot, Tenant.botId);
                                    //var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);

                                    WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, ten.botId, result.userId, result.TenantId.Value, result.ConversationId);
                                    var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);

                                    result.customerChat = CustomerChat;
                                    //await _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                                    SocketIOManager.SendChat(result, result.TenantId.Value);
                                }
                            }

                        }
                    }
                }
                catch
                {

                }






                return Ok(result);

            }


            return Ok();
        }

        [HttpPost("AssignTo")]
        public async Task<IActionResult> AssignTo(string contactId, int agentId, string agentName)
        {
            var userId = AbpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);

            var message = user.FullName + " Assign To :" + agentName +","+ contactId;
            UserNotification Notification = await SendNotfAsync(message, agentId);

            await _contactNotification.CreateContactNotificationAsync(AbpSession.TenantId, contactId, Notification.Notification.Id.ToString(), Notification.Notification.CreationTime.AddHours(AppSettingsModel.AddHour), message, Convert.ToInt32(AbpSession.UserId));
            var result = await _dbService.AssignTo(contactId, agentId, agentName, true, Notification);


            if (result != null)
            {
                // await _hub.Clients.All.SendAsync("brodCastAgentMessage", result);
                SocketIOManager.SendChat(result, result.TenantId.Value);

                return Ok(result);

            }


            return Ok();
        }

        [HttpPost("BlockCustomer")]
        public async Task<IActionResult> BlockCustomer(string contactId, int agentId, string agentName, bool isBlock)
        {
            var message = "Blocked By :" + agentName;
            var result = await _dbService.BlockCustomer(contactId, agentId, agentName, isBlock);
            return Ok(result);
        }

        [HttpPost("UnlockedByAgent")]
        public async Task<IActionResult> UnlockedByAgent(string contactId, int agentId, string agentName)
        {
            var message = "Open By  :" + agentName;
            UserNotification Notification = await SendNotfAsync(message, agentId);
            await _contactNotification.CreateContactNotificationAsync(AbpSession.TenantId, contactId, Notification.Notification.Id.ToString(), Notification.Notification.CreationTime.AddHours(2), message, Convert.ToInt32(AbpSession.UserId));
            var result = await _dbService.LockedAndUnlockedByAgent(contactId, agentId, agentName, false, Notification);

            if (result != null)
            {
                // await _hub.Clients.All.SendAsync("brodCastAgentMessage", result);
                SocketIOManager.SendChat(result, (int)AbpSession.TenantId);


                return Ok(result);

            }


            return Ok();
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

        [Route("UpdateComplaint")]
        [HttpGet]
        public async Task<CustomerModel> UpdateComplaintAsync(string contactId, bool isComplaint, string username)
        {

            var result = await _dbService.UpdateComplaint(contactId, 0, isComplaint, username);
            if (result != null)
            {

                // await _hub.Clients.All.SendAsync("brodCastAgentMessage", result);
                SocketIOManager.SendChat(result, result.TenantId.Value);
            }


            return result;

        }

        #region public contact in template
        [HttpPost("NickNameUpdate")]
        public async Task<Dictionary<string, dynamic>> NickNameUpdateAsync(int contactID, string nickName)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();

                int tenantId = AbpSession.TenantId.Value;
                var result = await _dbService.NickNameUpdateAsync(tenantId, contactID, nickName);
                if (result != 0)
                {
                    response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactID } };
                }
                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        [HttpGet("ContactsGetAll")]
        public Dictionary<string, dynamic> ContactsGetAll(string searchTerm = null, int pageNumber = 0, int pageSize = 20)
        {
            try
            {
                List<ContactsTeamInboxs> contactsTeamInboxes = new List<ContactsTeamInboxs>();
                var response = new Dictionary<string, dynamic>();

                if (!string.IsNullOrEmpty(searchTerm))
                    searchTerm = searchTerm.ToLower();

                int tenantId = AbpSession.TenantId.Value;

                contactsTeamInboxes =  _dbService.ContactsTeamInbox(tenantId, pageNumber, pageSize, searchTerm);
                if (contactsTeamInboxes != null)
                {
                    response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", contactsTeamInboxes } };
                }
                else
                {
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "Contacts Get failed." } };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        #endregion

        #region public Api For Campign
        /// <summary>
        /// for send Campign
        /// </summary>
        /// <param name="teamInboxDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendCampign")]
        public async Task<Dictionary<string, dynamic>> SendCampignAsync(TeamInboxDto teamInboxDto)
        {
            try
            {
                int tenantId = AbpSession.TenantId.Value;
                string userId = tenantId + "_" + teamInboxDto.phoneNumber;

                var CustomerCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var customerResult = CustomerCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == tenantId);
                var Customer = customerResult.Result;
                if (Customer == null)
                {
                    teamInboxDto.isExternal=true;
                    teamInboxDto.CustomerOPT=0;
                }



                return await sendCampignAsync(teamInboxDto);
            }
            catch (Exception ex)
            {
                var response = new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
                return response;
            }
        }
        #endregion

        #region private Api For Campign
        /// <summary>
        /// for send Campign
        /// </summary>
        /// <param name="teamInboxDto"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, dynamic>> sendCampignAsync(TeamInboxDto teamInboxDto)
        {
            try
            {
                var response = new Dictionary<string, dynamic>();
                int tenantId = AbpSession.TenantId.Value;
                var NewContact = new Dictionary<string, dynamic>();
                #region Create Contact
                if (teamInboxDto.isExternal)
                {
                    NewContact = await NewContactAsync(teamInboxDto.phoneNumber, teamInboxDto.contactName, teamInboxDto.isExternal);
                    int Values = NewContact.Values.FirstOrDefault();
                    var messageError = NewContact.Values.LastOrDefault();
                    switch (Values)
                    {
                        case -1:
                            return new Dictionary<string, dynamic> { { "state", -1 }, { "message", messageError } };
                        case 1:
                            return new Dictionary<string, dynamic> { { "state", 6 }, { "message", messageError } };
                        case 2:
                            break;
                        case 3:
                            return new Dictionary<string, dynamic> { { "state", 7 }, { "message", messageError } };
                        case 4:
                            return new Dictionary<string, dynamic> { { "state", 8 }, { "message", messageError } };
                    }
                }
                else if (teamInboxDto.CustomerOPT == 1)
                {
                    return new Dictionary<string, dynamic> { { "state", 10 }, { "message", "The Contact is Opt Out" } };
                }
                #endregion

                #region ChecDailyLimit
                int DailyLimit = DailyLimitGet();
                if (DailyLimit == 0)
                {
                    response = new Dictionary<string, dynamic> { { "state", 5 }, { "message", "You have exceeded your daily limit" } };
                    return response;
                }
                #endregion

                #region Wallet Information
                var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                if (walletModel == null)
                {
                    _walletAppService.CreateWallet(tenantId);
                    walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                    return response;
                }
                else if (walletModel.TotalAmount <= 0)
                {
                    response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                    return response;
                }
                #endregion

                #region For check if the tenant is have an active campaign or not
                //bool status = SendCampaignCheck(tenantId);
                //if (status == false)
                //{
                //    response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The tenant is have an active campaign" } };
                //    return response;
                //}
                #endregion

                #region create name for new campign And Check if 
                long campaignId = 0;
                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();
                DateTime dateTime = DateTime.UtcNow;
                TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                whatsAppCampaignModel.Title = teamInboxDto.contactName + (long)timeSpan.TotalSeconds; ;
                whatsAppCampaignModel.Language = teamInboxDto.language;
                whatsAppCampaignModel.TemplateId = teamInboxDto.templateId;
                whatsAppCampaignModel.Type = 2;

                campaignId = _whatsAppMessageTemplateAppService.AddWhatsAppCampaign(whatsAppCampaignModel);
                if (campaignId == 0)
                {
                    response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", "The tenant is have an active campaign" } };
                    return response;
                }

                #region statistic Campaign
                WhatsAppCampaignModel statistics = new WhatsAppCampaignModel
                {
                    Id = campaignId,
                    SentTime = DateTime.UtcNow,
                    Status = (int)WhatsAppCampaignStatusEnum.Draft
                };
                updateWhatsAppCampaign(statistics);
                #endregion
                #endregion

                #region Add as external contact 
                //long statusId = await addBulkAsExternalContact(teamInboxDto, campaignId);
                #endregion

                #region check if you hava Funds gretar than or equal convarsaction prise 
                bool isFailed = true;
                double Price = 0.014;
                if (walletModel.TotalAmount > 0)
                {
                    var category = GetTemplatesCategory(teamInboxDto.templateId);
                    //UTILITY MARKETING
                    if (category == "MARKETING" || category == "UTILITY"|| category == "AUTHENTICATION")
                    {
                        if (walletModel.TotalAmount >= (decimal)0.014)
                        {
                            walletModel.TotalAmount = walletModel.TotalAmount - (decimal)0.014;
                            isFailed = false;
                        }
                        else
                        {
                            response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                        }
                    }
                }

                if (!isFailed)
                {
                    var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == tenantId + "_" + teamInboxDto.phoneNumber);
                    var customer = new CustomerModel();
                    int resultContactID = 0;
                    try
                    {
                        customer = customerResult.Result;
                        if (!int.TryParse(customer.ContactID, out resultContactID))
                        {
                            return new Dictionary<string, dynamic> { { "state", -1 }, { "message", "An error occurred while Sending template" } };
                        }
                    }
                    catch
                    {
                        return new Dictionary<string, dynamic> { { "state", -1 }, { "message", "An error occurred while Sending template" } };
                    }






                    WhatsAppContactsDto contacts = new WhatsAppContactsDto();
                    List<ListContactToCampin> OuterList = new List<ListContactToCampin>();
                    if (teamInboxDto.isExternal)
                    {
                        OuterList.Add(new ListContactToCampin
                        {
                            Id = resultContactID,
                            templateVariables = teamInboxDto.templateVariables,
                            haderVariablesTemplate = teamInboxDto.headerVariabllesTemplate,
                            firstButtonURLVariabllesTemplate = teamInboxDto.firstButtonURLVariabllesTemplate,
                            secondButtonURLVariabllesTemplate = teamInboxDto.secondButtonURLVariabllesTemplate,
                            carouselVariabllesTemplate = teamInboxDto.CarouselTemplate,
                            buttonCopyCodeVariabllesTemplate = teamInboxDto.buttonCopyCodeVariabllesTemplate,

                            ContactName = teamInboxDto.contactName,
                            PhoneNumber = teamInboxDto.phoneNumber,
                            CustomerOPT = 0
                        });
                    }
                    else
                    {
                        OuterList.Add(new ListContactToCampin
                        {
                            Id = resultContactID,
                            templateVariables = teamInboxDto.templateVariables,
                            haderVariablesTemplate = teamInboxDto.headerVariabllesTemplate,
                            firstButtonURLVariabllesTemplate = teamInboxDto.firstButtonURLVariabllesTemplate,
                            secondButtonURLVariabllesTemplate = teamInboxDto.secondButtonURLVariabllesTemplate,
                            carouselVariabllesTemplate = teamInboxDto.CarouselTemplate,
                            buttonCopyCodeVariabllesTemplate = teamInboxDto.buttonCopyCodeVariabllesTemplate,
                            ContactName = teamInboxDto.contactName,
                            PhoneNumber = teamInboxDto.phoneNumber,
                            CustomerOPT = teamInboxDto.CustomerOPT
                        });
                    }
                    long returnValue = 0;
                    var tenantInfo = GetTenantInfo(tenantId);
                    switch (teamInboxDto.campaignStatus)
                    {
                        //1 >> send now ,,, 2 >> send Scheduled
                        case 1:
                            {
                                // Display the count of each split list
                                string sendcompaing = "campaign1";

                                var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignAddOnDB;

                                MessageTemplateModel objWhatsAppTemplateModel = _whatsAppMessageTemplateAppService.GetTemplateById(teamInboxDto.templateId);
                                MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                                if (templateWA != null && templateWA.status == "APPROVED")
                                {
                                    // objWhatsAppTemplateModel.components = templateWA.components;
                                   //  objWhatsAppTemplateModel.components = objWhatsAppTemplateModel.components;

                                    string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);

                                    CampinQueueNew campinQueueNew = new CampinQueueNew();



                                    TemplateVariablles templateVariables = null;
                                    HeaderVariablesTemplate headerVariabllesTemplate = null;
                                    FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate = null;
                                    SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate = null;
                                    CarouselVariabllesTemplate carouselVariabllesTemplate = null;
                                    ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate = null;
                                    if (teamInboxDto.templateVariables!=null)
                                    {
                                         templateVariables = teamInboxDto.templateVariables;
                                    }
                                    if (teamInboxDto.headerVariabllesTemplate != null)
                                    {
                                        headerVariabllesTemplate = teamInboxDto.headerVariabllesTemplate;
                                    }
                                    if (teamInboxDto.firstButtonURLVariabllesTemplate != null)
                                    {
                                        firstButtonURLVariabllesTemplate = teamInboxDto.firstButtonURLVariabllesTemplate;
                                    }
                                    if (teamInboxDto.secondButtonURLVariabllesTemplate != null)
                                    {
                                        secondButtonURLVariabllesTemplate = teamInboxDto.secondButtonURLVariabllesTemplate;
                                    }
                                    if (teamInboxDto.CarouselTemplate != null)
                                    {
                                        carouselVariabllesTemplate = teamInboxDto.CarouselTemplate;
                                    }
                                    if (teamInboxDto.buttonCopyCodeVariabllesTemplate != null)
                                    {
                                        buttonCopyCodeVariabllesTemplate = teamInboxDto.buttonCopyCodeVariabllesTemplate;
                                    }
                                    string str = JsonConvert.SerializeObject(OuterList);

                                    string templateVariablesjson = JsonConvert.SerializeObject(headerVariabllesTemplate);

                                    string headerVarjson = JsonConvert.SerializeObject(headerVariabllesTemplate);

                                    string firstURLVariabllesjson = JsonConvert.SerializeObject(firstButtonURLVariabllesTemplate);

                                    string secURLVariabllesjson = JsonConvert.SerializeObject(secondButtonURLVariabllesTemplate);
                                    string carouselVariabllesjson = JsonConvert.SerializeObject(carouselVariabllesTemplate);
                                    string copyCodeVariabllesjson = JsonConvert.SerializeObject(buttonCopyCodeVariabllesTemplate);

                                    string TemplateJson = JsonConvert.SerializeObject(objWhatsAppTemplateModel);
                                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                                         new System.Data.SqlClient.SqlParameter("@Contacts",str)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateId",teamInboxDto.templateId)
                                        ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",teamInboxDto.isExternal)
                                        ,new System.Data.SqlClient.SqlParameter("@JopName",sendcompaing)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateName",objWhatsAppTemplateModel.name)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignName",whatsAppCampaignModel.Title)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateJson",TemplateJson)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesJson",templateVariablesjson)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesHeaderJson",headerVarjson)
                                         , new System.Data.SqlClient.SqlParameter("@URLButton1VariablesTemplate", firstURLVariabllesjson)
                                         ,new System.Data.SqlClient.SqlParameter("@URLButton2VariablesTemplate", secURLVariabllesjson)
                                         ,new System.Data.SqlClient.SqlParameter("@carouselVariabllesTemplatejson", carouselVariabllesjson)
                                    };
                                    var OutputParameter = new System.Data.SqlClient.SqlParameter
                                    {
                                        SqlDbType = SqlDbType.BigInt,
                                        ParameterName = "@Id",
                                        Direction = ParameterDirection.Output
                                    };
                                    sqlParameters.Add(OutputParameter);

                                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                                    if (OutputParameter.Value != DBNull.Value)
                                    {
                                        campinQueueNew.messageTemplateModel = objWhatsAppTemplateModel;
                                        campinQueueNew.campaignId = campaignId;
                                        campinQueueNew.templateId = teamInboxDto.templateId;
                                        campinQueueNew.IsExternal = teamInboxDto.isExternal;
                                        campinQueueNew.TenantId = tenantInfo.TenantId;
                                        campinQueueNew.D360Key = tenantInfo.D360Key;
                                        campinQueueNew.AccessToken = tenantInfo.AccessToken;
                                        campinQueueNew.functionName = sendcompaing;
                                        campinQueueNew.msg = msg;
                                        campinQueueNew.type = type;
                                        campinQueueNew.contacts = null;
                                        campinQueueNew.templateVariables = null;
                                        campinQueueNew.headerVariabllesTemplate = null;
                                        campinQueueNew.campaignName = whatsAppCampaignModel.Title;
                                        campinQueueNew.rowId = Convert.ToInt64(OutputParameter.Value);
                                        // SetCampinQueueContact(campinQueueNew);
                                        SetCampinInFun(campinQueueNew);
                                    }
                                    else
                                    {
                                        return new Dictionary<string, dynamic> { { "state", -1 }, { "message", "An error occurred while Sending template" } };
                                    }

                                    #region Add in transaction table 

                                    TransactionModel transactionModel = new TransactionModel();

                                    var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                                    transactionModel.DoneBy = usersDashModel.Name;
                                    transactionModel.TotalTransaction = (decimal)Price;
                                    transactionModel.TotalRemaining = walletModel.TotalAmount;
                                    transactionModel.TransactionDate = DateTime.UtcNow;
                                    transactionModel.CategoryType = objWhatsAppTemplateModel.category;
                                    transactionModel.TenantId = tenantInfo.TenantId;

                                    var result = addTransaction(transactionModel, OuterList.Count, objWhatsAppTemplateModel.name, campaignId);
                                    #endregion

                                    #region statistic Campaign
                                    statistics = new WhatsAppCampaignModel
                                    {
                                        Id = campaignId,
                                        SentTime = DateTime.UtcNow,
                                        Status = (int)WhatsAppCampaignStatusEnum.Active
                                    };
                                    updateWhatsAppCampaign(statistics);
                                    #endregion

                                    response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Sent Successfully" } };
                                }
                                else
                                {
                                    response = new Dictionary<string, dynamic> { { "state", 9 }, { "message", "template Not APPROVED" } };
                                }

                                return response;
                            }
                        case 2:
                            {
                                MessageTemplateModel objWhatsAppTemplateModel = _whatsAppMessageTemplateAppService.GetTemplateById(teamInboxDto.templateId);
                                MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                                if (templateWA != null && templateWA.status == "APPROVED")
                                {
                                    objWhatsAppTemplateModel.components = templateWA.components;
                                    var res = sendCampaignScheduledTeamInpox(campaignId, teamInboxDto.templateId, whatsAppCampaignModel.Title, objWhatsAppTemplateModel.name, teamInboxDto.sendTime, teamInboxDto.isExternal, OuterList);
                                    if (!res.status)
                                    {
                                        long campinId = ChangeCampaignActive(tenantId, campaignId);

                                        response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", res.Message } };
                                        return response;
                                    }

                                    #region statistic Campaign
                                    DateTime dateTimes;
                                    if (DateTime.TryParseExact(teamInboxDto.sendTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimes))
                                    {
                                        dateTimes = dateTimes.AddHours(AppSettingsModel.DivHour);
                                    }

                                    statistics = new WhatsAppCampaignModel
                                    {
                                        Id = campaignId,
                                        SentTime = dateTimes,
                                        Status = (int)WhatsAppCampaignStatusEnum.Scheduled
                                    };
                                    updateWhatsAppCampaign(statistics);
                                    #endregion

                                    return response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", "Sent Successfully" } };
                                    //returnValue = _whatsAppMessageTemplateAppService.AddScheduledCampaign(contacts, teamInboxDto.sendTime, campaignId, teamInboxDto.templateId, true);
                                }
                                break;
                            }
                    }

                    if (returnValue > 0 || returnValue == -10) { }
                    else
                    {
                        response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", "You don't have enough Funds" } };
                    }
                }
                #endregion
                return response;
            }
            catch (Exception ex)
            {
                var response = new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
                return response;
            }
        }
        private SendCampinStatesModel sendCampaignScheduledTeamInpox(long campaignId, long templateId, string campaignNmae, string templateName, string sendTime, bool isExternal, List<ListContactToCampin> OuterList)
        {
            try
            {
                #region ChecDailyLimit
                int DailyLimit = DailyLimitGet();
                if (DailyLimit == 0)
                {
                    return new SendCampinStatesModel()
                    {
                        Message = "You have exceeded your daily limit",
                        status = false
                    };
                }
                #endregion

                #region Send Campign
                int tenantId = AbpSession.TenantId.Value;

                #region Wallet Information
                var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);

                if (walletModel == null)
                {
                    _walletAppService.CreateWallet(tenantId);
                    walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                    return new SendCampinStatesModel()
                    {
                        Message = "You don't have enough Funds",
                        status = false
                    };
                }
                else if (walletModel.TotalAmount <= 0)
                {
                    return new SendCampinStatesModel()
                    {
                        Message = "You don't have enough Funds",
                        status = false
                    };
                }
                #endregion

                #region check if you hava Funds gretar than or equal convarsaction prise 
                double Price = 0.014;
                decimal totalPrice = 0;

                totalPrice = 1 * (decimal)Price;

                //UTILITY MARKETING
                if (walletModel.TotalAmount >= totalPrice)
                {
                    var tenantInfo = GetTenantInfo(tenantId);

                    var category = GetTemplatesCategory(templateId);

                    long returnValue = addScheduledCampaignonOnDB(campaignId, templateId, campaignNmae, templateName, sendTime, isExternal, OuterList);
                    if (returnValue > 0)
                    {
                        #region Add in transaction table 
                        TransactionModel transactionModel = new TransactionModel();

                        var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                        transactionModel.DoneBy = usersDashModel.Name;
                        transactionModel.TotalTransaction = totalPrice;
                        transactionModel.TotalRemaining = walletModel.TotalAmount - totalPrice;
                        transactionModel.TransactionDate = DateTime.UtcNow;
                        transactionModel.CategoryType = category;
                        transactionModel.TenantId = tenantId;

                        var result = addTransaction(transactionModel, 1, templateName, campaignId);
                        #endregion

                        return new SendCampinStatesModel()
                        {
                            Message = "Sent Successfully",
                            status = true
                        };
                    }
                    else
                    {
                        return new SendCampinStatesModel()
                        {
                            Message = "Date string is InValid",
                            status = true
                        };
                    }
                }
                else
                {
                    return new SendCampinStatesModel()
                    {
                        Message = "You don't have enough Funds",
                        status = false
                    };
                }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                return new SendCampinStatesModel()
                {
                    Message = ex.Message,
                    status = false
                };
            }
        }
        private long addScheduledCampaignonOnDB(long campaignId, long templateId, string campaignNmae, string templateName, string sendTime, bool isExternal, List<ListContactToCampin> OuterList)
        {
            try
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(sendTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    // Date string is valid
                    dateTime = dateTime.AddHours(AppSettingsModel.DivHour);
                }
                else
                { return 0; }

                // Display the count of each split list
                string JopName = "SendScheduledCampaign1";

                var SP_Name = Constants.WhatsAppCampaign.SP_ScheduledCampaignAddOnDB;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Contacts",JsonConvert.SerializeObject(OuterList))
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",templateId)
                    ,new System.Data.SqlClient.SqlParameter("@SendTime",dateTime)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",isExternal)
                    ,new System.Data.SqlClient.SqlParameter("@JopName",JopName)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateName",templateName)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignName",campaignNmae)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Id",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


                if (OutputParameter.Value != DBNull.Value)
                {
                    WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
                    {
                        Id = campaignId,
                        SentTime = dateTime,
                        Status = (int)WhatsAppCampaignStatusEnum.Scheduled
                    };
                    updateWhatsAppCampaign(whatsAppCampaignModel);
                }
                else
                { return 0; }


                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.Id)
                    ,new System.Data.SqlClient.SqlParameter("@SentTime",whatsAppCampaignModel.SentTime)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.Status)
                    ,new System.Data.SqlClient.SqlParameter("@SentCampaignId",Guid.NewGuid())
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long addTransaction(TransactionModel model, int totalCount, string TemplateName, long campaignId)
        {
            try
            {
                var SP_Name = Constants.Transaction.SP_CampaignMinusPrice;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@DoneBy",model.DoneBy)
                    ,new System.Data.SqlClient.SqlParameter("@TotalTransaction",model.TotalTransaction)
                    ,new System.Data.SqlClient.SqlParameter("@TotalRemaining",model.TotalRemaining)
                    ,new System.Data.SqlClient.SqlParameter("@TransactionDate",model.TransactionDate)
                    ,new System.Data.SqlClient.SqlParameter("@CategoryType",model.CategoryType)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",model.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@totalCount",totalCount)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateName",TemplateName)
                    ,new System.Data.SqlClient.SqlParameter("@campaignId",campaignId)
                  };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Output",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SetCampinInFun(CampinQueueNew obj)
        {


            //CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

            try
            {
                List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);

                if (campaigns.Count()>0)
                {

                    WhatsAppCampaignModel whatsAppCampaignModel2 = new WhatsAppCampaignModel
                    {
                        Id = obj.campaignId,
                        SentTime = DateTime.UtcNow,
                        Status = 3 // as sent
                    };
                    updateWhatsAppCampaign2(whatsAppCampaignModel2, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());





                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.urlSendCampaignProject);
                    var content = new StringContent("{\n    \"campaignId\": "+obj.campaignId+"\n}", null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
                        //{
                        //    id = obj.campaignId,
                        //    sentTime = DateTime.UtcNow,
                        //    status = 3 // as sent
                        //};
                        //updateWhatsAppCampaign(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());
                    }
                    else
                    {
                        WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
                        {
                            Id = obj.campaignId,
                            SentTime = DateTime.UtcNow,
                            Status = 4 // as sent
                        };
                        updateWhatsAppCampaign2(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());

                    }


                }









            }
            catch (Exception ex)
            {


            }
        }

        private static List<SendCampaignNow> GetCampaign(long rowId)
        {
            try
            {
                var SP_Name = "[dbo].[SendCampaignGetFromDB]";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };
                List<SendCampaignNow> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, AppSettingsModel.ConnectionStrings).ToList();
                return model;
            }
            catch
            {
                return new List<SendCampaignNow>();
            }
        }
        public static SendCampaignNow MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                SendCampaignNow model = new SendCampaignNow
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent")
                };

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new SendCampaignNow();
            }
        }
        private static void updateWhatsAppCampaign2(WhatsAppCampaignModel whatsAppCampaignModel, int TenantId, int count)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.Id)
                    ,new System.Data.SqlClient.SqlParameter("@SentTime",whatsAppCampaignModel.SentTime)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.Status)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Count",count)
                    ,new System.Data.SqlClient.SqlParameter("@SentCampaignId",Guid.NewGuid())
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetCampinQueueContact(CampinQueueNew campinQueueNew)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference(campinQueueNew.functionName);
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           campinQueueNew
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(campinQueueNew);
            }
        }
        private string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {
                            type = item.format.ToLower();
                        }
                        if (item.type.Equals("BUTTONS"))
                        {
                            for (int i = 0; i < item.buttons.Count; i++)
                            {
                                result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
                            }
                        }
                        result += item.text;

                    }

                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static async Task<MessageTemplateModel> getTemplateByWhatsId(TenantInfoDto tenant, string templateId)
        {
            using (var httpClient = new HttpClient())
            {
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + templateId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        try
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var WhatsAppTemplate = await content.ReadAsStringAsync();
                                MessageTemplateModel objWhatsAppTemplate = new MessageTemplateModel();
                                objWhatsAppTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(WhatsAppTemplate);
                                return objWhatsAppTemplate;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }
        private TenantInfoDto GetTenantInfo(int tenantId)
        {
            TenantInfoDto tenantInfoDto = new TenantInfoDto();
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantInfoGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                tenantInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapGetTenantInfo, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return tenantInfoDto;
            }
            catch
            {
                return tenantInfoDto;
            }
        }
        /// <summary>
        /// get Daily Limit For same tenant
        /// </summary>
        /// <returns></returns>
        private int DailyLimitGet()
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_DailylimitGetCount;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@OutputParameter",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// For check if the tenant is have an active campaign or not
        /// </summary>
        /// <returns>true if no have activecampaign ,, false if have activecampaign in same tenant</returns>
        private bool SendCampaignCheck(int tenantId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppSendCampaignValidationGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                bool result = (bool)OutputParameter.Value;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// get Category type for Templates
        /// </summary>
        /// <param name="templatesId">templates Id</param>
        /// <returns>return category type fpr template </returns>
        private string GetTemplatesCategory(long templatesId)
        {
            try
            {
                string category = "";
                var SP_Name = Constants.WhatsAppTemplates.SP_GetTemplatesCategory;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@templatesId",templatesId)
                };

                category = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplatesCategory, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return category;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// for get a Coutry Code 
        /// </summary>
        /// <param name="phone">Phone Number</param>
        /// <returns></returns>
        private CoutryTelCodeModel getCoutryISO(string phone)
        {
            List<CoutryTelCodeModel> TelCodes = new List<CoutryTelCodeModel>
                   {
                       new CoutryTelCodeModel("93", "AF",1),
                       new CoutryTelCodeModel("355", "AL",1),
                       new CoutryTelCodeModel("213", "DZ",2),
                       new CoutryTelCodeModel("1-684", "AS",1),
                       new CoutryTelCodeModel("376", "AD",1),
                       new CoutryTelCodeModel("244", "AO",1),
                       new CoutryTelCodeModel("1-264", "AI",1),
                       new CoutryTelCodeModel("672", "AQ",1),
                       new CoutryTelCodeModel("1-268", "AG",1),
                       new CoutryTelCodeModel("54", "AR",1),
                       new CoutryTelCodeModel("374", "AM",1),
                       new CoutryTelCodeModel("297", "AW",1),
                       new CoutryTelCodeModel("61", "AU",1),
                       new CoutryTelCodeModel("43", "AT",(decimal)1.5),
                       new CoutryTelCodeModel("994", "AZ",1),
                       new CoutryTelCodeModel("1-242", "BS",1),
                       new CoutryTelCodeModel("973", "BH",1),
                       new CoutryTelCodeModel("880", "BD",1),
                       new CoutryTelCodeModel("1-246", "BB",1),
                       new CoutryTelCodeModel("375", "BY",(decimal)1.5),
                       new CoutryTelCodeModel("32", "BE",2),
                       new CoutryTelCodeModel("501", "BZ",1),
                       new CoutryTelCodeModel("229", "BJ",1),
                       new CoutryTelCodeModel("1-441", "BM",1),
                       new CoutryTelCodeModel("975", "BT",1),
                       new CoutryTelCodeModel("591", "BO",1),
                       new CoutryTelCodeModel("387", "BA",1),
                       new CoutryTelCodeModel("267", "BW",1),
                       new CoutryTelCodeModel("55", "BR",1),
                       new CoutryTelCodeModel("246", "IO",1),
                       new CoutryTelCodeModel("1-284", "VG",1),
                       new CoutryTelCodeModel("673", "BN",1),
                       new CoutryTelCodeModel("359", "BG",(decimal)1.5),
                       new CoutryTelCodeModel("226", "BF",1),
                       new CoutryTelCodeModel("257", "BI",1),
                       new CoutryTelCodeModel("855", "KH",1),
                       new CoutryTelCodeModel("237", "CM",1),
                       new CoutryTelCodeModel("1", "CA",(decimal)0.5),
                       new CoutryTelCodeModel("238", "CV",1),
                       new CoutryTelCodeModel("1-345", "KY",1),
                       new CoutryTelCodeModel("236", "CF",1),
                       new CoutryTelCodeModel("235", "TD",1),
                       new CoutryTelCodeModel("56", "CL",1),
                       new CoutryTelCodeModel("86", "CN",1),
                       new CoutryTelCodeModel("61", "CX",1),
                       new CoutryTelCodeModel("61", "CC",1),
                       new CoutryTelCodeModel("57", "CO",(decimal)0.5),
                       new CoutryTelCodeModel("269", "KM",1),
                       new CoutryTelCodeModel("682", "CK",1),
                       new CoutryTelCodeModel("506", "CR",1),
                       new CoutryTelCodeModel("385", "HR",(decimal)1.5),
                       new CoutryTelCodeModel("53", "CU",1),
                       new CoutryTelCodeModel("599", "CW",1),
                       new CoutryTelCodeModel("357", "CY",1),
                       new CoutryTelCodeModel("420", "CZ",(decimal)1.5),
                       new CoutryTelCodeModel("243", "CD",1),
                       new CoutryTelCodeModel("45", "DK",2),
                       new CoutryTelCodeModel("253", "DJ",1),
                       new CoutryTelCodeModel("1-767", "DM",1),
                       new CoutryTelCodeModel("1-809", "DO",1),
                       new CoutryTelCodeModel("1-829", "DO",1),
                       new CoutryTelCodeModel("1-849", "DO",1),
                       new CoutryTelCodeModel("670", "TL",1),
                       new CoutryTelCodeModel("593", "EC",1),
                       new CoutryTelCodeModel("20", "EG",(decimal)1.5),
                       new CoutryTelCodeModel("503", "SV",1),
                       new CoutryTelCodeModel("240", "GQ",1),
                       new CoutryTelCodeModel("291", "ER",1),
                       new CoutryTelCodeModel("372", "EE",(decimal)1.5),
                       new CoutryTelCodeModel("251", "ET",1),
                       new CoutryTelCodeModel("500", "FK",1),
                       new CoutryTelCodeModel("298", "FO",1),
                       new CoutryTelCodeModel("679", "FJ",1),
                       new CoutryTelCodeModel("358", "FI",1),
                       new CoutryTelCodeModel("33", "FR",2),
                       new CoutryTelCodeModel("689", "PF",1),
                       new CoutryTelCodeModel("241", "GA",1),
                       new CoutryTelCodeModel("220", "GM",1),
                       new CoutryTelCodeModel("995", "GE",1),
                       new CoutryTelCodeModel("49", "DE",2),
                       new CoutryTelCodeModel("233", "GH",1),
                       new CoutryTelCodeModel("350", "GI",1),
                       new CoutryTelCodeModel("30", "GR",1),
                       new CoutryTelCodeModel("299", "GL",(decimal)0.5),
                       new CoutryTelCodeModel("1-473", "GD",1),
                       new CoutryTelCodeModel("1-671", "GU",1),
                       new CoutryTelCodeModel("502", "GT",1),
                       new CoutryTelCodeModel("44-1481", "GG",1),
                       new CoutryTelCodeModel("224", "GN",1),
                       new CoutryTelCodeModel("245", "GW",1),
                       new CoutryTelCodeModel("592", "GY",1),
                       new CoutryTelCodeModel("509", "HT",1),
                       new CoutryTelCodeModel("504", "HN",1),
                       new CoutryTelCodeModel("852", "HK",1),
                       new CoutryTelCodeModel("36", "HU",(decimal)1.5),
                       new CoutryTelCodeModel("354", "IS",1),
                       new CoutryTelCodeModel("91", "IN",(decimal)0.5),
                       new CoutryTelCodeModel("62", "ID",(decimal)0.5),
                       new CoutryTelCodeModel("98", "IR",1),
                       new CoutryTelCodeModel("964", "IQ",1),
                       new CoutryTelCodeModel("353", "IE",1),
                       new CoutryTelCodeModel("44-1624", "IM",1),
                       new CoutryTelCodeModel("972", "IL",(decimal)0.5),
                       new CoutryTelCodeModel("39", "IT",1),
                       new CoutryTelCodeModel("225", "CI",1),
                       new CoutryTelCodeModel("1-876", "JM",1),
                       new CoutryTelCodeModel("81", "JP",1),
                       new CoutryTelCodeModel("44-1534", "JE",1),
                       new CoutryTelCodeModel("962", "JO",1),
                       new CoutryTelCodeModel("7", "KZ",1),
                       new CoutryTelCodeModel("254", "KE",1),
                       new CoutryTelCodeModel("686", "KI",1),
                       new CoutryTelCodeModel("383", "XK",1),
                       new CoutryTelCodeModel("965", "KW",1),
                       new CoutryTelCodeModel("996", "KG",1),
                       new CoutryTelCodeModel("856", "LA",1),
                       new CoutryTelCodeModel("371", "LV",1),
                       new CoutryTelCodeModel("961", "LB",1),
                       new CoutryTelCodeModel("266", "LS",1),
                       new CoutryTelCodeModel("231", "LR",1),
                       new CoutryTelCodeModel("218", "LY",2),
                       new CoutryTelCodeModel("423", "LI",1),
                       new CoutryTelCodeModel("370", "LT",1),
                       new CoutryTelCodeModel("352", "LU",1),
                       new CoutryTelCodeModel("853", "MO",1),
                       new CoutryTelCodeModel("389", "MK",1),
                       new CoutryTelCodeModel("261", "MG",1),
                       new CoutryTelCodeModel("265", "MW",1),
                       new CoutryTelCodeModel("60", "MY",1),
                       new CoutryTelCodeModel("960", "MV",1),
                       new CoutryTelCodeModel("223", "ML",1),
                       new CoutryTelCodeModel("356", "MT",1),
                       new CoutryTelCodeModel("692", "MH",1),
                       new CoutryTelCodeModel("222", "MR",1),
                       new CoutryTelCodeModel("230", "MU",1),
                       new CoutryTelCodeModel("262", "YT",1),
                       new CoutryTelCodeModel("52", "MX",(decimal)0.5),
                       new CoutryTelCodeModel("691", "FM",1),
                       new CoutryTelCodeModel("373", "MD",(decimal)1.5),
                       new CoutryTelCodeModel("377", "MC",1),
                       new CoutryTelCodeModel("976", "MN",1),
                       new CoutryTelCodeModel("382", "ME",1),
                       new CoutryTelCodeModel("1-664", "MS",1),
                       new CoutryTelCodeModel("212", "MA",1),
                       new CoutryTelCodeModel("258", "MZ",1),
                       new CoutryTelCodeModel("95", "MM",1),
                       new CoutryTelCodeModel("264", "NA",1),
                       new CoutryTelCodeModel("674", "NR",1),
                       new CoutryTelCodeModel("977", "NP",1),
                       new CoutryTelCodeModel("31", "NL",1),
                       new CoutryTelCodeModel("599", "AN",1),
                       new CoutryTelCodeModel("687", "NC",1),
                       new CoutryTelCodeModel("64", "NZ",1),
                       new CoutryTelCodeModel("505", "NI",1),
                       new CoutryTelCodeModel("227", "NE",1),
                       new CoutryTelCodeModel("234", "NG",1),
                       new CoutryTelCodeModel("683", "NU",1),
                       new CoutryTelCodeModel("850", "KP",1),
                       new CoutryTelCodeModel("1-670", "MP",1),
                       new CoutryTelCodeModel("47", "NO",1),
                       new CoutryTelCodeModel("968", "OM",1),
                       new CoutryTelCodeModel("92", "PK",1),
                       new CoutryTelCodeModel("680", "PW",1),
                       new CoutryTelCodeModel("970", "PS",1),
                       new CoutryTelCodeModel("507", "PA",1),
                       new CoutryTelCodeModel("675", "PG",1),
                       new CoutryTelCodeModel("595", "PY",1),
                       new CoutryTelCodeModel("51", "PE",1),
                       new CoutryTelCodeModel("63", "PH",1),
                       new CoutryTelCodeModel("64", "PN",1),
                       new CoutryTelCodeModel("48", "PL",(decimal)1.5),
                       new CoutryTelCodeModel("351", "PT",1),
                       new CoutryTelCodeModel("1-787", "PR",1),
                       new CoutryTelCodeModel("1-939", "PR",1),
                       new CoutryTelCodeModel("974", "QA",1),
                       new CoutryTelCodeModel("242", "CG",1),
                       new CoutryTelCodeModel("262", "RE",1),
                       new CoutryTelCodeModel("40", "RO",(decimal)1.5),
                       new CoutryTelCodeModel("7", "RU",1),
                       new CoutryTelCodeModel("250", "RW",1),
                       new CoutryTelCodeModel("590", "BL",1),
                       new CoutryTelCodeModel("290", "SH",1),
                       new CoutryTelCodeModel("1-869", "KN",1),
                       new CoutryTelCodeModel("1-758", "LC",1),
                       new CoutryTelCodeModel("590", "MF",1),
                       new CoutryTelCodeModel("508", "PM",(decimal)0.5),
                       new CoutryTelCodeModel("1-784", "VC",1),
                       new CoutryTelCodeModel("685", "WS",1),
                       new CoutryTelCodeModel("378", "SM",1),
                       new CoutryTelCodeModel("239", "ST",1),
                       new CoutryTelCodeModel("966", "SA",(decimal)0.5),
                       new CoutryTelCodeModel("221", "SN",1),
                       new CoutryTelCodeModel("381", "RS",1),
                       new CoutryTelCodeModel("248", "SC",1),
                       new CoutryTelCodeModel("232", "SL",1),
                       new CoutryTelCodeModel("65", "SG",1),
                       new CoutryTelCodeModel("1-721", "SX",1),
                       new CoutryTelCodeModel("421", "SK",(decimal)1.5),
                       new CoutryTelCodeModel("386", "SI",1),
                       new CoutryTelCodeModel("677", "SB",1),
                       new CoutryTelCodeModel("252", "SO",1),
                       new CoutryTelCodeModel("27", "ZA",(decimal)0.5),
                       new CoutryTelCodeModel("82", "KR",1),
                       new CoutryTelCodeModel("211", "SS",2),
                       new CoutryTelCodeModel("34", "ES",1),
                       new CoutryTelCodeModel("94", "LK",1),
                       new CoutryTelCodeModel("249", "SD",2),
                       new CoutryTelCodeModel("597", "SR",1),
                       new CoutryTelCodeModel("47", "SJ",1),
                       new CoutryTelCodeModel("268", "SZ",1),
                       new CoutryTelCodeModel("46", "SE",1),
                       new CoutryTelCodeModel("41", "CH",1),
                       new CoutryTelCodeModel("963", "SY",1),
                       new CoutryTelCodeModel("886", "TW",1),
                       new CoutryTelCodeModel("992", "TJ",1),
                       new CoutryTelCodeModel("255", "TZ",1),
                       new CoutryTelCodeModel("66", "TH",1),
                       new CoutryTelCodeModel("228", "TG",1),
                       new CoutryTelCodeModel("690", "TK",1),
                       new CoutryTelCodeModel("676", "TO",1),
                       new CoutryTelCodeModel("1-868", "TT",1),
                       new CoutryTelCodeModel("216", "TN",2),
                       new CoutryTelCodeModel("90", "TR",(decimal)0.5),
                       new CoutryTelCodeModel("993", "TM",1),
                       new CoutryTelCodeModel("1-649", "TC",1),
                       new CoutryTelCodeModel("688", "TV",1),
                       new CoutryTelCodeModel("1-340", "VI",1),
                       new CoutryTelCodeModel("256", "UG",1),
                       new CoutryTelCodeModel("380", "UA",(decimal)1.5),
                       new CoutryTelCodeModel("971", "AE",(decimal)0.5),
                       new CoutryTelCodeModel("44", "GB",1),
                       new CoutryTelCodeModel("1", "US",(decimal)0.5),
                       new CoutryTelCodeModel("598", "UY",1),
                       new CoutryTelCodeModel("998", "UZ",1),
                       new CoutryTelCodeModel("678", "VU",1),
                       new CoutryTelCodeModel("379", "VA",1),
                       new CoutryTelCodeModel("58", "VE",1),
                       new CoutryTelCodeModel("84", "VN",1),
                       new CoutryTelCodeModel("681", "WF",1),
                       new CoutryTelCodeModel("212", "EH",1),
                       new CoutryTelCodeModel("967", "YE",1),
                       new CoutryTelCodeModel("260", "ZM",1),
                       new CoutryTelCodeModel("263", "ZW",1),
            };


            CoutryTelCodeModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx)).FirstOrDefault();
            return result;

        }
        /// <summary>
        /// I will send the country code so that I can know the name of the country as well as the chat rates for it
        /// </summary>
        /// <param name="ISOCodes">country code </param>
        /// <returns>country Model</returns>
        private CountryCodeModel CountryISOCodeGet(string ISOCodes)
        {
            try
            {
                CountryCodeModel countryCodeDashModel = new CountryCodeModel();
                var SP_Name = Constants.Country.SP_CountryISOCodeGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@ISOCodes",ISOCodes)
                };

                countryCodeDashModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCountryCode, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return countryCodeDashModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Add as External Contact
        /// </summary>
        /// <param name="teamInboxDto"></param>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private async Task<long> addBulkAsExternalContact(TeamInboxDto teamInboxDto, long campaignId)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_SendCampaignTeamInbox;

                if (teamInboxDto.templateVariables != null)
                {
                    teamInboxDto.templateVariables = new TemplateVariablles
                    {
                        VarOne = teamInboxDto.templateVariables.VarOne ?? " ",
                        VarTwo = teamInboxDto.templateVariables.VarTwo ?? " ",
                        VarThree = teamInboxDto.templateVariables.VarThree ?? " ",
                        VarFour = teamInboxDto.templateVariables.VarFour ?? " ",
                        VarFive = teamInboxDto.templateVariables.VarFive ?? " "
                        // VarSix = teamInboxDto.templateVariables.VarSix ?? " ",
                        // VarSeven = teamInboxDto.templateVariables.VarSeven ?? " ",
                        // VarEight = teamInboxDto.templateVariables.VarEight ?? " ",
                        // VarNine = teamInboxDto.templateVariables.VarNine ?? " ",
                        // VarTen = teamInboxDto.templateVariables.VarTen ?? " "



                    };
                }
                else
                {
                    teamInboxDto.templateVariables = new TemplateVariablles
                    {
                        VarOne =  " ",
                        VarTwo = " ",
                        VarThree = " ",
                        VarFour = " ",
                        VarFive = " "
                        //VarSix = " ",
                        //VarSeven = " ",
                        //VarEight = " ",
                        //VarNine = " ",
                        //VarTen = " ",
                    };
                }


                if (teamInboxDto.headerVariabllesTemplate != null)
                {
                    teamInboxDto.headerVariabllesTemplate = new HeaderVariablesTemplate
                    {
                        VarOne = teamInboxDto.headerVariabllesTemplate.VarOne ?? " "
                    };
                }
                else
                {
                    teamInboxDto.headerVariabllesTemplate = new HeaderVariablesTemplate
                    {
                        VarOne = " "
                    };
                }
                if (teamInboxDto.firstButtonURLVariabllesTemplate != null)
                {
                    teamInboxDto.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate
                    {
                        VarOne = teamInboxDto.firstButtonURLVariabllesTemplate.VarOne ?? " "
                    };
                }
                else
                {
                    teamInboxDto.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate
                    {
                        VarOne = " "
                    };
                }
                if (teamInboxDto.secondButtonURLVariabllesTemplate != null)
                {
                    teamInboxDto.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate
                    {
                        VarOne = teamInboxDto.secondButtonURLVariabllesTemplate.VarOne ?? " "
                    };
                }
                else
                {
                    teamInboxDto.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate
                    {
                        VarOne = " "
                    };
                }
                string templateVariables = JsonConvert.SerializeObject(teamInboxDto.templateVariables);
                string headerVariabllesTemplate = JsonConvert.SerializeObject(teamInboxDto.headerVariabllesTemplate);
                string firstButtonURLVariabllesTemplate = JsonConvert.SerializeObject(teamInboxDto.firstButtonURLVariabllesTemplate);
                string secondButtonURLVariabllesTemplate = JsonConvert.SerializeObject(teamInboxDto.secondButtonURLVariabllesTemplate);

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@PhoneNumber",teamInboxDto.phoneNumber)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                    ,new System.Data.SqlClient.SqlParameter("@ContactName",teamInboxDto.contactName)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",teamInboxDto.templateId)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateVariables",templateVariables)
                    ,new System.Data.SqlClient.SqlParameter("@HeaderVariablesTemplate",headerVariabllesTemplate)
                    ,new System.Data.SqlClient.SqlParameter("@URLButton1VariablesTemplate",firstButtonURLVariabllesTemplate)
                    ,new System.Data.SqlClient.SqlParameter("@URLButton2VariablesTemplate",secondButtonURLVariabllesTemplate)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long result = (long)OutputParameter.Value;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long ChangeCampaignActive(int tenantId, long campaignId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_ChangeCampaignActive;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@RowsAffected",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long result = (long)OutputParameter.Value;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region private
        private async Task<bool> sendToWhatsApp(CustomerModel user, int selectedLiveChatID)
        {
            var Tenant = _dbService.GetTenantInfoById(user.TenantId.Value).Result;

            string from = user.phoneNumber;
            DirectLineConnector directLineConnector = new DirectLineConnector(_IDocumentClient);
            var micosoftConversationID = directLineConnector.CheckIsNewConversationD360(Tenant.D360Key, Tenant.DirectLineSecret, user.userId, Tenant.botId).Result;
            // var Bot = directLineConnector.StartBotConversationD360(user.userId, user.ContactID.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", Tenant.DirectLineSecret, Tenant.botId, user.phoneNumber, user.TenantId.ToString(), user.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null).Result;

            var Bot = directLineConnector.StartBotConversationD360E(user.userId, user.ContactID.ToString(), micosoftConversationID.MicrosoftBotId, "EvaluationQuestion", Tenant.DirectLineSecret, Tenant.botId, user.phoneNumber, selectedLiveChatID.ToString() + "," + user.TenantId.ToString() + "," + Tenant.EvaluationText + "," + "ar", user.displayName, Tenant.PhoneNumber, Tenant.isOrderOffer.ToString(), micosoftConversationID.watermark, null).Result;




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

            foreach (var msgBot in botListMessages)
            {


                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await new WhatsAppAppService().BotChatWithCustomer(msgBot, from, Tenant.botId);
                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                {
                    var result = await new WhatsAppAppService().postToFB(postWhatsAppMessageModel, Tenant.D360Key, Tenant.AccessToken, Tenant.IsD360Dialog);
                    if (result)
                    {
                        WhatsAppContent model = new WhatsAppAppService().PrepareMessageContent(msgBot, Tenant.botId, user.userId, Tenant.TenantId.Value, user.ConversationId);
                        var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model);
                        user.customerChat = CustomerChat;
                        //await _hub.Clients.All.SendAsync("brodCastAgentMessage", user);
                        SocketIOManager.SendContact(user, user.TenantId.Value);
                    }
                }
            }

            return true;

        }

        private async Task<UserNotification> SendNotfAsync(string message, long agentID)
        {
            var userIdentifier = ToUserIdentifier(AbpSession.TenantId, agentID);

            await _appNotifier.SendMessageAsync(userIdentifier, message);

            var notifications = await _userNotificationManager.GetUserNotificationsAsync(
              userIdentifier);

            notifications.Sort((x, y) => DateTime.Compare(x.Notification.CreationTime, y.Notification.CreationTime));

            var notf = notifications.ToArray().Last();

            return notf;
        }

        private UserIdentifier ToUserIdentifier(int? TargetTenantId, long TargetUserId)
        {
            return new UserIdentifier(TargetTenantId, TargetUserId);
        }

        private bool IsBundleActive()
        {
            var tenant = _dbService.GetTenantInfoById(AbpSession.TenantId.Value).Result;
            return tenant.IsBundleActive;
        }

        private static ConversationSessionsModel getConversationSessions(int TenantId, string PhoneNumber, string ConversationID)
        {
            try
            {


                ConversationSessionsModel ConversationSessionsEntity = new ConversationSessionsModel();
                var SP_Name = "GetConversationSessions";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@ConversationID",ConversationID)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                };

                ConversationSessionsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSessions, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                return ConversationSessionsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
        {
            try
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "creation_timestamp");
                model.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "expiration_timestamp");
                // model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                return model;
            }
            catch (Exception)
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = 0;
                model.expiration_timestamp = 0;
                // model.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");

                return model;
            }
        }
        #endregion

        private static async Task SendToRestaurantsBot(CustomerModel jsonData)
        {
            try
            {
                var constra = JsonConvert.SerializeObject(jsonData);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettingsModel.BotApi +"api/RestaurantsChatBot/RestaurantsMessageHandler");
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

        //[HttpPost("PostMessage")]
        //public async Task<IActionResult> PostMessage(PostMessageModel postMessage)
        //{
        //    if (string.IsNullOrEmpty(postMessage.UserId) || string.IsNullOrEmpty(postMessage.Text))
        //    {
        //        return BadRequest();
        //    }

        //    if (!IsBundleActive())
        //    {
        //        return BadRequest();
        //    }
        //    message.text = postMessage.Text;
        //    message.agentName = postMessage.AgentName;
        //    message.agentId = postMessage.AgentId;
        //    message.type = "text";

        //    HttpStatusCode result = HttpStatusCode.OK;
        //    var customer = await _dbService.GetCustomer(postMessage.UserId);
        //    if (customer != null && customer.ConversationId != null)
        //    {
        //        //Todo Make the connector as a Service and return status in this method
        //        result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.ConversationId, message);
        //        if (result == HttpStatusCode.Created)
        //        {
        //            var CustomerChat = _dbService.UpdateCustomerChat(AbpSession.TenantId, message, postMessage.UserId, customer.ConversationId);
        //            customer.customerChat = CustomerChat;
        //            // await _hub.Clients.All.SendAsync("brodCastAgentMessage", customer);
        //            //  SocketIOManager.SendChat(customer, customer.TenantId);

        //        }

        //    }
        //    else
        //    {
        //        result = HttpStatusCode.BadRequest;
        //    }

        //    return Ok(result);
        //}


        //[HttpPost("PostAttachment")]
        //public async Task<IActionResult> PostAttachment([FromForm] TeamInboxAttachmentModel model)
        //{
        //    if (model == null || model.UserID == null)
        //    {
        //        return BadRequest();
        //    }
        //    if (!IsBundleActive())
        //    {
        //        return BadRequest();
        //    }
        //    var types = AppsettingsModel.AttacmentTypesAllowed;
        //    HttpStatusCode result = HttpStatusCode.OK;
        //    var customer = await _dbService.GetCustomer(model.UserID);
        //    var tenant = await _dbService.GetTenantInfoById(AbpSession.TenantId.Value);
        //    if (customer != null && customer.ConversationId != null)
        //    {
        //        if (model.FormFile != null)
        //        {

        //            List<ListAttachmentModel> listAttachmentModels = new List<ListAttachmentModel>();

        //            foreach (var item in model.FormFile)
        //            {
        //                if (item.Length > 0)
        //                {
        //                    var formFile = item;
        //                    long ContentLength = formFile.Length;
        //                    byte[] fileData = null;
        //                    using (var ms = new MemoryStream())
        //                    {
        //                        formFile.CopyTo(ms);
        //                        fileData = ms.ToArray();
        //                    }

        //                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
        //                    AttachmentContent attachmentContent = new AttachmentContent()
        //                    {
        //                        Content = fileData,
        //                        Extension = Path.GetExtension(formFile.FileName),
        //                        MimeType = formFile.ContentType,

        //                    };

        //                    var url = await azureBlobProvider.Save(attachmentContent);

        //                    var extention = Path.GetExtension(url);
        //                    var type = types[extention];

        //                    listAttachmentModels.Add(new ListAttachmentModel
        //                    {
        //                        fileName = formFile.FileName,
        //                        mediaUrl = url,
        //                        type = type
        //                    });

        //                }

        //            }

        //            foreach (var item in listAttachmentModels)
        //            {

        //                var content = new Content()
        //                {
        //                    type = item.type,
        //                    text = model.Text,
        //                    fileName = item.fileName,
        //                    mediaUrl = item.mediaUrl,
        //                    altText = model.altText,
        //                    agentName = model.agentName,
        //                    agentId = model.agentId
        //                };

        //                if (!string.IsNullOrEmpty(tenant.AccessToken))
        //                {



        //                    WhatsAppAppService whatsAppApiService = new WhatsAppAppService();

        //                    var x = await whatsAppApiService.SendToWhatsAppNew(JsonConvert.SerializeObject(content), tenant.D360Key, tenant.AccessToken, tenant.IsD360Dialog);

        //                    var modelR = JsonConvert.DeserializeObject<WhatsAppResult>(x);
        //                    var CustomerChat = _dbService.UpdateCustomerChat(AbpSession.TenantId, content, model.UserID, customer.ConversationId);
        //                    customer.customerChat = CustomerChat;

        //                    // await _hub.Clients.All.SendAsync("brodCastAgentMessage", customer);
        //                    SocketIOManager.SendChat(customer, customer.TenantId.Value);





        //                }
        //                else
        //                {
        //                    result = await SunshineConnector.PostMsgToSmooch(customer.SunshineAppID, customer.ConversationId, content);

        //                    if (result == HttpStatusCode.Created)
        //                    {
        //                        var CustomerChat = _dbService.UpdateCustomerChat(AbpSession.TenantId, content, model.UserID, customer.ConversationId);
        //                        customer.customerChat = CustomerChat;
        //                        //  await _hub.Clients.All.SendAsync("brodCastAgentMessage", customer);
        //                        SocketIOManager.SendChat(customer, customer.TenantId.Value);

        //                    }
        //                }

        //            }

        //        }
        //        else
        //        {
        //            result = HttpStatusCode.BadRequest;
        //        }
        //    }
        //    else
        //    {
        //        result = HttpStatusCode.BadRequest;
        //    }

        //    return Ok(result);

        //}
        //[HttpPost("PostD360AttachmentPhone")]
        //public async Task<IActionResult> PostD360AttachmentPhone(D360PostAttachmentModel model)
        //{
        //    if (model == null || model.To == null)
        //    {
        //        return BadRequest();
        //    }
        //    if (!IsBundleActive())
        //    {
        //        return BadRequest();
        //    }
        //    HttpStatusCode result = HttpStatusCode.OK;

        //    if (model.FormFile != null)
        //    {
        //        var types = AppsettingsModel.AttacmentTypesAllowed;
        //        var customer = await _dbService.GetCustomerWithTenantId(AbpSession.TenantId + "_" + model.To, AbpSession.TenantId);
        //        List<ListAttachmentModel> listAttachmentModels = new List<ListAttachmentModel>();
        //        foreach (var item in model.FormFile)
        //        {
        //            if (item.Length > 0)
        //            {
        //                var formFile = item;
        //                long ContentLength = formFile.Length;
        //                byte[] fileData = null;
        //                using (var ms = new MemoryStream())
        //                {
        //                    formFile.CopyTo(ms);
        //                    fileData = ms.ToArray();
        //                }



        //                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
        //                AttachmentContent attachmentContent = new AttachmentContent()
        //                {
        //                    Content = fileData,
        //                    Extension = Path.GetExtension(formFile.FileName),
        //                    MimeType = formFile.ContentType,

        //                };

        //                //UTF8Encoding encoder = new UTF8Encoding();
        //                //SHA256Managed sha256hasher = new SHA256Managed();
        //                //byte[] hashedDataBytes = sha256hasher.ComputeHash(fileData);
        //                string filepath = System.IO.Path.GetDirectoryName(formFile.FileName);
        //                //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), formFile.FileName);
        //                var url = await azureBlobProvider.Save(attachmentContent);

        //                var extention = Path.GetExtension(url);
        //                var type = types[extention];
        //                //var filePath = Path.GetTempFileName();


        //                try
        //                {
        //                    //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot\\UploadFile\\",
        //                    //           formFile.FileName);

        //                    //using (var stream = new FileStream("wwwroot\\UploadFile\\" + formFile.FileName, FileMode.Create))
        //                    //{
        //                    //    await formFile.CopyToAsync(stream);

        //                    //    //stream.Position = 0;
        //                    //}

        //                    listAttachmentModels.Add(new ListAttachmentModel
        //                    {
        //                        typeContent = item.ContentType,
        //                        FilePath = url,
        //                        content = fileData,
        //                        //sh256 = Convert.ToBase64String(hashedDataBytes),
        //                        fileName = formFile.FileName,
        //                        mediaUrl = url,
        //                        type = type
        //                    });

        //                }
        //                catch
        //                {

        //                    listAttachmentModels.Add(new ListAttachmentModel
        //                    {
        //                        typeContent = item.ContentType,
        //                        //FilePath = path,
        //                        content = fileData,
        //                        //sh256 = Convert.ToBase64String(hashedDataBytes),
        //                        fileName = formFile.FileName,
        //                        mediaUrl = url,
        //                        type = type
        //                    });

        //                }




        //            }


        //        }

        //        foreach (var item in listAttachmentModels)
        //        {

        //            SendWhatsAppD360Model masseges = new SendWhatsAppD360Model();
        //            if (item.type.ToLower() == "image")
        //            {


        //                masseges = new SendWhatsAppD360Model
        //                {
        //                    fileName = item.fileName,
        //                    mediaUrl = item.mediaUrl,
        //                    typeContent = item.typeContent,
        //                    filePath = item.FilePath,
        //                    Content = item.content,
        //                    to = model.To,
        //                    type = "image",
        //                    image = new SendWhatsAppD360Model.Image
        //                    {
        //                        mime_type = item.type,
        //                        Content = item.content,
        //                        link = item.mediaUrl,
        //                        caption = item.fileName
        //                    }

        //                };


        //            }
        //            else if (item.type.ToLower() == "video")
        //            {
        //                masseges = new SendWhatsAppD360Model
        //                {
        //                    fileName = item.fileName,
        //                    mediaUrl = item.mediaUrl,
        //                    typeContent = item.typeContent,
        //                    filePath = item.FilePath,
        //                    Content = item.content,
        //                    to = model.To,
        //                    type = "video",
        //                    video = new SendWhatsAppD360Model.Video
        //                    {
        //                        mime_type = item.type,
        //                        Content = item.content,
        //                        link = item.mediaUrl,
        //                        caption = item.fileName
        //                    }

        //                };

        //            }
        //            else if (item.type.ToLower() == "audio")
        //            {
        //                masseges = new SendWhatsAppD360Model
        //                {
        //                    fileName = item.fileName,
        //                    mediaUrl = item.mediaUrl,
        //                    typeContent = item.typeContent,
        //                    filePath = item.FilePath,
        //                    Content = item.content,

        //                    to = model.To,
        //                    type = "voice",
        //                    voice = new SendWhatsAppD360Model.Voice
        //                    {
        //                        mime_type = item.type,
        //                        Content = item.content,
        //                        link = item.mediaUrl,
        //                        caption = item.fileName
        //                    }

        //                };

        //            }
        //            else
        //            {
        //                masseges = new SendWhatsAppD360Model
        //                {
        //                    fileName = item.fileName,

        //                    mediaUrl = item.mediaUrl,
        //                    typeContent = item.typeContent,
        //                    filePath = item.FilePath,
        //                    Content = item.content,
        //                    to = model.To,
        //                    type = "document",
        //                    document = new SendWhatsAppD360Model.Document
        //                    {
        //                        mime_type = item.type,
        //                        Content = item.content,
        //                        link = item.mediaUrl,
        //                        caption = item.fileName
        //                    }

        //                };

        //            }
        //            var content = new Content()
        //            {
        //                type = item.type,
        //                text = model.Text,
        //                fileName = item.fileName,
        //                mediaUrl = item.mediaUrl,
        //                //altText = model.altText,
        //                agentName = model.agentName,
        //                agentId = model.agentId
        //            };

        //            result = await WhatsAppDialogConnector.PostMsgToSmooch(customer.D360Key, masseges, _telemetry);

        //            if (result == HttpStatusCode.Created)
        //            {
        //                var CustomerChat = _dbService.UpdateCustomerChatD360(AbpSession.TenantId, content, AbpSession.TenantId + "_" + model.To, customer.ConversationId);
        //                customer.customerChat = CustomerChat;
        //                // await _hub.Clients.All.SendAsync("brodCastAgentMessage", customer);
        //                SocketIOManager.SendChat(customer, customer.TenantId.Value);

        //            }

        //        }


        //        return Ok(result);

        //    }

        //    return BadRequest();
        //}

    }
}

using Abp.Application.Services.Dto;
using Abp.Web.Models;
using DuoVia.FuzzyStrings;
using Framework.Data;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Generic.Dto;
using Infoseed.MessagingPortal.Group;
using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Wallet;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using InfoSeedParser;
using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Interfaces;
using InfoSeedParser.Parsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUglify.Helpers;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;
using Parameter = Infoseed.MessagingPortal.WhatsApp.Dto.Parameter;
using Task = System.Threading.Tasks.Task;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public class WhatsAppMessageTemplateAppService : MessagingPortalAppServiceBase, IWhatsAppMessageTemplateAppService
    {
       // public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindb.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";
        public string connectionStringMongoDB = "mongodb+srv://infoseed:P%40ssw0rd@campagindbstg.global.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000";

        private readonly IDocumentClient _IDocumentClient;
        private readonly IContactsAppService _contactsAppService;
        private readonly ICaptionBotAppService _captionBotAppService;
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        private readonly IGroupAppService _groupAppService;
        private readonly IWalletAppService _walletAppService;
        private readonly ICampaginExcelExporter _campaginExcelExporter;

        private readonly IContactNewParser _ContactNewParser;


        private string url = "https://startcampingstgnew.azurewebsites.net/api/startCampaign";
        //private string url = "https://startcampign.azurewebsites.net/api/startCampaign";
        public WhatsAppMessageTemplateAppService(
            IDocumentClient iDocumentClient,
            IContactsAppService contactsAppService,
            ICaptionBotAppService captionBotAppService,
            TenantDashboardAppService tenantDashboardAppService,
            IGroupAppService groupAppService,
            IWalletAppService walletAppService,
            ICampaginExcelExporter campaginExcelExporter
            )
        {
            _ContactParser = new ParserFactory().CreateParserContact(nameof(ContactExcelParser));
            _ContactNewParser = new ParserFactory().CreateNewParserContact(nameof(ContactExcelNewParser));
            _IDocumentClient = iDocumentClient;
            _contactsAppService = contactsAppService;
            _captionBotAppService = captionBotAppService;
            _tenantDashboardAppService = tenantDashboardAppService;
            _groupAppService = groupAppService;
            _walletAppService = walletAppService;
            _campaginExcelExporter=campaginExcelExporter;
        }


        public async Task<FileDto> BackUpCampaginForAll(int campaignId)
        {

            List<CampaginMongoModel> model = new List<CampaginMongoModel>();
            try
            {
                // Connection string from Azure Cosmos DB for MongoDB (vCore)
                string connectionString = AppSettingsModel.connectionStringMongoDB;

                // MongoDB database and collection details

                string databaseName = AppSettingsModel.databaseName;
                string collectionName = AppSettingsModel.collectionName;


                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Get the database
                var database = client.GetDatabase(databaseName);

                // Get the collection
                var collection = database.GetCollection<CampaginMongoModel>(collectionName);


                // Build the filter
                var filter = Builders<CampaginMongoModel>.Filter.Eq("campaignId", campaignId);

                try
                {
                    // Find the first matching document
                    var filterResult = await collection.Find(filter).ToListAsync();

                    model=filterResult;


                    foreach (var mo in model)
                    {



                        if (mo.status=="not_send")
                        {

                            mo.statusCode=0;

                        }

                        if(mo.statusCode==200 && !mo.is_sent)
                        {

                            mo.failedDetails = "Inactive WhatsApp number or The recipient’s phone being offline for an extended period.";
                        }
                        switch (mo.statusCode)
                        {
                            case 0:
                                mo.failedDetails = "not sent . ";
                                break;
                            case 132015:
                                mo.failedDetails = "Template is Paused, The message template is currently paused. Please check the template status.";
                                break;
                            case 131048:
                                mo.failedDetails = "Spam rate limit hit, Message failed to send because previous messages exceeded the allowed limit.";
                                break;
                            case 132000:
                                mo.failedDetails = "Template Param Count Mismatch, The number of parameters provided does not match the expected count for the template.";
                                break;
                            case 131008:
                                mo.failedDetails = "Template Param Count Mismatch, The number of parameters provided does not match the expected count for the template.";
                                break;
                            case 132005:
                                mo.failedDetails = "Text is Too Long, The message text exceeds the maximum character limit.";
                                break;
                            case 132016:
                                mo.failedDetails = "Template is Disabled, The message template has been disabled and is not available for use.";
                                break;
                            case 368:
                                mo.failedDetails = "Temporarily Blocked for Policy Violations, Please review policy adherence to resume messaging.";
                                break;
                            case 131042:
                                mo.failedDetails = "Payment Issue, Ensure sufficient balance or resolve payment issues.";
                                break;
                            case 131051:
                                mo.failedDetails = "Unsupported Message Type, The message format is not supported by the system.";
                                break;
                            case 100:
                                mo.failedDetails = "The request included one or more unsupported or misspelled parameters .";
                                break;
                            case 131031:
                                mo.failedDetails = "Health status, Account has been locked. Contact support for assistance.";
                                break;
                            case 33:
                                mo.failedDetails = "The business phone number has been deleted, Please register a new number.";
                                break;
                            case 131005:
                                mo.failedDetails = "Access denied, Permission is either not granted or has been removed.";
                                break;
                            case 131045:
                                mo.failedDetails = "Incorrect certificate, Message failed to send due to a phone number registration error.";
                                break;
                            case 131052:
                                mo.failedDetails = "Media download error, Failed to download media. Check media URL or format.";
                                break;
                            case 131053:
                                mo.failedDetails = "Media upload error, Uploading media failed. Ensure media meets platform requirements.";
                                break;
                            case 131056:
                                mo.failedDetails = "Pair Rate Limit Exceeded, Too many pairing requests within a short period.";
                                break;
                            case 130497:
                                mo.failedDetails = "Restricted from Messaging Users in This Country, Ensure compliance with country-specific regulations.";
                                break;
                            case 131021:
                                mo.failedDetails = "Sender and Recipient are the Same, Avoid sending messages to the same number.";
                                break;
                            case 131026:
                                mo.failedDetails = "Message Undeliverable, The message could not be delivered to the recipient. old version of WhatsApp issues or an invalid number..";
                                break;
                            case 130472:
                                mo.failedDetails = "User's number is part of an experiment, Cannot send messages to this user currently.";
                                break;
                            case 131047:
                                mo.failedDetails = "More than 24 hours have passed since the recipient last replied to the sender number, Use a template to reinitiate conversation.";
                                break;
                            case 131049:
                                mo.failedDetails = "Meta chose not to deliver, Content violates platform policies or limitations.";
                                break;
                            case 1:
                                mo.failedDetails = "API Unknown, An unknown error occurred. Contact support.";
                                break;
                            case 2:
                                mo.failedDetails = "API Service Overloaded, Too many requests. Try again later.";
                                break;
                            case 135000:
                                mo.failedDetails = "Generic user error, An unspecified user error occurred.";
                                break;
                            case 131000:
                                mo.failedDetails = "Something wrong, Unexpected issue encountered. Contact support.";
                                break;
                            case 131016:
                                mo.failedDetails = "Service unavailable, The service is temporarily unavailable.";
                                break;
                            case 131057:
                                mo.failedDetails = "Account in Maintenance Mode, Maintenance is in progress. Service will resume shortly.";
                                break;
                            case 133004:
                                mo.failedDetails = "Server Temporarily Unavailable, Please try again later.";
                                break;
                            case 4:
                                mo.failedDetails = "Too Many Calls (Inbound/Outbound), Limit API requests to prevent throttling.";
                                break;
                            case 80007:
                                mo.failedDetails = "Rate Limit Exceeded, Sending more than allowed per second.";
                                break;
                            case 130429:
                                mo.failedDetails = "Rate limit hit, The app has reached the API's throughput limit.";
                                break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }









            }
            catch (Exception ex)
            {

            }


            return _campaginExcelExporter.BackUpCampaginForAll(model); 
        }

        public async Task<WhatsAppAnalyticsModel> GetWhatsAppAnalyticAsync(DateTime start, DateTime end)
        {
            return await getWhatsAppAnalyticAsync(start, end);
        }

        public long SendMessageTemplate(WhatsAppContactsDto contacts, long templateId, long campaignId,bool IsContact)
        {
            return sendMessageTemplate(contacts, templateId, campaignId, IsContact);
        }
        static List<List<T>> SplitList<T>(List<T> list, int numberOfLists)
        {
            int chunkSize = (int)Math.Ceiling((double)list.Count / numberOfLists);

            return Enumerable.Range(0, numberOfLists)
                .Select(i => list.Skip(i * chunkSize).Take(chunkSize).ToList())
                .ToList();
        }


         static List<List<T>> SplitListD<T>(List<T> items, int size)
        {
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < items.Count; i += size)
            {
                result.Add(items.GetRange(i, Math.Min(size, items.Count - i)));
            }
            return result;
        }
        [HttpPost]
        public SendCampinStatesModel SendCampaignNew(CampinToQueueDto contactsEntity, string sendTime = null)
        {
            try
            {
                #region create camp
                var  checkIdRes = TitleCompaignCheck(contactsEntity.campaignName);
                if (!checkIdRes.IsSuccess)
                {
                    return new SendCampinStatesModel()
                    {
                        Message = checkIdRes.ErrorEn,
                        status = false
                    };
                }
                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();
                DateTime dateTime = DateTime.UtcNow;
                TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                whatsAppCampaignModel.Title = contactsEntity.campaignName;
                whatsAppCampaignModel.Language = contactsEntity.templateLanguage;
                whatsAppCampaignModel.TemplateId = contactsEntity.templateId;
                whatsAppCampaignModel.Type=1;
                contactsEntity.campaignId = AddWhatsAppCampaign(whatsAppCampaignModel);
                #endregion
                if (sendTime == null)
                {
                    if (contactsEntity.groupId != 0)
                    {
                        return sendCampignFromGroup(contactsEntity);
                    }
                    else
                    {
                        return sendCampaignNow(contactsEntity);
                    }
                }
                else
                {
                    if (contactsEntity.groupId != 0)
                    {
                        return sendCampignFromGroupScheduled(contactsEntity, sendTime);
                    }
                    else
                    {
                        return sendCampaignScheduled(contactsEntity, sendTime);
                    }
                }
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
        public SendCampinStatesModel SendTemplate(ContactsEntity contactsModel, WhatsAppContactsDto contacts, long templateId, long campaignId, bool IsContact)
        {

            ContactsEntity contactsEntity = new ContactsEntity();
            SendCampinStatesModel sendCampinStatesModel = new SendCampinStatesModel();
            //var response = new Dictionary<string, dynamic>();
            int pageNumber, pageSize, tenantId = 0;
            pageNumber = 0;
            pageSize = 5000;
            tenantId = (int)AbpSession.TenantId;
            var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);

            //GetFilterContacts

            if (contacts.CountryCode != null)
            {
                contactsEntity = GetFilterContacts(contacts);
            }
            else 
            { 
                contactsEntity = GetExternalContacts(templateId, campaignId, pageNumber, pageSize, tenantId);
            }
            bool isFailed = true;
            int counts = GetDailyLimit(tenantId);
            if (contactsEntity.TotalCount <= counts)
            {
                
                if (walletModel.TotalAmount > 0 && contactsEntity.contacts != null)
                {
                    var category = GetTemplatesCategory(templateId);
                    //double totalCost = 0;


                    foreach (var model in contactsEntity.contacts)
                    {
                        var ISO = getCoutryISO(model.PhoneNumber);
                        var Country = CountryISOCodeGet(ISO.Iso);
                        //UTILITY MARKETING
                        if (category == "MARKETING")
                        {
                            if (Country != null && walletModel.TotalAmount >= Math.Round((decimal)Country.MarketingPrice, 3))
                            {
                                //totalCost += Country.MarketingPrice;
                                walletModel.TotalAmount = walletModel.TotalAmount - Math.Round((decimal)Country.MarketingPrice, 3);
                                isFailed = false;
                            }
                            else if (walletModel.TotalAmount >= (decimal)0.027)
                            {
                                //totalCost += 0.027;
                                walletModel.TotalAmount = walletModel.TotalAmount - (decimal)0.027;
                                isFailed = false;
                            }
                            else
                            {
                                isFailed = true;
                                break;
                            }
                        }
                        else if (category == "UTILITY")
                        {
                            if (Country != null && walletModel.TotalAmount >= Math.Round((decimal)Country.UtilityPrice, 3))
                            {
                                //totalCost += Country.UtilityPrice;
                                walletModel.TotalAmount = walletModel.TotalAmount - Math.Round((decimal)Country.UtilityPrice, 3);
                                isFailed = false;
                            }
                            else if (walletModel.TotalAmount >= (decimal)0.019)
                            {
                                //totalCost += 0.019;
                                walletModel.TotalAmount = walletModel.TotalAmount - (decimal)0.019;
                                isFailed = false;
                            }
                            else
                            {
                                isFailed = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                sendCampinStatesModel.Message = "You have exceeded your daily limit";
                sendCampinStatesModel.status = false;
                return sendCampinStatesModel;
            }
            if (isFailed)
            {
                sendCampinStatesModel.Message = "You don't have enough Funds";
                sendCampinStatesModel.status = false;
            }
            else
            {
                long returnValue = SendMessageTemplate(contacts, templateId, campaignId, IsContact);
                if (returnValue == 1)
                {    //response = new Dictionary<string, dynamic> { { "state", true }, { "message", "success" } };
                    sendCampinStatesModel.Message = "Sent Successfully";
                    sendCampinStatesModel.status = true;
                }
                else
                {
                    sendCampinStatesModel.Message = "You don't have enough Funds";
                    sendCampinStatesModel.status = false;
                }
            }
            return sendCampinStatesModel;
        }
        /// <summary>
        /// addAsExternal  Contact
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task addAsExternalContact(ContactsEntity contactsEntity,long templateId, long campaignId)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_SendCampaignNew;

                string @membersJson = JsonConvert.SerializeObject(contactsEntity.contacts);

                //var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                //    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                //    ,new System.Data.SqlClient.SqlParameter("@CampaignId",input.campaignId)
                //    ,new System.Data.SqlClient.SqlParameter("@TemplateId",input.templateId)
                //    ,new System.Data.SqlClient.SqlParameter("@membersJson",membersJson)
                //};

                //SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static int GetDailyLimit(int? tenantId)
        {
            try
            {
                var SP_Name = Constants.Tenant.SP_GetDailyLimit;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
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
            catch
            {
                return 0;
            }
        }
        public long SendMessageTemplateNew( string parm,string title)
        {
            int tenantId = AbpSession.TenantId.Value;

            //  get the campain from table (WhatsAppMessageCampaign) send (title) and (TenantId)  to get the campaignId and templateId 
            WhatsAppCampaignModel campaign = getWhatsAppCampaignByName(title, tenantId);
            long templateId = campaign.TemplateId;
            long campaignId = campaign.Id;

            return sendMessageTemplate(null, templateId, campaignId, false, parm);
        }

        public bool SendCampaignValidation()
        {
            return sendCampaignValidation();
        }
        public async Task <WhatsAppMessageTemplateModel> GetWhatsAppMessageTemplateAsync()
        {
            try
            {

                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);

                return await getTemplatesFromWA(tenant);


            }
            catch (Exception ex)
            {
                return new WhatsAppMessageTemplateModel();
            }

        }
        public async Task<WhatsAppEntity> GetWhatsAppTemplateForCampaign(int pageNumber = 0, int pageSize = 50, int? tenantId = null)
        {
            return await getWhatsAppTemplateForCampaign(pageNumber, pageSize,tenantId);

        }
        public List<MessageTemplateModel> GetLocalTemplates()
        {
            return getWhatsAppTemplate();
        }
        public WhatsAppEntity GetCampaignByTemplateId(long templateId)
        {
            return getCampaignByTemplateId(templateId);
        }
        public long GetTemplateIdByName(string templateName)
        {
            return getTemplateIdByName(templateName);
        }
        public MessageTemplateModel GetTemplateById(long templateId)
        {
            return getTemplateById(templateId);
        }
        public MessageTemplateModel GetTemplateByWhatsAppId(string templateId)
        {
            return getTemplateByWhatsAppId(templateId);
        }
        public WhatsAppEntity GetWhatsAppCampaign(int pageNumber = 0, int pageSize = 50, int? tenantId = null, int type = 1)
        {
            tenantId ??= AbpSession.TenantId.Value;

            return getWhatsAppCampaign(tenantId.Value, pageNumber, pageSize, type);
        }
        [HttpGet]
        public async Task<CampaignStatisticsDto> GetDetailsWhatsAppCampaign(long campaignId)
        {
            return await getDetailsWhatsAppCampaign(campaignId);
        }
        public WhatsAppEntity GetWhatsAppCampaignHistory(long campaignId)
        {
            return getWhatsAppCampaignHistory(campaignId);
        }

        public int GetContactsCount()
        {
            try
            {
                return getContactsCount();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public DailylimitCount GetDailylimitCount()
        {
            try
            {
                return getDailylimitCount();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ContactsEntity GetFilterContacts(WhatsAppContactsDto contacts)
        {
            return getFilterContacts(contacts);
        }
        public async Task<CampinToQueueDto> GetNewFilterContacts(WhatsAppContactsDto contacts)
        {
            return  await getNewFilterContacts(contacts);
        }
        public ContactsEntity GetExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null)
        {
            return getExternalContacts(templateId, campaignId,pageNumber,pageSize,tenantId);
        }
        public CampinToQueueDto GetNewExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null)
        {
            return getNewExternalContacts(templateId, campaignId, pageNumber, pageSize, tenantId);
        }
        [HttpPost]
        [DontWrapResult]

        public async Task<WhatsAppHeaderUrl> GetWhatsAppMediaLink([FromForm] UploadFileModel file)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a =>
                a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId.Value);

            WhatsAppMediaResult whatsAppMediaResult = new WhatsAppMediaResult();
            var httpClient = new HttpClient();

            var originalFileName = Path.GetFileNameWithoutExtension(file.FormFile.FileName);
            var extension = Path.GetExtension(file.FormFile.FileName);
            var uniqueFileName = $"{originalFileName}_{Guid.NewGuid()}{extension}";

            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAppID +
                          "/uploads?access_token=" + tenant.AccessToken +
                          "&file_length=" + file.FormFile.Length +
                          "&file_type=" + file.FormFile.ContentType;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

            var response = await httpClient.PostAsync(postUrl, new StringContent("", Encoding.UTF8, "application/json"));
            var result = await response.Content.ReadAsStringAsync();

            WhatsAppSession whatsAppSession = JsonConvert.DeserializeObject<WhatsAppSession>(result);
            WhatsAppHeaderUrl whatsAppHeaderUrl = await GetWhatsAppMediaURL(file, whatsAppSession);
            whatsAppHeaderUrl.filename = uniqueFileName;
            whatsAppHeaderUrl.InfoSeedUrl = await getInfoSeedUrlFile(file, uniqueFileName);
            whatsAppMediaResult.header = whatsAppHeaderUrl;
            whatsAppMediaResult.session = whatsAppSession;

            var xx = JsonConvert.SerializeObject(whatsAppMediaResult);

            return whatsAppHeaderUrl;
        }


        [HttpPost]
        [DontWrapResult]
        public async Task<WhatsAppMediaID> GetWhatsAppMediaID([FromForm] UploadFileModel file)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId.Value);
            WhatsAppMediaResult whatsAppMediaResult = new WhatsAppMediaResult();

            string phoneNumberId = tenant.D360Key; // You must ensure this is correct
            string accessToken = tenant.AccessToken;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var postUrl = $"https://graph.facebook.com/v17.0/{phoneNumberId}/media";

            var form = new MultipartFormDataContent();
            form.Add(new StringContent("whatsapp"), "messaging_product");

            var fileContent = new StreamContent(file.FormFile.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.FormFile.ContentType);
            form.Add(fileContent, "file", file.FormFile.FileName);

            var response = await httpClient.PostAsync(postUrl, form);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to upload media: {result}");
            }

            var mediaId = JsonConvert.DeserializeObject<WhatsAppMediaID>(result);
            return mediaId;
        }


        public Task<string> GetInfoSeedUrlFile([FromForm] UploadFileModel file)
        {
            return getInfoSeedUrlFile(file);
        }
        private async Task<string> getInfoSeedUrlFile([FromForm] UploadFileModel model,string filename="")
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


                    if (string.IsNullOrEmpty(filename))
                    {
                        AttachmentContent attachmentContent = new AttachmentContent()
                        {
                            Content = fileData,
                            Extension = Path.GetExtension(filename),
                            MimeType = formFile.ContentType,
                            fileName=filename.Replace(Path.GetExtension(filename), "")

                        };

                        url = await azureBlobProvider.Save(attachmentContent);


                    }
                    else
                    {
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
            }
            return url;
        }
        private async Task<WhatsAppHeaderUrl> GetWhatsAppMediaURL([FromForm] UploadFileModel file, WhatsAppSession session)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId.Value);


            var client = new HttpClient();

            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + session.id;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", tenant.AccessToken);
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var formFile = file.FormFile;

            byte[] fileData = null;
            using (var ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                fileData = ms.ToArray();
            }

            var filename = Guid.NewGuid().ToString()+formFile.FileName;
            content.Add(new StreamContent(new MemoryStream(fileData)), formFile.ContentType, filename);

            var message = await client.PostAsync(postUrl, content);
            var result = await message.Content.ReadAsStringAsync();
            WhatsAppHeaderUrl whatsAppHeaderUrl = new WhatsAppHeaderUrl();
            whatsAppHeaderUrl = JsonConvert.DeserializeObject<WhatsAppHeaderUrl>(result);

            whatsAppHeaderUrl.filename=filename;
            return whatsAppHeaderUrl;



        }
        public async Task<WhatsAppTemplateResultModel> AddWhatsAppMessageTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null)
        {
            try
            {

                tenantId ??= AbpSession.TenantId.Value;
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);

                var clonedTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(
                    JsonConvert.SerializeObject(messageTemplateModel)
                );

                WhatsAppComponentModel componentButtons = clonedTemplate.components.FirstOrDefault(x => x.type == "BUTTONS");
                if (componentButtons != null && componentButtons.buttons != null)
                {
                    componentButtons.buttons = componentButtons.buttons
                        .Where(b => b.type == "QUICK_REPLY")
                        .Concat(componentButtons.buttons.Where(b => b.type != "QUICK_REPLY"))
                        .ToList();
                }
                WhatsAppComponentModel componentButtons2 = messageTemplateModel.components.FirstOrDefault(x => x.type == "BUTTONS");
                if (componentButtons2 != null && componentButtons2.buttons != null)
                {
                    componentButtons2.buttons = componentButtons2.buttons
                        .Where(b => b.type == "QUICK_REPLY")
                        .Concat(componentButtons.buttons.Where(b => b.type != "QUICK_REPLY"))
                        .ToList();
                }
                WhatsAppComponentModel componentBody = clonedTemplate.components.FirstOrDefault(x => x.type == "BODY");
           
                if (clonedTemplate.category == "AUTHENTICATION")
                {
                    WhatsAppComponentModel componentFooter = clonedTemplate.components.FirstOrDefault(x => x.type == "FOOTER");

                    if (componentFooter != null)
                    {
                        if (componentFooter.code_expiration_minutes == null)
                        {
                            clonedTemplate.components.Remove(componentFooter);
                        }
                    }
                    messageTemplateModel.VariableCount = 1;
                    clonedTemplate.parameter_format = null;
                    if (componentBody != null)
                    {
                        componentBody.text = null;
                        int index = clonedTemplate.components.FindIndex(x => x.type == "BODY");
                        if (index != -1)
                        {

                            clonedTemplate.components[index] = componentBody;
                        }
                    }

                    clonedTemplate.sub_category = null;
                }

                if (clonedTemplate.sub_category == "carousel")
                {
                    clonedTemplate.sub_category = null;
                    foreach (var component in clonedTemplate.components)
                    {
                        if (component.cards != null)
                        {
                            foreach (var card in component.cards)
                            {
                                card.variableCount = null;
                                if (card.components != null)
                                {
                                    card.components[0].example.mediaCard = null;
                                    card.components[0].example.mediaID = null;
                                    if (card.components[1].example.body_text[0].Length == 0)
                                    {
                                        card.components[1].example = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    clonedTemplate.sub_category = null;
                }

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var xx = JsonConvert.SerializeObject(messageTemplateModel, settings);
                var xy = JsonConvert.SerializeObject(clonedTemplate, settings);
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "/message_templates");
                request.Headers.Add("Authorization", "Bearer " + tenant.AccessToken);

                var content = new StringContent(JsonConvert.SerializeObject(clonedTemplate, settings), Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonConvert.DeserializeObject<WhatsAppApiErrorResponse>(result);

                    return new WhatsAppTemplateResultModel
                    {
                        success = false,
                        error = new WhatsAppTemplateResultModel.Error
                        {
                            message = errorResponse?.error?.message ?? "Unknown error occurred",
                            type = errorResponse?.error?.type,
                            code = errorResponse?.error?.code ?? 0,
                            fbtrace_id = errorResponse?.error?.fbtrace_id,
                            error_user_msg = errorResponse?.error?.error_user_msg,
                            error_user_title = errorResponse?.error?.error_user_title
                        }
                    };
                }

                var objWhatsAppTemplateResultModel = JsonConvert.DeserializeObject<WhatsAppTemplateResultModel>(result);

                messageTemplateModel.id = objWhatsAppTemplateResultModel.Id;
                if (objWhatsAppTemplateResultModel.Id != null)
                {
                    messageTemplateModel.TenantId = tenantId.Value;
                    addWhatsAppMessageTemplate(messageTemplateModel);
                }

                objWhatsAppTemplateResultModel.success = true;
                return objWhatsAppTemplateResultModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<WhatsAppTemplateResultModel> UpdateTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null)
        {
            try
            {
                tenantId ??= AbpSession.TenantId.Value;
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);
                WhatsAppComponentModel componentBody = messageTemplateModel.components.Where(x => x.type == "BODY").FirstOrDefault();
                //componentBody.text = HtmlToPlainText(componentBody.text);
                WhatsAppComponentModel carousel = messageTemplateModel.components.Where(x => x.type == "carousel").FirstOrDefault();



                var clonedTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(
                    JsonConvert.SerializeObject(messageTemplateModel)
                );
                WhatsAppComponentModel componentButtons = clonedTemplate.components.FirstOrDefault(x => x.type == "BUTTONS");
                if (componentButtons != null && componentButtons.buttons != null)
                {
                    componentButtons.buttons = componentButtons.buttons
                        .Where(b => b.type == "QUICK_REPLY")
                        .Concat(componentButtons.buttons.Where(b => b.type != "QUICK_REPLY"))
                        .ToList();
                }
                WhatsAppComponentModel componentButtons2 = messageTemplateModel.components.FirstOrDefault(x => x.type == "BUTTONS");
                if (componentButtons2 != null && componentButtons2.buttons != null)
                {
                    componentButtons2.buttons = componentButtons2.buttons
                        .Where(b => b.type == "QUICK_REPLY")
                        .Concat(componentButtons.buttons.Where(b => b.type != "QUICK_REPLY"))
                        .ToList();
                }
                WhatsAppComponentModel componentBody2 = clonedTemplate.components.FirstOrDefault(x => x.type == "BODY");

                if (clonedTemplate.category == "AUTHENTICATION")
                {
                    WhatsAppComponentModel componentFooter = clonedTemplate.components.FirstOrDefault(x => x.type == "FOOTER");

                    if (componentFooter != null)
                    {
                        if (componentFooter.code_expiration_minutes == null)
                        {
                            clonedTemplate.components.Remove(componentFooter);
                        }
                    }
                    messageTemplateModel.VariableCount = 1;
                    clonedTemplate.parameter_format = null;
                    if (componentBody2 != null)
                    {
                        componentBody2.text = null;
                        int index = clonedTemplate.components.FindIndex(x => x.type == "BODY");
                        if (index != -1)
                        {

                            clonedTemplate.components[index] = componentBody2;
                        }
                    }

                    clonedTemplate.sub_category = null;
                }
                if (carousel!=null)
                {
                    clonedTemplate.sub_category = null;
                    foreach (var component in clonedTemplate.components)
                    {
                        if (component.cards != null)
                        {
                            foreach (var card in component.cards)
                            {
                                if (card.components != null)
                                {
                                    foreach (var cardComponent in card.components)
                                    {
                                        if (cardComponent.example != null)
                                        {
                                            cardComponent.example.mediaCard = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var xx = JsonConvert.SerializeObject(messageTemplateModel, settings);
                var xy = JsonConvert.SerializeObject(clonedTemplate, settings);
                var httpClient = new HttpClient();

                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + messageTemplateModel.id + "?components=" + JsonConvert.SerializeObject(clonedTemplate.components, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) + "&access_token=" + tenant.AccessToken;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                var response = await httpClient.PostAsync(postUrl, new StringContent("", Encoding.UTF8, "application/json"));
                var content = response.Content;
                var result = await content.ReadAsStringAsync();
                WhatsAppTemplateResultModel objWhatsAppTemplateResultModel = new WhatsAppTemplateResultModel();
                objWhatsAppTemplateResultModel = JsonConvert.DeserializeObject<WhatsAppTemplateResultModel>(result);
                if (objWhatsAppTemplateResultModel.success)
                {
                    updateWhatsAppMessageTemplate(messageTemplateModel);
                }
                return objWhatsAppTemplateResultModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long AddWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            return addWhatsAppCampaign(whatsAppCampaignModel);
        }
        [HttpGet]
        public CampinToQueueDto GroupGetByIdForCampign(long groupId)
        {
            try
            {
                return membersGet(groupId);
            }
            catch
            {
                return new CampinToQueueDto();
            }
        }
        public long AddScheduledCampaign(WhatsAppContactsDto contacts, string sendTime, long campaignId, long templateId, bool isExternalContact)
        {
            return   addScheduledCampaign(contacts, sendTime, campaignId, templateId, isExternalContact);
        }

        public void UpdateActivationScheduledCampaign(long campaignId)
        {
            updateActivationScheduledCampaign(campaignId);
        }
        public void UpdateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            updateWhatsAppCampaign(whatsAppCampaignModel);
        }

        public async Task<WhatsAppTemplateResultModel> DeleteWhatsAppMessageTemplateAsync(string templateName)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);
            var httpClient = new HttpClient();
            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "/message_templates?name=" + templateName;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

            var response = await httpClient.DeleteAsync(postUrl);
            var content = response.Content;
            var result = await content.ReadAsStringAsync();
            WhatsAppTemplateResultModel objWhatsAppTemplateResultModel = new WhatsAppTemplateResultModel();
            objWhatsAppTemplateResultModel = JsonConvert.DeserializeObject<WhatsAppTemplateResultModel>(result);
            if (objWhatsAppTemplateResultModel.success)
            {
                deleteWhatsAppMessageTemplate(templateName);
            }
            return objWhatsAppTemplateResultModel;

        }
        public void DeleteWhatsAppCampaign(long campaignId)
        {
            deleteWhatsAppCampaign(campaignId);
        }
        [HttpPost]
        public async Task<ContactParserModel> ReadFromExcel([FromForm] UploadFileModel file, long campaignId, long templateId)
        {
            try
            {
                ContactParserModel contactParserModel = new ContactParserModel();
                contactParserModel.Contacts = new List<WhatsAppContactsDto>();
                if (file == null || file.FormFile.Length == 0)
                {
                    return contactParserModel;
                }

                var formFile = file.FormFile;

                byte[] fileData = null;
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    fileData = ms.ToArray();
                }

                var ContactList = _ContactParser.ParseContact(new ParseConfig()
                {
                    ContactConfig = new ContactConfigurationExcelFile(),
                    FileData = fileData,
                    FileName = formFile.FileName,
                    Parser = nameof(ContactExcelParser)
                });

                contactParserModel.Contacts = ContactList.Contacts;

                var dailylimit = GetDailylimitCount();

                if (contactParserModel.Contacts.Count <= dailylimit.DailyLimit)
                {
                    return contactParserModel;
                }
                else
                    return contactParserModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public CampinToQueueDto ReadFromExcelNew([FromForm] UploadFileModel file, long campaignId, long templateId)
        {
            try
            {
                CampinToQueueDto contactParserModel = new CampinToQueueDto();
                contactParserModel.contacts = new List<ListContactToCampin>();
                if (file == null || file.FormFile.Length == 0)
                {
                    return contactParserModel;
                }

                var formFile = file.FormFile;

                byte[] fileData = null;
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    fileData = ms.ToArray();
                }

                var ContactList = _ContactNewParser.ParseContactNew(new ParseConfig()
                {
                    ContactConfig = new ContactConfigurationExcelFile(),
                    FileData = fileData,
                    FileName = formFile.FileName,
                    Parser = nameof(ContactExcelNewParser)
                });

                contactParserModel.contacts = ContactList.contacts;

                var dailylimit = GetDailylimitCount();

                if (contactParserModel.contacts.Count <= dailylimit.DailyLimit)
                {
                    return contactParserModel;
                }
                else
                    return contactParserModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GetAllDashboard GetStatistics(int? tenantId = null)
        {
            GetAllDashboard statistics = getStatistics(tenantId);

            return statistics;
        }
        public decimal GetContactRate(List<string> contacts)
        {
            return getContactRate(contacts);
        }

        public async Task<BookingContact> SendBookingTemplatesAsync(BookingModel booking, CaptionDto template)
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);
                if (tenant.IsBundleActive)
                {
                    return await SendBookingTemplate(booking, tenant, template);
                }
                else
                {
                    return new BookingContact();
                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BotReservedWordsEntity GetBotReservedWords(int? pageNumber = 0, int? pageSize = 20, int? tenantId = null ,string keyFilter = "")
        {
            return getBotReservedWords(pageNumber, pageSize, tenantId, keyFilter);
        }
        public List<ActionsModel> GetAllActions()
        {
            return getAllActions();
        }
        public BotReservedWordsModel GetByIdBotReservedWords(long Id)
        {
            return getByIdBotReservedWords(Id);
        }
        public ResultBotReservedWordsModel AddBotReservedWord(BotReservedWordsModel model)
        {
            return addBotReservedWord(model);
        }
        public void DeleteBotReservedWord(long Id) 
        {
            deleteBotReservedWord(Id);
        }
        public ResultBotReservedWordsModel UpdateBotReservedWord(BotReservedWordsModel model)
        {
            return updateBotReservedWord(model);
        }
        public long AddExternalContact(WhatsAppContactsDto contact)
        {
            return addExternalContact(contact);
        }
        public ApiResponse<long> TitleCompaignCheck(string title)
        {
            return titleCompaignCheck(title);
        }

        #region Booking

        private async Task<BookingContact> SendBookingTemplate(BookingModel booking, TenantModel tenant, CaptionDto template)
        {
            try
            {
                var statistics = getStatistics(booking.TenantId);
                MessageTemplateModel objWhatsAppTemplateModel = getTemplateById(booking.TemplateId);
                MessageTemplateModel templateWA = getTemplateByWhatsAppId(tenant,objWhatsAppTemplateModel.id).Result;
                BookingContact bookingContact = new BookingContact();
                if (templateWA != null && templateWA.status == Enum.GetName(typeof(WhatsAppTemplateStatusEnum),WhatsAppTemplateStatusEnum.APPROVED))
                {
                    objWhatsAppTemplateModel.components = templateWA.components;
                    bool isContact = _contactsAppService.CheckIfExistContactByPhoneNumber(booking.PhoneNumber);
                    string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);
                    PostMessageTemplateModel _postMessageTemplateModel = prepareBookingMessageTemplate(objWhatsAppTemplateModel, booking, template.Text);
                    var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    bookingContact = await SendBookingTemplateToWhatsApp(tenant, postBody, _postMessageTemplateModel.to,isContact, msg, type, objWhatsAppTemplateModel.mediaLink,template.TextResourceId);
                    bookingContact.BookingId = booking.Id;

                    if (bookingContact.IsSent)
                    {
                        decimal UsageBIRate = 0, UsageFreeRate = 0;

                        CoutryTelCodeModel countryCode = getCoutryISO(booking.PhoneNumber);
                        if (statistics.RemainingBIConversation >= countryCode.Rate)
                        {
                            UsageBIRate += countryCode.Rate;
                            statistics.RemainingBIConversation -= countryCode.Rate;
                        }
                        if (statistics.RemainingFreeConversation >= countryCode.Rate && statistics.RemainingBIConversation < countryCode.Rate)
                        {
                            UsageFreeRate += 1;
                            statistics.RemainingFreeConversation -= 1;
                        }
                        if (UsageBIRate > 0 || UsageFreeRate > 0)
                        {
                            UpdateBIConversation(UsageBIRate, UsageFreeRate);
                        }
                    }
                    
                    return bookingContact;
                }
                else
                {
                    return bookingContact;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PostMessageTemplateModel prepareBookingMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, BookingModel booking, string templateText)
        {
            try
            {
                PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
                WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
                WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();

                List<Component> components = new List<Component>();
                Component componentHeader = new Component();
                Parameter parameterHeader = new Parameter();

                Component componentBody = new Component();
                Parameter parameterBody1 = new Parameter();
                Parameter parameterBody2 = new Parameter();
                Parameter parameterBody3 = new Parameter();
                Parameter parameterBody4 = new Parameter();
                Parameter parameterBody5 = new Parameter();

                ImageTemplate image = new ImageTemplate();
                VideoTemplate video = new VideoTemplate();
                DocumentTemplate document = new DocumentTemplate();


                if (objWhatsAppTemplateModel.components != null)
                {
                    components = new List<Component>();
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type == "HEADER")
                        {
                            componentHeader.type = item.type;
                            parameterHeader.type = item.format.ToLower();
                            if (item.format == "IMAGE")
                            {
                                image.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.image = image;
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
                            }
                            if (item.format == "VIDEO")
                            {
                                video.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.video = video;
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                            }
                            if (item.format == "DOCUMENT")
                            {
                                document.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.document = document;

                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                            }
                            components.Add(componentHeader);

                        }
                        if (item.type == "BODY")
                        {
                            if (objWhatsAppTemplateModel.VariableCount >= 1)
                            {
                                componentBody.parameters = new List<Parameter>();

                                parameterBody1.type = "TEXT";
                                parameterBody1.text = templateText;
                                componentBody.parameters.Add(parameterBody1);

                                if (objWhatsAppTemplateModel.VariableCount >= 2)
                                {
                                    parameterBody2.type = "TEXT";
                                    parameterBody2.text = booking.PhoneNumber;
                                    componentBody.parameters.Add(parameterBody2);

                                    if (objWhatsAppTemplateModel.VariableCount >= 3)
                                    {
                                        parameterBody3.type = "TEXT";
                                        parameterBody3.text = booking.BookingDateTime.ToString();
                                        componentBody.parameters.Add(parameterBody3);

                                        //if (objWhatsAppTemplateModel.VariableCount >= 4)
                                        //{
                                        //    parameterBody4.type = "TEXT";
                                        //    parameterBody4.text = contact.templateVariables.VarFour;
                                        //    componentBody.parameters.Add(parameterBody4);

                                        //    if (objWhatsAppTemplateModel.VariableCount >= 5)
                                        //    {
                                        //        parameterBody5.type = "TEXT";
                                        //        parameterBody5.text = contact.templateVariables.VarFive;
                                        //        componentBody.parameters.Add(parameterBody5);
                                        //    }
                                        //}
                                    }
                                }

                            }

                            componentBody.type = item.type;

                            components.Add(componentBody);
                        }
                    }

                }
                whatsAppLanguageModel.code = objWhatsAppTemplateModel.language;
                whatsAppTemplateModel.language = whatsAppLanguageModel;
                whatsAppTemplateModel.name = objWhatsAppTemplateModel.name;
                whatsAppTemplateModel.components = components;
                postMessageTemplateModel.template = whatsAppTemplateModel;
                postMessageTemplateModel.to = booking.PhoneNumber;
                //postMessageTemplateModel.to = "962786464718";
                postMessageTemplateModel.messaging_product = "whatsapp";
                postMessageTemplateModel.type = "template";
                return postMessageTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task<BookingContact> SendBookingTemplateToWhatsApp(TenantModel tenant, string postBody, string phoneNumber, bool isContacts, string msg, string type, string mediaUrl,int textResourceId)
        {
            try
            {
                var httpClient = new HttpClient();
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.D360Key + "/messages";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
                    templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);
                    BookingContact bookingContact = new BookingContact
                    {
                        TenantId = tenant.TenantId.Value,
                        IsSent = true,
                        MessageId = templateResult.messages.FirstOrDefault().id,
                        PhoneNumber = phoneNumber,
                        TemplateTypeId = textResourceId,
                        MessageRate = getCoutryISO(phoneNumber).Rate,
                        IsReminderSent = false

                    };

                    if (isContacts)
                    {
                        UpdateCustomerChat(phoneNumber, msg, type, mediaUrl);
                    }
                    return bookingContact;
                }
                else
                {
                    BookingContact bookingContact = new BookingContact
                    {
                        TenantId = tenant.TenantId.Value,
                        IsSent = false,
                        MessageId = "",
                        PhoneNumber = phoneNumber,
                        TemplateTypeId = textResourceId,
                        MessageRate = 0,
                        IsReminderSent = false

                    };

                    return bookingContact;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion



        #region Statistics Methods
        private GetAllDashboard getStatistics(int? tenantId = null)
        {
            try
            {
                tenantId ??= AbpSession.TenantId;
                
                GetAllDashboard statistics = new GetAllDashboard();
                var SP_Name = Constants.Dashboard.SP_ConversationMeasurementsGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Year",DateTime.Now.Year)
                    ,new System.Data.SqlClient.SqlParameter("@Month",DateTime.Now.Month)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };
                statistics = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapDashboardStatistic, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                statistics.RemainingUIConversation = Math.Ceiling(statistics.TotalUIConversation - statistics.TotalUsageUIConversation);
                statistics.RemainingBIConversation = Math.Ceiling(statistics.TotalBIConversation - statistics.TotalUsageBIConversation);
                statistics.RemainingFreeConversation = Math.Ceiling(statistics.TotalFreeConversationWA - statistics.TotalUsageFreeConversation);
                statistics.TotalRemainingBIConversation = Math.Ceiling(statistics.RemainingBIConversation + statistics.RemainingFreeConversation);
                
                return statistics;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static long ConvertDatetimeToUnixTimeStamp(DateTime date)
        {
            DateTime originDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - originDate;
            return (long)Math.Floor(diff.TotalSeconds);
        }
        private async Task<WhatsAppAnalyticsModel> getWhatsAppAnalyticAsync(DateTime start, DateTime end)
        {
            int startTime , endTime =0;
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId.Value);

            WhatsAppAnalyticsModel objWhatsAppAnalytic = new WhatsAppAnalyticsModel();

            var httpClient = new HttpClient();
            string[] obj = { "CONVERSATION_DIRECTION", "CONVERSATION_TYPE", "COUNTRY", "PHONE" };

            startTime = (int)ConvertDatetimeToUnixTimeStamp(start);
            endTime = (int)ConvertDatetimeToUnixTimeStamp(end);

            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "?fields=conversation_analytics.start(" + startTime + ").end(" + endTime + ").granularity(DAILY).dimensions(" + obj[0] + "," + obj[1] + "," + obj[2] + "," + obj[3] + ")&access_token = EAAPZAQZBRzVzYBACa0sXe5WLgOOmSnVDoUV3qKYMB8aVZCV5h9P4mY9gBnS65J5kADUaVpJ3xd80wFokjnl9GwQ0S5yNpN8lUqR6x9xncgmRY5ZCbgwrqXiFZClLrUEydvZAbHMfq2WWCkrmWrcH5z5vO6tHuvzv4fvOwgmdefvZC889hAXLuRbN6xVAHZBZAg8wCbrntry64zrgDgLfioZCHF";

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

            var response = await httpClient.GetAsync(postUrl);
            
            var content = response.Content;
            var result = await response.Content.ReadAsStringAsync();
            var WhatsAppAnalytic = await content.ReadAsStringAsync();
            objWhatsAppAnalytic = JsonConvert.DeserializeObject<WhatsAppAnalyticsModel>(WhatsAppAnalytic);
            

            return objWhatsAppAnalytic;
        }

        private void UpdateBIConversation(decimal? usageBI, decimal? usageFree)
        {
            try
            {
                var SP_Name = Constants.Dashboard.SP_ConversationMeasurementBIUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                new System.Data.SqlClient.SqlParameter("@UsageBi",usageBI),
                new System.Data.SqlClient.SqlParameter("@UsageFree",usageFree),
                new System.Data.SqlClient.SqlParameter("@TenantId", AbpSession.TenantId.Value)

            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        #endregion


        #region Contact Methods
        private async void UpdateCustomerChat(string phoneNumber, string msg, string type, string mediaUrl)
        {
            string userId = (int)AbpSession.TenantId.Value + "_" + phoneNumber;
            //   var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection);

            CustomerChat customerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                TenantId = (int)AbpSession.TenantId.Value,
                userId = userId,
                text = msg,
                type = type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = mediaUrl,
                UnreadMessagesCount = 0,
                agentName = "admin",
                agentId = AbpSession.UserId.Value.ToString(),
            };


            var itemsCollection2 = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
            await itemsCollection2.CreateItemAsync(customerChat);
        }

        private readonly IContactParser _ContactParser;
        private int getContactsCount()
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactCountFilterGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);



                return  Convert.ToInt32(OutputParameter.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DailylimitCount getDailylimitCount()
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_DailylimitCount;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };

                var model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapDailylimitCount, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return (model != null) ? model : new DailylimitCount();
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private decimal getContactRateCount(DataTable contacts)
        {
            try
            {
                decimal rateCount = 0;
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
                foreach (DataRow row in contacts.Rows)
                {
                    string PhoneNumber = row["PhoneNumber"].ToString();
                    CoutryTelCodeModel result = TelCodes.Where(x => PhoneNumber.StartsWith(x.Pfx)).FirstOrDefault();
                    rateCount += result.Rate;
                }
                return rateCount;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private decimal getContactRate(List<string> contacts)
        {
            try
            {
                decimal rateCount = 0;
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
                foreach (var contact in contacts)
                {
                    CoutryTelCodeModel result = TelCodes.Where(x => contact.StartsWith(x.Pfx)).FirstOrDefault();
                    rateCount += result.Rate;
                }
                return rateCount;
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ContactsEntity getFilterContacts(WhatsAppContactsDto contacts)
        {
            try
            {
                if (contacts.tenantId == null)
                {
                    contacts.tenantId = AbpSession.TenantId;
                }

                ContactsEntity contactsEntity = new ContactsEntity();

                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactFilterGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@phone",contacts.PhoneNumber)
                   ,new System.Data.SqlClient.SqlParameter("@contactName",contacts.ContactName)
                   ,new System.Data.SqlClient.SqlParameter("@countryCode",contacts.CountryCode)
                   ,new System.Data.SqlClient.SqlParameter("@city",contacts.City)
                   ,new System.Data.SqlClient.SqlParameter("@branch",contacts.Branch)
                   ,new System.Data.SqlClient.SqlParameter("@joiningFrom",contacts.JoiningFrom)
                   ,new System.Data.SqlClient.SqlParameter("@joiningTo",contacts.JoiningTo)
                   ,new System.Data.SqlClient.SqlParameter("@orderTimeFrom",contacts.OrderTimeFrom )
                   ,new System.Data.SqlClient.SqlParameter("@orderTimeTo", contacts.OrderTimeTo)
                   ,new System.Data.SqlClient.SqlParameter("@totalSessions",contacts.TotalSessions)
                   ,new System.Data.SqlClient.SqlParameter("@totalOrderMin",contacts.TotalOrderMin)
                   ,new System.Data.SqlClient.SqlParameter("@totalOrderMax",contacts.TotalOrderMax)

                   ,new System.Data.SqlClient.SqlParameter("@interestedOfOne",contacts.InterestedOfOne)
                   ,new System.Data.SqlClient.SqlParameter("@interestedOfTwo",contacts.InterestedOfTwo)
                   ,new System.Data.SqlClient.SqlParameter("@interestedOfThree",contacts.InterestedOfThree)

                   ,new System.Data.SqlClient.SqlParameter("@isOpt",contacts.IsOpt)
                   

                   ,new System.Data.SqlClient.SqlParameter("@TemplateId",contacts.TemplateId)
                   ,new System.Data.SqlClient.SqlParameter("@CampaignId",contacts.CampaignId)

                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",contacts.pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@PageSize",contacts.pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",contacts.tenantId)


                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                var OutputParameter2 = new System.Data.SqlClient.SqlParameter();
                OutputParameter2.SqlDbType = SqlDbType.BigInt;
                OutputParameter2.ParameterName = "@TotalOptOut";
                OutputParameter2.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter2);

                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapFilterContacts, AppSettingsModel.ConnectionStrings).ToList();


                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                contactsEntity.TotalOptOut = Convert.ToInt32(OutputParameter2.Value);
                contactsEntity.contacts = lstContacts;
                return contactsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<CampinToQueueDto> getNewFilterContacts(WhatsAppContactsDto contacts)
        {
            try
            {
                if (contacts.tenantId == null)
                {
                    contacts.tenantId = AbpSession.TenantId;
                }

                CampinToQueueDto contactsEntity = new CampinToQueueDto();

                contacts.pageNumber = 0;
                contacts.pageSize = 2147483647;

                List<ListContactToCampin> lstContacts = new List<ListContactToCampin>();
                var SP_Name = Constants.WhatsAppCampaign.SP_ContactFilterGetCmap;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@contactName",contacts.ContactName)
                   ,new System.Data.SqlClient.SqlParameter("@countryCode",contacts.CountryCode)
                   ,new System.Data.SqlClient.SqlParameter("@city",contacts.City)
                   ,new System.Data.SqlClient.SqlParameter("@branch",contacts.Branch)
                   ,new System.Data.SqlClient.SqlParameter("@joiningFrom",contacts.JoiningFrom)
                   ,new System.Data.SqlClient.SqlParameter("@joiningTo",contacts.JoiningTo)
                   ,new System.Data.SqlClient.SqlParameter("@orderTimeFrom",contacts.OrderTimeFrom )
                   ,new System.Data.SqlClient.SqlParameter("@orderTimeTo", contacts.OrderTimeTo)
                   ,new System.Data.SqlClient.SqlParameter("@totalSessions",contacts.TotalSessions)
                   ,new System.Data.SqlClient.SqlParameter("@totalOrderMin",contacts.TotalOrderMin)
                   ,new System.Data.SqlClient.SqlParameter("@totalOrderMax",contacts.TotalOrderMax)
                   ,new System.Data.SqlClient.SqlParameter("@interestedOfOne",contacts.InterestedOfOne)
                   ,new System.Data.SqlClient.SqlParameter("@interestedOfTwo",contacts.InterestedOfTwo)
                   ,new System.Data.SqlClient.SqlParameter("@interestedOfThree",contacts.InterestedOfThree)
                   ,new System.Data.SqlClient.SqlParameter("@isOpt",contacts.IsOpt)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",contacts.pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@PageSize",contacts.pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",contacts.tenantId)
                };

                var OutputParameter2 = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalOptOut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter2);

                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapNewFilterContacts, AppSettingsModel.ConnectionStrings).ToList();
                var sortedList = lstContacts.DistinctBy(x => x.PhoneNumber).OrderBy(c => c.CustomerOPT != 1).ToList();

                //var sortedList = lstContacts.OrderBy(c => c.CustomerOPT == 1).ToList();



                contactsEntity.TotalCount = sortedList.Count;
                contactsEntity.TotalOptOut = Convert.ToInt32(OutputParameter2.Value);
                contactsEntity.contacts = sortedList;
                return  contactsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ContactsEntity getExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null)
        {
            try
            {
                ContactsEntity contactsEntity = new ContactsEntity();

                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
                var SP_Name = Constants.Contacts.SP_ContactsExternalByCampaignGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                   ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                   ,new System.Data.SqlClient.SqlParameter("@TemplateId",templateId)


                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapExternalContacts, AppSettingsModel.ConnectionStrings).ToList();


                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                contactsEntity.contacts = lstContacts;
                return contactsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CampinToQueueDto getNewExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null)
        {
            try
            {
                CampinToQueueDto contactsEntity = new CampinToQueueDto();

                List<ListContactToCampin> lstContacts = new List<ListContactToCampin>();
                var SP_Name = Constants.Contacts.SP_ContactsExternalByCampaignGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                   ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                   ,new System.Data.SqlClient.SqlParameter("@TemplateId",templateId)


                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 DataReaderMapper.MapnewExternalContacts, AppSettingsModel.ConnectionStrings).ToList();


                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                contactsEntity.contacts = lstContacts;
                return contactsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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


            CoutryTelCodeModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx) ).FirstOrDefault();
            return result;

        }
        public bool checkPhoneNumber(string phone)
        {
            try
            {
                CoutryTelCodeModel coutryCode = getCoutryISO(phone);
                if (coutryCode != null)
                {
                    string ISO = coutryCode.Iso;

                    PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
                    //var isPossibleNumber = phoneUtil.IsPossibleNumber(phone, ISO);
                    if (phoneUtil.IsPossibleNumber(phone, ISO))
                    {
                        var phoneNumber = phoneUtil.Parse(phone, ISO);
                        bool isValidNumber = phoneUtil.IsValidNumber(phoneNumber);

                        return isValidNumber;
                    }
                    else
                    {
                        return false;
                    }
                    
                }else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }

        }
        private SendCampinStatesModel addScheduledTemplate(WhatsAppContactsNew contacts, string sendTime, string templateName)
        {
            try
            {
                SendCampinStatesModel sendCampinStatesModel = new SendCampinStatesModel();

                int pageNumber, pageSize, tenantId = 0;
                pageNumber = 0;
                pageSize = int.MaxValue;
                tenantId = (int)AbpSession.TenantId;
                var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);

                if (contacts.IsExternal)
                {
                    DataTable tbl = new DataTable();
                    tbl = getExternalForTable(contacts.contacts, (long)contacts.CampaignId, (long)contacts.TemplateId);
                    addBulkExternalContact(tbl);
                }

                var dailyLimit = getDailylimitCount();

                if (dailyLimit.DailyLimit >= contacts.contacts.Count)
                {
                    #region Send Campign

                    #region Wallet Information
                    if (walletModel == null)
                    {
                        _walletAppService.CreateWallet(tenantId);
                        walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    else if (walletModel.TotalAmount <= 0)
                    {
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    #endregion

                    #region check if you hava Funds gretar than or equal convarsaction prise 

                    double Price = 0.014;
                    decimal totalPrice = 0;
                    if (walletModel.TotalAmount > 0)
                    {
                        totalPrice = contacts.contacts.Count * (decimal)Price;
                        //UTILITY MARKETING
                        if (walletModel.TotalAmount >= totalPrice)
                        {
                            var category = GetTemplatesCategory((long)contacts.TemplateId);

                            //long returnValue = addScheduledCampaignNew(contacts, sendTime, (long)contacts.CampaignId, (long)contacts.TemplateId, contacts.IsExternal);
                            //if (returnValue > 0)
                            //{
                            //    sendCampinStatesModel.Message = "Sent Successfully";
                            //    sendCampinStatesModel.status = true;
                            //
                            //    #region Add in transaction table 
                            //
                            //    TransactionModel transactionModel = new TransactionModel();
                            //
                            //    var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                            //    transactionModel.DoneBy = usersDashModel.Name;
                            //    transactionModel.TotalTransaction = totalPrice;
                            //    transactionModel.TotalRemaining = walletModel.TotalAmount - totalPrice;
                            //    transactionModel.TransactionDate = DateTime.UtcNow;
                            //    transactionModel.CategoryType = category;
                            //    transactionModel.TenantId = tenantId;
                            //
                            //    var result = addTransaction(transactionModel, contacts.contacts.Count, templateName, (long)contacts.CampaignId);
                            //
                            //    #endregion
                            //}
                            //else
                            //{
                            //    sendCampinStatesModel.Message = "You don't have enough Funds";
                            //    sendCampinStatesModel.status = false;
                            //}
                        }
                        else
                        {
                            sendCampinStatesModel.Message = "You don't have enough Funds";
                            sendCampinStatesModel.status = false;
                        }
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    sendCampinStatesModel.Message = "You have exceeded your daily limit";
                    sendCampinStatesModel.status = false;
                    return sendCampinStatesModel;
                }
                return sendCampinStatesModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<ListContactToCampin> getOptOutContactByTenantId(int tenantId)
        {
            try
            {
                List<ListContactToCampin> contacts = new List<ListContactToCampin>();
                var SP_Name = Constants.Contacts.SP_ContactsOptOutByTenantIdGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                };

                contacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapExternalContactsForTable, AppSettingsModel.ConnectionStrings).ToList();
                return contacts;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private DataTable getExternalForTable(List<ListContactToCampin> contacts, long campaignId, long templateId)
        {
            try
            {
                int tenantId = AbpSession.TenantId.Value;

                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                tbl.Columns.Add(new DataColumn("TenantId", typeof(int)));
                tbl.Columns.Add(new DataColumn("CampaignId", typeof(long)));
                tbl.Columns.Add(new DataColumn("ContactName", typeof(string)));
                tbl.Columns.Add(new DataColumn("TemplateId", typeof(long)));
                tbl.Columns.Add(new DataColumn("TemplateVariables", typeof(string)));

                List<ListContactToCampin> lstLocalOptOutContacts = getOptOutContactByTenantId(AbpSession.TenantId.Value);
                IEnumerable<ListContactToCampin> distinctList = contacts.Where(c => !lstLocalOptOutContacts.Any(lc => lc.PhoneNumber == c.PhoneNumber)).DistinctBy(x => x.PhoneNumber).ToList();

                foreach (var item in distinctList)
                {
                    if (checkPhoneNumber(item.PhoneNumber))
                    {
                        DataRow dr = tbl.NewRow();
                        dr["PhoneNumber"] = item.PhoneNumber;
                        dr["TenantId"] = tenantId;
                        dr["CampaignId"] = campaignId;
                        dr["ContactName"] = item.ContactName;
                        dr["TemplateId"] = templateId;
                        dr["TemplateVariables"] = JsonConvert.SerializeObject(item.templateVariables);

                        tbl.Rows.Add(dr);
                    }
                }
                return tbl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private DataTable getExternalContactTable(List<WhatsAppContactsDto> contacts,long campaignId,long templateId)
        {
            try
            {
                int tenantId = AbpSession.TenantId.Value;

                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                tbl.Columns.Add(new DataColumn("TenantId", typeof(int)));
                tbl.Columns.Add(new DataColumn("CampaignId", typeof(long)));
                tbl.Columns.Add(new DataColumn("ContactName", typeof(string)));
                tbl.Columns.Add(new DataColumn("TemplateId", typeof(long)));
                tbl.Columns.Add(new DataColumn("TemplateVariables", typeof(string)));

               
                List<WhatsAppContactsDto> lstLocalOptOutContacts = _contactsAppService.GetOptOutContactByTenantId(AbpSession.TenantId.Value);
                IEnumerable<WhatsAppContactsDto> distinctList = contacts.Where(c=> !lstLocalOptOutContacts.Any(lc => lc.PhoneNumber == c.PhoneNumber)).DistinctBy(x => x.PhoneNumber).ToList();

                foreach (var item in distinctList)
                {
                    if (checkPhoneNumber(item.PhoneNumber))
                    {
                        DataRow dr = tbl.NewRow();
                        dr["PhoneNumber"] = item.PhoneNumber;
                        dr["TenantId"] = tenantId;
                        dr["CampaignId"] = campaignId;
                        dr["ContactName"] = item.ContactName;
                        dr["TemplateId"] = templateId;
                        dr["TemplateVariables"] = JsonConvert.SerializeObject(item.templateVariables);

                        tbl.Rows.Add(dr);
                    }
                }
                return tbl;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private bool addBulkExternalContact(DataTable tbl)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsExternalBulkAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tblContactExternalTableType",tbl)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private long addExternalContact(WhatsAppContactsDto contact)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactExternalAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@PhoneNumber",contact.PhoneNumber),
                    new System.Data.SqlClient.SqlParameter("@TenantId",contact.tenantId),
                    new System.Data.SqlClient.SqlParameter("@CampaignId",contact.CampaignId),
                    new System.Data.SqlClient.SqlParameter("@ContactName",contact.ContactName),
                    new System.Data.SqlClient.SqlParameter("@TemplateId",contact.TemplateId),
                    new System.Data.SqlClient.SqlParameter("@TemplateVariables",JsonConvert.SerializeObject(contact.templateVariables)),
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Id",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (long)OutputParameter.Value;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void addContactCampaign(DataTable tbl)
        {

            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsCampaignlBulkAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                                {
                                    new System.Data.SqlClient.SqlParameter("@ContactCampaignTable",tbl)
                                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion


        #region Template Methods

        private List<MessageTemplateModel> getWhatsAppTemplate()
        {
            try
            {
                WhatsAppEntity TemplateEntity = new WhatsAppEntity();
                List<MessageTemplateModel> lstWhatsAppTemplateModel = new List<MessageTemplateModel>();
                List<MessageTemplateModel> lstWhatsAppTemplate = new List<MessageTemplateModel>();
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                new System.Data.SqlClient.SqlParameter("@PageSize",int.MaxValue)
               ,new System.Data.SqlClient.SqlParameter("@PageNumber",0)
               ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)




            };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplate, AppSettingsModel.ConnectionStrings).ToList();

                return lstWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WhatsAppEntity getCampaignByTemplateId(long templateId)
        {
            try
            {
                WhatsAppEntity whatsAppEntity = new WhatsAppEntity();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignByTemplateGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TemplateId",templateId)
                };
                
                whatsAppEntity.lstWhatsAppCampaignModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCampaign, AppSettingsModel.ConnectionStrings).ToList();

                return whatsAppEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private long getTemplateIdByName(string templateName)
        {
            try
            {
                MessageTemplateModel template = new MessageTemplateModel();
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplateByNameGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TemplateName",templateName),
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                };

                template = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplate, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                if (template != null)
                {
                    return template.LocalTemplateId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<WhatsAppEntity> getWhatsAppTemplateForCampaign(int pageNumber = 0, int pageSize = 50, int? tenantId = null)  
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == (int)AbpSession.TenantId.Value); 

                WhatsAppEntity TemplateEntity = new WhatsAppEntity();
                List<MessageTemplateModel> lstWhatsAppTemplateModel = new List<MessageTemplateModel>();
                List<MessageTemplateModel> lstWhatsAppTemplate = new List<MessageTemplateModel>();
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapTemplate, AppSettingsModel.ConnectionStrings).ToList();

            

                Task<WhatsAppMessageTemplateModel> objWhatsAppTemplate = getTemplatesFromWA(tenant);
                var res = objWhatsAppTemplate.Result.data;

                if (res != null)
                {

                    foreach (var item in lstWhatsAppTemplateModel)
                    {
                        if (res.Any(x => x.id == item.id && x.status == "APPROVED" && item.isDeleted == false))
                        {
                            lstWhatsAppTemplate.Add(item);
                        }
                    }
                }
                lstWhatsAppTemplate = lstWhatsAppTemplate.Where(x => x.language == "ar" || x.language == "en").ToList();
                TemplateEntity.TotalCount = lstWhatsAppTemplate.Count();
                TemplateEntity.lstWhatsAppTemplateModel = lstWhatsAppTemplate;
                return TemplateEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MessageTemplateModel getTemplateById(long templateId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGetById;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@TemplateId", templateId) };
                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplate, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return objWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private MessageTemplateModel getTemplateByWhatsAppId(string templateId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_TemplateGetByWhatsAppId;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@TemplateId", templateId) };
                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplate, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                //if (objWhatsAppTemplateModel != null)
                //{
                //    foreach (var item in objWhatsAppTemplateModel.components)
                //    {
                //        if (item.text != null)
                //        {
                //            item.text = PlainTextTohtml(item.text);
                //            //item.text = PlainTextTohtml(item.text);
                //        }
                //    }
                //}
                
                return objWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private SendCampinStatesModel sendCampaignNow(CampinToQueueDto contactsEntity)
        {
            SendCampinStatesModel sendCampinStatesModel = new SendCampinStatesModel();
            try
            {
                var dailyLimit = getDailylimitCount();

                if (dailyLimit.DailyLimit >= contactsEntity.contacts.Count)
                {
                    #region Send Campign

                    int tenantId = AbpSession.TenantId.Value;

                    #region Wallet Information
                    var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                    if (walletModel == null)
                    {
                        _walletAppService.CreateWallet(tenantId);
                        walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    else if (walletModel.TotalAmount <= 0)
                    {
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    #endregion

                    #region check if you hava Funds gretar than or equal convarsaction prise 
                    bool isFailed = true;
                    double Price = 0.014;
                    decimal totalPrice = 0;
                    if (walletModel.TotalAmount > 0)
                    {
                        totalPrice = contactsEntity.contacts.Count * (decimal)Price;
                        //UTILITY MARKETING
                        if (walletModel.TotalAmount >= totalPrice)
                        {
                            var tenantInfo = GetTenantInfo(tenantId);
                            // Split the list into 10 smaller lists
                            List<List<ListContactToCampin>> splitLists = SplitList(contactsEntity.contacts, 10);

                            // Display the count of each split list
                            string sendcompaing = "campaign";
                            int count = 1;
                            var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignAddOnDB;

                            MessageTemplateModel objWhatsAppTemplateModel = getTemplateById(contactsEntity.templateId);
                            MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                            if (templateWA != null && templateWA.status == "APPROVED")
                            {
                                objWhatsAppTemplateModel.components = templateWA.components;

                                string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);

                                CampinQueueNew campinQueueNew = new CampinQueueNew();
                                foreach (var OuterList in splitLists)
                                {
                                    foreach (var contact in OuterList)
                                    {
                                        contact.haderVariablesTemplate = contactsEntity.headerVariabllesTemplate;
                                        contact.firstButtonURLVariabllesTemplate = contactsEntity.firstButtonURLVariabllesTemplate;
                                        contact.secondButtonURLVariabllesTemplate = contactsEntity.secondButtonURLVariabllesTemplate;
                                        contact.carouselVariabllesTemplate = contactsEntity.CarouselTemplate;
                                        contact.buttonCopyCodeVariabllesTemplate = contactsEntity.buttonCopyCodeVariabllesTemplate;
                                        contact.templateVariables = contactsEntity.templateVariables;
                                    }
                                    var JopName = sendcompaing + count.ToString();
                                    if (OuterList.Count == 0)
                                    {
                                        break;
                                    }
                                    string str = JsonConvert.SerializeObject(OuterList);
                                    string TemplateJson = JsonConvert.SerializeObject(objWhatsAppTemplateModel);
                                    if (contactsEntity.templateVariables==null)
                                    {
                                        contactsEntity.templateVariables=new TemplateVariablles();
                                    }
                                    TemplateVariablles templateVariables = null;
                                    HeaderVariablesTemplate headerVariabllesTemplate = null;
                                    FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate = null;
                                    SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate = null;
                                    CarouselVariabllesTemplate carouselVariabllesTemplate = null;
                                    ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate = null;
                                    if (contactsEntity.templateVariables != null)
                                    {
                                        templateVariables = contactsEntity.templateVariables;
                                    }
                                    if (contactsEntity.headerVariabllesTemplate != null)
                                    {
                                        headerVariabllesTemplate = contactsEntity.headerVariabllesTemplate;
                                    }
                                    if (contactsEntity.firstButtonURLVariabllesTemplate != null)
                                    {
                                        firstButtonURLVariabllesTemplate = contactsEntity.firstButtonURLVariabllesTemplate;
                                    }
                                    if (contactsEntity.secondButtonURLVariabllesTemplate != null)
                                    {
                                        secondButtonURLVariabllesTemplate = contactsEntity.secondButtonURLVariabllesTemplate;
                                    }
                                    if (contactsEntity.CarouselTemplate != null)
                                    {
                                        carouselVariabllesTemplate = contactsEntity.CarouselTemplate;
                                    }
                                    if (contactsEntity.buttonCopyCodeVariabllesTemplate != null)
                                    {
                                        buttonCopyCodeVariabllesTemplate = contactsEntity.buttonCopyCodeVariabllesTemplate;
                                    }
                                    //string str = JsonConvert.SerializeObject(OuterList);
                                    string templateVariablesjson = JsonConvert.SerializeObject(templateVariables);
                                    string headerVarjson = JsonConvert.SerializeObject(headerVariabllesTemplate);
                                    string firstURLVariabllesjson = JsonConvert.SerializeObject(firstButtonURLVariabllesTemplate);
                                    string secURLVariabllesjson = JsonConvert.SerializeObject(secondButtonURLVariabllesTemplate);
                                    string carouselVariabllesjson = JsonConvert.SerializeObject(carouselVariabllesTemplate);
                                    string copyCodeVariabllesjson = JsonConvert.SerializeObject(buttonCopyCodeVariabllesTemplate);
                                    string TemplateVariablesJson = JsonConvert.SerializeObject(contactsEntity.templateVariables);


                                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                                         new System.Data.SqlClient.SqlParameter("@Contacts",str)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignId",contactsEntity.campaignId)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateId",contactsEntity.templateId)
                                        ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",contactsEntity.IsExternal)
                                        ,new System.Data.SqlClient.SqlParameter("@JopName",JopName)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateName",contactsEntity.templateName)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignName",contactsEntity.campaignName)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateJson",TemplateJson)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesJson","")
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

                                    count++;
                                    
                                        if (OutputParameter.Value != DBNull.Value)
                                        {
                                          
                                                campinQueueNew.messageTemplateModel = objWhatsAppTemplateModel;
                                                campinQueueNew.campaignId = contactsEntity.campaignId;
                                                campinQueueNew.templateId = contactsEntity.templateId;
                                                campinQueueNew.IsExternal = contactsEntity.IsExternal;
                                                campinQueueNew.TenantId = tenantInfo.TenantId;
                                                campinQueueNew.D360Key = tenantInfo.D360Key;
                                                campinQueueNew.AccessToken = tenantInfo.AccessToken;
                                                campinQueueNew.functionName = JopName;
                                                campinQueueNew.msg = msg;
                                                campinQueueNew.type = type;
                                                campinQueueNew.contacts = null;
                                                campinQueueNew.templateVariables = null;
                                                campinQueueNew.campaignName = contactsEntity.campaignName;
                                                campinQueueNew.rowId = Convert.ToInt64(OutputParameter.Value);
                                             //SetCampinQueueContact(campinQueueNew);
                                             // SetCampinInFun(campinQueueNew);
                                         }
                                        else
                                        {
                                            continue;
                                        }

                                    
                                   
                                }


                                SetCampinInFun(campinQueueNew);


                                #region Add in transaction table 

                                TransactionModel transactionModel = new TransactionModel();

                                var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                                transactionModel.DoneBy = usersDashModel.Name;
                                transactionModel.TotalTransaction = totalPrice;
                                transactionModel.TotalRemaining = walletModel.TotalAmount - totalPrice;
                                transactionModel.TransactionDate = DateTime.UtcNow;
                                transactionModel.CategoryType = objWhatsAppTemplateModel.category;
                                transactionModel.TenantId = tenantInfo.TenantId;

                                var result = addTransaction(transactionModel, contactsEntity.contacts.Count, objWhatsAppTemplateModel.name, contactsEntity.campaignId);

                                #endregion

                                #region statistic Campaign
                                WhatsAppCampaignModel statistics = new WhatsAppCampaignModel
                                {
                                    Id = contactsEntity.campaignId,
                                    SentTime = DateTime.UtcNow,
                                    Status = (int)WhatsAppCampaignStatusEnum.Active
                                };
                                updateWhatsAppCampaign(statistics);
                                #endregion

                                sendCampinStatesModel.Message = "Sent Successfully";
                                sendCampinStatesModel.status = true;
                            }
                            else
                            {
                                sendCampinStatesModel.Message = "template Not APPROVED";
                                sendCampinStatesModel.status = false;
                            }
                        }
                        else
                        {
                            sendCampinStatesModel.Message = "You don't have enough Funds";
                            sendCampinStatesModel.status = false;
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    sendCampinStatesModel.Message = "You have exceeded your daily limit";
                    sendCampinStatesModel.status = false;
                }
            }
            catch (Exception ex)
            {
                sendCampinStatesModel.Message = ex.Message;
                sendCampinStatesModel.status = false;
            }
            return sendCampinStatesModel;
        }
        private SendCampinStatesModel sendCampaignScheduled(CampinToQueueDto contactsEntity, string sendTime)
        {
            try
            {
                var dailyLimit = getDailylimitCount();

                if (dailyLimit.DailyLimit >= contactsEntity.contacts.Count)
                {
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

                    totalPrice = contactsEntity.contacts.Count * (decimal)Price;
                    //UTILITY MARKETING
                    if (walletModel.TotalAmount >= totalPrice)
                    {
                        var tenantInfo = GetTenantInfo(tenantId);
                        
                        var category = GetTemplatesCategory(contactsEntity.templateId);

                        long returnValue = addScheduledCampaignonOnDB(contactsEntity, sendTime);
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

                            var result = addTransaction(transactionModel, contactsEntity.contacts.Count, contactsEntity.templateName, contactsEntity.campaignId);
                            #endregion

                            #region statistic Campaign
                            DateTime dateTimes;
                            if (DateTime.TryParseExact(sendTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimes))
                            {
                                dateTimes = dateTimes.AddHours(AppSettingsModel.DivHour);
                            }

                            WhatsAppCampaignModel statistics = new WhatsAppCampaignModel
                            {
                                Id = contactsEntity.campaignId,
                                SentTime = dateTimes,
                                Status = (int)WhatsAppCampaignStatusEnum.Scheduled
                            };
                            updateWhatsAppCampaign(statistics);
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
                                status = false
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
                else
                {
                    return new SendCampinStatesModel()
                    {
                        Message = "You have exceeded your daily limit",
                        status = false
                    };
                }
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
        private long addTransaction(TransactionModel model, int totalCount ,string TemplateName,long campaignId)
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
        private static PostMessageTemplateModel prepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, ListContactToCampin contact,bool IsExternal , TemplateVariables templateVariables)
        {
            try
            {
                PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
                WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
                WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();

                List<Component> components = new List<Component>();
                Component componentHeader = new Component();
                Parameter parameterHeader = new Parameter();

                Component componentBody = new Component();
                Parameter parameterBody1 = new Parameter();
                Parameter parameterBody2 = new Parameter();
                Parameter parameterBody3 = new Parameter();
                Parameter parameterBody4 = new Parameter();
                Parameter parameterBody5 = new Parameter();

                ImageTemplate image = new ImageTemplate();
                VideoTemplate video = new VideoTemplate();
                DocumentTemplate document = new DocumentTemplate();


                if (objWhatsAppTemplateModel.components != null)
                {
                    components = new List<Component>();
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type == "HEADER")
                        {
                            componentHeader.type = item.type;
                            parameterHeader.type = item.format.ToLower();
                            if (item.format == "IMAGE")
                            {
                                image.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.image = image;
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
                            }
                            if (item.format == "VIDEO")
                            {
                                video.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.video = video;
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                            }
                            if (item.format == "DOCUMENT")
                            {
                                document.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.document = document;

                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };

                            }
                            components.Add(componentHeader);

                        }
                        if (item.type == "BODY")
                        {
                            if (contact.templateVariables != null && IsExternal)
                            {
                                if (objWhatsAppTemplateModel.VariableCount >= 1)
                                {
                                    componentBody.parameters = new List<Parameter>();

                                    parameterBody1.type = "TEXT";
                                    parameterBody1.text = contact.templateVariables.VarOne;
                                    componentBody.parameters.Add(parameterBody1);

                                    if (objWhatsAppTemplateModel.VariableCount >= 2)
                                    {
                                        parameterBody2.type = "TEXT";
                                        parameterBody2.text = contact.templateVariables.VarTwo;
                                        componentBody.parameters.Add(parameterBody2);

                                        if (objWhatsAppTemplateModel.VariableCount >= 3)
                                        {
                                            parameterBody3.type = "TEXT";
                                            parameterBody3.text = contact.templateVariables.VarThree;
                                            componentBody.parameters.Add(parameterBody3);

                                            if (objWhatsAppTemplateModel.VariableCount >= 4)
                                            {
                                                parameterBody4.type = "TEXT";
                                                parameterBody4.text = contact.templateVariables.VarFour;
                                                componentBody.parameters.Add(parameterBody4);

                                                if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                {
                                                    parameterBody5.type = "TEXT";
                                                    parameterBody5.text = contact.templateVariables.VarFive;
                                                    componentBody.parameters.Add(parameterBody5);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else if (templateVariables != null && !IsExternal)
                            {
                                if (objWhatsAppTemplateModel.VariableCount >= 1)
                                {
                                    componentBody.parameters = new List<Parameter>();

                                    parameterBody1.type = "TEXT";
                                    parameterBody1.text = templateVariables.VarOne;
                                    componentBody.parameters.Add(parameterBody1);

                                    if (objWhatsAppTemplateModel.VariableCount >= 2)
                                    {
                                        parameterBody2.type = "TEXT";
                                        parameterBody2.text = templateVariables.VarTwo;
                                        componentBody.parameters.Add(parameterBody2);

                                        if (objWhatsAppTemplateModel.VariableCount >= 3)
                                        {
                                            parameterBody3.type = "TEXT";
                                            parameterBody3.text = templateVariables.VarThree;
                                            componentBody.parameters.Add(parameterBody3);

                                            if (objWhatsAppTemplateModel.VariableCount >= 4)
                                            {
                                                parameterBody4.type = "TEXT";
                                                parameterBody4.text = templateVariables.VarFour;
                                                componentBody.parameters.Add(parameterBody4);

                                                if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                {
                                                    parameterBody5.type = "TEXT";
                                                    parameterBody5.text = templateVariables.VarFive;
                                                    componentBody.parameters.Add(parameterBody5);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        
                            componentBody.type = item.type;

                            components.Add(componentBody);
                        }
                    }

                }
                whatsAppLanguageModel.code = objWhatsAppTemplateModel.language;
                whatsAppTemplateModel.language = whatsAppLanguageModel;
                whatsAppTemplateModel.name = objWhatsAppTemplateModel.name;
                whatsAppTemplateModel.components = components;
                postMessageTemplateModel.template = whatsAppTemplateModel;
                postMessageTemplateModel.to = contact.PhoneNumber;
                //postMessageTemplateModel.to = "962786464718";
                postMessageTemplateModel.messaging_product = "whatsapp";
                postMessageTemplateModel.type = "template";
                return postMessageTemplateModel;
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

        private async Task<WhatsAppMessageTemplateModel> getTemplatesFromWA(TenantModel tenant)
        {
            
            var httpClient = new HttpClient();
            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + tenant.WhatsAppAccountID + "/message_templates?limit=250";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

            var response = await httpClient.GetAsync(postUrl);
            var content = response.Content;
            try
            {
                var WhatsAppTemplate = await content.ReadAsStringAsync();
                WhatsAppMessageTemplateModel objWhatsAppTemplate = new WhatsAppMessageTemplateModel();
                objWhatsAppTemplate = JsonConvert.DeserializeObject<WhatsAppMessageTemplateModel>(WhatsAppTemplate);
                return objWhatsAppTemplate;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task<MessageTemplateModel> getTemplateByWhatsAppId(TenantModel tenant,string templateId)
        {
            var httpClient = new HttpClient();
            var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + templateId;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

            var response = await httpClient.GetAsync(postUrl);
            var content = response.Content;
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
        public async Task SyncTemplateAsync()
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId.Value);

            WhatsAppMessageTemplateModel whatsAppMessageTemplateModel = new WhatsAppMessageTemplateModel();
            whatsAppMessageTemplateModel = await getTemplatesFromWA(tenant);

            List<MessageTemplateModel> messageTemplateModel = new List<MessageTemplateModel>();
            messageTemplateModel = getWhatsAppTemplate();

            List<MessageTemplateModel> TempMessageTemplateModel = new List<MessageTemplateModel>();
            foreach (var item in whatsAppMessageTemplateModel.data)
            {

                if (!messageTemplateModel.Any(x => x.id == item.id))
                {
                    if (item.name=="booking_template_19" ||item.name=="booking_template_ar_19"||item.name=="reminder_booking_19"||item.name=="reminder_booking_ar_19")
                    {
                        item.VariableCount=1;
                    }
                    addWhatsAppMessageTemplate(item);
                }     
            }
      
        }
        private long addWhatsAppMessageTemplate(MessageTemplateModel messageTemplateModel)
        {
            try
            {
                if (messageTemplateModel.TenantId == 0)
                {
                    messageTemplateModel.TenantId = AbpSession.TenantId.Value;
                }
                    var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesAdd;
                if (messageTemplateModel.TenantId != 0)
                {

                
                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                         new System.Data.SqlClient.SqlParameter("@TemplateName",messageTemplateModel.name)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateLanguage",messageTemplateModel.language)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateComponent",JsonConvert.SerializeObject(messageTemplateModel.components))
                        ,new System.Data.SqlClient.SqlParameter("@TemplateStatus",messageTemplateModel.status)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateCategory",messageTemplateModel.category)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateSubCategory",messageTemplateModel.sub_category)
                        ,new System.Data.SqlClient.SqlParameter("@WhatsAppTemplateId",messageTemplateModel.id)
                        ,new System.Data.SqlClient.SqlParameter("@MediaType",messageTemplateModel.mediaType)
                        ,new System.Data.SqlClient.SqlParameter("@MediaLink",messageTemplateModel.mediaLink)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",messageTemplateModel.TenantId)
                        ,new System.Data.SqlClient.SqlParameter("@VariableCount",messageTemplateModel.VariableCount)
                        ,new System.Data.SqlClient.SqlParameter("@BtnOneActionId",messageTemplateModel.BtnOneActionId)
                        ,new System.Data.SqlClient.SqlParameter("@BtnTwoActionId",messageTemplateModel.BtnTwoActionId)
                        ,new System.Data.SqlClient.SqlParameter("@BtnThreeActionId",messageTemplateModel.BtnThreeActionId)
                    };

                    var OutputParameter = new System.Data.SqlClient.SqlParameter();
                    OutputParameter.SqlDbType = SqlDbType.BigInt;
                    OutputParameter.ParameterName = "@TemplateId";
                    OutputParameter.Direction = ParameterDirection.Output;
                    sqlParameters.Add(OutputParameter);

                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                    return (long)OutputParameter.Value;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateWhatsAppMessageTemplate(MessageTemplateModel messageTemplateModel)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplateUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@Id",messageTemplateModel.LocalTemplateId)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateComponent",JsonConvert.SerializeObject(messageTemplateModel.components))
                    ,new System.Data.SqlClient.SqlParameter("@TemplateStatus",Enum.GetName(typeof(WhatsAppTemplateStatusEnum), WhatsAppTemplateStatusEnum.PENDING))
                        ,new System.Data.SqlClient.SqlParameter("@TemplateCategory",messageTemplateModel.category)
                     ,new System.Data.SqlClient.SqlParameter("@TemplateSubCategory",messageTemplateModel.sub_category)
                    ,new System.Data.SqlClient.SqlParameter("@MediaType",messageTemplateModel.mediaType)
                    ,new System.Data.SqlClient.SqlParameter("@MediaLink",messageTemplateModel.mediaLink)
                    ,new System.Data.SqlClient.SqlParameter("@VariableCount",messageTemplateModel.VariableCount)
                    ,new System.Data.SqlClient.SqlParameter("@BtnOneActionId",messageTemplateModel.BtnOneActionId)
                    ,new System.Data.SqlClient.SqlParameter("@BtnTwoActionId",messageTemplateModel.BtnTwoActionId)
                    ,new System.Data.SqlClient.SqlParameter("@BtnThreeActionId",messageTemplateModel.BtnThreeActionId)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        private string deleteWhatsAppMessageTemplate(string templateName)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesDelete;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@TemplateName",templateName)

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);
                return "deleted";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool sendCampaignValidation()
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppSendCampaignValidationGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;

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
        private long sendMessageTemplate(WhatsAppContactsDto contactsFilters, long templateId, long campaignId, bool IsContact, string parameters= "")
        {
            try
            {
                Guid guidId = Guid.NewGuid();
                SetSendCampaignInQueue(
                    new WhatsAppFunModel() 
                    { 
                        whatsAppContactsDto=contactsFilters,
                        templateId=templateId,
                        campaignId=campaignId,
                        TenantId = AbpSession.TenantId.Value,
                        UserId = AbpSession.UserId.Value.ToString(),
                        GuidId = guidId,
                        IsContact=IsContact,
                        Parameters=parameters
                    });
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
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
                                result = result+"\n\r" + (i+1)+"-"+item.buttons[i].text;
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

        #endregion


        #region Campaign Methods
        private static string NumberToWord(int number)
        {
            return number switch
            {
                1 => "One",
                2 => "Two",
                3 => "Three",
                4 => "Four",
                5 => "Five",
                6 => "Six",
                7 => "Seven",
                8 => "Eight",
                9 => "Nine",
                10 => "Ten",
                11 => "Eleven",
                12 => "Twelve",
                13 => "Thirteen",
                14 => "Fourteen",
                15 => "Fifteen",
                _ => throw new ArgumentOutOfRangeException(nameof(number))
            };
        }

        private static List<int> ExtractPlaceholderNumbers(string text)
        {
            var numbers = new List<int>();
            var pattern = @"\{\{(\d+)\}\}";  // Captures the digits inside double curly braces

            var matches = Regex.Matches(text, pattern);

            foreach (Match match in matches)
            {
                if (int.TryParse(match.Groups[1].Value, out int number))
                {
                    numbers.Add(number);
                }
            }

            return numbers;
        }
        private ApiResponse<long> titleCompaignCheck(string title)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_TitleCompaignCheck;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value) ,
                    new System.Data.SqlClient.SqlParameter("@title",title)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long Result = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
                if (Result == 0)
                {
                    return new ApiResponse<long> { Data = 0, ErrorEn = "OK", IsSuccess = true };
                }
                else
                {
                    return new ApiResponse<long> { Data = Result, ErrorEn = "Title of compaign is used before", IsSuccess = false };
                }
            }
            catch (Exception ex) 
            {
                return new ApiResponse<long> { Data = 0, ErrorEn = ex.Message, IsSuccess = false };
            }
        }
        private async Task<CampaignStatisticsDto> getDetailsWhatsAppCampaign(long campaignId)
        {
            try
            {
                CampaignStatisticsDto campaignStatisticsModel = new CampaignStatisticsDto();

                //int pageSize = int.MaxValue;
                //int pageNumber = 0;

                ////get form cosmodb for statistic Campaign
                //var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                //var campaignCosmoResult = await campaignCosmoDBModel.GetItemsRAsync(a =>
                //                a.tenantId == AbpSession.TenantId.Value
                //                && a.campaignId == campaignId.ToString()
                //                && a.itemType == 5
                //            , null, pageSize, pageNumber, x => x.phoneNumber);



                List<CampaginMongoModel> model = new List<CampaginMongoModel>();

                // Connection string from Azure Cosmos DB for MongoDB (vCore)
                string connectionString = AppSettingsModel.connectionStringMongoDB;

                // MongoDB database and collection details

                string databaseName = AppSettingsModel.databaseName;
                string collectionName = AppSettingsModel.collectionName;


                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Get the database
                var database = client.GetDatabase(databaseName);

                // Get the collection
                var collection = database.GetCollection<CampaginMongoModel>(collectionName);


                // Build the filter
                var filter = Builders<CampaginMongoModel>.Filter.Eq("campaignId", campaignId);

                try
                {
                    // Find the first matching document
                    var filterResult = await collection.Find(filter).ToListAsync();

                    model=filterResult;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }





                var campaignCosmo = model;


                if (campaignCosmo.Count > 0)
                {
                    campaignStatisticsModel.title = "ssss";// campaignCosmo[0].campaignName;
                    foreach (var item in campaignCosmo)
                    {
                        if (item.is_read) { campaignStatisticsModel.TotalRead += 1; }
                        if (item.is_delivered) { campaignStatisticsModel.TotalDelivered += 1; }
                        if (item.is_sent) { campaignStatisticsModel.TotalSent += 1; }
                        if (!item.is_read && !item.is_delivered && !item.is_sent) { campaignStatisticsModel.TotalFailed += 1; }
                        if (item.is_accepted) { campaignStatisticsModel.TotalNumbers += 1; }
                        if (!item.is_read && !item.is_delivered && !item.is_sent&& !item.is_accepted) { campaignStatisticsModel.TotalNumbers += 1; }
                        // if (item.isReplied) { campaignStatisticsModel.TotalReplied += 1; }
                    }
                    if (campaignStatisticsModel.TotalNumbers > 0)
                    {
                        campaignStatisticsModel.TotalProcessing = (campaignStatisticsModel.TotalNumbers - campaignStatisticsModel.TotalDelivered) - campaignStatisticsModel.TotalFailed;
                        campaignStatisticsModel.TotalNumbers = campaignStatisticsModel.TotalNumbers;

                        if (campaignStatisticsModel.TotalNumbers > 0)
                        {
                            campaignStatisticsModel.SentPercentage = (campaignStatisticsModel.TotalSent > 0) ? ((float)campaignStatisticsModel.TotalSent / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                            campaignStatisticsModel.DeliveredPercentage = (campaignStatisticsModel.TotalDelivered > 0) ? ((float)campaignStatisticsModel.TotalDelivered / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                            campaignStatisticsModel.ReadPercentage = (campaignStatisticsModel.TotalRead > 0) ? ((float)campaignStatisticsModel.TotalRead / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                            campaignStatisticsModel.RepliedPercentage = (campaignStatisticsModel.TotalReplied > 0) ? ((float)campaignStatisticsModel.TotalReplied / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                            campaignStatisticsModel.ProcessingPercentage = (campaignStatisticsModel.TotalProcessing > 0) ? ((float)campaignStatisticsModel.TotalProcessing / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                            campaignStatisticsModel.FailedPercentage = (campaignStatisticsModel.TotalFailed > 0) ? ((float)campaignStatisticsModel.TotalFailed / campaignStatisticsModel.TotalNumbers) * 100 : 0;
                        }
                    }
                }


                if (campaignCosmo.Count==0)
                {
                    try
                    {
                        CampaignStatisticsDto campaignStatisticsModel2 = new CampaignStatisticsDto();

                        int pageSize = int.MaxValue;
                        int pageNumber = 0;

                        //get form cosmodb for statistic Campaign
                        var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                        var campaignCosmoResult = await campaignCosmoDBModel.GetItemsRAsync(a =>
                                        a.tenantId == AbpSession.TenantId.Value
                                        && a.campaignId == campaignId.ToString()
                                        && a.itemType == 5
                                    , null, pageSize, pageNumber, x => x.phoneNumber);

                        var campaignCosmo2 = campaignCosmoResult.Item1.ToList();


                        if (campaignCosmo2.Count > 0)
                        {
                            campaignStatisticsModel2.title = campaignCosmo2[0].campaignName;
                            foreach (var item in campaignCosmo2)
                            {
                                if (item.isRead) { campaignStatisticsModel2.TotalRead += 1; }
                                if (item.isDelivered) { campaignStatisticsModel2.TotalDelivered += 1; }
                                if (item.isSent) { campaignStatisticsModel2.TotalSent += 1; }
                                if (item.isFailed) { campaignStatisticsModel2.TotalFailed += 1; }
                                if (item.isReplied) { campaignStatisticsModel2.TotalReplied += 1; }
                            }
                            if (campaignStatisticsModel2.TotalSent > 0)
                            {
                                campaignStatisticsModel2.TotalProcessing = (campaignStatisticsModel2.TotalSent - campaignStatisticsModel2.TotalDelivered) - campaignStatisticsModel2.TotalFailed;
                                campaignStatisticsModel2.TotalNumbers = campaignStatisticsModel2.TotalSent;

                                if (campaignStatisticsModel2.TotalNumbers > 0)
                                {
                                    campaignStatisticsModel2.SentPercentage = (campaignStatisticsModel2.TotalSent > 0) ? ((float)campaignStatisticsModel2.TotalSent / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                    campaignStatisticsModel2.DeliveredPercentage = (campaignStatisticsModel2.TotalDelivered > 0) ? ((float)campaignStatisticsModel2.TotalDelivered / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                    campaignStatisticsModel2.ReadPercentage = (campaignStatisticsModel2.TotalRead > 0) ? ((float)campaignStatisticsModel2.TotalRead / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                    campaignStatisticsModel2.RepliedPercentage = (campaignStatisticsModel2.TotalReplied > 0) ? ((float)campaignStatisticsModel2.TotalReplied / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                    campaignStatisticsModel2.ProcessingPercentage = (campaignStatisticsModel2.TotalProcessing > 0) ? ((float)campaignStatisticsModel2.TotalProcessing / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                    campaignStatisticsModel2.FailedPercentage = (campaignStatisticsModel2.TotalFailed > 0) ? ((float)campaignStatisticsModel2.TotalFailed / campaignStatisticsModel2.TotalNumbers) * 100 : 0;
                                }
                            }
                        }

                        return campaignStatisticsModel2;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                }








                return campaignStatisticsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //try
            //{
            //    CampaignStatisticsDto campaignStatisticsModel = new CampaignStatisticsDto();




            //    var campaignCosmoDBModel = new DocumentCosmoseDB<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            //    try
            //    {
            //        campaignStatisticsModel.TotalRead=await campaignCosmoDBModel.GetItemCountAsync(a => a.tenantId == AbpSession.TenantId.Value&& (a.campaignId == campaignId.ToString() || campaignId == 0) && a.isRead);
            //        campaignStatisticsModel.TotalDelivered=await campaignCosmoDBModel.GetItemCountAsync(a => a.tenantId == AbpSession.TenantId.Value&& (a.campaignId == campaignId.ToString() || campaignId == 0)  && a.isDelivered);
            //        campaignStatisticsModel.TotalSent=await campaignCosmoDBModel.GetItemCountAsync(a => a.tenantId == AbpSession.TenantId.Value&& (a.campaignId == campaignId.ToString() || campaignId == 0)  && a.isSent);
            //        campaignStatisticsModel.TotalFailed=await campaignCosmoDBModel.GetItemCountAsync(a => a.tenantId == AbpSession.TenantId.Value&& (a.campaignId == campaignId.ToString() || campaignId == 0)  && a.isFailed);
            //        campaignStatisticsModel.TotalReplied=await campaignCosmoDBModel.GetItemCountAsync(a => a.tenantId == AbpSession.TenantId.Value&& (a.campaignId == campaignId.ToString() || campaignId == 0) && a.isReplied);

            //    }
            //    catch
            //    {

            //    }



            //    if (campaignStatisticsModel.TotalSent > 0)
            //    {
            //        campaignStatisticsModel.TotalProcessing = (campaignStatisticsModel.TotalSent - campaignStatisticsModel.TotalDelivered) - campaignStatisticsModel.TotalFailed;
            //        campaignStatisticsModel.TotalNumbers = campaignStatisticsModel.TotalSent;

            //        if (campaignStatisticsModel.TotalNumbers > 0)
            //        {
            //            campaignStatisticsModel.SentPercentage = (campaignStatisticsModel.TotalSent > 0 )? ((float)campaignStatisticsModel.TotalSent / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //            campaignStatisticsModel.DeliveredPercentage = (campaignStatisticsModel.TotalDelivered > 0) ? ((float)campaignStatisticsModel.TotalDelivered / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //            campaignStatisticsModel.ReadPercentage = (campaignStatisticsModel.TotalRead > 0) ?  ((float)campaignStatisticsModel.TotalRead / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //            campaignStatisticsModel.RepliedPercentage = (campaignStatisticsModel.TotalReplied > 0) ?  ((float)campaignStatisticsModel.TotalReplied / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //            campaignStatisticsModel.ProcessingPercentage = (campaignStatisticsModel.TotalProcessing > 0) ?  ((float)campaignStatisticsModel.TotalProcessing / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //            campaignStatisticsModel.FailedPercentage = (campaignStatisticsModel.TotalFailed > 0) ?  ((float)campaignStatisticsModel.TotalFailed / campaignStatisticsModel.TotalNumbers) * 100 : 0;
            //        }
            //    }                    


            //    return campaignStatisticsModel;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        private WhatsAppEntity getWhatsAppCampaign(int tenantId, int pageNumber = 0, int pageSize = 50, int type = 1)
        {
            try
            {
                WhatsAppEntity campaignEntity = new WhatsAppEntity();
                List<WhatsAppCampaignModel> lstWhatsAppCampaignModel = new List<WhatsAppCampaignModel>();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                      ,new System.Data.SqlClient.SqlParameter("@Type",type)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);
                lstWhatsAppCampaignModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),DataReaderMapper.MapCampaign, AppSettingsModel.ConnectionStrings).ToList();


                campaignEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                campaignEntity.lstWhatsAppCampaignModel = lstWhatsAppCampaignModel;
                return campaignEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WhatsAppCampaignModel getWhatsAppCampaignByName(string title,int tenantId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignByNameGet;

                WhatsAppCampaignModel campaign = new WhatsAppCampaignModel();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> 
                { 
                    new System.Data.SqlClient.SqlParameter("@Title", title) ,
                    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId) ,
                };
                campaign = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCampaign, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return campaign;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private WhatsAppEntity getWhatsAppCampaignHistory(long campaignId)
        {
            try
            {
                WhatsAppEntity campaignHistoryEntity = new WhatsAppEntity();
                List<WhatsAppCampaignHistoryModel> lstWhatsAppCampaignHistoryModel = new List<WhatsAppCampaignHistoryModel>();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignHistoryGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)

                };
                lstWhatsAppCampaignHistoryModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapCampaignHistory, AppSettingsModel.ConnectionStrings).ToList();
                campaignHistoryEntity.lstwhatsAppCampaignHistoryModels = lstWhatsAppCampaignHistoryModel;
                return campaignHistoryEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long addWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            try
            {

                int tenantId = AbpSession.TenantId.Value;

                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@CampaignTilte",whatsAppCampaignModel.Title)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignLanguage",whatsAppCampaignModel.Language)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",whatsAppCampaignModel.TemplateId)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignType",1)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId)
                    ,new System.Data.SqlClient.SqlParameter("@Type",whatsAppCampaignModel.Type)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@CampaignId";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (long)OutputParameter.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CampinToQueueDto membersGet(long groupId)
        {
            try
            {
                CampinToQueueDto campinToQueueDto = new CampinToQueueDto();
                campinToQueueDto.contacts = new List<ListContactToCampin>();
                var SP_Name = Constants.Groups.SP_MembersGetAllForCamp;
                int tenantId = AbpSession.TenantId.Value;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@groupId",groupId) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                campinToQueueDto.contacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapMembersForCamp, AppSettingsModel.ConnectionStrings).ToList();
                campinToQueueDto.TotalCount = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;
                campinToQueueDto.TotalOptOut =  campinToQueueDto.contacts.Count;

                return campinToQueueDto;
            }
            catch
            {
                return new CampinToQueueDto();
            }
        }
        private SendCampinStatesModel sendCampignFromGroup(CampinToQueueDto contactsEntity)
        {
            SendCampinStatesModel sendCampinStatesModel = new SendCampinStatesModel();
            try
            {
                var dailyLimit = getDailylimitCount();

                #region Get ContactFrom Group

                contactsEntity.contacts = GetContactFromGroup(contactsEntity.groupId);
                #endregion

                if (dailyLimit.DailyLimit >= contactsEntity.contacts.Count)
                {
                    #region Send Campign
                    int tenantId = AbpSession.TenantId.Value;

                    #region Wallet Information
                    var walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                    if (walletModel == null)
                    {
                        _walletAppService.CreateWallet(tenantId);
                        walletModel = _tenantDashboardAppService.WalletGetByTenantId(tenantId);
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    else if (walletModel.TotalAmount <= 0)
                    {
                        sendCampinStatesModel.Message = "You don't have enough Funds";
                        sendCampinStatesModel.status = false;
                        return sendCampinStatesModel;
                    }
                    #endregion

                    #region check if you hava Funds gretar than or equal convarsaction prise 

                    double Price = 0.014;
                    decimal totalPrice = 0;
                    if (walletModel.TotalAmount > 0)
                    {
                        totalPrice = contactsEntity.contacts.Count * (decimal)Price;
                        //UTILITY MARKETING
                        if (walletModel.TotalAmount >= totalPrice)
                        {
                            var tenantInfo = GetTenantInfo(tenantId);
               
                            // Display the count of each split list
                            string sendcompaing = "campaign";
                            int count = 1;
                            var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignAddOnDB;

                            MessageTemplateModel objWhatsAppTemplateModel = getTemplateById(contactsEntity.templateId);
                            MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                            if (templateWA != null && templateWA.status == "APPROVED")
                            {
                                //objWhatsAppTemplateModel.components = templateWA.components;

                                string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);
                                
                                var variables = ExtractPlaceholderNumbers(msg);


                                // Split the list into 5000 smaller lists
                                List<List<ListContactToCampin>> splitLists = SplitListD(contactsEntity.contacts, 5000);

                                CampinQueueNew campinQueueNew = new CampinQueueNew();
                             
                                foreach (var OuterList in splitLists)
                                {
                                    foreach (var contact in OuterList)
                                    {
                                        contact.haderVariablesTemplate = contactsEntity.headerVariabllesTemplate;
                                        contact.firstButtonURLVariabllesTemplate = contactsEntity.firstButtonURLVariabllesTemplate;
                                        contact.secondButtonURLVariabllesTemplate = contactsEntity.secondButtonURLVariabllesTemplate;
                                        contact.carouselVariabllesTemplate = contactsEntity.CarouselTemplate;
                                        contact.buttonCopyCodeVariabllesTemplate = contactsEntity.buttonCopyCodeVariabllesTemplate;

                                        if (templateWA.components.Any(x => x.type.ToUpper() == "CAROUSEL"))
                                        {
                                            contact.templateVariables = contactsEntity.templateVariables;
                                        }
                                        else
                                        {
                                            var templateVars = new TemplateVariablles();

                                            foreach (int varNum in Enumerable.Range(1, 15))
                                            {
                                                string key = $"var{varNum}";
                                                string propName = $"Var{NumberToWord(varNum)}";

                                                var prop = typeof(TemplateVariablles).GetProperty(propName);
                                                if (prop != null && prop.CanWrite)
                                                {
                                                    // Assign value only if it appears in message, otherwise null
                                                    string value = variables.Contains(varNum) && contact.variables.TryGetValue(key, out var val) ? val : null;
                                                    prop.SetValue(templateVars, value);
                                                }
                                            }

                                            contact.templateVariables = templateVars;
                                        }
                                    }


                                    var JopName = sendcompaing + count.ToString();
                                    if (OuterList.Count == 0)
                                    {
                                        break;
                                    }
                                    string str = JsonConvert.SerializeObject(OuterList);
                                    string TemplateJson = JsonConvert.SerializeObject(objWhatsAppTemplateModel);
                                    string OuterListjson = JsonConvert.SerializeObject(OuterList);

                                    if (contactsEntity.templateVariables==null)
                                    {
                                        contactsEntity.templateVariables=new TemplateVariablles();
                                    }
                                    string TemplateVariablesJson = JsonConvert.SerializeObject(contactsEntity.templateVariables);


                                    TemplateVariablles templateVariables = null;
                                    HeaderVariablesTemplate headerVariabllesTemplate = null;
                                    FirstButtonURLVariabllesTemplate firstButtonURLVariabllesTemplate = null;
                                    SecondButtonURLVariabllesTemplate secondButtonURLVariabllesTemplate = null;
                                    CarouselVariabllesTemplate carouselVariabllesTemplate = null;
                                    ButtonCopyCodeVariabllesTemplate buttonCopyCodeVariabllesTemplate = null;
                                    if (contactsEntity.templateVariables != null)
                                    {
                                        templateVariables = contactsEntity.templateVariables;
                                    }
                                    if (contactsEntity.headerVariabllesTemplate != null)
                                    {
                                        headerVariabllesTemplate = contactsEntity.headerVariabllesTemplate;
                                    }
                                    if (contactsEntity.firstButtonURLVariabllesTemplate != null)
                                    {
                                        firstButtonURLVariabllesTemplate = contactsEntity.firstButtonURLVariabllesTemplate;
                                    }
                                    if (contactsEntity.secondButtonURLVariabllesTemplate != null)
                                    {
                                        secondButtonURLVariabllesTemplate = contactsEntity.secondButtonURLVariabllesTemplate;
                                    }
                                    if (contactsEntity.CarouselTemplate != null)
                                    {
                                        carouselVariabllesTemplate = contactsEntity.CarouselTemplate;
                                    }
                                    if (contactsEntity.buttonCopyCodeVariabllesTemplate != null)
                                    {
                                        buttonCopyCodeVariabllesTemplate = contactsEntity.buttonCopyCodeVariabllesTemplate;
                                    }
                                    //string str = JsonConvert.SerializeObject(OuterList);
                                    string templateVariablesjson = JsonConvert.SerializeObject(templateVariables);
                                    string headerVarjson = JsonConvert.SerializeObject(headerVariabllesTemplate);
                                    string firstURLVariabllesjson = JsonConvert.SerializeObject(firstButtonURLVariabllesTemplate);
                                    string secURLVariabllesjson = JsonConvert.SerializeObject(secondButtonURLVariabllesTemplate);
                                    string carouselVariabllesjson = JsonConvert.SerializeObject(carouselVariabllesTemplate);
                                    string copyCodeVariabllesjson = JsonConvert.SerializeObject(buttonCopyCodeVariabllesTemplate);

                                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                                         new System.Data.SqlClient.SqlParameter("@Contacts",str)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignId",contactsEntity.campaignId)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateId",contactsEntity.templateId)
                                        ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                                        ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                                        ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",contactsEntity.IsExternal)
                                        ,new System.Data.SqlClient.SqlParameter("@JopName",JopName)
                                        ,new System.Data.SqlClient.SqlParameter("@TemplateName",contactsEntity.templateName)
                                        ,new System.Data.SqlClient.SqlParameter("@CampaignName",contactsEntity.campaignName)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateJson",TemplateJson)
                                         ,new System.Data.SqlClient.SqlParameter("@TemplateVariablesJson",TemplateVariablesJson)
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

                                    count++;
                                    if (OutputParameter.Value != DBNull.Value)
                                    {
                                        if (JopName=="campaign1")
                                        {
                                           
                                            campinQueueNew.messageTemplateModel = objWhatsAppTemplateModel;
                                            campinQueueNew.campaignId = contactsEntity.campaignId;
                                            campinQueueNew.templateId = contactsEntity.templateId;
                                            campinQueueNew.IsExternal = contactsEntity.IsExternal;
                                            campinQueueNew.TenantId = tenantInfo.TenantId;
                                            campinQueueNew.D360Key = tenantInfo.D360Key;
                                            campinQueueNew.AccessToken = tenantInfo.AccessToken;
                                            campinQueueNew.functionName = JopName;
                                            campinQueueNew.msg = msg;
                                            campinQueueNew.type = type;
                                            campinQueueNew.contacts = null;
                                            campinQueueNew.templateVariables = contactsEntity.templateVariables;
                                            campinQueueNew.campaignName = contactsEntity.campaignName;
                                            campinQueueNew.rowId = Convert.ToInt64(OutputParameter.Value);
                                            //SetCampinQueueContact(campinQueueNew);
                                            //SetCampinInFun(campinQueueNew);
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                SetCampinInFun(campinQueueNew);
                                #region Add in transaction table 

                                TransactionModel transactionModel = new TransactionModel();

                                var usersDashModel = _tenantDashboardAppService.GetUserInfo((long)AbpSession.UserId.Value);
                                transactionModel.DoneBy = usersDashModel.Name;
                                transactionModel.TotalTransaction = totalPrice;
                                transactionModel.TotalRemaining = walletModel.TotalAmount - totalPrice;
                                transactionModel.TransactionDate = DateTime.UtcNow;
                                transactionModel.CategoryType = objWhatsAppTemplateModel.category;
                                transactionModel.TenantId = tenantInfo.TenantId;

                                var result = addTransaction(transactionModel, contactsEntity.contacts.Count, objWhatsAppTemplateModel.name, contactsEntity.campaignId);

                                #endregion

                                #region statistic Campaign
                                WhatsAppCampaignModel statistics = new WhatsAppCampaignModel
                                {
                                    Id = contactsEntity.campaignId,
                                    SentTime = DateTime.UtcNow,
                                    Status = (int)WhatsAppCampaignStatusEnum.Active
                                };
                                updateWhatsAppCampaign(statistics);
                                #endregion

                                sendCampinStatesModel.Message = "Sent Successfully";
                                sendCampinStatesModel.status = true;
                            }
                            else
                            {
                                sendCampinStatesModel.Message = "template Not APPROVED";
                                sendCampinStatesModel.status = false;
                            }
                        }
                        else
                        {
                            sendCampinStatesModel.Message = "You don't have enough Funds";
                            sendCampinStatesModel.status = false;
                        }
                    }
                    #endregion
                    #endregion
                }
                else
                {
                    sendCampinStatesModel.Message = "You have exceeded your daily limit";
                    sendCampinStatesModel.status = false;
                }
            }
            catch (Exception ex)
            {
                sendCampinStatesModel.Message = ex.Message;
                sendCampinStatesModel.status = false;
            }
            return sendCampinStatesModel;
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
        private SendCampinStatesModel sendCampignFromGroupScheduled(CampinToQueueDto contactsEntity, string sendTime)
        {
            try
            {
                var dailyLimit = getDailylimitCount();

                #region Get ContactFrom Group
                contactsEntity.contacts = GetContactFromGroup(contactsEntity.groupId);
                #endregion

                if (dailyLimit.DailyLimit >= contactsEntity.contacts.Count)
                {
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

                    totalPrice = contactsEntity.contacts.Count * (decimal)Price;
                    //UTILITY MARKETING
                    if (walletModel.TotalAmount >= totalPrice)
                    {
                        var tenantInfo = GetTenantInfo(tenantId);

                        var category = GetTemplatesCategory(contactsEntity.templateId);
                        List<ListContactToCampin> contact = new List<ListContactToCampin>();


                        foreach (var contacts in contactsEntity.contacts)
                        {

                            if (contactsEntity.templateVariables != null)
                            {
                                contacts.templateVariables = new TemplateVariablles
                                {
                                    VarOne = contactsEntity.templateVariables.VarOne,
                                    VarTwo = contactsEntity.templateVariables.VarTwo,
                                    VarThree = contactsEntity.templateVariables.VarThree,
                                    VarFour = contactsEntity.templateVariables.VarFour,
                                    VarFive = contactsEntity.templateVariables.VarFive,
                                    VarSix = contactsEntity.templateVariables.VarSix,
                                    VarSeven = contactsEntity.templateVariables.VarSeven,
                                    VarEight = contactsEntity.templateVariables.VarEight,
                                    VarNine = contactsEntity.templateVariables.VarNine,
                                    VarTen = contactsEntity.templateVariables.VarTen,
                                    VarEleven = contactsEntity.templateVariables.VarEleven,
                                    VarTwelve = contactsEntity.templateVariables.VarTwelve,
                                    VarThirteen = contactsEntity.templateVariables.VarThirteen,
                                    VarFourteen = contactsEntity.templateVariables.VarFourteen,
                                    VarFifteen = contactsEntity.templateVariables.VarFifteen
                                };
                            }
                            else
                            {

                                contacts.templateVariables = new TemplateVariablles
                                {
                                    VarOne = null,
                                    VarTwo = null,
                                    VarThree = null,
                                    VarFour = null,
                                    VarFive = null,
                                    VarSix = null,
                                    VarSeven = null,
                                    VarEight = null,
                                    VarNine = null,
                                    VarTen = null,
                                    VarEleven = null,
                                    VarTwelve = null,
                                    VarThirteen = null,
                                    VarFourteen = null,
                                    VarFifteen = null
                                };

                            }

                            if (contactsEntity.headerVariabllesTemplate != null)
                            {
                                contacts.haderVariablesTemplate = new HeaderVariablesTemplate
                                {
                                    VarOne = contactsEntity.headerVariabllesTemplate.VarOne
                                };
                            }
                            else
                            {
                                contacts.haderVariablesTemplate = new HeaderVariablesTemplate
                                {
                                    VarOne = null
                                };
                            }
                            if (contactsEntity.firstButtonURLVariabllesTemplate != null)
                            {
                                contacts.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate
                                {
                                    VarOne = contactsEntity.firstButtonURLVariabllesTemplate.VarOne
                                };
                            }
                            else
                            {
                                contacts.firstButtonURLVariabllesTemplate = new FirstButtonURLVariabllesTemplate
                                {
                                    VarOne = null
                                };
                            }
                            if (contactsEntity.secondButtonURLVariabllesTemplate != null)
                            {
                                contacts.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate
                                {
                                    VarOne = contactsEntity.secondButtonURLVariabllesTemplate.VarOne
                                };
                            }
                            else
                            {
                                contacts.secondButtonURLVariabllesTemplate = new SecondButtonURLVariabllesTemplate
                                {
                                    VarOne = null
                                };
                            }
                            if (contactsEntity.CarouselTemplate != null)
                            {
                                contacts.carouselVariabllesTemplate = new CarouselVariabllesTemplate
                                {
                                    cards = contactsEntity.CarouselTemplate.cards
                                };
                            }
                            else{
                            contacts.carouselVariabllesTemplate = new CarouselVariabllesTemplate
                                {
                                    cards = null
                                };
                            }
                            if (contactsEntity.buttonCopyCodeVariabllesTemplate != null)
                            {
                                contacts.buttonCopyCodeVariabllesTemplate = new ButtonCopyCodeVariabllesTemplate
                                {
                                    VarOne = contactsEntity.buttonCopyCodeVariabllesTemplate.VarOne
                                };
                            }
                            else
                            {
                                contacts.buttonCopyCodeVariabllesTemplate = new ButtonCopyCodeVariabllesTemplate
                                {
                                    VarOne = contactsEntity.buttonCopyCodeVariabllesTemplate.VarOne
                                };
                            }
                            contact.Add(contacts);
                        }

                        contactsEntity.contacts = contact;
                        long returnValue = addScheduledCampaignonOnDB(contactsEntity, sendTime);
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

                            var result = addTransaction(transactionModel, contactsEntity.contacts.Count, contactsEntity.templateName, contactsEntity.campaignId);
                            #endregion

                            #region statistic Campaign
                            DateTime dateTimes;
                            if (DateTime.TryParseExact(sendTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimes))
                            {
                                dateTimes = dateTimes.AddHours(AppSettingsModel.DivHour);
                            }

                            WhatsAppCampaignModel statistics = new WhatsAppCampaignModel
                            {
                                Id = contactsEntity.campaignId,
                                SentTime = dateTimes,
                                Status = (int)WhatsAppCampaignStatusEnum.Scheduled
                            };
                            updateWhatsAppCampaign(statistics);
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
                                status = false
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
                else
                {
                    return new SendCampinStatesModel()
                    {
                        Message = "You have exceeded your daily limit",
                        status = false
                    };
                }
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
        private List<ListContactToCampin> GetContactFromGroup(long groupId)
        {
            try
            {
                List<ListContactToCampin> listContactToCampins = new List<ListContactToCampin>();
                var SP_Name = Constants.Contacts.SP_ContactGetFromGroup;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@groupId",groupId)
                };

                listContactToCampins  = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MaContactsFromGrouyps, AppSettingsModel.ConnectionStrings).ToList();

                return listContactToCampins;
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
        /// Add as External Contact
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task addBulkAsExternalContact(SendCampignFromGroupDto input)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_SendCampaignFromGroup;

                string @membersJson = JsonConvert.SerializeObject(input.listContact);

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",input.campaignId)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",input.templateId)
                    ,new System.Data.SqlClient.SqlParameter("@membersJson",membersJson)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
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
        private long addScheduledCampaign(WhatsAppContactsDto contacts,string sendDateTime, long campaignId, long templateId,bool isExternalContact)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppScheduledCampaignAdd;

                DateTime dateTime;
                if (DateTime.TryParseExact(sendDateTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    // Date string is valid
                    dateTime = dateTime.AddHours(AppSettingsModel.DivHour);
                }
                else
                {
                    // Date string is not valid
                    return -1;  
                }
                //DateTime dateTime = DateTime.Parse(sendDateTime, CultureInfo.InvariantCulture);
                //dateTime = dateTime.AddHours(AppSettingsModel.DivHour);
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                     new System.Data.SqlClient.SqlParameter("@Contacts",JsonConvert.SerializeObject(contacts))
                    ,new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                    ,new System.Data.SqlClient.SqlParameter("@TemplateId",templateId)
                    ,new System.Data.SqlClient.SqlParameter("@SendTime",dateTime)
                    ,new System.Data.SqlClient.SqlParameter("@StatusId",WhatsAppCampaignStatusEnum.Scheduled)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                    ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                    ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",isExternalContact)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "Id",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                var result = (long)OutputParameter.Value;
                if (result > 0)
                {
                    WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
                    {
                        Id = campaignId,
                        Status = (int)WhatsAppCampaignStatusEnum.Scheduled,
                        SentTime = dateTime,
                    };

                    updateWhatsAppCampaign(whatsAppCampaignModel);
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long addScheduledCampaignonOnDB(CampinToQueueDto contactsEntity, string sendTime)
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

                // Split the list into 10 smaller lists
                List<List<ListContactToCampin>> splitLists = SplitList(contactsEntity.contacts, 10);

                // Display the count of each split list
                string sendcompaing = "SendScheduledCampaign";
                int count = 1;

                var SP_Name = Constants.WhatsAppCampaign.SP_ScheduledCampaignAddOnDB;
                foreach (var OuterList in splitLists)
                {
                    var JopName = sendcompaing + count.ToString();
                    if (OuterList.Count == 0) { break; }

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                         new System.Data.SqlClient.SqlParameter("@Contacts",JsonConvert.SerializeObject(OuterList))
                        ,new System.Data.SqlClient.SqlParameter("@CampaignId",contactsEntity.campaignId)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateId",contactsEntity.templateId)
                        ,new System.Data.SqlClient.SqlParameter("@SendTime",dateTime)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                        ,new System.Data.SqlClient.SqlParameter("@IsExternalContact",contactsEntity.IsExternal)
                        ,new System.Data.SqlClient.SqlParameter("@JopName",JopName)
                        ,new System.Data.SqlClient.SqlParameter("@TemplateName",contactsEntity.templateName)
                        ,new System.Data.SqlClient.SqlParameter("@CampaignName",contactsEntity.campaignName)
                    };
                    var OutputParameter = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.BigInt,
                        ParameterName = "@Id",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(OutputParameter);

                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    count++;
                    if (OutputParameter.Value != DBNull.Value)
                    { continue; }
                    else
                    { return 0; }
                }
                return  1;
            }
            catch (Exception ex)
            {
                throw ex;
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

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void deleteWhatsAppCampaign(long campaignId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignDelete;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)

            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetSendCampaignInQueue(WhatsAppFunModel message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("campaign-sync");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(message);
            }
        }
        private void SetCampinQueue(CampinQueue message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference(message.functionName);
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(message);
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
        private void updateActivationScheduledCampaign(long campaignId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignActivationUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignStatus",WhatsAppCampaignStatusEnum.InActive)
                    ,new System.Data.SqlClient.SqlParameter("@IsActive",false)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //private string HtmlToPlainText(string html)
        //{

        //    html=html.Replace("<strong><em>", "*_");
        //    html=html.Replace("</em></strong>", "_* ");


        //    html=html.Replace("<strong>", "*");
        //    html=html.Replace(" </strong>", "* ");

        //    html=html.Replace("<strong>", "*");
        //    html=html.Replace("</strong>", "* ");


        //    html=html.Replace("<em>", "_");
        //    html=html.Replace(" </em>", "_ ");

        //    html=html.Replace("<em>", "_");
        //    html=html.Replace("</em>", "_ ");
        //    const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
        //    const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
        //    const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        //    var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
        //    var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
        //    var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

        //    var text = html;
        //    //Decode html specific characters
        //    text = System.Net.WebUtility.HtmlDecode(text);
        //    //Remove tag whitespace/line breaks
        //    text = tagWhiteSpaceRegex.Replace(text, "><");
        //    //Replace <br /> with line breaks
        //    text = lineBreakRegex.Replace(text, Environment.NewLine);
        //    //Strip formatting
        //    text = stripFormattingRegex.Replace(text, string.Empty);

        //    return text;
        //}


        //private static string PlainTextTohtml(string text)
        //{
        //    text=text.Replace("*_", " ");
        //    text=text.Replace("_*", " ");
        //    text=text.Replace("*", " ");
        //    text=text.Replace("_", " ");
        //    return text;
        //}
        #endregion

        #region Bot

        private ResultBotReservedWordsModel addBotReservedWord(BotReservedWordsModel model)
        {
            try
            { 
                //In order to find if there are repeated words
                if (model != null)
                {
                    List<string> words = new List<string>();
                    ResultBotReservedWordsModel resultBotReservedWordsModel = new ResultBotReservedWordsModel();
                    List<string> liveChat = new List<string> { "live chat" };
                    List<string> Request = new List<string> { "Request" };

                    var reservedWords = getBotReservedWordsByActionId(model.ActionId, model.TenantId);
                    foreach (var item in reservedWords)
                    {
                        if (model.ActionId == item.ActionId && model.TenantId == item.TenantId)
                        {
                            resultBotReservedWordsModel.Id = 0;
                            if (model.ActionId == 1)
                                resultBotReservedWordsModel.Name = liveChat;
                            else if (model.ActionId == 2)
                                resultBotReservedWordsModel.Name = Request;
                            
                            return resultBotReservedWordsModel;
                        }
                        words.AddRange(item.ButtonText.Split(','));
                    }

                    var newWords = model.ButtonText.Split(',').ToList();
                    List<string> newWordWithOutSpase= new List<string>();
                    foreach (var word in newWords) {
                        newWordWithOutSpase.Add(word.Trim());
                    }
                    if (words.Intersect(newWordWithOutSpase).Any())
                    {
                        resultBotReservedWordsModel.Id = 0;
                        resultBotReservedWordsModel.Name = words.Intersect(newWordWithOutSpase).ToList();
                        return resultBotReservedWordsModel;
                    }
                }

                var SP_Name = Constants.Bot.SP_BotReservedWordsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@ButtonText",model.ButtonText),
                    new System.Data.SqlClient.SqlParameter("@Action",model.Action),
                    new System.Data.SqlClient.SqlParameter("@TenantId",model.TenantId),
                    new System.Data.SqlClient.SqlParameter("@TriggersBot",model.TriggersBot),
                    new System.Data.SqlClient.SqlParameter("@ActionId",model.ActionId),
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@Id",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                ResultBotReservedWordsModel finalresultBotReservedWordsModel = new ResultBotReservedWordsModel();
                finalresultBotReservedWordsModel.Id = (long)OutputParameter.Value;
                return finalresultBotReservedWordsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void deleteBotReservedWord(long Id)
        {
            try
            {
                var SP_Name = Constants.Bot.SP_BotReservedWordsDelete;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@Id",Id)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ResultBotReservedWordsModel updateBotReservedWord(BotReservedWordsModel model)
        {
            try
            {
                //In order to find if there are repeated words
                if (model != null)
                {
                    List<string> words = new List<string>();
                    ResultBotReservedWordsModel resultBotReservedWordsModel = new ResultBotReservedWordsModel();
                    List<string> liveChat = new List<string> { "live chat" };
                    List<string> Request = new List<string> { "Request" };

                    var reservedWords = getBotReservedWordsByActionId(model.ActionId, model.TenantId);
                    foreach (var item in reservedWords)
                    {
                        if (item.Id == model.Id) 
                        {
                            //Because here he adjusts to the same Row
                        }
                        else {
                            if (model.ActionId == item.ActionId && model.TenantId == item.TenantId)
                            {
                                resultBotReservedWordsModel.Id = 0;
                                if (model.ActionId == 1)
                                    resultBotReservedWordsModel.Name = liveChat;
                                else if (model.ActionId == 2)
                                    resultBotReservedWordsModel.Name = Request;

                                return resultBotReservedWordsModel;
                            }
                            words.AddRange(item.ButtonText.Split(','));
                        }
                    }

                    var newWords = model.ButtonText.Split(',').ToList();
                    List<string> newWordWithOutSpase = new List<string>();
                    foreach (var word in newWords)
                    {
                        newWordWithOutSpase.Add(word.Trim());
                    }
                    if (words.Intersect(newWordWithOutSpase).Any())
                    {
                        resultBotReservedWordsModel.Id = 0;
                        resultBotReservedWordsModel.Name = words.Intersect(newWordWithOutSpase).ToList();
                        return resultBotReservedWordsModel;
                    }
                }

                var SP_Name = Constants.Bot.SP_BotReservedWordsUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@Id",model.Id)
                    ,new System.Data.SqlClient.SqlParameter("@ButtonText",model.ButtonText)
                    ,new System.Data.SqlClient.SqlParameter("@Action",model.Action)
                    ,new System.Data.SqlClient.SqlParameter("@TriggersBot",model.TriggersBot)
                    ,new System.Data.SqlClient.SqlParameter("@ActionId",model.ActionId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutputId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                ResultBotReservedWordsModel finalresultBotReservedWordsModel = new ResultBotReservedWordsModel();
                finalresultBotReservedWordsModel.Id = (long)OutputParameter.Value;
                return finalresultBotReservedWordsModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private BotReservedWordsModel getByIdBotReservedWords(long Id)
        {
            try
            {
                BotReservedWordsModel model = new BotReservedWordsModel();
                var SP_Name = Constants.Bot.SP_BotReservedWordsGetById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@Id",Id),
                };


                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBotReservedWords, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<BotReservedWordsModel> getBotReservedWordsByActionId(long actionId,int tenantId)
        {
            try
            {
                List<BotReservedWordsModel> model = new List<BotReservedWordsModel>();
                var SP_Name = Constants.Bot.SP_BotReservedWordByActionIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@ActionId",actionId),
                   new System.Data.SqlClient.SqlParameter("@TenantId",tenantId),
                };


                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBotReservedWords, AppSettingsModel.ConnectionStrings).ToList();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private BotReservedWordsEntity getBotReservedWords(int? pageNumber = 0, int? pageSize = 20, int? tenantId = null, string keyFilter = "")
        {
            try
            {
                tenantId ??= AbpSession.TenantId.Value;
                BotReservedWordsEntity model = new BotReservedWordsEntity();
                var SP_Name = Constants.Bot.SP_BotReservedWordsGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@TenantId",tenantId),
                   new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber),
                   new System.Data.SqlClient.SqlParameter("@PageSize",pageSize),
                };
                if (keyFilter != "" && keyFilter != null && keyFilter != "null")
                {
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@Key", keyFilter));
                }
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };

                sqlParameters.Add(OutputParameter);

                model.lstBotReservedWordsModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBotReservedWords, AppSettingsModel.ConnectionStrings).ToList();
                model.TotalCount = Convert.ToInt32(OutputParameter.Value);

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<ActionsModel> getAllActions()
        {
            try
            {
                List<ActionsModel> model = new List<ActionsModel>();
                var SP_Name = Constants.Bot.SP_ActionsGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapActions, AppSettingsModel.ConnectionStrings).ToList();
                

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Check Wallet amount 
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
        private string GetTemplatesCategory (long templatesId)
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
        #endregion

        #region KeyWord public methods
        /// <summary>
        /// For add new key word
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// { { "state", -1 }, { "message", ex.Message } };  >>> Exception
        /// { { "state", 1 }, { "message", actionId} };   >>> was added before
        /// { { "state", 2 }, { "message", "message", model.action + " has been added successfully" } }; 
        /// { { "state", 3 }, { "message", buttonText} };  >>> was Used before
        /// { { "state", 4 }, { "message", "An execution error occurred" } }; >>> When the actionId does not exist within the bot flow table
        /// </returns>
        [HttpPost]
        public Task<Dictionary<string, dynamic>> KeyWordAdd(KeyWordModel model)
        {
            return keyWordAdd(model);
        }
        /// <summary>
        /// For Update key word
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// { { "state", -1 }, { "message", ex.Message } };  >>> Exception
        /// { { "state", 1 }, { "message", actionId} };   >>> was added before
        /// { { "state", 2 }, { "message", "message", model.action + " has been Updated successfully" } }; 
        /// { { "state", 3 }, { "message", buttonText} };  >>> was Used before
        /// { { "state", 4 }, { "message", "An execution error occurred" } }; >>> When the actionId does not exist within the bot flow table
        /// </returns>
        [HttpPut]
        public Dictionary<string, dynamic> KeyWordUpdate(KeyWordModel model)
        {
            return keyWordUpdate(model);
        }
        /// <summary>
        /// get Key Word by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public KeyWordModel KeyWordGetById(long id)
        {
            return keyWordGetById(id);
        }
        /// <summary>
        /// get all key word
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<KeyWordModel> KeyWordGetByAll(int? pageNumber = 0, int? pageSize = 20)
        {
            return keyWordGetByAll(pageNumber, pageSize);
        }

        /// <summary>
        /// get  key word
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public KeyWordModel KeyWordGetByKey(int tenantId, string key)
        {
            var Listkeys= keyWordGetByAll(0, 1000000, tenantId).Items.ToList();


            if (Listkeys.Count()>0) {

                KeyWordModel foundkey = null;
                foreach (var k in Listkeys)
                {
                   

                    switch (k.KeyWordType)
                    {
                        case 1:

                            var patt1 = k.buttonText.Split(",");

                            foreach (var p in patt1)
                            {
                                double similarity = GetSimilarityPercentage(p, key)* 100;

                                switch (k.FuzzyMatch)
                                {
                                    case 0:// 20%
                                        if (similarity>=20)
                                        {

                                            foundkey=k;
                                        }
                                        break;
                                    case 1:// 50%
                                        if (similarity>=50)
                                        {
                                            foundkey=k;

                                        }
                                        break;
                                    case 2:// 80%
                                        if (similarity>=80)
                                        {

                                            foundkey=k;
                                        }
                                        break;
                                }

                                if (foundkey!=null)
                                {

                                    break;
                                }
                            }

                            break;
                     

                        case 2:
                            var patt = k.buttonText.Split(",");

                            foreach(var p in patt)
                            {
                                if (p==key)
                                {
                                    foundkey=k;

                                }

                                if (foundkey!=null)
                                {

                                    break;
                                }
                            }
                          
                            break;

                        case 3:
                            if (key.ToLower().Trim().Contains(k.buttonText.ToLower().Trim()))
                            {
                                foundkey=k;
                            }
                            break;


                    }


                    if (foundkey!=null)
                    {

                        break;
                    }
                  



                }

                return foundkey;

            }


            return null;
        }

        static double GetSimilarityPercentage(string key, string input)
        {
            // Calculate Levenshtein distance as similarity
            double maxLength = Math.Max(key.Length, input.Length);
            double distance = key.LevenshteinDistance(input);

            // Similarity percentage formula
            double similarity = 1.0 - (distance / maxLength);

            return similarity;
        }




        /// <summary>
        /// for delete the key word
        /// </summary>
        /// <param name="id"></param>
        /// <returns> true or false </returns>
        [HttpDelete]
        public bool KeyWordDelete(long id)
        {
            return keyWordDelete(id);
        }
        #endregion


        #region KeyWord private methods
        private async Task<Dictionary<string, dynamic>> keyWordAdd(KeyWordModel model)
        {
            try
            {
                if (model != null)
                {
                    int tenantId = AbpSession.TenantId.Value;
                    var response = new Dictionary<string, dynamic>();

                    var SP_Name = Constants.Bot.Sp_KeyWordGetByTenantId;

                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                    {
                        new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                        new System.Data.SqlClient.SqlParameter("@actionId",model.actionId),
                        new System.Data.SqlClient.SqlParameter("@buttonText",model.buttonText)
                    };
                    var actionIdOutPut = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.BigInt,
                        ParameterName = "@actionIdOutPut",
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(actionIdOutPut);
                    var buttonTextOutPut = new System.Data.SqlClient.SqlParameter
                    {
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "@buttonTextOutPut",
                        Size = 4000, // Specify the size based on your requirements
                        Direction = ParameterDirection.Output
                    };
                    sqlParameters.Add(buttonTextOutPut);

                    //var resultModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapKeyWordModel, AppSettingsModel.ConnectionStrings).ToList();

                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                    var resultActionId = (actionIdOutPut.Value != DBNull.Value) ? Convert.ToInt32(actionIdOutPut.Value) : 0;
                    var resultButtonText = (buttonTextOutPut.Value != DBNull.Value) ? buttonTextOutPut.Value.ToString() : null;

                    if (resultActionId == 0 && resultButtonText == null)
                    {
                        model.tenantId = tenantId;
                        long keyWordId = AddkeyWord(model);
                        if (keyWordId != 0)
                        {
                            response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", model.action } };
                        }
                        else
                        {
                            response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", "An execution error occurred" } };
                        }
                    }
                    else if (resultActionId != 0)
                    {
                        response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", model.action } };
                    }
                    else if (resultButtonText != null)
                    {
                        response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", resultButtonText } };
                    }

                    return response;
                }
                return new Dictionary<string, dynamic>(); 
            }
            catch(Exception ex) 
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        private Dictionary<string, dynamic> keyWordUpdate(KeyWordModel model)
        {
            try
            {
                if (model != null)
                {
                    int tenantId = 0;
                    try
                    {
                         
                        tenantId = AbpSession.TenantId.Value;
                      
                    }
                    catch
                    {
                        tenantId=model.tenantId;

                    }
                 
                    var response = new Dictionary<string, dynamic>();

                    var SP_Name = Constants.Bot.Sp_KeyWordGetByTenantIdInUpdated;
                    var newWords = model.buttonText.Split(',').Select(word => word.Trim()).ToList();
                    string resultButtonText = null;
                    int resultActionId = 0;
                    foreach (var word in newWords)
                    {
                        var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", model.id),
                            new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                            new System.Data.SqlClient.SqlParameter("@actionId", model.actionId),
                            new System.Data.SqlClient.SqlParameter("@buttonText", word)
                        };

                        var actionIdOutPut = new System.Data.SqlClient.SqlParameter
                        {
                            SqlDbType = SqlDbType.BigInt,
                            ParameterName = "@actionIdOutPut",
                            Direction = ParameterDirection.Output
                        };
                        sqlParameters.Add(actionIdOutPut);

                        var buttonTextOutPut = new System.Data.SqlClient.SqlParameter
                        {
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "@buttonTextOutPut",
                            Size = 4000, // Specify the size based on your requirements
                            Direction = ParameterDirection.Output
                        };
                        sqlParameters.Add(buttonTextOutPut);

                        SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                        resultActionId = (actionIdOutPut.Value != DBNull.Value) ? Convert.ToInt32(actionIdOutPut.Value) : 0;
                        resultButtonText = (buttonTextOutPut.Value != DBNull.Value) ? buttonTextOutPut.Value.ToString() : null;
                        if (resultActionId != 0) { break; }
                        if (resultButtonText != null) { break; }
                            
                    }

                    if (resultActionId == 0 && resultButtonText == null) 
                    {
                        model.tenantId = tenantId;
                        long keyWordId = UpdateKeyWord(model);
                        if (keyWordId != 0)
                        {
                            response = new Dictionary<string, dynamic> { { "state", 2 }, { "message", model.action  } };
                        }
                        else
                        {
                            response = new Dictionary<string, dynamic> { { "state", 4 }, { "message", "An execution error occurred" } };
                        }
                    }
                    else if (resultActionId != 0)
                    {
                        response = new Dictionary<string, dynamic> { { "state", 1 }, { "message", model.action } };
                    }
                    else if (resultButtonText != null)
                    {
                        response = new Dictionary<string, dynamic> { { "state", 3 }, { "message", resultButtonText } };
                    }

                    return response;
                }
                return new Dictionary<string, dynamic>();
            }
            catch (Exception ex)
            {
                return new Dictionary<string, dynamic> { { "state", -1 }, { "message", ex.Message } };
            }
        }
        private long AddkeyWord(KeyWordModel model)
        {
            try
            {
                var SP_Name = Constants.Bot.Sp_KeyWordAdd;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",model.tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@action",model.action) ,
                    new System.Data.SqlClient.SqlParameter("@actionId",model.actionId) ,
                    new System.Data.SqlClient.SqlParameter("@triggersBot",model.triggersBot) ,
                    new System.Data.SqlClient.SqlParameter("@triggersBotId",model.triggersBotId) ,
                    new System.Data.SqlClient.SqlParameter("@buttonText",model.buttonText) ,

                    new System.Data.SqlClient.SqlParameter("@KeyUse",model.KeyUse),
                    new System.Data.SqlClient.SqlParameter("@KeyWordType",model.KeyWordType),
                    new System.Data.SqlClient.SqlParameter("@FuzzyMatch",model.FuzzyMatch)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
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
        private long UpdateKeyWord(KeyWordModel model)
        {
            try
            {
                var SP_Name = Constants.Bot.Sp_KeyWordUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",model.id) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",model.tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@action",model.action) ,
                    new System.Data.SqlClient.SqlParameter("@actionId",model.actionId) ,
                    new System.Data.SqlClient.SqlParameter("@triggersBot",model.triggersBot) ,
                    new System.Data.SqlClient.SqlParameter("@triggersBotId",model.triggersBotId) ,
                    new System.Data.SqlClient.SqlParameter("@buttonText",model.buttonText),


                     new System.Data.SqlClient.SqlParameter("@KeyUse",model.KeyUse),
                      new System.Data.SqlClient.SqlParameter("@KeyWordType",model.KeyWordType),
                       new System.Data.SqlClient.SqlParameter("@FuzzyMatch",model.FuzzyMatch)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
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
        private KeyWordModel keyWordGetById(long id)
        {
            try 
            {
                KeyWordModel keyWordModel = new KeyWordModel();

                var SP_Name = Constants.Bot.Sp_KeyWordGetById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@id",id)
                };

                keyWordModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapKeyWordModel, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return keyWordModel;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private PagedResultDto<KeyWordModel> keyWordGetByAll(int? pageNumber = 0, int? pageSize = 20, int? tenantId=null)
        {
            try
            {
                List<KeyWordModel> keyWordModel = new List<KeyWordModel>();

                if (tenantId==null)
                {
                    tenantId = AbpSession.TenantId.Value;

                }
                 

                var SP_Name = Constants.Bot.Sp_KeyWordGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId),
                    new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                keyWordModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapKeyWordModel, AppSettingsModel.ConnectionStrings).ToList();

                int totalCount = (OutputParameter.Value != DBNull.Value) ? Convert.ToInt32(OutputParameter.Value) : 0;

                return new PagedResultDto<KeyWordModel>(totalCount, keyWordModel); 
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
        private bool keyWordDelete(long id)
        {
            try
            {
                var SP_Name = Constants.Bot.Sp_KeyWordDelete;
                int tenantId = AbpSession.TenantId.Value;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@Id",id) ,
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) 
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                long result =(OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
                return (result != 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
